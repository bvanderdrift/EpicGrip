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
using PWS.TheGame.Upgrades;

namespace PWS.Screens.Shop
{
    class OffensiveUpgradeShop
    {
        //The Background
        static Sprite background;

        //Int to keep track of the currently selected upgrade
        static int currentUpgrade;

        //The examples
        static Sprite exampleM;
        static Sprite exampleR;
        static Sprite exampleCR;

        //The position of the examples
        static Vector2 examplePos;

        //Speed of rotation of the examples
        static float rotationSpeed = .03f;

        //Sprites for the preview
        static Sprite preview;
        static Sprite previewLA;
        static Sprite previewRA;

        //Sprite and font for the info box
        static Sprite infoBox;
        static SpriteFont infoFont;

        //Info variables
        static int costs;
        static string name;
        static int damage;
        static int range;
        static string description;

        static public void Instantiate()
        {
            //Instantiate the Background
            background = new Sprite();

            //Instantiate the examples
            exampleM = new Sprite();
            exampleR = new Sprite();
            exampleCR = new Sprite();

            //Instantiate the preview box
            preview = new Sprite();
            previewLA = new Sprite();
            previewRA = new Sprite();

            //Instaniate the info box
            infoBox = new Sprite();
        }

        static public void Initialize()
        {
            //Initialize the background
            background.Initialize(Vector2.Zero);

            //Set the example position
            examplePos = new Vector2(300, 400);

            //Initialize the examples
            exampleM.Initialize(examplePos);
            exampleR.Initialize(examplePos);
            exampleCR.Initialize(examplePos);

            exampleM.Origin = new Vector2(64);
            exampleR.Origin = new Vector2(128, 32);
            exampleCR.Origin = exampleR.Origin;

            //Initialize the preview box
            preview.Initialize(new Vector2(50, 170));
            previewLA.Initialize(new Vector2(50, 170));
            previewRA.Initialize(new Vector2(50, 170));

            //Initialize the info box
            infoBox.Initialize(new Vector2(600, 170));
        }

        static public void LoadContent(ContentManager content)
        {
            //Load the background
            background.LoadContent(content.Load<Texture2D>("Graphics/Basic Backgrounds/12"));

            //Load the examples
            exampleM.LoadContent(content.Load<Texture2D>("Graphics/Shop/OffUpgrades Shop/MineEx"));
            exampleR.LoadContent(content.Load<Texture2D>("Graphics/Shop/OffUpgrades Shop/RocketEx"));
            exampleCR.LoadContent(content.Load<Texture2D>("Graphics/Shop/OffUpgrades Shop/CRocketEx"));

            //Load the preview box
            preview.LoadContent(content.Load<Texture2D>("Graphics/Shop/OffUpgrades Shop/Preview"));
            previewLA.LoadContent(content.Load<Texture2D>("Graphics/Shop/OffUpgrades Shop/PreviewLA"));
            previewRA.LoadContent(content.Load<Texture2D>("Graphics/Shop/OffUpgrades Shop/PreviewRA"));

            //Load the infobox and font
            infoBox.LoadContent(content.Load<Texture2D>("Graphics/Shop/OffUpgrades Shop/InfoBox"));
            infoFont = content.Load<SpriteFont>("Graphics/Shop/OffUpgrades Shop/InfoFont");
        }

        static public void Update()
        {
            //Get the shop users controller stat
            GamePadState state = GamePad.GetState(InfoPacket.Players[ShopScreen.ShopUser]);

            //Update the costs
            switch (currentUpgrade)
            {
                case 0:
                    costs = 500;
                    name = "Mine";
                    damage = 25;
                    range = 200;
                    description = "5 Sec. Delay";
                    break;
                case 1:
                    costs = 2300;
                    name = "Missle";
                    damage = 50;
                    range = 300;
                    description = "Fast";
                    break;
                case 2:
                    costs = 3000;
                    name = "Controllable Missle";
                    damage = 50;
                    range = 300;
                    description = "Controllable";
                    break;
                default:
                    break;
            }

            //Update the background
            background.Update();

            //Update the sprites
            exampleM.Update();
            exampleR.Update();
            exampleCR.Update();

            exampleM.Rotation += rotationSpeed;
            exampleR.Rotation += rotationSpeed;
            exampleCR.Rotation += rotationSpeed;

            //Update the preview
            preview.Update();
            previewLA.Update();
            previewRA.Update();

            //Update the infobox
            infoBox.Update();

            //Adjust the current selection
            if (Math.Abs(state.ThumbSticks.Left.X) >= .5f && Math.Abs(InfoPacket.PreviousStates[ShopScreen.ShopUser].ThumbSticks.Left.X) < .5f &&
                !ShopScreen.notEnoughMoneyNotice.IsShowing &&
                !ShopScreen.areYouSurePopup.IsShowing)
            {
                currentUpgrade += (int)(state.ThumbSticks.Left.X * 1.98f);
                currentUpgrade = (int)MathHelper.Clamp(currentUpgrade, 0, 2);
            }

            //Check if the player wants to buy item
            if (state.Buttons.A == ButtonState.Released && InfoPacket.PreviousStates[ShopScreen.ShopUser].Buttons.A == ButtonState.Pressed &&
                !ShopScreen.notEnoughMoneyNotice.IsShowing &&
                !ShopScreen.areYouSurePopup.IsShowing)
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

            //If the player wants to buy item, buy the item
            if (ShopScreen.areYouSurePopup.JustClosed && ShopScreen.areYouSurePopup.Answer == true)
            {
                InfoPacket.PlayerStatistics[ShopScreen.ShopUser].Money -= costs;
                InfoPacket.PlayerStatistics[ShopScreen.ShopUser].UnlockOffensiveUpgrade(currentUpgrade);
                ShopScreen.PlayChaChing();
            }

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
        }

        static public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the background
            background.Draw(spriteBatch);

            //Draw the preview
            preview.Draw(spriteBatch);
            previewLA.Draw(spriteBatch);
            previewRA.Draw(spriteBatch);

            //Draw the current upgrade preview
            switch (currentUpgrade)
            {
                case 0:
                    exampleM.Draw(spriteBatch);
                    break;
                case 1:
                    exampleR.Draw(spriteBatch);
                    break;
                case 2:
                    exampleCR.Draw(spriteBatch);
                    break;
                default:
                    break;
            }

            //Draw the infobox + info
            infoBox.Draw(spriteBatch);
            spriteBatch.DrawString(infoFont, 
                "Name: " + name + "\n" +
            "Damage: " + damage + "\n" +
            "Explosion Range: " + range + "\n" +
            "Description: " + description + "\n" +
            "Costs :" + costs
            , new Vector2(655, 290), Color.Black);
        }
    }
}
