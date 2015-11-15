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
using PWS.Screens.Shop;
using PWS.TheGame;
using PWS.Popups;

namespace PWS.Screens
{
    enum CurrentShopScreen
    {
        MainShopScreen,
        CarShopScreen,
        DefUpgrScreen,
        AttUpgrScreen,
        ArenaShopScreen,
    }

    class ShopScreen
    {
        static CurrentShopScreen currentScreen;
        static Sprite background;
        static ButtonGroup buttons;

        //A variable to represent the current shopUser
        static int shopUser;

        //Sprite to show the users money and its font
        static Sprite moneyDisplay;
        static SpriteFont moneyFont;

        //Everything for the example
        static Car example;
        static Sprite exampleRoad;

        //Popup to show when the player doesn't have enough money
        static public Popup notEnoughMoneyNotice;
        static public Popup areYouSurePopup;

        //Cha-Ching sound and Error sound
        static SoundEffect chaching;
        static SoundEffect error;

        public static int ShopUser
        {
            get { return shopUser; }
        }

        public static Car ExampleCar
        {
            get { return example; }
            set { example = value; }
        }

        static public void Instantiate()
        {
            background = new Sprite();
            buttons = new ButtonGroup();
            moneyDisplay = new Sprite();

            ArenaShop.Instantiate();
            CarShop.Instantiate();
            DefensiveUpgradeShop.Instantiate();
            OffensiveUpgradeShop.Instantiate();

            //Instantiate the example
            example = new Car();
            exampleRoad = new Sprite();

            //Instantiate the popups
            notEnoughMoneyNotice = new Popup();
            areYouSurePopup = new Popup();
        }

        static public void Initialize()
        {
            background.Initialize(Vector2.Zero);
            currentScreen = CurrentShopScreen.MainShopScreen;

            moneyDisplay.Initialize(new Vector2(50, 600));

            ArenaShop.Initialize();
            CarShop.Initialize();
            DefensiveUpgradeShop.Initialize();
            OffensiveUpgradeShop.Initialize();

            //Set up all the buttons
            string[] texts = new string[4];
            texts[0] = "Cars";
            texts[1] = "Shields";
            texts[2] = "Weapons";
            texts[3] = "Arenas";
            buttons.Initialize(texts, 800);

            //Set up the car & example
            example.Initialize((int)ShopScreen.ShopUser);
            example.MakeAI();
            example.DriveExampleCircles();
            example.Position = new Vector2(1000, 300);
            example.Rotation = (float)Math.PI;
            exampleRoad.Initialize(new Vector2(790, 140));

            //Initialize the popups
            notEnoughMoneyNotice.Initialize(Popups.Type.Ok);
            areYouSurePopup.Initialize(Popups.Type.NoOk);
        }

        static public void LoadContent(ContentManager content)
        {
            background.LoadContent(content.Load<Texture2D>("Graphics/Basic Backgrounds/5"));
            moneyDisplay.LoadContent(content.Load<Texture2D>("Graphics/Shop/MoneyDisplay"));
            moneyFont = content.Load<SpriteFont>("Graphics/Shop/MoneyFont");

            ArenaShop.LoadContent(content);
            CarShop.LoadContent(content);
            DefensiveUpgradeShop.LoadContent(content);
            OffensiveUpgradeShop.LoadContent(content);

            //Load content of the car
            example.LoadContent(content);
            example.SetExample(0, 0);

            //Load the example road
            exampleRoad.LoadContent(content.Load<Texture2D>("Graphics/Shop/CarShop/ExampleRoad"));

            //Load the content of the popups
            notEnoughMoneyNotice.LoadContent(content);
            areYouSurePopup.LoadContent(content);

            //Load chaching sound and error sound
            chaching = content.Load<SoundEffect>("Audio/Effects/Menu/Lar");
            error = content.Load<SoundEffect>("Audio/Effects/Menu/Error");
        }

        static public void Update()
        {
            GamePadState state = GamePad.GetState(InfoPacket.Players[shopUser]);

            #region global shop updates
            moneyDisplay.Update();
            exampleRoad.Update();
            example.Update();

            //Set the colour of the car to the shop users colour
            example.Color = InfoPacket.PlayerStatistics[shopUser].CarColour;

            //Update the popups
            notEnoughMoneyNotice.Update();
            areYouSurePopup.Update();
            #endregion

            #region updates per screen
            if (currentScreen == CurrentShopScreen.MainShopScreen)
            {
                //Return to main menu when B is pressed
                if (state.Buttons.B == ButtonState.Released && InfoPacket.PreviousStates[shopUser].Buttons.B == ButtonState.Pressed)
                {
                    ScreenManager.ChangeToMainMenu();
                }


                buttons.Update(shopUser);

                //Check if a selection is made
                if (state.Buttons.A == ButtonState.Released && InfoPacket.PreviousStates[shopUser].Buttons.A == ButtonState.Pressed)
                {
                    if (buttons.CurrentlySelect == 1)
                    {
                        ChangeToArenaShop();
                    }
                    else if (buttons.CurrentlySelect == 2)
                    {
                        ChangeToCarShop();
                    }
                    else if (buttons.CurrentlySelect == 3)
                    {
                        ChangeToDefUpgrShop();
                    }
                    else if (buttons.CurrentlySelect == 4)
                    {
                        ChangeToAttUpgrShop();
                    }
                }

                background.Update();
            }
            else if (currentScreen == CurrentShopScreen.CarShopScreen)
            {
                CarShop.Update();
            }
            else if (currentScreen == CurrentShopScreen.ArenaShopScreen)
            {
                ArenaShop.Update();
            }
            else if (currentScreen == CurrentShopScreen.DefUpgrScreen)
            {
                DefensiveUpgradeShop.Update();
            }
            else if (currentScreen == CurrentShopScreen.AttUpgrScreen)
            {
                OffensiveUpgradeShop.Update();
            }
            #endregion
        }

        static public void Draw(SpriteBatch spriteBatch)
        {
            if (currentScreen == CurrentShopScreen.MainShopScreen)
            {
                background.Draw(spriteBatch);
                buttons.Draw(spriteBatch);
            }
            else if (currentScreen == CurrentShopScreen.CarShopScreen)
            {
                CarShop.Draw(spriteBatch);
            }
            else if (currentScreen == CurrentShopScreen.ArenaShopScreen)
            {
                ArenaShop.Draw(spriteBatch);
            }
            else if (currentScreen == CurrentShopScreen.DefUpgrScreen)
            {
                DefensiveUpgradeShop.Draw(spriteBatch);
            }
            else if (currentScreen == CurrentShopScreen.AttUpgrScreen)
            {
                OffensiveUpgradeShop.Draw(spriteBatch);
            }

            //Darw everything for the moneyDisplay and the text in it
            moneyDisplay.Draw(spriteBatch);
            spriteBatch.DrawString(moneyFont, InfoPacket.PlayerStatistics[shopUser].Money.ToString(), 
                moneyDisplay.Position + new Vector2(320 - moneyFont.MeasureString(InfoPacket.PlayerStatistics[shopUser].Money.ToString()).X, 30), Color.Black);

            //Draw the popups
            notEnoughMoneyNotice.Draw(spriteBatch);
            areYouSurePopup.Draw(spriteBatch);
        }

        static void ChangeToArenaShop()
        {
            currentScreen = CurrentShopScreen.ArenaShopScreen;
        }

        static void ChangeToCarShop()
        {
            currentScreen = CurrentShopScreen.CarShopScreen;
        }

        static void ChangeToDefUpgrShop()
        {
            currentScreen = CurrentShopScreen.DefUpgrScreen;
        }

        static void ChangeToAttUpgrShop()
        {
            currentScreen = CurrentShopScreen.AttUpgrScreen;
        }

        static public void Open(int shopUser)
        {
            ShopScreen.shopUser = shopUser;
            currentScreen = CurrentShopScreen.MainShopScreen;
            buttons.Speed = 0;
        }

        static public void DrawExample(SpriteBatch spriteBatch)
        {
            //Draw the road
            exampleRoad.Draw(spriteBatch);

            //Draw the example car
            example.Draw(spriteBatch);
        }

        static public void ShowRUSure(int costs)
        {
            ShopScreen.areYouSurePopup.Show("Are you sure?", "Are you sure you want to buy this item for " + costs + " money?", shopUser);
        }

        static public void ShowNEMoney(int costs)
        {
            ShopScreen.notEnoughMoneyNotice.Show("Ooh no!", "You do not have the money to buy this item!" + "\n"
                        + "It seems you need " + (costs - InfoPacket.PlayerStatistics[(int)ShopScreen.ShopUser].Money) + " more!", ShopScreen.ShopUser);
        }

        static public void PlayChaChing()
        {
            chaching.Play();
        }

        static public void PlayError()
        {
            error.Play();
        }
    }
}
