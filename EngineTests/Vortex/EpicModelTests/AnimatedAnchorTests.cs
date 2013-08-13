using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Psy.Core;
using Psy.Core.EpicModel;
using Psy.Graphics.Models;
using Psy.Graphics.Models.Compilers;
using SlimMath;

namespace EngineTests.Vortex.EpicModelTests
{
    public class BaseAnimatedAnchor
    {
        protected EpicModel Model;
        protected Anchor Anchor;
        protected ModelPart Cuboid;
        protected Animation Animation;

        protected void AssertVector(Vector3 updated, Vector3 test)
        {
            Assert.AreEqual(test.X, updated.X, 0.0001f, "X coordinate");
            Assert.AreEqual(test.Y, updated.Y, 0.0001f, "Y coordinate");
            Assert.AreEqual(test.Z, updated.Z, 0.0001f, "Z coordinate");
        }

        protected void AssertMatrix(Matrix calculated, Matrix expected)
        {
            for (int i = 0; i < 16; i++)
            {
                var cE = calculated[i];
                var eE = calculated[i];

                Assert.AreEqual(cE, eE, 0.001f, string.Format("Matrix element {0}", i));
            }
        }

        public virtual void SetUp()
        {
            Model = new EpicModel();

            Cuboid = EpicEdit.Model.Factories.Cuboid.CreateCuboid();
            Cuboid.Pivot.Position = new Vector3(-0.5f, 0, 0);
            Cuboid.Position = new Vector3(1, 1, 0);
            Cuboid.Size = new Vector3(1, 1, 1);

            Anchor = Cuboid.AddAnchor("Test_Anchor");
            Anchor.Position = new Vector3(0.5f, 0, 0);
            Anchor.Rotation = new Vector3();

            Model.ModelParts.Add(Cuboid);

            Animation = Model.GetAnimation(AnimationType.Walking, true);

            var frame1 = Animation.AddFrame();
            frame1.Time = 0.0f;

            var frame2 = Animation.AddFrame();
            frame2.Time = 2.0f;

            var anchorAnimState = frame2.AnchorAnimStates.Single(x => x.Anchor == Anchor);
            anchorAnimState.Rotation = new Vector3(0, 0, (float)Math.PI / 2.0f);

            var cuboidAnimState = frame2.ModelPartAnimStates.Single(x => x.ModelPart == Cuboid);
            cuboidAnimState.Position = new Vector3(0, 0, 0);
            cuboidAnimState.Rotation = new Vector3(0, 0, (float)Math.PI / 2.0f);
        }
    }

    public class ModelInstanceAnimatedAnchorTests : BaseAnimatedAnchor
    {
        protected EpicModelCompiler Compiler;
        protected CompiledModel CompiledModel;
        protected ModelInstance ModelInstance;
        protected MaterialCache MaterialCache;

        [TestFixtureSetUp]
        public override void SetUp()
        {
            base.SetUp();

            Compiler = new EpicModelCompiler(Model);
            CompiledModel = Compiler.Compile();

            MaterialCache = new MaterialCache();
            MaterialCache.Add("example_material");
            ModelInstance = new ModelInstance(CompiledModel, MaterialCache);
        }

        [Test]
        public void ItShouldCalculateAnchorPositionForEachFrame()
        {
            var anchorIndex = CompiledModel.Anchors.FindIndex(x => x.Name == "Test_Anchor");

            ModelInstance.RunAnimation(AnimationType.Walking, false, false);
            for (var i = 0; i < CompiledModel.Animations[AnimationType.Walking].Frames.Count; i++)
            {
                ModelInstance.Update(1/24.0f);

                var calculatedAnchorPosition = ModelInstance.Anchors[anchorIndex].Position;
                var expectedAnchorPosition = CompiledModel.Animations[AnimationType.Walking].Frames[i].AnchorStates[anchorIndex].Position;

                Trace.WriteLine(string.Format("[{2}] calc:{0}    expt:{1}", calculatedAnchorPosition, expectedAnchorPosition, i));

                AssertVector(calculatedAnchorPosition, expectedAnchorPosition);

                var calculatedMatrix = ModelInstance.GetAnchorMatrix("Test_Anchor");
                var expectedMatrix = ModelInstance.Anchors[anchorIndex].GetMatrix();
                AssertMatrix(calculatedMatrix, expectedMatrix);

            }
        }
    }

    public class CompilingAnimatedAnchorTests : BaseAnimatedAnchor
    {
        protected EpicModelCompiler Compiler;
        protected CompiledModel CompiledModel;

        [TestFixtureSetUp]
        public override void SetUp()
        {
            base.SetUp();

            Compiler = new EpicModelCompiler(Model);
            CompiledModel = Compiler.Compile();
        }

        [Test]
        public void ItShouldCompileTheAnchorAtTheCorrectFirstFramePosition()
        {
            var animation = CompiledModel.Animations[AnimationType.Walking];
            var firstFrame = animation.Frames[0];
            var anchorIndex = CompiledModel.Anchors.FindIndex(x => x.Name == "Test_Anchor");

            var anchorFrame = firstFrame.AnchorStates[anchorIndex];

            AssertVector(anchorFrame.Position, new Vector3(2, 1, 0));
        }

        [Test]
        public void ItShouldCompileTheAnchorAtTheCorrectLastFramePosition()
        {
            var animation = CompiledModel.Animations[AnimationType.Walking];
            var firstFrame = animation.Frames.Last();
            var anchorIndex = CompiledModel.Anchors.FindIndex(x => x.Name == "Test_Anchor");

            var anchorFrame = firstFrame.AnchorStates[anchorIndex];

            AssertVector(anchorFrame.Position, new Vector3(0, 1, 0));
        }
    }


    public class AnimatedAnchorTests : BaseAnimatedAnchor
    {
        [TestFixtureSetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        [Test]
        public void AnchorIsTranslatedInTheFirstFrame()
        {
            Animation.ApplyAtTime(0);

            var pos = Anchor.GetAbsolutePosition();
            AssertVector(pos, new Vector3(2, 1, 0));
        }

        [Test]
        public void AnchorIsTranslatedInTheLastFrame()
        {
            Animation.ApplyAtTime(2.0f);

            var pos = Anchor.GetAbsolutePosition();
            Trace.WriteLine(pos);
            AssertVector(pos, new Vector3(0, 1, 0));
        }

        [Test]
        public void AnchorIsRotatedInTheFirstFrame()
        {
            Animation.ApplyAtTime(0);

            var v = new Vector3(1, 0, 0);
            var rot = Anchor.GetAbsoluteRotation();
            Trace.WriteLine(rot);
            var vdash = Vector3.Transform(v, rot);
            AssertVector(vdash, new Vector3(1, 0, 0));
        }

        [Test]
        public void AnchorIsRotatedInTheLastFrame()
        {
            Animation.ApplyAtTime(2.0f);

            var v = new Vector3(1, 0, 0);
            var rot = Anchor.GetAbsoluteRotation();
            Trace.WriteLine(rot);
            var vdash = Vector3.Transform(v, rot);
            AssertVector(vdash, new Vector3(-1, 0, 0));
        }
    }
}