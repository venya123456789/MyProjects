using TanksGame.GameEntities;

namespace TanksGame.CardFactory
{
    public class ArenaMapBuilder : IMapBuilder
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
                new Wall(60, 60, 120, 25),
                new Wall(60, 60, 25, 100),
                new Wall(MAP_WIDTH - 180, 60, 120, 25),
                new Wall(MAP_WIDTH - 85, 60, 25, 100),
                new Wall(60, MAP_HEIGHT - 85, 120, 25),
                new Wall(60, MAP_HEIGHT - 160, 25, 100),
                new Wall(MAP_WIDTH - 180, MAP_HEIGHT - 85, 120, 25),
                new Wall(MAP_WIDTH - 85, MAP_HEIGHT - 160, 25, 100),
                new Wall(540, 280, 80, 80),
                new Wall(380, 340, 60, 25),
                new Wall(840, 340, 60, 25),
                new Wall(280, 200, 25, 80),
                new Wall(280, 420, 25, 80),
                new Wall(MAP_WIDTH - 305, 200, 25, 80),
                new Wall(MAP_WIDTH - 305, 420, 25, 80)
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

            CreateArenaSlowZones(tiles, walls);
            return tiles;
        }

        private void CreateArenaSlowZones(List<Tile> tiles, List<Wall> walls)
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

            for (int x = 160; x < 280; x += TILE_SIZE)
            {
                for (int y = 100; y < 200; y += TILE_SIZE)
                {
                    SetTileAt(tiles, x, y, TileType.Slow);
                }
            }
            for (int x = MAP_WIDTH - 280; x < MAP_WIDTH - 160; x += TILE_SIZE)
            {
                for (int y = 100; y < 200; y += TILE_SIZE)
                {
                    SetTileAt(tiles, x, y, TileType.Slow);
                }
            }
            for (int x = 160; x < 280; x += TILE_SIZE)
            {
                for (int y = MAP_HEIGHT - 200; y < MAP_HEIGHT - 100; y += TILE_SIZE)
                {
                    SetTileAt(tiles, x, y, TileType.Slow);
                }
            }
            for (int x = MAP_WIDTH - 280; x < MAP_WIDTH - 160; x += TILE_SIZE)
            {
                for (int y = MAP_HEIGHT - 200; y < MAP_HEIGHT - 100; y += TILE_SIZE)
                {
                    SetTileAt(tiles, x, y, TileType.Slow);
                }
            }

            for (int x = 300; x < 450; x += TILE_SIZE)
            {
                for (int y = 150; y < 300; y += TILE_SIZE)
                {
                    SetTileAt(tiles, x, y, TileType.Slow);
                }
            }
            for (int x = MAP_WIDTH - 450; x < MAP_WIDTH - 300; x += TILE_SIZE)
            {
                for (int y = 150; y < 300; y += TILE_SIZE)
                {
                    SetTileAt(tiles, x, y, TileType.Slow);
                }
            }
            for (int x = 300; x < 450; x += TILE_SIZE)
            {
                for (int y = MAP_HEIGHT - 300; y < MAP_HEIGHT - 150; y += TILE_SIZE)
                {
                    SetTileAt(tiles, x, y, TileType.Slow);
                }
            }
            for (int x = MAP_WIDTH - 450; x < MAP_WIDTH - 300; x += TILE_SIZE)
            {
                for (int y = MAP_HEIGHT - 300; y < MAP_HEIGHT - 150; y += TILE_SIZE)
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