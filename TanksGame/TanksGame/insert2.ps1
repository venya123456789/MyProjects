$method = @'


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
                isReloading = tank.IsReloading && tank.IsMachinegunMode;
            }
            else
            {
                currentAmmo = tank.RegularAmmo;
                maxAmmo = tank.MaxAmmo;
                isReloading = tank.IsReloading && !tank.IsMachinegunMode;
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

'@

$content = Get-Content "GameForm.cs" -Raw -Encoding UTF8
$content = $content -replace "(\r?\n        private void DrawBuffIndicator\(float x, float y, BuffType buff, bool isPlayer1\))", "$method`r`n`$1"
Set-Content -Path "GameForm.cs" -Value $content -Encoding UTF8 -NoNewline