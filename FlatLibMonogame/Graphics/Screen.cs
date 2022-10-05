using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlatLibMonogame.Graphics
{
    public sealed class Screen : IDisposable
    {
        private readonly static int MinDim = 64;
        private readonly static int MaxDim = 4096;


        private bool isDisposed;
        private Game game;
        private RenderTarget2D target;
        private bool isSet;

        public int Width
        {
            get { return target.Width; }
        }

        public int Height
        {
            get { return target.Height; }
        }

        public Screen(Game game, int width, int height)
        {
            width = Util.Clamp(width, Screen.MinDim, Screen.MaxDim);
            height = Util.Clamp(height, Screen.MinDim, Screen.MaxDim);

            this.game = game ?? throw new ArgumentNullException("game");

            this.target = new RenderTarget2D(this.game.GraphicsDevice, width, height);
            this.isSet = false;
        }

        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.target?.Dispose();
            this.isDisposed = true;
        }

        public void Set()
        {
            if (this.isSet)
            {
                throw new Exception("Render taget is already set.");
            }

            this.game.GraphicsDevice.SetRenderTarget(this.target);
            this.isSet = true;
        }
        public void Unset()
        {
            if (!this.isSet)
            {
                throw new Exception("Render taget is not set.");
            }

            this.game.GraphicsDevice.SetRenderTarget(null); 
            this.isSet = false;
        }   

        public void Present(Sprites sprites, bool textrueFultering = true)
        {
            if (sprites is null)
            {
                throw new ArgumentNullException("sprites");
            }
#if DEBUG
            this.game.GraphicsDevice.Clear(Color.HotPink);
#else   
            this.game.GraphicsDevice.Clear(Color.Red);     
#endif
            Rectangle destinationRectagnle = this.CalculateDestinationRectangle();

            sprites.begin(null, textrueFultering);
            sprites.Draw(this.target, null, destinationRectagnle, Color.White);
            sprites.end();
        }

#region beregner opløsningen på skræmen
        internal Rectangle CalculateDestinationRectangle()
        {
            Rectangle backbufferBounds = this.game.GraphicsDevice.PresentationParameters.Bounds;
            float backbufferAspectRation = (float)backbufferBounds.Width / (float)backbufferBounds.Height;
            float screenAspectRatio = (float)this.Width / (float)this.Height;

            float rx = 0f;
            float ry = 0f;
            float rw = backbufferBounds.Width;
            float rh = backbufferBounds.Height;

            if(backbufferAspectRation > screenAspectRatio)
            {
                rw = rh * screenAspectRatio;
                rx = (float)(backbufferBounds.Width - rw) / 2f;
            }
            else if( backbufferAspectRation < screenAspectRatio)
            {
                rh = rw / screenAspectRatio;
                ry = (float)(backbufferBounds.Height - rh) / 2f;
            }
            Rectangle result = new Rectangle((int)rx, (int)ry, (int)rw, (int)rh);
            return result;
        }
#endregion
    }
}
