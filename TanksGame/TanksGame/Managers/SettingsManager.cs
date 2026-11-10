using System;
using System.IO;
using System.Windows.Forms;

namespace TanksGame.Managers
{
    public static class SettingsManager
    {
        private static string settingsFile = "settings.txt";
        
        public static float TankSpeed { get; set; } = 3.6f;
        public static float BulletSpeed { get; set; } = 10.5f;
        public static bool RicochetEnabled { get; set; } = true;
        public static int PlayerHp { get; set; } = 6;
        public static int SelectedMap { get; set; } = 1;
        
        public static bool MusicEnabled { get; set; } = true;
        public static bool ShootSoundEnabled { get; set; } = true;
        public static bool ExplosionSoundEnabled { get; set; } = true;
        public static float MasterVolume { get; set; } = 0.7f;
        
        public static Keys P1_Up { get; set; } = Keys.W;
        public static Keys P1_Down { get; set; } = Keys.S;
        public static Keys P1_Left { get; set; } = Keys.A;
        public static Keys P1_Right { get; set; } = Keys.D;
        public static Keys P1_Fire { get; set; } = Keys.E;
        public static Keys P1_SwitchWeapon { get; set; } = Keys.Q;
        
        public static Keys P2_Up { get; set; } = Keys.I;
        public static Keys P2_Down { get; set; } = Keys.K;
        public static Keys P2_Left { get; set; } = Keys.J;
        public static Keys P2_Right { get; set; } = Keys.L;
        public static Keys P2_Fire { get; set; } = Keys.O;
        public static Keys P2_SwitchWeapon { get; set; } = Keys.U;
        
        public static float MachinegunFireRate { get; set; } = 0.08f;
        public static int MachinegunDamage { get; set; } = 1;
        
        public static void LoadSettings()
        {
            try
            {
                if (!File.Exists(settingsFile))
                {
                    SaveSettings();
                }
                
                if (File.Exists(settingsFile))
                {
                    var lines = File.ReadAllLines(settingsFile);
                    foreach (var line in lines)
                    {
                        var parts = line.Split('=');
                        if (parts.Length == 2)
                        {
                            string key = parts[0];
                            string value = parts[1];
                            
                            switch (key)
                            {
                                case "TankSpeed":
                                    if (float.TryParse(value, out float ts)) TankSpeed = ts;
                                    break;
                                case "BulletSpeed":
                                    if (float.TryParse(value, out float bs)) BulletSpeed = bs;
                                    break;
                                case "RicochetEnabled":
                                    RicochetEnabled = value == "1";
                                    break;
                                case "PlayerHp":
                                    if (int.TryParse(value, out int hp)) PlayerHp = hp;
                                    break;
                                case "SelectedMap":
                                    if (int.TryParse(value, out int map)) SelectedMap = map;
                                    break;
                                case "MusicEnabled":
                                    MusicEnabled = value == "1";
                                    break;
                                case "ShootSoundEnabled":
                                    ShootSoundEnabled = value == "1";
                                    break;
                                case "ExplosionSoundEnabled":
                                    ExplosionSoundEnabled = value == "1";
                                    break;
                                case "MasterVolume":
                                    if (float.TryParse(value, out float mv)) MasterVolume = mv;
                                    break;
                                case "P1_Up":
                                    if (Enum.TryParse<Keys>(value, out var p1u)) P1_Up = p1u;
                                    break;
                                case "P1_Down":
                                    if (Enum.TryParse<Keys>(value, out var p1d)) P1_Down = p1d;
                                    break;
                                case "P1_Left":
                                    if (Enum.TryParse<Keys>(value, out var p1l)) P1_Left = p1l;
                                    break;
                                case "P1_Right":
                                    if (Enum.TryParse<Keys>(value, out var p1r)) P1_Right = p1r;
                                    break;
                                case "P1_Fire":
                                    if (Enum.TryParse<Keys>(value, out var p1f)) P1_Fire = p1f;
                                    break;
                                case "P2_Up":
                                    if (Enum.TryParse<Keys>(value, out var p2u)) P2_Up = p2u;
                                    break;
                                case "P2_Down":
                                    if (Enum.TryParse<Keys>(value, out var p2d)) P2_Down = p2d;
                                    break;
                                case "P2_Left":
                                    if (Enum.TryParse<Keys>(value, out var p2l)) P2_Left = p2l;
                                    break;
                                case "P2_Right":
                                    if (Enum.TryParse<Keys>(value, out var p2r)) P2_Right = p2r;
                                    break;
                                case "P2_Fire":
                                    if (Enum.TryParse<Keys>(value, out var p2f)) P2_Fire = p2f;
                                    break;
                                case "P1_SwitchWeapon":
                                    if (Enum.TryParse<Keys>(value, out var p1sw)) P1_SwitchWeapon = p1sw;
                                    break;
                                case "P2_SwitchWeapon":
                                    if (Enum.TryParse<Keys>(value, out var p2sw)) P2_SwitchWeapon = p2sw;
                                    break;
                                case "MachinegunFireRate":
                                    if (float.TryParse(value, out float mfr)) MachinegunFireRate = mfr;
                                    break;
                                case "MachinegunDamage":
                                    if (int.TryParse(value, out int md)) MachinegunDamage = md;
                                    break;
                            }
                        }
                    }
                }
            }
            catch
            {
                LoadDefaults();
            }
        }
        
        public static void SaveSettings()
        {
            try
            {
                var lines = new string[]
                {
                    $"TankSpeed={TankSpeed}",
                    $"BulletSpeed={BulletSpeed}",
                    $"RicochetEnabled={(RicochetEnabled ? "1" : "0")}",
                    $"PlayerHp={PlayerHp}",
                    $"SelectedMap={SelectedMap}",
                    $"MusicEnabled={(MusicEnabled ? "1" : "0")}",
                    $"ShootSoundEnabled={(ShootSoundEnabled ? "1" : "0")}",
                    $"ExplosionSoundEnabled={(ExplosionSoundEnabled ? "1" : "0")}",
                    $"MasterVolume={MasterVolume}",
                    $"P1_Up={P1_Up}",
                    $"P1_Down={P1_Down}",
                    $"P1_Left={P1_Left}",
                    $"P1_Right={P1_Right}",
                    $"P1_Fire={P1_Fire}",
                    $"P2_Up={P2_Up}",
                    $"P2_Down={P2_Down}",
                    $"P2_Left={P2_Left}",
                    $"P2_Right={P2_Right}",
                    $"P2_Fire={P2_Fire}",
                    $"P1_SwitchWeapon={P1_SwitchWeapon}",
                    $"P2_SwitchWeapon={P2_SwitchWeapon}",
                    $"MachinegunFireRate={MachinegunFireRate}",
                    $"MachinegunDamage={MachinegunDamage}"
                };
                File.WriteAllLines(settingsFile, lines);
            }
            catch { }
        }
        
        public static void LoadDefaults()
        {
            TankSpeed = 3.6f;
            BulletSpeed = 10.5f;
            RicochetEnabled = true;
            PlayerHp = 6;
            SelectedMap = 1;
            
            MusicEnabled = true;
            ShootSoundEnabled = true;
            ExplosionSoundEnabled = true;
            MasterVolume = 0.7f;
            
            P1_Up = Keys.W;
            P1_Down = Keys.S;
            P1_Left = Keys.A;
            P1_Right = Keys.D;
            P1_Fire = Keys.E;
            P1_SwitchWeapon = Keys.Q;
            
            P2_Up = Keys.I;
            P2_Down = Keys.K;
            P2_Left = Keys.J;
            P2_Right = Keys.L;
            P2_Fire = Keys.O;
            P2_SwitchWeapon = Keys.U;
            
            MachinegunFireRate = 0.08f;
            MachinegunDamage = 1;
        }
    }
}
