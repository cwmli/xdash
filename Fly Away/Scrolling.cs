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
    class Scrolling
    {
        private Vector2 screenpos;
        private int screenwidth, screenheight, sIndex, tempSize;
        private float scrollTextureDepth = 0;
        private Texture2D tempTexture;
        public static readonly List<Texture2D> scrollTexture = new List<Texture2D>();
        public static readonly List<int> sizeTexture = new List<int>();
        List<string> scrollType = new List<string>();

        private void LoadList()
        {
            scrollType.Add("backgrass");
            scrollType.Add("road");
            scrollType.Add("frontgrass");
        }

        private void getScreenPos(int tindex)
        {
            if (tindex == 0)//backgrass
            {
                this.screenpos = new Vector2(0, screenheight - sizeTexture[1]);
                this.scrollTextureDepth = 0.9f;
             }
            else if (tindex == 1)//road
            {
                this.screenpos = new Vector2(0, screenheight);
                this.scrollTextureDepth = 0.9f;
            }
            else //frontgrass
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
                for (int i = 0; i <= scrollType.Count - 1; i++)
                {
                    tempTexture = theContentManager.Load<Texture2D>(scrollType[i]);
                    scrollTexture.Add(tempTexture);
                    tempSize = (int)(theContentManager.Load<Texture2D>(scrollType[i]).Height);
                    sizeTexture.Add(tempSize);
                    Console.WriteLine("Current sizeTexture count: " + sizeTexture.Count +" sizeTexture value: " + sizeTexture[i]);
                }
            }

            getScreenPos(sIndex);
       }


        public Scrolling (GraphicsDevice graphics, int i)
        {
            screenwidth = graphics.Viewport.Width;
            screenheight = graphics.Viewport.Height;
            this.sIndex = i; //texture selection (backgrass, frontgrass, road) in respective order

        }

        public void Update(float deltaX)
        {
           screenpos.X += deltaX;
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
                Color.White, 0, new Vector2(0, sizeTexture[sIndex]), 1, SpriteEffects.None, scrollTextureDepth); //positions not setting correctly
            Console.WriteLine(sizeTexture[sIndex] + " " + screenpos +" " + scrollTextureDepth);
        }
    }
}
