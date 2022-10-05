using Microsoft.Xna.Framework.Input;
using System;
using System.Net.Http.Headers;

namespace FlatLibMonogame.Input
{
    public sealed class FlatKeyboard
    {
        private static readonly Lazy<FlatKeyboard> Lazy = new Lazy<FlatKeyboard>(() => new FlatKeyboard());

        public static FlatKeyboard Instance
        {
            get { return Lazy.Value; }
        }

        private KeyboardState prevKeyboardState;
        private KeyboardState currKeyboardState;

        public FlatKeyboard()
        {
            this.prevKeyboardState = Keyboard.GetState();
            this.currKeyboardState = prevKeyboardState;
        }

        public void Update()
        {
            this.prevKeyboardState = currKeyboardState;
            this.currKeyboardState = Keyboard.GetState();
        }

        public bool IsKeyDown(Keys keys)
        {
            return this.currKeyboardState.IsKeyDown(keys);
        }

        public bool IsKeyClicked(Keys key)
        {
            return this.currKeyboardState.IsKeyDown(key) && ! this.prevKeyboardState.IsKeyDown(key);
        }


    }
}
