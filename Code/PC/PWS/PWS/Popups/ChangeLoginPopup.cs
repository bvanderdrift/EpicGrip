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

namespace PWS.Popups
{
    //A class based on the popup. This is because I want to add extra code to the "End" popup
    //This is code is that the game resets when this popup ends!
    class ChangeLoginPopup : Popup
    {
        //I override the EndPopup method to add a command where the game will be reset.
        protected override void EndPopup()
        {
            ScreenManager.Reset();
            base.EndPopup();
        }
    }
}
