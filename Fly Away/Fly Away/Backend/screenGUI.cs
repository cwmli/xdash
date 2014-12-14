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
    class screenGUI
    {
        #region EnergyBar variables
        const string assetEnergyBar = "healthbar";
        const string assetCurrentEnergy = "healthbarE";
        public static Vector2 energybarPos;
        public static Texture2D eBarTexture, eBarTextureOVER;
        #endregion

        /// <summary>
        /// Energy bar for playerPlane GUI
        /// </summary>
        public void LoadEnergyBar(GraphicsDevice graphics, ContentManager theContentManager)
        {
            eBarTexture = theContentManager.Load<Texture2D>(assetEnergyBar);
            eBarTextureOVER = theContentManager.Load<Texture2D>(assetCurrentEnergy);
            energybarPos.X = graphics.Viewport.Width * 1 / 32;
            energybarPos.Y = graphics.Viewport.Height * 1 / 32;
        }
       
        public void drawGUI(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(eBarTexture, energybarPos,
                    new Rectangle(0, 0, eBarTexture.Width, eBarTexture.Height), Color.White,
                    0.0f, Vector2.Zero, 1, SpriteEffects.None, 0);
        }
     }
}
