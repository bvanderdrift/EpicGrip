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
using PWS.TheGame.Upgrades.Offensive;

namespace PWS.TheGame
{
    enum EngineState
    {
        Starting,
        Playing,
        Paused,
        Ended,
    }

    class GameEngine
    {
        #region Variables & Properties

        //A variable to contain the current arena
        static Arena currentArena;

        //An array for all the cars
        static Car[] cars;

        //A integer to represent the current amount of cars in the game
        static int carsInGame;

        //Font for HP
        static SpriteFont hpFont;

        //A enumeration to represent the state the engine is in
        static EngineState engineState;

        //A sprite to use as fader
        static Sprite fader;

        //Variables for the ending screen
        static string endingTitle;
        static SpriteFont endingFont;
        static SpriteFont endingTitleFont;
        static string[] endingTexts;
        static string[] moneyGainedTexts;
        static Sprite bButton;
        static Sprite xButton;
        
        //A variable to represent the time played
        static int totalTimePlayed;

        //List of upgrades
        static List<Mine> mines;
        static List<Rocket> rockets;
        static List<CRocket> cRockets;

        //Startdistance from walls
        static float wallOffSet = 70;

        static public Arena CurrentArena
        {
            get { return currentArena; }
            set { currentArena = value; }
        }

        static public EngineState State
        {
            get { return engineState; }
        }
        #endregion

        static public void Instantiate()
        {
            //Instantiate the arena post processing class
            ArenaPostProcessing.Instantiate();

            //Instantiate the pause screen
            PauseMenu.Instantiate();

            //Instantiate the b-button
            bButton = new Sprite();
            xButton = new Sprite();

            //Instantiate the fader
            fader = new Sprite();

            cars = new Car[4];
            //Instantiate the cars
            for (int i = 0; i < cars.Length; i++)
            {
                cars[i] = new Car();
            }

            //Instantiate the upgrade lists
            mines = new List<Mine>();
            rockets = new List<Rocket>();
            cRockets = new List<CRocket>();

            //Instantiate the audioengine
            AudioEngine.Instantiate();
        }

        static public void Initialize()
        {
            ArenaPostProcessing.Initialize();

            //Initialize the pause menu
            PauseMenu.Initialize();

            //Initialize the fader
            fader.Initialize(Vector2.Zero);
            
            //Initialize the cars
            for (int i = 0; i < cars.Length; i++)
            {
                cars[i].Initialize(i);
            }

            //Initialize the audio engine
            AudioEngine.Initialize();
        }

        static public void LoadContent(ContentManager content)
        {
            ArenaPostProcessing.LoadContent(content);

            //Load the content of the pauze menu
            PauseMenu.LoadContent(content);

            //Load the fader
            fader.LoadContent(content.Load<Texture2D>("Graphics/Shade"));

            //Load the fonts
            hpFont = content.Load<SpriteFont>("Graphics/InGameGraphics/HPFont");
            endingTitleFont = content.Load<SpriteFont>("Graphics/InGameGraphics/EndingTitleFont");
            endingFont = content.Load<SpriteFont>("Graphics/InGameGraphics/EndingFont");

            //Load content of the cars
            for (int i = 0; i < 4; i++)
            {
                cars[i].LoadContent(content);
            } 

            //Load the buttons and initialize them here, because they need the endingFont to initialize
            bButton.Initialize(new Vector2(640 - 32 - endingFont.MeasureString("Main Menu").X / 2 - 100, 650));
            xButton.Initialize(new Vector2(640 - 32 - endingFont.MeasureString("Rematch!").X / 2 + 100, 650));

            bButton.LoadContent(content.Load<Texture2D>("Graphics/XboxButtons/32/facebutton_b"));
            xButton.LoadContent(content.Load<Texture2D>("Graphics/XboxButtons/32/facebutton_x"));

            //Load the upgrades
            Mine.LoadContentStatic(content);
            Rocket.LoadContentStatic(content);

            //Load the audio
            AudioEngine.LoadContent(content);
        }

        static public void Update()
        {
            //Get the GamePadState of player 1
            GamePadState player1 = GamePad.GetState(InfoPacket.Players[0]);

            //Update current arena
            currentArena.Update();

            #region Update cars and car statistics


            //Check collisions
            for (int i = 0; i < carsInGame; i++)
            {
                for (int j = i + 1; j < carsInGame; j++)
                {
                    cars[i].CheckCollision(ref cars[j], 10);
                }
            }
            //Update the cars
            for (int i = 0; i < carsInGame; i++)
            {
                cars[i].Update();
            }
            //Check how many total loss cars there are and correct their health
            int totalLossCars = 0;
            for (int i = 0; i < carsInGame; i++)
            {
                if (cars[i].Health <= 0)
                {
                    cars[i].Health = 0;
                    totalLossCars++;
                }
            }

            #region Check if there is winner
            //Check if there is winner
            if (totalLossCars >= carsInGame - 1 && engineState != EngineState.Ended)
            {
                for (int i = 0; i < carsInGame; i++)
                {
                    if (cars[i].TimeAlive > totalTimePlayed)
                    {
                        totalTimePlayed = cars[i].TimeAlive;
                    }

                    if (cars[i].Health > 0)
                    {
#if XBOX
                        End(InfoPacket.PlayerProfiles[i].Gamertag);
#endif

#if WINDOWS
                        End("Player " + (i + 1));
#endif
                        break;
                    }
                }

                if (engineState != EngineState.Ended)
                {
                    End("Nobody");
                }
            }
            #endregion
            #endregion

            ArenaPostProcessing.Update();

            int fadeSpeed = 3;

            //Update the fader
            fader.Update();

            #region Update the fader according to the state
            if (engineState == EngineState.Starting)
            {
                fader.Color = new Color(fader.Color.R - fadeSpeed,
                    fader.Color.G - fadeSpeed,
                    fader.Color.B - fadeSpeed,
                        fader.Color.A - fadeSpeed);

                //If the fader is nearly gone, start the game
                if (fader.Color.A < fadeSpeed)
                {
                    StartPlay();
                }
            }
            else if (engineState == EngineState.Ended)
            {
                //When the game has ended, I want the background to be darkened
                fader.Color = new Color(100, 100, 100, 100);
            }
            else if (engineState == EngineState.Paused)
            {
                //When the game is paused, I want the background to be darkened
                fader.Color = new Color(100, 100, 100, 100);
            }
            else if (engineState == EngineState.Playing)
            {
                fader.Color = new Color(0, 0, 0, 0);
            }
            #endregion

            #region Code for when the results are shown

            if (engineState == EngineState.Ended)
            {
                for (int i = 0; i < carsInGame; i++)
                {
#if WINDOWS
                    endingTexts[i] = "Player" + (i + 1) + ":" + "\n" +
                        "Time Alive: " + (int)(cars[i].TimeAlive / 1000) + " seconds" + "\n" +
                        "Damage Dealt: " + cars[i].DamageDealt + "\n" +
                        "Winner Bonus (Health Left): " + "\n" +
                        "Money Gained: " + "\n" +
                        "Current Money: ";

                    moneyGainedTexts[i] = "\n" +
                        "+" + ((int)(((float)cars[i].TimeAlive / (float)totalTimePlayed) * 100)).ToString() + "\n" +
                        "+" + cars[i].DamageDealt.ToString() + "\n" +
                        "+" + cars[i].Health + "\n" +
                        ((int)((cars[i].TimeAlive / totalTimePlayed) * 100) + cars[i].DamageDealt + cars[i].Health).ToString() + "\n" +
                        InfoPacket.PlayerStatistics[i].Money.ToString();

#endif

#if XBOX
                    endingTexts[i] = InfoPacket.PlayerProfiles[i].Gamertag + ":" + "\n" +
                        "Time Alive: " + (int)(cars[i].TimeAlive / 1000) + " seconds" + "\n" +
                        "Damage Dealt: " + cars[i].DamageDealt + "\n" +
                        "Winner Bonus: " + "\n" +
                        "Money Gained: " + "\n" +
                        "Current Money: ";

                    moneyGainedTexts[i] = "\n" +
                        "+" + ((int)(((float)cars[i].TimeAlive / (float)totalTimePlayed) * 100)).ToString() + "\n" +
                        "+" + cars[i].DamageDealt.ToString() + "\n" +
                        "+" + (cars[i].Health + cars[i].Shield.Health) + "\n" +
                        (int)(((float)((float)cars[i].TimeAlive / (float)totalTimePlayed) * 100) + cars[i].DamageDealt + (float)cars[i].Health) +"\n" +
                        InfoPacket.PlayerStatistics[i].Money.ToString();
#endif
                }
            }

            #endregion

            #region After game player input code
            //Check for input when result screen is shown
            if (engineState == EngineState.Ended)
            {
                //If player presses B, go back to the main menu
                if (player1.Buttons.B == ButtonState.Pressed && InfoPacket.PreviousStates[0].Buttons.B == ButtonState.Released)
                {
                    //Go back to the main menu
                    ScreenManager.ChangeToMainMenu();

                    //Close the current arena
                    currentArena.Close();
                }

                //If player 1 presses X, restart the engine
                if (player1.Buttons.X == ButtonState.Pressed && InfoPacket.PreviousStates[0].Buttons.X == ButtonState.Released)
                {
                    //Restart the engine
                    Start(currentArena);
                }
            }
            #endregion

            #region Update pause screen
            //Check if the game should pause
            for (int i = 0; i < carsInGame; i++)
            {
                if (GamePad.GetState((PlayerIndex)i).Buttons.Start == ButtonState.Pressed && engineState == EngineState.Playing)
                {
                    engineState = EngineState.Paused;
                    PauseMenu.Open(i);
                }
            }
            

            //Update the pause screen
            if (engineState == EngineState.Paused)
            {
                PauseMenu.Update();
            }
            #endregion

            #region Update the Mines

            //Create a list for the mines to delete
            List<Mine> deletableMines = new List<Mine>();

            //Update each mine in the game
            foreach (Mine mine in mines)
            {
                mine.Update();

                if (mine.State == MineState.Activated)
                {
                    for (int i = 0; i < carsInGame; i++)
                    {
                        if (mine.Bounds.Intersects(cars[i].Bounds, 4) != Vector2.Zero && !(engineState == EngineState.Ended))
                        {
                            //Call the explode method
                            mine.Explode();

                            //Correct the cars stats
                            cars[i].DecreaseHealth((int)mine.Damage);

                            if (i != mine.Owner)
                            {
                                cars[mine.Owner].DamageDealt += (cars[i].LastHealthT - cars[i].HealthT);
                            }

                            //Call the car has hit mine method
                            cars[i].HitMine(mine);

                            //Set the hasHit value of this car for this mine to true
                            mine.HasHit[i] = true;
                        }
                    }
                }

                if (mine.State == MineState.Exploded)
                {
                    for (int i = 0; i < carsInGame; i++)
                    {
                        //Check if the car is hit by the blast and not yet hit
                        if (mine.ExplosionBounds.Intersects(cars[i].Bounds, 10) != Vector2.Zero &&
                            !mine.HasHit[i])
                        {
                            //Corrent the cars stats
                            cars[i].DecreaseHealth((int)(mine.Damage * (1f - (float)(mine.ExplosionBounds.Radius / mine.DamageRange))));

                            if (i != mine.Owner && engineState == EngineState.Ended)
                            {
                                cars[mine.Owner].DamageDealt += (cars[i].LastHealthT - cars[i].HealthT);
                            }

                            //Call the car has hit mine method
                            cars[i].HitMine(mine);

                            //Set the hasHit value of this car for this mine to true
                            mine.HasHit[i] = true;
                        }
                    }
                }

                if (mine.FullyBlown)
                {
                    deletableMines.Add(mine);
                }
            }

            //Delete all mines from the mines list that are in the deletableMines list
            foreach (Mine mine in deletableMines)
            {
                mines.Remove(mine);
            }
            #endregion

            #region Update the Rockets
            List<Rocket> deletableRockets = new List<Rocket>();

            foreach (Rocket rocket in rockets)
            {
                //Update the rockets
                rocket.Update();

                if (rocket.State == RocketState.Armed)
                {
                    for (int i = 0; i < carsInGame; i++)
                    {
                        if (rocket.Bounds.Intersects(cars[i].Bounds, 4) != Vector2.Zero && !(engineState == EngineState.Ended) && i != rocket.Owner)
                        {
                            //Call the explode method
                            rocket.Explode();

                            //Correct the cars stats
                            cars[i].DecreaseHealth((int)rocket.Damage);

                            if (i != rocket.Owner)
                            {
                                cars[rocket.Owner].DamageDealt += (cars[i].LastHealthT - cars[i].HealthT);
                            }

                            //Call the car has hit mine method
                            cars[i].HitRocket(rocket);

                            //Set the hasHit value of this car for this mine to true
                            rocket.HasHit[i] = true;
                        }
                    }
                }

                if (rocket.State == RocketState.Exploded)
                {
                    for (int i = 0; i < carsInGame; i++)
                    {
                        //Check if the car is hit by the blast and not yet hit
                        if (rocket.ExplosionBounds.Intersects(cars[i].Bounds, 10) != Vector2.Zero &&
                            !rocket.HasHit[i])
                        {
                            //Corrent the cars stats
                            cars[i].DecreaseHealth((int)(rocket.Damage * (1f - (float)(rocket.ExplosionBounds.Radius / rocket.DamageRange))));

                            if (i != rocket.Owner && engineState == EngineState.Ended)
                            {
                                cars[rocket.Owner].DamageDealt += (cars[i].LastHealthT - cars[i].HealthT);
                            }

                            //Call the car has hit mine method
                            cars[i].HitRocket(rocket);

                            //Set the hasHit value of this car for this mine to true
                            rocket.HasHit[i] = true;
                        }
                    }
                }

                //Check if the rocket should be deleted
                if (rocket.FullyBlown)
                {
                    deletableRockets.Add(rocket);
                }
            }

            //Remove every rocket in the rockets list that should be removed
            foreach (Rocket rocket in deletableRockets)
            {
                rockets.Remove(rocket);
            }
            #endregion

            #region Update the CRockets
            List<CRocket> deletableCRockets = new List<CRocket>();

            foreach (CRocket rocket in cRockets)
            {
                //Update the rockets
                rocket.Update();

                if (rocket.State == RocketState.Armed)
                {
                    for (int i = 0; i < carsInGame; i++)
                    {
                        if (rocket.Bounds.Intersects(cars[i].Bounds, 4) != Vector2.Zero && !(engineState == EngineState.Ended))
                        {
                            //Call the explode method
                            rocket.Explode();

                            //Correct the cars stats
                            cars[i].DecreaseHealth((int)rocket.Damage);

                            if (i != rocket.Owner)
                            {
                                cars[rocket.Owner].DamageDealt += (cars[i].LastHealthT - cars[i].Health);
                            }

                            //Call the car has hit mine method
                            cars[i].HitRocket(rocket);

                            //Set the hasHit value of this car for this mine to true
                            rocket.HasHit[i] = true;
                        }
                    }
                }

                if (rocket.State == RocketState.Exploded)
                {
                    for (int i = 0; i < carsInGame; i++)
                    {
                        //Check if the car is hit by the blast and not yet hit
                        if (rocket.ExplosionBounds.Intersects(cars[i].Bounds, 10) != Vector2.Zero &&
                            !rocket.HasHit[i])
                        {
                            //Corrent the cars stats
                            cars[i].DecreaseHealth((int)(rocket.Damage * (1f - (float)(rocket.ExplosionBounds.Radius / rocket.DamageRange))));

                            if (i != rocket.Owner && engineState == EngineState.Ended)
                            {
                                cars[rocket.Owner].DamageDealt += (cars[i].LastHealthT - cars[i].Health);
                            }

                            //Call the car has hit mine method
                            cars[i].HitRocket(rocket);

                            //Set the hasHit value of this car for this mine to true
                            rocket.HasHit[i] = true;
                        }
                    }
                }

                //Check if the rocket should be deleted
                if (rocket.FullyBlown)
                {
                    deletableCRockets.Add(rocket);
                }
            }

            //Remove every rocket in the rockets list that should be removed
            foreach (CRocket rocket in deletableCRockets)
            {
                cRockets.Remove(rocket);
            }
            #endregion

            //Update the buttons
            bButton.Update();
            xButton.Update();

            //Update the audio engine
            AudioEngine.Update();
        }

        static public void Draw(SpriteBatch spriteBatch)
        {
            currentArena.Draw(spriteBatch);

            #region Drawing of upgrades
            #region Offensive Upgrades
            //Draw the mines
            foreach (Mine mine in mines)
            {
                mine.Draw(spriteBatch);
            }

            //Draw the rockets
            foreach (Rocket rocket in rockets)
            {
                rocket.Draw(spriteBatch);
            }

            //Draw the controllable rockets
            foreach (CRocket cRocket in cRockets)
            {
                cRocket.Draw(spriteBatch);
            }
            #endregion
            #endregion

            #region Drawing the health stats and cars
            //Drawing the health stats.
            for (int i = 0; i < carsInGame; i++)
            {
                cars[i].Draw(spriteBatch);

                Vector2 textPosition;

                if (i == 0)
                {
                    textPosition = new Vector2(currentArena.Bounds.X + 40, currentArena.Bounds.Y + 40);
                }
                else if (i == 1)
                {
                    textPosition = new Vector2(currentArena.Bounds.X + currentArena.Bounds.Width - hpFont.MeasureString(cars[i].Health.ToString()).X - 40, currentArena.Bounds.Y + 40);
                }
                else if (i == 2)
                {
                    textPosition = new Vector2(currentArena.Bounds.X + 40, currentArena.Bounds.Y + currentArena.Bounds.Height - hpFont.MeasureString(cars[i].Health.ToString()).Y - 40);
                }
                else
                {
                    textPosition = new Vector2(currentArena.Bounds.X + currentArena.Bounds.Width - 40, currentArena.Bounds.Y + currentArena.Bounds.Height - 40) - hpFont.MeasureString(cars[i].Health.ToString());
                }

                if (cars[i].Shield != null)
                {
                    spriteBatch.DrawString(hpFont, (cars[i].Health + cars[i].Shield.Health).ToString(), textPosition, currentArena.TextColour);
                }
                else
                {
                    spriteBatch.DrawString(hpFont, cars[i].Health.ToString(), textPosition, currentArena.TextColour);
                }

            }
            #endregion

            #region PostDrawing of Upgrades
            #region Offensive Upgrades
            //Draw the explosions of the mines
            foreach (Mine mine in mines)
            {
                mine.PostDraw(spriteBatch);
            }

            //Draw the rockets
            foreach (Rocket rocket in rockets)
            {
                rocket.PostDraw(spriteBatch);
            }

            //Draw the controllable rockets
            foreach (CRocket cRocket in cRockets)
            {
                cRocket.PostDraw(spriteBatch);
            }
            #endregion
            #endregion

            ArenaPostProcessing.Draw(spriteBatch);

            //Updates to be called in a situation where the game is not actually playing
            if (engineState == EngineState.Ended || engineState == EngineState.Starting || engineState == EngineState.Paused)
            {
                fader.Draw(spriteBatch);

                //Everything to draw when the game has ended
                if (engineState == EngineState.Ended)
                {
                    //Draw the Title
                    spriteBatch.DrawString(endingTitleFont, endingTitle, new Vector2(640, 100) - endingTitleFont.MeasureString(endingTitle) / 2, Color.LightGreen);

                    //Draw the results for each car in game
                    for (int i = 0; i < carsInGame; i++)
                    {
                        Vector2 textPosition;

                        #region Set the text positions
                        if (i == 0)
                        {
                            textPosition = new Vector2(427, 300);
                        }
                        else if (i == 1)
                        {
                            textPosition = new Vector2(853, 300);
                        }
                        else if (i == 2)
                        {
                            textPosition = new Vector2(427, 550);
                        }
                        else
                        {
                            textPosition = new Vector2(853, 550);
                        }
                        #endregion

                        //A variable to contain the colour of the text
                        Color textColour;

                        if (cars[i].Health > 0)
                        {
                            textColour = Color.LightGreen;
                        }
                        else
                        {
                            textColour = new Color(255, 180, 180, 255);
                        }

                        spriteBatch.DrawString(endingFont, endingTexts[i], textPosition - new Vector2(100, 0) - endingFont.MeasureString(endingTexts[i]) / 2, textColour);
                        spriteBatch.DrawString(endingFont, moneyGainedTexts[i], textPosition + new Vector2(100, 0) - endingFont.MeasureString(moneyGainedTexts[i]) / 2, textColour);
                    }

                    //Draw the buttons & their texts
                    bButton.Draw(spriteBatch);
                    xButton.Draw(spriteBatch);
                    spriteBatch.DrawString(endingFont, "Main Menu", new Vector2(640 - endingFont.MeasureString("Main Menu").X / 2 - 100, 650), Color.White);
                    spriteBatch.DrawString(endingFont, "Rematch!", new Vector2(640 - endingFont.MeasureString("Rematch!").X / 2 + 100, 650), Color.White);
                }

                if (engineState == EngineState.Paused)
                {
                    PauseMenu.Draw(spriteBatch);
                }
            }
        }

        static public void Start(Arena currentArena)
        {
            //Set the currentarena, and open it
            GameEngine.currentArena = currentArena;
            GameEngine.currentArena.Open();

            //Set the engineState
            engineState = EngineState.Starting;

            //Set the shader to full colour;
            fader.Color = Color.White;

            //Set the engine state to playing
            engineState = EngineState.Playing;

#if WINDOWS
            //Code for debugging
            carsInGame = 2;
#endif

#if XBOX
            //Actual Code(for itiration is still nescecarry). 
            //Code for determainating how many cars should be in the game
            carsInGame = 0;
            for (int i = 0; i < cars.Length; i++)
            {
                if (InfoPacket.PlayerProfiles[i] != null)
                {
                    carsInGame++;
                }
            }
#endif

            //Set the positions for every car
            cars[0].Position = new Vector2(currentArena.Bounds.Left + wallOffSet, 720 / 2);
            cars[1].Position = new Vector2(currentArena.Bounds.Right - wallOffSet, 720 / 2);
            cars[2].Position = new Vector2(1280 / 2, currentArena.Bounds.Top + wallOffSet);
            cars[3].Position = new Vector2(1280 / 2, currentArena.Bounds.Bottom - wallOffSet);

            //Set the rotation for every car
            cars[0].Rotation = 0 * (float)(2 * Math.PI) / 4;
            cars[1].Rotation = 2 * (float)(2 * Math.PI) / 4;
            cars[2].Rotation = 1 * (float)(2 * Math.PI) / 4;
            cars[3].Rotation = 3 * (float)(2 * Math.PI) / 4;

            //Set up the cars in game
            for (int i = 0; i < carsInGame; i++)
            {
                //Actual appliance of the variables
                cars[i].Set();
            }

            ArenaPostProcessing.SetArena(currentArena);

            //Set up the EndingTexts array
            endingTexts = new string[carsInGame];
            moneyGainedTexts = new string[carsInGame];

            //Reset the total time playing
            totalTimePlayed = 0;

            //Go to playing immediatly for debugging
            StartPlay();

            //Clear the mine list
            mines.Clear();
            rockets.Clear();
            cRockets.Clear();
        }

        static public void StartPlay()
        {
            engineState = EngineState.Playing;
            fader.Color = new Color(0, 0, 0, 0);
        }

        //A method to be called when the game must end
        static public void End(string winner)
        {
            //Set the engineState to ended
            engineState = EngineState.Ended;

            //Set the title
            endingTitle = winner + " has won!";

            //Update the money of all the players in the game
            for (int i = 0; i < carsInGame; i++)
            {
                InfoPacket.PlayerStatistics[i].Money += (int)(((float)((float)cars[i].TimeAlive / (float)totalTimePlayed) * 100) + cars[i].DamageDealt + (float)cars[i].Health);
            }

            InfoPacket.Save();
        }

        static public void Continue()
        {
            engineState = EngineState.Playing;
        }

        static public void DropMine(Car sender)
        {
            Mine tempMine = new Mine();
            if (sender.LastFire >= tempMine.ReloadTime)
            {
                tempMine.Initialize();
                tempMine.LoadContent();

                mines.Add(tempMine);
                mines[mines.Count - 1].Drop(sender.Position, sender.Driver);

                //Set the last fire to 0
                sender.LastFire = 0;
            }
        }

        static public void FireRocket(Car sender)
        {
            Rocket tempRocket = new Rocket();
            if (sender.LastFire >= tempRocket.ReloadTime)
            {
                tempRocket.Initialize();
                tempRocket.LoadContent();

                rockets.Add(tempRocket);
                rockets[rockets.Count - 1].Fire(sender);

                //Set the last fire to 0
                sender.LastFire = 0;
            }
        }

        static public void FireCRocket(Car sender)
        {
            CRocket tempRocket = new CRocket();

            //Check if the player can fire again
            if (sender.LastFire >= tempRocket.ReloadTime)
            {
                tempRocket.Initialize();
                tempRocket.LoadContent();

                cRockets.Add(tempRocket);
                cRockets[cRockets.Count - 1].Fire(sender);

                //Set the last fire to 0
                sender.LastFire = 0;
            }
        }
    }
}
