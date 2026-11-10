using TanksGame.GameEntities;

namespace TanksGame.CardFactory
{
    public interface IMapBuilder
    {
        List<Wall> CreateWalls();
        List<Tile> CreateTiles(List<Wall> walls);
    }
}
