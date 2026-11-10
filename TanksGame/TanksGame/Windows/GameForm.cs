using OpenTK;
using OpenTK.Graphics.OpenGL;
using TanksGame.Managers;
using TanksGame.GameEntities;
using TanksGame.CardFactory;
using TanksGame.GameEntities.Decorators;

namespace TanksGame.Windows
{
    public partial class GameForm : Form
    {
        private GLControl glControl;
        private System.Windows.Forms.Timer gameTimer;  
        internal static List<Bullet> bullets = new List<Bullet>();
        internal static List<Explosion> explosions = new List<Explosion>();
        internal static bool gameOverTriggered = false;

        private Tank player1;
        private Tank player2;
        private SlowEffectDecorator player1SlowDecorator;
        private SlowEffectDecorator player2SlowDecorator;
        private List<Wall> walls = new List<Wall>();
        private List<Tile> tiles = new List<Tile>();
        private GameMapFactory mapFactory = new GameMapFactory();
        
        private bool[] keysPressed = new bool[256];
        private float countdownTimer = 30f;
        private float buffDuration = 10f;
        
        private const int MAP_WIDTH = 1280;
        private const int MAP_HEIGHT = 720;
        

        public GameForm()
        {
            Text = "RICHOCHET TANKS";
            Size = new Size(MAP_WIDTH + 20, MAP_HEIGHT + 40);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.None;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            SettingsManager.LoadSettings();
            SoundManager.Initialize();
            glControl = new GLControl();
            glControl.Dock = DockStyle.Fill;
            glControl.BackColor = Color.Black;
            Controls.Add(glControl);

            glControl.CreateControl();
            KeyPreview = true;

            this.KeyDown += (s, e) => 
            { 
                int keyIndex = (int)e.KeyCode;
                if (keyIndex >= 0 && keyIndex < 256) keysPressed[keyIndex] = true;
                if (e.KeyCode == Keys.Escape)
                {
                    gameTimer.Stop();
                    this.Close();
                }
            };
            this.KeyUp += (s, e) => { 
                int keyIndex = (int)e.KeyCode;
                if (keyIndex >= 0 && keyIndex < 256) keysPressed[keyIndex] = false; 
            };

            glControl.Load += GlControl_Load;
            glControl.Resize += (s, e) => SetupViewport();
            glControl.Paint += GlControl_Paint;

            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 16;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            CreateMap();
            ResetGame();

            glControl.Focus();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                if (gameTimer != null)
                {
                    gameTimer.Stop();
                    gameTimer.Enabled = false;
                }
            }
            catch { }
            
            try
            {
                SoundManager.StopMusic();
            }
            catch { }
            
            try
            {
                if (glControl != null)
                {
                    glControl.Dispose();
                }
            }
            catch { }
            
            base.OnFormClosing(e);
        }

        public void RestartGame()
        {
            SettingsManager.LoadSettings();
            CreateMap();
            ResetGame();
            gameTimer.Start();
        }

        private void ShowWinner(string winnerName, Color winnerColor)
        {
            gameTimer.Stop();
            
            var winnerForm = new WinnerForm(winnerName, winnerColor);
            winnerForm.Owner = this;
            
            winnerForm.OnRestart += () => {
                winnerForm.Close();
                RestartGame();
            };
            
            winnerForm.OnHome += () => {
                winnerForm.Close();
                this.Close();
            };
            
            winnerForm.ShowDialog();
        }

        private void GlControl_Load(object sender, EventArgs e)
        {
            GL.ClearColor(0.06f, 0.08f, 0.10f, 1f);
            SetupViewport();
        }

        private void SetupViewport()
        {
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, MAP_WIDTH, MAP_HEIGHT, 0, -1, 1);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        private void CreateMap()
        {
            var (factoryWalls, factoryTiles) = mapFactory.CreateMap(SettingsManager.SelectedMap);
            walls = factoryWalls;
            tiles = factoryTiles;
        }

        public Tile GetTileAtPosition(float x, float y)
        {
            foreach (var tile in tiles)
            {
                if (tile.Contains(x, y))
                    return tile;
            }
            return null;
        }

        private void ResetGame()
        {
            player1 = new Tank(80, 200, 45f, Color.FromArgb(220, 60, 60),
                SettingsManager.P1_Up, SettingsManager.P1_Down, SettingsManager.P1_Left, SettingsManager.P1_Right, SettingsManager.P1_Fire, SettingsManager.P1_SwitchWeapon, SettingsManager.PlayerHp);

            player2 = new Tank(MAP_WIDTH - 80, 200, 225f, Color.FromArgb(70, 130, 220),
                SettingsManager.P2_Up, SettingsManager.P2_Down, SettingsManager.P2_Left, SettingsManager.P2_Right, SettingsManager.P2_Fire, SettingsManager.P2_SwitchWeapon, SettingsManager.PlayerHp);

            bullets.Clear();
            explosions.Clear();
            
            countdownTimer = 30f;
            buffDuration = 0f;
            
            for (int i = 0; i < keysPressed.Length; i++)
                keysPressed[i] = false;
            
            gameOverTriggered = false;
        }
        
        private void ApplyRandomBuff(Tank tank)
        {
            if (tank == null) return;
            
            var random = new Random();
            int buffIndex = random.Next(0, 4);
            
            switch (buffIndex)
            {
                case 0:
                    tank.ApplyBuff(BuffType.Shield);
                    break;
                case 1:
                    tank.ApplyBuff(BuffType.Speed);
                    break;
                case 2:
                    tank.ApplyBuff(BuffType.Shrink);
                    break;
                default:
                    tank.ApplyBuff(BuffType.None);
                    break;
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (buffDuration > 0)
            {
                buffDuration -= 0.016f;
                if (buffDuration <= 0)
                {
                    buffDuration = 0f;
                    player1?.ClearBuff();
                    player2?.ClearBuff();
                    countdownTimer = 30f;
                }
            }
            else
            {
                countdownTimer -= 0.016f;
                if (countdownTimer <= 0)
                {
                    buffDuration = 10f;
                    countdownTimer = 30f;
                    ApplyRandomBuff(player1);
                    ApplyRandomBuff(player2);
                }
            }
            
            player1?.Update(keysPressed, walls, tiles, player2, explosions);
            player2?.Update(keysPressed, walls, tiles, player1, explosions);

            if (player1.Hp <= 0 && !gameOverTriggered)
            {
                gameOverTriggered = true;
                ShowWinner("СИНИЙ", Color.FromArgb(70, 130, 220));
            }
            else if (player2.Hp <= 0 && !gameOverTriggered)
            {
                gameOverTriggered = true;
                ShowWinner("КРАСНЫЙ", Color.FromArgb(220, 60, 60));
            }

            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                if (i >= bullets.Count) break;
                
                Bullet b = bullets[i];
                b.Update();

                if (b.CheckBounds(0, 0, MAP_WIDTH, MAP_HEIGHT))
                {
                    bullets.RemoveAt(i);
                    continue;
                }

                bool hitWall = false;
                foreach (Wall wall in walls)
                {
                    if (wall.Contains(b.X, b.Y, 5))
                    {
                        bullets.RemoveAt(i);
                        if (!b.IsMachinegun)
                        {
                            explosions.Add(new Explosion(b.X, b.Y));
                            SoundManager.PlayExplosion();
                        }
                        hitWall = true;
                        break;
                    }
                }

                if (hitWall || i >= bullets.Count) continue;

                if (b.Owner != player1 && player1?.IsHitBy(b) == true)
                {
                    if (player1.HasShield)
                    {
                        bullets.RemoveAt(i);
                        continue;
                    }
                    explosions.Add(new Explosion(player1.X, player1.Y));
                    SoundManager.PlayExplosion();
                    bullets.RemoveAt(i);
                    player1.Hp--;
                    continue;
                }

                if (b.Owner != player2 && player2?.IsHitBy(b) == true)
                {
                    if (player2.HasShield)
                    {
                        bullets.RemoveAt(i);
                        continue;
                    }
                    explosions.Add(new Explosion(player2.X, player2.Y));
                    SoundManager.PlayExplosion();
                    bullets.RemoveAt(i);
                    player2.Hp--;
                    continue;
                }

                if (b.IsDead) bullets.RemoveAt(i);
            }

            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update();
                if (explosions[i].IsDead) explosions.RemoveAt(i);
            }

            if (player1.Hp <= 0)
            {
                gameOverTriggered = true;
                ShowWinner("СИНИЙ", Color.FromArgb(70, 130, 220));
            }
            else if (player2.Hp <= 0)
            {
                gameOverTriggered = true;
                ShowWinner("КРАСНЫЙ", Color.FromArgb(220, 60, 60));
            }

            glControl.Invalidate();
        }

        private void GlControl_Paint(object sender, PaintEventArgs e)
        {
            if (!glControl.Context.IsCurrent) glControl.MakeCurrent();

            GL.Clear(ClearBufferMask.ColorBufferBit);

            DrawBackground();

            foreach (var tile in tiles) tile.Draw();
            foreach (var wall in walls) wall.Draw();
            foreach (var exp in explosions) exp.Draw();
            foreach (var bullet in bullets) bullet.Draw();

            player1?.Draw();
            player2?.Draw();

            DrawSimpleHUD();

            glControl.SwapBuffers();
        }

        private void DrawBackground()
        {
            // Градиентный фон
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(0.06f, 0.08f, 0.12f);
            GL.Vertex2(0, 0);
            GL.Vertex2(MAP_WIDTH, 0);
            GL.Color3(0.04f, 0.05f, 0.08f);
            GL.Vertex2(MAP_WIDTH, MAP_HEIGHT);
            GL.Vertex2(0, MAP_HEIGHT);
            GL.End();

            // Сетка
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Color4(0.2f, 0.25f, 0.3f, 0.3f);
            GL.Begin(PrimitiveType.Lines);

            for (int x = 0; x <= MAP_WIDTH; x += 40)
            {
                GL.Vertex2(x, 0);
                GL.Vertex2(x, MAP_HEIGHT);
            }
            for (int y = 0; y <= MAP_HEIGHT; y += 40)
            {
                GL.Vertex2(0, y);
                GL.Vertex2(MAP_WIDTH, y);
            }
            GL.End();
            GL.Disable(EnableCap.Blend);
        }

        private void DrawSimpleHUD()
        {
            // Красный HP
            GL.Color3(0.9f, 0.2f, 0.2f);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(20, 20); GL.Vertex2(20 + player1.Hp * 30, 20);
            GL.Vertex2(20 + player1.Hp * 30, 40); GL.Vertex2(20, 40);
            GL.End();

            // Красный патроны - пушка
            DrawAmmoBar(20, 45, player1, false);
            // Красный патроны - пулемет
            DrawAmmoBar(20, 60, player1, true);
            
            // Синий HP
            GL.Color3(0.2f, 0.5f, 0.9f);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(MAP_WIDTH - 20 - player2.Hp * 30, 20);
            GL.Vertex2(MAP_WIDTH - 20, 20);
            GL.Vertex2(MAP_WIDTH - 20, 40);
            GL.Vertex2(MAP_WIDTH - 20 - player2.Hp * 30, 40);
            GL.End();
            
            // Синий патроны - пушка
            DrawAmmoBar(MAP_WIDTH - 20 - player2.MaxAmmo * 12, 45, player2, false);
            // Синий патроны - пулемет
            DrawAmmoBar(MAP_WIDTH - 20 - 3 * 12, 60, player2, true);
            
            // Таймер баффов по центру
            DrawBuffTimer();
        }

        
        

        private void DrawAmmoBar(float x, float y, Tank tank, bool isMachinegun)
        {
            if (tank == null) return;
            
            int currentAmmo;
            int maxAmmo;
            bool isReloading;
            
            if (isMachinegun)
            {
                currentAmmo = tank.MachinegunAmmo;
                maxAmmo = tank.MaxMachinegunAmmo;
                isReloading = tank.IsMachinegunReloading;
            }
            else
            {
                currentAmmo = tank.RegularAmmo;
                maxAmmo = tank.MaxAmmo;
                isReloading = tank.IsRegularReloading;
            }
            
            if (isReloading)
            {
                GL.Color3(0.3f, 0.3f, 0.3f);
                float reloadTime = isMachinegun ? 20f : 10f;
                float reloadProgress = 1f - (tank.ReloadTimer / reloadTime);
                if (reloadProgress < 0) reloadProgress = 0;
                if (reloadProgress > 1) reloadProgress = 1;
                
                float barWidth = maxAmmo * 12;
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x, y);
                GL.Vertex2(x + barWidth * reloadProgress, y);
                GL.Vertex2(x + barWidth * reloadProgress, y + 12);
                GL.Vertex2(x, y + 12);
                GL.End();
                
                GL.Color3(0.5f, 0.5f, 0.5f);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x + barWidth * reloadProgress, y);
                GL.Vertex2(x + barWidth, y);
                GL.Vertex2(x + barWidth, y + 12);
                GL.Vertex2(x + barWidth * reloadProgress, y + 12);
                GL.End();
            }
            else
            {
                if (isMachinegun)
                {
                    GL.Color3(1f, 0.4f, 0.4f);
                }
                else
                {
                    GL.Color3(1f, 0.9f, 0.3f);
                }
                
                for (int i = 0; i < currentAmmo; i++)
                {
                    GL.Begin(PrimitiveType.Quads);
                    GL.Vertex2(x + i * 12, y);
                    GL.Vertex2(x + i * 12 + 10, y);
                    GL.Vertex2(x + i * 12 + 10, y + 10);
                    GL.Vertex2(x + i * 12, y + 10);
                    GL.End();
                }
                
                GL.Color3(0.3f, 0.3f, 0.3f);
                for (int i = currentAmmo; i < maxAmmo; i++)
                {
                    GL.Begin(PrimitiveType.Quads);
                    GL.Vertex2(x + i * 12, y);
                    GL.Vertex2(x + i * 12 + 10, y);
                    GL.Vertex2(x + i * 12 + 10, y + 10);
                    GL.Vertex2(x + i * 12, y + 10);
                    GL.End();
                }
            }
        }


        private void DrawBuffTimer()
        {
            float countdownValue;
            Color timerColor;
            
            if (buffDuration > 0)
            {
                countdownValue = buffDuration;
                timerColor = Color.FromArgb(255, 150, 100);
            }
            else
            {
                countdownValue = countdownTimer;
                timerColor = Color.FromArgb(180, 180, 180);
            }
            
            int displayValue = (int)Math.Ceiling(countdownValue);
            if (displayValue <= 0) displayValue = 0;
            
            DrawTimerNumber(MAP_WIDTH / 2, 40, displayValue, true, timerColor);
        }
        
        private void DrawTimerNumber(float centerX, float centerY, string text, Color color)
        {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;
            
            GL.Color3(r, g, b);
            
            float charWidth = 8f;
            float charHeight = 12f;
            float spacing = 2f;
            float totalWidth = text.Length * charWidth + (text.Length - 1) * spacing;
            float startX = centerX - totalWidth / 2;
            
            for (int i = 0; i < text.Length; i++)
            {
                DrawLetter(startX + i * (charWidth + spacing), centerY, text[i], charWidth, charHeight);
            }
        }
        
        private void DrawLetter(float x, float y, char c, float width, float height)
        {
            GL.Begin(PrimitiveType.Quads);
            
            if (c == 'Щ')
            {
                GL.Vertex2(x, y);
                GL.Vertex2(x + width, y);
                GL.Vertex2(x + width, y + height);
                GL.Vertex2(x, y + height);
                GL.End();
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x + width * 0.3f, y);
                GL.Vertex2(x + width * 0.3f, y + height * 0.4f);
                GL.Vertex2(x + width * 0.7f, y + height * 0.4f);
                GL.Vertex2(x + width * 0.7f, y);
                GL.End();
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x, y + height * 0.6f);
                GL.Vertex2(x + width, y + height * 0.6f);
                GL.Vertex2(x + width, y + height);
                GL.Vertex2(x, y + height);
                GL.End();
            }
            else if (c == 'И')
            {
                GL.Vertex2(x, y);
                GL.Vertex2(x + width, y);
                GL.Vertex2(x + width, y + height);
                GL.Vertex2(x, y + height);
                GL.End();
            }
            else if (c == 'Т')
            {
                GL.Vertex2(x, y);
                GL.Vertex2(x + width, y);
                GL.Vertex2(x + width, y + height * 0.2f);
                GL.Vertex2(x, y + height * 0.2f);
                GL.End();
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x + width * 0.5f, y);
                GL.Vertex2(x + width * 0.5f, y + height);
                GL.Vertex2(x + width * 0.5f + width * 0.15f, y + height);
                GL.Vertex2(x + width * 0.5f + width * 0.15f, y);
                GL.End();
            }
            else if (c == 'У')
            {
                GL.Vertex2(x, y);
                GL.Vertex2(x + width * 0.3f, y);
                GL.Vertex2(x + width * 0.3f, y + height);
                GL.Vertex2(x, y + height);
                GL.End();
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x + width * 0.3f, y + height * 0.5f);
                GL.Vertex2(x + width, y + height * 0.5f);
                GL.Vertex2(x + width, y + height);
                GL.Vertex2(x + width * 0.3f, y + height);
                GL.End();
            }
            else if (c == 'С')
            {
                GL.Vertex2(x, y);
                GL.Vertex2(x + width, y);
                GL.Vertex2(x + width, y + height * 0.2f);
                GL.Vertex2(x, y + height * 0.2f);
                GL.End();
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x, y + height * 0.8f);
                GL.Vertex2(x + width, y + height * 0.8f);
                GL.Vertex2(x + width, y + height);
                GL.Vertex2(x, y + height);
                GL.End();
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x, y + height * 0.2f);
                GL.Vertex2(x + width * 0.2f, y + height * 0.2f);
                GL.Vertex2(x + width * 0.2f, y + height * 0.8f);
                GL.Vertex2(x, y + height * 0.8f);
                GL.End();
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x + width * 0.8f, y + height * 0.2f);
                GL.Vertex2(x + width, y + height * 0.2f);
                GL.Vertex2(x + width, y + height * 0.8f);
                GL.Vertex2(x + width * 0.8f, y + height * 0.8f);
                GL.End();
            }
            else if (c == 'К')
            {
                GL.Vertex2(x, y);
                GL.Vertex2(x + width * 0.3f, y);
                GL.Vertex2(x + width * 0.3f, y + height);
                GL.Vertex2(x, y + height);
                GL.End();
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x + width * 0.7f, y);
                GL.Vertex2(x + width, y);
                GL.Vertex2(x + width, y + height * 0.45f);
                GL.Vertex2(x + width * 0.7f, y + height * 0.45f);
                GL.End();
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x + width * 0.7f, y + height * 0.55f);
                GL.Vertex2(x + width, y + height * 0.55f);
                GL.Vertex2(x + width, y + height);
                GL.Vertex2(x + width * 0.7f, y + height);
                GL.End();
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x + width * 0.3f, y + height * 0.4f);
                GL.Vertex2(x + width * 0.7f, y + height * 0.4f);
                GL.Vertex2(x + width * 0.7f, y + height * 0.6f);
                GL.Vertex2(x + width * 0.3f, y + height * 0.6f);
                GL.End();
            }
            else if (c == 'О')
            {
                GL.Vertex2(x + width * 0.2f, y);
                GL.Vertex2(x + width * 0.8f, y);
                GL.Vertex2(x + width * 0.8f, y + height);
                GL.Vertex2(x + width * 0.2f, y + height);
                GL.End();
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x, y + height * 0.2f);
                GL.Vertex2(x + width * 0.2f, y + height * 0.2f);
                GL.Vertex2(x + width * 0.2f, y + height * 0.8f);
                GL.Vertex2(x, y + height * 0.8f);
                GL.End();
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x + width * 0.8f, y + height * 0.2f);
                GL.Vertex2(x + width, y + height * 0.2f);
                GL.Vertex2(x + width, y + height * 0.8f);
                GL.Vertex2(x + width * 0.8f, y + height * 0.8f);
                GL.End();
            }
            else if (c == 'Р')
            {
                GL.Vertex2(x, y);
                GL.Vertex2(x + width * 0.7f, y);
                GL.Vertex2(x + width * 0.7f, y + height);
                GL.Vertex2(x, y + height);
                GL.End();
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x + width * 0.7f, y + height * 0.5f);
                GL.Vertex2(x + width, y + height * 0.5f);
                GL.Vertex2(x + width, y + height);
                GL.Vertex2(x + width * 0.7f, y + height);
                GL.End();
            }
            else if (c == 'Л')
            {
                GL.Vertex2(x, y);
                GL.Vertex2(x + width * 0.3f, y);
                GL.Vertex2(x + width * 0.3f, y + height);
                GL.Vertex2(x, y + height);
                GL.End();
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x, y + height * 0.7f);
                GL.Vertex2(x + width, y + height * 0.7f);
                GL.Vertex2(x + width, y + height);
                GL.Vertex2(x, y + height);
                GL.End();
            }
            else if (c == 'М')
            {
                GL.Vertex2(x, y);
                GL.Vertex2(x + width, y);
                GL.Vertex2(x + width, y + height);
                GL.Vertex2(x, y + height);
                GL.End();
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x + width * 0.5f, y);
                GL.Vertex2(x + width * 0.5f, y + height * 0.7f);
                GL.Vertex2(x + width * 0.5f + width * 0.2f, y + height * 0.7f);
                GL.Vertex2(x + width * 0.5f + width * 0.2f, y);
                GL.End();
            }
            else if (c == 'Ш')
            {
                GL.Vertex2(x, y);
                GL.Vertex2(x + width, y);
                GL.Vertex2(x + width, y + height * 0.15f);
                GL.Vertex2(x, y + height * 0.15f);
                GL.End();
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x + width * 0.33f, y);
                GL.Vertex2(x + width * 0.33f, y + height);
                GL.Vertex2(x + width * 0.33f + width * 0.12f, y + height);
                GL.Vertex2(x + width * 0.33f + width * 0.12f, y);
                GL.End();
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x + width * 0.67f, y);
                GL.Vertex2(x + width * 0.67f, y + height);
                GL.Vertex2(x + width * 0.67f + width * 0.12f, y + height);
                GL.Vertex2(x + width * 0.67f + width * 0.12f, y);
                GL.End();
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x + width * 0.33f, y + height * 0.7f);
                GL.Vertex2(x + width * 0.67f + width * 0.12f, y + height * 0.7f);
                GL.Vertex2(x + width * 0.67f + width * 0.12f, y + height);
                GL.Vertex2(x + width * 0.33f, y + height);
                GL.End();
            }
            else
            {
                GL.Vertex2(x, y);
                GL.Vertex2(x + width, y);
                GL.Vertex2(x + width, y + height);
                GL.Vertex2(x, y + height);
                GL.End();
            }
        }
        
        private void DrawTimerNumber(float centerX, float centerY, int number, bool padZero, Color color)
        {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;
            
            bool isTwoDigit = number >= 10 || (padZero && number >= 0);
            float digitWidth = 25f;
            float digitHeight = 40f;
            float spacing = 5f;
            float startX = centerX - ((isTwoDigit ? 2 : 1) * digitWidth + (isTwoDigit ? spacing : 0)) / 2;
            
            int firstDigit = isTwoDigit ? (number / 10) : 0;
            int secondDigit = number % 10;
            if (!isTwoDigit && secondDigit < 0) secondDigit = 0;
            
            if (isTwoDigit)
            {
                DrawDigit3D(startX, centerY, firstDigit, digitWidth, digitHeight, r, g, b);
                DrawDigit3D(startX + digitWidth + spacing, centerY, secondDigit, digitWidth, digitHeight, r, g, b);
            }
            else
            {
                DrawDigit3D(startX, centerY, secondDigit, digitWidth, digitHeight, r, g, b);
            }
        }
        
        private void DrawDigit3D(float x, float y, int digit, float width, float height, float r, float g, float b)
        {
            bool[] segments = GetSevenSegmentDigit(digit);
            
            float halfW = width / 2;
            float halfH = height / 2;
            float thickness = width / 4;
            
            if (segments[0])
            {
                GL.Color3(r, g, b);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x, y - halfH);
                GL.Vertex2(x + width, y - halfH);
                GL.Vertex2(x + width, y - halfH + thickness);
                GL.Vertex2(x, y - halfH + thickness);
                GL.End();
                
                GL.Color3(r * 0.6f, g * 0.6f, b * 0.6f);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x, y - halfH + thickness);
                GL.Vertex2(x + width, y - halfH + thickness);
                GL.Vertex2(x + width, y - halfH + thickness + 1);
                GL.Vertex2(x, y - halfH + thickness + 1);
                GL.End();
            }
            if (segments[1])
            {
                GL.Color3(r, g, b);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x + width - thickness, y - halfH);
                GL.Vertex2(x + width, y - halfH);
                GL.Vertex2(x + width, y);
                GL.Vertex2(x + width - thickness, y);
                GL.End();
                
                GL.Color3(r * 0.6f, g * 0.6f, b * 0.6f);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x + width - thickness, y);
                GL.Vertex2(x + width, y);
                GL.Vertex2(x + width, y + 1);
                GL.Vertex2(x + width - thickness, y + 1);
                GL.End();
            }
            if (segments[2])
            {
                GL.Color3(r, g, b);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x + width - thickness, y);
                GL.Vertex2(x + width, y);
                GL.Vertex2(x + width, y + halfH);
                GL.Vertex2(x + width - thickness, y + halfH);
                GL.End();
                
                GL.Color3(r * 0.6f, g * 0.6f, b * 0.6f);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x + width - thickness, y + halfH);
                GL.Vertex2(x + width, y + halfH);
                GL.Vertex2(x + width, y + halfH + 1);
                GL.Vertex2(x + width - thickness, y + halfH + 1);
                GL.End();
            }
            if (segments[3])
            {
                GL.Color3(r, g, b);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x, y + halfH - thickness);
                GL.Vertex2(x + width, y + halfH - thickness);
                GL.Vertex2(x + width, y + halfH);
                GL.Vertex2(x, y + halfH);
                GL.End();
                
                GL.Color3(r * 0.6f, g * 0.6f, b * 0.6f);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x, y + halfH);
                GL.Vertex2(x + width, y + halfH);
                GL.Vertex2(x + width, y + halfH + 1);
                GL.Vertex2(x, y + halfH + 1);
                GL.End();
            }
            if (segments[4])
            {
                GL.Color3(r, g, b);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x, y);
                GL.Vertex2(x + thickness, y);
                GL.Vertex2(x + thickness, y + halfH);
                GL.Vertex2(x, y + halfH);
                GL.End();
                
                GL.Color3(r * 0.6f, g * 0.6f, b * 0.6f);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x, y + halfH);
                GL.Vertex2(x + thickness, y + halfH);
                GL.Vertex2(x + thickness, y + halfH + 1);
                GL.Vertex2(x, y + halfH + 1);
                GL.End();
            }
            if (segments[5])
            {
                GL.Color3(r, g, b);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x, y - halfH);
                GL.Vertex2(x + thickness, y - halfH);
                GL.Vertex2(x + thickness, y);
                GL.Vertex2(x, y);
                GL.End();
                
                GL.Color3(r * 0.6f, g * 0.6f, b * 0.6f);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x, y);
                GL.Vertex2(x + thickness, y);
                GL.Vertex2(x + thickness, y + 1);
                GL.Vertex2(x, y + 1);
                GL.End();
            }
            if (segments[6])
            {
                GL.Color3(r, g, b);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x, y - thickness);
                GL.Vertex2(x + width, y - thickness);
                GL.Vertex2(x + width, y);
                GL.Vertex2(x, y);
                GL.End();
                
                GL.Color3(r * 0.6f, g * 0.6f, b * 0.6f);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x, y);
                GL.Vertex2(x + width, y);
                GL.Vertex2(x + width, y + 1);
                GL.Vertex2(x, y + 1);
                GL.End();
            }
        }
        
        private bool[] GetSevenSegmentDigit(int digit)
        {
            switch (digit)
            {
                case 0: return new bool[] { true, true, true, true, true, true, false };
                case 1: return new bool[] { false, true, true, false, false, false, false };
                case 2: return new bool[] { true, true, false, true, true, false, true };
                case 3: return new bool[] { true, true, true, true, false, false, true };
                case 4: return new bool[] { false, true, true, false, false, true, true };
                case 5: return new bool[] { true, false, true, true, false, true, true };
                case 6: return new bool[] { true, false, true, true, true, true, true };
                case 7: return new bool[] { true, true, true, false, false, false, false };
                case 8: return new bool[] { true, true, true, true, true, true, true };
                case 9: return new bool[] { true, true, true, true, false, true, true };
                default: return new bool[] { false, false, false, false, false, false, false };
            }
        }       
    }
}