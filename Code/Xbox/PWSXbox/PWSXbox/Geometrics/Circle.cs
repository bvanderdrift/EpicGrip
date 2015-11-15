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
    class Circle
    {
        //The radius of the circle
        float radius;

        //The position of the circle
        Vector2 position;

        //Dot for debugging
        static Sprite dot;
        Vector2[] edge;

        //Property of the radius
        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        //Property of the position
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        //Set the radius with instantiating the circle
        public Circle(float radius, Vector2 position)
        {
            this.radius = radius;
            this.position = position;
        }

        public void Initialize()
        {

        }

        static public void LoadContentStatic(ContentManager content)
        {
            dot = new Sprite();
            dot.Initialize(Vector2.Zero);
            dot.Origin = new Vector2(4);

            dot.LoadContent(content.Load<Texture2D>("Graphics/Debug Stuff/sDot"));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //if (edge != null)
            //{
            //    for (int i = 0; i < 40; i++)
            //    {
            //        dot.Position = edge[i];
            //        dot.Draw(spriteBatch);
            //    }
            //}
        }

        public Vector2 Intersects(Circle otherCircle)
        {

            return Vector2.Zero;
        }

        public Vector2 Intersects(RotatableRectangle otherRectangle, int precision)
        {
            Vector2[] edgesRect = new Vector2[precision * 4];
            Vector2[] edgesCircl = new Vector2[precision * 4];

            //Real Test

            #region Filling the cornersInOrder for both rectangles OLD METHOD


            //#region Filling the empty cornersInOrder with comparable value
            ////Filling the 'null' variable with something sure to be invalid
            ////'0' Needs to be the highest, so as filler I chose the lowest value possible (720p)
            //cornersInOrder1[0] = new Vector2(1280, 720);
            //cornersInOrder2[0] = new Vector2(1280, 720);
            ////'1' Needs to be the most to the left, so as filler I chose the most right value possible (720p)
            //cornersInOrder1[1] = new Vector2(1280, 720);
            //cornersInOrder2[1] = new Vector2(1280, 720);
            ////'2' Needs to be the lowest, so as filler I chose the highest value possible (720p)
            //cornersInOrder1[2] = new Vector2(0);
            //cornersInOrder2[2] = new Vector2(0);
            ////'3' Needs to be the most to the right, so as filler I chose the most left value possible (720p)
            //cornersInOrder1[3] = new Vector2(0);
            //cornersInOrder2[3] = new Vector2(0);
            //#endregion

            //#region Retrieving Nr1
            ////Checking every vector if it is higher, if so, fill nr1 up, we got us nr1!! :D
            //for (int i = 0; i < 4; i++)
            //{
            //    if (cornersInOrder1[0].Y > corners[i].Y)
            //    {
            //        cornersInOrder1[0] = corners[i];
            //    }
            //    if (cornersInOrder2[0].Y > rectangle.corners[i].Y)
            //    {
            //        cornersInOrder2[0] = rectangle.corners[i];
            //    }
            //}

            ////In case there are 2 on te same Y, we want the left one:
            //for (int i = 0; i < 4; i++)
            //{
            //    if (cornersInOrder1[0].X < corners[i].X && cornersInOrder1[0].Y == corners[i].Y)
            //    {
            //        cornersInOrder1[0] = corners[i];
            //    }
            //    if (cornersInOrder2[0].X < rectangle.corners[i].X && cornersInOrder2[0].Y == rectangle.corners[i].Y)
            //    {
            //        cornersInOrder2[0] = rectangle.corners[i];
            //    }
            //}
            //#endregion

            //#region Retrieving Nr2
            ////Nr2 is more difficult, we are sure it is the most to the left
            //for (int i = 0; i < 4; i++)
            //{
            //    if (cornersInOrder1[1].X >= corners[i].X)
            //    {
            //        cornersInOrder1[1] = corners[i];
            //    }
            //    if (cornersInOrder2[1].X >= rectangle.corners[i].X)
            //    {
            //        cornersInOrder2[1] = rectangle.corners[i];
            //    }
            //}

            ////In case of 2 of them, I want the lowest one :D
            //for (int i = 0; i < 4; i++)
            //{
            //    if (cornersInOrder1[1].X == corners[i].X && cornersInOrder1[1].Y < corners[i].Y)
            //    {
            //        cornersInOrder1[1] = corners[i];
            //    }
            //    if (cornersInOrder2[1].X == rectangle.corners[i].X && cornersInOrder2[1].Y < rectangle.corners[i].Y)
            //    {
            //        cornersInOrder2[1] = rectangle.corners[i];
            //    }
            //}
            //#endregion

            //#region Retrieving Nr3
            ////Now Nr3, I want the lowest one
            //for (int i = 0; i < 4; i++)
            //{
            //    if (cornersInOrder1[2].Y <= corners[i].Y)
            //    {
            //        cornersInOrder1[2] = corners[i];
            //    }
            //    if (cornersInOrder2[2].Y <= rectangle.corners[i].Y)
            //    {
            //        cornersInOrder2[2] = rectangle.corners[i];
            //    }
            //}

            ////In case of 2 of them, again I want the most to the right
            //for (int i = 0; i < 4; i++)
            //{
            //    if (cornersInOrder1[2].X < corners[i].X && cornersInOrder1[2].Y == corners[i].Y)
            //    {
            //        cornersInOrder1[2] = corners[i];
            //    }
            //    if (cornersInOrder2[2].X < rectangle.corners[i].X && cornersInOrder2[2].Y == rectangle.corners[i].Y)
            //    {
            //        cornersInOrder2[2] = rectangle.corners[i];
            //    }
            //}

            //#endregion

            //#region Retrieving Nr4
            ////Last one!, most right one :D
            //for (int i = 0; i < 4; i++)
            //{
            //    if (cornersInOrder1[3].X <= corners[i].X)
            //    {
            //        cornersInOrder1[3] = corners[i];
            //    }
            //    if (cornersInOrder2[3].X <= rectangle.corners[i].X)
            //    {
            //        cornersInOrder2[3] = rectangle.corners[i];
            //    }
            //}

            ////in case of 2, we want the bottomright blablabla
            //for (int i = 0; i < 4; i++)
            //{
            //    if (cornersInOrder1[3].X == corners[i].X && cornersInOrder1[3].Y > corners[i].Y)
            //    {
            //        cornersInOrder1[3] = corners[i];
            //    }
            //    if (cornersInOrder2[3].X == rectangle.corners[i].X && cornersInOrder2[3].Y > rectangle.corners[i].Y)
            //    {
            //        cornersInOrder2[3] = rectangle.corners[i];
            //    }
            //}
            //#endregion
            #endregion

            #region Filling the cornersInOrder NEW METHOD
            #region Set up the corners in the correct order from top counter-clockwise
            //Set up the arrays for the corner in order
            Vector2[] cornersInOrder = new Vector2[4];

            //Create variables to keep track of what is the lowest point yet.
            //I set them to a value sure to be more right or lower, and a buffer of 10 pixels
            float highestYValue = position.Y + radius + 10;
            float mostLefttXValue = position.X + radius + 10;
            int nrOneCorner = 0;

            #region fill corners in order of the other rectangle
            highestYValue = otherRectangle.Position.Y + otherRectangle.Radius + 10;
            mostLefttXValue = otherRectangle.Position.X + otherRectangle.Radius + 10;
            nrOneCorner = 0;

            //Check every corner
            for (int i = 0; i < 4; i++)
            {
                //Is the corner equal to a previously measured Y value? (The dots are on the same height)
                //Then check if this corner is more left then the other
                if (otherRectangle.Corners[i].Y == highestYValue &&
                    otherRectangle.Corners[i].X < mostLefttXValue)
                {
                    highestYValue = otherRectangle.Corners[i].Y;
                    mostLefttXValue = otherRectangle.Corners[i].X;
                    nrOneCorner = i;
                }

                //If the y value of the current corner is lower than the last lowest y value
                //This corner is higher. so set this as the nr1 corner
                if (otherRectangle.Corners[i].Y < highestYValue)
                {
                    highestYValue = otherRectangle.Corners[i].Y;
                    mostLefttXValue = otherRectangle.Corners[i].X;
                    nrOneCorner = i;
                }
            }

            //Now fill in the corners in order
            int currentCorner = nrOneCorner;
            for (int i = 0; i < 4; i++)
            {
                cornersInOrder[i] = otherRectangle.Corners[currentCorner];

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

            //For the rectangle
            for (int i = 0; i < 4; i++)
            {
                #region Gaining the steps
                float xStep, yStep;

                //Getting the steps
                if (i + 1 == 4)
                {
                    xStep = (cornersInOrder[0].X - cornersInOrder[i].X) / precision;
                    yStep = (cornersInOrder[0].Y - cornersInOrder[i].Y) / precision;
                }
                else
                {
                    xStep = (cornersInOrder[i + 1].X - cornersInOrder[i].X) / precision;
                    yStep = (cornersInOrder[i + 1].Y - cornersInOrder[i].Y) / precision;
                }
                #endregion

                for (int j = 0; j < precision; j++)
                {
                    //Filling the arrays
                    edgesRect[precision * i + j] = new Vector2(cornersInOrder[i].X + xStep * j, cornersInOrder[i].Y + yStep * j);
                }
            }

            //For the circle
            //Get the phase between each dot
            float phase = (float)(2 * Math.PI) / (precision * 4);

            //Get the position on a unit circle, multply it with the radius, and add the centre of the circle
            for (int i = 0; i < precision * 4; i++)
            {
                edgesCircl[i] = new Vector2
                    (
                        (float)(radius * Math.Cos(phase * i)),
                        (float)(radius * Math.Sin(phase * i))
                    ) + position;
            }

            #endregion

            #region Comparing the 2 Arrays
            Rectangle[] LeftToRight = new Rectangle[2 * precision];
            Rectangle[] RightToLeft = new Rectangle[2 * precision];

            for (int i = 0; i < 2 * precision; i++)
            {
                LeftToRight[i] = new Rectangle((int)edgesRect[i].X, (int)edgesRect[i].Y, (int)radius * 2, (int)(edgesRect[i + 1].Y - edgesRect[i].Y));
                RightToLeft[i] = new Rectangle((int)(edgesRect[precision * 4 - (i + 1)].X - radius * 2), (int)edgesRect[precision * 4 - (i + 1)].Y,
                    (int)radius * 2, (int)(edgesRect[precision * 4 - (i + 2)].Y - edgesRect[precision * 4 - (i + 1)].Y));
            }

            //Debug shizzle
            edge = edgesCircl;

            bool isInFromLeft = false;
            bool isInFromRight = false;

            for (int i = 0; i < precision * 4; i++)
            {
                for (int j = 0; j < 2 * precision; j++)
                {
                    if (LeftToRight[j].Intersects(new Rectangle((int)edgesCircl[i].X, (int)edgesCircl[i].Y, 4, 4)))
                    {
                        isInFromLeft = true;
                    }

                    if (RightToLeft[j].Intersects(new Rectangle((int)edgesCircl[i].X, (int)edgesCircl[i].Y, 4, 4)))
                    {
                        isInFromRight = true;
                    }

                    if (isInFromLeft && isInFromRight)
                    {
                        return new Vector2(edgesCircl[i].X, edgesCircl[i].Y);
                    }
                }
            }
            #endregion

            return Vector2.Zero;
        }
    }
}
