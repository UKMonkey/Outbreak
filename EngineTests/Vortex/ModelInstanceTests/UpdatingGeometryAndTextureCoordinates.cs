using EpicEdit.Model.Factories;
using NUnit.Framework;
using Psy.Core;
using Psy.Graphics.Models;
using Psy.Graphics.Models.Compilers;
using SlimMath;

namespace EngineTests.Vortex.ModelInstanceTests
{
    public class UpdatingGeometryAndTextureCoordinates
    {
        private Psy.Core.EpicModel.EpicModel _epicModel;
        private EpicModelCompiler _compiler;
        private CompiledModel _compiledModel;
        private MaterialCache _materialCache;
        private ModelInstance _modelInstance;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _materialCache = new MaterialCache();
            var testmtl = _materialCache.Add("testmtl");

            _epicModel = new Psy.Core.EpicModel.EpicModel();

            var cuboid = Cuboid.CreateCuboid();
            cuboid.Size = new Vector3(1.0f, 1.0f, 1.0f);
            cuboid.MaterialId = testmtl.Id;

            _epicModel.ModelParts.Add(cuboid);

            _compiler = new EpicModelCompiler(_epicModel);
            _compiledModel = _compiler.Compile();

            _modelInstance = new ModelInstance(_compiledModel, _materialCache);

            _modelInstance.Update(1/24.0f);
        }

        [Test]
        public void TheTriangleCountIsCorrect()
        {
            Assert.That(_modelInstance.MeshInstances[0].TriangleCount, Is.EqualTo(6 * 2));
        }

        [Test]
        public void TheVertexBufferIsTheCorrectLength()
        {
            var meshInstance = _modelInstance.MeshInstances[0];
            var count = meshInstance.VertexBuffer.Length;
            Assert.That(count, Is.EqualTo(6 * 2 * 3));
        }

        [Test]
        public void TextureCoordinatesAreCorrect()
        {
            for (var i = 0; i < _modelInstance.MeshInstances[0].VertexBuffer.Length; i += 6)
            {
                AssertTextureCoordinateMatches(i, 0, 1);
                AssertTextureCoordinateMatches(i + 1, 0, 0);
                AssertTextureCoordinateMatches(i + 2, 1, 0);
                AssertTextureCoordinateMatches(i + 3, 0, 1);
                AssertTextureCoordinateMatches(i + 4, 1, 0);
                AssertTextureCoordinateMatches(i + 5, 1, 1);
            }
        }

        [Test]
        public void ColoursAreCorrect()
        {
            for (var i = 0; i < _modelInstance.MeshInstances[0].VertexBuffer.Length; i += 1)
            {
                var colour = _modelInstance.MeshInstances[0].VertexBuffer[i].Colour;

                Assert.That(colour, Is.EqualTo(new Color4(1.0f, 1.0f, 1.0f, 1.0f)));
            }
        }

        private void AssertTextureCoordinateMatches(int index, int u, int v)
        {
            var coord = _modelInstance.MeshInstances[0].VertexBuffer[index].TextureCoordinate;
            Assert.That(coord, Is.EqualTo(new Vector2(u, v)), string.Format("Texture coordinate at index {0} is incorrect", index));
        }
    }
}