using EpicEdit.Model.Factories;
using NUnit.Framework;
using Psy.Graphics.Models;
using Psy.Graphics.Models.Compilers;
using SlimMath;

namespace EngineTests.Vortex.EpicModelTests
{
    public class CompilingGeometry
    {
        private Psy.Core.EpicModel.EpicModel _epicModel;
        private EpicModelCompiler _compiler;
        private CompiledModel _compiledModel;

        [TestFixtureSetUp]
        public void Compile()
        {
            _epicModel = new Psy.Core.EpicModel.EpicModel();

            var cuboid = Cuboid.CreateCuboid();
            cuboid.Size = new Vector3(1.0f, 1.0f, 1.0f);

            _epicModel.ModelParts.Add(cuboid);

            _compiler = new EpicModelCompiler(_epicModel);
            _compiledModel = _compiler.Compile();
        }

        [Test]
        public void ItProducesOneMesh()
        {
            Assert.That(_compiledModel.Meshes.Count, Is.EqualTo(1));
        }

        [Test]
        public void TheMeshContainsTheCorrectNumberOfTriangles()
        {
            Assert.That(_compiledModel.Meshes[0].Triangles.Length, Is.EqualTo(6 * 2));
        }

        [Test]
        public void TheMeshContainsTheCorrectNumberOfTextureCoordinates()
        {
            Assert.That(_compiledModel.Meshes[0].TextureCoordinateBuffer.Length, Is.EqualTo(6 * 2 * 3));
        }

        [Test]
        public void TheMeshContainsTheCorrectNumberOfVertexPivotIndices()
        {
            Assert.That(_compiledModel.Meshes[0].VertexPivotIndex.Length, Is.EqualTo(6 * 3 * 2));
        }

        [Test]
        public void TheMeshContainsTheCorrectNumberOfVertices()
        {
            Assert.That(_compiledModel.Meshes[0].Vertices.Length, Is.EqualTo(6 * 3 * 2));
        }
    }
}