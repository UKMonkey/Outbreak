using System;
using System.Linq;
using EpicEdit.Model.Factories;
using NUnit.Framework;
using Psy.Core.EpicModel;
using Psy.Core.Logging;
using Psy.Core.Logging.Loggers;
using Psy.Graphics.Models;
using Psy.Graphics.Models.Compilers;
using SlimMath;

namespace EngineTests.Vortex.EpicModelTests
{
    public class CompilingAnimations
    {
        private EpicModel _model;
        private EpicModelCompiler _compiler;
        private CompiledModel _compiledModel;

        [TestFixtureSetUp]
        public void SaveAndLoad()
        {
            Logger.Add(new TraceLogger { LoggerLevel = LoggerLevel.Trace });

            _model = new EpicModel();
            var cuboid = Cuboid.CreateCuboid();
            cuboid.MaterialId = 1;
            cuboid.Name = "Cuboid1";
            cuboid.AddAnchor("a1");
            cuboid.AddAnchor("a2");

            var cuboid2 = Cuboid.CreateCuboid();
            cuboid2.MaterialId = 1;
            cuboid2.Name = "Cuboid2";
            var a3 = cuboid2.AddAnchor("a3");
            
            var a4 = cuboid2.AddAnchor("a4");
            a4.Position = new Vector3(0.5f, 0.0f, 0.0f);

            cuboid.Pivot.SetParent(a3);

            _model.ModelParts.Add(cuboid);
            _model.ModelParts.Add(cuboid2);

            var cuboid3 = Cuboid.CreateCuboid();
            cuboid3.MaterialId = 0;
            _model.ModelParts.Add(cuboid3);

            var animation = _model.GetAnimation(AnimationType.Drop, true);
            animation.AddFrame().Time = 0.0f;
            animation.AddFrame().Time = 0.265f;
            animation.AddFrame().Time = 0.384f;

            var animation2 = _model.GetAnimation(AnimationType.Death3, true);
            animation2.AddFrame().Time = 0.0f;

            cuboid2.Rotation = new Vector3(0.0f, 0.0f, (float)(Math.PI/2.0f));
            cuboid2.Position = new Vector3(-1f, -1f, 0);

            var frame2 = animation2.AddFrame();
            frame2.LoadFromCurrentModelState(_model);
            frame2.Time = 0.519f;

            cuboid2.Rotation = new Vector3();
            cuboid2.Position = new Vector3();

            _compiler = new EpicModelCompiler(_model);
            _compiledModel = _compiler.Compile();
        }

        [Test]
        public void TextureCoordinateBufferIsPopulated()
        {
            Assert.That(_compiledModel.Meshes.Single(x => x.MaterialId == 1).TextureCoordinateBuffer.Length, Is.EqualTo(72));
            Assert.That(_compiledModel.Meshes.Single(x => x.MaterialId == 0).TextureCoordinateBuffer.Length, Is.EqualTo(36));
        }

        [Test]
        public void TrianglesListIsPopulated()
        {
            Assert.That(_compiledModel.Meshes.Single(x => x.MaterialId == 1).Triangles.Length, Is.EqualTo(24));
            Assert.That(_compiledModel.Meshes.Single(x => x.MaterialId == 0).Triangles.Length, Is.EqualTo(12));
        }

        [Test]
        public void VerticesBufferIsPopulated()
        {
            Assert.That(_compiledModel.Meshes.Single(x => x.MaterialId == 1).Vertices.Length, Is.EqualTo(72));
            Assert.That(_compiledModel.Meshes.Single(x => x.MaterialId == 0).Vertices.Length, Is.EqualTo(36));
        }

        [Test]
        public void AnimatedHaveBeenCreated()
        {
            Assert.That(_compiledModel.Animations.Count, Is.EqualTo(3));
        }

        [Test]
        public void DropAnimationHasBeenCreated()
        {
            Assert.That(_compiledModel.Animations.SingleOrDefault(x => x.Key == AnimationType.Drop), Is.Not.Null);
        }

        [Test]
        public void Death3AnimationHasBeenCreated()
        {
            Assert.That(_compiledModel.Animations.SingleOrDefault(x => x.Key == AnimationType.Death3), Is.Not.Null);
        }
    }
}