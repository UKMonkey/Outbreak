using System;
using Outbreak.Entities.Properties;
using Outbreak.Net.Messages;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Properties;
using Vortex.Interface.Net;

namespace Outbreak.Server
{
    public class Levelling
    {
        private readonly GameServer _gameServer;
        private readonly LevelExperienceCalculator _levelExperienceCalculator;

        public Levelling(GameServer gameServer, LevelExperienceCalculator levelExperienceCalculator)
        {
            _gameServer = gameServer;
            _levelExperienceCalculator = levelExperienceCalculator;
        }

        public void GiveExperienceToPlayer(RemotePlayer remotePlayer, Entity playerEntity, int amount)
        {
            if (!playerEntity.HasProperty((int)GameEntityPropertyEnum.Experience))
                return;

            var experience = playerEntity.GetProperty((int)GameEntityPropertyEnum.Experience).IntValue;
            var level = playerEntity.GetProperty((int)GameEntityPropertyEnum.Level).IntValue;
            var levelExperience = playerEntity.GetProperty((int)GameEntityPropertyEnum.LevelExperience).IntValue;

            var currentLevel = level;
            var currentExperience = experience;
            var currentLevelExperience = levelExperience;

            var experienceToDistribute = amount;

            while (experienceToDistribute > 0)
            {
                var toNextLevel = currentLevelExperience - currentExperience;

                if (experienceToDistribute >= toNextLevel)
                {
                    experienceToDistribute -= toNextLevel;
                    currentLevel++;
                    currentLevelExperience = _levelExperienceCalculator.GetLevelExperienceRequirement(level);
                    currentExperience = 0;
                }
                else
                {
                    currentExperience += experienceToDistribute;
                    experienceToDistribute = 0;
                }
            }

            if (experience != currentExperience)
            {
                playerEntity.SetProperty(new EntityProperty((int)GameEntityPropertyEnum.Experience, currentExperience));
            }

            if (levelExperience != currentLevelExperience)
            {
                playerEntity.SetProperty(new EntityProperty((int)GameEntityPropertyEnum.LevelExperience,
                                                            currentLevelExperience));
            }

            if (level != currentLevel)
            {
                playerEntity.SetProperty(new EntityProperty((int)GameEntityPropertyEnum.Level, currentLevel));
            }

            var xpMessage = new ServerPlayerXPIncrease { Amount = amount };
            _gameServer.Engine.SendMessageToClient(xpMessage, remotePlayer);

            if (currentLevel != level)
            {
                PlayerLevelUp(remotePlayer, currentLevel);
            }
        }

        public void PlayerLevelUp(RemotePlayer remotePlayer, int newLevel)
        {
            var levelUpMessage = new ServerPlayerLevelUpMessage
            {
                RemotePlayer = remotePlayer,
                NewLevel = newLevel
            };
            _gameServer.Engine.SendMessage(levelUpMessage);

            var entity = _gameServer.GetEntityForRemotePlayer(remotePlayer);

            var maxHealth = entity.GetMaxHealth();
            var newMaxHealth = GetNewMaxHealth(newLevel);
            var healthDifference = newMaxHealth - maxHealth;

            entity.SetMaxHealth(newMaxHealth);
            entity.SetHealth(entity.GetHealth() + healthDifference);
        }

        private static int GetNewMaxHealth(int newLevel)
        {
            return (int)Math.Round(Consts.PlayerStartHealth * Math.Pow(1.2, newLevel));
        }
    }
}