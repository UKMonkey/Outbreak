using System;
using System.Collections.Generic;
using System.Xml;
using Outbreak.Items.Containers;
using Psy.Core;
using Psy.Core.FileSystem;
using Vortex.Interface;

namespace Outbreak.Items
{
    public static class WearableItemStatsCache
    {
        private const string FileName = "armour.xml";

        private static readonly List<string> Materials = new List<string>();
        private static readonly Dictionary<string, short> Durability = new Dictionary<string, short>();
        private static readonly Dictionary<string, Dictionary<DamageTypeEnum, float>> Defences = new Dictionary<string, Dictionary<DamageTypeEnum, float>>();

        private static readonly Dictionary<InventorySpecialSlotEnum, Dictionary<DamageTypeEnum, float>> SlotMultipliers = new Dictionary<InventorySpecialSlotEnum, Dictionary<DamageTypeEnum, float>>();


        private static DamageTypeEnum GetDamageType(string name)
        {
            var names = Enum.GetNames(typeof (DamageTypeEnum));
            var values = Enum.GetValues(typeof (DamageTypeEnum));

            var loweredName = name.ToLower();

            for (var i=0; i<names.Length; ++i)
            {
                var enumName = names[i].ToLower();
                if (loweredName == enumName)
                    return (DamageTypeEnum)values.GetValue(i);
            }

            throw new Exception("Unable to convert " + name + " to a known damage type");
        }

        private static InventorySpecialSlotEnum GetSlotType(string name)
        {
            switch (name.ToLower())
            {
                case "head":
                    return InventorySpecialSlotEnum.HeadArmour;
                case "body":
                    return InventorySpecialSlotEnum.BodyArmour;
                case "legs":
                    return InventorySpecialSlotEnum.LegArmour;
                case "feet":
                    return InventorySpecialSlotEnum.FootArmour;
                default:
                    throw new Exception("Unrecognised slot type in armour file");
            }
        }

        private static float GetDefenceValue(string value)
        {
            return Single.Parse(value);
        }

        private static void LoadArmourData(XmlNode armourNode)
        {
            string name = null;
            var defs = new Dictionary<DamageTypeEnum, float>();

            foreach (XmlNode data in armourNode.ChildNodes)
            {
                if (data.Name == "name")
                {
                    name = data.InnerText;
                }
                else if (data.Name == "multiplier")
                {
                    if (data.Attributes == null)
                        throw new Exception("No attributes provided in armour multiplier");

                    var type = GetDamageType(data.Attributes.GetNamedItem("type").Value);
                    var value = GetDefenceValue(data.Attributes.GetNamedItem("value").Value);

                    defs[type] = value;
                }
            }

            if (name == null)
                throw new Exception("No name provided in armour data");
            var slotId = GetSlotType(name);
            SlotMultipliers[slotId] = defs;
        }

        private static void LoadMaterialData(XmlNode materialNode)
        {
            string name = null;
            short ?durability = null;
            var defs = new Dictionary<DamageTypeEnum, float>();

            foreach (XmlNode data in materialNode.ChildNodes)
            {
                if (data.Name == "name")
                {
                    name = data.InnerText;
                }
                else if (data.Name == "multiplier")
                {
                    if (data.Attributes == null)
                        throw new Exception("No attributes specified in defence node");

                    var type = GetDamageType(data.Attributes.GetNamedItem("type").Value);
                    var value = GetDefenceValue(data.Attributes.GetNamedItem("value").Value);

                    defs[type] = value;
                }
                else if (data.Name == "durability")
                {
                    durability = Int16.Parse(data.InnerText);
                }
                else
                {
                    throw new Exception("Unrecognised data in materials file");
                }
            }

            if (name == null)
                throw new Exception("Unable to load materials - no name specified");
            if (durability == null)
                throw new Exception("Unable to obtain durability for " + name);

            Materials.Add(name);
            Durability[name] = durability.Value;
            Defences[name] = defs;
        }

        private static void ReadData()
        {
            lock (Materials)
            {
                if (Materials.Count != 0)
                    return;

                var assetPath = Lookup.GetAssetPath(FileName);
                var document = new XmlDocument();

                document.Load(assetPath);
                var rootNode = document.DocumentElement;
                if (rootNode == null)
                    throw new AssetLoadException("Unable to load file " + FileName);

                foreach (XmlNode child in rootNode.ChildNodes)
                {
                    if (child.Name == "material")
                        LoadMaterialData(child);
                    else if (child.Name == "armour")
                        LoadArmourData(child);
                    else if (child.NodeType != XmlNodeType.Comment)
                        throw new Exception("Unrecognised armour tag: " + child.Name);
                }
            }
        }

        public static Dictionary<DamageTypeEnum, float> GetBaseSlotDefences(InventorySpecialSlotEnum slot)
        {
            if (SlotMultipliers.Count == 0)
                ReadData();
            return SlotMultipliers[slot];
        }

        public static List<string> GetMaterials()
        {
            if (Materials.Count == 0)
                ReadData();
            return Materials;
        }

        public static Dictionary<DamageTypeEnum, float> GetBaseDamageDefences(string material, Dictionary<DamageTypeEnum, float> multipliers)
        {
            if (Defences.Count == 0)
                ReadData();

            var map = new Dictionary<DamageTypeEnum, float>();
            foreach (var item in Defences[material])
            {
                map[item.Key] = item.Value * multipliers[item.Key];
            }
            return map;
        }

        public static short GetBaseMaxDurability(string material)
        {
            if (Durability.Count == 0)
                ReadData();

            return Durability[material];
        }
    }
}
