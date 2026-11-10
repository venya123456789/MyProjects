namespace TanksGame.GameEntities.Decorators
{
    public class SlowEffectDecorator : TankDecorator
    {
        private float _slowDuration;
        private float _slowFactor;
        private float _elapsedTime;
        private bool _isActive;

        public SlowEffectDecorator(ITank tank, float duration, float slowFactor = 0.4f)
            : base(tank)
        {
            _slowDuration = duration;
            _slowFactor = slowFactor;
            _elapsedTime = 0;
            _isActive = true;
        }

        public bool IsSlowActive => _isActive && _elapsedTime < _slowDuration;
        public float SlowRemainingTime => Math.Max(0, _slowDuration - _elapsedTime);

        public void Deactivate()
        {
            _isActive = false;
        }

        public override void Update(bool[] keys, List<Wall> walls, List<Tile> tiles, ITank enemy)
        {
            if (_isActive)
            {
                _elapsedTime += 0.016f;
                if (_elapsedTime >= _slowDuration)
                {
                    _isActive = false;
                }
            }
            base.Update(keys, walls, tiles, enemy);
        }

        public float GetModifiedSpeed(float baseSpeed)
        {
            if (IsSlowActive)
            {
                return baseSpeed * _slowFactor;
            }
            return baseSpeed;
        }

        public float GetSlowFactor()
        {
            return IsSlowActive ? _slowFactor : 1f;
        }
    }
}
