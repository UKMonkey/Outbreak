using EpicEdit.Model.Factories;
using NUnit.Framework;
using Psy.Core.EpicModel;
using SlimMath;

namespace EngineTests.Vortex.EpicModelTests
{
    public class SetParent
    {
        private EpicModel _model;
        private ModelPart _parent;
        private Anchor _connectingAnchor;
        private ModelPart _child;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _model = new EpicModel();

            _parent = Cuboid.CreateCuboid();
            _parent.Position = new Vector3(0.25f, 0.25f, -0.25f);
            _model.ModelParts.Add(_parent);
            _parent.Size = new Vector3(2, 2, 2);

            _connectingAnchor = _parent.AddAnchor("Connecting_Anchor");
            _connectingAnchor.Position = new Vector3(1, 1, -1);

            _child = Cuboid.CreateCuboid();
            _model.ModelParts.Add(_child);
            _child.Position = new Vector3(2, 2, 0);
            _child.Pivot.SetParent(_connectingAnchor);
        }

        [Test]
        public void ChildPositionShouldBeAdjusted()
        {
            Assert.That(_child.Position, Is.EqualTo(new Vector3(0.75f, 0.75f, 1.25f)));
        }

        [Test]
        public void ChildPivotShouldNowBeConnectedToConnectingAnchor()
        {
            Assert.That(_child.Pivot.Parent, Is.SameAs(_connectingAnchor));
        }

        [Test]
        public void ChildAbsolutePositionShouldBeCalculated()
        {
            var absolutePosition = _child.GetAbsolutePosition();
            Assert.That(absolutePosition, Is.EqualTo(new Vector3(2, 2, 0)));
        }
    }
}