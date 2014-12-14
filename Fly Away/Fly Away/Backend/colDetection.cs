using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Fly_Away
{
    class colDetection
    {
        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels
        /// between two sprites.
        /// </summary>
        /// <param name="rectangleA">Bounding rectangle of the first sprite</param>
        /// <param name="dataA">Pixel data of the first sprite</param>
        /// <param name="rectangleB">Bouding rectangle of the second sprite</param>
        /// <param name="dataB">Pixel data of the second sprite</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        public static bool checkIntersects(Rectangle rectangleA, Color[] dataA,
                                    Rectangle rectangleB, Color[] dataB, out bool? playerCollideBoxRight, out bool? playerUnclimable)
        {
            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);
            collisionPoint position;
            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y+=5)
            {
                for (int x = left; x < right; x+=5)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        //get the point of collision
                        position.PositionX = x;
                        position.PositionY = y;
                        //get collision area
                        if (getCollisionRight(rectangleA, position.PositionX, position.PositionY))
                        {
                            playerCollideBoxRight = true;
                            //Console.WriteLine("BoxRight detected. Sent out true.");
                        }
                        else
                        {
                            playerCollideBoxRight = false;
                        }

                        if(getUnclimable(rectangleA, position.PositionX, position.PositionY))
                        {
                            playerUnclimable = true;
                        }
                        else
                        {
                            playerUnclimable = false;
                        }
                        return true;
                    }
                }
            }
            // No intersection found
            playerCollideBoxRight = null;
            playerUnclimable = null;
            return false;
        }

        private struct collisionPoint
        {
            public int PositionX;
            public int PositionY;
        }

        public static bool getCollisionRight(Rectangle rectangleA, int PositionX, int PositionY)
        {
            //if there is obstacle to right and is not higher than max climb, then climb
            if (PositionX > rectangleA.Width / 2 && PositionY < rectangleA.Y + 123 && PositionY > rectangleA.Y + 96) 
            {
                return true;
            }
            else
            {    
                return false;
            }   
        }

        public static bool getUnclimable(Rectangle rectangleA, int PositionX, int PositionY)
        {
            if (PositionX > rectangleA.Width / 2 && PositionY < rectangleA.Y + 95)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
