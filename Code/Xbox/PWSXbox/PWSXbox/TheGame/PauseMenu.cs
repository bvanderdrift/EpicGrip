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
using PWS.Controls;
using PWS.Screens;
using PWS.Popups;
using PWS.Arenas;
using PWS.Graphics;
using PWS.Geometrics;

namespace PWS.TheGame
{
    class PauseMenu
    {
        static Color selectedColour;
        static int user;
        static int selected;
        static SpriteFont font;

        static string[] texts;

        static bool startedPress;

        public static void Instantiate()
        {
            texts = new string[2];
        }

        public static void Initialize()
        {
            texts[0] = "Continue";
            texts[1] = "Quit";

            startedPress = false;
        }

        public static void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("Graphics/InGameGraphics/PauseMenuFont");
        }

        public static void Update()
        {
            //Get the gamepadstate of the current user
            GamePadState controller = GamePad.GetState(InfoPacket.Players[user]);

            #region Code to scroll through the options
            if (controller.ThumbSticks.Left.Y > .5f && InfoPacket.PreviousStates[user].ThumbSticks.Left.Y <= .5f)
            {
                selected--;
            }

            if (controller.ThumbSticks.Left.Y < -.5f && InfoPacket.PreviousStates[user].ThumbSticks.Left.Y >= -.5f)
            {
                selected++;
            }

            if (selected > texts.Length - 1)
            {
                selected = 0;
            }

            if (selected < 0)
            {
                selected = texts.Length - 1;
            }
            #endregion

            //Check if a player selected 

            //Make the colour blackness on a sinus wave controlled by the elapsed gametime
            float colourValue = 205 + 50 * (float)Math.Sin(InfoPacket.GameTime.TotalGameTime.TotalMilliseconds / 100);
            selectedColour = new Color((byte)colourValue, (byte)colourValue, (byte)colourValue, 255);

            //If the player starts a press, keep that in mind, because the player can stop pressing, while the press was started for driving
            if ((controller.Buttons.B == ButtonState.Pressed && InfoPacket.PreviousStates[user].Buttons.B == ButtonState.Released) ||
                (controller.Buttons.A == ButtonState.Pressed && InfoPacket.PreviousStates[user].Buttons.A == ButtonState.Released))
            {
                startedPress = true;
            }

            if (controller.Buttons.B == ButtonState.Released && InfoPacket.PreviousStates[user].Buttons.B == ButtonState.Pressed && startedPress)
            {
                GameEngine.Continue();
            }

            if (controller.Buttons.A == ButtonState.Released && InfoPacket.PreviousStates[user].Buttons.A == ButtonState.Pressed && startedPress)
            {
                if (selected == 0)
                {
                    GameEngine.Continue();
                }
                else if (selected == 1)
                {
                    ScreenManager.ChangeToMainMenu();
                    GameEngine.CurrentArena.Close();
                    GameEngine.Continue();
                }
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < texts.Length; i++)
            {
                if (i == selected)
                {
                    spriteBatch.DrawString(font, texts[i], new Vector2(640, 360) - font.MeasureString(texts[i]) / 2 + new Vector2(0, 0 - 200 + 100 * i), selectedColour);
                }
                else
                {
                    spriteBatch.DrawString(font, texts[i], new Vector2(640, 360) - font.MeasureString(texts[i]) / 2 + new Vector2(0, 0 - 200 + 100 * i), Color.White);
                }
            }
        }

        public static void Open(int user)
        {
            PauseMenu.user = user;
            startedPress = false;
        }
    }
}
