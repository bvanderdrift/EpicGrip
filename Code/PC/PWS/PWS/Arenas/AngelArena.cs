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
    class AngelArena : Arena
    {
        public AngelArena()
        {
            arenaTexture = new Sprite();

            drivingArea = new Rectangle(126, 82, 1029, 557);
        }

        public void Initialize()
        {
            arenaTexture.Initialize(Vector2.Zero);
            bounciness = .6f;
            name = "Angel Arena";

            //Intialize the colour to contrast with the background
            textColour = new Color(0, 0, 0, 128);
        }

        public void LoadContent(ContentManager content)
        {
            arenaTexture.LoadContent(content.Load<Texture2D>("Graphics/Arenas/6"));
            backgroundSong = content.Load<Song>("Audio/Songs/Angel Theme");
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
