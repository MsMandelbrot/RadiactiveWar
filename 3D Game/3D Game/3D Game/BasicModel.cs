using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3D_Game
{
    class BasicModel
    {
        public Model model { get; protected set; }
        protected Matrix world = Matrix.Identity;

        public BasicModel(Model m)
        {
            model = m;
        }

     
        
        public virtual void Update()
        {

        }

        public void Draw(Camera camera)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            //model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.EnableDefaultLighting();
                    be.Projection = camera.projection;
                    be.View = camera.view;
                    be.World =  mesh.ParentBone.Transform*GetWorld();//это строчка помогает при ошибке в разнице ориентации хны и 3д макса
                    //be.World = GetWorld()*mesh.ParentBone.Transform ;
                    //be.World = transforms[mesh.ParentBone.Index] * GetWorld();
                    //be.World = GetWorld();
                }

                mesh.Draw();

                // ... and then transform the BoundingSphere using that matrix.
                //BoundingSphere sphere = TransformBoundingSphere(mesh.BoundingSphere, world);
                // now draw the sphere with our renderer
                //BoundingSphereRenderer.Draw(sphere, camera.view, camera.projection);
            }


        }

        
        public virtual Matrix GetWorld()
        {
            return world;
        }

       /* BoundingSphere CreateBoundingSphereForModel(Model model, Matrix worldMatrix)
        {
            Matrix[] boneTransforms = new Matrix[this.model.Bones.Count];
            this.model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            BoundingSphere boundingSphere = new BoundingSphere();
            BoundingSphere meshSphere;

            for (int i = 0; i < model.Meshes.Count; i++)
            {
                meshSphere = model.Meshes[i].BoundingSphere.Transform(boneTransforms[i]);
                boundingSphere = BoundingSphere.CreateMerged(boundingSphere, meshSphere);
            }
            return boundingSphere.Transform(worldMatrix);
        }*/

        public bool CollidesWith(Model otherModel, Matrix otherWorld)
        {
            // Loop through each ModelMesh in both objects and compare
            // all bounding spheres for collisions
            //BoundingSphere bs1 = CreateBoundingSphereForModel(model1, world1);
           // BoundingSphere bs2 = CreateBoundingSphereForModel(model2, world2);
            foreach (ModelMesh myModelMeshes in model.Meshes)
            {
                foreach (ModelMesh hisModelMeshes in otherModel.Meshes)
                {
                    //if (myModelMeshes.BoundingSphere.Transform(
                    if (myModelMeshes.BoundingSphere.Transform(
                        GetWorld()).Intersects(
                        hisModelMeshes.BoundingSphere.Transform(otherWorld)))
                        return true;
                }
            }
            return false;
        }
        //Check intersection between two models
       /* public bool CollidesWith(Model model1, Matrix world1, Model model2, Matrix world2)
        {
            BoundingSphere bs1 = CreateBoundingSphereForModel(model1, world1);
            BoundingSphere bs2 = CreateBoundingSphereForModel(model2, world2);

            if (bs1.Intersects(bs2))
                return true;

            return false;
        }*/
    }
}
