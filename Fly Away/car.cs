using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Fly_Away
{
    class car
    {
        private Texture2D loadCarTexture;
        private Texture2D loadWheelTexture;
        private float rotationAngle;
        private int textureindex;
        private int wheelselect;
        private Vector2 carscreenpos;
        private Vector2 origin;
        private int carindex;
        private int wheelindex;
        public Vector2 Position {get {return carscreenpos;}} //allow for Game1.cs to get car position

        List<string> cartype = new List<string>();
        List<string> wheeltype = new List<string>();

        public static readonly List<Texture2D> bkTexture = new List<Texture2D>();
        public static readonly List<Texture2D> orgTexture = new List<Texture2D>();

        public static readonly List<Texture2D> wlTexture = new List<Texture2D>();

        private void CarVariant()
        {
            cartype.Add("car_variant_1_bluecolor");//0
            cartype.Add("car_variant_1_greencolor");//1
            cartype.Add("car_variant_1_purplecolor");//2
            cartype.Add("car_variant_2_orangecolor");//3
            cartype.Add("car_variant_2_redcolor");//4
            cartype.Add("truck");//5
            cartype.Add("van");//6
        }

        private void WheelVariant()
        {
            wheeltype.Add("carwheel");
            wheeltype.Add("bigwheel");
        }

        private void getWheelType()
        {
            if (bkTexture[textureindex].Width > 255) //greater than width of smaller car then use large wheels
            {
                wheelselect = 1;
            }

            else
            {
                wheelselect = 0;
            }
        }


        public void Load(ContentManager theContentManager)
        {
            CarVariant();
            WheelVariant();
            for (carindex = 0; carindex <= cartype.Count - 1; carindex += 1)
            {
                //loads ALL textures for each string/file defined in arraylist "cartype" and add to bkTexture arraylist 

                loadCarTexture = theContentManager.Load<Texture2D>((string)cartype[carindex]);
                this.origin = new Vector2(0, loadCarTexture.Height);
                bkTexture.Add(loadCarTexture);
            }
            for (wheelindex = 0; wheelindex <= wheeltype.Count - 1; wheelindex += 1)
            {
                loadWheelTexture = theContentManager.Load<Texture2D>((string)wheeltype[wheelindex]);
                wlTexture.Add(loadWheelTexture);
            }
        }

        public static class Shared
        {
            public static readonly Random Random = new Random();
        }

        public car(Vector2 position, int bktextureindex)
        {
            this.carscreenpos = position;
            this.textureindex = bktextureindex;
            if (bkTexture.Count > 0)
            {
                getWheelType();
            }
        }

        //control speed
        public void UpdateCar(float deltaX) //get scroll to move back after passing screen
        {
            carscreenpos.X -= deltaX;
        }

        public void UpdateWheels(float elapsed)
        {
            rotationAngle -= elapsed;
            float circle = MathHelper.Pi * 2;
            rotationAngle = rotationAngle % circle;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw((Texture2D)bkTexture[textureindex],carscreenpos, null, Color.White, 0, new Vector2 (0,bkTexture[textureindex].Height),1,SpriteEffects.None,0);
            //fr wheel (do rotations) - wheel pos relative to car position
            batch.Draw((Texture2D)wlTexture[wheelselect], new Vector2(carscreenpos.X + bkTexture[textureindex].Width * 1 / 5, carscreenpos.Y - bkTexture[textureindex].Width * 1/10), 
                null, Color.White, rotationAngle, new Vector2(wlTexture[wheelselect].Width / 2, wlTexture[wheelselect].Height / 2), 1, SpriteEffects.None, 0.1f);
            //bk wheel (do rotations)
            batch.Draw((Texture2D)wlTexture[wheelselect], new Vector2(carscreenpos.X + bkTexture[textureindex].Width * 4/5, carscreenpos.Y - bkTexture[textureindex].Width * 1/10),
                null, Color.White, rotationAngle, new Vector2(wlTexture[wheelselect].Width / 2, wlTexture[wheelselect].Height / 2), 1, SpriteEffects.None, 0.1f);
        }
    }
}
