using OpenTK.Graphics.OpenGL;

namespace TanksGame.GameEntities
{
    public class Wall
    {
        public float X { get; }
        public float Y { get; }
        public float Width { get; }
        public float Height { get; }

        public Wall(float x, float y, float w, float h)
        {
            X = x; Y = y; Width = w; Height = h;
        }

        public bool Contains(float px, float py, float padding = 0)
        {
            return px >= X - padding && px <= X + Width + padding &&
                   py >= Y - padding && py <= Y + Height + padding;
        }

        public void Draw()
        {
            GL.Color3(0.35f, 0.38f, 0.42f);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(X, Y);
            GL.Vertex2(X + Width, Y);
            GL.Vertex2(X + Width, Y + Height);
            GL.Vertex2(X, Y + Height);
            GL.End();

            GL.Color3(0.5f, 0.53f, 0.57f);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(X, Y);
            GL.Vertex2(X + Width, Y);
            GL.Vertex2(X + Width, Y + 4);
            GL.Vertex2(X, Y + 4);
            GL.End();

            GL.Color3(0.25f, 0.27f, 0.3f);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(X, Y);
            GL.Vertex2(X + 4, Y);
            GL.Vertex2(X + 4, Y + Height);
            GL.Vertex2(X, Y + Height);
            GL.End();

            GL.Color3(0.6f, 0.6f, 0.65f);
            float boltSize = 3f;
            float[] boltPositions = { 6f, Width - 6f };
            foreach (float bx in boltPositions)
            {
                foreach (float by in new float[] { 6f, Height - 6f })
                {
                    GL.Begin(PrimitiveType.TriangleFan);
                    GL.Vertex2(X + bx, Y + by);
                    for (int i = 0; i <= 360; i += 45)
                    {
                        float a = i * (float)Math.PI / 180f;
                        GL.Vertex2(X + bx + (float)Math.Cos(a) * boltSize,
                                   Y + by + (float)Math.Sin(a) * boltSize);
                    }
                    GL.End();
                }
            }

            GL.Color3(0.2f, 0.22f, 0.25f);
            GL.LineWidth(2f);
            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex2(X, Y);
            GL.Vertex2(X + Width, Y);
            GL.Vertex2(X + Width, Y + Height);
            GL.Vertex2(X, Y + Height);
            GL.End();
        }
    }
}
