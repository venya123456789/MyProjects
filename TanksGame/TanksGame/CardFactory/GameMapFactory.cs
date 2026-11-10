namespace TanksGame.CardFactory
{
    public class GameMapFactory : MapFactoryBase
    {
        public override IMapBuilder GetMapBuilder(int mapType)
        {
            switch (mapType)
            {
                case 1:
                    return new ArenaMapBuilder();
                case 2:
                    return new FortressMapBuilder();
                case 3:
                    return new DuelMapBuilder();
                default:
                    return new ArenaMapBuilder();
            }
        }
    }
}
