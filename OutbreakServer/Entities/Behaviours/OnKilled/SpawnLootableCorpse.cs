using Outbreak.Entities.Properties;
using Outbreak.Items;
using Outbreak.Items.Containers;
using Psy.Core;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;
using Vortex.Interface.EntityBase.Properties;
using EntityTypeEnum = Outbreak.Entities.EntityTypeEnum;

namespace Outbreak.Server.Entities.Behaviours.OnKilled
{
    public class SpawnLootableCorpse : IEntityBehaviour
    {
        private readonly GameServer _gameServer;

        public SpawnLootableCorpse(GameServer gameServer)
        {
            _gameServer = gameServer;
        }

        public void PerformBehaviour(Entity target, Entity instigator)
        {
            var position = target.GetPosition();

            var entity = _gameServer.EntityFactory.Get((int) EntityTypeEnum.ZombieCorpse);
            entity.SetPosition(position);
            entity.SetNameplate("Corpse");
            entity.SetNameplateColour(Colours.Red);
            entity.SetRotation(StaticRng.Random.NextFloat(0, 6.2f));

            var inventory = StaticInventoryCache.Instance.CreateNewInventory(true);
            inventory.Initialise((int)StashSize.Small, InventoryType.Corpse);

            if (StaticRng.Random.NextDouble(0, 100) < 20)
            {
                var food = _gameServer.ItemGeneratorDictionary[ItemTypeEnum.Food].Generate();
                inventory.AddItem(food, true);
            }
            if (StaticRng.Random.NextDouble(0, 100) < 20)
            {
                var ammo = _gameServer.ItemGeneratorDictionary[ItemTypeEnum.PistolAmmo].Generate();
                inventory.AddItem(ammo, true);
            }
            if (StaticRng.Random.NextDouble(0, 100) < 20)
            {
                var gun = _gameServer.ItemGeneratorDictionary[ItemTypeEnum.Pistol].Generate();
                inventory.AddItem(gun, true);
            }

            entity.SetProperty(new EntityProperty((int)GameEntityPropertyEnum.InventoryId, inventory.Id));

            _gameServer.Engine.SpawnEntity(entity);
        }
    }
}