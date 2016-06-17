using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3D_Game
{
 
    public struct VertexCube: IVertexType
    {
        //!!
        public Vector3 Position;

        public static VertexElement[] Elements = { new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0) };

        //constr
        public VertexCube(Vector3 pos)
        {
            Position = pos;
        }

        //!
        public static VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, 
            VertexElementUsage.Position, 0));

       
     
       VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }
       
    } 

    public class SkyBox : Microsoft.Xna.Framework.GameComponent
    {
        private static VertexCube[] CubeMdl = null;
        private static short[] CubeInd = null;

        //
        private VertexDeclaration CubeMdl1;

        public SkyBox(Game game)
            : base(game)
        {
            // generate the geometry...
                CubeMdl = new VertexCube[8];
                CubeInd = new short[6 * 6];

                CubeMdl[0] = new VertexCube(new Vector3(-512, -512, -512));
                CubeMdl[1] = new VertexCube(new Vector3(512, -512, -512));
                CubeMdl[2] = new VertexCube(new Vector3(-512, 512, -512));
                CubeMdl[3] = new VertexCube(new Vector3(512, 512, -512));

                CubeMdl[4] = new VertexCube(new Vector3(-512, -512, 512));
                CubeMdl[5] = new VertexCube(new Vector3(512, -512, 512));
                CubeMdl[6] = new VertexCube(new Vector3(-512, 512, 512));
                CubeMdl[7] = new VertexCube(new Vector3(512, 512, 512));

                //change
               
                CubeMdl1 = new VertexDeclaration(VertexCube.VertexDeclaration.GetVertexElements());
                            
                CubeInd[0] = 0;
                CubeInd[1] = 2;
                CubeInd[2] = 3;
                CubeInd[3] = 0;
                CubeInd[4] = 3;
                CubeInd[5] = 1;

                CubeInd[6] = 1;
                CubeInd[7] = 3;
                CubeInd[8] = 7;
                CubeInd[9] = 1;
                CubeInd[10] = 7;
                CubeInd[11] = 5;

                CubeInd[12] = 4;
                CubeInd[13] = 7;
                CubeInd[14] = 6;
                CubeInd[15] = 4;
                CubeInd[16] = 5;
                CubeInd[17] = 7;

                CubeInd[18] = 4;
                CubeInd[19] = 6;
                CubeInd[20] = 2;
                CubeInd[21] = 4;
                CubeInd[22] = 2;
                CubeInd[23] = 0;

                CubeInd[24] = 2;
                CubeInd[25] = 6;
                CubeInd[26] = 7;
                CubeInd[27] = 2;
                CubeInd[28] = 7;
                CubeInd[29] = 3;

                CubeInd[30] = 0;
                CubeInd[31] = 5;
                CubeInd[32] = 4;
                CubeInd[33] = 0;
                CubeInd[34] = 1;
                CubeInd[35] = 5;

        }

         public void Draw()
        {
            VertexBuffer vb = new VertexBuffer(Global.Graphics, typeof(VertexCube), CubeMdl.Length, BufferUsage.None);
            Global.Graphics.SetVertexBuffer(vb); 
            Global.Graphics.DrawUserIndexedPrimitives<VertexCube>(PrimitiveType.TriangleList, CubeMdl, 0, 8, CubeInd, 0, 12);

        }
    };
}
