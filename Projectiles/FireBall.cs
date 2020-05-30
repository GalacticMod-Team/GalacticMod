using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Galactic.Projectiles {
    public class FireBall : ModProjectile {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Fireball");
        }

        public override void SetDefaults() {
            projectile.width = 48;
            projectile.height = 48;
            projectile.timeLeft = 3600;
            projectile.aiStyle = 0;
            projectile.light = 1f;
            projectile.hostile = true;
        }

        // Additional hooks/methods here.
    }
}