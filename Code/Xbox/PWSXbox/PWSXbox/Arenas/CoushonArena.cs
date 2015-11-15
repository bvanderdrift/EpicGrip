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
    class CoushonArena : Arena
    {
        public CoushonArena()
        {
            arenaTexture = new Sprite();

            drivingArea = new Rectangle(124, 98, 1030, 524);
        }

        public void Initialize()
        {
            //Initialize the arena texture
            arenaTexture.Initialize(Vector2.Zero);

            //Set the bounciness of the arena
            bounciness = .7f;

            //Set the name of the arena
            name = "Coushon Arena";

            //Intialize the colour to contrast with the background
            textColour = new Color(255, 0, 195, 128);
        }

        public void LoadContent(ContentManager content)
        {
            arenaTexture.LoadContent(content.Load<Texture2D>("Graphics/Arenas/3"));
            backgroundSong = content.Load<Song>("Audio/Songs/Coushon Theme");
            hitWallSound = content.Load<SoundEffect>("Audio/Effects/In_Game/Wall_Collisions/Rest_Hit");
        }

        public override void Update()
        {
            arenaTexture.Update();
            base.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            arenaTexture.Draw(spriteBatch);
        }
    }
}
