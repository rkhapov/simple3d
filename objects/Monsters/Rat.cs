using System;
using System.Numerics;
using musics;
using simple3d;
using simple3d.Drawing;
using simple3d.Levels;
using simple3d.Sounds;

namespace objects.Monsters
{
    public class Rat : AnimatedMonster
    {
        private enum RatState
        {
            Static,
            PlayerFollowing,
            Attack,
            Dead
        }

        public override Animation CurrentAnimation => GetCurrentAnimation();

        private readonly Animation staticAnimation;
        private readonly Animation playerFollowing;
        private readonly Animation attackAnimation;
        private readonly Animation deadAnimation;

        private readonly ISound deathSound;
        private readonly ISound attackSound;
        private readonly ISound hitSound;

        private RatState state;

        public Rat(
            Animation staticAnimation,
            Animation playerFollowing,
            Animation attackAnimation,
            Vector2 position, Vector2 size, float directionAngle, ISound deathSound, Animation deadAnimation, ISound attackSound, ISound hitSound) : base(
            position, size, directionAngle, 84)
        {
            this.staticAnimation = staticAnimation;
            this.playerFollowing = playerFollowing;
            this.attackAnimation = attackAnimation;
            this.deathSound = deathSound;
            this.deadAnimation = deadAnimation;
            this.attackSound = attackSound;
            this.hitSound = hitSound;

            state = RatState.Static;
        }

        public static Rat Create(ResourceCachedLoader loader, Vector2 position, Vector2 size, float directionAngle)
        {
            var staticAnimation = loader.GetAnimation("./animations/rat/static");
            var playerFollowerAnimation = loader.GetAnimation("./animations/rat/moving");
            var attackAnimation = loader.GetAnimation("./animations/rat/attack");
            var deathSound = loader.GetSound(MusicResourceHelper.RatDeathSoundPath);
            var deadAnimation = loader.GetAnimation("./animations/rat/dead");
            var attackSound = loader.GetSound(MusicResourceHelper.RatAttackPath);
            var hitSound = loader.GetSound(MusicResourceHelper.RatHitPath);

            return new Rat(
                staticAnimation,
                playerFollowerAnimation,
                attackAnimation,
                position,
                size,
                directionAngle,
                deathSound,
                deadAnimation,
                attackSound,
                hitSound);
        }

        private Animation GetCurrentAnimation()
        {
            return state switch
            {
                RatState.Static => staticAnimation,
                RatState.PlayerFollowing => playerFollowing,
                RatState.Attack => attackAnimation,
                RatState.Dead => deadAnimation,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private const float AttackDistance = 2f;
        private const float AttackDistanceSquared = AttackDistance * AttackDistance;

        public override void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            base.OnWorldUpdate(scene, elapsedMilliseconds);

            if (state == RatState.Dead)
            {
                return;
            }

            if (!IsAlive)
            {
                SetState(RatState.Dead);
                Size = Vector2.Zero;
                return;
            }

            if (state == RatState.Attack && GetCurrentAnimation().IsOver)
            {
                SetState(RatState.Static);
                return;
            }

            if (state == RatState.Static)
            {
                if (PlayerInAttackDistance(scene))
                {
                    SetState(RatState.Attack);
                    DoLeftMeleeAttackOnPlayer(scene, 5);
                    return;
                }

                if (HaveWallOnStraightWayToPlayer(scene))
                {
                    return;
                }

                SetState(RatState.PlayerFollowing);
            }

            if (state == RatState.PlayerFollowing)
            {
                if (PlayerInAttackDistance(scene))
                {
                    SetState(RatState.Attack);
                    DoLeftMeleeAttackOnPlayer(scene, 5);
                }
                else if (HaveWallOnStraightWayToPlayer(scene))
                    SetState(RatState.Static);
                else
                {
                    var fromPlayerToUs = scene.Player.Position - Position;
                    DirectionAngle = MathF.Atan2(fromPlayerToUs.Y, fromPlayerToUs.X);
                    MoveOnDirection(scene, elapsedMilliseconds);
                }
            }
        }

        private bool PlayerInAttackDistance(Scene scene)
        {
            return (Position - scene.Player.Position).LengthSquared() < AttackDistanceSquared;
        }

        private void SetState(RatState newState)
        {
            state = newState;

            if (state == RatState.Attack)
            {
                attackSound.Play(0);
            }

            attackAnimation.Reset();
            playerFollowing.Reset();
            staticAnimation.Reset();
        }

        public override void OnLeftMeleeAttack(Scene scene, int damage)
        {
            DoReceiveDamage(damage);
        }

        private void DoReceiveDamage(int damage)
        {
            ReceiveDamage(damage);
            PlayHitSound();
        }

        public override void OnRightMeleeAttack(Scene scene, int damage)
        {
            DoReceiveDamage(damage);
        }

        public override void OnShoot(Scene scene, int damage)
        {
            DoReceiveDamage(damage);
        }

        private void PlayHitSound()
        {
            if (!IsAlive)
            {
                deathSound.Play(0);
            }
            else
            {
                hitSound.Play(0);
            }
        }

        protected override int ViewDistance => 15;
        protected override float MoveSpeed => 0.003f;
    }
}