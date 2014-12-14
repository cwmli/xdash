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
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Sprite basebackground;
        paperplane mPaperPlane;

        List<car> cars = new List<car>();
        List<Scrolling> scroll = new List<Scrolling>();

        //float carSpawnTimer;
        const float carSpawnDelay = 2; 
        int recentCarSpawn;

        public Game1()
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

            int screenwidth = GraphicsDevice.Viewport.Width;

            //start one car
            CreateCar();
            

            for (int i = 0; i < 3; i++)
            {
                scroll.Add(new Scrolling(GraphicsDevice,i));
            }

            basebackground = new Sprite();
            mPaperPlane = new paperplane();

            //scrolling sprites
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            foreach (car car in cars)
            {
                car.Load(this.Content); //change load method: should load all textures and store them into bkTexture list in car.cs not foreach car in cars individually.
            }


            foreach (Scrolling scrolling in scroll)
            {
                scrolling.Load(this.Content);
            }

            mPaperPlane.LoadContent(this.Content);
            mPaperPlane.LoadEnergyBar(GraphicsDevice, this.Content);

            basebackground.LoadContent(this.Content, "background");
            basebackground.Position = new Vector2(0, 0);


            // TODO: use this.Content to load your game content here
        }

        void CreateCar()
        {
            recentCarSpawn = cars.Count;
            int screenwidth = GraphicsDevice.Viewport.Width;
            int bkTextureIndex = car.Shared.Random.Next(7);
            Console.WriteLine(bkTextureIndex);
            cars.Add(new car(new Vector2(screenwidth + 250, 650), bkTextureIndex));
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

            mPaperPlane.Update(gameTime);
            mPaperPlane.eUpdate(gameTime);


            //scrolling stuff
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //keep at same speed
            foreach (Scrolling scrolling in scroll)
            {
                scrolling.Update(elapsed * 1200);
            }

            //car stuff
            //car spawning //OLD VERSION
            //carSpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            //if (carSpawnTimer >= carSpawnDelay && cars.Count < 6) 
            //{
            //    carSpawnTimer -= carSpawnDelay; //subtracts "used" time
            //    CreateCar();
            //    Console.WriteLine("New car inserted.");
            //}

            //carspawning
            if (cars[recentCarSpawn].Position.X - (int)GraphicsDevice.Viewport.Width <= -100 && cars.Count < 6) //&& carSpawnTimer >= carSpawnDelay)
            {
                //carSpawnTimer -= carSpawnDelay;
                CreateCar();
                Console.WriteLine("New car inserted. Most Recent Spawn: " + recentCarSpawn);
            }


            //car scroll effect/moving
            foreach (car car in cars)
            {
                car.UpdateCar(elapsed * mPaperPlane.PLANE_SPEED);
                car.UpdateWheels(elapsed * 1200 * 1/5);
                //Console.WriteLine(car.Position);
            }

            //remove cars that are offscreen
            for (int i = cars.Count - 1; i >= 0; i--)
            {
                if (cars[i].Position.X < -250)
                {
                    cars.RemoveAt(i);
                    recentCarSpawn -= 1;
                    Console.WriteLine("Car removed from list cars.");
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            basebackground.Draw(this.spriteBatch);
            spriteBatch.End();



            spriteBatch.Begin();
            foreach (Scrolling scrolling in scroll)
            {
                 scrolling.Draw(spriteBatch);
            }            
            foreach (car car in cars)
            {
                car.Draw(spriteBatch);
            }

            mPaperPlane.Draw(spriteBatch);
            mPaperPlane.eCurrentDraw(spriteBatch);
            mPaperPlane.eDraw(spriteBatch);


            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
