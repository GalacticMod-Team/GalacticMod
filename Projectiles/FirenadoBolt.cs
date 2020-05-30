using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Threading.Tasks;

namespace Galactic.Projectiles {
    public class FirenadoBolt : ModProjectile {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Firenado Bolt");
        }

        public override void SetDefaults() {
            projectile.width = 48;
            projectile.height = 48;
            projectile.timeLeft = 3600;
            projectile.aiStyle = 0;
            projectile.light = 1f;
            projectile.hostile = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            // summon a tornado
            SummonWave(projectile.position);
            return true;
        }


        private async void SummonWave(Vector2 position) {
            float width = 160f;
            float height = 42;
            float newHeight = height * 0.6f;
            float newWidth = width * 0.6f;
            float baseHeight = (float)(newHeight * 2 + (16 * (Math.Floor(position.Y / 16))));
            int[] projectiles = new int[10];
            for (int i = 1; i <= 10; i++) {
                Main.NewText(i, Color.Red);
                var p = Projectile.NewProjectile(position.X - width / 2, baseHeight, 0, 0, mod.ProjectileType("Firenado"), projectile.damage, 5, projectile.owner);
                projectiles[i - 1] = p;
                Main.projectile[p].scale = 0.1f * i + 0.6f;
                Main.projectile[p].ai[1] = (0.4f * i + 0.8f) * 10f;
                Main.projectile[p].ai[1] = (2) * 10f;
                Main.projectile[p].ai[0] = -1;
                baseHeight -= (i * 0.1f + 0.6f) * height;
                newWidth = (i * 0.1f + 0.6f) * width;
                await Task.Delay(50);
            }
            for (int i = 0; i < 10; i++) {
                Main.projectile[projectiles[i]].ai[0] = i * 5;
            }
        }

        // Additional hooks/methods here.
    }
}