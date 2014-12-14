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

    class spritePlayer : genSprite
    {
        #region playerPlane variables
        const string assetnamePlayerStand = "Player/player";
        const string assetnamePlayerRun = "Player/playerrunSS";
        const string assetnamePlayerJump = "Player/playerjumpSS";
        const string assetnamePlayerFall = "Player/playerjumpfallSS";
        const string assetnameJumpEffect = "Player/jumpeffect";
        const string assetnamePlayerDash = "Player/elbowdash";

        private bool jumpAvailable = true;
        private int maxJumps = 2;
        private int jumpsUsed = 0;
        private float playerStartJumpY;
        private float playerStartX;
        private float jumpLimit = 100f;

        private bool dashAvailable = true;
        private int dashUsed = 0;

        const float frameDelay = 0.1f;
        private int currentFrame = 0;
        private int maxFrames = 5;
        public int playerRunTexture,playerStandTexture, playerJumpTexture, playerFallTexture, playerJumpEffect, playerDashTexture;
        private bool addFrames = true;
        public bool runningRight, jumpingRight, fallingRight, dashingRight, jumpLimiter;

        const int START_POSITION_X = 200;
        const int START_POSITION_Y = 635;

        public int airSpeed = 250;
        private int uphillSpeed = 200;
        private int runSpeed = 0; //default 250
        const int MOVE_UP = -1;
        const int MOVE_DOWN = 1;
        const float MOVE_LEFT = -0.9f;
        const float MOVE_RIGHT = 0.9f;
        Vector2 playerDirection = Vector2.Zero;
        Vector2 playerVelocity = Vector2.Zero;
        Vector2 playerAccelerationRate = new Vector2(0.5f, 0.75f);
        Vector2 playerStartingPosition = new Vector2(START_POSITION_X,START_POSITION_Y);

        private int playerCurrentEnergy = 100;
        public State playerCurrentState = State.Landed;

        public Nullable<bool> playerCollideBoxRight = false;
        public Nullable<bool> playerUnclimable = false;
        public bool playercarCollision = false;
        public bool playercarCollisionPrevious;
        
        public Vector2 playerPos { get { return playerPosition; } }
        public int playerWidth { get { return base.size.Width; } }
        public int playerHeight { get { return base.size.Height; } }
        public float animationDelay { get { return frameDelay; } }

        MouseState mousestatePrevious;
        #endregion

        public enum State
        {
            Landed,
            Flying,
            Falling,
            Dashing
        }

        public void LoadContent(ContentManager theContentManager)
        {
            playerPosition = new Vector2(START_POSITION_X, START_POSITION_Y);
            //Load standing sprite
            base.LoadCache(theContentManager,assetnamePlayerStand); 
            //Load moving spritesheets
            base.LoadCache(theContentManager, assetnamePlayerRun);
            base.LoadCache(theContentManager, assetnamePlayerJump);
            base.LoadCache(theContentManager, assetnamePlayerFall);
            base.LoadCache(theContentManager, assetnameJumpEffect);
            base.LoadCache(theContentManager, assetnamePlayerDash);
        }       

        public void Update(GameTime theGameTime) //check input 
        {
            MouseState mousestateCurrent = Mouse.GetState();

            updateLanded();
            updateFalling();
            updateDash(mousestateCurrent);
            updateFlying(mousestateCurrent);
            mouseFollow(mousestateCurrent);
            mousestatePrevious = mousestateCurrent;

            if (playercarCollision)
            {
                playercarCollisionPrevious = playercarCollision;
                playerCurrentState = State.Landed;
            }

            if (playercarCollision == false && playercarCollisionPrevious == true && playerCurrentState != State.Flying)
            {
                playerCurrentState = State.Falling;
                playercarCollisionPrevious = playercarCollision;
            }

            if (playerPosition.X == 1152)
            {
                playerCurrentState = State.Landed;
            }
            //keep player in playfield
            playerPosition.Y = MathHelper.Clamp(playerPosition.Y, 0, 580);
            playerPosition.X = MathHelper.Clamp(playerPosition.X, 0, 1152);
            //keep energy bounds within 0 and 100
            playerCurrentEnergy = (int)MathHelper.Clamp(playerCurrentEnergy, 0, 100);
            findPlayerTextures();

            base.Update(theGameTime, playerVelocity, playerDirection);
        }

        #region Player controls
        private void mouseFollow(MouseState aCurrentMouseState)
        {
            //player follow cursor RIGHT
            if (playerPosition.X < aCurrentMouseState.X)
            {
                runningRight = true;
                playerVelocity.X = runSpeed;
                playerDirection.X = MOVE_RIGHT;

                //go up slant //climb unavailable
                if (playerCollideBoxRight == true)
                {
                    playerVelocity.Y = uphillSpeed;
                    playerDirection.Y = MOVE_UP;
                }
            }

            //follow cursor LEFT
            if (playerPosition.X > aCurrentMouseState.X)
            {
                runningRight = false;
                playerVelocity.X = runSpeed;
                playerDirection.X = MOVE_LEFT;
            }

            //STOP
            if (playerPosition.X >= aCurrentMouseState.X && playerPosition.X <= aCurrentMouseState.X+63)
            {
                runningRight = false;
                dashingRight = false;
                runSpeed = 0;
                playerVelocity.X = 0;
                playerDirection.X = 0;
            }
        }
        
        private void updateDash(MouseState aCurrentMouseState)
        {
            if (aCurrentMouseState.RightButton == ButtonState.Pressed && dashAvailable == true)
            {
                jumpingRight = false;
                playerStartX = playerPosition.X;
                airSpeed = 600;
                playerCurrentState = State.Dashing;
                dashingRight = true;           
            }

            if (playerCurrentState == State.Dashing)
            {
                playerVelocity.X = airSpeed;
                playerDirection.X = MOVE_RIGHT;
                playerDirection.Y = 0;
                playerCurrentEnergy -= 2;

                if (playerCurrentEnergy == 0)
                {
                    dashingRight = false;
                    dashAvailable = false;
                    airSpeed = 250;                 
                    playerCurrentState = State.Falling;
                }
                else if (aCurrentMouseState.RightButton == ButtonState.Released)
                {
                    dashingRight = false;
                    airSpeed = 250;
                    playerCurrentState = State.Falling;
                }
            }
        }

        private void updateFlying(MouseState aCurrentMouseState)
        {
            if (aCurrentMouseState.LeftButton == ButtonState.Pressed && jumpAvailable == true)
            {
                if (jumpLimiter)
                {
                    jumpsUsed += 1;
                    jumpLimiter = false;
                }
                playerCurrentState = State.Flying;
                playerStartJumpY = playerPosition.Y;
                jumpingRight = true;
                currentFrame = 0;
                if (jumpsUsed >= maxJumps)
                {
                    jumpAvailable = false;
                }
            }
             
            //check when gravity will be re-applied to the player 
            if (playerCurrentState == State.Flying)
            {
                //single jump mode
                playerVelocity.Y = airSpeed;
                playerDirection.Y = MOVE_UP;
                //calculate distance differences
                if ((playerStartJumpY - playerPosition.Y) >= jumpLimit)
                {
                    jumpingRight = false;
                    fallingRight = true;
                    jumpLimiter = true;
                    currentFrame = 0;
                    playerCurrentState = State.Falling;
                }
               
                //glide mode
                //if (playerCurrentEnergy == 0)
                //{
                //    jumpingRight = false;
                //    playerCurrentState = State.Falling;
                //    jumpAvailable = false;
                //}
                //else if (aCurrentMouseState.LeftButton == ButtonState.Released)
                //{
                //    playerCurrentState = State.Falling;
                //   jumpingRight = false;
                //}

                //if (playerPosition.Y > playerStartingPosition.Y)
                //{
                //   playerPosition.Y = playerStartingPosition.Y;
                //    playerCurrentState = State.Landed;
                //    playerDirection = Vector2.Zero;
                //}
            }
        }

        private void updateFalling()
        {
            if (playerCurrentState == State.Falling)
            {
                airSpeed = 250;
                playerVelocity.Y = airSpeed;
                playerDirection.Y = MOVE_DOWN;
                fallingRight = true;
                currentFrame = 0;
            }
        }

        private void updateLanded()
        {
            if (playerCurrentState == State.Landed)
            {
                playerVelocity.Y = 0;
                playerDirection.Y = 0;
                jumpAvailable = true;
                jumpsUsed = 0;
                dashUsed = 0;
                dashAvailable = true;
                fallingRight = false;

                if(playerCurrentEnergy != 100 && dashingRight != true)
                {
                    playerCurrentEnergy += 20;                    
                }
            }
        }
        #endregion

        public void eCurrentDraw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(screenGUI.eBarTextureOVER, new Vector2(screenGUI.energybarPos.X, screenGUI.energybarPos.Y + 20), new Rectangle((int)screenGUI.energybarPos.X, (int)screenGUI.energybarPos.Y,
                (int)(screenGUI.eBarTexture.Width * ((double)playerCurrentEnergy / 100)), 20), Color.Orange);
        }

        public void animator(float elapsed)
        {
            getMaxFrames();
            if (elapsed >= frameDelay)
            {
                Console.WriteLine("fallingRight: " + fallingRight
                    + "\njumpingRight: " + jumpingRight
                    + "\nrunningRight: " + runningRight
                    + "\naddFrames: " + addFrames
                    + "\ncurrentFrame: " + currentFrame
                    + "\njumpAvailable: " + jumpAvailable
                    + "\njumpsUsed: " + jumpsUsed
                    + "\ndashAvailable: " + dashAvailable
                    + "\ndashUsed: " + dashUsed);
                //loop only for running
                if (currentFrame >= maxFrames)
                {
                    if (fallingRight || jumpingRight || runningRight)
                    {
                        addFrames = false;
                        if (fallingRight)
                        {
                            currentFrame = 0;
                            addFrames = true;
                        }
                        else if (jumpingRight)
                        {
                            jumpingRight = false;
                            currentFrame = 0;
                            addFrames = true;
                        }
                        else if (runningRight)
                        {
                            currentFrame = 0;
                            addFrames = true;
                        }
                    }
                    else
                    {
                        currentFrame = 0;
                    }
                }

                if (addFrames)
                {
                    currentFrame += 1;
                }
            }
        }

        private void getMaxFrames()
        {
          //check maxframes for animation
            if(runningRight && jumpingRight == false && fallingRight == false)
            {
             maxFrames = 5;
            }
            else if (jumpingRight)
            {
              maxFrames = 3;
            }
            else if (fallingRight)
            {
                maxFrames = 1;
            }
        }
        
        private void findPlayerTextures()
        {
            //searches through spriteTextureCache to find the correct texture
            int trackerCacheTag = 0;
            foreach (string CacheTag in spriteTextureCacheTag)
            {
                if (CacheTag == "Player/playerrunSS")
                {
                    playerRunTexture = trackerCacheTag;
                }
                else if (CacheTag == "Player/playerjumpSS")
                {
                    playerJumpTexture = trackerCacheTag;
                }
                else if (CacheTag == "Player/playerjumpfallSS")
                {
                    playerFallTexture = trackerCacheTag;
                }
                else if (CacheTag == "Player/jumpeffect")
                {
                    playerJumpEffect = trackerCacheTag;
                }
                else if (CacheTag == "Player/elbowdash")
                {
                    playerDashTexture = trackerCacheTag;
                }
                else if (CacheTag == "Player/player")
                {
                    playerStandTexture = trackerCacheTag;
                }
                
                trackerCacheTag++;
            }
        }

        #region Animations & other player fx
        new public void Draw(SpriteBatch batch)
        {
            if (runningRight == true && jumpingRight == false && fallingRight == false && dashingRight == false && playerCurrentState == State.Landed)
            {
                animationRun(batch);
            }
            else if(jumpingRight == true && dashingRight == false)
            {
                animationJump(batch);
                animationJumpEffect(batch);
            }
            else if (fallingRight == true && dashingRight == false)
            {
                animationFall(batch);
            }
            else if (dashingRight == true)
            {
                dashEffect(batch);
            }
            else
            {
                animationStand(batch);
            }
        }

        private void animationStand(SpriteBatch batch)
        {
            batch.Draw((Texture2D)spriteTextureCache[playerStandTexture],
                new Rectangle((int)playerPosition.X, (int)playerPosition.Y, 64, 128),
                new Rectangle(0, 0, 64, 128), Color.White);
        }
    
        private void animationRun(SpriteBatch batch)
        {
            batch.Draw((Texture2D)spriteTextureCache[playerRunTexture], 
                new Rectangle((int)playerPosition.X, (int)playerPosition.Y, 64, 128),
                new Rectangle(currentFrame*64,0,64,128),Color.White);
        }

        private void animationJump(SpriteBatch batch)
        {
            batch.Draw((Texture2D)spriteTextureCache[playerJumpTexture],
                new Rectangle((int)playerPosition.X, (int)playerPosition.Y, 64, 128),
                new Rectangle(currentFrame * 64, 0, 64, 128), Color.White);
        }

        private void animationJumpEffect(SpriteBatch batch)
        {
            batch.Draw((Texture2D)spriteTextureCache[playerJumpEffect],
                new Rectangle((int)playerPosition.X - 10, (int)playerStartJumpY + 128, 84, 25),
                new Rectangle(currentFrame * 84, 0, 84, 25), Color.White);
        }

        private void animationFall(SpriteBatch batch)
        {
            batch.Draw((Texture2D)spriteTextureCache[playerFallTexture],
                new Rectangle((int)playerPosition.X, (int)playerPosition.Y, 64, 128),
                new Rectangle(currentFrame * 64, 0, 64, 128), Color.White);
        }
        private void dashEffect(SpriteBatch batch)
        {
            batch.Draw((Texture2D)spriteTextureCache[playerDashTexture],
                new Rectangle((int)playerPosition.X, (int)playerPosition.Y, 64, 128),
                new Rectangle(0, 0, 64, 128), Color.White);
        }
        #endregion
    }
        
}
