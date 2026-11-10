using OpenTK.Graphics.OpenGL;

namespace TanksGame.GameEntities
{
    public class Explosion
    {
        private float cx, cy;
        private float radius = 6f;
        public bool IsDead => radius > 80f;

        public Explosion(float x, float y) { cx = x; cy = y; }

        public void Update() => radius += 4.2f;

        public void Draw()
        {
            float alpha = 1f - (radius / 80f);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Color4(1f, 0.4f, 0.1f, alpha * 0.5f);
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Vertex2(cx, cy);
            for (int i = 0; i <= 360; i += 15)
            {
                float a = i * (float)Math.PI / 180f;
                float r = radius * (1f + (float)Math.Sin(i * 3) * 0.15f);
                GL.Vertex2(cx + (float)Math.Cos(a) * r, cy + (float)Math.Sin(a) * r);
            }
            GL.End();

            GL.Color4(1f, 0.7f, 0.2f, alpha * 0.7f);
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Vertex2(cx, cy);
            for (int i = 0; i <= 360; i += 15)
            {
                float a = i * (float)Math.PI / 180f;
                GL.Vertex2(cx + (float)Math.Cos(a) * radius * 0.7f, cy + (float)Math.Sin(a) * radius * 0.7f);
            }
            GL.End();

            GL.Color4(1f, 0.95f, 0.7f, alpha);
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Vertex2(cx, cy);
            for (int i = 0; i <= 360; i += 30)
            {
                float a = i * (float)Math.PI / 180f;
                GL.Vertex2(cx + (float)Math.Cos(a) * radius * 0.3f, cy + (float)Math.Sin(a) * radius * 0.3f);
            }
            GL.End();

            GL.Disable(EnableCap.Blend);
        }
    }
}
