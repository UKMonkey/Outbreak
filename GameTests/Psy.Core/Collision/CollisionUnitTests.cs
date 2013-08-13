using NUnit.Framework;
using Psy.Core.Collision;
using Psy.Core;
using SlimMath;

namespace UnitTests.Psy.Core.Collision
{
    public class CollisionUnitTests
    {
        [Test]
        public void TestBuilder()
        {
            var builder = new MeshBuilder<Mesh>(new Mesh());
            var mesh =
                builder.WithId(5).
                        Rectangle(new Vector3(0, 0, 0),
                                new Vector3(0, 0, 10),
                                new Vector3(5, 0, 10),
                                new Vector3(5, 0, 0)).
                         GetMesh();

            //mesh.GetTriangles(Direction.North);

            var tester = new MeshCollisionTester(mesh);
            var point = new Vector3(2.5f, 10, 5);
            var direction = new Vector3(0.01f, -1, 0.1f);
            var result = tester.CollideWithRay(point, direction);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.RayCollisionResult.HasCollided);
            Assert.AreEqual(5, result.TriangleId);
            Assert.AreEqual(result.RayCollisionResult.CollisionMesh, mesh);
            Assert.AreEqual(result.RayCollisionResult.CollisionPoint, point + 10 * direction);
        }
    }
}
