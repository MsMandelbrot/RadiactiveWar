using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace _3D_Game
{
  public class Ocean : Microsoft.Xna.Framework.GameComponent
    {
        // the ocean's required content
        private Effect oceanEffect;
        private Texture2D[] OceanNormalMaps;

        // The two-triangle generated model for the ocean
        private VertexPositionNormalTexture[] OceanVerts;
        private VertexDeclaration OceanVD;

   
        public Ocean(Game game)
            : base(game)
        {

        }

        public void Load(ContentManager Content)
        {

            // load the shader
            oceanEffect = Content.Load<Effect>(@"effects\OceanShader");
            // load the normal maps
            OceanNormalMaps = new Texture2D[4];
            for (int i = 0; i < 4; i++)
            OceanNormalMaps[i] = Content.Load<Texture2D>(@"textures\Ocean" + (i + 1) + "_N");

            // generate the geometry
            OceanVerts = new VertexPositionNormalTexture[6];
            OceanVerts[0] = new VertexPositionNormalTexture(new Vector3(-1000, 0, -1000), Vector3.Up, new Vector2(0, 0));
            OceanVerts[1] = new VertexPositionNormalTexture(new Vector3( 1000, 0, -1000), Vector3.Up, new Vector2(1, 0));
            OceanVerts[2] = new VertexPositionNormalTexture(new Vector3(-1000, 0,  1000), Vector3.Up, new Vector2(0, 1));
            OceanVerts[3] = OceanVerts[2];
            OceanVerts[4] = OceanVerts[1];
            OceanVerts[5] = new VertexPositionNormalTexture(new Vector3( 1000, 0,  1000), Vector3.Up, new Vector2(1, 1));
            OceanVD = new VertexDeclaration(VertexPositionTexture.VertexDeclaration.GetVertexElements()); 
        }

        public void Draw(GameTime gameTime, Camera cam, TextureCube skyTexture, Matrix proj)
        {
            // start the shader
            oceanEffect.CurrentTechnique.Passes[0].Apply();

            // set the transforms
            oceanEffect.Parameters["World"].SetValue(Matrix.Identity);
            oceanEffect.Parameters["View"].SetValue(cam.view);
            oceanEffect.Parameters["Projection"].SetValue(proj);
            oceanEffect.Parameters["EyePos"].SetValue(cam.cameraPosition);

            // choose and set the ocean textures
            int oceanTexIndex = ((int)(gameTime.TotalGameTime.TotalSeconds) % 4);
            oceanEffect.Parameters["ColorMap"].SetValue(OceanNormalMaps[(oceanTexIndex + 1) % 4]);
            oceanEffect.Parameters["NormalMap"].SetValue(OceanNormalMaps[(oceanTexIndex) % 4]);
            
            oceanEffect.Parameters["textureLerp"].SetValue((((((float)gameTime.TotalGameTime.TotalSeconds) - (int)(gameTime.TotalGameTime.TotalSeconds)) * 2 - 1) * 0.5f) + 0.5f);

            // set the time used for moving waves
            oceanEffect.Parameters["TotalTime"].SetValue((float)gameTime.TotalGameTime.TotalSeconds * 0.02f);

            // set the sky texture
            oceanEffect.Parameters["cubeTex"].SetValue(skyTexture);

            oceanEffect.CurrentTechnique.Passes[0].Apply();

            // draw our geometry

            VertexBuffer vb = new VertexBuffer(Global.Graphics, typeof(VertexPositionTexture), OceanVerts.Length, BufferUsage.None);
            Global.Graphics.SetVertexBuffer(vb); 

            Global.Graphics.DrawUserPrimitives(PrimitiveType.TriangleList, OceanVerts,  0, 2);

            // and we're done!
           
        }
    }
}
