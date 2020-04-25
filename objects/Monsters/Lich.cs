using System;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using musics;
using objects.Monsters.Algorithms;
using objects.Weapons;
using simple3d;
using simple3d.Drawing;
using simple3d.Levels;
using simple3d.Sounds;

namespace objects.Monsters
{
    public class Lich : BaseMonster
    {
        private enum LichState
        {
            Static,
            Dead,
            Shooting,
            RunningFromPlayer
        }

        private readonly Animation staticAnimation;
        private readonly Animation deadAnimation;
        private readonly Animation shootingAnimation;
        private readonly Animation runningAnimation;

        private readonly Animation fireballAnimation;
        private readonly Animation fireballBlowing;
        private readonly ISound fireballBlowSound;

        private LichState state;

        public Lich(Vector2 position, Vector2 size, float directionAngle, Animation staticAnimation,
            Animation deadAnimation, Animation shootingAnimation, Animation runningAnimation, Animation fireballAnimation, Animation fireballBlowing, ISound fireballBlowSound) : base(position, size,
            directionAngle, 42)
        {
            this.staticAnimation = staticAnimation;
            this.deadAnimation = deadAnimation;
            this.shootingAnimation = shootingAnimation;
            this.runningAnimation = runningAnimation;
            this.fireballAnimation = fireballAnimation;
            this.fireballBlowing = fireballBlowing;
            this.fireballBlowSound = fireballBlowSound;
            this.state = LichState.Static;
        }

        public static Lich Create(ResourceCachedLoader loader, Vector2 position, Vector2 size, float direction)
        {
            var staticAnimation = loader.GetAnimation("./animations/lich/static");
            var deadAnimation = loader.GetAnimation("./animations/lich/dead");
            var shootingAnimation = loader.GetAnimation("./animations/lich/shooting");
            var runningAnimation = loader.GetAnimation("./animations/lich/running");
            var fireBallAnimation = loader.GetAnimation("./animations/fireball/moving");
            var fireBallBlowing = loader.GetAnimation("./animations/fireball/blow");
            var fireBallBlowSound = loader.GetSound(MusicResourceHelper.FireBallBlowPath);

            return new Lich(
                position, size, direction,
                staticAnimation,
                deadAnimation,
                shootingAnimation,
                runningAnimation,
                fireBallAnimation,
                fireBallBlowing,
                fireBallBlowSound);
        }

        public override void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            base.OnWorldUpdate(scene, elapsedMilliseconds);

            if (state == LichState.Dead)
                return;

            if (!IsAlive)
            {
                SetState(LichState.Dead);
                Size = Vector2.Zero;
                return;
            }

            if (state == LichState.Shooting && GetCurrentAnimation().IsOver)
            {
                scene.AddObject(new FireBall(Position,
                    new Vector2(0.1f, 0.1f), 0, 3000, scene.Player,
                    fireballAnimation.GetClearCopy(), fireballBlowing.GetClearCopy(), fireballBlowSound));
                SetState(LichState.RunningFromPlayer);
                return;
            }

            if (state == LichState.Static)
            {
                if (CanSeePlayer(scene))
                {
                    SetState(LichState.RunningFromPlayer);
                }
            }

            if (state == LichState.RunningFromPlayer)
            {
                if (CanShoot(scene))
                {
                    SetState(LichState.Shooting);
                }
                else
                {
                    if (!TryRunFromPlayer(scene, elapsedMilliseconds))
                        SetState(LichState.Shooting);
                }
            }
        }

        private const int ShootingDistance = 10;

        private bool CanShoot(Scene scene)
        {
            var myPoint = MapPoint.FromVector2(Position);
            var playerPoint = MapPoint.FromVector2(scene.Player.Position);

            return myPoint.GetManhattanDistanceTo(playerPoint) >= ShootingDistance;
        }

        private bool TryRunFromPlayer(Scene scene, float elapsedMilliseconds)
        {
            var myPoint = MapPoint.FromVector2(Position);
            var playerPoint = MapPoint.FromVector2(scene.Player.Position);
            var pointsToRun = playerPoint
                .GetPointsAtRadius(scene.Map, ShootingDistance + 5)
                .Where(p => p.GetManhattanDistanceTo(playerPoint) >= ShootingDistance)
                .OrderBy(p => p.GetManhattanDistanceTo(myPoint))
                .ToArray();

            if (pointsToRun.Length < 1)
                return false;

            var pathToPoint = PathFinder.FindPath(scene.Map, pointsToRun[0], myPoint);

            if (pathToPoint == null)
            {
                return false;
            }

            DirectionAngle = GetAngleToPoint(pathToPoint[1]);

            MoveOnDirection(scene, elapsedMilliseconds);

            return true;
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

        protected override int ViewDistance => 15;
        protected override float MoveSpeed => 0.003f;
        public override Animation CurrentAnimation => GetCurrentAnimation();

        private void SetState(LichState newState)
        {
            state = newState;

            staticAnimation.Reset();
            deadAnimation.Reset();
            runningAnimation.Reset();
            shootingAnimation.Reset();
        }

        private Animation GetCurrentAnimation()
        {
            return state switch
            {
                LichState.Static => staticAnimation,
                LichState.Dead => deadAnimation,
                LichState.Shooting => shootingAnimation,
                LichState.RunningFromPlayer => runningAnimation,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}