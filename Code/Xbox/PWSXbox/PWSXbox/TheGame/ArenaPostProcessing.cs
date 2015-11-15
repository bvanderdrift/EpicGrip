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
using PWS.Arenas;
using PWS.Graphics;
using PWS;

namespace PWS.TheGame
{
    //A class to postprocess for custom classes
    //For example, draw item OVER the cars, draw animated items OUTSIDE of the driving space
    class ArenaPostProcessing
    {
        #region Variables & Properties
        //Name to identify what arena is being used at the moment
        static string arenaName;

        //Angel Arena post processing items
        static Sprite leftHalo;
        static Sprite rightHalo;

        //NightClub post processing items
        static Sprite[] lights;
        #endregion

        static public void Instantiate()
        {
            leftHalo = new Sprite();
            rightHalo = new Sprite();

            lights = new Sprite[100];
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i] = new Sprite();
            }
        }

        static public void Initialize()
        {
            //Initialization for Angel Arena
            leftHalo.Initialize(new Vector2(67, 40));
            rightHalo.Initialize(new Vector2(1060, 40));


            #region Initialization for NightClub Arena
            //Random for random light placement
            Random random = new Random();
            for (int i = 0; i < lights.Length; i++)
            {
                int whichArea = (int)(4 * random.NextDouble());

                float xPos;
                float yPos;

                //whichArea areas:
                //0 = left
                //1 = top
                //2 = right
                //3 = bottom
                if (whichArea == 0)
                {
                    yPos = 720 * (float)random.NextDouble();
                    xPos = 160 * (float)random.NextDouble();
                }
                else if(whichArea == 1)
                {
                    yPos = 100 * (float)random.NextDouble();
                    xPos = 1280 * (float)random.NextDouble();
                }
                else if (whichArea == 2)
                {
                    yPos = 720 * (float)random.NextDouble();
                    xPos = 1115 + 160 * (float)random.NextDouble();
                }
                else
                {
                    yPos = 620 + 100 * (float)random.NextDouble();
                    xPos = 1280 * (float)random.NextDouble();
                }

                lights[i].Initialize(new Vector2(xPos, yPos));

                //Colour selection
                int whichColour = (int)(3 * random.NextDouble());

                if (whichColour == 0)
                {
                    lights[i].Color = Color.Green;
                }
                else if (whichColour == 1)
                {
                    lights[i].Color = Color.Red;
                }
                else if (whichColour == 2)
                {
                    lights[i].Color = Color.Blue;
                }

                //Set origin to centre of the spotlight
                lights[i].Origin = new Vector2(32);
            }
            #endregion
        }

        static public void LoadContent(ContentManager content)
        {
            leftHalo.LoadContent(content.Load<Texture2D>("Graphics/Arenas/PostProcessing/Angel Arena/Halo Left"));
            rightHalo.LoadContent(content.Load<Texture2D>("Graphics/Arenas/PostProcessing/Angel Arena/Halo Right"));

            Texture2D light = content.Load<Texture2D>("Graphics/Arenas/PostProcessing/NightClub Arena/SpotLight");
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].LoadContent(light);
            }
        }

        static public void Update()
        {
            if (arenaName == "Angel Arena")
            {
                leftHalo.Update();
                rightHalo.Update();
            }

            if (arenaName == "Night Club Arena")
            {
                for (int i = 0; i < lights.Length; i++)
                {
                    lights[i].Update();
                    lights[i].Scale = new Vector2(.5f + (float)Math.Sin((float)i + (float)InfoPacket.GameTime.TotalGameTime.TotalMilliseconds / 500));
                    lights[i].Rotation = (float)Math.Sin((float)i + (float)InfoPacket.GameTime.TotalGameTime.TotalMilliseconds / 500);
                }
            }
        }

        static public void Draw(SpriteBatch spriteBatch)
        {
            if (arenaName == "Angel Arena")
            {
                leftHalo.Draw(spriteBatch);
                rightHalo.Draw(spriteBatch);
            }

            if (arenaName == "Night Club Arena")
            {
                for (int i = 0; i < lights.Length; i++)
                {
                    lights[i].Draw(spriteBatch);
                }
            }
        }

        static public void SetArena(Arena currentArena)
        {
            arenaName = currentArena.Name;
        }
    }
}
