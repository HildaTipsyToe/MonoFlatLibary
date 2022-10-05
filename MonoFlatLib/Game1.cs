using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FlatLibMonogame;
using FlatLibMonogame.Graphics;
using FlatLibMonogame.Input;

namespace MonoFlatLib
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private Shapes shapes;
        private Screen screen;
        private Camera camera;
        private Sprites sprites;
        private int Height = 720;
        private int Width = 1280;

        public Game1()
        {
            this.graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            this.graphics.PreferredBackBufferHeight = Height;
            this.graphics.PreferredBackBufferWidth = Width;
            this.graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            this.shapes = new Shapes(this);
            this.screen = new Screen(this, this.Width, this.Height);
            this.camera = new Camera(screen);
            this.sprites = new Sprites(this);


            base.Initialize();
        }

        protected override void LoadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            this.screen.Set();

            this.shapes.Begin(null);
            this.shapes.DrawCircleFill(this.screen.Width / 2, this.screen.Height / 2, 50f, 36, Color.White);
            this.shapes.End();

            this.screen.Unset();
            this.screen.Present(this.sprites);


            base.Draw(gameTime);
        }
    }
}