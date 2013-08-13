using System;
using System.Collections.Generic;
using Vortex.Interface;
using Outbreak.Entities.Properties;

namespace Outbreak.Entities
{
    public class EntityTypeCache
    {
        private readonly Dictionary<string, EntityType> _typeCache;

        public EntityTypeCache()
        {
            _typeCache = new Dictionary<string, EntityType>(10);
        }

        public EntityType Get(string typeName)
        {
            if (!_typeCache.ContainsKey(typeName))
            {
                _typeCache[typeName] =
                    EntityType.LoadFromFile(String.Format("{0}.xml", typeName), typeof(GameEntityPropertyEnum));
            }

            return _typeCache[typeName];
        }
    }
}