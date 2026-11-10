using TanksGame.GameEntities;

namespace TanksGame.CardFactory
{
    public abstract class MapFactoryBase
    {
        protected const int MAP_WIDTH = 1280;
        protected const int MAP_HEIGHT = 720;
        protected const int TILE_SIZE = 40;

        public abstract IMapBuilder GetMapBuilder(int mapType);

        public (List<Wall> walls, List<Tile> tiles) CreateMap(int mapType)
        {
            IMapBuilder builder = GetMapBuilder(mapType);
            List<Wall> walls = builder.CreateWalls();
            List<Tile> tiles = builder.CreateTiles(walls);
            return (walls, tiles);
        }

        protected List<Wall> CreateBorders()
        {
            List<Wall> borders = new List<Wall>
            {
                new Wall(0, 0, MAP_WIDTH, 15),
                new Wall(0, MAP_HEIGHT - 15, MAP_WIDTH, 15),
                new Wall(0, 0, 15, MAP_HEIGHT),
                new Wall(MAP_WIDTH - 15, 0, 15, MAP_HEIGHT)
            };
            return borders;
        }

        protected List<Tile> CreateBaseTiles()
        {
            List<Tile> baseTiles = new List<Tile>();
            for (int x = 15; x < MAP_WIDTH - 15; x += TILE_SIZE)
            {
                for (int y = 15; y < MAP_HEIGHT - 15; y += TILE_SIZE)
                {
                    if (!IsWallAt(x, y, new List<Wall>()))
                        baseTiles.Add(new Tile(x, y, TileType.Normal));
                }
            }
            return baseTiles;
        }

        protected bool IsWallAt(int x, int y, List<Wall> walls)
        {
            foreach (var wall in walls)
            {
                if (wall.Contains(x + TILE_SIZE / 2, y + TILE_SIZE / 2, TILE_SIZE / 2))
                    return true;
            }
            return false;
        }

        protected void SetTileAt(List<Tile> tiles, int x, int y, TileType type)
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
