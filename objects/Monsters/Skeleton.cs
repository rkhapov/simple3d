using System;
using System.Numerics;
using simple3d;
using simple3d.Drawing;
using simple3d.Levels;

namespace objects.Monsters
{
    public class Skeleton : AnimatedMonster
    {
        private enum SkeletonState
        {
            Static,
            FollowingAndBlocking,
            AttackLeft,
            AttackRight,
            BLock
        }
        protected override int ViewDistance => 20;
        protected override float MoveSpeed => 0.003f;
        public override Animation CurrentAnimation => GetCurrentAnimation();

        private readonly Animation staticAnimation;
        private readonly Animation followingAndBlockingAnimation;
        private readonly Animation attackRightAnimation;
        private readonly Animation attackLeftAnimation;
        private readonly Animation blockAnimation;

        private SkeletonState state;

        public Skeleton(
            Animation staticAnimation,
            Animation followingAndBlockingAnimation,
            Animation attackRightAnimation,
            Animation attackLeftAnimation,
            Animation blockAnimation,
            Vector2 position, Vector2 size, float directionAngle) : base(position, size, directionAngle, 84)
        {
            this.staticAnimation = staticAnimation;
            this.followingAndBlockingAnimation = followingAndBlockingAnimation;
            this.attackRightAnimation = attackRightAnimation;
            this.attackLeftAnimation = attackLeftAnimation;
            this.blockAnimation = blockAnimation;

            state = SkeletonState.Static;
        }

        public static Skeleton Create(ResourceCachedLoader loader, Vector2 position, Vector2 size, float directionAngle)
        {
            var staticAnimation = loader.GetAnimation("./animations/rat/static");
            var followingAndBlockingAnimation = loader.GetAnimation("./animations/rat/moving");
            var attackRightAnimation = loader.GetAnimation("./animations/rat/attack");
            var attackLeftAnimation = loader.GetAnimation("./animations/rat/attack");
            var blockAnimation = loader.GetAnimation("./animations/rat/static");

            return new Skeleton(
                staticAnimation,
                followingAndBlockingAnimation,
                attackRightAnimation,
                attackLeftAnimation,
                blockAnimation,
                position,
                size,
                directionAngle);
        }

        private Animation GetCurrentAnimation()
        {
            return state switch
            {
                SkeletonState.Static => staticAnimation,
                SkeletonState.FollowingAndBlocking => followingAndBlockingAnimation,
                SkeletonState.AttackRight => attackRightAnimation,
                SkeletonState.AttackLeft => attackLeftAnimation,
                SkeletonState.BLock => blockAnimation,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private const float AttackDistance = 2f;
        private const float AttackDistanceSquared = AttackDistance * AttackDistance;
        private const float BlockingChance = 0.7f;

        public override void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            base.OnWorldUpdate(scene, elapsedMilliseconds);

            if (state == SkeletonState.AttackLeft && GetCurrentAnimation().IsOver)
            {
                SetState(SkeletonState.Static);
                DoLeftMeleeAttackOnPlayer(scene, 42);
                return;
            }
            
            if (state == SkeletonState.AttackRight && GetCurrentAnimation().IsOver)
            {
                SetState(SkeletonState.Static);
                DoRightMeleeAttackOnPlayer(scene, 42);
                return;
            }

            if (state == SkeletonState.Static)
            {
                if (PlayerInAttackDistance(scene))
                {
                    SetState(IsRightAttack(0.5f) ? SkeletonState.AttackRight : SkeletonState.AttackLeft);
                    return;
                }

                if (HaveWallOnStraightWayToPlayer(scene))
                {
                    return;
                }

                SetState(SkeletonState.FollowingAndBlocking);
            }

            if (state == SkeletonState.FollowingAndBlocking)
            {
                if (PlayerInAttackDistance(scene))
                    SetState(IsRightAttack(0.5f) ? SkeletonState.AttackRight : SkeletonState.AttackLeft);
                
                else if (HaveWallOnStraightWayToPlayer(scene))
                    SetState(SkeletonState.Static);
                
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

        private bool IsBlockSuccessful(float probability)
        {
            var rand = new Random();
            var result = rand.NextDouble();

            return probability - result >= 0;
        }

        private bool IsRightAttack(float probability)
        {
            var rand = new Random();
            var result = rand.NextDouble();

            return probability - result >= 0;
        }

        private void SetState(SkeletonState newState)
        {
            state = newState;
            attackRightAnimation.Reset();
            attackLeftAnimation.Reset();
            followingAndBlockingAnimation.Reset();
            blockAnimation.Reset();
            staticAnimation.Reset();
        }

        public override void OnLeftMeleeAttack(Scene scene, int damage)
        {
            SetState(SkeletonState.BLock);
            Console.WriteLine("Blocking");
            if (!IsBlockSuccessful(BlockingChance))
            {
                Console.WriteLine($"Block unsuccessful {Health}");
                ReceiveDamage(damage);   
            }

            if (!IsAlive)
                scene.RemoveObject(this);
        }

        public override void OnRightMeleeAttack(Scene scene, int damage)
        {
            SetState(SkeletonState.BLock);
            if(!IsBlockSuccessful(BlockingChance))
                ReceiveDamage(damage);

            
            if (!IsAlive)
                scene.RemoveObject(this);
        }

        public override void OnShoot(Scene scene, int damage)
        {
            if (!IsAlive)
                scene.RemoveObject(this);
        }
    }
}