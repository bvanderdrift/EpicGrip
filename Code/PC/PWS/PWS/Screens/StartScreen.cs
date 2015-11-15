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
using PWS.Graphics;

namespace PWS.Screens
{
    //First screen shown when the game starts
    //This screen is for the player to descide what controller controls player one
    class StartScreen
    {
        static Button startButton;
        static Sprite background;

        //Instantiate the variables
        static public void Instantiate()
        {
            startButton = new Button();

            background = new Sprite();
        }

        //Initialize the variables
        static public void Initialize()
        {
            startButton.Initialize("Press Start!");
            startButton.Position = new Vector2(1280 / 2 - 128, 550);

            background.Initialize(Vector2.Zero);
        }

        static public void LoadContent(ContentManager content)
        {
            background.LoadContent(content.Load<Texture2D>("Graphics/Basic backgrounds/1"));
        }

        static public void Update()
        {
            //Update the Button
            startButton.Update(InfoPacket.GameTime);

            //Update the Sprite
            background.Update();

            //Scan all the controllers for a pressed start button
            for (int i = 0; i < 4; i++)
            {
                if (GamePad.GetState((PlayerIndex)i).Buttons.Start == ButtonState.Pressed)
                {
                    InfoPacket.Players[0] = (PlayerIndex)i;

                    bool hasLoggedIn = false;
                    for (int j = 0; j < SignedInGamer.SignedInGamers.Count; j++)
                    {
                        //Check if this controller is already signed in
                        if ((PlayerIndex)i == SignedInGamer.SignedInGamers[j].PlayerIndex)
                        {
                            InfoPacket.PlayerProfiles[0] = SignedInGamer.SignedInGamers[j];
                            InfoPacket.GetDevice();
                            hasLoggedIn = true;
                            break;
                        }
                        //If not, force the controller to log in
                        else
                        {
                            hasLoggedIn = false;
                        }
                    }

                    if (!Guide.IsVisible && !hasLoggedIn)
                    {
                        Guide.ShowSignIn(1, false);
                    }
                }
            }

            if (GamePad.GetState(InfoPacket.Players[0]).Buttons.Start == ButtonState.Pressed &&
                InfoPacket.StorageDevice != null &&
                InfoPacket.PlayerProfiles[0] != null)
            {
                ScreenManager.ChangeToMainMenu();
                InfoPacket.Load();
            }
        }

        static public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the background first
            background.Draw(spriteBatch);

            //Draw the button
            startButton.Draw(spriteBatch);
        }
    }
}
