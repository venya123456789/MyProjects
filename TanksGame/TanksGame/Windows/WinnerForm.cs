namespace TanksGame.Windows
{
    public class WinnerForm : Form
    {
        private Label lblWinner;
        private Button btnRestart;
        private Button btnHome;
        private Color winnerColor;

        public Action OnRestart { get; set; }
        public Action OnHome { get; set; }

        public WinnerForm(string winnerName, Color color)
        {
            winnerColor = color;
            
            Text = "ПОБЕДА";
            Size = new Size(500, 350);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.FromArgb(25, 25, 35);

            lblWinner = new Label
            {
                Text = winnerName + "\nПОБЕДИЛ!",
                Font = new Font("Arial", 36, FontStyle.Bold),
                ForeColor = color,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(450, 150),
                Location = new Point(25, 30)
            };
            Controls.Add(lblWinner);

            btnRestart = new Button
            {
                Text = "РЕСТАРТ",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(60, 120, 60),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Size = new Size(200, 50),
                Location = new Point(30, 220),
                Cursor = Cursors.Hand
            };
            btnRestart.MouseEnter += (s, e) => btnRestart.BackColor = ControlPaint.Light(Color.FromArgb(60, 120, 60), 0.2f);
            btnRestart.MouseLeave += (s, e) => btnRestart.BackColor = Color.FromArgb(60, 120, 60);
            btnRestart.Click += (s, e) => OnRestart?.Invoke();
            Controls.Add(btnRestart);

            btnHome = new Button
            {
                Text = "В МЕНЮ",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(120, 60, 60),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Size = new Size(200, 50),
                Location = new Point(250, 220),
                Cursor = Cursors.Hand
            };
            btnHome.MouseEnter += (s, e) => btnHome.BackColor = ControlPaint.Light(Color.FromArgb(120, 60, 60), 0.2f);
            btnHome.MouseLeave += (s, e) => btnHome.BackColor = Color.FromArgb(120, 60, 60);
            btnHome.Click += (s, e) => OnHome?.Invoke();
            Controls.Add(btnHome);
        }
    }
}
