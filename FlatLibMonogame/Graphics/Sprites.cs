using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace FlatLibMonogame.Graphics
{
    public sealed class Sprites
    {
        private bool isDisposed;
        private Game game;
        private SpriteBatch sprite;
        private BasicEffect effect;

        public Sprites(Game game)
        {
            if( game is null)
            {
                throw new ArgumentNullException("game");
            }

            this.game = game;

            this.isDisposed = false;

            this.sprite = new SpriteBatch(this.game.GraphicsDevice);

            this.effect = new BasicEffect(this.game.GraphicsDevice);

            this.effect.FogEnabled = false;
            this.effect.TextureEnabled = true;
            this.effect.LightingEnabled = false;
            this.effect.VertexColorEnabled = true;
            this.effect.World = Matrix.Identity;
            this.effect.Projection = Matrix.Identity;
            this.effect.View = Matrix.Identity;
        }

        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.effect?.Dispose();
            this.sprite?.Dispose();
            this.isDisposed = true;
        }

        public void begin(Camera camera, bool isTextureFilteringEnabled)
        {
            SamplerState sampler = SamplerState.PointClamp;
            if (isTextureFilteringEnabled)
            {
                sampler = SamplerState.LinearClamp;
            }

            if (camera is null)
            {
                Viewport vp = this.game.GraphicsDevice.Viewport;
                this.effect.Projection = Matrix.CreateOrthographicOffCenter(0, vp.Width, 0, vp.Height, 0f, 1f);
                this.effect.View = Matrix.Identity;
            }
            else
            {
                camera.UpdateMatrices();

                this.effect.View = camera.View;
                this.effect.Projection = camera.Projection;
            }


            this.sprite.Begin(blendState: BlendState.AlphaBlend, samplerState: sampler, rasterizerState: RasterizerState.CullNone,effect:  this.effect);
        }
        public void end()
        {
            this.sprite.End();
        }
        #region Tegner med spritebatch, en box
        public void Draw(Texture2D texture,Vector2 origin, Vector2 position, Color color)
        {
            this.sprite.Draw(texture, position, null, color, 0f, origin, 1f, SpriteEffects.FlipVertically, 0f);
        }
        public void Draw( Texture2D texture, Rectangle? sourceRectangle, Vector2 origin, Vector2 postion, float roation, Vector2 scale, Color color)
        {
            this.sprite.Draw(texture, postion, sourceRectangle, color, roation, origin, scale, SpriteEffects.FlipVertically, 0f);
        }

        public void Draw(Texture2D texture, Rectangle? sourceRectangle, Rectangle desinationRectangle, Color color)
        {
            this.sprite.Draw(texture, desinationRectangle, sourceRectangle, color, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0f);
        }
        #endregion
    }
}
