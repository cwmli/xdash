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

namespace Fly_Away
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class FlyAway : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        spritePlayer currentPlayer;
        screenGUI GUI;
        List<carLogic> carList = new List<carLogic>();
        List<roadScroll> scrollTextures = new List<roadScroll>();
        List<backgroundParallax> parallaxTextures = new List<backgroundParallax>();
        Texture2D menuBackground, menuBackground_1, menuTitle;

        //timers && trackers
        float animatedelay;
        const float carSpawnDelay = 2; 
        int recentCarSpawn;
        int collidedwithcar;

        enum ScreenState
        {
            menu,
            game,
            end
        }

        ScreenState currentScreen;
        
        public FlyAway()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.ApplyChanges();
            this.IsMouseVisible = true;
            currentScreen = ScreenState.menu;
            #region Setup GameState
         
            //init car (loading textures etc)
            createCar();    
            for (int i = 0; i < 3; i++)
            {
                scrollTextures.Add(new roadScroll(GraphicsDevice,i));
            }
            for (int i = 0; i < 4; i++)
            {
                parallaxTextures.Add(new backgroundParallax(i));
            }
            currentPlayer = new spritePlayer();
            GUI = new screenGUI();
            #endregion

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            #region GameState related
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            foreach (backgroundParallax texture in parallaxTextures)
            {
                texture.Load(this.Content);
            }
            foreach (roadScroll scrolling in scrollTextures)
            {
                scrolling.Load(this.Content);
            }
            currentPlayer.LoadContent(this.Content);
            GUI.LoadEnergyBar(GraphicsDevice, this.Content);
            #endregion
            #region MenuState related
            menuBackground = Content.Load<Texture2D>("Scene/newtitle");
            menuBackground_1 = Content.Load<Texture2D>("Scene/watersparkles");
            menuTitle = Content.Load<Texture2D>("Scene/gametitle");
            #endregion
        }

        void createCar()
        {
            recentCarSpawn = carList.Count;
            int screenwidth = GraphicsDevice.Viewport.Width;
            int bkTextureIndex = carLogic.Shared.Random.Next(7);
            carList.Add(new carLogic(new Vector2(screenwidth + 250, 675), bkTextureIndex, this.Content));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            switch (currentScreen)
            {
                case ScreenState.menu:
                    {
                        UpdateMenuScreen(gameTime);
                        break;
                    }
                case ScreenState.game:
                    {
                        UpdateGameScreen(gameTime);
                        break;
                    }
                case ScreenState.end:
                    {
                        UpdateEndScreen(gameTime);
                        break;
                    }
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Transparent);

            switch (currentScreen)
            {
                case ScreenState.menu:
                    {
                        DrawMenuScreen();
                        break;
                    }
                case ScreenState.game:
                    {
                        DrawGameScreen();
                        break;
                    }
            }
            base.Draw(gameTime);
        }
        
        private void DrawMenuScreen()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(menuBackground, new Rectangle(0, 0, menuBackground.Width, menuBackground.Height), Color.White);
            spriteBatch.Draw(menuBackground_1, new Rectangle(0, 470, menuBackground_1.Width, menuBackground_1.Height), Color.White);
            spriteBatch.Draw(menuTitle, new Rectangle(140, 0, menuTitle.Width, menuTitle.Height), Color.White);
            spriteBatch.End();
        }
        private void DrawGameScreen()
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, null);
            foreach (backgroundParallax texture in parallaxTextures)
            {
                texture.parallaxDraw(spriteBatch);
            }
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, null);
            foreach (roadScroll scrollable in scrollTextures)
            {
                scrollable.Draw(spriteBatch);
            }
            foreach (carLogic car in carList)
            {
                car.Draw(spriteBatch);
            }

            currentPlayer.Draw(spriteBatch);
            currentPlayer.eCurrentDraw(spriteBatch);

            GUI.drawGUI(spriteBatch);
            spriteBatch.End();
        }
        private void DrawEndScreen()
        {

        }
        private void UpdateMenuScreen(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                currentScreen = ScreenState.game;
            }
        }
        private void UpdateGameScreen(GameTime gameTime)
        {
            currentPlayer.Update(gameTime);

            if (currentPlayer.playerUnclimable == true)
            {
                currentScreen = ScreenState.end;
            }

            //get elasped time in seconds for use in background scrolling 
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //animation
            animatedelay += (float)gameTime.ElapsedGameTime.TotalSeconds;
            currentPlayer.animator(animatedelay);
            //clear delay
            if (animatedelay >= currentPlayer.animationDelay)
            {
                animatedelay = 0;
            }

            //updates background movement causing scroll/moving effect
            foreach (roadScroll scrollable in scrollTextures)
            {
                scrollable.Update(elapsed * 700);
            }

            foreach (backgroundParallax texture in parallaxTextures)
            {
                texture.updateParallax(elapsed * 50);
            }

            //get bounding rectangle for playerPlane
            Rectangle playerRect = new Rectangle((int)currentPlayer.playerPos.X, (int)currentPlayer.playerPos.Y,
                64, 128);

            //---------------------------------------CARSECTION--------------------------------------//
            //carspawning

            if (carList[recentCarSpawn].carPos.X - (int)GraphicsDevice.Viewport.Width <= -100 && carList.Count < 6) //&& carSpawnTimer >= carSpawnDelay)
            {
                //carSpawnTimer -= carSpawnDelay;
                createCar();
                //Console.WriteLine("New car created. Most Recent Spawn: " + recentCarSpawn);
            }


            //car scroll effect/moving
            foreach (carLogic car in carList)
            {
                car.UpdateCar(elapsed * currentPlayer.airSpeed);
                car.UpdateWheels(elapsed * 1200 * 1 / 5);
            }

            for (int i = carList.Count - 1; i >= 0; i--)
            {
                //check car positions and remove if it is offscreen
                if (carList[i].carPos.X < -250)
                {
                    carList.RemoveAt(i);
                    recentCarSpawn -= 1;
                    //Console.WriteLine("Car removed from list cars.");
                }
            }

            for (int i = 0; i < carList.Count; i++)
            {
                //get bounding rectangle of current car in list cars
                //yOffset accounts for origin change in draw method
                Rectangle carRect = new Rectangle((int)carList[i].carPos.X, (int)carList[i].carPos.Y - carList[i].yOffset,
                    genSprite.spriteTextureCache[(int)carList[i].pub_TextureIndex].Width,
                    genSprite.spriteTextureCache[(int)carList[i].pub_TextureIndex].Height);

                //check car collision with player
                if (colDetection.checkIntersects(playerRect, genSprite.spriteTextureData[1],
                                carRect, genSprite.spriteTextureData[carList[i].pub_TextureIndex], out currentPlayer.playerCollideBoxRight, out currentPlayer.playerUnclimable))
                {
                    //Console.WriteLine("Collision found with " + playerRect + " and " + carRect + "...");
                    //Console.WriteLine("BoxRight: " + currentPlayer.playerCollideBoxRight);
                    currentPlayer.playercarCollision = true; //use bool playercarCollision to activate any preceeding functions/events
                    collidedwithcar = i; //keep the car (using index) player collided with in cars list
                }

                if (currentPlayer.playercarCollision == true)
                {
                    //Console.WriteLine("Checking player with previous collision...");
                    //get the rectangle
                    Rectangle collidedwithcarRect = new Rectangle((int)carList[collidedwithcar].carPos.X, (int)carList[collidedwithcar].carPos.Y - carList[collidedwithcar].yOffset,
                        genSprite.spriteTextureCache[(int)carList[collidedwithcar].pub_TextureIndex].Width,
                        genSprite.spriteTextureCache[(int)carList[collidedwithcar].pub_TextureIndex].Height);

                    //check if the player is no longer collided with the car they had first collided
                    if (colDetection.checkIntersects(playerRect, genSprite.spriteTextureData[1],
                                    collidedwithcarRect, genSprite.spriteTextureData[carList[collidedwithcar].pub_TextureIndex],
                                    out currentPlayer.playerCollideBoxRight, out currentPlayer.playerUnclimable) == false)
                    {
                        currentPlayer.playercarCollision = false;
                        //Console.WriteLine("Left collision bounds setting playercarCollision to false.");
                    }
                }
            }
        }
        private void UpdateEndScreen(GameTime gameTime)
        {

        }
    }
}
