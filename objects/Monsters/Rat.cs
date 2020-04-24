using System;
using System.Numerics;
using simple3d;
using simple3d.Drawing;
using simple3d.Levels;
using simple3d.MathUtils;

namespace objects.Monsters
{
    public class Rat : AnimatedMonster
    {
        private enum RatState
        {
            Static,
            PlayerFollowing,
            Attack
        }

        public override Animation CurrentAnimation => GetCurrentAnimation();

        private readonly Animation staticAnimation;
        private readonly Animation playerFollowing;
        private readonly Animation attackAnimation;

        private RatState state;

        public Rat(
            Animation staticAnimation,
            Animation playerFollowing,
            Animation attackAnimation,
            Vector2 position, Vector2 size, float directionAngle) : base(position, size, directionAngle, 84)
        {
            this.staticAnimation = staticAnimation;
            this.playerFollowing = playerFollowing;
            this.attackAnimation = attackAnimation;

            state = RatState.Static;
        }

        public static Rat Create(ResourceCachedLoader loader, Vector2 position, Vector2 size, float directionAngle)
        {
            var staticAnimation = loader.GetAnimation("./animations/rat/static");
            var playerFollowerAnimation = loader.GetAnimation("./animations/rat/moving");
            var attackAnimation = loader.GetAnimation("./animation/rat/attack");

            return new Rat(
                staticAnimation,
                playerFollowerAnimation,
                attackAnimation,
                position,
                size,
                directionAngle);
        }

        private Animation GetCurrentAnimation()
        {
            return state switch
            {
                RatState.Static => staticAnimation,
                RatState.PlayerFollowing => playerFollowing,
                RatState.Attack => attackAnimation,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private const float AttackDistance = 0.3f;
        private const float AttackDistanceSquared = AttackDistance * AttackDistance;

        public override void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            base.OnWorldUpdate(scene, elapsedMilliseconds);

            if (state == RatState.Attack && GetCurrentAnimation().IsOver)
            {
                SetState(RatState.Static);
                return;
            }

            if (state == RatState.Attack)
            {
                return;
            }

            if (state == RatState.Static)
            {
                if (HaveWallOnStraightWayToPlayer(scene))
                    return;

                if ((Position - scene.Player.Position).LengthSquared() < AttackDistanceSquared)
                {
                    SetState(RatState.Attack);
                    return;
                }

                DirectionAngle = this.GetAngleToPlayer(scene.Player);
                SetState(RatState.PlayerFollowing);
            }

            if (state == RatState.PlayerFollowing)
            {
                MoveOnDirection(scene, elapsedMilliseconds);
            }
        }

        private void SetState(RatState newState)
        {
            state = newState;
            attackAnimation.Reset();
            playerFollowing.Reset();
            staticAnimation.Reset();
        }

        public override void OnLeftMeleeAttack(Scene scene, int damage)
        {
            ReceiveDamage(damage);
        }

        public override void OnRightMeleeAttack(Scene scene, int damage)
        {
            ReceiveDamage(damage);
        }

        public override void OnShoot(Scene scene, int damage)
        {
            ReceiveDamage(damage);
        }

        protected override int ViewDistance => 5;
        protected override float MoveSpeed => 0.003f;
    }
}