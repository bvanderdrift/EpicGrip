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
using PWS.Geometrics;

namespace PWS.TheGame.Upgrades.Defensive
{
    class Shield
    {
        //The sprite
        Sprite shield;
        Sprite glow;

        //Health of the bumper
        int health;
        int maxHealth;

        //Bounds of the bumper
        RotatableRectangle bounds;

        //Strength of shield
        int strength;

        //Property for the bounds
        public RotatableRectangle Bounds
        {
            get { return bounds; }
        }

        //Property for the health
        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        //Property to get the strenght
        public int Strenght
        {
            get { return strength; }
            set { strength = value; }
        }

        public Shield()
        {
            //Instantiate the shield graphics
            shield = new Sprite();
            glow = new Sprite();
        }

        public Shield(Shield bumper)
        {
            this.health = bumper.health;
            this.shield = bumper.shield;
            this.glow = bumper.glow;
            this.maxHealth = bumper.maxHealth;
            this.bounds = bumper.bounds;
        }

        virtual public void Initialize()
        {
            //Initialie the shield sprite and glow sprite
            shield.Initialize(Vector2.Zero);
            glow.Initialize(Vector2.Zero);
        }

        virtual public void LoadContent(ContentManager content)
        {
            //Load the shield graphics
            shield.LoadContent(content.Load<Texture2D>("Graphics/Cars/Def_Upgr/Shield"));
            glow.LoadContent(content.Load<Texture2D>("Graphics/Cars/Def_Upgr/Shield_Glow"));

            //Set the origin of the shield to the middle
            shield.Origin = new Vector2(shield.Texture.Bounds.Width, shield.Texture.Bounds.Height) / 2;
            glow.Origin = new Vector2(glow.Texture.Bounds.Width, glow.Texture.Bounds.Height) / 2;

            //Set the bounds
            bounds = new RotatableRectangle(0, 0, shield.Texture.Width, shield.Texture.Height);
        }

        public void Update(Car owner)
        {
            //Update the sprites
            shield.Update();
            glow.Update();

            shield.Position = owner.Position;
            shield.Rotation = owner.Rotation;

            glow.Position = owner.Position;
            glow.Rotation = owner.Rotation;

            //Update the bounds
            bounds.Position = owner.Position;
            bounds.Rotation = owner.Rotation;

            //If health is 0, set the visibility to false
            shield.Visible = health != 0;
            glow.Visible = health != 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            glow.Draw(spriteBatch);
            shield.Draw(spriteBatch);
        }

        public void Damage(int damage)
        {
            health -= damage;
        }

        public void Set(Car owner)
        {
            //Get the strenght of the shield
            strength = InfoPacket.PlayerStatistics[owner.Driver].DefensiveUpgradeUsing;

            //Set the statistics to the shield upgrade using
            switch (strength)
            {
                case 0:
                    maxHealth = 0;
                    break;
                case 1:
                    maxHealth = 25;
                    shield.Color = new Color(0, 255, 0);
                    glow.Color = new Color(0, 128, 0);
                    break;
                case 2:
                    maxHealth = 50;
                    shield.Color = new Color(0, 255, 255);
                    glow.Color = new Color(0, 204, 202);
                    break;
                case 3:
                    maxHealth = 100;
                    shield.Color = new Color(0, 38, 255);
                    glow.Color = new Color(0, 19, 130);
                    break;
                case 4:
                    maxHealth = 125;
                    shield.Color = new Color(255, 119, 0);
                    glow.Color = new Color(255, 178, 0);
                    break;
                case 5:
                    maxHealth = 150;
                    shield.Color = new Color(255, 0, 0);
                    glow.Color = new Color(154, 0, 0);
                    break;
                case 6:
                    maxHealth = 200;
                    shield.Color = new Color(0, 0, 0);
                    glow.Color = new Color(255, 0, 0);
                    break;
                default:
                    break;
            }

            //Set the health to full
            health = maxHealth;
        }

        public void SetExample(int strength)
        {
            switch (strength)
            {
                case 0:
                    maxHealth = 0;
                    break;
                case 1:
                    maxHealth = 25;
                    shield.Color = new Color(0, 255, 0);
                    glow.Color = new Color(0, 128, 0);
                    break;
                case 2:
                    maxHealth = 50;
                    shield.Color = new Color(0, 255, 255);
                    glow.Color = new Color(0, 204, 202);
                    break;
                case 3:
                    maxHealth = 100;
                    shield.Color = new Color(0, 38, 255);
                    glow.Color = new Color(0, 19, 130);
                    break;
                case 4:
                    maxHealth = 125;
                    shield.Color = new Color(255, 119, 0);
                    glow.Color = new Color(255, 178, 0);
                    break;
                case 5:
                    maxHealth = 150;
                    shield.Color = new Color(255, 0, 0);
                    glow.Color = new Color(154, 0, 0);
                    break;
                case 6:
                    maxHealth = 200;
                    shield.Color = new Color(0, 0, 0);
                    glow.Color = new Color(255, 0, 0);
                    break;
                default:
                    break;
            }

            //Set the health to full
            health = maxHealth;
        }
    }
}
