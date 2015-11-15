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

namespace PWS
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameMain : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        InfoPacket InfoPacket;
        ChangeLoginPopup clPopup;

        Sprite brightness;

        public GameMain()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            InfoPacket.Initialize(new GameTime());

            //Instantiate the ChangeLoginPopup
            clPopup = new ChangeLoginPopup();

            //Add a GamerServicesComponent to be able to use Xbox Live
            this.Components.Add(new GamerServicesComponent(this));

            SignedInGamer.SignedOut += new EventHandler<SignedOutEventArgs>(SignedOut);

            //Instantiate eveything in the game
            ScreenManager.Instantiate();

            //Initiate the brightness
            brightness = new Sprite();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.ApplyChanges();

            //Initializes the screenmanager
            ScreenManager.Initialize();

            //Initialize the ChangeLoginPopup
            clPopup.Initialize(Popups.Type.Ok);

            //Let the music repeat, so there will be no moment without music
            MediaPlayer.IsRepeating = true;

            brightness.Initialize(Vector2.Zero);
            brightness.Color = new Color(0, 0, 0, 30);

            //Unlock all items to test everything
            for (int i = 0; i < 4; i++)
            {
                //InfoPacket.PlayerStatistics[i].UnlockAll();
            }

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Loads all the content for every screen
            ScreenManager.LoadContent(Content);

            //Load the content for the ChangeLoginPopup
            clPopup.LoadContent(Content);

            //Toggle full screen
            //graphics.ToggleFullScreen();

            brightness.LoadContent(Content.Load<Texture2D>("Graphics/Shade"));
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //updates the InfoPacket with the gametime and the game itself. THIS MUST ALWAYS BE THE FIRST COMMAND IN THE UPDATE METHOD
            InfoPacket.GameTime = gameTime;
            InfoPacket.TheGame = this;

            //Update everything except for the audio
            //Here are the changes to the InfoPacket made as well!
            if (!Guide.IsVisible
                && !clPopup.IsShowing)
            {
                ScreenManager.Update();
            }

            //Update the ChangedLoginPopup
            clPopup.Update();

            brightness.Update();

            //Muting the game
            //MediaPlayer.Volume = 0;

            //Set The PrevStates
            for (int i = 0; i < 4; i++)
            {
                InfoPacket.PreviousStates[i] = GamePad.GetState(InfoPacket.Players[i]);

                //Make the players able to delete their saves
                if (GamePad.GetState(InfoPacket.Players[i]).Buttons.Y == ButtonState.Pressed)
                {
                    InfoPacket.Delete(i);
                }
            }



            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //Clear the screen of any previously drawn stuff, 
            //creating a clear blue screen.
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            //Further drawing will be handled in the ScreenManager.
            ScreenManager.Draw(spriteBatch);

            //Draw the ChangedLoginPopup
            clPopup.Draw(spriteBatch);

            //brightness.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        //Event to be triggered when a player signs out of the game
        void SignedOut(object sender, SignedOutEventArgs e)
        {
            clPopup.Show("Oohooh!", "It seems a player has changed account!" +
                "\nYou will now be directed back to the start screen!", 0);
        }
    }
}
