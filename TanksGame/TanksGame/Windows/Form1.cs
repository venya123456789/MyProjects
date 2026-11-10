using TanksGame.Managers;

namespace TanksGame.Windows
{
    public partial class Form1 : Form
    {
        private Button btnPlay;
        private Button btnSettings;
        private Button btnExit;
        private Label lblTitle;

        public Form1()
        {
            SettingsManager.LoadSettings();
            SoundManager.Initialize();
            
            
            this.Text = "Танки — Курсовой ИТИ-2 2026";
            this.Size = new Size(900, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.Black;
            this.DoubleBuffered = true;

           
            try
            {
                string bgPath = "menu_bg.jpg";
                this.BackgroundImage = Image.FromFile(bgPath);
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch
            {
                
                this.BackColor = Color.FromArgb(30, 30, 40);
            }

            
            lblTitle = new Label
            {
                Text = "Т А Н К И",
                Font = new Font("Arial", 72, FontStyle.Bold),
                ForeColor = Color.LimeGreen,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(700, 120),
                Location = new Point((this.ClientSize.Width - 700) / 2, 60),
                AutoSize = false
            };
            this.Controls.Add(lblTitle);

            
            btnPlay = CreateStyledButton("ИГРАТЬ", 36, Color.DarkGreen, Color.ForestGreen);
            btnPlay.Location = new Point((this.ClientSize.Width - 350) / 2, 220);
            btnPlay.Size = new Size(350, 90);
            btnPlay.Click += BtnPlay_Click;
            this.Controls.Add(btnPlay);

            
            btnSettings = CreateStyledButton("НАСТРОЙКИ", 24, Color.DarkSlateGray, Color.SlateGray);
            btnSettings.Location = new Point((this.ClientSize.Width - 350) / 2, 340);
            btnSettings.Size = new Size(350, 70);
            btnSettings.Click += BtnSettings_Click;
            this.Controls.Add(btnSettings);

            
            btnExit = CreateStyledButton("ВЫХОД", 24, Color.Maroon, Color.Firebrick);
            btnExit.Location = new Point((this.ClientSize.Width - 350) / 2, 440);
            btnExit.Size = new Size(350, 70);
            btnExit.Click += BtnExit_Click;
            this.Controls.Add(btnExit);

            
            this.Resize += (s, e) =>
            {
                lblTitle.Location = new Point((this.ClientSize.Width - lblTitle.Width) / 2, 60);
                btnPlay.Location = new Point((this.ClientSize.Width - btnPlay.Width) / 2, 220);
                btnSettings.Location = new Point((this.ClientSize.Width - btnSettings.Width) / 2, 340);
                btnExit.Location = new Point((this.ClientSize.Width - btnExit.Width) / 2, 440);
            };
        }

        private Button CreateStyledButton(string text, float fontSize, Color normalColor, Color hoverColor)
        {
            Button btn = new Button
            {
                Text = text,
                Font = new Font("Arial", fontSize, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = normalColor,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0, MouseOverBackColor = hoverColor },
                Cursor = Cursors.Hand
            };

            
            btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = normalColor;

            return btn;
        }

        private void BtnPlay_Click(object sender, EventArgs e)
        {
            this.Hide();
            var gameForm = new GameForm();
            gameForm.FormClosed += (s, args) => this.Show();
            gameForm.Show();
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            var settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}