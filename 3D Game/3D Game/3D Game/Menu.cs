using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace _3D_Game
{
    class Menu
    {
        // Закрытые поля-ссылки на рисунки
        //Texture2D menuTexture;  // Заставка меню
        public Texture2D buttonStart;   // Кнопка входа в игру
        public Texture2D buttonExit;   // Кнопка выхода из приложения
        public Texture2D buttonHelp;   // Кнопка о программе

        // Закрытые поля-ссылки на координаты
        Vector2 /*menuPosition, */ buttonStartPosition, buttonExitPosition, buttonHelpPosition;
        // Свойства
        public Vector2 ButtonStartPosition
        {
            get { return buttonStartPosition; }
            set { buttonStartPosition = value; }
        }

        public Vector2 ButtonAboutPosition
        {
            get { return buttonHelpPosition; }
            set { buttonHelpPosition = value; }
        }

        public Vector2 ButtonExitPosition
        {
            get { return buttonExitPosition; }
            set { buttonExitPosition = value; }
        }
        
        // Конструктор
        public Menu()
        {
            //menuPosition = new Vector2(0, 0);
            buttonStartPosition = new Vector2(600, 73);
            buttonHelpPosition = new Vector2(550, 171);
            buttonExitPosition = new Vector2(550, 294);
        }
    
        // Загрузка рисунков
        public void Load(ContentManager content)
        {
            //menuTexture = content.Load<Texture2D>("images\\menu");
            buttonStart = content.Load<Texture2D>("images\\start");
            buttonHelp = content.Load<Texture2D>("images\\help");
            buttonExit = content.Load<Texture2D>("images\\esc");
        }
    
        // Вывод на экран
        public void DrawMenu(SpriteBatch spriteBatch, int buttonState)
        {
            //spriteBatch.Draw(menuTexture, menuPosition, Color.White);
    
            switch (buttonState)
            {
                case 1:
                    spriteBatch.Draw(buttonStart, buttonStartPosition, Color.Yellow);
                    spriteBatch.Draw(buttonHelp, buttonHelpPosition, Color.White);
                    spriteBatch.Draw(buttonExit, buttonExitPosition, Color.White);
                    break;
                case 2:
                    spriteBatch.Draw(buttonStart, buttonStartPosition, Color.White);
                    spriteBatch.Draw(buttonHelp, buttonHelpPosition, Color.Yellow);
                    spriteBatch.Draw(buttonExit, buttonExitPosition, Color.White);
                    break;

                case 3:
                    spriteBatch.Draw(buttonStart, buttonStartPosition, Color.White);
                    spriteBatch.Draw(buttonHelp, buttonHelpPosition, Color.White);
                    spriteBatch.Draw(buttonExit, buttonExitPosition, Color.Yellow);
                    break;
            }
        }
    }
}
