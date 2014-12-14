using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Fly_Away
{
    class backgroundParallax
    {
        public static readonly List<Texture2D> parallaxTexture = new List<Texture2D>();
        private Vector2 screenPosition = new Vector2(0, 0);
        private float parallaxTextureDepth;
        private int selected;

        public void Load(ContentManager theContentManager)
        {
            if (parallaxTexture.Count == 0)
            {
                parallaxTexture.Add(theContentManager.Load<Texture2D>("Scene/background_layer1"));
                parallaxTexture.Add(theContentManager.Load<Texture2D>("Scene/background_layer2"));
                parallaxTexture.Add(theContentManager.Load<Texture2D>("Scene/background_layer3"));
                parallaxTexture.Add(theContentManager.Load<Texture2D>("Scene/background_layer4"));
            }

            setDepths(this.selected);
        }

        public void setDepths(float parallaxTexture)
        {
            if (parallaxTexture == 0)
            {
                this.parallaxTextureDepth = 0.5f;
            }
            else if (parallaxTexture == 1)
            {
                this.parallaxTextureDepth = 0.7f;
            }
            else if (parallaxTexture == 2)
            {
                this.parallaxTextureDepth = 0.9f;
            }
            else
            {
                this.parallaxTextureDepth = 0.8f;
                this.screenPosition = new Vector2(0, 20);
            }
        }

        public backgroundParallax (int selector)
        {
            this.selected = selector;
        }

        public void updateParallax(float parallaxSpeed)
        {
            screenPosition.X += parallaxSpeed / parallaxTextureDepth;
            screenPosition.X = screenPosition.X % 1280;
        }

        public void parallaxDraw(SpriteBatch spritebatch)
        {
            if (screenPosition.X < 1280)
            {
                spritebatch.Draw((Texture2D)parallaxTexture[selected], screenPosition, null,
                    Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, parallaxTextureDepth);
            }
            spritebatch.Draw((Texture2D)parallaxTexture[selected], screenPosition - new Vector2(1280,0), null,
                    Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, parallaxTextureDepth);

        }
    }  
}
