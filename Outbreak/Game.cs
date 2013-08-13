using System;
using System.Collections.Generic;
using Outbreak.Items.Containers;
using Outbreak.Resources;
using SlimMath;
using Vortex.Interface;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Properties;
using Vortex.Interface.Net;
using Outbreak.Entities;
using Outbreak.Entities.Properties;
using Outbreak.Items.Containers.InventorySpecs;
using Psy.Core;
using Vortex.Interface.Traits;

namespace Outbreak
{
    public abstract class Game : IGame
    {
        public string Version { get { return "3"; } }
        public IEngine Engine { get; private set; }
        public IEntityFactory EntityFactory { get { return Engine.EntityFactory; } }

        private ModelEquipper _modelEquipper;

        public GameTime GameTime;
        public string GameName { get; protected set; }

        public abstract void OnBegin();
        public abstract void OnNetworkStatusChanged(NetworkStatus networkStatus);
        public abstract void UpdateWorld();

        protected void OnAttach(IEngine engine)
        {
            Engine = engine;
            RegisterStaticFloatingItemCache();
            RegisterStaticItemSpecCache();
            RegisterStaticInventoryCache(StaticItemSpecCache.Instance);

            RegisterEntityTypes();
            RegisterEntityBehaviours();

            RegisterSounds();

            _modelEquipper = new ModelEquipper(StaticItemSpecCache.Instance, Engine.CompiledModelCache);

            Engine.OnEntitiesGenerated += RegisterEntityCleanup;
            Engine.OnEntitiesGenerated += EngineOnOnEntitiesGenerated;
        }

        private void EngineOnOnEntitiesGenerated(List<Entity> updated)
        {
            foreach (var entity in updated)
            {
                _modelEquipper.Equip(entity);
                entity.OnPropertyChanged += EntityPropertyChanged;
            }
        }

        private void EntityPropertyChanged(Entity entity, Trait prop)
        {
            if (prop.PropertyId == (short)GameEntityPropertyEnum.PrimaryWeaponItem)
            {
                _modelEquipper.EquipPrimary(entity);
            }
            else if (prop.PropertyId == (short)GameEntityPropertyEnum.SecondaryWeaponItem)
            {
                _modelEquipper.EquipSecondary(entity);
            }
            else if (prop.PropertyId == (short)GameEntityPropertyEnum.HeadSlotItem)
            {
                _modelEquipper.EquipHeadSlotItem(entity);
            }
        }

        private void RegisterEntityCleanup(List<Entity> entities)
        {
            foreach (var item in entities)
                item.OnDeath += PerformCleanup;
        }

        // TODO - go back and make sure this works
        protected virtual void PerformCleanup(Entity item)
        {
//            if (item.HasInventory())
//                StaticInventoryCache.Instance.RemoveInventory(item.GetInventoryId());
//            if (item.HasFloatingItemId())
//                StaticFloatingItemCache.Instance.RemoveFloatingItem(item.GetFloatingItemId());
        }

        protected virtual void RegisterEntityBehaviours()
        {
        }

        protected virtual void RegisterEntityTypes()
        {
            EntityFactory.
                Add(EntityTypeEnum.Player, "Player").
                    RegisterDefaultProperty(EntityPropertyEnum.NameplateColour, new Color4(1.0f, 0.2f, 0.9f, 0.1f)).
                    RegisterDefaultProperty(EntityPropertyEnum.Static, false).
                    RegisterDefaultProperty(EntityPropertyEnum.Solid, true).
                    RegisterDefaultProperty(EntityPropertyEnum.MaxHealth, Consts.PlayerStartHealth).
                    RegisterDefaultProperty(EntityPropertyEnum.Health, Consts.PlayerStartHealth).
                    RegisterDefaultProperty(EntityPropertyEnum.MovementVector, new Vector3(0, 0, 0)).
                    RegisterDefaultProperty(EntityPropertyEnum.ViewRange, (float)20).
                    RegisterDefaultProperty(EntityPropertyEnum.MeleeViewRange, (float)2).
                    RegisterDefaultProperty(EntityPropertyEnum.ViewAngleRange, (float)Math.PI / 4).

                    RegisterDefaultProperty(GameEntityPropertyEnum.IsGod, false).
                    RegisterDefaultProperty(GameEntityPropertyEnum.IsHuman, true).
                    RegisterDefaultProperty(GameEntityPropertyEnum.WalkSpeed, 0.05f).
                    RegisterDefaultProperty(GameEntityPropertyEnum.RunSpeed, 0.1f).
                    RegisterDefaultProperty(GameEntityPropertyEnum.KeyboardMovementDir, (byte)Direction.None).
                    RegisterDefaultProperty(GameEntityPropertyEnum.IsRunning, true).

                    RegisterDefaultProperty(GameEntityPropertyEnum.Experience, 0).
                    RegisterDefaultProperty(GameEntityPropertyEnum.Level, 1).
                    RegisterDefaultProperty(GameEntityPropertyEnum.LevelExperience, 0).
                    RegisterDefaultProperty(GameEntityPropertyEnum.Hunger, 0).
                    RegisterDefaultProperty(GameEntityPropertyEnum.Mood, 0).

                    RegisterDefaultProperty(GameEntityPropertyEnum.EquippedItem, (byte)InventorySpecialSlotEnum.PrimaryWeapon).
                    RegisterDefaultProperty(EntityPropertyEnum.Model, Models.Player01).


                Add(EntityTypeEnum.InventoryItem, "InventoryItem").
                    RegisterDefaultProperty(EntityPropertyEnum.Static, true).
                    RegisterDefaultProperty(EntityPropertyEnum.Solid, false).
                    RegisterDefaultProperty(GameEntityPropertyEnum.DestroyOnCollected, true).
                    RegisterDefaultProperty(EntityPropertyEnum.Model, Models.HealthPack01).

                Add(EntityTypeEnum.Zombie, "Zombie").
                    RegisterDefaultProperty(EntityPropertyEnum.MaxHealth, Consts.ZombieHealth).
                    RegisterDefaultProperty(EntityPropertyEnum.Health, Consts.ZombieHealth).
                    RegisterDefaultProperty(EntityPropertyEnum.Static, false).
                    RegisterDefaultProperty(EntityPropertyEnum.Solid, true).
                    RegisterDefaultProperty(EntityPropertyEnum.MovementVector, new Vector3(0, 0, 0)).
                    RegisterDefaultProperty(EntityPropertyEnum.ViewRange, (float)20).
                    RegisterDefaultProperty(EntityPropertyEnum.ViewAngleRange, (float)Math.PI / 2).
                    RegisterDefaultProperty(GameEntityPropertyEnum.IsZombie, true).
                    RegisterDefaultProperty(GameEntityPropertyEnum.ChaseMinTime, (long)1500).
                    RegisterDefaultProperty(GameEntityPropertyEnum.IsRunning, false).
                    RegisterDefaultProperty(GameEntityPropertyEnum.WalkSpeed, 0.005f).
                    RegisterDefaultProperty(GameEntityPropertyEnum.RunSpeed, 0.02f).
                    RegisterDefaultProperty(EntityPropertyEnum.Model, Models.Zombie01).
                    RegisterDefaultServerProperty(GameEntityPropertyEnum.ChaseThinkCount, 0).
                    RegisterDefaultProperty(GameEntityPropertyEnum.EquippedItem, (byte)InventorySpecialSlotEnum.PrimaryWeapon).

                Add(EntityTypeEnum.BasicWall, "BasicWall").
                    RegisterDefaultProperty(EntityPropertyEnum.Static, true).
                    RegisterDefaultProperty(EntityPropertyEnum.Solid, true).
                    RegisterDefaultProperty(EntityPropertyEnum.Model, Models.Wall01).

                Add(EntityTypeEnum.BasicDoor, "BasicDoor").
                    RegisterDefaultProperty(EntityPropertyEnum.Static, true).
                    RegisterDefaultProperty(EntityPropertyEnum.Solid, true).
                    RegisterDefaultProperty(EntityPropertyEnum.MaxHealth, Consts.DoorHealth).
                    RegisterDefaultProperty(EntityPropertyEnum.Health, Consts.DoorHealth).
                    RegisterDefaultProperty(EntityPropertyEnum.Model, Models.Door01).

                Add(EntityTypeEnum.SmallStash, "SmallStash").
                    RegisterDefaultProperty(EntityPropertyEnum.Static, true).
                    RegisterDefaultProperty(EntityPropertyEnum.Model, Models.AmmoCrate01).
                    RegisterDefaultProperty(EntityPropertyEnum.Nameplate, "Small Stash").
                    RegisterDefaultProperty(EntityPropertyEnum.NameplateColour, Colours.LightBlue).

                Add(EntityTypeEnum.ZombieCorpse, "ZombieCorpse").
                    RegisterDefaultProperty(EntityPropertyEnum.Model, Models.AmmoCrate01).
                    RegisterDefaultProperty(EntityPropertyEnum.Static, true).

                Add(EntityTypeEnum.Chair, "Chair").
                    RegisterDefaultProperty(EntityPropertyEnum.Model, Models.Chair01).
                    RegisterDefaultProperty(EntityPropertyEnum.MaxHealth, Consts.ChairHealth).
                    RegisterDefaultProperty(EntityPropertyEnum.Health, Consts.ChairHealth).
                    RegisterDefaultProperty(EntityPropertyEnum.Static, true).
                    RegisterDefaultProperty(EntityPropertyEnum.Solid, true).

                Add(EntityTypeEnum.ShopShelf, "ShopShelf").
                    RegisterDefaultProperty(EntityPropertyEnum.Model, Models.EmptyShelf01).
                    RegisterDefaultProperty(EntityPropertyEnum.Static, true).
                    RegisterDefaultProperty(EntityPropertyEnum.Solid, true);
        }

        protected abstract void RegisterStaticItemSpecCache();
        protected abstract void RegisterStaticFloatingItemCache();
        protected abstract void RegisterStaticInventoryCache(IItemSpecCache cache);

        private void RegisterSounds()
        {
            var fields = typeof(Sound).GetFields();
            foreach (var field in fields)
            {
                var fieldValue = field.GetRawConstantValue().ToString();
                Engine.AudioLookup.RegisterSound(fieldValue);
            }
        }
    }
}
