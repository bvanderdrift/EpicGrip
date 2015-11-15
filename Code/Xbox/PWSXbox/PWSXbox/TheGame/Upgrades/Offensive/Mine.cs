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
    enum MineState
    {
        Dropped,
        Activated,
        Exploded,
    }

    class Mine
    {
        //An integer to represent the time left until activation
        int countdown;

        //A circle to represent the damage radius
        Circle damageRadius;
        Circle bounds;

        //The sprites for the light and the mine
        Sprite mine;
        Sprite mineLight;

        //Sprite for the explosion
        Sprite explosion;

        //The colour of the light
        Color lightColor;

        //Position of the mine
        Vector2 position;

        //Variable for activation state
        MineState state;

        //Damage
        const float damage = 25;

        //Damage Range
        const float damageRange = 200;

        //Number of the owner
        int owner;

        //Static textures for the mine
        static Texture2D mineT;
        static Texture2D mineLightT;
        static Texture2D explosionT;

        //Speed to expand the explosion
        const float expansionSpeed = damageRange / 25;

        //Array of booleans to know if a player has been hit by the explosion already
        bool[] hasHit;

        //Time it takes to reload
        const int reloadTime = 5;

        //Properties
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Circle Bounds
        {
            get { return bounds; }
        }

        public Circle ExplosionBounds
        {
            get { return damageRadius; }
        }

        public MineState State
        {
            get { return state; }
        }

        public int Owner
        {
            get { return owner; }
        }

        public bool FullyBlown
        {
            get
            {
                if (damageRadius.Radius >= damageRange)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public float Damage
        {
            get { return damage; }
        }

        public bool[] HasHit
        {
            get { return hasHit; }
            set { hasHit = value; }
        }

        public float DamageRange
        {
            get { return damageRange; }
        }

        public int ReloadTime
        {
            get { return reloadTime; }
        }

        public Mine()
        {
            //Instantiate the damage radius, mine sprites & bounds of the mine
            damageRadius = new Circle(16, Vector2.Zero);
            mine = new Sprite();
            mineLight = new Sprite();
            bounds = new Circle(16, Vector2.Zero);
            explosion = new Sprite();

            //Instantiate the array
            hasHit = new bool[4];
        }

        public void Initialize()
        {
            //Set the countdown to 3 seconds (3000 milliseconds)
            countdown = 3000;

            //Set the lightColor to white
            lightColor = Color.White;

            //Initialize the sprites
            mine.Initialize(Vector2.Zero);
            mine.Origin = new Vector2(16);
            mineLight.Initialize(Vector2.Zero);
            mineLight.Origin = new Vector2(16);

            explosion.Initialize(Vector2.Zero);
            explosion.Origin = new Vector2(128);

            damageRadius.Initialize();

            //Initialize the hasHit array
            for (int i = 0; i < 4; i++)
            {
                hasHit[i] = false;
            }
        }

        static public void LoadContentStatic(ContentManager content)
        {
            //Load the sprites
            mineT = content.Load<Texture2D>("Graphics/InGameGraphics/Offensive Items/Mine/Mine");
            mineLightT = content.Load<Texture2D>("Graphics/InGameGraphics/Offensive Items/Mine/Mine_Light");
            explosionT = content.Load<Texture2D>("Graphics/InGameGraphics/Offensive Items/Explosion");

            Circle.LoadContentStatic(content);
        }

        public void LoadContent()
        {
            mine.LoadContent(mineT);
            mineLight.LoadContent(mineLightT);
            explosion.LoadContent(explosionT);
        }

        public void Update()
        {
            //Update the sprites
            mine.Update();
            mineLight.Update();
            explosion.Update();

            //Update the positions;
            mine.Position = position;
            mineLight.Position = position;
            explosion.Position = position;

            //Update the circles
            damageRadius.Position = position;
            bounds.Position = position;

            //Set the colour of the light
            mineLight.Color = lightColor;

            if (state == MineState.Dropped)
            {
                //Count down
                countdown -= InfoPacket.GameTime.ElapsedGameTime.Milliseconds;

                //Set the colour of the mine every half second
                if (countdown % 1000 > 500)
                {
                    lightColor = Color.LightGreen;
                }
                else
                {
                    lightColor = Color.White;
                }

                //If countdown is 0 (3s are over), activate the mine
                if (countdown <= 0)
                {
                    countdown = 0;
                    state = MineState.Activated;
                }
            }
            else if (state == MineState.Activated)
            {
                //Change the colour to red
                lightColor = Color.Red;
            }
            else if (state == MineState.Exploded)
            {
                //Make the damage radius bigger
                damageRadius.Radius += expansionSpeed;

                //Set the right scale to the damageRadius
                explosion.Scale = new Vector2(damageRadius.Radius / 128);

                //Make the explosion effect fade out
                byte value = (byte)(255f * (1f - (damageRadius.Radius / damageRange)));
                explosion.Color = new Color(value, value, value, value);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the mine items
            mine.Draw(spriteBatch);
            mineLight.Draw(spriteBatch);
        }

        public void PostDraw(SpriteBatch spriteBatch)
        {
            //Draw the explosion
            explosion.Draw(spriteBatch);


            damageRadius.Draw(spriteBatch);
        }

        public void Drop(Vector2 position, int owner)
        {
            //Set the position of the mine to the current position
            this.position = position;

            //Set the right state
            state = MineState.Dropped;

            //Set the owner
            this.owner = owner;

            //Make the explosion sprite invisible
            explosion.Visible = false;
        }

        public void Explode()
        {
            //Set the mine invisible
            mine.Visible = false;
            mineLight.Visible = false;

            //Set the explosion to visible
            explosion.Visible = true;

            //Set the state right
            state = MineState.Exploded;

            //Set the explosion to the right scale
            explosion.Scale = new Vector2(damageRadius.Radius / 128);

            //Trigger the explode sound
            AudioEngine.PlayExplosion();
        }
    }
}
