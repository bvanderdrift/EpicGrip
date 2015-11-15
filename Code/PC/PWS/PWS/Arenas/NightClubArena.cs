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
    class NightClubArena : Arena
    {
        public NightClubArena()
        {
            arenaTexture = new Sprite();

            drivingArea = new Rectangle(171, 110, 938, 500);
        }

        public void Initialize()
        {
            arenaTexture.Initialize(Vector2.Zero);
            bounciness = .6f;
            name = "Night Club Arena";

            //Intialize the colour to contrast with the background
            textColour = new Color(255, 97, 0, 128);
        }

        public void LoadContent(ContentManager content)
        {
            arenaTexture.LoadContent(content.Load<Texture2D>("Graphics/Arenas/7"));
            backgroundSong = content.Load<Song>("Audio/Songs/NightClub Theme");
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
