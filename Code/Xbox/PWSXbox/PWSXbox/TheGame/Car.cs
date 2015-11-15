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
using PWS.TheGame.Upgrades.Offensive;
using PWS.TheGame.Upgrades.Defensive;

namespace PWS.TheGame
{
    enum DriveState
    {
        Braking,
        Rolling,
        Gassing,
    }
    class Car
    {
        #region Veriables & Properties
        //Variable to indentify with a controller and profile
        int playerNumber;

        //Variable to identify what car is shown
        int carShown;

        //Array for textures of the different cars
        static Texture2D[] carTextures;
        static Texture2D[] windowTextures;

        //The actual sprite and window sprite
        Sprite carSprite;
        Sprite windowSprite;

        //Variable for the position of the car
        Vector2 position;

        //Float to represent the rotation
        float rotation;

        //Speed variables
        float speed;
        Vector2 splitSpeed;
        float maxSpeed;

        float acceleration;
        float maxAcceleration;
        float resistance;

        float rotationSpeed;

        //Damage variables of the car
        float damageFactor;
        const float stdFactor = 1;
        int damageDealt;

        //Rotatable Rectangle to set the bounds of the car
        RotatableRectangle bounds;

        //A Boolean for if the car is playercontrolled
        bool playerControlled;

        //AI Variables
        float simulatedLTSX;

        //Check if in examplemode or not
        bool inExampleMode;

        //Variables for the fumes
        Texture2D fumeTexture;
        List<Sprite> fumes;
        int timeToNextFume;
        const int fumePeriod = 25;
        const int fumeFadeSpeed = 8;
        const float shrinkSpeed = .02f;

        //Shadows
        Vector2 lastPosition;
        float lastRotation;

        //Health Variable
        int totalHealth;
        int currentHealth;

        //A variable representing the momentum speed split in X and Y
        Vector2 momentum;

        //Variable to keep track of the time the player is alive
        int msAlive;

        //Variable to represent the cars colour
        Color color;

        //Variable to check how long ago the player has fired a weapon
        int lastFire;

        //Variable for the current bumper
        Shield shield;

        //Variable for example bumper
        int bumperShown;

        //Variable for last health
        int lastHealth;

        #region Properties
        public int CarShown
        {
            get { return carShown; }
            set { carShown = value; }
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public RotatableRectangle Bounds
        {
            get { return bounds; }
            set { bounds = value; }
        }

        public float DamageFactor
        {
            get { return damageFactor; }
            set { damageFactor = value; }
        }

        public bool InExampleMode
        {
            get { return inExampleMode; }
            set { inExampleMode = value; }
        }

        public float MaxSpeed
        {
            get { return maxSpeed; }
        }

        public float MaxAcceleration
        {
            get { return maxAcceleration; }
        }

        public float MaxRotationSpeed
        {
            get { return rotationSpeed; }
        }

        //Property for health
        public int Health
        {
            get { return currentHealth; }
            set { currentHealth = value; }
        }

        //Property to get the time in game
        public int TimeAlive
        {
            get { return msAlive; }
        }

        //Property tot get the damage dealt
        public int DamageDealt
        {
            get { return damageDealt; }
            set { 
                damageDealt = value;
            }
        }

        //Property tot get max health
        public int MaxHealth
        {
            get { return totalHealth; }
        }

        //Property to get or set the cars colour
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        //Property to get the driver
        public int Driver
        {
            get { return playerNumber; }
        }

        //Property to get how long ago something was fired
        public int LastFire
        {
            get { return (int)(lastFire / 1000); }
            set { lastFire = value * 1000; }
        }

        //Property to get the current bumper
        public Shield Shield
        {
            get { return shield; }
        }

        //Property to change bumper shown
        public int BumperShown
        {
            get { return bumperShown; }
            set { bumperShown = value; }
        }

        //Property to get last health
        public int LastHealthT
        {
            get { return lastHealth; }
        }

        //Property to get Healt incl. Shield
        public int HealthT
        {
            get { return (currentHealth + shield.Health); }
        }
        #endregion
        #endregion

        public Car()
        {
            //Instantiate the texture array
            carTextures = new Texture2D[4];
            windowTextures = new Texture2D[4];

            //Instantiate the car sprite
            carSprite = new Sprite();
            windowSprite = new Sprite();

            //Instantiate the bounds
            bounds = new RotatableRectangle(0, 0, 80, 40);

            //Instantiate the shield
            shield = new Shield();
        }

        public void Initialize(int PlayerNumber)
        {
            playerNumber = PlayerNumber;
            carShown = 1;
            bumperShown = 0;

            //Initialize the sprites and adjust the origin
            carSprite.Initialize(Vector2.Zero);
            carSprite.Origin = new Vector2(40, 20);

            windowSprite.Initialize(Vector2.Zero);
            windowSprite.Origin = new Vector2(40, 20);

            //Initialize the split speed
            splitSpeed = Vector2.Zero;

            //Initialize the acceleration and speed;
            speed = 0;
            acceleration = 0;
            resistance = .05f;

            //Maxes binded with the car. This is for the first car
            maxSpeed = 7;
            maxAcceleration = .3f;
            rotationSpeed = .2f;

            //Set the car to playerControlled
            playerControlled = true;

            //AI Initialazations
            simulatedLTSX = 0;

            //Initialize everything for the fumes
            fumes = new List<Sprite>();

            //Initiaize the damage dealt integer
            damageDealt = 0;

            //Set the lastFire very high, so the player can fire immidiatly
            lastFire = 0;

            //Initialize the shield
            shield.Initialize();
        }

        public void LoadContent(ContentManager content)
        {
            for (int i = 0; i < carTextures.Length; i++)
            {
                carTextures[i] = content.Load<Texture2D>("Graphics/Cars/Car " + (i + 1));
                windowTextures[i] = content.Load<Texture2D>("Graphics/Cars/Car " + (i + 1) + " Window");
            }

            fumeTexture = content.Load<Texture2D>("Graphics/Cars/Fume");
            bounds.LoadContent(content);

            //Load the content of the shield
            shield.LoadContent(content);
        }

        public void Update()
        {
            //If in example mode, the speed needs to be the maxSpeed
            if (inExampleMode)
            {
                speed = 4;
            }

            #region Keep the rotation between 0 and 2PI
            //Keep the rotation between 0 and 2PI
            while (rotation > 2 * Math.PI)
            {
                rotation -= 2 * (float)Math.PI;
            }
            while (rotation < 0)
            {
                rotation += 2 * (float)Math.PI;
            }
            #endregion

            #region Update the sprites
            carSprite.Update();
            windowSprite.Update();
            #endregion

            lastPosition = position;
            lastRotation = rotation;
            GamePadState state = GamePad.GetState(InfoPacket.Players[playerNumber]);

            #region Change Speed, location and rotation to the player input and momentum
            //Change the speed to the player input
            if (playerControlled && GameEngine.State != EngineState.Paused)
            {
                if (state.Buttons.A == ButtonState.Pressed && GameEngine.State == EngineState.Playing && Health > 0)
                {
                    acceleration = (1 - (Math.Abs(speed) / maxSpeed)) * maxAcceleration;
                }
                else if (state.Buttons.B == ButtonState.Pressed && GameEngine.State == EngineState.Playing && Health > 0)
                {
                    if (speed > 0)
                    {
                        acceleration = -(1 - (Math.Abs(speed) / maxSpeed)) * maxAcceleration * 2;
                    }
                    else
                    {
                        acceleration = -(1 - (Math.Abs(speed) / (maxSpeed / 2))) * maxAcceleration;
                    }
                }
                else
                {
                    acceleration = -(speed / maxSpeed) * resistance;
                }

                if (GameEngine.State == EngineState.Playing && Health > 0)
                {
                    //Change the cars rotation
                    rotation += (int)(speed / Math.Abs(speed)) * state.ThumbSticks.Left.X * (rotationSpeed - 0.75f * rotationSpeed * (Math.Abs(speed) / maxSpeed)) * (Math.Abs(speed) / maxSpeed);
                }       
            }
            else
            {
                if (inExampleMode)
                {
                    //Keep the car rotating with the ratio 0.0075 to the speed
                    rotation -= .0075f * speed;

                    //Set the car stats to the current car used by the player
                    if (carShown == 0)
                    {
                        totalHealth = 100;
                        maxSpeed = 5;
                        maxAcceleration = .1f;
                        damageFactor = 1 * stdFactor;
                    }
                    else if (carShown == 1)
                    {
                        totalHealth = 200;
                        maxSpeed = 6;
                        maxAcceleration = .2f;
                        damageFactor = 2 * stdFactor;
                    }
                    else if (carShown == 2)
                    {
                        totalHealth = 400;
                        maxSpeed = 4;
                        maxAcceleration = .15f;
                        damageFactor = 6 * stdFactor;
                    }
                    else if (carShown == 3)
                    {
                        totalHealth = 300;
                        maxSpeed = 9;
                        maxAcceleration = .5f;
                        damageFactor = 2 * stdFactor;
                    }

                    //Load the current car texture
                    carSprite.LoadContent(Car.carTextures[carShown]);
                    windowSprite.LoadContent(Car.windowTextures[carShown]);

                    //Set good color
                    carSprite.Color = color;

                    //Change Shield
                    shield.SetExample(bumperShown);
                }
            }

            if (GameEngine.State != EngineState.Paused || inExampleMode)
            {
                speed += acceleration;

                //Now split the speed in a xSpeed an ySpeed relevant to the rotation
                splitSpeed.X = speed * (float)Math.Cos(rotation);
                splitSpeed.Y = speed * (float)Math.Sin(rotation);

                //Now set the new position!
                position += splitSpeed + momentum;
                
                //Make the momentum lose its kenetic power
                momentum = momentum / new Vector2(1.1f);
            }

            bounds.Rotation = rotation;
            bounds.Position = position;
            bounds.Update();
            #endregion

            if (GameEngine.State != EngineState.Paused)
            {
                #region Update the fumes
                //Time passed since the last fume, and update it
                timeToNextFume += InfoPacket.GameTime.ElapsedGameTime.Milliseconds;

                //If it is time for the next fume, add one to the list with the current position of the car.
                if (timeToNextFume >= fumePeriod && speed > 0)
                {
                    //Set up the actual sprite
                    Sprite fume = new Sprite();
                    fume.Initialize(position);
                    fume.LoadContent(fumeTexture);
                    fume.Origin = new Vector2(15);
                    fume.Rotation = rotation;
                    fume.Update();

                    //Add the sprite to the list
                    fumes.Add(fume);

                    //Reset the time
                    timeToNextFume = 0;
                }

                //Instatiate a list to delete any faded-away sprites
                List<Sprite> deletableSprites = new List<Sprite>();

                //Update every sprite in the fumes list
                foreach (Sprite fumeSprite in fumes)
                {
                    //If the fume sprite is nealry invisible, set it up to be deleted from the list
                    if (fumeSprite.Color.R < fumeFadeSpeed)
                    {
                        deletableSprites.Add(fumeSprite);
                    }
                    fumeSprite.Scale -= new Vector2(shrinkSpeed);

                    //Since all values need to be the same, I only need to use one value, and set all 4 of the colour values to this value
                    fumeSprite.Color = new Color(fumeSprite.Color.R - fumeFadeSpeed,
                        fumeSprite.Color.R - fumeFadeSpeed,
                        fumeSprite.Color.R - fumeFadeSpeed,
                        fumeSprite.Color.R - fumeFadeSpeed);
                }

                //Go past all the deletable sprites, and delete them
                foreach (Sprite fumeSprite in deletableSprites)
                {
                    fumes.Remove(fumeSprite);
                }
                #endregion
            }
            

            #region Check for collisions
            #region with a wall

            if (!inExampleMode)
            {
                Side outOfBoundsSide = bounds.CheckOutOfBounds(GameEngine.CurrentArena.Bounds);
                if (outOfBoundsSide != Side.None)
                {
                    position = lastPosition;
                    rotation = lastRotation;
                    speed = -speed * GameEngine.CurrentArena.Bounciness;
                    DecreaseHealth((int)((Math.Abs(speed) * stdFactor) / GameEngine.CurrentArena.Bounciness));

                    if (Math.Abs(speed) > .5f)
                    {
                        GameEngine.CurrentArena.HitWallSound.Play();
                    }
                }

            }

            #endregion
            #endregion

            //Update the time in game
            if (currentHealth > 0 && GameEngine.State == EngineState.Playing)
            {
                msAlive += InfoPacket.GameTime.ElapsedGameTime.Milliseconds;
            }
            lastFire += InfoPacket.GameTime.ElapsedGameTime.Milliseconds;

            #region Check if fire weapons/upgrades
            if (state.Triggers.Right > .3f && InfoPacket.PreviousStates[playerNumber].Triggers.Right <= .3f && GameEngine.State == EngineState.Playing && currentHealth > 0)
            {
                switch (InfoPacket.PlayerStatistics[playerNumber].OffensiveUpgradeUsing)
                {
                    case 1:
                        GameEngine.DropMine(this);
                        break;
                    case 2:
                        GameEngine.FireRocket(this);
                        break;
                    case 3:
                        GameEngine.FireCRocket(this);
                        break;
                    default:
                        break;
                }
            }
            #endregion

            //Update the current shield
            shield.Update(this);

            //Set the graphics of the car to the statistics after all the updates
            carSprite.Position = position;
            windowSprite.Position = position;

            carSprite.Rotation = rotation;
            windowSprite.Rotation = rotation;

            //Set shadow of health
            lastHealth = currentHealth + shield.Health;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the fumes
            foreach (Sprite fumeSprite in fumes)
            {
                fumeSprite.Draw(spriteBatch);
            }

            //Draw the shield
            shield.Draw(spriteBatch);

            //Draw the car
            carSprite.Draw(spriteBatch);

            //Draw the windows
            windowSprite.Draw(spriteBatch);

            //Draw the bounds(debugging)
            bounds.Draw(spriteBatch);
        }

        public void Set()
        {
            //Set the car showing
            carShown = InfoPacket.PlayerStatistics[playerNumber].CarUsing;

            //Set the spritetexture as the currently used cartexture by the player using this class.
            carSprite.LoadContent(carTextures[InfoPacket.PlayerStatistics[playerNumber].CarUsing]);
            windowSprite.LoadContent(windowTextures[InfoPacket.PlayerStatistics[playerNumber].CarUsing]);

            //Set the colour of the car
            color = InfoPacket.PlayerStatistics[playerNumber].CarColour;
            carSprite.Color = color;

            //Set the rotation of the sprite
            carSprite.Rotation = rotation;
            windowSprite.Rotation = rotation;
            
            //Set the car stats to the current car used by the player
            if (carShown == 0)
            {
                totalHealth = 100;
                maxSpeed = 5;
                maxAcceleration = .1f;
                damageFactor = 1.5f * stdFactor;
            }
            else if (carShown == 1)
            {
                totalHealth = 200;
                maxSpeed = 6;
                maxAcceleration = .2f;
                damageFactor = 2 * stdFactor;
            }
            else if (carShown == 2)
            {
                totalHealth = 300;
                maxSpeed = 4;
                maxAcceleration = .15f;
                damageFactor = 4 * stdFactor;
            }
            else if (carShown == 3)
            {
                totalHealth = 300;
                maxSpeed = 9;
                maxAcceleration = .5f;
                damageFactor = 2 * stdFactor;
            }

            rotationSpeed = maxSpeed * .05f;

            //Set health to full
            currentHealth = totalHealth;

            //Set damageDealt to 0
            damageDealt = 0;

            //Set time in game to 0
            msAlive = 0;

            //Set the speed to 0
            speed = 0;

            //Set the rotation speed
            rotationSpeed -= .0075f * maxSpeed;

            //Set the last fire very high
            lastFire = 100000;

            //Set the shield
            shield.Set(this);

            if (shield.Strenght != 0)
            {
                bounds = shield.Bounds;
            }
        }

        public void SetExample(int carNumber, int shieldNumber)
        {
            //Set the spritetexture as the currently used cartexture by the player using this class.
            carSprite.LoadContent(carTextures[carNumber]);
            windowSprite.LoadContent(windowTextures[carNumber]);

            //Set the car showing
            carShown = carNumber;

            //Set the colour of the car
            carSprite.Color = InfoPacket.PlayerStatistics[playerNumber].CarColour;

            //Set the rotation of the sprite
            carSprite.Rotation = rotation;
            windowSprite.Rotation = rotation;

            //Set the shield
            shield.Strenght = shieldNumber;
            shield.Set(this);
        }

        //Method to call when this car is going to a PC-controlled state
        //It will make the update method ignore any player input
        public void MakeAI()
        {
            playerControlled = false;
        }

        //Method to make the car drive circles
        public void DriveExampleCircles()
        {
            speed = maxSpeed;
            simulatedLTSX = -1;
            inExampleMode = true;
        }

        //Method to check for collision
        public void CheckCollision(ref Car otherCar, int precision)
        {
            Vector2 collisionPosition = bounds.Intersects(otherCar.bounds, precision);

            if (collisionPosition != Vector2.Zero)
            {
                //Set the last rotation even if there is no front collision
                //rotation = lastRotation;
                //otherCar.rotation = otherCar.lastRotation;

                #region Check if this car does damage
                //First, I want the angle of impact from both cars
                float dx = collisionPosition.X - position.X;
                float dy = collisionPosition.Y - position.Y;

                float angleOfImpact = (float)Math.Atan(dy / dx);

                //If dx is lower than 0, the tanges returns the angle on the wrong side of the
                //Unit circle, this needs to be corrected
                if (dx < 0)
                {
                    angleOfImpact += (float)Math.PI;
                }

                //Set the range of the angle between 0 and 2*PI
                while (angleOfImpact > 2 * Math.PI)
                {
                    angleOfImpact -= 2 * (float)Math.PI;
                }

                while (angleOfImpact < 0)
                {
                    angleOfImpact += 2 * (float)Math.PI;
                }

                //We now have the correct angle of impact!
                //Lets adjust this to the cars angle, and store it in a new variable!
                float relativeAngleOfImpact = rotation - angleOfImpact;

                //Get the angle of one of the front corners
                //The other front corner has the negative angle
                float frontCornersAngle = (float)(Math.Atan((float)(bounds.Height / 2) / (float)(bounds.Width / 2)) + (Math.PI / (precision / 5)));

                while (relativeAngleOfImpact < -Math.PI)
                {
                    relativeAngleOfImpact += (float)Math.PI * 2;
                }

                //Check if the collision is between the front corners. This would mean the driver hit the other car on purpose
                //If the car is driving backwards, check for back corners.
                if (speed > 0)
                {
                    if (relativeAngleOfImpact < frontCornersAngle || relativeAngleOfImpact > (Math.PI * 2) - frontCornersAngle)
                    {
                        position = lastPosition;

                        if (speed > .5f)
                        {
                            otherCar.DecreaseHealth((int)(damageFactor * Math.Abs(speed)));
                            damageDealt += otherCar.lastHealth - otherCar.HealthT;

                            otherCar.TakeMomentum(splitSpeed + momentum);
                        }

                        speed = 0;
                    }
                    else
                    {
                        rotation = lastRotation;
                    }
                }
                else if (speed < 0)
                {
                    if (relativeAngleOfImpact < frontCornersAngle + Math.PI && relativeAngleOfImpact > (Math.PI * 2) - frontCornersAngle + Math.PI)
                    {
                        position = lastPosition;

                        if (speed < -.5f)
                        {
                            otherCar.DecreaseHealth((int)(damageFactor * Math.Abs(speed)));
                            damageDealt += otherCar.lastHealth - otherCar.HealthT;

                            otherCar.TakeMomentum(splitSpeed + momentum);
                        }

                        speed = 0;
                    }
                    else
                    {
                        rotation = lastRotation;
                    }
                }


                #endregion

                #region check if other car does damage
                //First, I want the angle of impact from both cars
                dx = collisionPosition.X - otherCar.position.X;
                dy = collisionPosition.Y - otherCar.position.Y;

                angleOfImpact = (float)Math.Atan(dy / dx);

                //If dx is lower than 0, the tanges returns the angle on the wrong side of the
                //Unit circle, this needs to be corrected
                if (dx < 0)
                {
                    angleOfImpact += (float)Math.PI;
                }

                //Set the range of the angle between 0 and 2*PI
                while (angleOfImpact > 2 * Math.PI)
                {
                    angleOfImpact -= 2 * (float)Math.PI;
                }

                while (angleOfImpact < 0)
                {
                    angleOfImpact += 2 * (float)Math.PI;
                }

                //We now have the correct angle of impact!
                //Lets adjust this to the cars angle, and store it in a new variable!
                relativeAngleOfImpact = otherCar.rotation - angleOfImpact;

                //Get the angle of one of the front corners
                //The other front corner has the negative angle
                frontCornersAngle = (float)(Math.Atan((float)(otherCar.bounds.Height / 2) / (float)(otherCar.bounds.Width / 2)) + (Math.PI / (precision / 5)));

                //Check if the collision is between the front corners. This would mean the driver hit the other car on purpose
                //If the car is driving backwards, check for back corners.
                if (otherCar.speed > 0)
                {
                    if (relativeAngleOfImpact < frontCornersAngle || relativeAngleOfImpact > (Math.PI * 2) - frontCornersAngle)
                    {
                        otherCar.position = otherCar.lastPosition;

                        if (otherCar.speed > .5f)
                        {
                            DecreaseHealth((int)(otherCar.damageFactor * Math.Abs(otherCar.speed)));
                            otherCar.damageDealt += lastHealth - HealthT;

                            TakeMomentum(otherCar.splitSpeed + otherCar.momentum);
                        }

                        otherCar.speed = 0;

                    }
                    else
                    {
                        otherCar.rotation = otherCar.lastRotation;
                    }
                }
                else if (otherCar.speed < 0)
                {
                    if (relativeAngleOfImpact < frontCornersAngle + Math.PI && relativeAngleOfImpact > -frontCornersAngle + Math.PI)
                    {
                        otherCar.position = otherCar.lastPosition;

                        if (otherCar.speed < -.5f)
                        {
                            DecreaseHealth((int)(otherCar.damageFactor * Math.Abs(otherCar.speed)));
                            otherCar.damageDealt += Math.Min((int)(otherCar.damageFactor * Math.Abs(otherCar.speed)), HealthT);

                                                    TakeMomentum(otherCar.splitSpeed + otherCar.momentum);
                        }

                        otherCar.speed = 0;

                    }
                    else
                    {
                        otherCar.rotation = otherCar.lastRotation;
                    }
                }
                #endregion
            }
        }

        public void TakeMomentum(Vector2 splitSpeed)
        {
            momentum = splitSpeed;
        }

        public void DecreaseHealth(int value)
        {
            if (shield.Health != 0)
            {
                shield.Damage(Math.Min(shield.Health, value));

                if (shield.Health == 0)
                {
                    //Set the bounds back to no bumper
                    bounds = new RotatableRectangle((int)position.X, (int)position.Y, 80, 40);
                    bounds.Rotation = rotation;

                    //Keep the health in bounds
                    shield.Health = 0;
                }
            }
            else
            {
                currentHealth -= value;

                if (currentHealth < 0)
                {
                    currentHealth = 0;
                }
            }
        }

        #region Hit By... Methods
        public void HitMine(Mine mine)
        {
            Vector2 momentum;

            //First, I want the angle of impact from both cars
            float dx = mine.Position.X - position.X;
            float dy = mine.Position.Y - position.Y;

            float angleOfImpact = (float)Math.Atan(dy / dx);

            //If dx is lower than 0, the tanges returns the angle on the wrong side of the
            //Unit circle, this needs to be corrected
            if (dx < 0)
            {
                angleOfImpact += (float)Math.PI;
            }

            //Set the range of the angle between 0 and 2*PI
            while (angleOfImpact > 2 * Math.PI)
            {
                angleOfImpact -= 2 * (float)Math.PI;
            }

            while (angleOfImpact < 0)
            {
                angleOfImpact += 2 * (float)Math.PI;
            }

            momentum = new Vector2((1f - (mine.ExplosionBounds.Radius / mine.DamageRange)) * -(float)Math.Cos(angleOfImpact) * 15, (1f - (mine.ExplosionBounds.Radius / mine.DamageRange)) * -(float)Math.Sin(angleOfImpact) * 15);

            speed = 0;

            if (momentum.X > 0)
            {
                TakeMomentum(momentum);
            }
        }

        public void HitRocket(Rocket rocket)
        {
            Vector2 momentum;

            //First, I want the angle of impact from both cars
            float dx = rocket.Position.X - position.X;
            float dy = rocket.Position.Y - position.Y;

            float angleOfImpact = (float)Math.Atan(dy / dx);

            //If dx is lower than 0, the tanges returns the angle on the wrong side of the
            //Unit circle, this needs to be corrected
            if (dx < 0)
            {
                angleOfImpact += (float)Math.PI;
            }

            //Set the range of the angle between 0 and 2*PI
            while (angleOfImpact > 2 * Math.PI)
            {
                angleOfImpact -= 2 * (float)Math.PI;
            }

            while (angleOfImpact < 0)
            {
                angleOfImpact += 2 * (float)Math.PI;
            }

            momentum = new Vector2((1f - (rocket.ExplosionBounds.Radius / rocket.DamageRange)) * -(float)Math.Cos(angleOfImpact) * 15, (1f - (rocket.ExplosionBounds.Radius / rocket.DamageRange)) * -(float)Math.Sin(angleOfImpact) * 15);

            speed = 0;
            TakeMomentum(momentum);
        }
        #endregion
    }
}
