using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace FlatLibMonogame.Graphics
{
    public sealed class Camera
    {
        public readonly static float MinZ = 1f;
        public readonly static float MaxZ = 2048f;

        public readonly static int MinZoom = 1;
        public readonly static int MaxZoom = 20;
        

        private Vector2 position;
        private float z;
        private float Basez;

        private float aspectRatio;
        private float fieldOfView;

        private Matrix view;
        private Matrix proj;

        private int Zoom;

        public Vector2 Position
        {
            get { return this.position; }
        }
        public float Z
        {
            get { return this.z; }
        }
        public float BaseZ
        {
            get { return this.Basez; }
        }

        public Matrix Projection
        {
            get { return this.proj; }
        }
        public Matrix View
        {
            get { return this.view; }
        }
        public Camera(Screen screen)
        {
            if(screen is null)
            {
                throw new ArgumentNullException("Screen");
            }
            this.aspectRatio = (float)screen.Width / screen.Height;
            this.fieldOfView = MathHelper.PiOver2;

            this.position = new Vector2(0, 0);
            this.Basez = this.GetZFromHeight(screen.Height);
            this.z = this.Basez;

            this.UpdateMatrices();

            this.Zoom = 1;

        }

        public void UpdateMatrices()
        {
            this.view = Matrix.CreateLookAt(new Vector3(0, 0, this.z), Vector3.Zero, Vector3.Up);
            this.proj = Matrix.CreatePerspectiveFieldOfView(this.fieldOfView, this.aspectRatio, Camera.MinZ, Camera.MaxZ);
        }

        public float GetZFromHeight(float height)
        {
            return (0.5f * height) / MathF.Tan(0.5f*fieldOfView);
            
        }
        public float GetHeightFromZ()
        {
            return this.z * MathF.Tan(0.5f * this.fieldOfView) * 2f;
        }

        public void MoveZ(float amount)
        {
            this.z += amount;
            this.z = Util.Clamp(this.z, Camera.MinZ, Camera.MaxZ);
        }
        public void ResetZ()
        {
            this.z = this.BaseZ;
        }
        public void MoveZ(Vector2 amount)
        {
            this.position += amount;
        }
        public void MoveTo(Vector2 position)
        {
            this.position = position;
        }
        public void IncZoom()
        {
            this.Zoom++;
            this.Zoom = Util.Clamp(this.Zoom, Camera.MinZoom, Camera.MaxZoom);
            this.z = this.BaseZ / this.Zoom;
        }
        public void DecZoom()
        {
            this.Zoom--;
            this.Zoom = Util.Clamp(this.Zoom, Camera.MinZoom, Camera.MaxZoom);
            this.z = this.BaseZ / this.Zoom;
        }

        public void SetZoom(int amount)
        {
            this.Zoom = amount;
            this.Zoom = Util.Clamp(this.Zoom, Camera.MinZoom, Camera.MaxZoom);
            this.z = this.BaseZ / this.Zoom;
        }

        public void GetExtents(out float width, out float height)
        {
            height = this.GetHeightFromZ();
            width = height * this.aspectRatio;
        }
        public void GetExtents(out float left, out float right, out float bottom, out float top)
        {
            this.GetExtents(out float width, out float height);
            
            left = this.position.X - width * 0.5f;
            right = left + width;
            bottom = this.position.Y - height * 0.5f;
            top = bottom + height;
        }

        public void GetExtents(out Vector2 min, out Vector2 max)
        {
            this.GetExtents(out float left, out float right, out float bottom, out float top);
            min = new Vector2(left, bottom);
            max = new Vector2(right, top);
        }
    }
}
