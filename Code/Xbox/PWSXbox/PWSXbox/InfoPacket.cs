using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using PWS.Controls;

namespace PWS
{
    class InfoPacket
    {
        //A packet of information used regularly by the Update method. This is to prevent many parameters in the Update method.
        static GameTime gameTime;
        static PlayerIndex[] players;
        static Game theGame;

        //Gamerprofiles and playstats
        static SignedInGamer[] playerProfiles;
        static PlayerStats[] playerStatistics;

        //Boolean for if a popup is showing
        static bool popupShowing;

        //An array for the previous state of every controller
        static GamePadState[] prevStates;

        //A constant for the amount of arenas in the game
        static int amountOfArenas = 7;

        //Device to save the statistics of players
        static StorageDevice device;

        //Result variable
        static IAsyncResult result;

        static public GameTime GameTime
        {
            get { return gameTime; }
            set { gameTime = value; }
        }

        static public PlayerIndex[] Players
        {
            get { return players; }
            set { players = value; }
        }

        static public GamePadState[] PreviousStates
        {
            get { return prevStates; }
            set { prevStates = value; }
        }

        static public Game TheGame
        {
            get { return theGame; }
            set { theGame = value; }
        }

        static public SignedInGamer[] PlayerProfiles
        {
            get { return playerProfiles; }
            set { playerProfiles = value; }
        }

        static public bool PopupShowing
        {
            get { return popupShowing; }
            set { popupShowing = value; }
        }

        static public int AmountOfArenas
        {
            get { return amountOfArenas; }
        }

        static public PlayerStats[] PlayerStatistics
        {
            get { return playerStatistics; }
            set { playerStatistics = value; }
        }

        static public StorageDevice Device
        {
            get { return device; }
        }

        static public void Initialize(GameTime gameTime)
        {
            //Instantiate the arrays
            players = new PlayerIndex[4];
            playerProfiles = new SignedInGamer[4];
            playerStatistics = new PlayerStats[4];
            prevStates = new GamePadState[4];

            //Set the gameTime
            InfoPacket.gameTime = gameTime;

            //Set every player to the standard value
            //Set every profile to null (nothing)
            for (int i = 0; i < 4; i++)
            {
                players[i] = (PlayerIndex)i;
                playerProfiles[i] = null;
                playerStatistics[i] = new PlayerStats();
                prevStates[i] = new GamePadState();
            }

            //Instantiate theGame variable
            theGame = new Game();

            //Initialize the boolean
            popupShowing = false;

            //device = new StorageDevice[4];
        }

        static public void GetDevice()
        {
            if (device == null)
            {
                if (result != null)
                {
                    if (result.IsCompleted)
                    {
                        device = StorageDevice.EndShowSelector(result);
                    }
                }

                if (!Guide.IsVisible && device == null)
                {
                    result = StorageDevice.BeginShowSelector(0, null, null);
                }
            }

            //for (int i = 0; i < 4; i++)
            //{
            //    if (device[i] == null)
            //    {
            //        if (result != null)
            //        {
            //            if (result.IsCompleted)
            //            {
            //                device[i] = StorageDevice.EndShowSelector(result);
            //                result = null;
            //            } 
            //        }

            //        if (!Guide.IsVisible && device[i] == null)
            //        {
            //            result = StorageDevice.BeginShowSelector((PlayerIndex)i, null, null);
            //        }

            //        break;
            //    }
            //}
            
        }

        static public void Save()
        {
            for (int i = 0; i < 4; i++)
            {
                if (playerProfiles[i] != null)
                {
                    //Create a XML serializer
                    XmlSerializer serializer = new XmlSerializer(typeof(PlayerStats));

                    //Create space for a container
                    StorageContainer container;

                    //Open an container
                    result = device.BeginOpenContainer(playerProfiles[i].Gamertag, null, null);
                    result.AsyncWaitHandle.WaitOne();
                    container = device.EndOpenContainer(result);

                    //If the file already exists, delete it first
                    if (container.FileExists("Statistics.sav"))
                    {
                        container.DeleteFile("Statistics.sav");
                    }

                    //Create the file, and save the stream
                    Stream stream = container.CreateFile("Statistics.sav");

                    //Write in the file, and close everything
                    serializer.Serialize(stream, playerStatistics[i]);
                    stream.Close();
                    container.Dispose();
                }
            }
        }

        static public void Load()
        {
            for (int i = 0; i < 4; i++)
            {
                if (playerProfiles[i] != null)
                {
                    //Create a XML serializer
                    XmlSerializer serializer = new XmlSerializer(typeof(PlayerStats));

                    //Create space for a container
                    StorageContainer container;

                    //Open an container
                    result = device.BeginOpenContainer(playerProfiles[i].Gamertag, null, null);
                    result.AsyncWaitHandle.WaitOne();
                    container = device.EndOpenContainer(result);

                    //Check if there is a save game already, if so load it
                    if (container.FileExists("Statistics.sav"))
                    {
                        Stream stream = container.OpenFile("Statistics.sav", FileMode.Open);

                        playerStatistics[i] = (PlayerStats)serializer.Deserialize(stream);
                        stream.Close();
                    }

                    container.Dispose();
                }
            }
        }

        static public void Delete(int controller)
        {
            if (playerProfiles[controller] != null)
            {
                //Create space for a container
                StorageContainer container;

                //Open an container
                result = device.BeginOpenContainer(playerProfiles[controller].Gamertag, null, null);
                result.AsyncWaitHandle.WaitOne();
                container = device.EndOpenContainer(result);

                //Check if there is a save game already, if so load it
                if (container.FileExists("Statistics.sav"))
                {
                    container.DeleteFile("Statistics.sav");
                }

                container.Dispose();
            }
        }
    }
}
