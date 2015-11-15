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
    class CreditsMenu
    {
        //Sprite containing the background
        static Sprite background;

        //String containing the credits
        static string credits;

        //Font
        static SpriteFont font;

        static public void Instantiate()
        {
            //Instantiate the background
            background = new Sprite();
        }

        static public void Initialize()
        {
            //Initialize the background
            background.Initialize(Vector2.Zero);

            //Initialize the credits
            credits = "Music:" + "\n" +
                "- Climb to Alera by Dan-O at Danosongs.com (Wooden Arena Theme)" + "\n" +
                "- Remember the Dreams by MachinimaSound.com (Metallic Arena Theme)" + "\n" +
                "- Happy Alley by Kevin MacLeod (incompetech.com) (Coushon Arena Theme)" + "\n" +
                "- The Land of the Wizard by MachinimaSound.com (Hell Arena Theme)" + "\n" +
                "- The Hyperborean Menance by MachinimaSound.com (Scary Arena Theme)" + "\n" +
                "- Winter Dawn by MachinimaSound.com (Angel Arena Theme)" + "\n" +
                "- Skirt Shaker by MachinimaSound.com (Night Club Arena Theme)" + "\n" +
                "\n" +
                "Sound Effects:" + "\n" +
                "- Menu Click by MDS96 (http://sandbox.yoyogames.com/users/MDS96)" + "\n" +
                "- Explosion Sound by Mike Koenig at SoundBible.com" + "\n" +
                "- Wooden Wall Collision sound by Mike Koenig at SoundBible.com" + "\n" +
                "- Metallic Wall Collision sound by Caroline Ford at SoundBible.com" + "\n" +
                "\n" +
                "Graphics:" + "\n" +
                "- XNA Button Pack 3 By Jeff Jenkins at Sinnex.net" + "\n" +
                "- Satin Stitch Font By HaleysComet at dafont.com" + "\n" +
                "- Rest of the graphics by Beer van der Drift";
        }

        static public void LoadContent(ContentManager content)
        {
            //Load the background
            background.LoadContent(content.Load<Texture2D>("Graphics/Basic Backgrounds/7"));

            //Load the font
            font = content.Load<SpriteFont>("Graphics/Credits/CreditsFont");
        }

        static public void Update()
        {
            GamePadState state = GamePad.GetState(InfoPacket.Players[0]);

            //Return to main menu when B is pressed
            if (state.Buttons.B == ButtonState.Released && InfoPacket.PreviousStates[0].Buttons.B == ButtonState.Pressed)
            {
                ScreenManager.ChangeToMainMenu();
            }

            //Update the background
            background.Update();
        }

        static public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the background
            background.Draw(spriteBatch);

            //Draw the credits
            spriteBatch.DrawString(font, credits, new Vector2(100, 100), new Color(0, 255, 0));
        }
    }
}
