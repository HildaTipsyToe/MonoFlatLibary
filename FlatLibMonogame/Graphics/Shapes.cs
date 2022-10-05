using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace FlatLibMonogame.Graphics
{
    public sealed class Shapes : IDisposable
    {
        public static readonly float MinLineThicknesss = 1f;
        public static readonly float MaxLineThickness = 10f;

        private bool IsDisposed;
        private Game game;
        private BasicEffect effect;
        private VertexPositionColor[] vertices;
        private int[] indices;

        private int shapesCount;
        private int vertexCount;
        private int indexCount;

        private bool isStarted;

        private Camera camera;

        public Shapes(Game game)
        {
            this.IsDisposed = false;
            this.game = game ?? throw new ArgumentNullException("game");
            this.effect = new BasicEffect(this.game.GraphicsDevice);
            this.effect.FogEnabled = false;
            this.effect.TextureEnabled = false;
            this.effect.LightingEnabled = false;
            this.effect.VertexColorEnabled = true;
            this.effect.World = Matrix.Identity;
            this.effect.View = Matrix.Identity;
            this.effect.Projection = Matrix.Identity;

            const int MaxVertexCount = 1024;
            const int MaxIndexCount = MaxVertexCount * 3;

            this.vertices = new VertexPositionColor[MaxVertexCount];
            this.indices = new int[MaxIndexCount];

            this.shapesCount = 0;
            this.vertexCount = 0;
            this.indexCount = 0;

            this.isStarted = false;
            this.camera = null;
        }

        public void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            this.effect?.Dispose();
            this.IsDisposed = true;
        }

        public void Begin(Camera camera)
        {
            if (this.isStarted)
            {
                throw new Exception("batching is already started");
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

            this.camera = camera;

            //Viewport vp = this.game.GraphicsDevice.Viewport;
            //this.effect.Projection = Matrix.CreateOrthographicOffCenter(0, vp.Width, 0, vp.Height, 0f, 1f);

            this.isStarted = true;


        }
        public void End()
        { 
            this.Flush();
            this.isStarted = false;
        }
        public void Flush()
        {
            if(this.shapesCount == 0)
            {
                return;
            }

            this.EnsureStarted();

            foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.TriangleList,
                    this.vertices,
                    0,
                    this.vertexCount,
                    this.indices,
                    0,
                    this.indexCount / 3);
            }

            this.shapesCount = 0;
            this.vertexCount = 0;
            this.indexCount = 0;
        }
        private void EnsureStarted()
        {
            if (!this.isStarted)
            {
                throw new Exception("batching was never started");
            }
        }
        private void EnsureSpace(int shapeVertexcount, int shapeIndexcount)
        {
            if(shapeVertexcount > this.vertices.Length)
            {
                throw new Exception("Maximum shape vertex count is: " + this.vertices.Length);
            }

            if(shapeIndexcount > this.indices.Length)
            {
                throw new Exception("Maximum shape index count is: " + this.indices.Length);
            }

            if(this.vertexCount + shapeVertexcount > this.vertices.Length || 
                this.indexCount + shapeIndexcount > this.indices.Length)
            {
                Flush();
            }
        }
        public void DrawRectangleFill(float x, float y, float width, float height, Color color)
        {
            this.EnsureStarted();
            const int shapeVertexCount = 4;
            const int shapeIndexCount = 6;

            this.EnsureSpace(shapeVertexCount, shapeIndexCount);

            float left = x;
            float right = x + width;
            float buttom = y;
            float top = y + height;

            Vector2 a = new Vector2(left, top);
            Vector2 b = new Vector2(right, top);
            Vector2 c = new Vector2(right, buttom);
            Vector2 d = new Vector2(left, buttom);

            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = 1 + this.vertexCount;
            this.indices[this.indexCount++] = 2 + this.vertexCount;
            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = 2 + this.vertexCount;
            this.indices[this.indexCount++] = 3 + this.vertexCount;


            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(a, 0f), color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(b, 0f), color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(c, 0f), color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(d, 0f), color);

            this.shapesCount++;
        }
        public void DrawLineSlow(Vector2 a, Vector2 b, float Thickness, Color color)
        {
            this.EnsureStarted();

            const int shapeVertexCount = 4;
            const int shapeIndexCount = 6;

            this.EnsureSpace(shapeVertexCount, shapeIndexCount);

            Thickness = Util.Clamp(Thickness, MinLineThicknesss, MaxLineThickness);
            Thickness++;

            float halfThickness = Thickness / 2f;

            Vector2 e1 = b - a;
            e1.Normalize();
            e1 *= halfThickness;

            Vector2 e2 = -e1;
            Vector2 n1 = new Vector2(-e1.Y, e1.X);
            Vector2 n2 = -n1;

            Vector2 q1 = a + n1 * e2; 
            Vector2 q2 = b + n1 * e1; 
            Vector2 q3 = b + n2 * e1; 
            Vector2 q4 = a + n2 * e2;

            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = 1 + this.vertexCount;
            this.indices[this.indexCount++] = 2 + this.vertexCount;
            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = 2 + this.vertexCount;
            this.indices[this.indexCount++] = 3 + this.vertexCount;


            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(q1, 0f), color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(q2, 0f), color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(q3, 0f), color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(q4, 0f), color);

            this.shapesCount++;
        }
        public void DrawLine(Vector2 a, Vector2 b, float Thickness, Color color)
        {
            this.DrawLine(a.X,a.Y,b.X,b.Y, Thickness, color);
        }
        public void DrawLine(float ax, float ay, float bx, float by, float Thickness, Color color)
        {
            this.EnsureStarted();

            const int shapeVertexCount = 4;
            const int shapeIndexCount = 6;

            this.EnsureSpace(shapeVertexCount, shapeIndexCount);

            Thickness = Util.Clamp(Thickness, MinLineThicknesss, MaxLineThickness);
            Thickness++;

            //linjen forbliver den samme tykkelse, lige meget hvor meget du er zoomet ind
            if(this.camera != null)
            {
                Thickness *= (this.camera.Z / this.camera.BaseZ);
            }

            float halfThickness = Thickness / 2f;

            float e1x = bx - ax;
            float e1y = by - ay;
            Util.Normalize( ref e1x, ref e1y );

            e1x *= halfThickness;
            e1y *= halfThickness;

            float e2x = -e1x;
            float e2y = -e1y;

            float n1x = -e1y;
            float n1y = e1x;

            float n2x = -n1x;
            float n2y = -n1y;

            float q1x = ax + n1x + e2x;
            float q1y = ay + n1y + e2y;

            float q2x = bx + n1x + e1x;
            float q2y = by + n1y + e1y;

            float q3x = bx + n2x + e1x;
            float q3y = by + n2y + e1y;

            float q4x = ax + n2x + e2x;
            float q4y = ay + n2y + e2y;

            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = 1 + this.vertexCount;
            this.indices[this.indexCount++] = 2 + this.vertexCount;
            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = 2 + this.vertexCount;
            this.indices[this.indexCount++] = 3 + this.vertexCount;


            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(q1x, q1y, 0f), color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(q2x, q2y, 0f), color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(q3x, q3y, 0f), color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(q4x, q4y, 0f), color);

            this.shapesCount++;
        }
        public void DrawRectangle(float x, float y, float width, float height, float thickness, Color color)
        {
            float left = x;
            float right = x + width;
            float bottom = y;
            float top = y + height;

            this.DrawLine(left, top, right, top, thickness, color);
            this.DrawLine(right, top, right, bottom, thickness, color);
            this.DrawLine(right, bottom, left, bottom, thickness, color);
            this.DrawLine(left, top, left, bottom, thickness, color);
        }
        public void DrawCircle(float x, float y, float radius,  int Points, float thickness, Color color)
        {
            const int MinPoints = 3;
            const int MaxPoints = 256;

            Points = Util.Clamp(Points, MinPoints, MaxPoints);

            //float deltaAngle = MathHelper.TwoPi / (float)Points;
            //float angle = 0f;

            float rotation = MathHelper.TwoPi / (float)Points;
            float sin = MathF.Sin(rotation);
            float cos = MathF.Cos(rotation);

            //float ax = MathF.Sin(angle) * radius + x;
            //float ay = MathF.Cos(angle) * radius + y;

            float ax = radius;
            float ay = 0f;

            float bx = 0f;
            float by = 0f;

            for (int i = 0; i < Points; i++)
            {
                bx = cos * ax - sin * ay;
                by = sin * ax + cos * ay;

                //float bx = MathF.Sin(angle) * radius + x;
                //float by = MathF.Cos(angle) * radius + y;

                this.DrawLine(ax + x, ay + y, bx + x, by + y, thickness, color);

                ax = bx;
                ay = by;
            }

        }
        public void DrawCircleSlow(float x, float y, float radius, int Points, float thickness, Color color)
        {
            const int MinPoints = 3;
            const int MaxPoints = 256;

            Points = Util.Clamp(Points, MinPoints, MaxPoints);

            float deltaAngle = MathHelper.TwoPi / (float)Points;
            float angle = 0f;



            for (int i = 0; i < Points; i++)
            {
                float ax = MathF.Sin(angle) * radius + x;
                float ay = MathF.Cos(angle) * radius + y;

                angle += deltaAngle;

                float bx = MathF.Sin(angle) * radius + x;
                float by = MathF.Cos(angle) * radius + y;

                this.DrawLine(ax, ay, bx, by, thickness, color);

            }

        }
        public void DrawPolygon(Vector2[] vertices, FlatTransform transform, float thickness, Color color)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 a = vertices[i];
                Vector2 b = vertices[(i + 1) % vertices.Length];

                a = Util.Transform(a, transform);
                b = Util.Transform(b, transform);

                this.DrawLine(a, b, thickness, color);
            }
        }
        public void DrawCircleFill(float x, float y, float radius, int Points, Color color)
        {
            this.EnsureStarted();
            const int MinPoints = 3;
            const int MaxPoints = 256;

            int shapeVertexCount = Util.Clamp(Points, MinPoints, MaxPoints);
            int shapeTriangleCount = shapeVertexCount - 2;
            int shapeIndexCount = shapeTriangleCount * 3;

            this.EnsureSpace(shapeVertexCount, shapeIndexCount);

            int index = 1;

            for (int i = 0; i < shapeTriangleCount; i++)
            {
                this.indices[this.indexCount++] = 0 + this.vertexCount; 
                this.indices[this.indexCount++] = index + this.vertexCount; 
                this.indices[this.indexCount++] = index + 1 + this.vertexCount; 
                
                index++;
            }

            float rotation = MathHelper.TwoPi / (float)Points;

            float sin = MathF.Sin(rotation);
            float cos = MathF.Cos(rotation);


            float ax = radius;
            float ay = 0f;

            for (int i = 0; i < shapeVertexCount; i++)
            {
                float x1 = ax;
                float y1 = ay;

                this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(x1 + x, y1 + y,0f), color);

                ax = cos * x1 - sin * y1;
                ay = sin * x1 + cos * y1;
            }

            this.shapesCount++;
        }
        public void DrawPolygonFill(Vector2[] vertices, int[] triangleIndices, FlatTransform transform, Color color)
        {

#if DEBUG
            if(vertices is null)
            {
                throw new ArgumentNullException("Vertices");
            }
            if(indices is null)
            {
                throw new ArgumentNullException("indices");
            }
            if (vertices.Length < 3)
            {
                throw new ArgumentOutOfRangeException("verticess");
            }
            if(indices.Length < 3)
            {
                throw new ArgumentOutOfRangeException("indicess");
            }
#endif

            this.EnsureStarted();
            this.EnsureSpace(vertices.Length, triangleIndices.Length);
            for (int i = 0; i < triangleIndices.Length; i++)
            {
                this.indices[this.indexCount++] = triangleIndices[i] + this.vertexCount;
            }
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 vertex = vertices[i];
                vertex = Util.Transform(vertex, transform);

                this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(vertex.X, vertex.Y, 0f), color);
            }

            this.shapesCount++;
        }

    }
        
}
