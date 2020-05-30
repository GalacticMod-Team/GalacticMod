using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace Galactic.Projectiles {
    public class Firenado : ModProjectile {

        private int time = 360;

        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Firenado Bolt");
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults() {
            projectile.width = 160;
            projectile.height = 42;
            projectile.timeLeft = 3600;
            projectile.aiStyle = -1;
            projectile.light = 1f;
            projectile.hostile = true;

        }

        public override void AI() {
            if (++projectile.frameCounter >= 5) {
                projectile.frameCounter = 0;
                if (++projectile.frame >= 6) {
                    projectile.frame = 0;
                }
            }


            double waveX = projectile.ai[0] * Math.PI / 180;

            if (projectile.ai[0] >= 0) {
                projectile.position.X += (float)Math.Sin(waveX) * projectile.ai[1] / 10;

                if (projectile.ai[0] > time) {
                    projectile.ai[0] = 0;
                } else {
                    projectile.ai[0] += 4;
                }
            }
        }
    }
}