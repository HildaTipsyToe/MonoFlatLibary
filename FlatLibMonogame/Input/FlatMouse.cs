using FlatLibMonogame.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatLibMonogame.Input
{
    public sealed class FlatMouse
    {
        private static readonly Lazy<FlatMouse> Lazy = new Lazy<FlatMouse>(() => new FlatMouse());

        public static FlatMouse Instance
        {
            get { return Lazy.Value; }
        }

        private MouseState prevMouseState;
        private MouseState currMouseState;

        public Point WindowPosition
        {
            get { return this.currMouseState.Position; }
        }

        public FlatMouse()
        {
            this.prevMouseState = Mouse.GetState();
            this.currMouseState = prevMouseState;
        }

        public void Update()
        {
            this.prevMouseState = currMouseState;
            this.currMouseState = Mouse.GetState();
        }
        
        public bool IsLeftButtonDown()
        {
            return this.currMouseState.LeftButton == ButtonState.Pressed;
        }
        public bool IsRightButtonDown()
        {
            return this.currMouseState.RightButton == ButtonState.Pressed;
        }
        public bool isMiddleButtonDown()
        {
            return this.currMouseState.MiddleButton == ButtonState.Pressed;
        }

        public bool isLeftButtonClicked()
        {
            return this.currMouseState.LeftButton == ButtonState.Pressed && this.prevMouseState.LeftButton == ButtonState.Released;
        }

        public bool isRightButtonClicked()
        {
            return this.currMouseState.RightButton == ButtonState.Pressed && this.prevMouseState.RightButton == ButtonState.Released;
        }

        public bool isMiddleButtonClicked()
        {
            return this.currMouseState.MiddleButton == ButtonState.Pressed && this.prevMouseState.MiddleButton == ButtonState.Released;
        }

        public Vector2 GetScreenPosition(Screen screen)
        {
            Rectangle screenDestianationRectangle = screen.CalculateDestinationRectangle();

            Point windowPosition = this.WindowPosition;

            float sX = windowPosition.X - screenDestianationRectangle.X;
            float sY = windowPosition.Y - screenDestianationRectangle.Y;

            sX /= (float)screenDestianationRectangle.Width;
            sY /= (float)screenDestianationRectangle.Height;

            sX *= screen.Width;
            sY *= screen.Height;

            sY = (float)screen.Height - sY;

            return new Vector2(sX, sY);
        }
    }
}
