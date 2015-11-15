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
    enum RocketState
    {
        Ready,
        Fired,
        Armed,
        Exploded,
    }

    class Rocket
    {
        //Static textures, to load later
        static Texture2D rocketT;
        static Texture2D explosionT;
        static Texture2D tailT;
        static Texture2D cRocketT;

        //Sprites
        Sprite rocket;
        Sprite explosion;
        Sprite tail;
        protected Sprite cRocket;

        //Position of the rocket
        Vector2 position;

        //Rotation of the rocket
        protected float rotation;

        //Speed of the rocket
        protected float speed;

        //Bounds
        RotatableRectangle bounds;

        //Bounds of explosion
        Circle explosionBounds;

        //State of the rocket
        RocketState state;

        //Damage and explosion speed, and max size
        const float damage = 50;
        const float damageRange = 300;
        const float explosionSpeed = damageRange / 25;

        //Time to Arm
        protected int armTime;

        //Boolean to check if fully blown
        bool fullyBlown;

        //array to see which car has been hit already
        bool[] hasHit;

        //Int to see who is the owner
        protected int owner;

        //Seconds to reload;
        const int reloadTime = 10;

        //Properties
        public bool FullyBlown
        {
            get { return fullyBlown; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public RotatableRectangle Bounds
        {
            get { return bounds; }
        }

        public float Damage
        {
            get { return damage; }
        }

        public Circle ExplosionBounds
        {
            get { return explosionBounds; }
        }

        public bool[] HasHit
        {
            get { return hasHit; }
        }

        public float DamageRange
        {
            get { return damageRange; }
        }

        public int Owner
        {
            get { return owner; }
        }

        public RocketState State
        {
            get { return state; }
        }

        virtual public int ReloadTime
        {
            get { return reloadTime; }
        }

        public Rocket()
        {
            //Instantiate the sprites
            rocket = new Sprite();
            explosion = new Sprite();
            tail = new Sprite();
            cRocket = new Sprite();

            //Instantiate the bounds
            bounds = new RotatableRectangle(0, 0, 64, 16);
            explosionBounds = new Circle(16, Vector2.Zero);

            //Instantiate the array
            hasHit = new bool[4];
        }

        virtual public void Initialize()
        {
            //Initialize the sprites
            rocket.Initialize(Vector2.Zero);
            cRocket.Initialize(Vector2.Zero);

            //Set the state
            state = RocketState.Ready;

            //Set the origins
            rocket.Origin = new Vector2(32, 8);
            cRocket.Origin = new Vector2(32, 8);
            explosion.Origin = new Vector2(128);

            //Not fully blown
            fullyBlown = false;

            //Set the speed
            speed = 20f;

            for (int i = 0; i < 4; i++)
            {
                hasHit[i] = false;
            }
        }

        public static void LoadContentStatic(ContentManager content)
        {
            //Load the textures for the sprites
            rocketT = content.Load<Texture2D>("Graphics/InGameGraphics/Offensive Items/Rockets/Rocket");
            explosionT = content.Load<Texture2D>("Graphics/InGameGraphics/Offensive Items/Explosion");
            cRocketT = content.Load<Texture2D>("Graphics/InGameGraphics/Offensive Items/Rockets/CRocket");
        }

        public void LoadContent()
        {
            //Load the content of the sprites
            rocket.LoadContent(rocketT);
            explosion.LoadContent(explosionT);
            cRocket.LoadContent(cRocketT);
        }

        virtual public void Update()
        {
            //Update the sprites
            rocket.Update();
            explosion.Update();
            cRocket.Update();

            //Update the Bounds
            bounds.Update();
            bounds.Position = position;
            bounds.Rotation = rotation;
            explosionBounds.Position = position;

            //Update the sprites to the stats
            rocket.Position = position;
            cRocket.Position = position;
            rocket.Rotation = rotation;
            cRocket.Rotation = rotation;
            explosion.Position = position;

            //Update to the states
            if (state == RocketState.Fired)
            {
                //Get the X and Y speed
                Vector2 splitSpeed = new Vector2(speed * (float)Math.Cos(rotation), speed * (float)Math.Sin(rotation));

                //Add speed to position
                position += splitSpeed;

                //If out of arena, explode
                if (bounds.CheckOutOfBounds(GameEngine.CurrentArena.Bounds) != Side.None)
                {
                    Explode();
                }

                //Decrease armTime
                armTime -= InfoPacket.GameTime.ElapsedGameTime.Milliseconds;

                //If armtime is 0, arm
                if (armTime <= 0)
                {
                    Arm();
                }

            }
            else if (state == RocketState.Armed)
            {
                //Get the X and Y speed
                Vector2 splitSpeed = new Vector2(speed * (float)Math.Cos(rotation), speed * (float)Math.Sin(rotation));

                //Add speed to position
                position += splitSpeed;

                //If out of arena, explode
                if (bounds.CheckOutOfBounds(GameEngine.CurrentArena.Bounds) != Side.None)
                {
                    Explode();
                }
            }
            else if (state == RocketState.Exploded)
            {
                explosionBounds.Radius += explosionSpeed;

                //Set the right scale to the damageRadius
                explosion.Scale = new Vector2(explosionBounds.Radius / 128);

                byte value = (byte)(255f - 255 * (explosionBounds.Radius / damageRange));
                explosion.Color = new Color(value, value, value, value);

                if (255f - 255 * (explosionBounds.Radius / damageRange) <= 0)
                {
                    fullyBlown = true;
                }
            }
        }

        virtual public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the rocket
            rocket.Draw(spriteBatch);

        }

        public void PostDraw(SpriteBatch spriteBatch)
        {
            //Draw the explosion
            explosion.Draw(spriteBatch);
        }

        virtual public void Fire(Car sender)
        {
            //Set the position and rotation
            rotation = sender.Rotation;
            position = sender.Position;

            rocket.Position = position;
            cRocket.Position = position;
            explosion.Position = position;

            cRocket.Rotation = rotation;
            rocket.Rotation = rotation;
            bounds.Rotation = rotation;
            bounds.Position = position;

            //Set the state
            state = RocketState.Fired;

            //Set the armTime
            armTime = 0;

            //Set visibility
            rocket.Visible = true;
            cRocket.Visible = true;
            explosion.Visible = false;

            //Set the owner
            owner = sender.Driver;
        }

        public void Arm()
        {
            //Set the armTime
            armTime = 0;

            //Set the state
            state = RocketState.Armed;
        }

        public void Explode()
        {
            //Set the visibility
            rocket.Visible = false;
            cRocket.Visible = false;
            explosion.Visible = true;

            //Set the explosion bounds
            explosionBounds.Radius = 16;

            //Change the state
            state = RocketState.Exploded;

            //Trigger the explode sound
            AudioEngine.PlayExplosion();
        }
    }
}
