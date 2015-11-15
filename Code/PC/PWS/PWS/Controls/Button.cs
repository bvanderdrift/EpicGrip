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

namespace PWS.Controls
{
    enum BTNState
    {
        Released,
        Selected,
        Pressed,
    }

    class Button
    {
        #region Variables and Properties
        //Variables and Properties
        static Texture2D background;

        Color releasedColour;
        Color selectedColour;
        Color pressedColour;
        Color currentColour;

        BTNState btnState;

        string text;

        static SpriteFont font;

        Vector2 position;

        //Click sound
        static SoundEffect buttonClick;

        /// <summary>
        /// A Property for the Released State Color
        /// </summary>
        public Color ReleasedColour
        {
            get { return releasedColour; }
            set { releasedColour = value; }
        }

        /// <summary>
        /// A Property for the Selected State Color
        /// </summary>
        public Color SelectedColour
        {
            get { return selectedColour; }
            set { selectedColour = value; }
        }

        /// <summary>
        /// A Property for the Pressed State Color
        /// </summary>
        public Color PressedColour
        {
            get { return pressedColour; }
            set { pressedColour = value; }
        }

        /// <summary>
        /// The state of the button (READ ONLY)
        /// </summary>
        public BTNState State
        {
            get { return btnState; }
        }

        /// <summary>
        /// The text shown on the button
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        #endregion

        public Button()
        {

        }

        public void Initialize(string text)
        {
            //Initialize all variables
            releasedColour = Color.White;
            selectedColour = Color.Green;
            pressedColour = Color.DarkGreen;
            currentColour = releasedColour;
            this.text = text;
            position = Vector2.Zero;
        }

        static public void LoadContent(ContentManager content)
        {
            //Load textures & fonts
            background=content.Load<Texture2D>("Graphics/Controls/Button");
            font = content.Load<SpriteFont>("Graphics/Controls/ButtonFont");

            //Load the click sound
            buttonClick = content.Load<SoundEffect>("Audio/Effects/Menu/Click");
        }

        public void Update(GameTime gameTime)
        {
            //Give the button the correct colour compared to the state of the button.
            if (btnState == BTNState.Released)
            {
                currentColour = releasedColour;
            }
            else if (btnState == BTNState.Selected)
            {
                currentColour = selectedColour;
            }
            else
            {
                currentColour = pressedColour;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the sprite
            spriteBatch.Draw(background, position, currentColour);

            //Draw the text on the sprite
            spriteBatch.DrawString(font, text, new Vector2(position.X + 256 / 2 - font.MeasureString(text).X / 2, position.Y + 64/2 - font.MeasureString(text).Y / 2 + 5), Color.Black);
        }

        public void Select()
        {
            btnState = BTNState.Selected;
        }

        public void Deselect()
        {
            btnState = BTNState.Released;
        }

        public void Press()
        {
            btnState = BTNState.Pressed;
            buttonClick.Play();
        }
    }
}
