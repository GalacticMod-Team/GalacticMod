using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System;
//! note make robobee
namespace Galactic.NPCs.Bosses {
    [AutoloadBossHead]
    public class HellfireDragon : ModNPC {
        // AI variables
        private int attackTimer;
        private bool dashing = false;
        private bool stunned;
        private int stunnedTimer;
        private int frame = 0;
        private double counting;

        private int hoverHeight = 350;
        private int hoverWidth = 500;
        private int speedSlow = 20;
        private int speedFast = 25;
        private float bulletSpeed = 14.5f;
        private bool left = true;

        private enum phases {
            stunned = 600,
            shooting = 800,
            dashing = 1200,
        }


        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Hellfire Dragon");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults() {
            npc.width = 100;
            npc.height = 54;
            drawOffsetY = npc.height - npc.width;
            npc.scale = 3;

            npc.boss = true;
            npc.aiStyle = -1;
            npc.npcSlots = 5;
            npc.lifeMax = 100000;
            npc.damage = 30;
            npc.defense = 30;
            npc.knockBackResist = 0f;

            npc.lavaImmune = true;
            npc.noTileCollide = true;
            npc.noGravity = true;

            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            music = MusicID.Boss2;

            npc.value = Item.buyPrice(gold: 10);
            bossBag = mod.ItemType("HellfireDragonBag");
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale) {
            npc.lifeMax = (int)(npc.lifeMax * bossLifeScale);
            npc.damage = (int)(npc.damage * 1.3);
        }

        public override void AI() {
            npc.TargetClosest(true);
            Player player = Main.player[npc.target];
            Vector2 target = npc.HasPlayerTarget ? player.Center : Main.npc[npc.target].Center;
            npc.rotation = 0.0f;
            npc.netAlways = true;
            npc.TargetClosest(true);

            if (!Despawning(player)) {
                int distance = (int)Vector2.Distance(target, npc.Center);
                if (npc.ai[0] < (float)phases.shooting) {
                    Shooting(target, player, distance);
                } else if (npc.ai[0] >= (float)phases.shooting) {
                    Dashing(target, player, distance);
                }
                IncrementAI();
                npc.netUpdate = true;
            }
        }

        private bool Despawning(Player player) {
            // handles despawning
            if (npc.target < 0 || npc.target == 255 || player.dead || !player.active) {
                npc.TargetClosest(false);
                npc.direction = 1;
                npc.velocity.Y = npc.velocity.Y - 0.1f;
                if (npc.timeLeft > 20) {
                    npc.timeLeft = 20;
                }
                return true;
            }
            return false;
        }

        private void Shooting(Vector2 target, Player player, int distance) {
            // reduce movement is stunned
            if (stunned) {
                npc.velocity.X = 0;
                npc.velocity.Y = 0;
                stunnedTimer++;
                if (stunnedTimer >= 100) {
                    stunned = false;
                    stunnedTimer = 0;
                }
                if (npc.ai[0] % 10 == 0) {
                    ShootTowards(npc.Center, target, 0, mod.ProjectileType("FireBall"));
                }
            }

            // somtimes shoot a volley
            if (npc.ai[0] >= (float)phases.stunned) {
                stunned = true;
            } else {
                if (npc.ai[0] % 30 == 0) {
                    ShootVolley(npc.Center, target, 5, 20, mod.ProjectileType("FireBall"));
                }
            }

            // somtimes shoot a tornado bolt
            if (npc.ai[1] % 4 == 1 && npc.ai[0] == (float)phases.stunned) {
                ShootTowards(npc.Center, target, 0, mod.ProjectileType("FirenadoBolt"));
            }

            // hover above player
            target.Y -= hoverHeight;
            target.X -= (left ? -hoverWidth : hoverWidth);
            MoveTowards(npc, target, (distance > 300 ? speedFast : speedSlow), 30f);
        }

        private void ChangeSide() {
            left = !left;
        }
        private void Dashing(Vector2 target, Player player, int distance) {
            if (npc.ai[0] == (float)phases.dashing) {
                // change side to hover at
                ChangeSide();
                // dash attack
                stunned = false;
            }

            if (dashing) {
                // check if we stop dashing
                if (npc.ai[0] % 45 == 0) {
                    dashing = false;
                    ChangeSide();
                }
            } else {
                // prepare to dash
                target.X -= (left ? -hoverWidth : hoverWidth) * (float)1.7;
                MoveTowards(npc, target, ((int)Vector2.Distance(target, npc.Center) > 300 ? speedFast : speedSlow), 30f);
                if (npc.ai[0] % 160 == 0) {
                    dashing = true;
                    Vector2 vector = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float x = player.position.X + (player.width / 2) - vector.X;
                    float y = player.position.Y + (player.width / 2) - vector.Y;
                    y = 0;
                    double distance2 = Math.Sqrt(x * x + y * y);
                    float factor = (float)speedFast / distance;
                    npc.velocity.X = x * factor;
                    npc.velocity.Y = y * factor;
                }
            }
        }

        private void IncrementAI() {
            if (npc.ai[0] >= (float)phases.dashing) {
                npc.ai[0] = 0;
                npc.ai[1]++;
                ChangeSide();
            } else {
                npc.ai[0]++;
            }
        }

        public override void FindFrame(int frameHeight) {
            npc.frame.Y = 0;
            npc.frame.Height = 54;
        }

        private void ShootVolley(Vector2 origin, Vector2 target, int shots, int angle, int projectile) {
            if (shots % 2 == 0) {
                // if even amount of shots

            } else {
                // odd amount of shots
                ShootTowards(origin, target, 0, projectile);
                for (int i = 1; i < Math.Floor((decimal)(shots / 2)) + 1; i++) {
                    ShootTowards(origin, target, i * angle, projectile);
                    ShootTowards(origin, target, i * -angle, projectile);
                }
            }
        }

        private void ShootTowards(Vector2 origin, Vector2 target, int offset, int projectile) {
            Vector2 aimVel = target - origin;
            aimVel.Normalize();
            var theta = offset * Math.PI / 180;
            var cs = Math.Cos(theta);
            var sn = Math.Sin(theta);
            Vector2 shootVel = new Vector2((float)(aimVel.X * cs - aimVel.Y * sn), (float)(aimVel.X * sn + aimVel.Y * cs));
            shootVel *= bulletSpeed;
            Projectile.NewProjectile(origin.X, origin.Y, shootVel.X, shootVel.Y, projectile, npc.damage, 5);
        }

        private void MoveTowards(NPC npc, Vector2 target, float speed, float turnResistance) {
            var move = target - npc.Center;
            float length = move.Length();
            if (length > speed) {
                move *= speed / length;
            }
            move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
            length = move.Length();
            if (length > speed) {
                move *= speed / length;
            }
            npc.velocity = move;
        }

        public override void BossLoot(ref string name, ref int potionType) {
            potionType = ItemID.HealingPotion;
        }


    }
}
