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
using PWS.Graphics;

namespace PWS.Popups
{
    enum Type
    {
        Ok,
        NoOk,
    }

    class Popup
    {
        #region Variables & Properties
        //The textst to show in the popup
        protected string heading;
        protected string message;

        //Boolean for knowing if the popup should be drawn and if the popup is ready to end
        protected bool showing;
        bool answer;

        //Sprite for the actual background
        Sprite popupBgOk;
        Sprite popupBgNoOk;

        //Font for the texts
        SpriteFont headingFont;
        SpriteFont messageFont;

        //Shadow of controllerstate
        GamePadState prevState;

        //A variable to check if the popup has not just closed
        bool justClosed;

        //Variable to decide what type the current popup is
        Type type;

        //Integer to represent the sender of the popup
        int sender;

        //Property to get if a popup is showing
        public bool IsShowing
        {
            get { return showing; }
        }

        public bool JustClosed
        {
            get { return justClosed; }
        }

        public bool Answer
        {
            get { return answer; }
        }
        #endregion

        public Popup()
        {
            //Instantiate the sprite
            popupBgOk = new Sprite();
            popupBgNoOk = new Sprite();

            //Initialize the prevState to prevent errors in first update run
            prevState = new GamePadState();

        }

        public void Initialize(Type type)
        {
            //Initialize the basic variables
            heading = "";
            message = "";
            showing = false;
            justClosed = false;
            answer = false;

            //Initialize the sprite
            popupBgOk.Initialize(Vector2.Zero); 
            popupBgNoOk.Initialize(Vector2.Zero);

            //Set the type
            this.type = type;

            //Initialize the sender
            sender = 0;
        }

        public void LoadContent(ContentManager content)
        {
            //Load the background & font
            popupBgOk.LoadContent(content.Load<Texture2D>("Graphics/Popup/PopupOk"));
            popupBgNoOk.LoadContent(content.Load<Texture2D>("Graphics/Popup/PopupNoOk"));
            messageFont = content.Load<SpriteFont>("Graphics/Popup/MessageFont");
            headingFont = content.Load<SpriteFont>("Graphics/Popup/HeadingFont");
        }

        public void Update()
        {
            justClosed = false;
            GamePadState p1State = GamePad.GetState(InfoPacket.Players[sender]);
            popupBgOk.Update();
            popupBgNoOk.Update();

            if (showing && type == Type.Ok)
            {
                if (p1State.Buttons.A == ButtonState.Released &&
                    prevState.Buttons.A == ButtonState.Pressed)
                {
                    EndPopup();
                    InfoPacket.PopupShowing = false;
                }
            }

            if (showing && type == Type.NoOk)
            {
                if (p1State.Buttons.A == ButtonState.Released &&
                    prevState.Buttons.A == ButtonState.Pressed)
                {
                    EndPopup();
                    InfoPacket.PopupShowing = false;
                    answer = true;
                }

                if (p1State.Buttons.B == ButtonState.Released &&
                    prevState.Buttons.B == ButtonState.Pressed)
                {
                    EndPopup();
                    InfoPacket.PopupShowing = false;
                    answer = false;
                }
            }

            //Create shadow of the state
            prevState = p1State;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the background and the texts if the popup is showing
            if (showing)
            {
                if (type == Type.Ok)
                {
                    popupBgOk.Draw(spriteBatch);
                }
                else if (type == Type.NoOk)
                {
                    popupBgNoOk.Draw(spriteBatch);
                }

                spriteBatch.DrawString(headingFont, heading, new Vector2(350, 180), Color.White);
                spriteBatch.DrawString(messageFont, message, new Vector2(350, 280), Color.White);
            }
        }

        public virtual void Show(string heading, string message, int sender)
        {
            if (!justClosed && !showing)
            {
                //Set the heading & message
                this.heading = heading;
                this.message = message;

                //Set the game info to a state where the popup is showing
                InfoPacket.PopupShowing = true;
                showing = true;

                //Set the sender
                this.sender = sender;
            }
        }

        protected virtual void EndPopup()
        {
            showing = false;
            justClosed = true;
        }
    }
}
