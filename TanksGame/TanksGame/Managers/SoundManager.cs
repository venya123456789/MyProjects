using NAudio.Wave;

namespace TanksGame.Managers
{
    public static class SoundManager
    {
        private static WaveOutEvent? musicOutput;
        private static AudioFileReader? musicReader;
        private static WaveOutEvent? shootOutput;
        private static AudioFileReader? shootReader;
        private static WaveOutEvent? explosionOutput;
        private static AudioFileReader? explosionReader;
        
        private static bool isMusicPlaying = false;
        private static bool isDisposed = false;
        private static bool isInitialized = false;
        private static string soundsPath = "sounds";
        
        public static void Initialize()
        {
            if (isInitialized) return;
            isInitialized = true;
            
            try
            {
                if (!Directory.Exists(soundsPath))
                {
                    Directory.CreateDirectory(soundsPath);
                    return;
                }
                
                string musicPath = Path.Combine(soundsPath, "music.wav");
                string shootPath = Path.Combine(soundsPath, "shoot.wav");
                string explosionPath = Path.Combine(soundsPath, "explosion.wav");
                
                if (File.Exists(musicPath))
                {
                    try
                    {
                        musicReader = new AudioFileReader(musicPath);
                        musicOutput = new WaveOutEvent();
                        musicOutput.Init(musicReader);
                        musicOutput.PlaybackStopped += (s, e) =>
                        {
                            if (musicReader != null && musicOutput != null && SettingsManager.MusicEnabled)
                            {
                                musicReader.Position = 0;
                                musicOutput.Play();
                            }
                        };
                        musicOutput.Volume = SettingsManager.MusicEnabled ? SettingsManager.MasterVolume : 0f;
                        musicOutput.Play();
                        isMusicPlaying = true;
                    }
                    catch { }
                }
                
                if (File.Exists(shootPath))
                {
                    try
                    {
                        shootReader = new AudioFileReader(shootPath);
                        shootOutput = new WaveOutEvent();
                        shootOutput.Init(shootReader);
                    }
                    catch { }
                }
                
                if (File.Exists(explosionPath))
                {
                    try
                    {
                        explosionReader = new AudioFileReader(explosionPath);
                        explosionOutput = new WaveOutEvent();
                        explosionOutput.Init(explosionReader);
                    }
                    catch { }
                }
                
                UpdateSounds();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sound initialization error: {ex.Message}");
            }
        }
        
        public static void UpdateSounds()
        {
            if (isDisposed) return;
            try
            {
                if (musicOutput != null && musicReader != null)
                {
                    musicOutput.Volume = SettingsManager.MusicEnabled ? SettingsManager.MasterVolume : 0f;
                }
            }
            catch { }
        }
        
        public static void PlayShoot()
        {
            if (isDisposed || !SettingsManager.ShootSoundEnabled || shootReader == null || shootOutput == null) return;
            
            try
            {
                if (shootOutput.PlaybackState == PlaybackState.Playing)
                {
                    shootOutput.Stop();
                }
                shootReader.Position = 0;
                
                shootOutput.Volume = SettingsManager.MasterVolume;
                shootOutput.Play();
            }
            catch 
            {
                try
                {
                    shootReader?.Dispose();
                    shootOutput?.Dispose();
                    
                    string shootPath = Path.Combine(soundsPath, "shoot.wav");
                    if (File.Exists(shootPath))
                    {
                        shootReader = new AudioFileReader(shootPath);
                        shootOutput = new WaveOutEvent();
                        shootOutput.Init(shootReader);
                        shootOutput.Volume = SettingsManager.MasterVolume;
                        shootOutput.Play();
                    }
                }
                catch { }
            }
        }
        
        public static void PlayExplosion()
        {
            if (isDisposed || !SettingsManager.ExplosionSoundEnabled || explosionReader == null || explosionOutput == null) return;
            
            try
            {
                if (explosionOutput.PlaybackState == PlaybackState.Playing)
                {
                    explosionOutput.Stop();
                }
                explosionReader.Position = 0;
                
                explosionOutput.Volume = SettingsManager.MasterVolume;
                explosionOutput.Play();
            }
            catch
            {
                try
                {
                    explosionReader?.Dispose();
                    explosionOutput?.Dispose();
                    
                    string explosionPath = Path.Combine(soundsPath, "explosion.wav");
                    if (File.Exists(explosionPath))
                    {
                        explosionReader = new AudioFileReader(explosionPath);
                        explosionOutput = new WaveOutEvent();
                        explosionOutput.Init(explosionReader);
                        explosionOutput.Volume = SettingsManager.MasterVolume;
                        explosionOutput.Play();
                    }
                }
                catch { }
            }
        }
        
        public static void StopMusic()
        {
            if (isDisposed) return;
            try
            {
                musicOutput?.Stop();
                isMusicPlaying = false;
            }
            catch { }
        }
        
        public static void ResumeMusic()
        {
            if (isDisposed) return;
            try
            {
                if (SettingsManager.MusicEnabled && musicOutput != null && !isMusicPlaying)
                {
                    musicOutput.Play();
                    isMusicPlaying = true;
                }
            }
            catch { }
        }
        
        public static void Dispose()
        {
            if (isDisposed) return;
            isDisposed = true;
            try
            {
                musicOutput?.Stop();
                musicOutput?.Dispose();
                musicReader?.Dispose();
                shootOutput?.Dispose();
                shootReader?.Dispose();
                explosionOutput?.Dispose();
                explosionReader?.Dispose();
            }
            catch { }
        }
    }
}
