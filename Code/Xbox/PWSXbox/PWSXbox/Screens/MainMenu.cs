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
using PWS.Graphics;
using PWS.Controls;

namespace PWS.Screens
{
    //Class containting the code for the main menu.
    class MainMenu
    {
        #region Variables & Properties
        //The buttongroup for all the buttons
        static ButtonGroup buttons;

        //background
        static Sprite background;

        //A variable for checking the previous
        static GamePadState prevState = new GamePadState();
        #endregion

        //Method to Initiliaze
        static public void Instantiate()
        {
            //Initialize the buttongroup
            buttons = new ButtonGroup();

            background = new Sprite();
        }

        static public void Initialize()
        {            
            //creating the text array for the buttons
            string[] texts = new string[5];
            texts[0] = "New Game!";
            texts[1] = "Shop!";
            texts[2] = "Customize!";
            texts[3] = "Credits!";
            texts[4] = "Quit! :(";

            //Initialize the buttongroup
            buttons.Initialize(texts, 200);

            //Initialize the background
            background.Initialize(Vector2.Zero);
        }

        static public void LoadContent(ContentManager content)
        {
            background.LoadContent(content.Load<Texture2D>("Graphics/Basic backgrounds/2"));
        }

        static public void Update()
        {
            //Creating variable for current state of the controller
            GamePadState state = GamePad.GetState(InfoPacket.Players[0]);

            //Update the variables
            background.Update();
            buttons.Update(0);

            //Checking for any button releases and doing what is necesarry after the button is released
            if (state.Buttons.A == ButtonState.Released && prevState.Buttons.A == ButtonState.Pressed)
            {
                buttons.Speed = 0;

                if (buttons.CurrentlySelect == 1)
                {
                    ScreenManager.ChangeToArenaSelection();
                }
                else if (buttons.CurrentlySelect == 4)
                {
                    ScreenManager.ChangeToSettingsScreen();
                }
                else if (buttons.CurrentlySelect == 5)
                {
                    InfoPacket.TheGame.Exit();
                }
            }

            for (int i = 0; i < 4; i++)
            {
                GamePadState state1 = GamePad.GetState(InfoPacket.Players[i]);

                if (state1.Buttons.A == ButtonState.Released && InfoPacket.PreviousStates[i].Buttons.A == ButtonState.Pressed)
                {
                    if (buttons.CurrentlySelect == 2)
                    {
                        ScreenManager.ChangeToShopScreen(i);
                        break;
                    }
                    else if (buttons.CurrentlySelect == 3)
                    {
                        ScreenManager.ChangeToCustomizeScreen(i);
                        break;
                    }
                }
            }

            //The prevState has to be set to the last known state, which is at the end of the method a perfect position
            prevState = state;
        }

        static public void Draw(SpriteBatch spriteBatch)
        {
            background.Draw(spriteBatch);
            buttons.Draw(spriteBatch);
        }

        //Method to be called when the game has to be reset
        static public void Reset()
        {
            
        }
    }
}
