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

using System.Xml.Serialization;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Storage;

namespace _3D_Game
{
      public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Model stuff
        ModelManager modelManager;

        // Camera
        public Camera camera;

        private Ocean ocean;

        private SkyBox skyBox;
        private Effect skyboxEffect;
        private TextureCube skyTex;
        //----------------------------------
        Texture2D background;

       
        //public int currentScore = 0;
        //public int currentLost=0;
        //public int lost = 0;

        // Random
        public Random rnd { get; protected set; }

        // Shot variables
        float shotSpeed = 10;
        int shotDelay = 300;
        int shotCountdown = 0;

        SpriteFont scoreFont;
        SpriteFont Font1;

        // Crosshair
        Texture2D crosshairTexture;


        // Audio
        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;
        Cue trackCue;

        // состояния
        public enum GameState { Start, InGame, GameOver, Menu, Help };
        public GameState currentGameState = GameState.Start;

        KeyboardState keyboardState;
        //MouseState prevMouseState;

        private bool paused = false;
        private bool pauseKeyDown = false;

        public bool newLevel = false;

        // Для управления меню
        Menu menu = new Menu();
        int buttonState = 1;
        public float elapsedMilliseconds = 100;
        public float elapsedMillisecondsState = 300;

        public readonly string HighScoresFilename = "highscores.dat";
        //
        [Serializable]
        public struct HighScoreData
        {
            public string[] PlayerName;
            public int[] Score;
            //public int[] Level;

            public int Count;

            public HighScoreData(int count)
            {
                PlayerName = new string[count];
                Score = new int[count];

                Count = count;
            }
        }

        IAsyncResult result = null;
        bool inputName;
        HighScoreData data;
        int Score = 0;
        public string NAME;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            rnd = new Random();

            // Set preferred resolution
            graphics.PreferredBackBufferWidth = 900;
            graphics.PreferredBackBufferHeight = 600;
        }


        protected override void Initialize()
        {
            Global.Graphics = graphics.GraphicsDevice;

            //graphics.PreferredBackBufferWidth = 640;
            //graphics.PreferredBackBufferHeight = 480;
            //graphics.ApplyChanges();

            // Initialize model manager
           
                modelManager = new ModelManager(this);
                Components.Add(modelManager);
                modelManager.Enabled = false;
                modelManager.Visible = false;

                skyBox = new SkyBox(this);
                Components.Add(skyBox);
               
                ocean = new Ocean(this);
                Components.Add(ocean);
               
                camera = new Camera(this, new Vector3(0, 5, 0), Vector3.Zero, Vector3.Up);
                Components.Add(camera);
                
        //
            // Get the path of the save game
            string fullpath = HighScoresFilename;


            // Check to see if the save exists
            if (!File.Exists(fullpath))
            {
                //If the file doesn't exist, make a fake one...
                // Create the data to save
                HighScoreData data = new HighScoreData(5);

                data.PlayerName[0] = " ";
                data.Score[0] = 0;

                data.PlayerName[1] = " ";
                data.Score[1] = 0;

                data.PlayerName[2] = " ";
                data.Score[2] = 0;

                data.PlayerName[3] = " ";
                data.Score[3] = 0;

                data.PlayerName[4] = " ";
                data.Score[4] = 0;

                SaveHighScores(data, HighScoresFilename);
            }
            //

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // create our three game objects
            
            // let the ocean load its stuff
            ocean.Load(Content);

            // load some skybox content
            skyboxEffect = Content.Load<Effect>(@"effects\SkyShader");
            skyTex = Content.Load<TextureCube>(@"textures\Sky");

            // Load the crosshair texture
            crosshairTexture = Content.Load<Texture2D>(@"textures\crosshair");

            // Load sounds and play initial sounds
            audioEngine = new AudioEngine(@"Content\Audio\GameAudio.xgs");
            waveBank = new WaveBank(audioEngine, @"Content\Audio\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Audio\Sound Bank.xsb");
            
            // Play the soundtrack
            trackCue = soundBank.GetCue("Track1");
            trackCue.Play();

            background = Content.Load<Texture2D>("images\\background");

            scoreFont = Content.Load<SpriteFont>("fonts\\score");
            //score = Content.Load<SpriteFont>("fonts\\score");
            Font1 = Content.Load<SpriteFont>("fonts\\score");

            // Загрузка спрайтов меню
            menu.Load(this.Content);
        }

        protected override void UnloadContent()
        {
         
        }

        protected override void Update(GameTime gameTime)
        {

            switch (currentGameState)
            {

                case GameState.Start:

                  
                    modelManager.Enabled = false;
                    modelManager.Visible = false;
                    keyboardState = Keyboard.GetState();

                    // Stop the soundtrack loop
                    trackCue.Stop(AudioStopOptions.Immediate);

                    // Отслеживаем управление клавиатурой

                    elapsedMilliseconds -= gameTime.ElapsedGameTime.Milliseconds;

                    if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
                        if (elapsedMilliseconds <= 0)
                            {
                                if (buttonState == 1)
                                {
                                    buttonState = 2;

                                    menu.ButtonStartPosition = new Vector2(550, 73);
                                    menu.ButtonAboutPosition = new Vector2(600, 171);
                                    menu.ButtonExitPosition = new Vector2(550, 294);
                                   
                                }
                                
                                else if (buttonState == 2 )
                                {
                                    buttonState = 3;

                                    menu.ButtonStartPosition = new Vector2(550, 73);
                                    menu.ButtonAboutPosition = new Vector2(550, 171);
                                    menu.ButtonExitPosition = new Vector2(600, 294);
                                }
                                elapsedMilliseconds = 300;
                             
                            }
                            

                            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
                            if (elapsedMilliseconds <= 0)

                                {
                                    if (buttonState == 3)
                                    {
                                        buttonState = 2;


                                        menu.ButtonStartPosition = new Vector2(550, 73);
                                        menu.ButtonAboutPosition = new Vector2(600, 171);
                                        menu.ButtonExitPosition = new Vector2(550, 294);
                                    } 
                                    else if (buttonState==2)
                                    
                                    {
                                        buttonState = 1;


                                        menu.ButtonStartPosition = new Vector2(600, 73);
                                        menu.ButtonAboutPosition = new Vector2(550, 171);
                                        menu.ButtonExitPosition = new Vector2(550, 294);
                                    }
                                    elapsedMilliseconds = 300;


                                }
                    
                            
                    // Распознаем клавишу Enter для новой игры
                    if (elapsedMilliseconds <= 0)
                    if (buttonState == 1 && keyboardState.IsKeyDown(Keys.Enter))
                    {
                        elapsedMilliseconds = 500;
                        currentGameState = GameState.InGame;

                    }

                    else if (buttonState == 2 && keyboardState.IsKeyDown(Keys.Enter))
                    {
                        elapsedMilliseconds = 500;
                        currentGameState = GameState.Help;
                        buttonState = 1;
                    }
                    // Распознаем клавишу Enter для завершения приложения
                    else if (buttonState == 3 && keyboardState.IsKeyDown(Keys.Enter))
                    {
                        this.Exit();
                    }


                    break;

    case GameState.InGame:

            
            modelManager.Enabled = true;
            modelManager.Visible = true;
            keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape))
                currentGameState = GameState.Menu;

            if (trackCue.IsPlaying)
                trackCue.Stop(AudioStopOptions.Immediate);

            // To play a stopped cue, get the cue from the soundbank again
            trackCue = soundBank.GetCue("Track1");
            trackCue.Play();

            Pause();
            if (paused == false)
        {

            camera.Update(gameTime);

            // See if the player has fired a shot
            FireShots(gameTime);

            if (modelManager.Lost > modelManager.missesLeft) currentGameState = GameState.GameOver;//ЗАКАНЧИВАЕТСЯ ИГРА РАСКОММЕНТИТЬ
            }

            break; 

        case GameState.GameOver:

            modelManager.Enabled = false;
            modelManager.Visible = false;
            //InputYourName();
            SaveHighScore();
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                Exit();

            
            if (Keyboard.GetState().IsKeyDown(Keys.Delete))
            {
                DeleteHighScore();
                Exit();
            }
            // Stop the soundtrack loop
            trackCue.Stop(AudioStopOptions.Immediate);

            
            break;

        case GameState.Menu:

            modelManager.Enabled = false;
            modelManager.Visible = false;
            keyboardState = Keyboard.GetState();

            // Stop the soundtrack loop
            trackCue.Stop(AudioStopOptions.Immediate);

            // Отслеживаем управление клавиатурой

            elapsedMilliseconds -= gameTime.ElapsedGameTime.Milliseconds;

            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
                if (elapsedMilliseconds <= 0)
                {
                    if (buttonState == 1)
                    {
                        buttonState = 2;

                        menu.ButtonStartPosition = new Vector2(550, 73);
                        menu.ButtonAboutPosition = new Vector2(600, 171);
                        menu.ButtonExitPosition = new Vector2(550, 294);

                    }

                    else if (buttonState == 2)
                    {
                        buttonState = 3;

                        menu.ButtonStartPosition = new Vector2(550, 73);
                        menu.ButtonAboutPosition = new Vector2(550, 171);
                        menu.ButtonExitPosition = new Vector2(600, 294);
                    }
                    elapsedMilliseconds = 300;

                }


            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
                if (elapsedMilliseconds <= 0)
                {
                    if (buttonState == 3)
                    {
                        buttonState = 2;


                        menu.ButtonStartPosition = new Vector2(550, 73);
                        menu.ButtonAboutPosition = new Vector2(600, 171);
                        menu.ButtonExitPosition = new Vector2(550, 294);
                    }
                    else if (buttonState == 2)
                    {
                        buttonState = 1;


                        menu.ButtonStartPosition = new Vector2(600, 73);
                        menu.ButtonAboutPosition = new Vector2(550, 171);
                        menu.ButtonExitPosition = new Vector2(550, 294);
                    }
                    elapsedMilliseconds = 300;


                }


            // Распознаем клавишу Enter для новой игры
            if (elapsedMilliseconds <= 0)
                if (buttonState == 1 && keyboardState.IsKeyDown(Keys.Enter))
                {
                    elapsedMilliseconds = 500;
                    currentGameState = GameState.InGame;

                }

                else if (buttonState == 2 && keyboardState.IsKeyDown(Keys.Enter))
                {
                    elapsedMilliseconds = 500;
                    currentGameState = GameState.Help;
                    buttonState = 1;
                }
                // Распознаем клавишу Enter для завершения приложения
                else if (buttonState == 3 && keyboardState.IsKeyDown(Keys.Enter))
                {
                    this.Exit();
                }

            break;

        case GameState.Help:

            modelManager.Enabled = false;
            modelManager.Visible = false;
            keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape))
                currentGameState = GameState.Start;
            // Stop the soundtrack loop
            trackCue.Stop(AudioStopOptions.Immediate);

            break;

            }

            base.Update(gameTime);
        }

 
        protected override void Draw(GameTime gameTime)
        {
            switch (currentGameState)
            {
                   
    case GameState.Start:

            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            spriteBatch.Draw(background, new Rectangle(0, 0, Window.ClientBounds.Width,
                Window.ClientBounds.Height), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);

            
            menu.DrawMenu(spriteBatch, buttonState); // Рисует меню 

             spriteBatch.End();
             break;

    case GameState.InGame:

            GraphicsDevice.Clear(Color.Blue);
//
          // create a standard projection matrix
          //Matrix proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Global.ScreenWidth / (float)Global.ScreenHeight, 1.0f, 1000.0f);

            // the sky is never culled and should not write to the depth buffer
                     
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            skyboxEffect.CurrentTechnique.Passes[0].Apply();

            // set the sky shader variables
            skyboxEffect.Parameters["View"].SetValue(camera.view);
            skyboxEffect.Parameters["Projection"].SetValue(camera.projection);
            skyboxEffect.Parameters["cubeTex"].SetValue(skyTex);
            skyboxEffect.CurrentTechnique.Passes[0].Apply();

            // and draw
            skyBox.Draw();

            // turn depth buffering back on for the ocean
           GraphicsDevice.DepthStencilState = DepthStencilState.Default;
           Global.Graphics.Clear(ClearOptions.DepthBuffer, Color.Blue, 1.0f, 0);

            // and allow the ocean to draw itself
            ocean.Draw(gameTime, camera, skyTex, camera.projection);
                    
            spriteBatch.Begin();

           /* spriteBatch.Draw(crosshairTexture,
                new Vector2((Window.ClientBounds.Width / 2)
                    - (crosshairTexture.Width / 2),
                    (Window.ClientBounds.Height / 2)
                    - (crosshairTexture.Height / 2)),
                    Color.White);*/

            // Draw the current score
            string scoreText = "Score: " + modelManager.Points;
            spriteBatch.DrawString(scoreFont, scoreText,
                new Vector2(10, 10), Color.Black);

            // Let the player know how many misses he has left
            spriteBatch.DrawString(scoreFont, "Now " +
                modelManager.Lost + " barrels in ocean ",
                new Vector2(10, scoreFont.MeasureString(scoreText).Y + 20),
                Color.Black);

            spriteBatch.End();
//
           GraphicsDevice.DepthStencilState = DepthStencilState.Default;
           Global.Graphics.Clear(ClearOptions.DepthBuffer, Color.Blue, 1.0f, 0);

                    
//
break;

case GameState.GameOver:

            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            spriteBatch.Draw(background, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), 
                                null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            
            string gameover = "Game Over!";
            spriteBatch.DrawString(scoreFont, gameover,
                new Vector2((Window.ClientBounds.Width / 2)
                - (scoreFont.MeasureString(gameover).X / 2),
                50), Color.Yellow);

            gameover = "Your score: " + (modelManager.Points);
            spriteBatch.DrawString(scoreFont, gameover,
                new Vector2((Window.ClientBounds.Width / 2)
                - (scoreFont.MeasureString(gameover).X / 2),
                70), Color.Yellow);

            gameover = "Your lost score: " + (modelManager.Lost);
            spriteBatch.DrawString(scoreFont, gameover,
                new Vector2((Window.ClientBounds.Width / 2)
                - (scoreFont.MeasureString(gameover).X / 2),
                90), Color.Yellow);

            gameover = "Press Enter for exit";
            spriteBatch.DrawString(scoreFont, gameover,
                new Vector2((Window.ClientBounds.Width / 2)
                - (scoreFont.MeasureString(gameover).X / 2),
                110), Color.Yellow);

            gameover = "Press Delete for delete High Score";
            spriteBatch.DrawString(scoreFont, gameover,
                new Vector2((Window.ClientBounds.Width / 2)
                - (scoreFont.MeasureString(gameover).X / 2),
                130), Color.Yellow);
            
            spriteBatch.DrawString(Font1, "" + makeHighScoreString(), new Vector2((Window.ClientBounds.Width / 2)
                - (Font1.MeasureString(gameover).X / 2), 150), Color.Yellow);


            spriteBatch.End();
            break;

case GameState.Menu:

            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            spriteBatch.Draw(background, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            menu.DrawMenu(spriteBatch, buttonState); // Рисует меню 

            spriteBatch.End();
            break;

case GameState.Help:
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            spriteBatch.Draw(background, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height),
                                null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);


            string help = "Author: Moiseenko Anastasia";
            spriteBatch.DrawString(scoreFont, help,
                new Vector2((Window.ClientBounds.Width / 2)
                - (scoreFont.MeasureString(help).X / 2),
                100),
                Color.Yellow);

            help = "Press Escape for Exit to Menu";
            spriteBatch.DrawString(scoreFont, help,
                new Vector2((Window.ClientBounds.Width / 2)
                - (scoreFont.MeasureString(help).X / 2),
                130),
                Color.Yellow);


            spriteBatch.End();
            break;

            }

            base.Draw(gameTime);

            
        }

        protected void FireShots(GameTime gameTime)
        {
            if (shotCountdown <= 0)
            {
                // Did player press space bar or left mouse button?
                if (Keyboard.GetState().IsKeyDown(Keys.Space) ||
                    Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    // Add a shot to the model manager
                    modelManager.AddShot(
                        camera.cameraPosition + new Vector3(0, -5, 0),
                        camera.GetYaw * shotSpeed);

                  
                    // Play shot audio
                    PlayCue("Shot");

                    // Reset the shot countdown
                    shotCountdown = shotDelay;
                }
            }
            else
                shotCountdown -= gameTime.ElapsedGameTime.Milliseconds;
        }

        public void PlayCue(string cue)
        {
            soundBank.PlayCue(cue);
        }

        public static void SaveHighScores(HighScoreData data, string filename)
        {
            // Get the path of the save game
            string fullpath = filename;
            
            //foreach(string f in )


            // Open the file, creating it if necessary
            FileStream stream = File.Open(fullpath, FileMode.OpenOrCreate);
            try
            {
                // Convert the object to XML data and put it in the stream
                XmlSerializer serializer = new XmlSerializer(typeof(HighScoreData));
                serializer.Serialize(stream, data);
            }
            finally
            {
                // Close the file
                stream.Close();
            }
        }

        

        public static HighScoreData LoadHighScores(string filename)
        {
            HighScoreData data;

            // Get the path of the save game
            string fullpath = filename;

            // Open the file
            FileStream stream = File.Open(fullpath, FileMode.OpenOrCreate, FileAccess.Read);
            try
            {

                // Read the data from the file
                XmlSerializer serializer = new XmlSerializer(typeof(HighScoreData));
                data = (HighScoreData)serializer.Deserialize(stream);
            }
            finally
            {
                // Close the file
                stream.Close();
            }

            return (data);
        }

        private void DeleteHighScore()
        {
            string sourceDir = @"D:\топорная супер мега нано гейм\3D Game\3D Game\3D Game\bin\x86\Debug";
            try
            {
                string[] datList = Directory.GetFiles(sourceDir, "*.dat");
                foreach (string f in datList)
                    File.Delete(f);
            }
            catch (DirectoryNotFoundException dirNotFound)
            {
                Console.WriteLine(dirNotFound.Message);

            }
            
        }
        private void SaveHighScore()
        {
            // Create the data to save
            HighScoreData data = LoadHighScores(HighScoresFilename);

            int scoreIndex = -1;
            for (int i = 0; i < data.Count; i++)
            {
                if ((modelManager.Points) > data.Score[i])
                {
                    scoreIndex = i;
                    break;
                }
            }

            if (scoreIndex > -1)
            {
                //New high score found ... do swaps
                for (int i = data.Count - 1; i > scoreIndex; i--)
                {
                    data.PlayerName[i] = data.PlayerName[i - 1];
                    data.Score[i] = data.Score[i - 1];
                }

                data.PlayerName[scoreIndex] = "Your record"; //Retrieve User Name Here
                data.Score[scoreIndex] = modelManager.Points;

                SaveHighScores(data, HighScoresFilename);
            }
        }

        /* Iterate through data if highscore is called and make the string to be saved*/
        public string makeHighScoreString()
        {
            // Create the data to save
            HighScoreData data2 = LoadHighScores(HighScoresFilename);

            // Create scoreBoardString
            string scoreBoardString = "Highscores:\n\n";


            for (int i = 0; i < 5; i++)
            {
                scoreBoardString = scoreBoardString + data2.PlayerName[i] + "-" + data2.Score[i] + "\n";
            }
            return scoreBoardString;
        }


        public void InputYourName()
        {
            if (result == null && !Guide.IsVisible)
            {

                string title = "Name";
                string description = "Write your name in order to save your Score";
                string defaultText = "Petr";
                PlayerIndex playerIndex = new PlayerIndex();
                result = Guide.BeginShowKeyboardInput(playerIndex, title, description, defaultText, null, null);
                NAME = result.ToString();

            }

            if (result != null && result.IsCompleted)
            {
                NAME = Guide.EndShowKeyboardInput(result);
                result = null;
                inputName = false;
                SaveHighScore();
            }

        }

        //собственно пауза
        public void Pause()
        {
            if (keyboardState.IsKeyDown(Keys.P))
            {
                pauseKeyDown = true;
            }
            else if (pauseKeyDown)
            {
                pauseKeyDown = false;
                paused = !paused;
            }
        }

        public void ChangeGameState(GameState state, int level)
        {
            currentGameState = state;
        }

        
    }
}
