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
    class WoodenArena : Arena
    {
        public WoodenArena()
        {
            arenaTexture = new Sprite();

            drivingArea = new Rectangle(161, 68, 957, 583);
        }

        public void Initialize()
        {
            arenaTexture.Initialize(Vector2.Zero);

            textColour = new Color(128, 128, 128, 128);

            bounciness = .5f;

            name = "Wooden Arena";
        }

        public void LoadContent(ContentManager content)
        {
            arenaTexture.LoadContent(content.Load<Texture2D>("Graphics/Arenas/1"));
            backgroundSong = content.Load<Song>("Audio/Songs/Wooden Theme");
            hitWallSound = content.Load<SoundEffect>("Audio/Effects/In_Game/Wall_Collisions/Wooden_Hit");
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
