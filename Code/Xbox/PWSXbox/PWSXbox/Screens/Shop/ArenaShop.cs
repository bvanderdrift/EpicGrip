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
using PWS.Popups;

namespace PWS.Screens.Shop
{
    class ArenaShop
    {
        static Sprite background;

        //An array to contain all images of the arena's
        static Sprite[] arenas;

        //Integer to represent the currently selected arena
        static int currentArena;

        //The preview box
        static Sprite frame;

        //The info box and its font
        static Sprite infoBox;
        static SpriteFont infoFont;

        //Locked and scroll sprites
        static Sprite locked;
        static Sprite arrowLeft;
        static Sprite arrowRight;

        //Stats of a arena to show
        static string name;
        static float bounciness;
        static int costs;

        static public void Instantiate()
        {
            background = new Sprite();

            //Instantiate the array and the sprites in the array
            arenas = new Sprite[InfoPacket.AmountOfArenas];

            for (int i = 0; i < arenas.Length; i++)
            {
                arenas[i] = new Sprite();
            }

            //Instatiate the boxes
            frame = new Sprite();
            infoBox = new Sprite();

            //Instantiate the locked and scroll sprites
            arrowLeft = new Sprite();
            arrowRight = new Sprite();
            locked = new Sprite();
        }

        static public void Initialize()
        {
            background.Initialize(Vector2.Zero);

            //Initialize the sprites in the array
            for (int i = 0; i < arenas.Length; i++)
            {
                arenas[i].Initialize(new Vector2(600, 320));
                arenas[i].Scale = new Vector2(.5f);
            }

            //Initialize the frame and infobox
            frame.Initialize(new Vector2(560, 262));
            infoBox.Initialize(new Vector2(25, 120));

            //Set the current arena to 0
            currentArena = 0;

            //Set the arena stats to nothing
            name = "";
            bounciness = 0;

            //Initialize the locked and scroll sprites
            locked.Initialize(new Vector2(888, 460));
            arrowLeft.Initialize(new Vector2(1050, 260));
            arrowRight.Initialize(new Vector2(1180, 260));
        }

        static public void LoadContent(ContentManager content)
        {
            background.LoadContent(content.Load<Texture2D>("Graphics/Basic Backgrounds/8"));

            //Load the arenas
            for (int i = 0; i < arenas.Length; i++)
            {
                arenas[i].LoadContent(content.Load<Texture2D>("Graphics/Arenas/" + (i + 1)));
            }

            //Load the frame and info box
            frame.LoadContent(content.Load<Texture2D>("Graphics/ChooseArena/PreviewFrame"));
            infoBox.LoadContent(content.Load<Texture2D>("Graphics/ChooseArena/InfoBlock"));

            //Load the font
            infoFont = content.Load<SpriteFont>("Graphics/ChooseArena/ChooseArenaFont");

            //Load the lock
            locked.LoadContent(content.Load<Texture2D>("Graphics/ChooseArena/Locked"));

            //Load the arrows for scrolling
            arrowLeft.LoadContent(content.Load<Texture2D>("Graphics/ChooseArena/ArrowLeft"));
            arrowRight.LoadContent(content.Load<Texture2D>("Graphics/ChooseArena/ArrowRight"));
        }

        static public void Update()
        {
            GamePadState state = GamePad.GetState(InfoPacket.Players[ShopScreen.ShopUser]);

            //Set visibility of lock
            locked.Visible = !InfoPacket.PlayerStatistics[ShopScreen.ShopUser].HasArena[currentArena];

            //Update the arena sprites
            for (int i = 0; i < arenas.Length; i++)
            {
                arenas[i].Update();
            }

            #region set the name and bounciness and cost to the selected arena
            switch (currentArena)
            {
                case 0:
                    name = "Wooden Arena";
                    bounciness = .5f;
                    costs = 0;
                    break;
                case 1:
                    name = "Metallic Arena";
                    bounciness = .2f;
                    costs = 300;
                    break;
                case 2:
                    name = "Coushon Arena";
                    bounciness = .7f;
                    costs = 500;
                    break;
                case 3:
                    name = "Hell Arena";
                    bounciness = .4f;
                    costs = 500;
                    break;
                case 4:
                    name = "Scary Arena";
                    bounciness = .5f;
                    costs = 500;
                    break;
                case 5:
                    name = "Angel Arena";
                    bounciness = .6f;
                    costs = 700;
                    break;
                case 6:
                    name = "Nightclub Arena";
                    bounciness = .6f;
                    costs = 1000;
                    break;
                default:
                    break;
            }
            #endregion

            //If A is pressed by the shop user, check if he has enough money and assure he wants to buy the arena
            if (state.Buttons.A == ButtonState.Released && InfoPacket.PreviousStates[ShopScreen.ShopUser].Buttons.A == ButtonState.Pressed &&
                !ShopScreen.notEnoughMoneyNotice.JustClosed &&
                !ShopScreen.areYouSurePopup.JustClosed &&
                !InfoPacket.PlayerStatistics[ShopScreen.ShopUser].HasArena[currentArena])
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

            //If the answer to "Are You Sure" is true, buy the arena
            if (ShopScreen.areYouSurePopup.JustClosed && ShopScreen.areYouSurePopup.Answer == true)
            {
                InfoPacket.PlayerStatistics[ShopScreen.ShopUser].Money -= costs;
                InfoPacket.PlayerStatistics[ShopScreen.ShopUser].UnlockArena(currentArena);
                ShopScreen.PlayChaChing();
            }

            //A code to scroll up, but not go higher than the max. nr of arenas
            if (InfoPacket.PreviousStates[ShopScreen.ShopUser].ThumbSticks.Left.X < .5f &&
                state.ThumbSticks.Left.X > .5f &&
                currentArena < InfoPacket.AmountOfArenas - 1)
            {
                currentArena++;
            }

            //A code to scroll down, but not go lower than the first arena
            if (InfoPacket.PreviousStates[ShopScreen.ShopUser].ThumbSticks.Left.X > -.5f &&
                state.ThumbSticks.Left.X < -.5f &&
                currentArena > 0)
            {
                currentArena--;
            }

            //Change the arrow colour to a "available (red)" or "unavailable (gray)" colour
            if (currentArena == 0)
            {
                arrowLeft.Color = Color.Gray;
            }
            else
            {
                arrowLeft.Color = new Color(0, 255, 0);
            }

            if (currentArena == InfoPacket.AmountOfArenas - 1)
            {
                arrowRight.Color = Color.Gray;
            }
            else
            {
                arrowRight.Color = new Color(0, 255, 0);
            }

            //Update the sprites
            frame.Update();
            infoBox.Update();
            background.Update();
            locked.Update();
            arrowLeft.Update();
            arrowRight.Update();
        }

        static public void Draw(SpriteBatch spriteBatch)
        {
            background.Draw(spriteBatch);

            //Draw the preview
            arenas[currentArena].Draw(spriteBatch);
            frame.Draw(spriteBatch);

            //Draw the infobox and the information on it
            infoBox.Draw(spriteBatch);

            if (!InfoPacket.PlayerStatistics[ShopScreen.ShopUser].HasArena[currentArena])
            {
                spriteBatch.DrawString(infoFont, "Name: \"" + name + "\"" + "\n" +
                "Bounciness: " + (bounciness * 100) + "%" + "\n" +
                "Costs: " + costs, new Vector2(50, 200), Color.Black);
            }
            else
            {
                spriteBatch.DrawString(infoFont, "Name: \"" + name + "\"" + "\n" +
                                "Bounciness: " + (bounciness * 100) + "%" + "\n" +
                                "Already owned!", new Vector2(50, 200), Color.Black);
            }
            
            //Draw the lock
            locked.Draw(spriteBatch);

            //Graphics for Arena Count
            arrowLeft.Draw(spriteBatch);
            spriteBatch.DrawString(infoFont, (currentArena + 1).ToString(), new Vector2(1105, 270), Color.Black);
            spriteBatch.DrawString(infoFont, "/" + InfoPacket.AmountOfArenas, new Vector2(1135, 270), Color.Black);
            arrowRight.Draw(spriteBatch);
        }
    }
}
