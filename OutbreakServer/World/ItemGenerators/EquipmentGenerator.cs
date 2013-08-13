using System;
using System.Collections.Generic;
using Outbreak.Items;
using Outbreak.Items.Containers.InventoryItems;


namespace Outbreak.Server.World.ItemGenerators
{
    public enum EquipmentType
    {
        Shoes,
        Trousers,
        Top,
        Hat,
        Gloves,
    }

    public enum EquipmentMaterial
    {
        Leather,
        Woolen,
        Nylon,
        Spandex,
        Denim,
    }


    public class EquipmentGenerator : IItemGenerator
    {
        private readonly Dictionary<EquipmentMaterial, float> _materialColdProtectionFactors;
        private readonly Dictionary<EquipmentMaterial, float> _materialPhysicalDmgProtectionFactors;

        private readonly Dictionary<EquipmentType, float> _typeColdProtectionFactors;
        private readonly Dictionary<EquipmentType, float> _typePhysicalDmgProtectionFactors;

        protected Dictionary<EquipmentMaterial, float> MaterialColdProtection { get { return _materialColdProtectionFactors; } }
        protected Dictionary<EquipmentMaterial, float> MaterialPhysicalDmgProtection { get { return _materialPhysicalDmgProtectionFactors; } }
        protected Dictionary<EquipmentType, float> TypeColdProtection { get { return _typeColdProtectionFactors; } }
        protected Dictionary<EquipmentType, float> TypePhysicalDmgProtection { get { return _typePhysicalDmgProtectionFactors; } } 

        public EquipmentGenerator()
        {
            _materialColdProtectionFactors = GetMaterialColdProtection();
            _materialPhysicalDmgProtectionFactors = GetMaterialPhysicalDmgProtection();

            _typeColdProtectionFactors = GetTypeColdProtection();
            _typePhysicalDmgProtectionFactors = GetTypePyhsicalDmgProtection();
        }

        protected List<EquipmentType> GetAvailableTypes()
        {
            return new List<EquipmentType>{
                EquipmentType.Gloves,
                EquipmentType.Hat,
                EquipmentType.Shoes,
                EquipmentType.Top,
                EquipmentType.Trousers,
            };
        }

        protected List<EquipmentMaterial> GetAvailableMaterials(EquipmentType type)
        {
            switch (type)
            {
                case EquipmentType.Gloves:
                    return new List<EquipmentMaterial>{
                        EquipmentMaterial.Leather,
                        EquipmentMaterial.Woolen
                    };
                case EquipmentType.Hat:
                    return new List<EquipmentMaterial>{
                        EquipmentMaterial.Woolen
                    };
                case EquipmentType.Top:
                    return new List<EquipmentMaterial>{
                        EquipmentMaterial.Woolen,
                        EquipmentMaterial.Leather,
                        EquipmentMaterial.Denim,
                        EquipmentMaterial.Spandex,
                        EquipmentMaterial.Nylon
                    };
                case EquipmentType.Trousers:
                    return new List<EquipmentMaterial>{
                        EquipmentMaterial.Leather,
                        EquipmentMaterial.Denim,
                        EquipmentMaterial.Spandex,
                    };
                case EquipmentType.Shoes:
                    return new List<EquipmentMaterial>{
                        EquipmentMaterial.Leather
                    };
            }

            throw new NotImplementedException("Equipement type has no registered materials to be made of");
        }

        private Dictionary<EquipmentMaterial, float> GetMaterialColdProtection()
        {
            return new Dictionary<EquipmentMaterial, float>
                {
                    {EquipmentMaterial.Leather, 0.5f},
                    {EquipmentMaterial.Woolen, 1f},
                    {EquipmentMaterial.Nylon, 0.7f},
                    {EquipmentMaterial.Denim, 0.4f},
                    {EquipmentMaterial.Spandex, 0.1f}
                };
        }

        private Dictionary<EquipmentType, float> GetTypeColdProtection()
        {
            return new Dictionary<EquipmentType, float>
                {
                    {EquipmentType.Trousers, 0.7f},
                    {EquipmentType.Top, 1f},
                    {EquipmentType.Shoes, 0.3f},
                    {EquipmentType.Hat, 0.5f},
                    {EquipmentType.Gloves, 0.3f},
                };
        }

        private Dictionary<EquipmentMaterial, float> GetMaterialPhysicalDmgProtection()
        {
            return new Dictionary<EquipmentMaterial, float>
                {
                    {EquipmentMaterial.Woolen, 0.1f},
                    {EquipmentMaterial.Leather, 0.7f},
                    {EquipmentMaterial.Nylon, 0.4f},
                    {EquipmentMaterial.Spandex, 0},
                    {EquipmentMaterial.Denim, 0.7f}
                };
        }

        private Dictionary<EquipmentType, float> GetTypePyhsicalDmgProtection()
        {
            return new Dictionary<EquipmentType, float>
                {
                    {EquipmentType.Trousers, 1f},
                    {EquipmentType.Top, 1f},
                    {EquipmentType.Shoes, 0.3f},
                    {EquipmentType.Hat, 0.2f},
                    {EquipmentType.Gloves, 0.3f},
                };
        }


        // get a random type and a random material for that type
        // then get a random display image that'll fit that type & material
        // then check that it doesn't already exist  (if it does, don't remake it)
        // then establish what protections it should provide
        // then establish what the max durability should be
        // then set some random durability on it
        public InventoryItem Generate()
        {
        }
    }
}
