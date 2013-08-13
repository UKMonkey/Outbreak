using Psy.Core.EpicModel;
using SlimMath;

namespace EpicEdit.Model.Factories
{
    public static class Cuboid
    {
        public static ModelPart CreateCuboid()
        {
            var model = new ModelPart();

            model.Faces = new ModelPartFace[6];
            for (var i = 0; i < 6; i++)
            {
                model.Faces[i] = ModelPartFace.CreateSquare(model, i);
            }

            const int front = (int) CuboidFaceIndices.Front;
            model.Faces[front].VertexIndices = new[] { 0, 1, 2, 3 };
            model.Faces[front].Triangles[0] = new ModelTriangle(0, 1, 2, model.Faces[front], 0, 1, 2);
            model.Faces[front].Triangles[1] = new ModelTriangle(0, 2, 3, model.Faces[front], 0, 2, 3);

            const int back = (int) CuboidFaceIndices.Back;
            model.Faces[back].VertexIndices = new[] { 7, 6, 5, 4 };
            model.Faces[back].Triangles[0] = new ModelTriangle(7, 6, 5, model.Faces[back], 0, 1, 2);
            model.Faces[back].Triangles[1] = new ModelTriangle(7, 5, 4, model.Faces[back], 0, 2, 3);

            const int left = (int)CuboidFaceIndices.Left;
            model.Faces[left].VertexIndices = new[] { 4, 5, 1, 0 };
            model.Faces[left].Triangles[0] = new ModelTriangle(4, 5, 1, model.Faces[left], 0, 1, 2);
            model.Faces[left].Triangles[1] = new ModelTriangle(4, 1, 0, model.Faces[left], 0, 2, 3);

            const int right = (int) CuboidFaceIndices.Right;
            model.Faces[right].VertexIndices = new[] { 3, 2, 6, 7 };
            model.Faces[right].Triangles[0] = new ModelTriangle(3, 2, 6, model.Faces[right], 0, 1, 2);
            model.Faces[right].Triangles[1] = new ModelTriangle(3, 6, 7, model.Faces[right], 0, 2, 3);

            const int top = (int) CuboidFaceIndices.Top;
            model.Faces[top].VertexIndices = new[] { 1, 5, 6, 2 };
            model.Faces[top].Triangles[0] = new ModelTriangle(1, 5, 6, model.Faces[top], 0, 1, 2);
            model.Faces[top].Triangles[1] = new ModelTriangle(1, 6, 2, model.Faces[top], 0, 2, 3);

            const int bottom = (int) CuboidFaceIndices.Bottom;
            model.Faces[bottom].VertexIndices = new[] { 4, 0, 3, 7 };
            model.Faces[bottom].Triangles[0] = new ModelTriangle(4, 0, 3, model.Faces[bottom], 0, 1, 2);
            model.Faces[bottom].Triangles[1] = new ModelTriangle(4, 3, 7, model.Faces[bottom], 0, 2, 3);

            model.Vertices = new[]
            {
                new Vector3(-0.5f, -0.5f,  0.5f), // 0
                new Vector3(-0.5f, -0.5f, -0.5f), // 1
                new Vector3( 0.5f, -0.5f, -0.5f), // 2
                new Vector3( 0.5f, -0.5f,  0.5f), // 3
                new Vector3(-0.5f,  0.5f,  0.5f), // 4
                new Vector3(-0.5f,  0.5f, -0.5f), // 5
                new Vector3( 0.5f,  0.5f, -0.5f), // 6
                new Vector3( 0.5f,  0.5f,  0.5f), // 7
            };

            return model;
        }

        public static ModelPart CreateCuboid(string name, int materialId)
        {
            var cuboid = CreateCuboid();
            cuboid.Name = name;
            cuboid.MaterialId = materialId;
            return cuboid;
        }
    }
}