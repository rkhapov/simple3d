using System;
using System.Numerics;
using musics;
using objects.Monsters.Algorithms;
using simple3d;
using simple3d.Drawing;
using simple3d.Levels;
using simple3d.Sounds;

namespace objects.Monsters
{
    public class Skeleton : BaseMonster
    {
        private enum SkeletonState
        {
            Static,
            FollowingAndBlocking,
            AttackLeft,
            AttackRight,
            BLockRight,
            BlockLeft,
            Dead
        }
        protected override int ViewDistance => 20;
        protected override float MoveSpeed => 0.003f;
        public override Animation CurrentAnimation => GetCurrentAnimation();

        private readonly Animation staticAnimation;
        private readonly Animation followingAndBlockingAnimation;
        private readonly Animation attackRightAnimation;
        private readonly Animation attackLeftAnimation;
        private readonly Animation blockRightAnimation;
        private readonly Animation blockLeftAnimation;
        private readonly Animation deadAnimation;
        private readonly ISound hitSound;
        private readonly ISound deathSound;
        private readonly ISound shieldHit;

        private SkeletonState state;

        public Skeleton(
            Animation staticAnimation,
            Animation followingAndBlockingAnimation,
            Animation attackRightAnimation,
            Animation attackLeftAnimation,
            Animation blockRightAnimation,
            Animation blockLeftAnimation,
            Vector2 position, Vector2 size, float directionAngle, Animation deadAnimation, ISound deathSound, ISound hitSound, ISound shieldHit) : base(position, size, directionAngle, 84)
        {
            this.staticAnimation = staticAnimation;
            this.followingAndBlockingAnimation = followingAndBlockingAnimation;
            this.attackRightAnimation = attackRightAnimation;
            this.attackLeftAnimation = attackLeftAnimation;
            this.blockRightAnimation = blockRightAnimation;
            this.blockLeftAnimation = blockLeftAnimation;
            this.deadAnimation = deadAnimation;
            this.deathSound = deathSound;
            this.hitSound = hitSound;
            this.shieldHit = shieldHit;

            state = SkeletonState.Static;
        }

        public static Skeleton Create(ResourceCachedLoader loader, Vector2 position, float directionAngle)
        {
            var staticAnimation = loader.GetAnimation("./animations/skeleton/guard_right");
            var followingAndBlockingAnimation = loader.GetAnimation("./animations/skeleton/moving");
            var attackRightAnimation = loader.GetAnimation("./animations/skeleton/right_attack");
            var attackLeftAnimation = loader.GetAnimation("./animations/skeleton/left_attack");
            var blockRightAnimation = loader.GetAnimation("./animations/skeleton/guard_right");
            var blockLeftAnimation = loader.GetAnimation("./animations/skeleton/guard_left");
            var size = new Vector2(0.3f, 0.3f);
            var dead = loader.GetAnimation("./animations/skeleton/dead");
            var deathSound = loader.GetSound(MusicResourceHelper.SkeletonDeadPath);
            var hitSound = loader.GetSound(MusicResourceHelper.SkeletonHit);
            var shieldHit = loader.GetSound(MusicResourceHelper.SkeletonShieldHit);

            return new Skeleton(
                staticAnimation,
                followingAndBlockingAnimation,
                attackRightAnimation,
                attackLeftAnimation,
                blockRightAnimation,
                blockLeftAnimation,
                position,
                size,
                directionAngle,
                dead,
                deathSound,
                hitSound,
                shieldHit);
        }

        private Animation GetCurrentAnimation()
        {
            return state switch
            {
                SkeletonState.Static => staticAnimation,
                SkeletonState.FollowingAndBlocking => followingAndBlockingAnimation,
                SkeletonState.AttackRight => attackRightAnimation,
                SkeletonState.AttackLeft => attackLeftAnimation,
                SkeletonState.BLockRight => blockRightAnimation,
                SkeletonState.BlockLeft => blockLeftAnimation,
                SkeletonState.Dead => deadAnimation,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private const float AttackDistance = 2f;
        private const float AttackDistanceSquared = AttackDistance * AttackDistance;
        private const float BlockingChance = 0.7f;
        private const int Damage = 10;

        public override void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            base.OnWorldUpdate(scene, elapsedMilliseconds);

            if (state == SkeletonState.Dead)
                return;

            if (state == SkeletonState.AttackLeft && GetCurrentAnimation().IsOver)
            {
                SetState(SkeletonState.Static);
                if(PlayerInAttackDistance(scene))
                    DoLeftMeleeAttackOnPlayer(scene, Damage);
                return;
            }
            
            if (state == SkeletonState.AttackRight && GetCurrentAnimation().IsOver)
            {
                SetState(SkeletonState.Static);
                if(PlayerInAttackDistance(scene))
                    DoRightMeleeAttackOnPlayer(scene, Damage);
                return;
            }

            if ((state == SkeletonState.BLockRight || state ==SkeletonState.BlockLeft) && GetCurrentAnimation().IsOver)
            {
                SetState(IsRightAttack(0.5f) ? SkeletonState.AttackRight : SkeletonState.AttackLeft);
                return;
            }

            if (state == SkeletonState.Static)
            {
                if (PlayerInAttackDistance(scene))
                {
                    SetState(IsRightAttack(0.5f) ? SkeletonState.AttackRight : SkeletonState.AttackLeft);
                    return;
                }

                if (!CanSeePlayer(scene))
                {
                    return;
                }

                scene.EventsLogger.MonsterAttacks("Скелет");

                SetState(SkeletonState.FollowingAndBlocking);
            }

            if (state == SkeletonState.FollowingAndBlocking)
            {
                if (PlayerInAttackDistance(scene))
                    SetState(IsRightAttack(0.5f) ? SkeletonState.AttackRight : SkeletonState.AttackLeft);
                else
                {
                    if (TryGetDirectionToPlayer(scene, out var directionAngle))
                    {
                        DirectionAngle = directionAngle;
                        MoveOnDirection(scene, elapsedMilliseconds);
                    }
                    else
                        SetState(SkeletonState.Static);
                }
            }
        }

        private bool TryGetDirectionToPlayer(Scene scene, out float angle)
        {
            angle = 0.0f;

            var myPoint = MapPoint.FromVector2(Position);
            var playerPoint = MapPoint.FromVector2(scene.Player.Position);
            var path = PathFinder.FindPath(scene.Map, playerPoint, myPoint);

            if (path == null)
                return false;

            angle = GetAngleToPoint(path[1]);

            return true;
        }

        private bool PlayerInAttackDistance(Scene scene)
        {
            return (Position - scene.Player.Position).LengthSquared() < AttackDistanceSquared;
        }

        private bool IsBlockSuccessful(float probability)
        {
            var rand = new Random();
            var result = rand.NextDouble();

            return probability - result >= 0;
        }

        private static readonly Random rand = new Random();
        
        private bool IsRightAttack(float probability)
        {
            var result = rand.NextDouble();

            return probability - result >= 0;
        }

        private void SetState(SkeletonState newState)
        {
            state = newState;
            attackRightAnimation.Reset();
            attackLeftAnimation.Reset();
            followingAndBlockingAnimation.Reset();
            blockRightAnimation.Reset();
            blockLeftAnimation.Reset();
            staticAnimation.Reset();
        }

        public override void OnLeftMeleeAttack(Scene scene, int damage)
        {
            if (state == SkeletonState.Dead)
                return;

            SetState(SkeletonState.BlockLeft);
            if (!IsBlockSuccessful(BlockingChance))
            {
                SetState(SkeletonState.BLockRight);
                DoHit(scene, damage);
            }
            else
                DoDefence(scene);

            if (!IsAlive)
            {
                Die(scene);
            }
        }

        public override void OnRightMeleeAttack(Scene scene, int damage)
        {
            if (state == SkeletonState.Dead)
                return;
            
            SetState(SkeletonState.BLockRight);
            if (!IsBlockSuccessful(BlockingChance))
            {
                DoHit(scene, damage);
                SetState(SkeletonState.BlockLeft);
            }
            else
                DoDefence(scene);

            if (!IsAlive)
            {
                Die(scene);
            }
        }

        private void DoHit(Scene scene, int damage)
        {
            scene.EventsLogger.MonsterHit("Скелет", damage);
            ReceiveDamage(damage);
            hitSound.Play(0);
            hitSound.Play(1);
            hitSound.Play(0);
        }

        private void DoDefence(Scene scene)
        {
            scene.EventsLogger.SuccessfullyDefence("Скелет");
            shieldHit.Play(0);
        }

        private void Die(Scene scene)
        {
            state = SkeletonState.Dead;
            Size = Vector2.Zero;
            deathSound.Play(0);
            scene.EventsLogger.MonsterDeath("Скелет");
        }

        public override void OnShoot(Scene scene, int damage)
        {
            if (state != SkeletonState.AttackLeft && state != SkeletonState.AttackRight)
            {
                DoDefence(scene);
                return;
            }

            if (rand.Next() > 0.5)
            {
                scene.EventsLogger.ArrowMissed("Скелет");
                return;
            }
            
            DoHit(scene, damage);

            if (!IsAlive)
            {
                Die(scene);
            }
        }
    }
}