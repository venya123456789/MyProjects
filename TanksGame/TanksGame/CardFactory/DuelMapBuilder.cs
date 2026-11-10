using TanksGame.GameEntities;

namespace TanksGame.CardFactory
{
    public class DuelMapBuilder : IMapBuilder
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
                new Wall(580, 300, 120, 25),
                new Wall(580, 395, 120, 25),
                new Wall(580, 300, 25, 50),
                new Wall(675, 300, 25, 50),
                new Wall(580, 370, 25, 50),
                new Wall(675, 370, 25, 50),
                new Wall(300, 160, 25, 80),
                new Wall(400, 160, 25, 80),
                new Wall(500, 160, 25, 80),
                new Wall(755, 160, 25, 80),
                new Wall(855, 160, 25, 80),
                new Wall(955, 160, 25, 80),
                new Wall(300, MAP_HEIGHT - 240, 25, 80),
                new Wall(400, MAP_HEIGHT - 240, 25, 80),
                new Wall(500, MAP_HEIGHT - 240, 25, 80),
                new Wall(755, MAP_HEIGHT - 240, 25, 80),
                new Wall(855, MAP_HEIGHT - 240, 25, 80),
                new Wall(955, MAP_HEIGHT - 240, 25, 80),
                new Wall(80, 240, 25, 60),
                new Wall(80, 340, 25, 60),
                new Wall(150, 280, 60, 25),
                new Wall(80, MAP_HEIGHT - 300, 25, 60),
                new Wall(80, MAP_HEIGHT - 400, 25, 60),
                new Wall(150, MAP_HEIGHT - 305, 60, 25),
                new Wall(MAP_WIDTH - 105, 240, 25, 60),
                new Wall(MAP_WIDTH - 105, 340, 25, 60),
                new Wall(MAP_WIDTH - 210, 280, 60, 25),
                new Wall(MAP_WIDTH - 105, MAP_HEIGHT - 300, 25, 60),
                new Wall(MAP_WIDTH - 105, MAP_HEIGHT - 400, 25, 60),
                new Wall(MAP_WIDTH - 210, MAP_HEIGHT - 305, 60, 25),
                new Wall(420, 280, 25, 50),
                new Wall(835, 280, 25, 50),
                new Wall(420, 510, 25, 50),
                new Wall(835, 510, 25, 50)
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

            CreateDuelSlowZones(tiles, walls);
            return tiles;
        }

        private void CreateDuelSlowZones(List<Tile> tiles, List<Wall> walls)
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

            for (int x = 120; x < 260; x += TILE_SIZE)
            {
                for (int y = 280; y < 520; y += TILE_SIZE)
                {
                    SetTileAt(tiles, x, y, TileType.Slow);
                }
            }
            for (int x = MAP_WIDTH - 260; x < MAP_WIDTH - 120; x += TILE_SIZE)
            {
                for (int y = 280; y < 520; y += TILE_SIZE)
                {
                    SetTileAt(tiles, x, y, TileType.Slow);
                }
            }

            for (int x = 380; x < 700; x += TILE_SIZE)
            {
                for (int y = 120; y < 220; y += TILE_SIZE)
                {
                    SetTileAt(tiles, x, y, TileType.Slow);
                }
            }
            for (int x = 380; x < 700; x += TILE_SIZE)
            {
                for (int y = MAP_HEIGHT - 220; y < MAP_HEIGHT - 120; y += TILE_SIZE)
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
