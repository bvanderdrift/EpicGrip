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

namespace PWS.Screens.Shop
{
    class DefensiveUpgradeShop
    {
        //The background
        static Sprite background;

        //Currently SHown Bumper
        static int currentBumper;

        //Costs and name
        static int costs;
        static string quality;

        //Infobox items
        static Sprite infoBox;
        static SpriteFont infoFont;

        static public void Instantiate()
        {
            //Instantiate the background
            background = new Sprite();

            //Instantiate the infoBox
            infoBox = new Sprite();
        }

        static public void Initialize()
        {
            //Initialize the background
            background.Initialize(Vector2.Zero);

            //Initialize the info box
            infoBox.Initialize(new Vector2(100, 200));

            currentBumper = 1;
        }

        static public void LoadContent(ContentManager content)
        {
            //Load the background
            background.LoadContent(content.Load<Texture2D>("Graphics/Basic Backgrounds/10"));

            //Load the info box items
            infoBox.LoadContent(content.Load<Texture2D>("Graphics/Shop/Def_Upgr_Shop/InfoBox"));
            infoFont = content.Load<SpriteFont>("Graphics/Shop/Def_Upgr_Shop/InfoFont");
        }

        static public void Update()
        {
            GamePadState state = GamePad.GetState(InfoPacket.Players[ShopScreen.ShopUser]);

            //Set up the example car
            ShopScreen.ExampleCar.CarShown = InfoPacket.PlayerStatistics[ShopScreen.ShopUser].CarUsing;
            ShopScreen.ExampleCar.BumperShown = currentBumper;

            //Update the info box
            infoBox.Update();

            //Scroll trough options
            //Adjust the current selection
            if (Math.Abs(state.ThumbSticks.Left.X) >= .5f && Math.Abs(InfoPacket.PreviousStates[ShopScreen.ShopUser].ThumbSticks.Left.X) < .5f &&
                !ShopScreen.notEnoughMoneyNotice.IsShowing &&
                !ShopScreen.areYouSurePopup.IsShowing)
            {
                currentBumper += (int)(state.ThumbSticks.Left.X * 1.98f);
                currentBumper = (int)MathHelper.Clamp(currentBumper, 1, 6);
            }

            //If A is pressed by the shop user, check if he has enough money and assure he wants to buy the arena
            if (state.Buttons.A == ButtonState.Released && InfoPacket.PreviousStates[ShopScreen.ShopUser].Buttons.A == ButtonState.Pressed &&
                !ShopScreen.notEnoughMoneyNotice.JustClosed &&
                !ShopScreen.areYouSurePopup.JustClosed &&
                !InfoPacket.PlayerStatistics[ShopScreen.ShopUser].HasDefensiveUpgrade[currentBumper])
            {
                if (InfoPacket.PlayerStatistics[ShopScreen.ShopUser].Money >= costs)
                {
                    ShopScreen.ShowRUSure(costs);
                }
                else
                {
                    ShopScreen.ShowNEMoney(costs);
                    ShopScreen.PlayError();
                }
            }

            //If B is pressed, go back to the shop menu
            if (state.Buttons.B == ButtonState.Released && InfoPacket.PreviousStates[ShopScreen.ShopUser].Buttons.B == ButtonState.Pressed &&
                !ShopScreen.notEnoughMoneyNotice.JustClosed &&
                !ShopScreen.areYouSurePopup.JustClosed)
            {
                ScreenManager.ChangeToShopScreen(ShopScreen.ShopUser);
                InfoPacket.Save();
            }

            //Update the costs and quality
            switch (currentBumper)
            {
                case 1:
                    costs = 300;
                    quality = "Weak";
                    break;
                case 2:
                    costs = 600;
                    quality = "Low";
                    break;
                case 3:
                    costs = 1000;
                    quality = "Average";
                    break;
                case 4:
                    costs = 1700;
                    quality = "Good";
                    break;
                case 5:
                    costs = 2400;
                    quality = "Strong";
                    break;
                case 6:
                    costs = 3000;
                    quality = "Tough";
                    break;
                default:
                    break;
            }

            //If the answer to "Are You Sure" is true, buy the arena
            if (ShopScreen.areYouSurePopup.JustClosed && ShopScreen.areYouSurePopup.Answer == true)
            {
                InfoPacket.PlayerStatistics[ShopScreen.ShopUser].Money -= costs;
                InfoPacket.PlayerStatistics[ShopScreen.ShopUser].UnlockDefensiveUpgrade(currentBumper);
                ShopScreen.PlayChaChing();
            }

            background.Update();
        }

        static public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the background
            background.Draw(spriteBatch);

            //Draw the infoBox and text
            infoBox.Draw(spriteBatch);
            spriteBatch.DrawString(infoFont, "Quality: " + quality + "\n" + "Health: " + ShopScreen.ExampleCar.Shield.Health + "\n" + "Costs: " + costs, infoBox.Position + new Vector2(55, 120), Color.Black);

            //Draw the example
            ShopScreen.DrawExample(spriteBatch);
        }
    }
}
