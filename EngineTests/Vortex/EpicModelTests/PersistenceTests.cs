using System.IO;
using System.Linq;
using EpicEdit.Model.Factories;
using NUnit.Framework;
using Psy.Core.EpicModel;
using Psy.Core.EpicModel.Serialization;
using SlimMath;

namespace EngineTests.Vortex.EpicModelTests
{
    [TestFixture]
    public class PersistenceTests
    {
        private EpicModel _model;
        private EpicModel _readModel;

        [TestFixtureSetUp]
        public void SaveAndLoad()
        {
            _model = new EpicModel("test");
            var cuboid = Cuboid.CreateCuboid();
            cuboid.Name = "Cuboid1";
            cuboid.AddAnchor("a1");
            cuboid.AddAnchor("a2");
            cuboid.Position = new Vector3(1, 1, 0);

            var cuboid2 = Cuboid.CreateCuboid();
            cuboid2.Name = "Cuboid2";
            var a3 = cuboid2.AddAnchor("a3");
            cuboid2.AddAnchor("a4");
            a3.Position = new Vector3(1, 1, 0);

            a3.Rotation = new Vector3(1.0f, 2.0f, 3.0f);

            cuboid.Pivot.SetParent(a3);

            _model.ModelParts.Add(cuboid);
            _model.ModelParts.Add(cuboid2);
            _model.ModelParts.Add(cuboid.Clone());

            var animation = _model.GetAnimation(AnimationType.Drop, true);
            animation.AddFrame().Time = 0.123f;
            animation.AddFrame().Time = 0.265f;
            animation.AddFrame().Time = 0.384f;

            var animation2 = _model.GetAnimation(AnimationType.Death3, true);
            animation2.AddFrame().Time = 0.464f;
            animation2.AddFrame().Time = 0.519f;
            animation2.AddFrame().Time = 0.694f;

            var materialTranslator = new TestMaterialTranslator();

            var memoryStream = new MemoryStream(4096);

            var binaryWriter = new BinaryWriter(memoryStream);
            var writer = new EpicModelWriter(materialTranslator);
            writer.Write(binaryWriter, _model);

            memoryStream.Position = 0;

            var binaryReader = new BinaryReader(memoryStream);
            var reader = new EpicModelReader(materialTranslator, binaryReader);
            _readModel = reader.Read("test");
        }

        [Test]
        public void CorrectNumberOfAnimationsHaveBeenLoaded()
        {
            Assert.That(_readModel.Animations.Count, Is.EqualTo(_model.Animations.Count));
        }

        [Test]
        public void CuboidIsNotARootModelPart()
        {
            var cuboid = _readModel.ModelParts.Single(x => x.Name == "Cuboid1");
            Assert.That(cuboid.IsARootModelPart, Is.False);
        }

        [Test]
        public void CuboidHasItsCorrectParent()
        {
            var cuboid = _model.ModelParts.Single(x => x.Name == "Cuboid1");
            var readCuboid = _readModel.ModelParts.Single(x => x.Name == "Cuboid1");

            Assert.That(readCuboid.Pivot.Position, Is.EqualTo(cuboid.Pivot.Position));
            Assert.That(readCuboid.Pivot.Parent.Name, Is.EqualTo(cuboid.Pivot.Parent.Name));
        }

        [Test]
        public void CuboidHasItsCorrectPosition()
        {
            var cuboid = _model.ModelParts.Single(x => x.Name == "Cuboid1");
            var readCuboid = _readModel.ModelParts.Single(x => x.Name == "Cuboid1");

            Assert.That(readCuboid.Position, Is.EqualTo(cuboid.Position));
        }

        [Test]
        public void DropAnimationMatches()
        {
            var animation = _model.GetAnimation(AnimationType.Drop);
            var readAnimation = _readModel.GetAnimation(AnimationType.Drop);

            Assert.That(readAnimation, Is.Not.Null);

            foreach (var anim in animation.Keyframes.Zip(readAnimation.Keyframes, (x, y) => new { One = x, Other = y}))
            {
                Assert.That(anim.One.Time, Is.EqualTo(anim.Other.Time));

                for (var index = 0; index < anim.One.ModelPartAnimStates.Count; index++)
                {
                    var oneMpas = anim.One.ModelPartAnimStates[index];
                    var otherMpas = anim.Other.ModelPartAnimStates[index];
                    Assert.That(oneMpas.ModelPart.Id, Is.EqualTo(otherMpas.ModelPart.Id));
                }
            }
        }

        [Test]
        public void AnchorParentsAreCorrect()
        {
            var cuboid1 = _readModel.ModelParts.Single(x => x.Name == "Cuboid1");
            var cuboid2 = _readModel.ModelParts.Single(x => x.Name == "Cuboid2");
            var anchorA3 = cuboid2.Anchors.Single(x => x.Name == "a3");
            Assert.That(cuboid1.Pivot.Parent, Is.EqualTo(anchorA3));
        }

        [Test]
        public void AnchorChildrenAreCorrect()
        {
            var cuboid1 = _readModel.ModelParts.Single(x => x.Name == "Cuboid1");
            var cuboid2 = _readModel.ModelParts.Single(x => x.Name == "Cuboid2");
            var anchorA3 = cuboid2.Anchors.Single(x => x.Name == "a3");
            Assert.That(anchorA3.Children.Count, Is.EqualTo(1));
            Assert.That(anchorA3.Children[0], Is.EqualTo(cuboid1));
        }

        [Test]
        public void CorrectNumberOfModelParts()
        {
            Assert.AreEqual(_model.ModelParts.Count, _readModel.ModelParts.Count);
        }

        [Test]
        public void ModelVerticesMatch()
        {
            for (var i = 0; i < _model.ModelParts.Count; i++)
            {
                for (var j = 0; j < _model.ModelParts[i].Vertices.Length; j++)
                {
                    var vert1 = _model.ModelParts[i].Vertices[j];
                    var vert2 = _readModel.ModelParts[i].Vertices[j];

                    Assert.AreEqual(vert1.X, vert2.X, 0.01f);
                    Assert.AreEqual(vert1.Y, vert2.Y, 0.01f);
                    Assert.AreEqual(vert1.Z, vert2.Z, 0.01f);
                }
            }
        }

        [Test]
        public void AnchorsMatch()
        {
            for (var i = 0; i < _model.ModelParts.Count; i++)
            {
                for (var j = 0; j < _model.ModelParts[i].Anchors.Count; j++)
                {
                    var anchor1 = _model.ModelParts[i].Anchors[j];
                    var anchor2 = _readModel.ModelParts[i].Anchors[j];

                    Assert.AreEqual(anchor1.Rotation.X, anchor2.Rotation.X, 0.01f);
                    Assert.AreEqual(anchor1.Rotation.Y, anchor2.Rotation.Y, 0.01f);
                    Assert.AreEqual(anchor1.Rotation.Z, anchor2.Rotation.Z, 0.01f);
                    Assert.AreEqual(anchor1.Position.X, anchor2.Position.X, 0.01f);
                    Assert.AreEqual(anchor1.Position.Y, anchor2.Position.Y, 0.01f);
                    Assert.AreEqual(anchor1.Position.Z, anchor2.Position.Z, 0.01f);
                    Assert.AreEqual(anchor1.Name, anchor2.Name);

                    for (var k = 0; k < _model.ModelParts[i].Anchors[j].Children.Count; k++)
                    {
                        var child1 = _model.ModelParts[i].Anchors[j].Children[k];
                        var child2 = _readModel.ModelParts[i].Anchors[j].Children[k];

                        Assert.AreEqual(child1.Id, child2.Id);
                    }
                }
            }
        }
    }
}