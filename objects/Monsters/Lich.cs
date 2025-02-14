﻿using System;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using musics;
using objects.Monsters.Algorithms;
using objects.Weapons;
using simple3d;
using simple3d.Drawing;
using simple3d.Levels;
using simple3d.MathUtils;
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
        private readonly ISound evilLaugh;
        private readonly ISound deathSound;

        private LichState state;

        public Lich(Vector2 position, Vector2 size, float directionAngle, Animation staticAnimation,
            Animation deadAnimation, Animation shootingAnimation, Animation runningAnimation, Animation fireballAnimation, Animation fireballBlowing, ISound fireballBlowSound, ISound evilLaugh, ISound deathSound) : base(position, size,
            directionAngle, 120)
        {
            this.staticAnimation = staticAnimation;
            this.deadAnimation = deadAnimation;
            this.shootingAnimation = shootingAnimation;
            this.runningAnimation = runningAnimation;
            this.fireballAnimation = fireballAnimation;
            this.fireballBlowing = fireballBlowing;
            this.fireballBlowSound = fireballBlowSound;
            this.evilLaugh = evilLaugh;
            this.deathSound = deathSound;
            this.state = LichState.Static;
        }

        public static Lich Create(Vector2 position, float direction)
        {
            return Create(ResourceCachedLoader.Instance, position, direction);
        }

        public static Lich Create(ResourceCachedLoader loader, Vector2 position, float direction)
        {
            var staticAnimation = loader.GetAnimation("./animations/lich/static");
            var deadAnimation = loader.GetAnimation("./animations/lich/dead");
            var shootingAnimation = loader.GetAnimation("./animations/lich/shooting");
            var runningAnimation = loader.GetAnimation("./animations/lich/running");
            var fireBallAnimation = loader.GetAnimation("./animations/fireball/moving");
            var fireBallBlowing = loader.GetAnimation("./animations/fireball/blow");
            var fireBallBlowSound = loader.GetSound(MusicResourceHelper.FireBallBlowPath);
            var laugh = loader.GetSound(MusicResourceHelper.LichEvilLaughPath);
            var deathSound = loader.GetSound(MusicResourceHelper.LichDeadPath);
            var size = new Vector2(0.3f, 0.3f);

            return new Lich(
                position, size, direction,
                staticAnimation,
                deadAnimation,
                shootingAnimation,
                runningAnimation,
                fireBallAnimation,
                fireBallBlowing,
                fireBallBlowSound,
                laugh,
                deathSound);
        }

        private static readonly Random random = new Random();
        private const string LichName = "Лич";

        public override void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            base.OnWorldUpdate(scene, elapsedMilliseconds);

            if (state == LichState.Dead)
                return;

            if (!IsAlive)
            {
                SetState(LichState.Dead);
                Size = Vector2.Zero;
                deathSound.Play(0);
                scene.EventsLogger.MonsterDeath(LichName);
                return;
            }

            if (state == LichState.Shooting && GetCurrentAnimation().IsOver)
            {
                var fireBallProbability = CanSeePlayer(scene) ? 0.2 : 1;

                if (random.NextDouble() < fireBallProbability)
                {
                    SpawnFireBall(scene);
                }
                else
                {
                    SpawnShockBall(scene);
                }

                SetState(LichState.RunningFromPlayer);
                return;
            }

            if (state == LichState.Static)
            {
                if (CanSeePlayer(scene))
                {
                    scene.EventsLogger.MonsterAttacks(LichName);
                    evilLaugh.Play(0);
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

        private void SpawnShockBall(Scene scene)
        {
            var dd = scene.Player.Position - Position;
            var angle = MathF.Atan2(dd.Y, dd.X);

            scene.AddObject(ShockBall.Create(
                Position, new Vector2(0.1f, 0.1f), angle, this));
        }

        private void SpawnFireBall(Scene scene)
        {
            scene.AddObject(new FireBall(Position,
                new Vector2(0.1f, 0.1f), 0, 6000, scene.Player,
                fireballAnimation.GetClearCopy(), fireballBlowing.GetClearCopy(), fireballBlowSound));
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
            {
                Console.WriteLine("There is no points to Run1");
                return false;
            }

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
            DoReceiveDamage(scene, damage);
        }

        public override void OnRightMeleeAttack(Scene scene, int damage)
        {
            DoReceiveDamage(scene, damage);
        }

        public override void OnShoot(Scene scene, int damage)
        {
            DoReceiveDamage(scene, damage);
        }

        private void DoReceiveDamage(Scene scene, int damage)
        {
            scene.EventsLogger.MonsterHit("Лич", damage);
            ReceiveDamage(damage);
        }
        
        protected override int ViewDistance => 30;
        protected override float MoveSpeed => 0.004f;
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