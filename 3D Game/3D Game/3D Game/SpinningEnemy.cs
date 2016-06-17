using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace _3D_Game
{
    class SpinningEnemy : BasicModel
    {
        Matrix rotation = Matrix.Identity;

        // Rotation and movement variables
        float yawAngle = 0;
        float pitchAngle = 0;
        float rollAngle = 0;
        Vector3 direction;

        Vector3 position;

        public SpinningEnemy(Model m, Vector3 Position,
            Vector3 Direction, float yaw, float pitch, float roll)
            : base(m)
        {
            position = Position;
           
            world = Matrix.CreateTranslation(Position);
            yawAngle = yaw;
            pitchAngle = pitch;
            rollAngle = roll;
            direction = Direction;
        }

        public override void Update()
        {
            // Rotate model
            rotation *= Matrix.CreateFromYawPitchRoll(yawAngle,
                pitchAngle, rollAngle);

            // Move model
            world *= Matrix.CreateTranslation(direction);
            //world = Matrix.CreateTranslation(direction) * world;
            
                // Move model
           // if (direction.Y <= 60) world *= Matrix.CreateTranslation(direction); 
            
        }

        public override Matrix GetWorld()
        {
            return rotation * world;
        }
    }
}