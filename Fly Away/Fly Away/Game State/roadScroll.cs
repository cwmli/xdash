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
    class roadScroll
    {
        private Vector2 screenpos;
        private int screenwidth, screenheight, sIndex, tempSize;
        private float scrollTextureDepth = 0.9f;
        private Texture2D tempTexture;
        private float xMov;
        public static readonly List<Texture2D> scrollTexture = new List<Texture2D>();
        public static readonly List<int> sizeTexture = new List<int>();
        List<string> scrollType = new List<string>();

        private void LoadList()
        {
            scrollType.Add("Scene/backgrass");
            scrollType.Add("Scene/road");
            scrollType.Add("Scene/frontgrass");
        }

        private void setScreenPos(int tindex)
        {
            if (tindex == 0)//backgrass
            {
                this.screenpos = new Vector2(0, screenheight - sizeTexture[1]);
                this.scrollTextureDepth = 0.3f;
             }
            else if (tindex == 1)//road
            {
                this.screenpos = new Vector2(0, screenheight);
                this.scrollTextureDepth = 0.2f;
            }
            else if (tindex == 2)//frontgrass
            {
                this.screenpos = new Vector2(0, screenheight);
                this.scrollTextureDepth = 0f;
            }
        }

        public void Load(ContentManager theContentManager)
        {
            if (scrollType.Count == 0)
            {
                LoadList();
            }

            if (scrollTexture.Count == 0)
            {
                for (int i = 0; i <= scrollType.Count-1; i++)
                {
                    tempTexture = theContentManager.Load<Texture2D>(scrollType[i]);
                    scrollTexture.Add(tempTexture);
                    tempSize = (int)(theContentManager.Load<Texture2D>(scrollType[i]).Height);
                    sizeTexture.Add(tempSize);
                }
            }

            setScreenPos(sIndex);
       }


        public roadScroll (GraphicsDevice graphics, int i)
        {
            screenwidth = graphics.Viewport.Width;
            screenheight = graphics.Viewport.Height;
            this.sIndex = i; //texture selection (backgrass, frontgrass, road) in respective order

        }

        public void Update(float deltaX)
        {
            if (scrollTextureDepth != 0)
            {
                xMov = deltaX * scrollTextureDepth;
            }
            else
            {
                xMov = deltaX;
            }

            screenpos.X += xMov;
            screenpos.X = screenpos.X % 1280;
           
        }

        public void Draw(SpriteBatch batch)
        {

            if (screenpos.X < screenwidth) //check to see if texture is still onscreen
            {
                batch.Draw(scrollTexture[sIndex], screenpos, null,
                    Color.White, 0, new Vector2(0, sizeTexture[sIndex]), 1, SpriteEffects.None, scrollTextureDepth);
            }

            batch.Draw((Texture2D)scrollTexture[sIndex], screenpos - new Vector2(1280, 0), null,
                Color.White, 0, new Vector2(0, sizeTexture[sIndex]), 1, SpriteEffects.None, scrollTextureDepth); 
            //Console.WriteLine(sizeTexture[sIndex] + " " + screenpos +" " + scrollTextureDepth);
        }
    }
}
