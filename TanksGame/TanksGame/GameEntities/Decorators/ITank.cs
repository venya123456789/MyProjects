using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TanksGame.GameEntities.Decorators
{
    public interface ITank
    {
        float X { get; }
        float Y { get; }
        float Angle { get; }
        float CurrentSpeed { get; }
        int Hp { get; set; }
        bool HasShield { get; }
        bool HasSpeed { get; }
        bool HasShrink { get; }
        BuffType CurrentBuff { get; }

        void Update(bool[] keys, List<Wall> walls, List<Tile> tiles, ITank enemy);
        bool IsHitBy(Bullet bullet);
        void Draw();
    }
}
