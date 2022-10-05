using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatLibMonogame
{
    public static class Util
    {
        public static void ToggleFullScreen(GraphicsDeviceManager graphics)
        {
            graphics.HardwareModeSwitch = false;
            graphics.ToggleFullScreen();
        }

        public static int Clamp(int value, int min, int max)
        {
            if(min > max)
            {
                throw new ArgumentOutOfRangeException("The value of \"man is greater than the value of \"max");
            }

            if(value < min)
            {
                return min;
            }
            else if( value > max)
            {
                return max;
            }
            else
            {
                return value;
            }
        }
        public static float Clamp(float value, float min, float max)
        {
            if (min > max)
            {
                throw new ArgumentOutOfRangeException("The value of \"man is greater than the value of \"max");
            }

            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            else
            {
                return value;
            }
        }
        
        public static void Normalize(ref float x, ref float y)
        {
            float invLen = 1f / (float)Math.Sqrt(x * x + y * y);

            x *= invLen;
            y *= invLen; 
        }
        /*public static Vector2 TransformSlow(Vector2 position, FlatTransform transform)
        {
            //scale
            float sx = position.X * transform.ScaleX;
            float sy = position.Y * transform.ScaleY;

            //Rotation
            float rx = sx * transform.Cos - sy * transform.Sin;
            float ry = sy * transform.Sin + sx * transform.Cos;

            //Translation
            float tx = rx + transform.PosX;
            float ty = ry + transform.PosY;

            return new Vector2(tx, ty);

        }*/
        public static Vector2 Transform(Vector2 position, FlatTransform transform)
        {
            return new Vector2(
                position.X * transform.CosScaleX - position.Y * transform.SinScaleY + transform.PosX,
                position.X * transform.SinScaleX + position.Y * transform.CosScaleY + transform.PosY);

        }
    }
}
