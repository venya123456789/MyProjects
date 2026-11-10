using OpenTK.Graphics.OpenGL;
using TanksGame.Managers;

namespace TanksGame.GameEntities
{
    public class Bullet
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        private float angle;
        private float speed;
        private float distance;
        private float maxDistance;
        public bool IsDead { get; private set; }
        public object Owner { get; }
        public bool IsMachinegun { get; }

        public Bullet(float x, float y, float ang, object owner, bool isMachinegun)
        {
            X = x; Y = y; angle = ang; Owner = owner; IsMachinegun = isMachinegun;
            speed = isMachinegun ? SettingsManager.BulletSpeed * 1.3f : SettingsManager.BulletSpeed;
            maxDistance = isMachinegun ? 800f : 1000f;
            distance = 0;
            IsDead = false;
        }

        public void Update()
        {
            float rad = angle * (float)Math.PI / 180f;
            float moveX = (float)Math.Sin(rad) * speed;
            float moveY = -(float)Math.Cos(rad) * speed;
            X += moveX;
            Y += moveY;
            distance += speed;
            if (distance >= maxDistance) IsDead = true;
        }

        public bool CheckBounds(float minX, float minY, float maxX, float maxY)
        {
            return X < minX || X > maxX || Y < minY || Y > maxY;
        }

        public void Draw()
        {
            GL.Color3(IsMachinegun ? 1f : 1f, IsMachinegun ? 0.3f : 0.9f, IsMachinegun ? 0.3f : 0.2f);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Color4(IsMachinegun ? 1f : 1f, IsMachinegun ? 0.3f : 0.9f, IsMachinegun ? 0.3f : 0.2f, 0.9f);

            float size = IsMachinegun ? 4f : 6f;
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Vertex2(X, Y);
            for (int i = 0; i <= 360; i += 30)
            {
                float a = i * (float)Math.PI / 180f;
                GL.Vertex2(X + (float)Math.Cos(a) * size, Y + (float)Math.Sin(a) * size);
            }
            GL.End();

            GL.Disable(EnableCap.Blend);
        }
    }
}
