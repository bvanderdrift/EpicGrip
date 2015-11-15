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
using System.Xml.Serialization;
using PWS.Controls;
using PWS.Screens;
using PWS.Popups;
using PWS.Arenas;
using PWS.Graphics;

namespace PWS
{
    public class PlayerStats
    {
        #region Variables & Properties
        #region Variables
        //The name of the player (GamerTag)
        string name;

        //Variable representing the amount of money the player has
        int money;

        //Arrays to check if the player owns the item
        bool[] hasArena;
        bool[] hasCar;
        bool[] hasOffensiveUpgrade;
        bool[] hasDefensiveUpgrade;

        //Integers to check which item the player is currently using
        int carUsing;
        int offensiveUpgradeUsing;
        int defensiveUpgradeUsing;

        //A random generator
        static Random r;

        //Colour of the car
        Color carColour;
        #endregion

        #region Properties
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int CarUsing
        {
            get { return carUsing; }
            set { carUsing = value; }
        }

        public int OffensiveUpgradeUsing
        {
            get { return offensiveUpgradeUsing; }
            set { offensiveUpgradeUsing = value; }
        }

        public int DefensiveUpgradeUsing
        {
            get { return defensiveUpgradeUsing; }
            set { defensiveUpgradeUsing = value; }
        }

        public Color CarColour
        {
            get { return carColour; }
            set { carColour = value; }
        }

        public int Money
        {
            get { return money; }
            set { money = value; }
        }

        public bool[] HasArena
        {
            get { return hasArena; }
            set { hasArena = value; }
        }

        public bool[] HasCar
        {
            get { return hasCar; }
            set { hasCar = value; }
        }

        public bool[] HasOffensiveUpgrade
        {
            get { return hasOffensiveUpgrade; }
            set { hasOffensiveUpgrade = value; }
        }

        public bool[] HasDefensiveUpgrade
        {
            get { return hasDefensiveUpgrade; }
            set { hasDefensiveUpgrade = value; }
        }
        #endregion
        #endregion

        //The constructor
        public PlayerStats()
        {
            name = "Unknown";

            //Set up all arrays, new accounts have the 1st car and 1st arena. No Upgrades yet
            hasArena = new bool[7];
            hasArena[0] = true;
            for (int i = 1; i < hasArena.Length; i++)
            {
                hasArena[i] = false;
            }

            hasCar = new bool[4];
            hasCar[0] = true;
            for (int i = 1; i < hasCar.Length; i++)
            {
                hasCar[i] = false;
            }

            hasDefensiveUpgrade = new bool[7];
            hasDefensiveUpgrade[0] = true;
            for (int i = 1; i < hasDefensiveUpgrade.Length; i++)
            {
                hasDefensiveUpgrade[i] = false;
            }

            hasOffensiveUpgrade = new bool[4];
            hasOffensiveUpgrade[0] = true;
            for (int i = 1; i < hasOffensiveUpgrade.Length; i++)
            {
                hasOffensiveUpgrade[i] = false;
            }

            //Initialie the random generator
            if (r == null)
            {
                r = new Random();
            }

            switch ((int)(r.NextDouble() * 5))
            {
                case 0:
                    carColour = Color.LightGreen;
                    break;
                case 1:
                    carColour = Color.Red;
                    break;
                case 2:
                    carColour = Color.Orange;
                    break;
                case 3:
                    carColour = Color.LightBlue;
                    break;
                case 4:
                    carColour = Color.HotPink;
                    break;
                default:
                    break;
            }

            //Player starts of without money;
            money = 0;
        }

        #region Methods to Unlock items
        public void UnlockArena(int Number)
        {
            hasArena[Number] = true;
        }

        public void UnlockCar(int Number)
        {
            hasCar[Number] = true;
            carUsing = Number;
        }

        public void UnlockDefensiveUpgrade(int Number)
        {
            hasDefensiveUpgrade[Number] = true;
            defensiveUpgradeUsing = Number;
        }

        public void UnlockOffensiveUpgrade(int Number)
        {
            hasOffensiveUpgrade[Number] = true;
            offensiveUpgradeUsing = Number;
        }

        public void UnlockAll()
        {
            for (int i = 0; i < hasArena.Length; i++)
            {
                hasArena[i] = true;
            }

            for (int i = 0; i < hasCar.Length; i++)
            {
                hasCar[i] = true;
            }

            for (int i = 0; i < hasDefensiveUpgrade.Length; i++)
            {
                hasDefensiveUpgrade[i] = true;
            }

            for (int i = 0; i < hasOffensiveUpgrade.Length; i++)
            {
                hasOffensiveUpgrade[i] = true;
            }
        }
        #endregion
    }
}
