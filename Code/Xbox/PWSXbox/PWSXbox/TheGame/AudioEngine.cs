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

namespace PWS.TheGame
{
    class AudioEngine
    {
        //The sound effects
        static SoundEffect explosion;
        static SoundEffect collision;

        //Variable for volume
        static int volume;

        static public void Instantiate()
        {

        }

        static public void Initialize()
        {
            volume = 1;
        }

        static public void LoadContent(ContentManager content)
        {
            explosion = content.Load<SoundEffect>("Audio/Effects/In_Game/Blast");
        }

        static public void Update()
        {
            SoundEffect.MasterVolume = volume;
        }

        static public void PlayExplosion()
        {
            explosion.Play();
        }
    }
}
