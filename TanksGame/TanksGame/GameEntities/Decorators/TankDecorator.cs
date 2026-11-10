using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TanksGame.GameEntities.Decorators;

namespace TanksGame.GameEntities.Decorators
{
    public abstract class TankDecorator : ITank
    {
        protected ITank _tank;

        protected TankDecorator(ITank tank)
        {
            _tank = tank;
        }

        public float X => _tank.X;
        public float Y => _tank.Y;
        public float Angle => _tank.Angle;
        public float CurrentSpeed => _tank.CurrentSpeed;
        public int Hp { get => _tank.Hp; set => _tank.Hp = value; }
        public bool HasShield => _tank.HasShield;
        public bool HasSpeed => _tank.HasSpeed;
        public bool HasShrink => _tank.HasShrink;
        public BuffType CurrentBuff => _tank.CurrentBuff;

        public virtual void Update(bool[] keys, List<Wall> walls, List<Tile> tiles, ITank enemy)
        {
            _tank.Update(keys, walls, tiles, enemy);
        }

        public virtual bool IsHitBy(Bullet bullet)
        {
            return _tank.IsHitBy(bullet);
        }

        public virtual void Draw()
        {
            _tank.Draw();
        }
    }
}
