using Psy.Core.EpicModel.Serialization;

namespace EngineTests.Vortex.EpicModelTests
{
    public class TestMaterialTranslator : IMaterialTranslator
    {
        public string Translate(int materialId)
        {
            return "Material";
        }

        public int Translate(string textureName)
        {
            return 1;
        }
    }
}