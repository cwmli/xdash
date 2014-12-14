using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Fly_Away
{
    class genSprite
    {
        #region genSprite properties
        public Vector2 playerPosition = new Vector2(0, 0);
        public Vector2 origin = Vector2.Zero;
        public float rotation = 0.0f;
        public float depth = 0.2f;
        private Texture2D mSpriteTexture;
        public Rectangle size;
        public static readonly List<Color[]> spriteTextureData = new List<Color[]>();
        public static readonly List<string> spriteTextureDataTag = new List<string>();
        public static readonly List<Texture2D> spriteTextureCache = new List<Texture2D>();
        public static readonly List<string> spriteTextureCacheTag = new List<string>();
        private Color[] rawTextureData;
        public static Color[] playerTextureData;
        private float mScale = 1.0f;
        public string AssetName;
        #endregion

        /// <summary>
        /// genSprite.cs contains: scale function (to scale size of sprites
        /// to viewport dimensions), rewritten Load and Draw function from 
        /// xna framework, and movement updater. 
        /// </summary>

        public float Scale
        {
            get { return mScale; }
            set
            {
                mScale = value;
                //recalc size of newsprite
                size = new Rectangle(0, 0, (int)(mSpriteTexture.Width * Scale), (int)(mSpriteTexture.Height * Scale)); //calc size of sprite
            }

        }

        public void LoadContent(ContentManager theContentManager, string theAssetName)
        {
            mSpriteTexture = theContentManager.Load<Texture2D>(theAssetName);
            AssetName = theAssetName;
            size = new Rectangle(0,0,(int)(mSpriteTexture.Width * Scale), (int)(mSpriteTexture.Height * Scale)); //calc size of sprite        
            
            getCollisionData();
        }

        /// <summary>
        /// Loads textures and stores into a list (cached) to be accessed and used later.
        /// Function used exclusively for car spawning
        /// </summary>
        /// <param name="theContentManager">ContentManager</param>
        /// <param name="theAssetName">AssetName</param>
        public void LoadCache(ContentManager theContentManager, string theAssetName)
        {
            mSpriteTexture = theContentManager.Load<Texture2D>(theAssetName);
            AssetName = theAssetName;
            size = new Rectangle(0, 0, (int)(mSpriteTexture.Width * Scale), (int)(mSpriteTexture.Height * Scale));

            //extract collision data
            getCollisionData();
            //add loaded texture into texture2D list for use later
            spriteTextureCache.Add(mSpriteTexture);
            //track texture2D with tag
            spriteTextureCacheTag.Add(AssetName);
        }

        public void getCollisionData()
        {
            //extract collision data
            rawTextureData = new Color[mSpriteTexture.Width * mSpriteTexture.Height];
            mSpriteTexture.GetData<Color>(rawTextureData);
            //and store into spriteTextureData(color[]) list
            spriteTextureData.Add(rawTextureData);
            //track which Texture2Ds are for which sprite...correlates with spriteTextureData color list
            spriteTextureDataTag.Add(AssetName);
        }

        public static void sortTextureDataList()
        {
            int trackerDataTag = 0;
            foreach(string TextureDataTag in spriteTextureDataTag)
            {
                //sorts car related stuff into car list
                if(TextureDataTag.Contains("car_") || 
                    TextureDataTag == "truck" || TextureDataTag == "van")
                {
                    //carTextureData.Add(spriteTextureData[trackerDataTag]);
                }

                else if(TextureDataTag == "paperplane")
                {
                    playerTextureData = spriteTextureData[trackerDataTag];
                }
                trackerDataTag++;
            }
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(mSpriteTexture, playerPosition, 
                new Rectangle(0, 0, mSpriteTexture.Width, mSpriteTexture.Height), Color.White,
                rotation, origin, Scale, SpriteEffects.None, depth);
        }

        //Update for use with playerPlane
        public void Update(GameTime theGameTime, Vector2 theSpeed, Vector2 theDirection)
        {
            playerPosition += theDirection * theSpeed * (float)theGameTime.ElapsedGameTime.TotalSeconds;
        }

    }
}
