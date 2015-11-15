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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace PWS.Graphics
{
    public class Sprite
    {
        #region Fields and Properties

        protected Texture2D texture;
        protected Vector2 position;
        protected Rectangle source;
        protected Color color;
        protected float rotation;
        protected Vector2 origin;
        protected Vector2 scale;
        protected SpriteEffects effect;
        protected float layerDepth;
        protected Rectangle bounds;
        protected bool visible;
        const float radial = 360 / (2 * (float)Math.PI);
        Color[] pixelsArray;

        #region Properties

        public Color[] PixelsData
        {
            get { return pixelsArray; }
            set { pixelsArray = value; }
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        /// <summary>
        /// Vector 2 Only, Properties are unavailable!
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public float PosX
        {
            get { return position.X; }
            set { position.X = value; }
        }

        public float PosY
        {
            get { return position.Y; }
            set { position.Y = value; }
        }

        public Rectangle Source
        {
            get { return source; }
            set { source = value; }
        }

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        /// <summary>
        /// IN RADIALS! Not degrees.
        /// </summary>
        public float Rotation
        {
            get { return (rotation); }
            set { rotation = value; }
        }

        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        public Vector2 Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public SpriteEffects Effect
        {
            get { return effect; }
            set { effect = value; }
        }

        public float LayerDepth
        {
            get { return layerDepth; }
            set { layerDepth = value; }
        }

        public Rectangle Bounds
        {
            get { return bounds; }
        }

        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        #endregion

        #endregion

        public virtual void LoadContent(Texture2D texture)
        {
            this.texture = texture;
            source = new Rectangle(0,0 , texture.Width, texture.Height);
            bounds = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            pixelsArray = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(pixelsArray);
        }

        public virtual void Initialize(Vector2 position)
        {
            this.position = position;
            bounds.X = (int)position.X;
            bounds.Y = (int)position.Y;
            color = Color.White;
            rotation = 0;

            if (origin == null)
            {
                origin = new Vector2(0, 0);
            }

            scale = Vector2.One;
            effect = SpriteEffects.None;
            layerDepth = 1;
            visible = true;

        }

        public virtual void Update()
        {
            bounds.Height = source.Height;
            bounds.Width = source.Width;
            bounds.X = (int)position.X - (int)origin.X;
            bounds.Y = (int)position.Y - (int)origin.Y;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                spriteBatch.Draw(texture, position, source, color, rotation, origin, scale, effect, layerDepth);
            }
        }

        public void SetOriginToMiddle()
        {
            origin = new Vector2(texture.Width, texture.Height) / 2;
        }

        /// <summary>
        /// Pixel Per Pixel Intersection test, tests if not the bounds intersects, but truly per pixel. WATCH OUT: DOESN'T WORK WITH ROTATED SPRITES, use a movable of circulair sprite for that
        /// </summary>
        /// <param name="OtherSprite">The Other Sprite</param>
        /// <returns></returns>
        //public bool PPPIntersects(Sprite OtherSprite)
        //{
        //    Rectangle thingA = bounds;
        //    Rectangle thingB = OtherSprite.bounds;
        //    Color[] dataA = pixelsArray;
        //    Color[] dataB = OtherSprite.pixelsArray;

        //    // Find the bounds of the rectangle intersection
        //    int top = Math.Max(thingA.Top, thingB.Top);
        //    int bottom = Math.Min(thingA.Bottom, thingB.Bottom);
        //    int left = Math.Max(thingA.Left, thingB.Left);
        //    int right = Math.Min(thingA.Right, thingB.Right);

        //    // Check every point within the intersection bounds
        //    for (int y = top; y < bottom; y++)
        //    {
        //        for (int x = left; x < right; x++)
        //        {
        //            // Get the color of both pixels at this point
        //            Color colorA = dataA[(x - thingA.Left) +
        //                                 (y - thingA.Top) * thingA.Width];
        //            Color colorB = dataB[(x - thingB.Left) +
        //                                 (y - thingB.Top) * thingB.Width];

        //            // If both pixels are not completely transparent,
        //            if (colorA.A != 0 && colorB.A != 0)
        //            {
        //                // then an intersection has been found
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        //public bool Intersects(Sprite OtherSprite, int Pricision)
        //{
        //    return bounds.Intersects(OtherSprite.bounds);
        //}

        //public bool Intersects(MoveableSprite OtherSprite, int Pricision)
        //{
        //    return OtherSprite.Bounds.Intersects(bounds, Pricision);
        //}

        //public bool Intersects(CirculairSprite OtherSprite, int Pricision)
        //{
        //    return OtherSprite.Bounds.Intersects(bounds, Pricision);
        //}
    }
}
