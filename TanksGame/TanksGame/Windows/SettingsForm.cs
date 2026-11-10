using TanksGame.Managers;

namespace TanksGame.Windows
{
    public class SettingsForm : Form
    {
        private Panel panelButtons;
        private Panel panelContent;
        private Button btnControls;
        private Button btnMusic;
        private Button btnMap;
        private Button btnGame;
        private Button btnBack;
        private Label lblTitle;

        private TrackBar trackTankSpeed;
        private TrackBar trackBulletSpeed;
        private NumericUpDown numHp;
        private ComboBox cmbMap;

        public SettingsForm()
        {
            Text = "Настройки";
            Size = new Size(900, 650);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.FromArgb(25, 25, 35);

            lblTitle = new Label
            {
                Text = "НАСТРОЙКИ",
                Font = new Font("Arial", 28, FontStyle.Bold),
                ForeColor = Color.LimeGreen,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(900, 60),
                Location = new Point(0, 10)
            };
            Controls.Add(lblTitle);

            panelButtons = new Panel
            {
                Location = new Point(20, 80),
                Size = new Size(180, 500),
                BackColor = Color.Transparent
            };
            Controls.Add(panelButtons);

            panelContent = new Panel
            {
                Location = new Point(220, 80),
                Size = new Size(650, 500),
                BackColor = Color.FromArgb(35, 35, 50),
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(panelContent);

            Color btnColor = Color.FromArgb(60, 80, 60);

            btnControls = CreateButton("Управление", btnColor);
            btnControls.Click += (s, e) => ShowControls();
            panelButtons.Controls.Add(btnControls);

            btnMusic = CreateButton("Музыка", btnColor);
            btnMusic.Click += (s, e) => ShowMusic();
            panelButtons.Controls.Add(btnMusic);

            btnMap = CreateButton("Карта", btnColor);
            btnMap.Click += (s, e) => ShowMap();
            panelButtons.Controls.Add(btnMap);

            btnGame = CreateButton("Игра", btnColor);
            btnGame.Click += (s, e) => ShowGame();
            panelButtons.Controls.Add(btnGame);

            btnBack = CreateButton("Назад", Color.FromArgb(80, 60, 60));
            btnBack.Click += BtnBack_Click;
            panelButtons.Controls.Add(btnBack);

            SettingsManager.LoadSettings();
            ShowControls();
        }

        private Button CreateButton(string text, Color color)
        {
            var btn = new Button
            {
                Text = text,
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = color,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0, MouseOverBackColor = ControlPaint.Light(color, 0.15f) },
                Cursor = Cursors.Hand,
                Size = new Size(160, 55),
                Location = new Point(10, panelButtons.Controls.Count * 70)
            };
            return btn;
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            SettingsManager.SaveSettings();
            Close();
        }

        private void ShowControls()
        {
            panelContent.Controls.Clear();

            var title = new Label
            {
                Text = "УПРАВЛЕНИЕ",
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = Color.LimeGreen,
                Location = new Point(25, 20),
                Size = new Size(400, 35)
            };
            panelContent.Controls.Add(title);

            var lblP1 = new Label
            {
                Text = "Красный игрок (P1)",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 60, 60),
                Location = new Point(25, 70),
                Size = new Size(200, 25)
            };
            panelContent.Controls.Add(lblP1);

            var lblP2 = new Label
            {
                Text = "Синий игрок (P2)",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 220),
                Location = new Point(350, 70),
                Size = new Size(200, 25)
            };
            panelContent.Controls.Add(lblP2);

            CreateKeyButton("Вперёд", SettingsManager.P1_Up, 25, 100, Color.FromArgb(220, 60, 60), true);
            CreateKeyButton("Назад", SettingsManager.P1_Down, 25, 140, Color.FromArgb(220, 60, 60), true);
            CreateKeyButton("Влево", SettingsManager.P1_Left, 25, 180, Color.FromArgb(220, 60, 60), true);
            CreateKeyButton("Вправо", SettingsManager.P1_Right, 25, 220, Color.FromArgb(220, 60, 60), true);
            CreateKeyButton("Выстрел", SettingsManager.P1_Fire, 25, 260, Color.FromArgb(220, 60, 60), true);
            CreateKeyButton("Смена оружия", SettingsManager.P1_SwitchWeapon, 25, 300, Color.FromArgb(220, 60, 60), true);

            CreateKeyButton("Вперёд", SettingsManager.P2_Up, 350, 100, Color.FromArgb(70, 130, 220), false);
            CreateKeyButton("Назад", SettingsManager.P2_Down, 350, 140, Color.FromArgb(70, 130, 220), false);
            CreateKeyButton("Влево", SettingsManager.P2_Left, 350, 180, Color.FromArgb(70, 130, 220), false);
            CreateKeyButton("Вправо", SettingsManager.P2_Right, 350, 220, Color.FromArgb(70, 130, 220), false);
            CreateKeyButton("Выстрел", SettingsManager.P2_Fire, 350, 260, Color.FromArgb(70, 130, 220), false);
            CreateKeyButton("Смена оружия", SettingsManager.P2_SwitchWeapon, 350, 300, Color.FromArgb(70, 130, 220), false);

            var lblEsc = new Label
            {
                Text = "ESC - Выход в меню",
                Font = new Font("Arial", 12),
                ForeColor = Color.Gray,
                Location = new Point(25, 380),
                Size = new Size(250, 25)
            };
            panelContent.Controls.Add(lblEsc);

            var lblHint = new Label
            {
                Text = "Нажмите на кнопку для изменения клавиши",
                Font = new Font("Arial", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(25, 430),
                Size = new Size(400, 25)
            };
            panelContent.Controls.Add(lblHint);
        }

        private Button CreateKeyButton(string action, Keys currentKey, int x, int y, Color playerColor, bool isPlayer1)
        {
            var btn = new Button
            {
                Text = $"{action}: {GetKeyName(currentKey)}",
                Font = new Font("Arial", 11),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(50, 50, 60),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Location = new Point(x, y),
                Size = new Size(140, 32),
                Tag = new KeyBinding { Action = action, IsPlayer1 = isPlayer1 }
            };
            
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(70, 70, 80);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(50, 50, 60);
            btn.Click += BtnKey_Click;
            
            panelContent.Controls.Add(btn);
            return btn;
        }

        private void BtnKey_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            btn.Text = "Нажмите клавишу...";
            btn.BackColor = Color.FromArgb(100, 80, 50);
            
            var binding = btn.Tag as KeyBinding;
            if (binding == null) return;
            
            this.KeyPreview = true;
            this.KeyDown -= KeyCaptured;
            this.KeyDown += KeyCaptured;
            
            void KeyCaptured(object s, KeyEventArgs args)
            {
                if (args.KeyCode == Keys.Escape)
                {
                    btn.Text = $"{binding.Action}: {GetKeyName(GetCurrentKey(binding))}";
                    btn.BackColor = Color.FromArgb(50, 50, 60);
                    this.KeyDown -= KeyCaptured;
                    return;
                }
                
                if (IsKeyAlreadyAssigned(args.KeyCode, binding))
                {
                    MessageBox.Show("Эта клавиша уже назначена!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btn.Text = $"{binding.Action}: {GetKeyName(GetCurrentKey(binding))}";
                    btn.BackColor = Color.FromArgb(50, 50, 60);
                    this.KeyDown -= KeyCaptured;
                    return;
                }
                
                SetKey(binding, args.KeyCode);
                btn.Text = $"{binding.Action}: {GetKeyName(args.KeyCode)}";
                btn.BackColor = Color.FromArgb(50, 50, 60);
                this.KeyDown -= KeyCaptured;
            }
        }

        private bool IsKeyAlreadyAssigned(Keys key, KeyBinding exclude)
        {
            if (exclude.IsPlayer1)
            {
                if (key == SettingsManager.P1_Up || key == SettingsManager.P1_Down ||
                    key == SettingsManager.P1_Left || key == SettingsManager.P1_Right || key == SettingsManager.P1_Fire ||
                    key == SettingsManager.P1_SwitchWeapon)
                    return true;
            }
            else
            {
                if (key == SettingsManager.P2_Up || key == SettingsManager.P2_Down ||
                    key == SettingsManager.P2_Left || key == SettingsManager.P2_Right || key == SettingsManager.P2_Fire ||
                    key == SettingsManager.P2_SwitchWeapon)
                    return true;
            }
            return false;
        }

        private Keys GetCurrentKey(KeyBinding binding)
        {
            if (binding.IsPlayer1)
            {
                switch (binding.Action)
                {
                    case "Вперёд": return SettingsManager.P1_Up;
                    case "Назад": return SettingsManager.P1_Down;
                    case "Влево": return SettingsManager.P1_Left;
                    case "Вправо": return SettingsManager.P1_Right;
                    case "Выстрел": return SettingsManager.P1_Fire;
                    case "Смена оружия": return SettingsManager.P1_SwitchWeapon;
                }
            }
            else
            {
                switch (binding.Action)
                {
                    case "Вперёд": return SettingsManager.P2_Up;
                    case "Назад": return SettingsManager.P2_Down;
                    case "Влево": return SettingsManager.P2_Left;
                    case "Вправо": return SettingsManager.P2_Right;
                    case "Выстрел": return SettingsManager.P2_Fire;
                    case "Смена оружия": return SettingsManager.P2_SwitchWeapon;
                }
            }
            return Keys.None;
        }

        private void SetKey(KeyBinding binding, Keys key)
        {
            if (binding.IsPlayer1)
            {
                switch (binding.Action)
                {
                    case "Вперёд": SettingsManager.P1_Up = key; break;
                    case "Назад": SettingsManager.P1_Down = key; break;
                    case "Влево": SettingsManager.P1_Left = key; break;
                    case "Вправо": SettingsManager.P1_Right = key; break;
                    case "Выстрел": SettingsManager.P1_Fire = key; break;
                    case "Смена оружия": SettingsManager.P1_SwitchWeapon = key; break;
                }
            }
            else
            {
                switch (binding.Action)
                {
                    case "Вперёд": SettingsManager.P2_Up = key; break;
                    case "Назад": SettingsManager.P2_Down = key; break;
                    case "Влево": SettingsManager.P2_Left = key; break;
                    case "Вправо": SettingsManager.P2_Right = key; break;
                    case "Выстрел": SettingsManager.P2_Fire = key; break;
                    case "Смена оружия": SettingsManager.P2_SwitchWeapon = key; break;
                }
            }
        }

        private string GetKeyName(Keys key)
        {
            if (key == Keys.Space) return "Пробел";
            if (key == Keys.None) return "Не назначено";
            return key.ToString();
        }

        private class KeyBinding
        {
            public string Action { get; set; }
            public bool IsPlayer1 { get; set; }
        }

        private void ShowMusic()
        {
            panelContent.Controls.Clear();

            var title = new Label
            {
                Text = "ЗВУК",
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = Color.LimeGreen,
                Location = new Point(25, 20),
                Size = new Size(400, 35)
            };
            panelContent.Controls.Add(title);

            var lblVolume = new Label
            {
                Text = $"Общая громкость: {(int)(SettingsManager.MasterVolume * 100)}%",
                Font = new Font("Arial", 12),
                ForeColor = Color.White,
                Location = new Point(25, 70),
                Size = new Size(200, 25)
            };
            panelContent.Controls.Add(lblVolume);

            var trackVolume = new TrackBar
            {
                Location = new Point(25, 100),
                Size = new Size(400, 30),
                Minimum = 0,
                Maximum = 100,
                Value = (int)(SettingsManager.MasterVolume * 100)
            };
            trackVolume.ValueChanged += (s, e) =>
            {
                SettingsManager.MasterVolume = trackVolume.Value / 100f;
                lblVolume.Text = $"Общая громкость: {(int)(SettingsManager.MasterVolume * 100)}%";
                SoundManager.UpdateSounds();
            };
            panelContent.Controls.Add(trackVolume);

            var chkMusic = new CheckBox
            {
                Text = "Фоновая музыка",
                Location = new Point(25, 150),
                Size = new Size(250, 25),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Checked = SettingsManager.MusicEnabled
            };
            chkMusic.CheckedChanged += (s, e) =>
            {
                SettingsManager.MusicEnabled = chkMusic.Checked;
                SoundManager.UpdateSounds();
            };
            panelContent.Controls.Add(chkMusic);

            var chkShoot = new CheckBox
            {
                Text = "Звук выстрела",
                Location = new Point(25, 190),
                Size = new Size(250, 25),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Checked = SettingsManager.ShootSoundEnabled
            };
            chkShoot.CheckedChanged += (s, e) =>
            {
                SettingsManager.ShootSoundEnabled = chkShoot.Checked;
            };
            panelContent.Controls.Add(chkShoot);

            var chkExplosion = new CheckBox
            {
                Text = "Звук взрыва",
                Location = new Point(25, 230),
                Size = new Size(250, 25),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Checked = SettingsManager.ExplosionSoundEnabled
            };
            chkExplosion.CheckedChanged += (s, e) =>
            {
                SettingsManager.ExplosionSoundEnabled = chkExplosion.Checked;
            };
            panelContent.Controls.Add(chkExplosion);
        }

        private void ShowMap()
        {
            panelContent.Controls.Clear();

            var title = new Label
            {
                Text = "КАРТА",
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = Color.LimeGreen,
                Location = new Point(25, 20),
                Size = new Size(400, 35)
            };
            panelContent.Controls.Add(title);

            var lblSelect = new Label
            {
                Text = "Выберите карту:",
                Font = new Font("Arial", 13),
                ForeColor = Color.White,
                Location = new Point(25, 80),
                Size = new Size(200, 25)
            };
            panelContent.Controls.Add(lblSelect);

            cmbMap = new ComboBox
            {
                Location = new Point(25, 115),
                Size = new Size(250, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 12)
            };
            cmbMap.Items.Add("1. Стандартная карта");
            cmbMap.Items.Add("2. Форпост");
            cmbMap.Items.Add("3. Турнир");
            cmbMap.SelectedIndex = SettingsManager.SelectedMap - 1;
            cmbMap.SelectedIndexChanged += (s, e) => 
            {
                SettingsManager.SelectedMap = cmbMap.SelectedIndex + 1;
            };
            panelContent.Controls.Add(cmbMap);

            string[] mapDescriptions = new string[]
            {
                "Классическая карта с препятствиями\nпо центру и углам.",
                "Симметричная арена с центральным\nукреплением и фланговыми позициями.",
                "Дуэльная арена с секторными\nукрытиями и кольцевым центром."
            };

            var lblDesc = new Label
            {
                Text = mapDescriptions[cmbMap.SelectedIndex],
                Font = new Font("Arial", 11),
                ForeColor = Color.LightGray,
                Location = new Point(25, 170),
                Size = new Size(550, 60)
            };
            panelContent.Controls.Add(lblDesc);

            cmbMap.SelectedIndexChanged += (s, e) =>
            {
                lblDesc.Text = mapDescriptions[cmbMap.SelectedIndex];
            };
        }

        private void ShowGame()
        {
            panelContent.Controls.Clear();

            var title = new Label
            {
                Text = "НАСТРОЙКИ ИГРЫ",
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = Color.LimeGreen,
                Location = new Point(25, 20),
                Size = new Size(400, 35)
            };
            panelContent.Controls.Add(title);

            var lblTankSpeed = new Label
            {
                Text = $"Скорость танка: {SettingsManager.TankSpeed:F1}",
                Font = new Font("Arial", 12),
                ForeColor = Color.White,
                Location = new Point(25, 80),
                Size = new Size(200, 25)
            };
            panelContent.Controls.Add(lblTankSpeed);

            trackTankSpeed = new TrackBar
            {
                Location = new Point(25, 110),
                Size = new Size(400, 30),
                Minimum = 1,
                Maximum = 10,
                Value = (int)SettingsManager.TankSpeed
            };
            trackTankSpeed.ValueChanged += (s, e) =>
            {
                SettingsManager.TankSpeed = trackTankSpeed.Value;
                lblTankSpeed.Text = $"Скорость танка: {SettingsManager.TankSpeed:F1}";
            };
            panelContent.Controls.Add(trackTankSpeed);

            var lblBulletSpeed = new Label
            {
                Text = $"Скорость пули: {SettingsManager.BulletSpeed:F1}",
                Font = new Font("Arial", 12),
                ForeColor = Color.White,
                Location = new Point(25, 170),
                Size = new Size(200, 25)
            };
            panelContent.Controls.Add(lblBulletSpeed);

            trackBulletSpeed = new TrackBar
            {
                Location = new Point(25, 200),
                Size = new Size(400, 30),
                Minimum = 5,
                Maximum = 20,
                Value = (int)SettingsManager.BulletSpeed
            };
            trackBulletSpeed.ValueChanged += (s, e) =>
            {
                SettingsManager.BulletSpeed = trackBulletSpeed.Value;
                lblBulletSpeed.Text = $"Скорость пули: {SettingsManager.BulletSpeed:F1}";
            };
            panelContent.Controls.Add(trackBulletSpeed);

            var lblHp = new Label
            {
                Text = "Количество HP:",
                Font = new Font("Arial", 12),
                ForeColor = Color.White,
                Location = new Point(25, 260),
                Size = new Size(150, 25)
            };
            panelContent.Controls.Add(lblHp);

            numHp = new NumericUpDown
            {
                Location = new Point(180, 258),
                Size = new Size(60, 30),
                Minimum = 1,
                Maximum = 10,
                Value = SettingsManager.PlayerHp,
                Font = new Font("Arial", 11)
            };
            numHp.ValueChanged += (s, e) =>
            {
                SettingsManager.PlayerHp = (int)numHp.Value;
            };
            panelContent.Controls.Add(numHp);
        }
    }
}
