using System;
using System.Numerics;
using objects.Monsters.Algorithms;
using simple3d;
using simple3d.Drawing;
using simple3d.Levels;

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
            BlockLeft
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

        private SkeletonState state;

        public Skeleton(
            Animation staticAnimation,
            Animation followingAndBlockingAnimation,
            Animation attackRightAnimation,
            Animation attackLeftAnimation,
            Animation blockRightAnimation,
            Animation blockLeftAnimation,
            Vector2 position, Vector2 size, float directionAngle) : base(position, size, directionAngle, 84)
        {
            this.staticAnimation = staticAnimation;
            this.followingAndBlockingAnimation = followingAndBlockingAnimation;
            this.attackRightAnimation = attackRightAnimation;
            this.attackLeftAnimation = attackLeftAnimation;
            this.blockRightAnimation = blockRightAnimation;
            this.blockLeftAnimation = blockLeftAnimation;

            state = SkeletonState.Static;
        }

        public static Skeleton Create(ResourceCachedLoader loader, Vector2 position, Vector2 size, float directionAngle)
        {
            var staticAnimation = loader.GetAnimation("./animations/skeleton/guard_right");
            var followingAndBlockingAnimation = loader.GetAnimation("./animations/skeleton/guard_right");
            var attackRightAnimation = loader.GetAnimation("./animations/skeleton/right_attack");
            var attackLeftAnimation = loader.GetAnimation("./animations/skeleton/left_attack");
            var blockRightAnimation = loader.GetAnimation("./animations/skeleton/guard_right");
            var blockLeftAnimation = loader.GetAnimation("./animations/skeleton/guard_left");

            return new Skeleton(
                staticAnimation,
                followingAndBlockingAnimation,
                attackRightAnimation,
                attackLeftAnimation,
                blockRightAnimation,
                blockLeftAnimation,
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
                SkeletonState.BLockRight => blockRightAnimation,
                SkeletonState.BlockLeft => blockLeftAnimation,
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
                if(PlayerInAttackDistance(scene))
                    DoLeftMeleeAttackOnPlayer(scene, 2);
                return;
            }
            
            if (state == SkeletonState.AttackRight && GetCurrentAnimation().IsOver)
            {
                SetState(SkeletonState.Static);
                if(PlayerInAttackDistance(scene))
                    DoRightMeleeAttackOnPlayer(scene, 2);
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
            blockRightAnimation.Reset();
            blockLeftAnimation.Reset();
            staticAnimation.Reset();
        }

        public override void OnLeftMeleeAttack(Scene scene, int damage)
        {
            SetState(SkeletonState.BlockLeft);
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
            SetState(SkeletonState.BLockRight);
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