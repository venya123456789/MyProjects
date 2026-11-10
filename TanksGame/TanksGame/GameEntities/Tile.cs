using OpenTK.Graphics.OpenGL;

namespace TanksGame.GameEntities
{
    public class Tile
    {
        public float X { get; }
        public float Y { get; }
        public float Width { get; }
        public float Height { get; }
        public TileType Type { get; }

        public const int TILE_SIZE = 40;

        public Tile(float x, float y, TileType type)
        {
            X = x;
            Y = y;
            Width = TILE_SIZE;
            Height = TILE_SIZE;
            Type = type;
        }

        public bool Contains(float px, float py)
        {
            return px >= X && px <= X + Width && py >= Y && py <= Y + Height;
        }

        public bool IsImpassable => Type == TileType.Impassable || Type == TileType.ImpassableNoShoot || Type == TileType.Water;
        public bool BlocksShooting => Type == TileType.NoShoot || Type == TileType.ImpassableNoShoot;
        public bool IsSlow => Type == TileType.Slow;

        public void Draw()
        {
            float alpha = 0.25f;
            switch (Type)
            {
                case TileType.Normal:
                    alpha = 0.15f;
                    GL.Color4(0.1f, 0.12f, 0.16f, alpha);
                    break;
                case TileType.Slow:
                    alpha = 0.35f;
                    GL.Color4(0.45f, 0.3f, 0.12f, alpha);
                    break;
                case TileType.NoShoot:
                    alpha = 0.35f;
                    GL.Color4(0.45f, 0.3f, 0.12f, alpha);
                    break;
                case TileType.Impassable:
                    alpha = 0.35f;
                    GL.Color4(0.45f, 0.3f, 0.12f, alpha);
                    break;
                case TileType.ImpassableNoShoot:
                    alpha = 0.35f;
                    GL.Color4(0.45f, 0.3f, 0.12f, alpha);
                    break;
                case TileType.Water:
                    alpha = 0.5f;
                    GL.Color4(0.1f, 0.3f, 0.6f, alpha);
                    break;
            }

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(X, Y);
            GL.Vertex2(X + Width, Y);
            GL.Vertex2(X + Width, Y + Height);
            GL.Vertex2(X, Y + Height);
            GL.End();

            GL.Disable(EnableCap.Blend);
        }
    }
}
