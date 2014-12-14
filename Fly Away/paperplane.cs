using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Fly_Away
{

    class paperplane : Sprite
    {    
        const string PLANE_ASSETNAME = "paperplane";
        private bool canFly = true;
        const int START_POSITION_X = 1000;
        const int START_POSITION_Y = 635;
        public int PLANE_SPEED = 600;
        const int MOVE_UP = -1;
        const int MOVE_DOWN = 1;
        const int MOVE_LEFT = -1;
        const int MOVE_RIGHT = 1;
        const string ENERGY_BAR_ASSET = "healthbar";
        private Texture2D mSpriteTexture;
        private int mCurrentEnergy = 100;
        private Texture2D mSpriteTextureOVER;
        Vector2 ePosition = Vector2.Zero;

        public enum State
        {
            Landed,
            Flying
        }
        public State mCurrentState = State.Landed;

        Vector2 mDirection = Vector2.Zero;
        Vector2 mSpeed = Vector2.Zero;
        Vector2 mStartingPosition = new Vector2(START_POSITION_X,START_POSITION_Y);

        MouseState mPreviousMouseState;

        //paperplane functions
        public void LoadContent(ContentManager theContentManager)
        {
            Position = new Vector2(START_POSITION_X, START_POSITION_Y);
            base.LoadContent(theContentManager,PLANE_ASSETNAME); //use LoadContent() from inheritted class
        }       

        public void Update(GameTime theGameTime) //check input 
        {

            MouseState aCurrentMouseState = Mouse.GetState();

            UpdateLanded();
            UpdateFlying(aCurrentMouseState);

            mPreviousMouseState = aCurrentMouseState;

            base.Update(theGameTime, mSpeed, mDirection);
        }

        private void UpdateFlying(MouseState aCurrentMouseState)
        {
            if (aCurrentMouseState.LeftButton == ButtonState.Pressed && canFly == true)
            {
                PLANE_SPEED = 600;
                mSpeed.Y = PLANE_SPEED;
                mCurrentEnergy -= 1;
                mDirection.Y = MOVE_UP;
                mCurrentState = State.Flying;
            }
            
            //left right movement in air
            //if (aCurrentMouseState.IsKeyDown(Keys.Left) == true && mCurrentState == State.Flying)
            //{
            //    PLANE_SPEED = 220;
            //    mSpeed.X = PLANE_SPEED;
            //    mDirection.X = MOVE_LEFT;
            //}

            //if (aCurrentMouseState.IsKeyDown(Keys.Right) == true && mCurrentState == State.Flying)
            //{
            //   PLANE_SPEED = 100;
            //    mSpeed.X = PLANE_SPEED;
            //    mDirection.X = MOVE_RIGHT;
            //}
            
            //when to apply gravity 
            if (mCurrentState == State.Flying)
            {
                if (mCurrentEnergy == 0)
                {
                    PLANE_SPEED = 250;
                    mSpeed.Y = PLANE_SPEED;
                    mDirection.Y = MOVE_DOWN;                    
                    canFly = false;
                }
                else if (aCurrentMouseState.LeftButton == ButtonState.Released)
                {
                    PLANE_SPEED = 250;
                    mSpeed.Y = PLANE_SPEED;
                    mDirection.Y = MOVE_DOWN;//temporary sub for gravity

                    if (mCurrentEnergy == 0)
                    {
                        canFly = false;
                    }
                }

                if (Position.Y > mStartingPosition.Y)
                {
                    Position.Y = mStartingPosition.Y;
                    mCurrentState = State.Landed;
                    mDirection = Vector2.Zero;
                }
            }
        }

        private void UpdateLanded()
        {
            if (mCurrentState == State.Landed)
            {
                mSpeed = Vector2.Zero;
                mDirection = Vector2.Zero;
                PLANE_SPEED = 0;

                if(mCurrentEnergy != 100)
                {
                    mCurrentEnergy += 20;
                    canFly = true;
                }
            }
        }

        //energy bar functions
        public void LoadEnergyBar(GraphicsDevice graphics, ContentManager theContentManager)
        {
            mSpriteTexture = theContentManager.Load<Texture2D>(ENERGY_BAR_ASSET);
            mSpriteTextureOVER = theContentManager.Load<Texture2D>("healthbarE");
            ePosition.X = graphics.Viewport.Width * 1 / 32;
            ePosition.Y = graphics.Viewport.Height * 1 / 32;
        }

        public void eDraw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(mSpriteTexture, ePosition,
                    new Rectangle(0, 0, mSpriteTexture.Width, mSpriteTexture.Height), Color.White,
                    0.0f, Vector2.Zero, Scale, SpriteEffects.None, 0);
        }

        public void eCurrentDraw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(mSpriteTextureOVER, new Vector2(ePosition.X, ePosition.Y + 20), new Rectangle((int)ePosition.X, (int)ePosition.Y,
                (int)(mSpriteTexture.Width * ((double)mCurrentEnergy / 100)), 20), Color.Orange);
        }

        public void eUpdate(GameTime theGameTime)
        {
            mCurrentEnergy = (int)MathHelper.Clamp(mCurrentEnergy, 0, 100);
        }
    }
}
