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
    class HellArena : Arena
    {
        public HellArena()
        {
            arenaTexture = new Sprite();

            drivingArea = new Rectangle(108, 82, 1062, 556);
        }

        public void Initialize()
        {
            arenaTexture.Initialize(Vector2.Zero);

            bounciness = .4f;

            name = "Hell Arena";

            //Intialize the colour to contrast with the background
            textColour = new Color(0, 85, 255, 128);
        }

        public void LoadContent(ContentManager content)
        {
            arenaTexture.LoadContent(content.Load<Texture2D>("Graphics/Arenas/4"));
            backgroundSong = content.Load<Song>("Audio/Songs/Hell Theme");
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
