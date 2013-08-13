using System;
using EpicEdit.Model.Factories;
using NUnit.Framework;
using Psy.Core;
using Psy.Core.EpicModel;
using Psy.Graphics.Models;
using Psy.Graphics.Models.Compilers;
using SlimMath;

namespace EngineTests.Vortex.ModelInstanceTests
{
    public class UpdatingTranslationsAndRotations
    {
        private EpicModel _epicModel;
        private EpicModelCompiler _compiler;
        private CompiledModel _compiledModel;
        private MaterialCache _materialCache;
        private ModelInstance _modelInstance;
        private ModelPart _parentCuboid;
        private ModelPart _childCuboid;
        private Anchor _connector;
        private Material _parentMtl;
        private Material _childMtl;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _materialCache = new MaterialCache();
            _parentMtl = _materialCache.Add("testmtl1");
            _childMtl = _materialCache.Add("testmtl2");

            _epicModel = new EpicModel();

            _parentCuboid = Cuboid.CreateCuboid();
            _parentCuboid.Size = new Vector3(2, 2, 2);
            _parentCuboid.Rotation = new Vector3(0, 0, -(float)Math.PI / 4.0f);
            _parentCuboid.MaterialId = _parentMtl.Id;

            _connector = _parentCuboid.AddAnchor("Connector");
            _connector.Position = new Vector3(1, 1, 0);

            _childCuboid = Cuboid.CreateCuboid();
            _childCuboid.Position = new Vector3(-1, -1, 0);
            _childCuboid.Rotation = new Vector3(0, 0, -(float)Math.PI/2.0f);
            _childCuboid.MaterialId = _childMtl.Id;
            

            _epicModel.ModelParts.Add(_parentCuboid);
            _childCuboid.Pivot.SetParent(_connector);

            _compiler = new EpicModelCompiler(_epicModel);
            _compiledModel = _compiler.Compile();

            _modelInstance = new ModelInstance(_compiledModel, _materialCache);

            _modelInstance.Update(1/24.0f);
        }

        private void AssertVector(Vector4 updated, Vector3 test)
        {
            Assert.AreEqual(test.X, updated.X, 0.0001f);
            Assert.AreEqual(test.Y, updated.Y, 0.0001f);
            Assert.AreEqual(test.Z, updated.Z, 0.0001f);
        }

        [Test]
        public void ParentVerticesHaveBeenCalculatedCorrectly()
        {
            var parentIndex = _modelInstance.Model.Meshes.FindIndex(x => x.MaterialId == _parentMtl.Id);
            var parentMesh = _modelInstance.MeshInstances[parentIndex];

            var t0v0 = parentMesh.VertexBuffer[0];
            var t0v1 = parentMesh.VertexBuffer[1];
            var t0v2 = parentMesh.VertexBuffer[2];
            
            AssertVector(t0v0.Position, new Vector3(-1.414214f, 0, 1));
            AssertVector(t0v1.Position, new Vector3(-1.414214f, 0, -1));
            AssertVector(t0v2.Position, new Vector3(0, -1.414214f, -1));

        }
    }
}