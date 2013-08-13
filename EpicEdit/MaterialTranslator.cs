using System;
using System.IO;
using Psy.Core.EpicModel.Serialization;
using Psy.Core.FileSystem;

namespace EpicEdit
{
    public class MaterialTranslator : IMaterialTranslator
    {
        private readonly Editor _editor;

        public MaterialTranslator(Editor editor)
        {
            _editor = editor;
        }

        public string Translate(int materialId)
        {
            var filename = Path.GetFileName(_editor.Materials[materialId].TextureName);

            return _editor.Materials.HasMaterial(materialId)
                       ? filename
                       : "noTexture.png";
        }

        public int Translate(string textureName)
        {

            if (!Lookup.AssetExists(textureName))
            {
                throw new Exception(string.Format("Cannot find texture `{0}`", textureName));
            }

            var material = _editor.Materials.GetByTextureName(textureName) ?? _editor.Materials.Add(textureName);

            return material.Id;
        }
    }
}