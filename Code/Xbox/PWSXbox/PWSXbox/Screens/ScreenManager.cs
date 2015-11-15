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
using PWS.Arenas;
using PWS.TheGame;
using PWS.Controls;

namespace PWS.Screens
{
    //Enumeration for every screen needed
    enum CurrentScreen
    {
        StartScreen,
        MainMenu,
        SettingsMenu,
        SigninMenu,
        PlayScreen,
        UpgradeScreen,
        CustomizeScreen,
        ArenaSelection,
        ShopScreen,
    }

    class ScreenManager
    {
        static CurrentScreen currentScreen;

        static public CurrentScreen CurrentScreen
        {
            get { return currentScreen; }
        }

        static public void Instantiate()
        {
            //Instantiate every screen
            StartScreen.Instantiate();
            MainMenu.Instantiate();
            CreditsMenu.Instantiate();
            SigninMenu.Instantiate();
            UpgradeScreen.Instantiate();
            CustomizeScreen.Instantiate();
            ArenaSelection.Instantiate();
            ShopScreen.Instantiate();

            //Instantiate the game engine
            GameEngine.Instantiate();
        }

        static public void Initialize()
        {
            currentScreen = CurrentScreen.StartScreen;

            //Initialize every screen
            StartScreen.Initialize();
            MainMenu.Initialize();
            CreditsMenu.Initialize();
            SigninMenu.Initialize();
            UpgradeScreen.Initialize();
            CustomizeScreen.Initialize();
            ArenaSelection.Initialize();
            ShopScreen.Initialize();

            //Initialize the game engine
            GameEngine.Initialize();
        }

        static public void LoadContent(ContentManager content)
        {
            //Load the textures for the buttons
            ButtonGroup.LoadContent(content);

            //Load the content for every screen
            StartScreen.LoadContent(content);
            MainMenu.LoadContent(content);
            CreditsMenu.LoadContent(content);
            SigninMenu.LoadContent(content);
            UpgradeScreen.LoadContent(content);
            CustomizeScreen.LoadContent(content);
            ArenaSelection.LoadContent(content);
            ShopScreen.LoadContent(content);

            //Load the content of the game engine
            GameEngine.LoadContent(content);
        }

        static public void Update()
        {
            //For every state its own block of code to run when updating
            if (currentScreen == CurrentScreen.StartScreen)
            {
                StartScreen.Update();
            }
            else if (currentScreen == CurrentScreen.MainMenu)
            {
                MainMenu.Update( );
            }
            else if (currentScreen == CurrentScreen.SettingsMenu)
            {
                CreditsMenu.Update();
            }
            else if (currentScreen == CurrentScreen.SigninMenu)
            {
                SigninMenu.Update();
            }
            else if (currentScreen == CurrentScreen.PlayScreen)
            {
                GameEngine.Update();
            }
            else if (currentScreen == CurrentScreen.UpgradeScreen)
            {
                UpgradeScreen.Update();
            }
            else if (currentScreen == CurrentScreen.CustomizeScreen)
            {
                CustomizeScreen.Update();
            }
            else if (currentScreen == CurrentScreen.ArenaSelection)
            {
                ArenaSelection.Update();
            }
            else if (currentScreen == CurrentScreen.ShopScreen)
            {
                ShopScreen.Update();
            }
        }

        static public void Draw(SpriteBatch spriteBatch)
        {
            //For every state its own block of code to run when drawing
            if (currentScreen == CurrentScreen.StartScreen)
            {
                StartScreen.Draw(spriteBatch);
            }
            else if (currentScreen == CurrentScreen.MainMenu)
            {
                MainMenu.Draw(spriteBatch);
            }
            else if (currentScreen == CurrentScreen.SettingsMenu)
            {
                CreditsMenu.Draw(spriteBatch);
            }
            else if (currentScreen == CurrentScreen.SigninMenu)
            {
                SigninMenu.Draw(spriteBatch);
            }
            else if (currentScreen == CurrentScreen.PlayScreen)
            {
                GameEngine.Draw(spriteBatch);
            }
            else if (currentScreen == CurrentScreen.UpgradeScreen)
            {
                UpgradeScreen.Draw(spriteBatch);
            }
            else if (currentScreen == CurrentScreen.CustomizeScreen)
            {
                CustomizeScreen.Draw(spriteBatch);
            }
            else if (currentScreen == CurrentScreen.ArenaSelection)
            {
                ArenaSelection.Draw(spriteBatch);
            }
            else if (currentScreen == CurrentScreen.ShopScreen)
            {
                ShopScreen.Draw(spriteBatch);
            }
        }

        static public void ChangeToMainMenu()
        {
            MainMenu.Update();
            currentScreen = Screens.CurrentScreen.MainMenu;
        }

        static public void ChangeToSignInMenu()
        {
            SigninMenu.Update();
            currentScreen = Screens.CurrentScreen.SigninMenu;
        }

        static public void ChangeToArenaSelection()
        {
            ArenaSelection.JustOpened = true;
            ArenaSelection.Update();
            currentScreen = Screens.CurrentScreen.ArenaSelection;
        }

        static public void ChangeToPlayScreen(Arena arenaToUse)
        {
            currentScreen = Screens.CurrentScreen.PlayScreen;
            GameEngine.Start(arenaToUse);
        }

        static public void ChangeToShopScreen(int sender)
        {
            currentScreen = Screens.CurrentScreen.ShopScreen;
            ShopScreen.Open(sender);
        }

        static public void ChangeToCustomizeScreen(int sender)
        {
            currentScreen = Screens.CurrentScreen.CustomizeScreen;
            CustomizeScreen.Open(sender);
            CustomizeScreen.Update();
        }

        static public void ChangeToSettingsScreen()
        {
            currentScreen = Screens.CurrentScreen.SettingsMenu;
            CreditsMenu.Update();
        }

        static public void Reset()
        {
            Initialize();
            MediaPlayer.Stop();

            //Sign all the players out of the game (not the xbox)
            for (int i = 0; i < 4; i++)
            {
                InfoPacket.PlayerProfiles[i] = null;
            }
        }
    }
}
