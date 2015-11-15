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
using PWS.TheGame;

namespace PWS.Screens
{
    class CustomizeScreen
    {
        //The Background
        static Sprite background;

        //Arrows
        static Sprite[] arrows;

        //Positions of selection items
        static Vector2 selectionPosition;
        static int heightDifference;

        //Currently Selected in height
        static int currentlySelectedGroup;

        //Currently selected per group and maximum per group
        static int[] currentlySelected;
        static int[] maxInGroup = new int[4] { 4, 3, 3, 6 }; 

        //Current user
        static int user;

        //The lock sprite
        static Sprite locked;

        //Bool array to see if the current object is unlocked
        static bool[] unlocked;

        //Examples in selection list
        static Sprite exampleColour;
        static Sprite[] exampleCar;
        static Sprite[] exampleOffUpgr;
        static Sprite exampleShield;
        static Sprite exampleShieldGlow;

        //Preview-Example items
        static Sprite exampleRoad;
        static Car example;

        static public void Instantiate()
        {
            //Instantiate the sprite
            background = new Sprite();

            //Initialize the array
            arrows = new Sprite[8];

            //Instantiate the lock
            locked = new Sprite();

            //Instantiate the arrows
            for (int i = 0; i < arrows.Length; i++)
            {
                arrows[i] = new Sprite();
            }

            //Instantiate the currently selected array
            currentlySelected = new int[4];

            //Instantiate the unlocked array
            unlocked = new bool[4];

            //Instantiate the examples
            exampleColour = new Sprite();
            exampleCar = new Sprite[4];
            for (int i = 0; i < exampleCar.Length; i++)
            {
                exampleCar[i] = new Sprite();
            }
            exampleOffUpgr = new Sprite[4];
            for (int i = 0; i < exampleOffUpgr.Length; i++)
            {
                exampleOffUpgr[i] = new Sprite();
            }
            exampleShield = new Sprite();
            exampleShieldGlow = new Sprite();

            //Instantiate the example road
            exampleRoad = new Sprite();
            example = new Car();
        }

        static public void Initialize()
        {
            //Initialize the sprite
            background.Initialize(Vector2.Zero);

            //Set the position of all the selection things
            selectionPosition = new Vector2(150, 300);

            //Initialize the lock
            locked.Initialize(Vector2.Zero);

            //Set the height difference
            heightDifference = 100;

            //Initialize the arrows
            for (int i = 0; i < arrows.Length; i += 2)
            {
                arrows[i].Initialize(new Vector2(selectionPosition.X, selectionPosition.Y + i * heightDifference / 2));
                arrows[i + 1].Initialize(new Vector2(selectionPosition.X + 150, selectionPosition.Y + i * heightDifference / 2));

                //Mirror the right arrow
                arrows[i + 1].Effect = SpriteEffects.FlipHorizontally;
            }

            //Player has all the colours, so first is always unlocked
            unlocked[0] = true;

            //Initialize the examples
            exampleColour.Initialize(selectionPosition + new Vector2(75, 0));
            for (int i = 0; i < exampleCar.Length; i++)
            {
                exampleCar[i].Initialize(selectionPosition + new Vector2(75, heightDifference));
            }
            for (int i = 0; i < exampleOffUpgr.Length; i++)
            {
                exampleOffUpgr[i].Initialize(selectionPosition + new Vector2(75, 2 * heightDifference));
            }
            exampleShield.Initialize(selectionPosition + new Vector2(75, 3 * heightDifference));
            exampleShieldGlow.Initialize(selectionPosition + new Vector2(75, 3 * heightDifference));

            //Initialize the preview
            exampleRoad.Initialize(new Vector2(790, 140));
            example.Initialize(user);
        }

        static public void LoadContent(ContentManager content)
        {
            //Load the background
            background.LoadContent(content.Load<Texture2D>("Graphics/Basic Backgrounds/6"));

            //Load the lock and last Initialization
            locked.LoadContent(content.Load<Texture2D>("Graphics/Customization/Locked"));
            locked.Origin = new Vector2(locked.Texture.Width, locked.Texture.Height) / 2;

            //Temp texture
            Texture2D arrow = content.Load<Texture2D>("Graphics/Customization/Arrow");

            //Load the arrows and do last Initialization
            for (int i = 0; i < arrows.Length; i++)
            {
                arrows[i].LoadContent(arrow);
                arrows[i].Origin = new Vector2(arrows[i].Texture.Width, arrows[i].Texture.Height) / 2;
            }

            //Load the examples
            //Colour example
            exampleColour.LoadContent(content.Load<Texture2D>("Graphics/Customization/Color_Selection/Preview"));
            exampleColour.SetOriginToMiddle();

            //Car Examples
            for (int i = 0; i < exampleCar.Length; i++)
            {
                exampleCar[i].LoadContent(content.Load<Texture2D>("Graphics/Customization/Cars/Car_" + (i + 1)));
                exampleCar[i].SetOriginToMiddle();
            }

            //Upgrade examples (Offensive)
            for (int i = 0; i < exampleOffUpgr.Length; i++)
            {
                exampleOffUpgr[i].LoadContent(content.Load<Texture2D>("Graphics/Customization/Off_Upgr/Upgr_" + i));
                exampleOffUpgr[i].SetOriginToMiddle();
            }

            exampleShield.LoadContent(content.Load<Texture2D>("Graphics/Customization/Def_Upgr/Shield"));
            exampleShield.SetOriginToMiddle();
            exampleShieldGlow.LoadContent(content.Load<Texture2D>("Graphics/Customization/Def_Upgr/Shield_Glow"));
            exampleShieldGlow.SetOriginToMiddle();

            //Load the preview
            example.LoadContent(content);
            exampleRoad.LoadContent(content.Load<Texture2D>("Graphics/Customization/ExampleRoad"));
        }

        static public void Update()
        {
            GamePadState state = GamePad.GetState(InfoPacket.Players[user]);

            //Return to main menu when B is pressed
            if (state.Buttons.B == ButtonState.Released && InfoPacket.PreviousStates[0].Buttons.B == ButtonState.Pressed)
            {
                ScreenManager.ChangeToMainMenu();
                InfoPacket.Save();
            }

            //Update the arrows
            for (int i = 0; i < arrows.Length; i++)
            {
                arrows[i].Update();

                //Set the colour of the currently selected items arrows to green, rest to red
                if ((int)(i / 2) == currentlySelectedGroup)
                {
                    arrows[i].Color = new Color(0, 255, 0);
                }
                else
                {
                    arrows[i].Color = new Color(255, 0, 0);
                }                   
            }

            //Change the currently selected height
            if (Math.Abs(state.ThumbSticks.Left.Y) >= .5f && Math.Abs(InfoPacket.PreviousStates[user].ThumbSticks.Left.Y) < .5f)
            {
                currentlySelectedGroup -= (int)(state.ThumbSticks.Left.Y * 1.96f);
                currentlySelectedGroup = (int)MathHelper.Clamp(currentlySelectedGroup, 0, 3);
            }

            //Change the currently selected item
            if (Math.Abs(state.ThumbSticks.Left.X) >= .5f && Math.Abs(InfoPacket.PreviousStates[user].ThumbSticks.Left.X) < .5f)
            {
                currentlySelected[currentlySelectedGroup] += (int)(state.ThumbSticks.Left.X * 1.96f);
                currentlySelected[currentlySelectedGroup] = (int)MathHelper.Clamp(currentlySelected[currentlySelectedGroup], 0, maxInGroup[currentlySelectedGroup]);
            }

            //Set all the unlocked values
            unlocked[1] = InfoPacket.PlayerStatistics[user].HasCar[currentlySelected[1]];
            unlocked[2] = InfoPacket.PlayerStatistics[user].HasOffensiveUpgrade[currentlySelected[2]];
            unlocked[3] = InfoPacket.PlayerStatistics[user].HasDefensiveUpgrade[currentlySelected[3]];

            //Set the car to what the player selects, but only if the item is unlocked(except for colour)
            switch (currentlySelected[0])
            {
                case 0:
                    InfoPacket.PlayerStatistics[user].CarColour = Color.LightGreen;
                    break;
                case 10:
                    InfoPacket.PlayerStatistics[user].CarColour = Color.Red;
                    break;
                case 2:
                    InfoPacket.PlayerStatistics[user].CarColour = Color.Orange;
                    break;
                case 3:
                    InfoPacket.PlayerStatistics[user].CarColour = Color.LightBlue;
                    break;
                case 4:
                    InfoPacket.PlayerStatistics[user].CarColour = Color.HotPink;
                    break;
                default:
                    break;
            }

            if (unlocked[1])
            {
                InfoPacket.PlayerStatistics[user].CarUsing = currentlySelected[1];
            }

            if (unlocked[2])
            {
                InfoPacket.PlayerStatistics[user].OffensiveUpgradeUsing = currentlySelected[2];
            }

            if (unlocked[3])
            {
                InfoPacket.PlayerStatistics[user].DefensiveUpgradeUsing = currentlySelected[3];
            }

            //Update the background
            background.Update();

            //Update the examples

            //Color example:
            exampleColour.Update();
            switch (currentlySelected[0])
            {
                case 0:
                    exampleColour.Color = Color.LightGreen;
                    InfoPacket.PlayerStatistics[user].CarColour = Color.LightGreen;
                    break;
                case 1:
                    exampleColour.Color = Color.Red;
                    InfoPacket.PlayerStatistics[user].CarColour = Color.Red;
                    break;
                case 2:
                    exampleColour.Color = Color.Orange;
                    InfoPacket.PlayerStatistics[user].CarColour = Color.Orange;
                    break;
                case 3:
                    exampleColour.Color = Color.LightBlue;
                    InfoPacket.PlayerStatistics[user].CarColour = Color.LightBlue;
                    break;
                case 4:
                    exampleColour.Color = Color.HotPink;
                    InfoPacket.PlayerStatistics[user].CarColour = Color.HotPink;
                    break;
                default:
                    break;
            }

            //Shield example
            exampleShield.Update();
            exampleShieldGlow.Update();
            exampleShield.Visible = true;
            exampleShieldGlow.Visible = true;
            switch (currentlySelected[3])
            {
                case 0:
                    exampleShield.Visible = false;
                    exampleShieldGlow.Visible = false;
                    break;
                case 1:
                    exampleShield.Color = new Color(0, 255, 0);
                    exampleShieldGlow.Color = new Color(0, 128, 0);
                    break;
                case 2:
                    exampleShield.Color = new Color(0, 255, 255);
                    exampleShieldGlow.Color = new Color(0, 204, 202);
                    break;
                case 3:
                    exampleShield.Color = new Color(0, 38, 255);
                    exampleShieldGlow.Color = new Color(0, 19, 130);
                    break;
                case 4:
                    exampleShield.Color = new Color(255, 119, 0);
                    exampleShieldGlow.Color = new Color(255, 178, 0);
                    break;
                case 5:
                    exampleShield.Color = new Color(255, 0, 0);
                    exampleShieldGlow.Color = new Color(154, 0, 0);
                    break;
                case 6:
                    exampleShield.Color = new Color(0, 0, 0);
                    exampleShieldGlow.Color = new Color(255, 0, 0);
                    break;
                default:
                    break;
            }

            //Update the preview
            example.Update();

            example.CarShown = InfoPacket.PlayerStatistics[user].CarUsing;
            example.BumperShown = InfoPacket.PlayerStatistics[user].DefensiveUpgradeUsing;
            example.Color = InfoPacket.PlayerStatistics[user].CarColour;

            exampleRoad.Update();
        }

        static public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the background
            background.Draw(spriteBatch);

            //Draw the arrows
            for (int i = 0; i < arrows.Length; i++)
            {
                arrows[i].Draw(spriteBatch);
            }

            //Draw the examples
            exampleColour.Draw(spriteBatch);
            exampleCar[currentlySelected[1]].Draw(spriteBatch);
            exampleOffUpgr[currentlySelected[2]].Draw(spriteBatch);
            exampleShield.Draw(spriteBatch);
            exampleShieldGlow.Draw(spriteBatch);

            //Check if the lock should be drawn
            for (int i = 0; i < 4; i++)
            {
                locked.Position = selectionPosition + new Vector2(75, i * heightDifference);
                if (!unlocked[i])
                {
                    locked.Draw(spriteBatch);
                }
            }

            //Draw the preview
            exampleRoad.Draw(spriteBatch);
            example.Draw(spriteBatch);
        }

        static public void Open(int user)
        {
            //Get the currently selected value of the cars colour
            switch (InfoPacket.PlayerStatistics[user].CarColour.ToString())
            {
                case "{R:144 G:238 B:0 A:144}":
                    currentlySelected[0] = 0;
                    break;
                case "{R:255 G:0 B:0 A:255}":
                    currentlySelected[0] = 1;
                    break;
                case "{R:255 G:165 B:0 A:255}":
                    currentlySelected[0] = 2;
                    break;
                case "{R:173 G:216 B:230 A:255}":
                    currentlySelected[0] = 3;
                    break;
                case "{R:255 G:105 B:133 A:255}":
                    currentlySelected[0] = 3;
                    break;
                default:
                    break;
            }

            //Get the current car
            currentlySelected[1] = InfoPacket.PlayerStatistics[user].CarUsing;

            //Get the current offensive upgrade
            currentlySelected[2] = InfoPacket.PlayerStatistics[user].OffensiveUpgradeUsing;

            //Get the current defensive upgrade
            currentlySelected[3] = InfoPacket.PlayerStatistics[user].DefensiveUpgradeUsing;

            //Set the user
            CustomizeScreen.user = user;

            //Initialize the example
            example.SetExample(currentlySelected[1], currentlySelected[3]);
            example.Position = new Vector2(1000, 300);
            example.Rotation = (float)Math.PI;
            example.MakeAI();
            example.DriveExampleCircles();
        }
    }
}
