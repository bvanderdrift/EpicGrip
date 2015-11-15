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
using PWS.Arenas;
using PWS.Popups;

namespace PWS.Screens
{
    class ArenaSelection
    {
        //Declare the sprites
        static Sprite background;
        static Sprite frame;
        static Sprite infoBlock;
        static Sprite locked;
        static Sprite aButton;
        static Sprite bButton;
        static Sprite arrowLeft;
        static Sprite arrowRight;

        //The Arena's for information
        static WoodenArena woodenArena;
        static MetallicArena metallicArena;
        static CoushonArena coushonArena;
        static HellArena hellArena;
        static ScaryArena scaryArena;
        static AngelArena angelArena;
        static NightClubArena ncArena;

        //Which arena is showing? (number)
        static int showing;

        //Array for arenas
        static Arena[] arenas;

        //Font to use
        static SpriteFont font;

        //Number of total arenas
        static int numberOfArenas;

        //A popup for when the arena is locked
        static Popup arenaLockedMsg;

        //A bool to check if just opened
        static bool justOpened;

        static public bool JustOpened
        {
            get { return justOpened; }
            set { justOpened = value; }
        }

        //Method for instantiation, since this is a static variable it has no constructor
        static public void Instantiate()
        {
            //Instantiate the sprites
            background = new Sprite();
            frame = new Sprite();
            infoBlock = new Sprite();
            aButton = new Sprite();
            bButton = new Sprite();
            arrowLeft = new Sprite();
            arrowRight = new Sprite();
            locked = new Sprite();

            //Instantiating the arenas array
            arenas = new Arena[7];

            //Instantiate the popup
            arenaLockedMsg = new Popup();
        }

        static public void Initialize()
        {
            //Initialize the sprites
            background.Initialize(Vector2.Zero);
            frame.Initialize(new Vector2(560, 262));
            infoBlock.Initialize(new Vector2(25, 120));
            locked.Initialize(new Vector2(888, 460));
            aButton.Initialize(new Vector2(50, 546));
            bButton.Initialize(new Vector2(50, 610));
            arrowLeft.Initialize(new Vector2(1050, 260));
            arrowRight.Initialize(new Vector2(1180, 260));

            //Initializing the showing variable
            showing = 0;

            //Initialize the popup
            arenaLockedMsg.Initialize(Popups.Type.Ok);

            #region Initialize the arenas
            //Initialize the arenas
            woodenArena = new WoodenArena();
            woodenArena.Initialize();

            metallicArena = new MetallicArena();
            metallicArena.Initialize();

            coushonArena = new CoushonArena();
            coushonArena.Initialize();

            hellArena = new HellArena();
            hellArena.Initialize();

            scaryArena = new ScaryArena();
            scaryArena.Initialize();

            angelArena = new AngelArena();
            angelArena.Initialize();

            ncArena = new NightClubArena();
            ncArena.Initialize();
            #endregion
        }

        static public void LoadContent(ContentManager content)
        {
            //Load Background
            background.LoadContent(content.Load<Texture2D>("Graphics/Basic backgrounds/4"));

            //Load the frame
            frame.LoadContent(content.Load<Texture2D>("Graphics/ChooseArena/PreviewFrame"));

            //Load the infoblock
            infoBlock.LoadContent(content.Load<Texture2D>("Graphics/ChooseArena/InfoBlock"));

            //Load the lock
            locked.LoadContent(content.Load<Texture2D>("Graphics/ChooseArena/Locked"));

            //Load the A&B Button texture
            aButton.LoadContent(content.Load<Texture2D>("Graphics/XboxButtons/64/facebutton_a"));
            bButton.LoadContent(content.Load<Texture2D>("Graphics/XboxButtons/64/facebutton_b"));

            //Load the arrows for scrolling
            arrowLeft.LoadContent(content.Load<Texture2D>("Graphics/ChooseArena/ArrowLeft"));
            arrowRight.LoadContent(content.Load<Texture2D>("Graphics/ChooseArena/ArrowRight"));

            //Load the content for the popup
            arenaLockedMsg.LoadContent(content);

            #region load content of arenas
            woodenArena.LoadContent(content);
            metallicArena.LoadContent(content);
            coushonArena.LoadContent(content);
            hellArena.LoadContent(content);
            scaryArena.LoadContent(content);
            angelArena.LoadContent(content);
            ncArena.LoadContent(content);
            #endregion

            #region set the arenas array up
            arenas[0] = woodenArena;
            arenas[1] = metallicArena;
            arenas[2] = coushonArena;
            arenas[3] = hellArena;
            arenas[4] = scaryArena;
            arenas[5] = angelArena;
            arenas[6] = ncArena;
            #endregion

            font = content.Load<SpriteFont>("Graphics/ChooseArena/ChooseArenaFont");
        }

        static public void Update()
        {
            GamePadState state = GamePad.GetState(InfoPacket.Players[0]);

            //Return to main menu when B is pressed
            if (state.Buttons.B == ButtonState.Released && InfoPacket.PreviousStates[0].Buttons.B == ButtonState.Pressed)
            {
                ScreenManager.ChangeToMainMenu();
            }

            //A code to scroll up, but not go higher than the max. nr of arenas
            if (InfoPacket.PreviousStates[0].ThumbSticks.Left.X < .5f &&
                state.ThumbSticks.Left.X > .5f &&
                showing < InfoPacket.AmountOfArenas - 1)
            {
                showing++;
            }

            //A code to scroll down, but not go lower than the first arena
            if (InfoPacket.PreviousStates[0].ThumbSticks.Left.X > -.5f &&
                state.ThumbSticks.Left.X < -.5f &&
                showing > 0)
            {
                showing--;
            }

            //Change the arrow colour to a "available (red)" or "unavailable (gray)" colour
            if (showing == 0)
            {
                arrowLeft.Color = Color.Gray;
            }
            else
            {
                arrowLeft.Color = Color.Green;
            }

            if (showing == InfoPacket.AmountOfArenas - 1)
            {
                arrowRight.Color = Color.Gray;
            }
            else
            {
                arrowRight.Color = Color.Green;
            }

            #region Updating variables & objects
            //Update Variables
            numberOfArenas = InfoPacket.AmountOfArenas;

            //Update Sprites
            background.Update();
            frame.Update();
            infoBlock.Update();
            locked.Update();
            aButton.Update();
            bButton.Update();
            arrowLeft.Update();
            arrowRight.Update();

            //Update the Popup
            arenaLockedMsg.Update();

            //Update the currently showing arena
            arenas[showing].Update(); 
            #endregion

            if (!arenaLockedMsg.IsShowing && !justOpened)
            {
                //Make the locked sprite visible if the arena is still locked
                locked.Visible = !InfoPacket.PlayerStatistics[0].HasArena[showing];

                //Check if the player wants to choose the current arena
                if (state.Buttons.A == ButtonState.Released &&
                    InfoPacket.PreviousStates[0].Buttons.A == ButtonState.Pressed)
                {
                    if (!InfoPacket.PlayerStatistics[0].HasArena[showing])
                    {
                        arenaLockedMsg.Show("Oops!", "Sorry, it seems you have not yet unlocked this arena!\nTo unlock ths arena, buy it in the shop!", 0);
                        ShopScreen.PlayError();
                    }
                    else
                    {
                        ScreenManager.ChangeToPlayScreen(arenas[showing]);
                    }
                }
            }

            //Set justopened to false
            justOpened = false;
        }

        static public void Draw(SpriteBatch spriteBatch)
        {
            background.Draw(spriteBatch);

            Texture2D currentTexture = arenas[showing].Background.Texture;

            //Graphics for preview
            spriteBatch.Draw(currentTexture, new Vector2(600, 320), currentTexture.Bounds, Color.White, 0, Vector2.Zero, new Vector2(0.5f), SpriteEffects.None, 1);
            frame.Draw(spriteBatch);
            locked.Draw(spriteBatch);

            //Graphics for text area
            infoBlock.Draw(spriteBatch);
            spriteBatch.DrawString(font, "Name: \"" + arenas[showing].Name + "\"" + "\n" +
                "Bounciness: " + (arenas[showing].Bounciness * 100) + "%" + "\n" +
                "Available : ", new Vector2(50, 200), Color.Black);

            //Show Yes or No depening on if the arena is unlocked yet
            if (!InfoPacket.PlayerStatistics[0].HasArena[showing])
            {
                spriteBatch.DrawString(font, "No", new Vector2(271, 313), Color.Red);
            }
            else
            {
                spriteBatch.DrawString(font, "Yes!", new Vector2(271, 313), Color.ForestGreen);
            }
            
            //Graphics for "Continue"
            spriteBatch.DrawString(font, "Choose Arena!", new Vector2(114, 560), Color.Black);
            aButton.Draw(spriteBatch);
            spriteBatch.DrawString(font, "Back!", new Vector2(114, 624), Color.Black);
            bButton.Draw(spriteBatch);

            //Graphics for Arena Count
            arrowLeft.Draw(spriteBatch);
            spriteBatch.DrawString(font, (showing + 1).ToString(), new Vector2(1105, 270), Color.Black);
            spriteBatch.DrawString(font, "/" + numberOfArenas, new Vector2(1135, 270), Color.Black);
            arrowRight.Draw(spriteBatch);

            //Draw the popup (if showing ofcourse)
            arenaLockedMsg.Draw(spriteBatch);
        }
    }
}
