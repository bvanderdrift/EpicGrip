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
using PWS.Controls;
using PWS.Screens;
using PWS.Popups;
using PWS.Graphics;

namespace PWS.Arenas
{
    class Arena
    {
        #region Variables & Properties
        //Variable for how bouncy the wall of the arena is
        protected float bounciness;

        //Rectangle to set the bounds of the driving area
        protected Rectangle drivingArea;

        //Background song of the arena
        protected Song backgroundSong;

        //Sound when a car hits the wall
        protected SoundEffect hitWallSound;

        //The actual graphics of the arena
        protected Sprite arenaTexture;

        //The sprite for falling object
        protected Sprite fallingObject;

        //Boolean to check if the arena is open
        protected bool opened;
        
        //Best colour for the text to be readable
        protected Color textColour;

        //Name of the arena
        protected string name;

        //Necesarry properties of these variables
        public float Bounciness
        {
            get { return bounciness; }
        }

        public Rectangle Bounds
        {
            get { return drivingArea; }
        }

        public Song ArenaTheme
        {
            get { return backgroundSong; }
        }

        public SoundEffect HitWallSound
        {
            get { return hitWallSound; }
        }

        public Color TextColour
        {
            get { return textColour; }
        }

        public Sprite Background
        {
            get { return arenaTexture; }
        }

        public string Name
        {
            get { return name; }
        }
        #endregion

        //A standard update method for all the arena's
        public virtual void Update()
        {
            if (opened && MediaPlayer.Volume < 1)
            {
                MediaPlayer.Volume += .0025f;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            arenaTexture.Draw(spriteBatch);
        }

        //Method to be called when arena is opened
        public void Open()
        {
            opened = true;
            MediaPlayer.Play(backgroundSong);
            MediaPlayer.Volume = 0;
        }

        //Method to be caled when arena is closed
        public void Close()
        {
            opened = false;
            MediaPlayer.Stop();
        }
    }
}
