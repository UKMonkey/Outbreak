using Psy.Core.EpicModel;
using SlimMath;

namespace EpicEdit.Model.Factories
{
    public static class Plane
    {
         public static ModelPart CreatePlane(string name, int materialId)
         {
             var model = new ModelPart
             {
                 Faces = new ModelPartFace[1],
                 Name = name,
                 MaterialId = materialId
             };

             model.Faces[0] = ModelPartFace.CreateSquare(model, 0);

             model.Faces[0].VertexIndices = new[] { 0, 1, 2, 3 };
             model.Faces[0].Triangles[0] = new ModelTriangle(0, 1, 2, model.Faces[0], 0, 1, 2);
             model.Faces[0].Triangles[1] = new ModelTriangle(0, 2, 3, model.Faces[0], 0, 2, 3);

             model.Vertices = new[]
             {
                 new Vector3(-0.5f, 0, 0.5f), 
                 new Vector3(-0.5f, 0, -0.5f), 
                 new Vector3(0.5f, 0, -0.5f), 
                 new Vector3(0.5f, 0, 0.5f), 
             };

             return model;
         }
    }
}