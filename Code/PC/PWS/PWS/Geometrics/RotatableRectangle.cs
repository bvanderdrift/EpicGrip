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

namespace PWS.Geometrics
{
    enum Side
    {
        None,
        Right,
        Left,
        Top,
        Bottom,
    }
    class RotatableRectangle
    {
        Vector2[] corners;
        float rotation;
        Vector2 position;
        float[] phases;
        float[] startPhases;
        float radius;

        //Sizes of the rectangle
        int width;
        int height;

        //Temporary shit for checking if I done it right
        Vector2[] rectangleOne;
        Vector2[] rectangleTwo;

        Sprite dot;
        Sprite sDot;

        //Collision Position
        List<Vector2> collisionPositions;

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

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public float Radius
        {
            get { return radius; }
        }

        public Vector2[] Corners
        {
            get { return corners; }
        }

        public RotatableRectangle(int x, int y, int width, int height)
        {
            //Set the height & width
            this.height = height;
            this.width = width;

            //Instantiate the corners array
            corners = new Vector2[4];

            //Set every corner by hand, this can be done easily if the rotation is 0
            corners[0] = new Vector2(x - width / 2, y - height / 2);
            corners[1] = new Vector2(x - width / 2, y + height / 2);
            corners[2] = new Vector2(x + width / 2, y + height / 2);
            corners[3] = new Vector2(x + width / 2, y - height / 2);

            //Instantiate the phases and startPhases arrays
            phases = new float[4];
            startPhases = new float[4];

            //Set the position
            position = new Vector2(x, y);

            //Set the rotation, which always is 0 when beginning
            rotation = 0;

            //Calculate the radius of the circle the corners are on with Pythagoras
            radius = (float)Math.Sqrt((width / 2) * (width / 2) + (height / 2) * (height / 2));

            for (int i = 0; i < 4; i++)
            {
                //Get the delta X and delta Y of each corner 
                //according to the centre of the rectangle
                //These will be used as a "opposite" and 
                //"adjectent" in the tanges solving
                float dx = corners[i].X - position.X;
                float dy = corners[i].Y - position.Y;

                //Get the temporary phase of the corner. 
                //This will be the temporary phase in the bottem right corner
                float tempPhase = (float)Math.Atan(Math.Abs(dy) / Math.Abs(dx));

                if (dx > 0 && dy < 0)//Corner up right
                {
                    //The temporary phase is on the top half and not the bottom half
                    //So because this corner is also on the right
                    //It's just the negative
                    tempPhase = -tempPhase;
                }
                else if (dx < 0 && dy > 0)//Corner down left
                {
                    //This corner is on the bottem half as well, but then on the left
                    //It is the exact oppisite of the corner up right
                    //So I just add 1 PI.
                    tempPhase = (float)Math.PI - tempPhase;
                    phases[i] = tempPhase;
                    startPhases[i] = tempPhase;
                }
                else if (dx > 0 && dy > 0)//Corner down right
                {
                    //The temporary fase is already the phase of the corner
                    //in the bottom right quarter, so nothing to do here
                }
                else if (dx < 0 && dy < 0)//Corner up left
                {
                    //This corner is on the exact oppisite side of the corner
                    //down right, so I just substract PI.
                    tempPhase -= (float)Math.PI;
                }

                //Set the phases
                phases[i] = tempPhase;
                startPhases[i] = tempPhase;
            }

            dot = new Sprite();
            dot.Initialize(Vector2.Zero);
            dot.Origin = new Vector2(8);

            sDot = new Sprite();
            sDot.Initialize(Vector2.Zero);
            sDot.Origin = new Vector2(4);

            //Set up the collision list
            collisionPositions = new List<Vector2>();
        }

        public void LoadContent(ContentManager content)
        {
            dot.LoadContent(content.Load<Texture2D>("Graphics/Debug Stuff/Dot"));
            sDot.LoadContent(content.Load<Texture2D>("Graphics/Debug Stuff/sDot"));
        }

        public void Update()
        {
            dot.Update();
            sDot.Update();

            for (int i = 0; i < 4; i++)
            {
                //Set the new phase
                phases[i] = startPhases[i] + rotation;

                //Calculate the X and Y value of the corner
                corners[i] = new Vector2(radius * (float)Math.Cos(phases[i]) + position.X,
                    radius * (float)Math.Sin(phases[i]) + position.Y);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                collisionPositions.Clear();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //for (int i = 0; i < 4; i++)
            //{
            //    dot.Position = corners[i];
            //    dot.Draw(spriteBatch);
            //}

            //if (rectangleOne != null)
            //{
            //    for (int i = 0; i < rectangleOne.Length; i++)
            //    {
            //        sDot.Position = rectangleOne[i];
            //        sDot.Draw(spriteBatch);
            //        sDot.Position = rectangleTwo[i];
            //        sDot.Draw(spriteBatch);
            //    }
            //}

            //foreach (Vector2 position in collisionPositions)
            //{
            //    dot.Position = position;
            //    dot.Draw(spriteBatch);
            //}
        }

        public bool Intersects(Rectangle rectangle, int precision)
        {
            return false;
        }

        public Vector2 Intersects(RotatableRectangle rectangle, int precision)
        {
            Vector2[] edgesRect1 = new Vector2[precision * 4];
            Vector2[] edgesRect2 = new Vector2[precision * 4];

            #region Filling the cornersInOrder NEW METHOD
            #region Set up the corners in the correct order from top counter-clockwise
            //Set up the arrays for the corner in order
            Vector2[] cornersInOrder1 = new Vector2[4];
            Vector2[] cornersInOrder2 = new Vector2[4];

            //Create variables to keep track of what is the lowest point yet.
            //I set them to a value sure to be more right or lower, and a buffer of 10 pixels
            float highestYValue = position.Y + radius + 10;
            float mostLefttXValue = position.X + radius + 10;
            int nrOneCorner = 0;

            #region fill corners in order of this rectangle
            //Check every corner
            for (int i = 0; i < 4; i++)
            {
                //Is the corner equal to a previously measured Y value? (The dots are on the same height)
                //Then check if this corner is more left then the other
                if (corners[i].Y == highestYValue &&
                    corners[i].X < mostLefttXValue)
                {
                    highestYValue = corners[i].Y;
                    mostLefttXValue = corners[i].X;
                    nrOneCorner = i;
                }

                //If the y value of the current corner is lower than the last lowest y value
                //This corner is higher. so set this as the nr1 corner
                if (corners[i].Y < highestYValue)
                {
                    highestYValue = corners[i].Y;
                    mostLefttXValue = corners[i].X;
                    nrOneCorner = i;
                }
            }

            //Now fill in the corners in order
            int currentCorner = nrOneCorner;
            for (int i = 0; i < 4; i++)
            {
                cornersInOrder1[i] = corners[currentCorner];

                //Set the next corner as the current corner
                currentCorner++;

                //If we are currently getting the 5th corner, we will get an error.
                //The 5th corner should be the first, so:
                if (currentCorner > 3)
                {
                    currentCorner = 0;
                }
            }
            #endregion

            #region fill corners in order of the other rectangle
            highestYValue = rectangle.position.Y + rectangle.radius + 10;
            mostLefttXValue = rectangle.position.X + rectangle.radius + 10;
            nrOneCorner = 0;

            //Check every corner
            for (int i = 0; i < 4; i++)
            {
                //Is the corner equal to a previously measured Y value? (The dots are on the same height)
                //Then check if this corner is more left then the other
                if (rectangle.corners[i].Y == highestYValue &&
                    rectangle.corners[i].X < mostLefttXValue)
                {
                    highestYValue = rectangle.corners[i].Y;
                    mostLefttXValue = rectangle.corners[i].X;
                    nrOneCorner = i;
                }

                //If the y value of the current corner is lower than the last lowest y value
                //This corner is higher. so set this as the nr1 corner
                if (rectangle.corners[i].Y < highestYValue)
                {
                    highestYValue = rectangle.corners[i].Y;
                    mostLefttXValue = rectangle.corners[i].X;
                    nrOneCorner = i;
                }
            }

            //Now fill in the corners in order
            currentCorner = nrOneCorner;
            for (int i = 0; i < 4; i++)
            {
                cornersInOrder2[i] = rectangle.corners[currentCorner];

                //Set the next corner as the current corner
                currentCorner++;

                //If we are currently getting the 5th corner, we will get an error.
                //The 5th corner should be the first, so:
                if (currentCorner > 3)
                {
                    currentCorner = 0;
                }
            }
            #endregion
            #endregion
            #endregion

            #region Filling the edges arrays with the proper Vectors
            for (int i = 0; i < 4; i++)
            {
                #region Gaining the steps
                float xStep1, yStep1, xStep2, yStep2;

                //Getting the steps
                if (i + 1 == 4)
                {
                    xStep1 = (cornersInOrder1[0].X - cornersInOrder1[i].X) / precision;
                    yStep1 = (cornersInOrder1[0].Y - cornersInOrder1[i].Y) / precision;

                    xStep2 = (cornersInOrder2[0].X - cornersInOrder2[i].X) / precision;
                    yStep2 = (cornersInOrder2[0].Y - cornersInOrder2[i].Y) / precision;
                }
                else
                {
                    xStep1 = (cornersInOrder1[i + 1].X - cornersInOrder1[i].X) / precision;
                    yStep1 = (cornersInOrder1[i + 1].Y - cornersInOrder1[i].Y) / precision;

                    xStep2 = (cornersInOrder2[i + 1].X - cornersInOrder2[i].X) / precision;
                    yStep2 = (cornersInOrder2[i + 1].Y - cornersInOrder2[i].Y) / precision;
                }
                #endregion

                for (int j = 0; j < precision; j++)
                {
                    //Filling the arrays
                    edgesRect1[precision * i + j] = new Vector2(cornersInOrder1[i].X + xStep1 * j, cornersInOrder1[i].Y + yStep1 * j);
                    edgesRect2[precision * i + j] = new Vector2(cornersInOrder2[i].X + xStep2 * j, cornersInOrder2[i].Y + yStep2 * j);
                }
            }

            #endregion

            #region Comparing the 2 Arrays
            Rectangle[] LeftToRight = new Rectangle[2 * precision];
            Rectangle[] RightToLeft = new Rectangle[2 * precision];

            for (int i = 0; i < 2 * precision; i++)
            {
                LeftToRight[i] = new Rectangle((int)edgesRect1[i].X, (int)edgesRect1[i].Y, (int)radius * 2, (int)(edgesRect1[i + 1].Y - edgesRect1[i].Y + 1));
                RightToLeft[i] = new Rectangle((int)(edgesRect1[precision * 4 - (i + 1)].X - radius * 2), (int)edgesRect1[precision * 4 - (i + 1)].Y,
                    (int)radius * 2, (int)(edgesRect1[precision * 4 - (i + 2)].Y - edgesRect1[precision * 4 - (i + 1)].Y + 1));
            }

            bool isInFromLeft = false;
            bool isInFromRight = false;

            for (int i = 0; i < precision * 4; i++)
            {
                for (int j = 0; j < 2 * precision; j++)
                {
                    if (LeftToRight[j].Intersects(new Rectangle((int)edgesRect2[i].X, (int)edgesRect2[i].Y, 1, 1)))
                    {
                        isInFromLeft = true;
                    }

                    if (RightToLeft[j].Intersects(new Rectangle((int)edgesRect2[i].X, (int)edgesRect2[i].Y, 1, 1)))
                    {
                        isInFromRight = true;
                    }

                    if (isInFromLeft && isInFromRight)
                    {
                        collisionPositions.Add(new Vector2(edgesRect2[i].X, edgesRect2[i].Y));
                        return new Vector2(edgesRect2[i].X, edgesRect2[i].Y);
                    }
                }
            }

            

            #endregion

            //No side is returned, so return none.
            return Vector2.Zero;
        }

        public Side CheckOutOfBounds(Rectangle rectangle)
        {
            //Check for each corner if outside of
            //the driving area.
            for (int i = 0; i < 4; i++)
            {
                if (corners[i].X > rectangle.Right)
                {
                    return Side.Right;
                    //When a value is returned the code wills stop.
                    //This way the return false will only activate
                    //if no corner is outside of the bounds :)
                }
                if (corners[i].X < rectangle.Left)
                {
                    return Side.Left;
                    //When a value is returned the code wills stop.
                    //This way the return false will only activate
                    //if no corner is outside of the bounds :)
                }
                if (corners[i].Y > rectangle.Bottom)
                {
                    return Side.Bottom;
                    //When a value is returned the code wills stop.
                    //This way the return false will only activate
                    //if no corner is outside of the bounds :)
                }
                if (    corners[i].Y < rectangle.Top)
                {
                    return Side.Top;
                    //When a value is returned the code wills stop.
                    //This way the return false will only activate
                    //if no corner is outside of the bounds :)
                }
            }

            return Side.None;
        }
    }
}
