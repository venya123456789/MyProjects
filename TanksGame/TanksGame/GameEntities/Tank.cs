using TanksGame.Managers;
using TanksGame.Windows;
using OpenTK.Graphics.OpenGL;

namespace TanksGame.GameEntities
{
    public class Tank
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        private float angle;
        private float currentSpeed;
        private Color mainColor;
        private Keys keyUp, keyDown, keyLeft, keyRight, keyFire, keySwitchWeapon;

        private float shootTimer;
        private const float SHOOT_COOLDOWN = 0.48f;
        private float machinegunTimer;
        private bool isMachinegunMode = false;
        private bool switchPressed = false;

        private int maxAmmo = 5;
        private int currentAmmo = 5;
        private int machinegunAmmo = 3;
        private float regularReloadTimer = 0;
        private float machinegunReloadTimer = 0;
        private bool regularIsReloading = false;
        private bool machinegunIsReloading = false;
        private const float RELOAD_TIME = 10f;
        private const float MACHINEGUN_RELOAD_TIME = 20f;
        private const int MACHINEGUN_AMMO = 3;

        private BuffType currentBuff = BuffType.None;
        private float buffTimer = 0;
        private const float BUFF_DURATION = 10f;

        private const float TANK_HALF_SIZE = 17f;
        private const int TILE_SIZE = 40;

        public int Hp { get; set; }
        public float BuffTimer => buffTimer;
        public float BuffDuration => BUFF_DURATION;

        public Tank(float x, float y, float startAngle, Color color,
                    Keys up, Keys down, Keys left, Keys right, Keys fire, Keys switchWeapon, int hp)
        {
            X = x; Y = y; angle = startAngle; mainColor = color;
            keyUp = up; keyDown = down; keyLeft = left; keyRight = right; keyFire = fire;
            keySwitchWeapon = switchWeapon;
            Hp = hp;
        }

        public void ApplyBuff(BuffType buff)
        {
            currentBuff = buff;
            buffTimer = BUFF_DURATION;
        }

        public void ClearBuff()
        {
            currentBuff = BuffType.None;
            buffTimer = 0;
        }

        public bool HasShield => currentBuff == BuffType.Shield;
        public bool HasSpeed => currentBuff == BuffType.Speed;
        public bool HasShrink => currentBuff == BuffType.Shrink;
        public BuffType CurrentBuff => currentBuff;

        public int CurrentAmmo => currentAmmo;
        public int MaxAmmo => maxAmmo;
        public bool IsReloading => regularIsReloading || machinegunIsReloading;
        public bool IsRegularReloading => regularIsReloading;
        public bool IsMachinegunReloading => machinegunIsReloading;
        public float ReloadTimer => isMachinegunMode ? machinegunReloadTimer : regularReloadTimer;
        public bool IsMachinegunMode => isMachinegunMode;
        public int MachinegunAmmo => machinegunAmmo;
        public int MaxMachinegunAmmo => MACHINEGUN_AMMO;
        public int RegularAmmo => currentAmmo;

        public void Update(bool[] keys, List<Wall> walls, List<Tile> tiles, Tank enemy, List<Explosion> explosions)
        {
            if (Hp <= 0) return;

            int keyUpIdx = (int)keyUp;
            int keyDownIdx = (int)keyDown;
            int keyLeftIdx = (int)keyLeft;
            int keyRightIdx = (int)keyRight;
            int keyFireIdx = (int)keyFire;
            int keySwitchIdx = (int)keySwitchWeapon;

            bool upPressed = keyUpIdx >= 0 && keyUpIdx < 256 && keys[keyUpIdx];
            bool downPressed = keyDownIdx >= 0 && keyDownIdx < 256 && keys[keyDownIdx];
            bool leftPressed = keyLeftIdx >= 0 && keyLeftIdx < 256 && keys[keyLeftIdx];
            bool rightPressed = keyRightIdx >= 0 && keyRightIdx < 256 && keys[keyRightIdx];
            bool firePressed = keyFireIdx >= 0 && keyFireIdx < 256 && keys[keyFireIdx];
            bool switchPressedCurrent = keySwitchIdx >= 0 && keySwitchIdx < 256 && keys[keySwitchIdx];

            if (leftPressed) angle -= 4.2f;
            if (rightPressed) angle += 4.2f;

            float speedMultiplier = HasSpeed ? 1.8f : 1f;

            Tile currentTile = GetTileAtPosition(X, Y, tiles);
            float slowFactor = GetTileSlowFactor(currentTile);

            float target = 0;
            if (upPressed) target = SettingsManager.TankSpeed * speedMultiplier * slowFactor;
            if (downPressed) target = -2.2f * speedMultiplier * slowFactor;

            currentSpeed += (target - currentSpeed) * 0.25f;

            if (Math.Abs(currentSpeed) > 0.1f)
            {
            }

            float rad = angle * (float)Math.PI / 180f;
            float nx = X + (float)Math.Sin(rad) * currentSpeed;
            float ny = Y - (float)Math.Cos(rad) * currentSpeed;

            if (!IsColliding(nx, ny, walls, tiles))
            {
                X = nx;
                Y = ny;
            }

            if (switchPressedCurrent && !switchPressed)
            {
                isMachinegunMode = !isMachinegunMode;
            }
            switchPressed = switchPressedCurrent;

            if (shootTimer > 0) shootTimer -= 0.016f;
            if (machinegunTimer > 0) machinegunTimer -= 0.016f;

            bool canShoot = currentTile == null || !currentTile.BlocksShooting;

            // Regular reload logic
            if (regularIsReloading)
            {
                regularReloadTimer -= 0.016f;
                if (regularReloadTimer <= 0)
                {
                    currentAmmo = maxAmmo;
                    regularIsReloading = false;
                }
            }

            // Machinegun reload logic
            if (machinegunIsReloading)
            {
                machinegunReloadTimer -= 0.016f;
                if (machinegunReloadTimer <= 0)
                {
                    machinegunAmmo = MACHINEGUN_AMMO;
                    machinegunIsReloading = false;
                }
            }

            if (isMachinegunMode)
            {
                if (firePressed && machinegunTimer <= 0 && canShoot && machinegunAmmo > 0)
                {
                    machinegunTimer = SettingsManager.MachinegunFireRate;
                    machinegunAmmo--;

                    float radShoot = angle * (float)Math.PI / 180f;
                    float bulletSpeed = SettingsManager.BulletSpeed * 1.3f;
                    float range = 800f;

                    bool hitWallNow = false;
                    bool hitEnemy = false;
                    float hitX = 0, hitY = 0;

                    for (float dist = 45; dist <= range; dist += 15)
                    {
                        float checkX = X + (float)Math.Sin(radShoot) * dist;
                        float checkY = Y - (float)Math.Cos(radShoot) * dist;

                        foreach (var wall in walls)
                        {
                            if (wall.Contains(checkX, checkY, 5))
                            {
                                hitWallNow = true;
                                hitX = checkX;
                                hitY = checkY;
                                break;
                            }
                        }
                        if (hitWallNow) break;

                        if (enemy != null)
                        {
                            float dx = checkX - enemy.X;
                            float dy = checkY - enemy.Y;
                            if (dx * dx + dy * dy < 900)
                            {
                                hitEnemy = true;
                                hitX = enemy.X;
                                hitY = enemy.Y;
                                break;
                            }
                        }
                    }

                    if (hitEnemy)
                    {
                        if (!enemy.HasShield)
                        {
                            explosions.Add(new Explosion(hitX, hitY));
                            SoundManager.PlayExplosion();
                            enemy.Hp--;
                        }
                    }
                    else if (hitWallNow)
                    {
                        explosions.Add(new Explosion(hitX, hitY));
                        SoundManager.PlayExplosion();
                    }

                    if (machinegunAmmo <= 0 && !machinegunIsReloading)
                    {
                        machinegunReloadTimer = MACHINEGUN_RELOAD_TIME;
                        machinegunIsReloading = true;
                    }
                }
            }
            else
            {
                if (firePressed && shootTimer <= 0 && canShoot && currentAmmo > 0)
                {
                    shootTimer = SHOOT_COOLDOWN;
                    currentAmmo--;
                    float radShoot = angle * (float)Math.PI / 180f;
                    float bx = X + (float)Math.Sin(radShoot) * 45;
                    float by = Y - (float)Math.Cos(radShoot) * 45;
                    GameForm.bullets.Add(new Bullet(bx, by, angle, this, false));
                    SoundManager.PlayShoot();
                    if (currentAmmo <= 0 && !regularIsReloading)
                    {
                        regularReloadTimer = RELOAD_TIME;
                        regularIsReloading = true;
                    }
                }
            }
        }

        private static Tile GetTileAtPosition(float x, float y, List<Tile> tiles)
        {
            Tile bestTile = null;
            float bestDist = 1000f;
            foreach (var tile in tiles)
            {
                float cx = tile.X + tile.Width / 2;
                float cy = tile.Y + tile.Height / 2;
                float dx = x - cx;
                float dy = y - cy;
                float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestTile = tile;
                }
            }
            return bestTile;
        }

        private bool IsColliding(float tx, float ty, List<Wall> walls, List<Tile> tiles)
        {
            foreach (var w in walls)
                if (w.Contains(tx, ty, TANK_HALF_SIZE * 1.4f))
                    return true;

            Tile tile = GetTileAtPosition(tx, ty, tiles);
            if (tile != null && tile.IsImpassable)
                return true;

            return false;
        }

        public bool IsHitBy(Bullet bullet)
        {
            float dx = bullet.X - X;
            float dy = bullet.Y - Y;
            return dx * dx + dy * dy < TANK_HALF_SIZE * TANK_HALF_SIZE * 2.8f;
        }

        public void Draw()
        {
            GL.PushMatrix();
            GL.Translate(X, Y, 0);
            GL.Rotate(angle, 0, 0, 1);

            float scale = HasShrink ? 0.7f : 1f;
            GL.Scale(scale, scale, 1f);

            // Гусеницы (левая и правая)
            GL.Color3(0.15f, 0.15f, 0.15f);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(-22, -20);
            GL.Vertex2(-14, -20);
            GL.Vertex2(-14, 20);
            GL.Vertex2(-22, 20);
            GL.End();
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(14, -20);
            GL.Vertex2(22, -20);
            GL.Vertex2(22, 20);
            GL.Vertex2(14, 20);
            GL.End();

            // Гусеницы - текстура (полоски)
            GL.Color3(0.25f, 0.25f, 0.25f);
            for (int i = -15; i <= 15; i += 6)
            {
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(-21, i);
                GL.Vertex2(-15, i);
                GL.Vertex2(-15, i + 3);
                GL.Vertex2(-21, i + 3);
                GL.End();
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(15, i);
                GL.Vertex2(21, i);
                GL.Vertex2(21, i + 3);
                GL.Vertex2(15, i + 3);
                GL.End();
            }

            // Основной корпус
            float r = mainColor.R / 255f;
            float g = mainColor.G / 255f;
            float b = mainColor.B / 255f;
            GL.Color3(r * 0.8f, g * 0.8f, b * 0.8f);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(-18, -16);
            GL.Vertex2(18, -16);
            GL.Vertex2(18, 16);
            GL.Vertex2(-18, 16);
            GL.End();

            // Верхняя часть корпуса (светлее)
            GL.Color3(r, g, b);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(-14, -12);
            GL.Vertex2(14, -12);
            GL.Vertex2(14, 12);
            GL.Vertex2(-14, 12);
            GL.End();

            // Башня (круглая)
            GL.Color3(r * 0.9f, g * 0.9f, b * 0.9f);
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Vertex2(0, 0);
            for (int i = 0; i <= 360; i += 20)
            {
                float a = i * (float)Math.PI / 180f;
                GL.Vertex2((float)Math.Cos(a) * 11, (float)Math.Sin(a) * 11);
            }
            GL.End();

            // Башня - люк
            GL.Color3(r * 0.7f, g * 0.7f, b * 0.7f);
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Vertex2(0, 0);
            for (int i = 0; i <= 360; i += 30)
            {
                float a = i * (float)Math.PI / 180f;
                GL.Vertex2((float)Math.Cos(a) * 5, (float)Math.Sin(a) * 5);
            }
            GL.End();

            // Ствол пушки - обычный или пулемет
            if (isMachinegunMode)
            {
                // Пулемет - толстый короткий ствол
                GL.Color3(0.4f, 0.4f, 0.45f);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(-6, 2);
                GL.Vertex2(6, 2);
                GL.Vertex2(6, -25);
                GL.Vertex2(-6, -25);
                GL.End();

                // Ствол - дуло пулемета
                GL.Color3(0.3f, 0.3f, 0.35f);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(-7, -25);
                GL.Vertex2(7, -25);
                GL.Vertex2(7, -30);
                GL.Vertex2(-7, -30);
                GL.End();

                // Кожух пулемета
                GL.Color3(0.35f, 0.35f, 0.4f);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(-4, -15);
                GL.Vertex2(-8, -15);
                GL.Vertex2(-8, -23);
                GL.Vertex2(-4, -23);
                GL.End();
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(4, -15);
                GL.Vertex2(8, -15);
                GL.Vertex2(8, -23);
                GL.Vertex2(4, -23);
                GL.End();
            }
            else
            {
                // Обычная пушка - тонкий длинный ствол
                GL.Color3(0.3f, 0.3f, 0.35f);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(-4, 2);
                GL.Vertex2(4, 2);
                GL.Vertex2(4, -38);
                GL.Vertex2(-4, -38);
                GL.End();

                // Ствол - дуло
                GL.Color3(0.2f, 0.2f, 0.25f);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(-5, -38);
                GL.Vertex2(5, -38);
                GL.Vertex2(5, -42);
                GL.Vertex2(-5, -42);
                GL.End();
            }

            // Щит
            if (HasShield)
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                GL.Color4(0.3f, 0.8f, 1f, 0.5f);
                GL.Begin(PrimitiveType.TriangleFan);
                GL.Vertex2(0, 0);
                for (int i = 0; i <= 360; i += 15)
                {
                    float a = i * (float)Math.PI / 180f;
                    GL.Vertex2((float)Math.Cos(a) * 30, (float)Math.Sin(a) * 30);
                }
                GL.End();
                GL.Disable(EnableCap.Blend);
            }

            GL.PopMatrix();
        }
        private static float GetTileSlowFactor(Tile tile)
        {
                if (tile != null && tile.IsSlow) return 0.4f;
                return 1f;
        }
    }
}