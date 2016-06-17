using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace _3D_Game
{
    /// <summary>
    /// A basic free-camera class
    /// </summary>
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        //Camera matrices
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }

        // Camera vectors
        public Vector3 cameraPosition { get; protected set; }
        Vector3 cameraDirection;
        Vector3 cameraUp;
        // the rotation values
        private float pitch, yaw;

        
        public enum ControlType
        {
              MouseKeyboard,
        }

        public Vector3 GetCameraDirection
        {

            get { return cameraDirection; }
        }

        public Vector3 GetYaw
        {

            get { return Vector3.Transform(new Vector3(1, 0, 0), Matrix.CreateRotationZ(pitch) * Matrix.CreateRotationY(yaw)); }
        }
 
        public ControlType CurrentController { get; set; }

        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up)
            : base(game)
    
        {
            cameraPosition = pos;
            cameraDirection = target - pos;
            cameraDirection.Normalize();
            cameraUp = up;
            
            CreateLookAt();
            //GetViewMatrix(); ;

            CurrentController = ControlType.MouseKeyboard;

            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                (float)Game.Window.ClientBounds.Width /
                (float)Game.Window.ClientBounds.Height,
                1, 1000);

        }

        public override void Update(GameTime gameTime)
        {
                        
                // query the devices
                MouseState ms = Mouse.GetState();
              
                // move based on how far off the mouse is from screen center
                yaw -= (ms.X - (Global.ScreenWidth / 2)) * 0.1f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                pitch -= (ms.Y - (Global.ScreenHeight / 2)) * 0.1f * (float)gameTime.ElapsedGameTime.TotalSeconds;

                // reset the mouse to screen center
                Mouse.SetPosition(Global.ScreenWidth / 2, Global.ScreenHeight / 2);

                // generate the forward and right vectors
                // from the rotation values
                Vector3 fwd = Vector3.Transform(new Vector3(1, 0, 0), Matrix.CreateRotationZ(pitch) * Matrix.CreateRotationY(yaw));
                Vector3 rgt = Vector3.Transform(new Vector3(0, 0, 1), Matrix.CreateRotationZ(pitch) * Matrix.CreateRotationY(yaw));
                
            // Recreate the camera view matrix
                CreateLookAt();
                //GetViewMatrix();            
        }

        private void CreateLookAt()
        {
            Vector3 fwd = Vector3.Transform(new Vector3(1, 0, 0), Matrix.CreateRotationZ(pitch) * Matrix.CreateRotationY(yaw));
            view = Matrix.CreateLookAt(cameraPosition,
                //cameraPosition + cameraDirection, cameraUp);
                cameraPosition + fwd, cameraUp);

                    }

       /* public Matrix GetViewMatrix()
        {
            Vector3 fwd = Vector3.Transform(new Vector3(1, 0, 0), Matrix.CreateRotationZ(pitch) * Matrix.CreateRotationY(yaw));
            return Matrix.CreateLookAt(cameraPosition, cameraPosition + fwd, Vector3.Up);
        }*/
    }
}
