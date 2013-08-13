using System.Collections.Generic;

namespace Outbreak.Items.Containers.InventorySpecs
{
    public class ItemSpec
    {
        public readonly int Id;

        private readonly Dictionary<short, ItemSpecProperty> _properties;
        private int? _hash;
        private int Hash
        {
            get
            {
                if (_hash == null)
                    _hash = GetHashCode();
                return _hash.Value;
            }
        }

        public ItemSpec()
        {
            Id = 0;
            _properties = new Dictionary<short, ItemSpecProperty>();
        }

        public ItemSpec(int id)
        {
            Id = id;
            _properties = new Dictionary<short, ItemSpecProperty>();
        }

        private static HashSet<ItemSpecPropertyEnum> GetPropertiesToExclude()
        {
            return new HashSet<ItemSpecPropertyEnum>
                       {
                           ItemSpecPropertyEnum.Name,
                           ItemSpecPropertyEnum.ModelName,
                           ItemSpecPropertyEnum.Description,
                           ItemSpecPropertyEnum.ImageName
                       };
        }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;

            if (other.GetType() != typeof(ItemSpec))
                return false;

            return this == (ItemSpec) other;
        }

        public static bool operator != (ItemSpec a, ItemSpec b)
        {
            return !(a == b);
        }

        public static bool operator == (ItemSpec a, ItemSpec b)
        {
            var aIsNull = System.Object.ReferenceEquals(a, null);
            var bIsNull = System.Object.ReferenceEquals(b, null);
            
            if (aIsNull || bIsNull)
            {
                return aIsNull && bIsNull;
            }

            if (a.Hash != b.Hash)
                return false;
            if (a.Id == b.Id)
                return true;
            if (a._properties.Count != b._properties.Count)
                return false;

            // so fast checking is done, now we have to check each property is the same ...
            var props = GetPropertiesToExclude();
            foreach (var aProp in a.GetProperties())
            {
                var type = (ItemSpecPropertyEnum)aProp.PropertyId;
                if (props.Contains(type))
                    continue;

                var bProp = b.GetProperty(type);
                if (bProp == null)
                    return false;

                if (aProp.Value.Length != bProp.Value.Length)
                    return false;
                for (var i = 0; i < aProp.Value.Length; ++i)
                {
                    if (aProp.Value[i] != bProp.Value[i])
                        return false;
                }
            }

            return true;
        }

        public ItemSpec Clone()
        {
            return Clone(Id);
        }

        public ItemSpec Clone(int id)
        {
            var ret = new ItemSpec(id);
            foreach (var item in _properties)
                ret.SetProperty(item.Value);

            return ret;
        }

        public IEnumerable<ItemSpecProperty> GetProperties()
        {
            return _properties.Values;
        }

        public ItemSpecProperty GetProperty(ItemSpecPropertyEnum prop)
        {
            return _properties.ContainsKey((short)prop) ? _properties[(short)prop] : null;
        }

        public bool HasProperty(ItemSpecPropertyEnum prop)
        {
            return _properties.ContainsKey((short)prop);
        }

        public void SetProperty(ItemSpecProperty prop)
        {
            if (_properties.ContainsKey(prop.PropertyId))
                _properties[prop.PropertyId].Value = prop.Value;
            else
                _properties.Add(prop.PropertyId, new ItemSpecProperty(prop));
        }

        public void SetProperties(List<ItemSpecProperty> specProperties)
        {
            foreach (var itemSpecProperty in specProperties)
            {
                SetProperty(itemSpecProperty);
            }
        }

        public override int GetHashCode()
        {
            var props = GetProperties();
            var propsToIgnore = GetPropertiesToExclude();
            var hash = 0;

            foreach (var prop in props)
            {
                var type = (ItemSpecPropertyEnum)prop.PropertyId;
                if (propsToIgnore.Contains(type))
                    continue;

                var data = prop.Value;
                for (var i = 0; i < data.Length; ++i)
                    hash += ((int)(data[i]) << i);
            }

            return hash;
        }
    }
}
