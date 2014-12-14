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
    class carLogic : genSprite
    {
        #region Car properties
        private float rotationAngle;
        private int textureindex;
        public int pub_TextureIndex { get { return textureindex; } }
        private int wheelselect;
        private Vector2 carscreenpos;
        private bool carTexturesPieces;
        private int carindex;
        private int wheelindex;
        public Vector2 carPos {get {return carscreenpos;}} //allow for Game1.cs to get car position
        public int yOffset { get { return spriteTextureCache[textureindex].Height; } }

        List<string> cartype = new List<string>();
        List<string> wheeltype = new List<string>();
        #endregion

        //Add asset names to a cartype list for loading in Load()
        private void CarVariant()
        {
            cartype.Add("Car/car_variant_1_bluecolor");//0
            cartype.Add("Car/car_variant_1_greencolor");//1
            cartype.Add("Car/car_variant_1_purplecolor");//2
            cartype.Add("Car/car_variant_2_orangecolor");//3
            cartype.Add("Car/car_variant_2_redcolor");//4
            cartype.Add("Car/truck");//5
            cartype.Add("Car/van");//6
        }

        //Add asset names to a wheeltype list for loading in Load()
        private void WheelVariant()
        {
            wheeltype.Add("Car/carwheel");
            wheeltype.Add("Car/bigwheel");
        }

        //check the car size of the wheels to determine whether or not to use large or small wheels
        private void getWheelType()
        {
            if (spriteTextureCache[textureindex].Width > 255) 
            {
                int trackerCacheTag = 0;
                foreach (string CacheTag in spriteTextureCacheTag)
                {
                    if(CacheTag == "Car/bigwheel")
                    {
                        wheelselect = trackerCacheTag;
                    }
                    trackerCacheTag++;
                }
            }

            else
            {
                //search for the wheel texture in spriteTextureCache list
                int trackerCacheTag = 0;
                foreach (string CacheTag in spriteTextureCacheTag)
                {
                    if (CacheTag == "Car/carwheel")
                    {
                        wheelselect = trackerCacheTag;
                    }
                    trackerCacheTag++;
                }
            }
        }


        private void Load(ContentManager theContentManager)
        {
            //add the assetnames to the predefined lists for loading
            CarVariant();
            WheelVariant();
            //cycle through the list to load all necessary textures
            for (carindex = 0; carindex <= cartype.Count - 1; carindex += 1)
            {
                base.LoadCache(theContentManager,(string)cartype[carindex]);
            }
            for (wheelindex = 0; wheelindex <= wheeltype.Count - 1; wheelindex += 1)
            {
                base.LoadCache(theContentManager,(string)wheeltype[wheelindex]);
            }
        }

        //allows Game.cs to call this function while creating a car in createCar()
        public static class Shared
        {
            //the random value generated is used for randomized car textures/types
            public static readonly Random Random = new Random();
        }

        /// <summary>
        /// Creates a new car and gets the wheel type for the car ONLY IF textures are loaded
        /// </summary>
        /// <param name="position">Any Vector2 inputted into this argument will be the position of the car</param>
        /// <param name="spriteTextureCacheindex">spriteTextureCacheindex takes an int to choose a car texture</param>
        public carLogic(Vector2 position, int spriteTextureCacheindex, ContentManager theContentManager)
        {
            if (carTexturesPieces!=true)
            {
                Load(theContentManager);
                carTexturesPieces = true;
            }
            this.carscreenpos = position;
            this.textureindex = spriteTextureCacheindex;
        }

        /// <summary>
        /// Updates the movement of the cars
        /// </summary>
        /// <param name="deltaX">A value that controls the speed of which the car moves at</param>
        public void UpdateCar(float deltaX) //get scroll to move back after passing screen
        {
            carscreenpos.X -= deltaX;
        }

        //Shows wheel rotation effect
        public void UpdateWheels(float elapsed)
        {
            getWheelType();
            rotationAngle -= elapsed;
            float circle = MathHelper.Pi * 2;
            rotationAngle = rotationAngle % circle;
        }

        //Draw the car
        new public void Draw(SpriteBatch batch)
        {
            batch.Draw((Texture2D)spriteTextureCache[textureindex],carscreenpos, null, Color.White, 0, new Vector2 (0,spriteTextureCache[textureindex].Height),1,SpriteEffects.None,0.2f);
            //------------------------------------------------------------DRAW TEH WHEELZ---------------------------------------------------//
            //fr wheel (do rotations) - wheel pos relative to car position
            batch.Draw((Texture2D)spriteTextureCache[wheelselect], new Vector2(carscreenpos.X + spriteTextureCache[textureindex].Width * 1 / 5, carscreenpos.Y - spriteTextureCache[textureindex].Width * 1/10), 
                null, Color.White, rotationAngle, new Vector2(spriteTextureCache[wheelselect].Width / 2, spriteTextureCache[wheelselect].Height / 2), 1, SpriteEffects.None, 0.1f);
            //bk wheel (do rotations)
            batch.Draw((Texture2D)spriteTextureCache[wheelselect], new Vector2(carscreenpos.X + spriteTextureCache[textureindex].Width * 4/5, carscreenpos.Y - spriteTextureCache[textureindex].Width * 1/10),
                null, Color.White, rotationAngle, new Vector2(spriteTextureCache[wheelselect].Width / 2, spriteTextureCache[wheelselect].Height / 2), 1, SpriteEffects.None, 0.1f);
        }
    }
}
