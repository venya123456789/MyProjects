using TanksGame.GameEntities;

namespace TanksGame.CardFactory
{
    public class FortressMapBuilder : IMapBuilder
    {
        private const int MAP_WIDTH = 1280;
        private const int MAP_HEIGHT = 720;
        private const int TILE_SIZE = 40;

        public List<Wall> CreateWalls()
        {
            List<Wall> walls = new List<Wall>
            {
                new Wall(0, 0, MAP_WIDTH, 15),
                new Wall(0, MAP_HEIGHT - 15, MAP_WIDTH, 15),
                new Wall(0, 0, 15, MAP_HEIGHT),
                new Wall(MAP_WIDTH - 15, 0, 15, MAP_HEIGHT),
                new Wall(520, 250, 240, 30),
                new Wall(520, 440, 240, 30),
                new Wall(520, 250, 30, 100),
                new Wall(730, 250, 30, 100),
                new Wall(520, 370, 30, 100),
                new Wall(730, 370, 30, 100),
                new Wall(180, 80, 150, 25),
                new Wall(180, 80, 25, 80),
                new Wall(MAP_WIDTH - 330, 80, 150, 25),
                new Wall(MAP_WIDTH - 205, 80, 25, 80),
                new Wall(180, MAP_HEIGHT - 105, 150, 25),
                new Wall(180, MAP_HEIGHT - 160, 25, 80),
                new Wall(MAP_WIDTH - 330, MAP_HEIGHT - 105, 150, 25),
                new Wall(MAP_WIDTH - 205, MAP_HEIGHT - 160, 25, 80),
                new Wall(80, 260, 25, 80),
                new Wall(80, 380, 25, 80),
                new Wall(140, 320, 60, 25),
                new Wall(MAP_WIDTH - 105, 260, 25, 80),
                new Wall(MAP_WIDTH - 105, 380, 25, 80),
                new Wall(MAP_WIDTH - 200, 320, 60, 25),
                new Wall(350, 150, 25, 60),
                new Wall(350, 510, 25, 60),
                new Wall(MAP_WIDTH - 375, 150, 25, 60),
                new Wall(MAP_WIDTH - 375, 510, 25, 60),
                new Wall(450, 180, 25, 40),
                new Wall(805, 180, 25, 40),
                new Wall(450, 500, 25, 40),
                new Wall(805, 500, 25, 40)
            };
            return walls;
        }

        public List<Tile> CreateTiles(List<Wall> walls)
        {
            List<Tile> tiles = new List<Tile>();

            for (int x = 15; x < MAP_WIDTH - 15; x += TILE_SIZE)
            {
                for (int y = 15; y < MAP_HEIGHT - 15; y += TILE_SIZE)
                {
                    if (!IsWallAt(x, y, walls))
                        tiles.Add(new Tile(x, y, TileType.Normal));
                }
            }

            CreateFortressSlowZones(tiles, walls);
            return tiles;
        }

        private void CreateFortressSlowZones(List<Tile> tiles, List<Wall> walls)
        {
            for (int x = 40; x < 120; x += TILE_SIZE)
            {
                for (int y = 160; y < 240; y += TILE_SIZE)
                {
                    SetTileAt(tiles, x, y, TileType.Slow);
                }
            }
            for (int x = MAP_WIDTH - 120; x < MAP_WIDTH - 40; x += TILE_SIZE)
            {
                for (int y = 160; y < 240; y += TILE_SIZE)
                {
                    SetTileAt(tiles, x, y, TileType.Slow);
                }
            }

            for (int x = 140; x < 350; x += TILE_SIZE)
            {
                for (int y = 140; y < 280; y += TILE_SIZE)
                {
                    SetTileAt(tiles, x, y, TileType.Slow);
                }
            }
            for (int x = MAP_WIDTH - 350; x < MAP_WIDTH - 140; x += TILE_SIZE)
            {
                for (int y = 140; y < 280; y += TILE_SIZE)
                {
                    SetTileAt(tiles, x, y, TileType.Slow);
                }
            }

            for (int x = 400; x < 550; x += TILE_SIZE)
            {
                for (int y = 350; y < 500; y += TILE_SIZE)
                {
                    SetTileAt(tiles, x, y, TileType.Slow);
                }
            }
        }

        private bool IsWallAt(int x, int y, List<Wall> walls)
        {
            foreach (var wall in walls)
            {
                if (wall.Contains(x + TILE_SIZE / 2, y + TILE_SIZE / 2, TILE_SIZE / 2))
                    return true;
            }
            return false;
        }

        private void SetTileAt(List<Tile> tiles, int x, int y, TileType type)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].X == x && tiles[i].Y == y)
                {
                    tiles[i] = new Tile(x, y, type);
                    return;
                }
            }
            tiles.Add(new Tile(x, y, type));
        }
    }
}
