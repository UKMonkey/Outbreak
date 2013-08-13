using System.IO;
using NUnit.Framework;
using Psy.Core.EpicModel.Serialization;

namespace EngineTests.Vortex.EpicModelTests
{
    [TestFixture]
    public class FormatBackwardsCompatibilityTests
    {
        private static string GetPath(string filename)
        {
            const string path = @"Vortex/EpicModelTests/ReferenceFormats/";
            return Path.Combine(path.Replace('/', Path.DirectorySeparatorChar), filename);
        }

        private static void ReadModel(string filename)
        {
            var inputStream = new FileStream(GetPath(filename), FileMode.Open);
            var binaryReader = new BinaryReader(inputStream);
            var epicModelReader = new EpicModelReader(new TestMaterialTranslator(), binaryReader);

            var model = epicModelReader.Read(filename);
            Assert.That(model.ModelParts.Count, Is.GreaterThan(0));
        }

        [Test]
        public void ReadModelFormat1()
        {
            ReadModel("emf1.emd");
        }

        [Test]
        public void ReadModelFormat2()
        {
            ReadModel("emf2.emd");
        }

        [Test]
        public void ReadModelFormat3()
        {
            ReadModel("emf3.emd");
        }
    }
}