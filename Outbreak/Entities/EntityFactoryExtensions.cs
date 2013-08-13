using Outbreak.Entities.Behaviours;
using Vortex.Interface;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;
using Vortex.Interface.EntityBase.Damage;
using Vortex.Interface.EntityBase.Properties;
using Outbreak.Entities.Properties;

namespace Outbreak.Entities
{
    public static class EntityFactoryExtensions
    {
        public static Entity Get(this IEntityFactory factory, EntityTypeEnum type)
        {
            return factory.Get((short) type);
        }

        public static IEntityFactory Add(this IEntityFactory factory, EntityTypeEnum id, string name)
        {
            return factory.Add((short) id, name);
        }

        public static IEntityFactory RegisterDefaultProperty(this IEntityFactory factory, GameEntityPropertyEnum type, int value)
        {
            return factory.RegisterDefaultProperty(new EntityProperty((short) type, value));
        }

        public static IEntityFactory RegisterDefaultProperty(this IEntityFactory factory, GameEntityPropertyEnum type, long value)
        {
            return factory.RegisterDefaultProperty(new EntityProperty((short)type, value));
        }

        public static IEntityFactory RegisterDefaultProperty(this IEntityFactory factory, GameEntityPropertyEnum type, bool value)
        {
            return factory.RegisterDefaultProperty(new EntityProperty((short)type, value));
        }

        public static IEntityFactory RegisterDefaultProperty(this IEntityFactory factory, GameEntityPropertyEnum type, byte value)
        {
            return factory.RegisterDefaultProperty(new EntityProperty((short)type, value));
        }

        public static IEntityFactory RegisterDefaultProperty(this IEntityFactory factory, GameEntityPropertyEnum type, float value)
        {
            return factory.RegisterDefaultProperty(new EntityProperty((short)type, value));
        }



        public static IEntityFactory RegisterDefaultServerProperty(this IEntityFactory factory, GameEntityPropertyEnum type, int value)
        {
            return factory.RegisterDefaultProperty(new EntityProperty((short)type, value){IsDirtyable = false});
        }

        public static IEntityFactory RegisterDefaultServerProperty(this IEntityFactory factory, GameEntityPropertyEnum type, long value)
        {
            return factory.RegisterDefaultProperty(new EntityProperty((short)type, value){IsDirtyable = false});
        }

        public static IEntityFactory RegisterDefaultServerProperty(this IEntityFactory factory, GameEntityPropertyEnum type, bool value)
        {
            return factory.RegisterDefaultProperty(new EntityProperty((short)type, value){IsDirtyable = false});
        }

        public static IEntityFactory RegisterDefaultServerProperty(this IEntityFactory factory, GameEntityPropertyEnum type, byte value)
        {
            return factory.RegisterDefaultProperty(new EntityProperty((short)type, value){IsDirtyable = false});
        }

        public static IEntityFactory RegisterDefaultServerProperty(this IEntityFactory factory, GameEntityPropertyEnum type, float value)
        {
            return factory.RegisterDefaultProperty(new EntityProperty((short)type, value){IsDirtyable = false});
        }



        public static IEntityFactory RegisterBehaviour(this IEntityFactory factory, EntityTypeEnum type, EntityBehaviourEnum behaviour, IEntityBehaviour instance)
        {
            return factory.RegisterBehaviour((short) type, (short) behaviour, instance);
        }

        public static IEntityFactory RegisterBehaviour(this IEntityFactory factory, EntityTypeEnum type, GameEntityBehaviourEnum behaviour, IEntityBehaviour instance)
        {
            return factory.RegisterBehaviour((short)type, (short)behaviour, instance);
        }

        public static IEntityFactory RegisterDefaultDamageHandler(this IEntityFactory factory, EntityTypeEnum type, IEntityDamageHandler handler)
        {
            return factory.RegisterDefaultDamageHandler((short)type, handler);
        }

        public static IEntityFactory RegisterDamageHandler(this IEntityFactory factory, EntityTypeEnum type, IEntityDamageHandler handler, DamageTypeEnum dmgType)
        {
            return factory.RegisterDamageHandler((short)type, handler, dmgType);
        }
    }
}
