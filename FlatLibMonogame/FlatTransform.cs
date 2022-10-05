using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatLibMonogame
{
    public struct FlatTransform
    {
        public float PosX;
        public float PosY;
        public float CosScaleX;
        public float SinScaleX;
        public float SinScaleY;
        public float CosScaleY;


        public FlatTransform(Vector2 Position, float angle, Vector2 scale)
        {
            float sin = MathF.Sin(angle);
            float cos = MathF.Cos(angle);

            this.PosX = Position.X;
            this.PosY = Position.Y;
            this.CosScaleX = cos * scale.X;
            this.SinScaleX = sin * scale.X;
            this.CosScaleY = cos * scale.Y;
            this.SinScaleY = sin * scale.Y;
            
        }

        public FlatTransform(Vector2 Position, float angle, float scale)
        {
            float sin = MathF.Sin(angle);
            float cos = MathF.Cos(angle);

            this.PosX = Position.X;
            this.PosY = Position.Y;
            this.CosScaleX = cos * scale;
            this.SinScaleX = sin * scale;
            this.CosScaleY = cos * scale;
            this.SinScaleY = sin * scale;
        }
    }
}
