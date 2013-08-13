using System.Collections.Generic;
using System.Diagnostics;
using EpicEdit.Model.Factories;
using NUnit.Framework;
using Psy.Core;
using Psy.Core.EpicModel;
using Psy.Graphics.Models;
using Psy.Graphics.Models.Compilers;
using SlimMath;

namespace EngineTests.Vortex.EpicModelTests
{
    public class Performance
    {
        const int InstanceCount = 128;
        const int ChildCount = 128;

        private MaterialCache _materialCache;
        private List<ModelInstance> _instances;
        private int _vertexCount;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _materialCache = new MaterialCache();
            var material = _materialCache.Add("mat");

            _instances = new List<ModelInstance>();
            
            for (int i = 0; i < InstanceCount; i++)
            {
                _instances.Add(CreateModel(material));
            }
        }

        [Test]
        public void SinglePass()
        {
            var startTime = Timer.GetTime();
            PerformUpdate();
            var endTime = Timer.GetTime();

            Trace.WriteLine(string.Format("Duration {0:0.000}", endTime - startTime));

            var totalVertices = _vertexCount * InstanceCount;

            Trace.WriteLine(string.Format("Vertex count {0}", totalVertices));
            Trace.WriteLine(string.Format("Performance: {0:0.000}", totalVertices / (endTime - startTime)));
        }

        private void PerformUpdate()
        {
            var count = _instances.Count;
            for (var i = 0; i < count; i++)
            {
                _instances[i].Update(1/24.0f);
            }
        }

        private ModelInstance CreateModel(Material material)
        {
            var model = new EpicModel();
            var parent = Cuboid.CreateCuboid();
            parent.Position = new Vector3(0.25f, 0.25f, -0.25f);
            parent.MaterialId = material.Id;
            model.ModelParts.Add(parent);
            parent.Size = new Vector3(2, 2, 2);

            model.ModelParts.Add(parent);

            parent.MaterialId = material.Id;

            for (int i = 0; i < ChildCount; i++)
            {
                var child = Cuboid.CreateCuboid();

                child.Position = new Vector3(i, i, -i);
                child.Pivot.SetParent(parent.Pivot, true);
                child.MaterialId = material.Id;

                model.ModelParts.Add(child);
            }

            var emc = new EpicModelCompiler(model);
            var compiledModel = emc.Compile();

            var modelInstance = new ModelInstance(compiledModel, _materialCache);

            _vertexCount = modelInstance.MeshInstances[0].VertexBuffer.Length;

            return modelInstance;
        }
    }
}