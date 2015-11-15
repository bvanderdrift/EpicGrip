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
using PWS.Graphics;
using PWS.Geometrics;

namespace PWS.TheGame.Upgrades.Offensive
{
    class CRocket : Rocket
    {
        float rotationSpeed;

        const int reloadTime = 15;

        public override int ReloadTime
        {
            get { return reloadTime; }
        }

        public override void Initialize()
        {
            base.Initialize();

            //Set the rotation speed
            rotationSpeed = .1f;

            //Set the rockets speed
            speed = 10;
        }

        public override void Update()
        {
            GamePadState state = GamePad.GetState(InfoPacket.Players[owner]);

            base.Update();

            rotation += state.ThumbSticks.Right.X * rotationSpeed;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            cRocket.Draw(spriteBatch);
        }

        public override void Fire(Car sender)
        {
            base.Fire(sender);

            //Set the time to arm
            armTime = 400;
        }
    }
}
