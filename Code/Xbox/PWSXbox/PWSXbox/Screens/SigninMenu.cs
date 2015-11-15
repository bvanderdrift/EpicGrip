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

namespace PWS.Screens
{
    class SigninMenu
    {
        #region Variable & Properties
        //Background & Sprite for the background of sign in selection
        static Sprite background;
        static Sprite selectorBackground;

        //Gamer Profiles
        static SignedInGamer[] players;

        //Spritefont for the text
        static SpriteFont font;

        //Int variable for which players turn it is to log in
        static int nextLogin = 2;
        #endregion

        //Create an instance of the variables
        static public void Instantiate()
        {
            background = new Sprite();

            selectorBackground = new Sprite();
        }

        //Initialize the variables
        static public void Initialize()
        {
            background.Initialize(Vector2.Zero);

            selectorBackground.Initialize(new Vector2(290, 110));

            nextLogin = 2;
        }

        static public void LoadContent(ContentManager content)
        {
            background.LoadContent(content.Load<Texture2D>("Graphics/Basic backgrounds/3"));

            //Load texture for the selector background
            selectorBackground.LoadContent(content.Load<Texture2D>("Graphics/SigninMenu/Selector"));

            //Load the font
            font = content.Load<SpriteFont>("Graphics/SigninMenu/SigninFont");
        }

        static public void Update()
        {
            //Update the backgrounds
            background.Update();
            selectorBackground.Update();

            //Update the profiles
            players = InfoPacket.PlayerProfiles;

            #region Xbox Code
#if XBOX
            //Check every controller
            for (int i = 0; i < 4; i++)
            {
                if (GamePad.GetState((PlayerIndex)i).Buttons.A == ButtonState.Released &&
                    InfoPacket.PreviousStates[i].Buttons.A == ButtonState.Pressed &&
                    nextLogin > 2)
                {
                    ScreenManager.ChangeToMainMenu();
                }

                //Check if the current controller has the start button pressed
                if (GamePad.GetState((PlayerIndex)i).Buttons.Start == ButtonState.Pressed)
                {
                    
                    //Create a boolean to check if player is logged in already
                    bool falseLogin = false;

                    //Check if there are actually <nextLogin> number of people logged in
                    if (nextLogin <= SignedInGamer.SignedInGamers.Count)
                    {
                        //go past every logged in player to check if the controller actually is logged in
                        for (int j = 0; j < SignedInGamer.SignedInGamers.Count; j++)
                        {
                            //Is the controller signed into the xbox and not yet signed into the game?? Set everything up
                            //and stop checking!
                            if ((PlayerIndex)i == SignedInGamer.SignedInGamers[j].PlayerIndex)
                            {
                                //Check if the player has signed into the game yet
                                if (!InfoPacket.PlayerProfiles.Contains<SignedInGamer>(SignedInGamer.SignedInGamers[j]))
                                {
                                    InfoPacket.PlayerProfiles[nextLogin - 1] = SignedInGamer.SignedInGamers[j];
                                    InfoPacket.Players[nextLogin - 1] = (PlayerIndex)i;
                                    nextLogin++;

                                    InfoPacket.Load();
                                }

                                //This seems illogical, but it overrides any previous changes in the falseLogin boolean
                                falseLogin = false;
                                break;
                            }
                            //The controller has not signed in yet
                            else
                            {
                                falseLogin = true;
                            }
                        }
                    }
                    else
                    {
                        //Iterate through all the signed in gamers
                        for (int j = 0; j < SignedInGamer.SignedInGamers.Count; j++)
                        {
                            //Get the "j" value of the signed in gamer corresponding to the controller
                            if ((PlayerIndex)i == SignedInGamer.SignedInGamers[j].PlayerIndex)
                            {
                                //If the player has not signed into the game yet, let the player login.
                                if (!InfoPacket.PlayerProfiles.Contains<SignedInGamer>(SignedInGamer.SignedInGamers[j]))
                                {
                                    falseLogin = true;
                                    break;
                                }
                                //If the player has signed in to the game, do nothing
                                //but stop iterating trough the signed in gamers, since I found him already!
                                else
                                {
                                    falseLogin = false;
                                    break;
                                }
                            }
                            //if there is no signed profile for this controller
                            //the controller has nog signed in yet
                            else
                            {
                                falseLogin = true;
                            }
                        }
                    }
                    
                    //If the login is false, let the player sign in to the xbox first!
                    if (falseLogin)
                    {
                        Guide.ShowSignIn(1, false);
                    }
                }
            }
#endif
            #endregion

#if WINDOWS

#endif
        }

        static public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the backgrounds in the correct order
            background.Draw(spriteBatch);
            selectorBackground.Draw(spriteBatch);

            //Draw the strings
            #region Only Player one has signed in
            if (nextLogin == 2)
            {
                //Set up the strings
                string LineOne;
                string LineTwo;

                //Set up the vectors
                Vector2 LineOneDimensions;
                Vector2 LineTwoDimensions;

                #region Box One Text
                LineOne = "Welcome " + players[0].Gamertag + "!";
                LineTwo = "Press A to start!";
                LineOneDimensions = font.MeasureString(LineOne);
                LineTwoDimensions = font.MeasureString(LineTwo);

                spriteBatch.DrawString(font, LineOne, new Vector2(465 - LineOneDimensions.X / 2, 225 - .75f * LineOneDimensions.Y / 2), Color.White);
                spriteBatch.DrawString(font, LineTwo, new Vector2(465 - LineTwoDimensions.X / 2, 225 + .75f * LineTwoDimensions.Y / 2), Color.White);
                #endregion

                #region Box Two Text
                LineOne = "Player 2, please press start!";
                LineOneDimensions = font.MeasureString(LineOne);

                spriteBatch.DrawString(font, LineOne, new Vector2(815 - LineOneDimensions.X / 2, 225 - LineOneDimensions.Y / 2), Color.White);
                #endregion

                #region Box Three Text
                LineOne = "Player 3, please press start!";
                LineOneDimensions = font.MeasureString(LineOne);

                spriteBatch.DrawString(font, LineOne, new Vector2(465 - LineOneDimensions.X / 2, 495 - LineOneDimensions.Y / 2), Color.White);
                #endregion

                #region Box Four Text
                LineOne = "Player 4, please press start!";
                LineOneDimensions = font.MeasureString(LineOne);

                spriteBatch.DrawString(font, LineOne, new Vector2(815 - LineOneDimensions.X / 2, 495 - LineOneDimensions.Y / 2), Color.White);
                #endregion
            }
            #endregion

            #region Player 1 & Player 2 have signed in
            else if (nextLogin == 3)
            {
                //Set up the strings
                string LineOne;
                string LineTwo;

                //Set up the vectors
                Vector2 LineOneDimensions;
                Vector2 LineTwoDimensions;

                #region Box One Text
                LineOne = "Welcome " + players[0].Gamertag + "!";
                LineTwo = "Press A to start!";
                LineOneDimensions = font.MeasureString(LineOne);
                LineTwoDimensions = font.MeasureString(LineTwo);

                spriteBatch.DrawString(font, LineOne, new Vector2(465 - LineOneDimensions.X / 2, 225 - .75f * LineOneDimensions.Y / 2), Color.White);
                spriteBatch.DrawString(font, LineTwo, new Vector2(465 - LineTwoDimensions.X / 2, 225 + .75f * LineTwoDimensions.Y / 2), Color.White);
                #endregion

                #region Box Two Text
                LineOne = "Welcome " + players[1].Gamertag + "!";
                LineTwo = "Press A to start!";
                LineOneDimensions = font.MeasureString(LineOne);
                LineTwoDimensions = font.MeasureString(LineTwo);

                spriteBatch.DrawString(font, LineOne, new Vector2(815 - LineOneDimensions.X / 2, 225 - .75f * LineOneDimensions.Y / 2), Color.White);
                spriteBatch.DrawString(font, LineTwo, new Vector2(815 - LineTwoDimensions.X / 2, 225 + .75f * LineTwoDimensions.Y / 2), Color.White);
                #endregion

                #region Box Three Text
                LineOne = "Player 3, please press start!";
                LineOneDimensions = font.MeasureString(LineOne);

                spriteBatch.DrawString(font, LineOne, new Vector2(465 - LineOneDimensions.X / 2, 495 - LineOneDimensions.Y / 2), Color.White);
                #endregion

                #region Box Four Text
                LineOne = "Player 4, please press start!";
                LineOneDimensions = font.MeasureString(LineOne);

                spriteBatch.DrawString(font, LineOne, new Vector2(815 - LineOneDimensions.X / 2, 495 - LineOneDimensions.Y / 2), Color.White);
                #endregion
            }
            #endregion

            #region Player 1,2 & 3 have signed in
            else if (nextLogin == 4)
            {
                //Set up the strings
                string LineOne;
                string LineTwo;

                //Set up the vectors
                Vector2 LineOneDimensions;
                Vector2 LineTwoDimensions;

                #region Box One Text
                LineOne = "Welcome " + players[0].Gamertag + "!";
                LineTwo = "Press A to start!";
                LineOneDimensions = font.MeasureString(LineOne);
                LineTwoDimensions = font.MeasureString(LineTwo);

                spriteBatch.DrawString(font, LineOne, new Vector2(465 - LineOneDimensions.X / 2, 225 - .75f * LineOneDimensions.Y / 2), Color.White);
                spriteBatch.DrawString(font, LineTwo, new Vector2(465 - LineTwoDimensions.X / 2, 225 + .75f * LineTwoDimensions.Y / 2), Color.White);
                #endregion

                #region Box Two Text
                LineOne = "Welcome " + players[1].Gamertag + "!";
                LineTwo = "Press A to start!";
                LineOneDimensions = font.MeasureString(LineOne);
                LineTwoDimensions = font.MeasureString(LineTwo);

                spriteBatch.DrawString(font, LineOne, new Vector2(815 - LineOneDimensions.X / 2, 225 - .75f * LineOneDimensions.Y / 2), Color.White);
                spriteBatch.DrawString(font, LineTwo, new Vector2(815 - LineTwoDimensions.X / 2, 225 + .75f * LineTwoDimensions.Y / 2), Color.White);
                #endregion

                #region Box Three Text
                LineOne = "Welcome " + players[2].Gamertag + "!";
                LineTwo = "Press A to start!";
                LineOneDimensions = font.MeasureString(LineOne);
                LineTwoDimensions = font.MeasureString(LineTwo);

                spriteBatch.DrawString(font, LineOne, new Vector2(465 - LineOneDimensions.X / 2, 495 - .75f * LineOneDimensions.Y / 2), Color.White);
                spriteBatch.DrawString(font, LineTwo, new Vector2(465 - LineTwoDimensions.X / 2, 495 + .75f * LineTwoDimensions.Y / 2), Color.White);
                #endregion

                #region Box Four Text
                LineOne = "Player 4, please press start!";
                LineOneDimensions = font.MeasureString(LineOne);

                spriteBatch.DrawString(font, LineOne, new Vector2(815 - LineOneDimensions.X / 2, 495 - LineOneDimensions.Y / 2), Color.White);
                #endregion
            }
            #endregion

            #region All players have signed in
            else if (nextLogin == 5)
            {
                //Set up the strings
                string LineOne;
                string LineTwo;

                //Set up the vectors
                Vector2 LineOneDimensions;
                Vector2 LineTwoDimensions;

                #region Box One Text
                LineOne = "Welcome " + players[0].Gamertag + "!";
                LineTwo = "Press A to start!";
                LineOneDimensions = font.MeasureString(LineOne);
                LineTwoDimensions = font.MeasureString(LineTwo);

                spriteBatch.DrawString(font, LineOne, new Vector2(465 - LineOneDimensions.X / 2, 225 - .75f * LineOneDimensions.Y / 2), Color.White);
                spriteBatch.DrawString(font, LineTwo, new Vector2(465 - LineTwoDimensions.X / 2, 225 + .75f * LineTwoDimensions.Y / 2), Color.White);
                #endregion

                #region Box Two Text
                LineOne = "Welcome " + players[1].Gamertag + "!";
                LineTwo = "Press A to start!";
                LineOneDimensions = font.MeasureString(LineOne);
                LineTwoDimensions = font.MeasureString(LineTwo);

                spriteBatch.DrawString(font, LineOne, new Vector2(815 - LineOneDimensions.X / 2, 225 - .75f * LineOneDimensions.Y / 2), Color.White);
                spriteBatch.DrawString(font, LineTwo, new Vector2(815 - LineTwoDimensions.X / 2, 225 + .75f * LineTwoDimensions.Y / 2), Color.White);
                #endregion

                #region Box Three Text
                LineOne = "Welcome " + players[2].Gamertag + "!";
                LineTwo = "Press A to start!";
                LineOneDimensions = font.MeasureString(LineOne);
                LineTwoDimensions = font.MeasureString(LineTwo);

                spriteBatch.DrawString(font, LineOne, new Vector2(465 - LineOneDimensions.X / 2, 495 - .75f * LineOneDimensions.Y / 2), Color.White);
                spriteBatch.DrawString(font, LineTwo, new Vector2(465 - LineTwoDimensions.X / 2, 495 + .75f * LineTwoDimensions.Y / 2), Color.White);
                #endregion

                #region Box Four Text
                LineOne = "Welcome " + players[3].Gamertag + "!";
                LineTwo = "Press A to start!";
                LineOneDimensions = font.MeasureString(LineOne);
                LineTwoDimensions = font.MeasureString(LineTwo);

                spriteBatch.DrawString(font, LineOne, new Vector2(815 - LineOneDimensions.X / 2, 495 - .75f * LineOneDimensions.Y / 2), Color.White);
                spriteBatch.DrawString(font, LineTwo, new Vector2(815 - LineTwoDimensions.X / 2, 495 + .75f * LineTwoDimensions.Y / 2), Color.White);
                #endregion
            }
            #endregion
        }
    }
}
