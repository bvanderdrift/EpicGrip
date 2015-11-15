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
using PWS.Popups;

namespace PWS.Screens.Shop
{
    class CarShop
    {
        static Sprite background;

        static int currentlySelected;
        static Sprite infoBox;
        static SpriteFont boxFont;
        static int cost;

        

        static public void Instantiate()
        {
            //Initialize the sprites & Car
            background = new Sprite();
            infoBox = new Sprite();

        }

        static public void Initialize()
        {
            background.Initialize(Vector2.Zero);

            currentlySelected = 0;

            //Initialize the info box
            infoBox.Initialize(new Vector2(50, 150));

        }

        static public void LoadContent(ContentManager content)
        {
            background.LoadContent(content.Load<Texture2D>("Graphics/Basic Backgrounds/9"));

            //Load the info box
            infoBox.LoadContent(content.Load<Texture2D>("Graphics/Shop/CarShop/CarInfoBox"));

            //Load the info box font
            boxFont = content.Load<SpriteFont>("Graphics/Shop/CarShop/InfoBoxFont");
        }

        static public void Update()
        {
            GamePadState state = GamePad.GetState(InfoPacket.Players[ShopScreen.ShopUser]);

            //Set the car to the currently selected car
            ShopScreen.ExampleCar.CarShown = currentlySelected;

            //Update the sprites
            infoBox.Update();

            //Return to main menu when B is pressed
            if (state.Buttons.B == ButtonState.Released &&
                InfoPacket.PreviousStates[ShopScreen.ShopUser].Buttons.B == ButtonState.Pressed &&
                !ShopScreen.notEnoughMoneyNotice.JustClosed &&
                !ShopScreen.areYouSurePopup.JustClosed &&
                !ShopScreen.notEnoughMoneyNotice.IsShowing &&
                !ShopScreen.areYouSurePopup.IsShowing)
            {
                ScreenManager.ChangeToShopScreen(ShopScreen.ShopUser);
                InfoPacket.Save();
            }

            //Adjust the current selection
            if (Math.Abs(state.ThumbSticks.Left.X) >= .5f && Math.Abs(InfoPacket.PreviousStates[ShopScreen.ShopUser].ThumbSticks.Left.X) < .5f &&
                !ShopScreen.notEnoughMoneyNotice.IsShowing &&
                !ShopScreen.areYouSurePopup.IsShowing)
            {
                currentlySelected += (int)(state.ThumbSticks.Left.X * 1.98f);
                currentlySelected = (int)MathHelper.Clamp(currentlySelected, 0, 3);
            }

            //Try to buy item when player presses "A"
            if (state.Buttons.A == ButtonState.Released &&
                InfoPacket.PreviousStates[(int)ShopScreen.ShopUser].Buttons.A == ButtonState.Pressed &&
                !ShopScreen.notEnoughMoneyNotice.IsShowing &&
                !ShopScreen.areYouSurePopup.IsShowing &&
                !InfoPacket.PlayerStatistics[(int)ShopScreen.ShopUser].HasCar[currentlySelected])
            {
                if (InfoPacket.PlayerStatistics[(int)ShopScreen.ShopUser].Money < cost)
                {
                    ShopScreen.ShowNEMoney(cost);
                    ShopScreen.PlayError();
                }
                else
                {
                    ShopScreen.ShowRUSure(cost);
                }
            }

            if (ShopScreen.areYouSurePopup.JustClosed && ShopScreen.areYouSurePopup.Answer == true)
            {
                InfoPacket.PlayerStatistics[ShopScreen.ShopUser].Money -= cost;
                InfoPacket.PlayerStatistics[ShopScreen.ShopUser].UnlockCar(currentlySelected);
                ShopScreen.PlayChaChing();
            }

            switch (currentlySelected)
            {
                case 0:
                    cost = 0;
                    break;
                case 1:
                    cost = 1000;
                    break;
                case 2:
                    cost = 2500;
                    break;
                case 3:
                    cost = 3000;
                    break;
                default:
                    break;
            }

            //Update the background
            background.Update();
        }

        static public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the background
            background.Draw(spriteBatch);

            //Draw example
            ShopScreen.DrawExample(spriteBatch);

            //Draw the infobox
            infoBox.Draw(spriteBatch);
            if (InfoPacket.PlayerStatistics[(int)ShopScreen.ShopUser].HasCar[currentlySelected])
            {
                spriteBatch.DrawString(boxFont,
                   "Maximum Speed: " + ShopScreen.ExampleCar.MaxSpeed * 10 + "\n" +
                   "Acceleration: " + ShopScreen.ExampleCar.MaxAcceleration * 10 + "\n" +
                   "Damage: " + ShopScreen.ExampleCar.DamageFactor + "\n" +
                   "Health: " + ShopScreen.ExampleCar.MaxHealth + "\n" +
                   "Cost: OWNED! :D",
                   new Vector2(60, 220), Color.Black);

            }
            else
            {
                spriteBatch.DrawString(boxFont,
                    "Maximum Speed: " + ShopScreen.ExampleCar.MaxSpeed * 10 + "\n" +
                    "Acceleration: " + ShopScreen.ExampleCar.MaxAcceleration * 10 + "\n" +
                    "Damage: " + ShopScreen.ExampleCar.DamageFactor + "\n" +
                    "Health: " + ShopScreen.ExampleCar.MaxHealth + "\n" +
                    "Cost: " + cost,
                    new Vector2(60, 220), Color.Black);
            }
        }
    }
}
