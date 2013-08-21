using System;
using System.Linq;
using Outbreak.Entities.Properties;
using Outbreak.Items;
using Outbreak.Items.ItemGenerators;
using Outbreak.Resources;
using Outbreak.Server.Entities.Behaviours.OnCollision;
using Outbreak.Server.Entities.Behaviours.OnHearWeapon;
using Outbreak.Server.Entities.Behaviours.OnInteract;
using Outbreak.Server.Entities.Behaviours.OnInventoryChange;
using Outbreak.Server.Entities.Behaviours.OnKilled;
using Outbreak.Server.Entities.Behaviours.OnPlayerChange;
using Outbreak.Server.Entities.Behaviours.OnSpawn;
using Outbreak.Server.Entities.Behaviours.OnTakeDamage;
using Outbreak.Server.Entities.Behaviours.OnThinking;
using Outbreak.Server.Entities.Damage;
using Outbreak.Server.Entities.TraitEvents;
using Outbreak.Server.Items;
using Outbreak.Server.Items.Users;
using Outbreak.Server.Persistance.File.Chunks;
using Outbreak.Server.Persistance.File.FloatingItems;
using Outbreak.Server.Persistance.File.Inventories;
using Outbreak.Server.Persistance.File.ItemSpecs;
using Outbreak.Server.WeaponHandler.Melee;
using Outbreak.Server.WeaponHandler.Ranged;
using Outbreak.Server.World.ItemGenerators;
using Outbreak.Server.World.ItemGenerators.Equipment;
using Outbreak.Server.World.ItemGenerators.Materials;
using Outbreak.Server.World.ItemGenerators.Weapons;
using Outbreak.Server.World.ItemGenerators.Weapons.Melee;
using Outbreak.Server.World.ItemGenerators.Weapons.Ranged;
using SlimMath;
using Vortex.Interface;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;
using Vortex.Interface.EntityBase.Properties;
using Vortex.Interface.Net;
using Vortex.Interface.World.Wrapper;
using Outbreak.Entities;
using Outbreak.Items.Containers;
using Outbreak.Items.Containers.FloatingItems;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Net.Messages;
using Outbreak.Server.Items.Containers;
using Outbreak.Server.WeaponHandler;
using Outbreak.Server.World.Providers;
using Outbreak.Server.World.Providers.Biome;
using Psy.Core;
using Psy.Core.Configuration;
using Psy.Core.Tasks;
using EntityHandler = Outbreak.Server.Persistance.File.Entites.EntityHandler;
using EntityTypeEnum = Outbreak.Entities.EntityTypeEnum;
using GameEntityBehaviourEnum = Outbreak.Entities.Behaviours.GameEntityBehaviourEnum;
using System.Collections.Generic;
using Vortex.Interface.World.Blocks;

namespace Outbreak.Server
{
    public class GameServer : Game, IGameServer
    {
        private const bool UseFileDataStore = false;

        public new IServer Engine { get; private set; }

        private EntityHandler _entityHandler;
        //private StartArguments _arguments;
        private readonly RespawnTimerCollection _respawnTimer;
        public WeaponHandlerFactory WeaponHandlers;
        private WeatherController _weatherController;
        private MessageHandler _serverMessageHandler;
        private LevelNameplateUpdater _playerLevelNameplateUpdater;
        public ItemGeneratorDictionary ItemGeneratorDictionary;
        private IInventoryCache _inventoryCache;
        public readonly LevelExperienceCalculator LevelExperienceCalculator;
        public InventoryItemUser ItemUserManager;

        public GameServer(StartArguments arguments)
        {
            //_arguments = arguments;
            _respawnTimer = new RespawnTimerCollection(this);
            LevelExperienceCalculator = new LevelExperienceCalculator();
            GameTime = new GameTime();
            GameName = "Outbreak";  //TODO make this name read in from some file that also has the seed
        }

        public void OnAttach(IServer engine)
        {
            Engine = engine;
            _weatherController = new WeatherController();

            base.OnAttach(engine);
            StaticTaskQueue.TaskQueue.CreateRepeatingTaskWithDelay("GameServer._respawnTimer", _respawnTimer.Update,
                                                                   1000, 1000);
            RegisterWeaponHandlers();

            _playerLevelNameplateUpdater = new LevelNameplateUpdater(engine);
            ItemUserManager = new InventoryItemUser(engine);
        }

        protected override void PerformCleanup(Entity item)
        {
            base.PerformCleanup(item);
            ItemUserManager.CancelUsage(item);
        }

        private void RegisterWeaponHandlers()
        {
            WeaponHandlers = new WeaponHandlerFactory();
            WeaponHandlers.AddHandler(new SemiAutomaticHandler(this));
            WeaponHandlers.AddHandler(new ShotgunHandler(this));
            WeaponHandlers.AddHandler(new AutomaticHandler(this));
            WeaponHandlers.AddHandler(new FrontArchRepeated(this));
            WeaponHandlers.AddHandler(new SingleTargetRepeated(this));
        }

        protected override void RegisterStaticItemSpecCache()
        {
            ItemSpecHandler handler = null;
            if (UseFileDataStore)
            {
                handler = new ItemSpecHandler(this);
                handler.Init();
            }

            StaticItemSpecCache.Instance = new ItemSpecCache(Engine, handler);

            if (handler != null)
                StaticItemSpecCache.Instance.OnItemAdded += handler.SaveItemSpec;
        }

        protected override void RegisterStaticFloatingItemCache()
        {
            StaticFloatingItemCache.Instance = new FloatingItemCache();

            if (UseFileDataStore)
            {
                var cacheHandler = new FloatingItemHandler(this);

                StaticFloatingItemCache.Instance.OnAdded += cacheHandler.SaveItem;
                StaticFloatingItemCache.Instance.OnRemoved += cacheHandler.DeleteItem;

                cacheHandler.Init();

                cacheHandler.LoadItems(StaticFloatingItemCache.Instance);
            }
        }

        protected override void RegisterStaticInventoryCache(IItemSpecCache cache)
        {
            InventoryHandler persister = null;
            if (UseFileDataStore)
            {
                persister = new InventoryHandler(this);
                persister.Init();
            }

            StaticInventoryCache.Instance = new InventoryCache(Engine, persister, persister);
            _inventoryCache = StaticInventoryCache.Instance;
        }

        protected override void RegisterEntityTypes()
        {
            base.RegisterEntityTypes();
            Engine.RegisterRequirement((short)EntityTypeEnum.Zombie, SpawnRequirements.HumanDistanceRequirement);
            Engine.RegisterRequirement((short)EntityTypeEnum.Zombie, SpawnRequirements.ZombieLightRequirement);
            Engine.RegisterRequirement((short)EntityTypeEnum.Zombie, (a,b,c,d,e) =>
                    SpawnRequirements.DistanceToOthersRequirement(4,a,b,c,d,e));
        }

        public override void OnBegin()
        {
            _serverMessageHandler = new MessageHandler(this, _inventoryCache);
            _serverMessageHandler.RegisterMessageCallbacks();

            Engine.RegisterMessageCallback(typeof(ClientRequestEntityControlMessage), AnnounceClientEntitiesControlSituationToClient);

            ServerSetup();
        }

        protected override void RegisterEntityBehaviours()
        {
            base.RegisterEntityBehaviours();
            EntityFactory.
                RegisterBehaviour(EntityTypeEnum.InventoryItem, EntityBehaviourEnum.OnInteract, new AddToInventory()).

                RegisterBehaviour(EntityTypeEnum.BasicDoor, EntityBehaviourEnum.OnInteract, new StartDoorRotate(0.03f)).
                RegisterBehaviour(EntityTypeEnum.BasicDoor, EntityBehaviourEnum.OnKilled, new SpawnItems(this, ItemTypeEnum.WoodPlank, 2, 4)).
                RegisterDefaultDamageHandler(EntityTypeEnum.BasicDoor, new WoodenDamageHandler()).

                RegisterBehaviour(EntityTypeEnum.Chair, EntityBehaviourEnum.OnKilled, new SpawnItems(this, ItemTypeEnum.WoodPlank, 1, 2)).
                RegisterDefaultDamageHandler(EntityTypeEnum.Chair, new WoodenDamageHandler()).

                RegisterBehaviour(EntityTypeEnum.ZombieCorpse, EntityBehaviourEnum.OnInteract, new OpenInventory(this)).

                RegisterBehaviour(EntityTypeEnum.SmallStash, EntityBehaviourEnum.OnInteract, new OpenInventory(this)).
                RegisterBehaviour(EntityTypeEnum.SmallStash, EntityBehaviourEnum.OnSpawn, new FillStash(this, StashSize.Small)).

                RegisterBehaviour(EntityTypeEnum.Player, EntityBehaviourEnum.PlayerChanged, new UpdateNameplate(Engine)).
                RegisterBehaviour(EntityTypeEnum.Player, EntityBehaviourEnum.OnSpawn, new CreateInventory(Consts.PlayerBackpackSize, true)).
                RegisterBehaviour(EntityTypeEnum.Player, EntityBehaviourEnum.OnSpawn, new AddItemsToInventory(this, new[] { ItemTypeEnum.CricketBat })).
                RegisterBehaviour(EntityTypeEnum.Player, EntityBehaviourEnum.OnSpawn, new AddItemsToInventory(this, new[] { ItemTypeEnum.Food })).
                RegisterBehaviour(EntityTypeEnum.Player, EntityBehaviourEnum.OnSpawn, new AddItemsToInventory(this, new[] { ItemTypeEnum.FirstAidPack })).
                RegisterBehaviour(EntityTypeEnum.Player, EntityBehaviourEnum.OnSpawn, new AddItemsToInventory(this, new[] { ItemTypeEnum.HeadItem })).
                RegisterBehaviour(EntityTypeEnum.Player, EntityBehaviourEnum.Think, new PerformReload()).
                RegisterBehaviour(EntityTypeEnum.Player, EntityBehaviourEnum.Think, new GetHungry()).
                RegisterBehaviour(EntityTypeEnum.Player, EntityBehaviourEnum.OnKilled, new DropInventory(Engine)).
                RegisterBehaviour(EntityTypeEnum.Player, GameEntityBehaviourEnum.OnPrimaryWeaponChange, new UpdateVisibleItems(InventorySpecialSlotEnum.PrimaryWeapon, GameEntityPropertyEnum.PrimaryWeaponItem)).
                RegisterBehaviour(EntityTypeEnum.Player, GameEntityBehaviourEnum.OnSecondaryWeaponChange, new UpdateVisibleItems(InventorySpecialSlotEnum.SecondaryWeapon, GameEntityPropertyEnum.SecondaryWeaponItem)).
                RegisterBehaviour(EntityTypeEnum.Player, GameEntityBehaviourEnum.OnHeadSlotChange, new UpdateVisibleItems(InventorySpecialSlotEnum.HeadArmour, GameEntityPropertyEnum.HeadSlotItem)).
                RegisterDefaultDamageHandler(EntityTypeEnum.Player, new BasicDamageHandler()).

                RegisterBehaviour(EntityTypeEnum.ShopShelf, EntityBehaviourEnum.OnSpawn, new PopulateShelf(this)).
                RegisterBehaviour(EntityTypeEnum.ShopShelf, EntityBehaviourEnum.OnInteract, new OpenInventory(this)).
                RegisterBehaviour(EntityTypeEnum.ShopShelf, GameEntityBehaviourEnum.OnInventoryChange, new DeleteInventoryIfEmpty()).
                RegisterBehaviour(EntityTypeEnum.ShopShelf, GameEntityBehaviourEnum.OnInventoryChange, new ChangeModelIfEmpty(Models.EmptyShelf01)).

                RegisterBehaviour(EntityTypeEnum.Zombie, EntityBehaviourEnum.OnCollisionWithEntity, new TurnRandomly(Engine)).
                RegisterBehaviour(EntityTypeEnum.Zombie, EntityBehaviourEnum.OnCollisionWithTerrain, new TurnRandomly(Engine)).
                RegisterBehaviour(EntityTypeEnum.Zombie, EntityBehaviourEnum.OnKilled, new GiveExperience(this, 100)).
                RegisterBehaviour(EntityTypeEnum.Zombie, EntityBehaviourEnum.Think, new ChaseHumans(Engine)).
                RegisterBehaviour(EntityTypeEnum.Zombie, EntityBehaviourEnum.Think, new MakeZombieNoise(Engine)).
                RegisterBehaviour(EntityTypeEnum.Zombie, EntityBehaviourEnum.Think, new AttackChaseTarget(this)).
                RegisterBehaviour(EntityTypeEnum.Zombie, GameEntityBehaviourEnum.OnTakeDamage, new GetAngry(this)).
                RegisterBehaviour(EntityTypeEnum.Zombie, GameEntityBehaviourEnum.OnHearWeaponNoise, new TurnToFace()).
                RegisterBehaviour(EntityTypeEnum.Zombie, EntityBehaviourEnum.OnKilled, new SpawnLootableCorpse(this)).
                RegisterBehaviour(EntityTypeEnum.Zombie, EntityBehaviourEnum.OnSpawn, new CreateInventory(Consts.ZombieBackpackSize, false)).
                RegisterBehaviour(EntityTypeEnum.Zombie, EntityBehaviourEnum.OnSpawn, new AddItemsToInventory(this, ItemTypeEnumConsts.ZombieWeapons)).
                RegisterDefaultDamageHandler(EntityTypeEnum.Zombie, new BasicDamageHandler());
        }

        private void ServerSetup()
        {
            var serverConsoleCommands = new ConsoleCommands(this);
            serverConsoleCommands.BindConsoleCommands();

            Engine.ConsoleText("This is server.");
            Engine.Listen(StaticConfigurationManager.ConfigurationManager.GetInt("Net.Port"));
            Engine.MaterialCache.LoadMaterials("materials.xml");
            Engine.LoadMap();
        }

        private static void RegisterItemGenerators(ItemGeneratorDictionary itemGeneratorDictionary)
        {
            itemGeneratorDictionary.Register(ItemTypeEnum.HeadItem, HeadGenerator.GetGenerator());
            itemGeneratorDictionary.Register(ItemTypeEnum.Pistol, PistolGenerator.GetGenerator());
            itemGeneratorDictionary.Register(ItemTypeEnum.PistolAmmo, PistolAmmoGenerator.GetGenerator());
            itemGeneratorDictionary.Register(ItemTypeEnum.Uzi, UziGenerator.GetGenerator());
            itemGeneratorDictionary.Register(ItemTypeEnum.Shotgun, ShotgunGenerator.GetGenerator());
            itemGeneratorDictionary.Register(ItemTypeEnum.ShotgunAmmo, ShotgunAmmoGenerator.GetGenerator());
            itemGeneratorDictionary.Register(ItemTypeEnum.FirstAidPack, FirstAidGenerator.GetGenerator());
            itemGeneratorDictionary.Register(ItemTypeEnum.CricketBat, CricketBatGenerator.GetGenerator());
            itemGeneratorDictionary.Register(ItemTypeEnum.ZombieMaul, new ZombieMaulGenerator());
            itemGeneratorDictionary.Register(ItemTypeEnum.Food, new FoodGenerator());
            itemGeneratorDictionary.Register(ItemTypeEnum.WoodPlank, WoodPlankGenerator.GetGenerator());
        }

        public WorldDataHandler GetWorldHandling()
        {
            var ret = new WorldDataHandler();

            ItemGeneratorDictionary = new ItemGeneratorDictionary();
            RegisterItemGenerators(ItemGeneratorDictionary);

            var generator = new TranslatorBiomeWorldProvider(1337, this, ItemGeneratorDictionary);
            var provider = new SerialWorldProvider();

            if (UseFileDataStore)
            {
                var entityHandler = new EntityHandler(this, Engine, _inventoryCache);
                var chunkHandler = new ChunkHandler(this);
                //var triggerHandler = new TriggerHandler(this);

                chunkHandler.Init();
                entityHandler.Init();

                provider.AddChunkLoader(chunkHandler);
                provider.AddEntityLoader(entityHandler);
                //provider.AddTriggerLoader(triggerHandler);

                ret.WorldSaver = new WorldSaverWrapper(entityHandler, chunkHandler, null);
                _entityHandler = entityHandler;
            }
            else
            {
                ret.WorldSaver = new WorldSaverWrapper();
            }

            provider.AddChunkLoader(generator);
            provider.AddEntityLoader(generator);
            provider.AddTriggerLoader(generator);
            ret.WorldProvider = provider;

            return ret;
        }

        public override void OnNetworkStatusChanged(NetworkStatus networkStatus)
        {
            Engine.ConsoleText("Network state changed to " + networkStatus);
        }

        public override void UpdateWorld()
        {
            WeaponHandlers.UpdateHandlers();
            GameTime.Tick();
            _weatherController.Tick();
            Engine.OutsideLightingColour = OutsideLightingCalculator.GetOutsideLighting(GameTime);
            Engine.IsRaining = _weatherController.IsRaining;
            ItemUserManager.Update();
        }

        /// <summary>
        /// Server method - called when a client connects
        /// </summary>
        /// <param name="player"></param>
        public void OnClientConnected(RemotePlayer player)
        {
            Engine.ConsoleText(
                String.Format("{0} has connected", player.PlayerName),
                new Color4(1.0f, 0.0f, 1.0f, 0.0f));

            // tell the client which map to load.
            var msg = new ServerLoadMapMessage
                          {
                              ClientId = player.ClientId,
                              GameTimeHour = (byte)GameTime.Hour,
                              GameTimeMinute = (byte)GameTime.Minute
                          };
            Engine.SendMessageToClient(msg, player);

            var msg2 = new ServerGameNameMessage
                           {
                               Name = GameName
                           };
            Engine.SendMessageToClient(msg2, player);

            SpawnPlayer(player);
        }

        private void OnPlayerDead(Entity playerChar)
        {
            // client game can believe that if the entity that it was associated with
            // is destroyed - then it's dead
            Engine.StopTrackingEntity(playerChar);

            var player = playerChar.GetPlayer(Engine);
            if (player == null)
                return;

            _respawnTimer.RespawnPlayer(player, 2);
            Engine.ConsoleText("Respawning player in 2 seconds...");
        }

        private Vector2 GetPlayerSpawnPosition(RemotePlayer player)
        {
            return new Rectangle(new Vector2(60, 221), new Vector2(92, 200)).RandomPointInside();
        }

        private void RemoveEntityIdFromPlayer(Entity entity)
        {
            var player = entity.GetPlayer(Engine);
            if (player == null)
                return;

            player.EntityId = null;
        }

        public Entity SpawnPlayer(RemotePlayer player)
        {
            if (!Engine.ConnectedClients.Contains(player))
            {
                // client is no longer connected
                Engine.ConsoleText("Attempted to respawn player but they are no longer connected");
                return null;
            }

            Entity entity = null;
            if (_entityHandler != null)
                entity = _entityHandler.LoadPlayerEntity(player.PlayerName);

            if (entity == null)
            {
                var spawnPosition = GetPlayerSpawnPosition(player);
                entity = Engine.EntityFactory.Get(EntityTypeEnum.Player);

                entity.SetPosition(spawnPosition.AsVector3());
                entity.SetMaxHealth(Consts.PlayerStartHealth);
                entity.SetHealth(Consts.PlayerStartHealth);
                entity.SetProperty(
                    new EntityProperty(
                        (int)GameEntityPropertyEnum.LevelExperience,
                        LevelExperienceCalculator.GetLevelExperienceRequirement(1)));

                Engine.SpawnEntity(entity);
            }

            entity.SetPlayer(player);
            entity.OnDeath += RemoveEntityIdFromPlayer;
            entity.OnDeath += OnPlayerDead;
            entity.OnPropertyChanged += _playerLevelNameplateUpdater.UpdateNameplate;

            player.EntityId = entity.EntityId;

            entity.PlayerChanged();
            Engine.TrackEntity(entity);
            
            _playerLevelNameplateUpdater.UpdateNameplate(entity);

            Engine.SetClientFocus(player, entity);
            AnnounceClientEntityControlSituationToClients(player, entity);

            return entity;
        }

        /// <summary>
        /// When a new player joins we need to inform all the other clients as to
        /// which entity they control.
        /// </summary>
        private void AnnounceClientEntityControlSituationToClients(RemotePlayer player, Entity entity)
        {
            var msg = new ServerClientEntityControlMessage 
                        {
                            EntityId = entity.EntityId, 
                            ClientId = player.ClientId
                        };

            Engine.SendMessage(msg);
        }

        /// <summary>
        /// Server - Inform a client who controls which entities.
        /// </summary>
        /// <param name="msg"></param>
        private void AnnounceClientEntitiesControlSituationToClient(Message msg)
        {
            var message = (ClientRequestEntityControlMessage) msg;
            var player = message.Sender;

            foreach (var playerControlledEntity in Engine.Entities.Where(item => item.GetPlayerId() != null))
            {
                var playerId = playerControlledEntity.GetPlayerId();
                if (playerId == null)
                    continue;
                
                var outMsg = new ServerClientEntityControlMessage
                              {
                                  EntityId = playerControlledEntity.EntityId,
                                  ClientId = playerId.Value
                              };

                Engine.SendMessageToClient(outMsg, player);
            }
        }

        /// <summary>
        /// Server - called when a client disconnects
        /// </summary>
        /// <param name="player"></param>
        public void OnClientDisconnected(RemotePlayer player)
        {
            Engine.ConsoleText(
                String.Format("{0} has disconnected", player.PlayerName),
                new Color4(1.0f, 1.0f, 0.0f, 0.0f));

            var entityToKill = Engine.Entities.FirstOrDefault(e => e.GetPlayerId() == player.ClientId);
            if (entityToKill == null)
                return;

            Engine.StopTrackingEntity(entityToKill);
            entityToKill.Destroy();
        }

        public Entity GetEntityForRemotePlayer(RemotePlayer remotePlayer)
        {
            return Engine.GetEntity(remotePlayer);
        }

        public IEnumerable<BlockProperties> GetBlockTypes()
        {
            yield break;
        }

        public short GetChunkWorldSize()
        {
            return 16;
        }
    }
}
