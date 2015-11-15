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
    class MetallicArena : Arena
    {
        public MetallicArena()
        {
            arenaTexture = new Sprite();

            drivingArea = new Rectangle(97, 67, 1085, 586);
        }

        public void Initialize()
        {
            arenaTexture.Initialize(Vector2.Zero);

            bounciness = .2f;

            name = "Metallic Arena";

            //Intialize the colour to contrast with the background
            textColour = new Color(128, 128, 128, 128);
        }

        public void LoadContent(ContentManager content)
        {
            arenaTexture.LoadContent(content.Load<Texture2D>("Graphics/Arenas/2"));
            backgroundSong = content.Load<Song>("Audio/Songs/Metallic Theme");
            hitWallSound = content.Load<SoundEffect>("Audio/Effects/In_Game/Wall_Collisions/Metallic_Hit");
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
