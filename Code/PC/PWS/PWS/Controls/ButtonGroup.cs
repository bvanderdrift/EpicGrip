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

namespace PWS.Controls
{
    class ButtonGroup
    {
        #region Variables & Properties
        Button[] buttons;

        float[] positions;
        int currentlySelected;
        int buttonCount;

        string[] texts;

        float percentageMoved;
        float spacing;
        float ySpeed;
        const float maxSpeed = 16;
        float acceleration;
        float maxAcceleration;
        float xPosition;

        bool moving;
        bool movingUp;

        //Extra buttons for the buttons coming in from top/bottom
        Button extraButtonBottem;
        Button extraButtonTop;

        //Properties
        public int CurrentlySelect
        {
            get { return currentlySelected; }
        }

        public float Speed
        {
            get { return ySpeed; }
            set { ySpeed = value; }
        }
        #endregion

        public ButtonGroup()
        {
            
        }

        /// <summary>
        /// Initialize the ButtonGroup
        /// </summary>
        public void Initialize(string[] texts, int xPosition)
        {
            //Instantiate all the buttons
            buttons = new Button[texts.Length];
            for (int i = 0; i < texts.Length; i++)
            {
                buttons[i] = new Button();
            }
            extraButtonBottem = new Button();
            extraButtonTop = new Button();

            //Initialize the currently selected button
            currentlySelected = 1;

            //Save the number of buttons in "buttonCount" variable
            buttonCount = texts.Length;

            //set the position on the x-axis for the buttons.
            this.xPosition = xPosition;

            ySpeed = 0;
            maxAcceleration = 3 / (float)buttonCount;

            //First the method checks if the amount of strings in the texts array is the same as the amount of buttons in the ButtonGroup
            if (texts.Length != buttonCount)
            {
                throw new Exception("The amount of strings in the \"texts\" variable is not the same as the amount of buttons in this group!");
            }

            //Initialize the arrays to the amount of buttons.
            positions = new float[buttonCount];
            this.texts = new string[buttonCount];
            float middlePosition = buttonCount / 2;

            //This code will repeat itself buttonCounts value times, and every value will be changed to the corresponding button.
            for (int i = 0; i < buttonCount; i++)
            {
                if ((float)i < (float)buttonCount / 2)
                {
                    positions[i] = middlePosition + (float)i;
                }
                else
                {
                    positions[i] = middlePosition - (float)(buttonCount - i);
                }
                this.texts[i] = texts[i];
                
                buttons[i].Initialize(this.texts[i]);
            }

            //The spacing between the buttons is the height of the screen (720 pixels) devided by the amount of buttons. This will make the first button at 0 pixels, and the last at 720 pixels)
            spacing = 720 / (buttonCount - 1);

            //Initializing the extra buttons for no other use than to avoid a error
            extraButtonBottem.Initialize("");
            extraButtonTop.Initialize("");
        }

        static public void LoadContent(ContentManager content)
        {
            Button.LoadContent(content);
        }

        public void Update(int user)
        {
            //Variables and Arrays for the Update method
            GamePadState playerOne = GamePad.GetState(InfoPacket.Players[user]);

            #region Code for checking which button is the selected button and if pressed as well as correcting positions on axis.
            extraButtonBottem.Position = new Vector2(xPosition, 720 + spacing - 32 + percentageMoved * spacing);
            extraButtonBottem.Update(InfoPacket.GameTime);

            extraButtonTop.Position = new Vector2(xPosition, 0 - spacing - 32 + percentageMoved * spacing);
            extraButtonTop.Update(InfoPacket.GameTime);

            for (int i = 0; i < buttonCount; i++)
            {
                buttons[i].Position = new Vector2(xPosition, positions[i] * spacing - 32 + percentageMoved * spacing);

                buttons[i].Update(InfoPacket.GameTime);

                //Change the state of every button according to the positions.
                if (positions[i] == (buttonCount - 1) / 2)
                {
                    //If the "A" button on any controller is selected, give the current button a pressed state, or else a selected state
                    if (playerOne.Buttons.A == ButtonState.Pressed && InfoPacket.PreviousStates[0].Buttons.A == ButtonState.Released)
                    {
                        buttons[i].Press();
                    }
                    else if (buttons[i].State != BTNState.Pressed)
                    {
                        buttons[i].Select();
                    }
                }
                else
                {
                    buttons[i].Deselect();
                }
            }
            #endregion

            #region Code for Scrolling
            //Checking if moving and in which direction the group has to move
            if (playerOne.ThumbSticks.Left.Y < -.3f)
            {
                moving = true;
                movingUp = true;
            }
            else if (playerOne.ThumbSticks.Left.Y > .3f)
            {
                moving = true;
                movingUp = false;
            }
            else
            {
                moving = false;
            }

            //The code for movement
            if (moving)
            {
                acceleration = (1 - Math.Abs(ySpeed / maxSpeed)) * maxAcceleration;
                //Code for when the group is scrolling up
                if (movingUp)
                {
                    ySpeed -= acceleration;
                    percentageMoved += ySpeed / spacing;
                }
                //Code for when the group is scrolling down
                else if (!movingUp)
                {
                    ySpeed += acceleration;
                    percentageMoved += ySpeed / spacing;
                }
            }
            else
            {
                //Here it doesn't matter if the scrolling is going up or down
                //this is because the acceleration changes reletivily to the ySpeed being positive or negative
                acceleration = (ySpeed / maxSpeed) * maxAcceleration;

                ySpeed -= acceleration;
                percentageMoved += ySpeed / spacing;
            }

            for (int i = 0; i < buttonCount; i++)
            {
                if (positions[i] == buttonCount - 1)
                {
                    extraButtonTop.Text = buttons[i].Text;
                }
                else if (positions[i] == 0)
                {
                    extraButtonBottem.Text = buttons[i].Text;
                }
            }

            //Code to be executed when the centre button has to be changed
            if (Math.Abs(percentageMoved) >= 1)
            {
                percentageMoved = 0;

                //If the scrolling is going up, this code will be executed (REMEMBER: I don't use movingUp variable because that determines in which direction the
                //button SHOULD go, and not necesarily is going!
                if (ySpeed < 0)
                {
                    currentlySelected++;
                    //Change this button to the position of the previous/next button
                    float tempPosition = positions[buttonCount - 1];
                    for (int i = buttonCount - 1; i >= 0; i--)
                    {
                        if (i == 0)
                        {
                            positions[i] = tempPosition;
                        }
                        else
                        {
                            positions[i] = positions[i - 1];
                        }
                    }
                }
                //If the scrolling is going down
                else
                {
                    currentlySelected--;
                    //Change this button to the position of the previous/next button
                    float tempPosition = positions[0];
                    for (int i = 0; i < buttonCount; i++)
                    {
                        if (i == buttonCount - 1)
                        {
                            positions[i] = tempPosition;
                        }
                        else
                        {
                            positions[i] = positions[i + 1];
                        }
                    }
                }
            }

            //correct the currentlySelected to in the bounds
            if (currentlySelected == 0)
            {
                currentlySelected = buttonCount;
            }

            if (currentlySelected == buttonCount + 1)
            {
                currentlySelected = 1;
            }
            #endregion
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < buttonCount; i++)
            {
                buttons[i].Draw(spriteBatch);
            }
            extraButtonBottem.Draw(spriteBatch);
            extraButtonTop.Draw(spriteBatch);
        }
    }
}
