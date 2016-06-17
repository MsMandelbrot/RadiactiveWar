using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace _3D_Game
{
    public class ModelManager : DrawableGameComponent
    {
        // List of models
        List<BasicModel> models = new List<BasicModel>();

        SpriteBatch spriteBatch;

        Texture2D crosshairTexture;

        BasicModel boat;
        // Spawn variables

        Vector3 maxSpawnLocation = new Vector3(500, 150, 500);//было 150
        int nextSpawnTime = 0;
        int timeSinceLastSpawn = 0;
        float maxRollAngle = MathHelper.Pi / 40;

        public int currentScore = 0;
        public int currentLost=0;

        // Enemy count
        int enemiesThisLevel = 0;

        // Misses variables
        //int missedThisLevel = 0;


        // Current level
        public int currentLevel = 0;
        public int lost = 0;


        // List of LevelInfo objects
        List<LevelInfo> levelInfoList = new List<LevelInfo>(  );

        // Shot stuff
        List<BasicModel> shots = new List<BasicModel>(  );
        float shotMinZo = -2000;
        float shotMinZp = 2000;

        //Explosion stuff
        List<ParticleExplosion> explosions = new List<ParticleExplosion>();
        ParticleExplosionSettings particleExplosionSettings = new ParticleExplosionSettings();
        ParticleSettings particleSettings = new ParticleSettings();
        Texture2D explosionTexture;
        Texture2D explosionColorsTexture;
        Effect explosionEffect;

        
        public ModelManager(Game game)
            : base(game)
        {
            // Initialize game levels
            levelInfoList.Add(new LevelInfo(2000, 4000, 150, 1, 2, 2));
            //levelInfoList.Add(new LevelInfo(900, 2800, 22, 2, 4, 9));
            
        }

        public override void Initialize()
        {
            // Set initial spawn time
            SetNextSpawnTime();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // Load explosion textures and effect
            explosionTexture = Game.Content.Load<Texture2D>(@"Textures\Particle");
            explosionColorsTexture = Game.Content.Load<Texture2D>(@"Textures\ParticleColors");
            explosionEffect = Game.Content.Load<Effect>(@"effects\particle");

            // Set effect parameters that don't change per particle
            explosionEffect.CurrentTechnique = explosionEffect.Techniques["Technique1"];
            explosionEffect.Parameters["theTexture"].SetValue(explosionTexture);

            //boat = new BasicModel(Game.Content.Load<Model>(@"models\boat"));
            boat = new SpinningEnemy(
                Game.Content.Load<Model>(@"models\boat"),
                 new Vector3(0, 0, 0), new Vector3 (0,0,1), 0, 0, 0);

            // Load the crosshair texture
            crosshairTexture = Game.Content.Load<Texture2D>(@"textures\crosshair");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // Check to see if it's time to spawn
            CheckToSpawnEnemy(gameTime);

            //boat.Update();

            // Update models
            UpdateModels();

            // Update shots
            UpdateShots();

            // Update explosions
            UpdateExplosions(gameTime);

            base.Update(gameTime);
        }

        protected void UpdateModels()
        {
            // Loop through all models and call Update
            for (int i = 0; i < models.Count; ++i)
            {
                // Update each model
                if (models[i].GetWorld().Translation.Y <= -110)//!!!!!!!!!!!!!!!!тормозим бочки
                {
                    models[i].Update();
                } 
                //models[i].Update();
                // Remove models that are out of bounds
                /*if (models[i].GetWorld().Translation.Z >
                    ((Game1)Game).camera.cameraPosition.Z + 100)
                {
                    models.RemoveAt(i);
                    --i;
                    ((Game1)Game).AddLost(1); //можно поменять позаморочнее
                }*/

              
            }
        }

        public override void Draw(GameTime gameTime)
        {

            boat.Draw(((Game1)Game).camera);

            // Loop through and draw each model
            foreach (BasicModel bm in models)
            {
                bm.Draw(((Game1)Game).camera);
            }

            // Loop through and draw each shot
            foreach (BasicModel bm in shots)
            {
                bm.Draw(((Game1)Game).camera);
            }

            // Loop through and draw each particle explosion
            foreach (ParticleExplosion pe in explosions)
            {
                pe.Draw(((Game1)Game).camera);
            }

            spriteBatch.Begin();

             spriteBatch.Draw(crosshairTexture,
                 new Vector2((900 / 2)
                     - (crosshairTexture.Width / 2),
                     (600 / 2)
                     - (crosshairTexture.Height / 2)),
                     Color.White);
             spriteBatch.End();
             //
             GraphicsDevice.DepthStencilState = DepthStencilState.Default;
             Global.Graphics.Clear(ClearOptions.DepthBuffer, Color.Blue, 1.0f, 0);
           
            base.Draw(gameTime);
        }

        private void SetNextSpawnTime()
        {
            // Reset the variables to indicate the next enemy spawn time
            nextSpawnTime = ((Game1)Game).rnd.Next(
                levelInfoList[currentLevel].minSpawnTime,
                levelInfoList[currentLevel].maxSpawnTime);
            timeSinceLastSpawn = 0;
        }

        protected void CheckToSpawnEnemy(GameTime gameTime)
        {
            // Time to spawn a new enemy?
            if (enemiesThisLevel <
                levelInfoList[currentLevel].numberEnemies)
            {
                timeSinceLastSpawn += gameTime.ElapsedGameTime.Milliseconds;
                if (timeSinceLastSpawn > nextSpawnTime)
                {
                    SpawnEnemy();
                }
            }
        }

        private void SpawnEnemy()
        {

            // Generate random position with random X and random Y
            // between -maxX and maxX and -maxY and maxY. Z is always
            // the same for all ships.
            Vector3 position = new Vector3(((Game1)Game).rnd.Next(
                -(int)maxSpawnLocation.X, (int)maxSpawnLocation.X),
                -maxSpawnLocation.Y,((Game1)Game).rnd.Next(
                -(int)maxSpawnLocation.Z, (int)maxSpawnLocation.Z));

            // Direction will always be (0, 0, Z), where
            // Z is a random value between minSpeed and maxSpeed
            Vector3 direction = new Vector3(0, ((Game1)Game).rnd.Next(levelInfoList[currentLevel].minSpeed,
                levelInfoList[currentLevel].maxSpeed), 0);

            // Get a random roll rotation between -maxRollAngle and maxRollAngle
            /*float rollRotation = (float)((Game1)Game).rnd.NextDouble() *
                    maxRollAngle - (maxRollAngle / 2);*/

            // Add model to the list
            models.Add(new SpinningEnemy(
                Game.Content.Load<Model>(@"models\супербочка"),
                //Game.Content.Load<Model>(@"models\spaceship"),
                 position, direction, 0, 0, 0));
           

            currentLost++;//можно тоже поменять позаморочнее
          

            // Increment # of enemies this level and set next spawn time
            ++enemiesThisLevel;
            SetNextSpawnTime();
        }

        public void AddShot(Vector3 position, Vector3 direction)
        {
            shots.Add(new SpinningEnemy(
                Game.Content.Load<Model>(@"models\ammo"),
                position, direction, 0, 0, 0));
        }

        protected void UpdateShots()
        {
            // Loop through shots
            for (int i = 0; i < shots.Count; ++i)
            {
                // Update each shot
                shots[i].Update();

                // If shot is out of bounds, remove it from game
                if ((shots[i].GetWorld().Translation.Z < shotMinZo) || (shots[i].GetWorld().Translation.Z > shotMinZp))
                {
                    shots.RemoveAt(i);
                    --i;
                }
                else
                {
                    // If shot is still in play, check for collisions
                    for (int j = 0; j < models.Count; ++j)
                    {
                       // if (shots[i].CollidesWith(models[j].model,
                        //    models[j].GetWorld()))
                        if (shots[i].CollidesWith(models[j].model,
                          models[j].GetWorld()))
                        {
                            // Collision! add an explosion.
                            explosions.Add(new ParticleExplosion(GraphicsDevice,
                                models[j].GetWorld().Translation,
                                 ((Game1)Game).rnd.Next(
                                    particleExplosionSettings.minLife,
                                    particleExplosionSettings.maxLife),
                                ((Game1)Game).rnd.Next(
                                    particleExplosionSettings.minRoundTime,
                                    particleExplosionSettings.maxRoundTime),
                                ((Game1)Game).rnd.Next(
                                    particleExplosionSettings.minParticlesPerRound,
                                    particleExplosionSettings.maxParticlesPerRound),
                                ((Game1)Game).rnd.Next(
                                    particleExplosionSettings.minParticles,
                                    particleExplosionSettings.maxParticles),
                                explosionColorsTexture, particleSettings,
                                explosionEffect));

                            // Collision! Remove the ship and the shot.
                            currentScore++; //можно поменять позаморочнее
                            currentLost--;
                            //lost--;
                           // ((Game1)Game).AddLost(-1);//можно тоже поменять позаморочнее
                            models.RemoveAt(j);
                            --j;//
                            shots.RemoveAt(i);
                            --i;
                            //
                            ((Game1)Game).PlayCue("Explosions");
                            break;
                        }
                    }
                }
            }
        }

        protected void UpdateExplosions(GameTime gameTime)
        {
            // Loop through and update explosions
            for (int i = 0; i < explosions.Count; ++i)
            {
                explosions[i].Update(gameTime);
                // If explosion is finished, remove it
                if (explosions[i].IsDead)
                {
                    explosions.RemoveAt(i);
                    --i;
                }
            }
        }

        public int missesLeft
        {
            get
            {
                return
                    levelInfoList[currentLevel].missesAllowed;
            }
        }

            public int Points
        {
            get
            {
            return currentScore;
                }
        }

        public int Lost
        {
            get
            {
                return currentLost;
            }
        }

       
    }
}
