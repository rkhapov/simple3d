using System;
using System.Numerics;
using musics;
using objects.Monsters;
using objects.Monsters.Algorithms;
using simple3d;
using simple3d.Drawing;
using simple3d.Levels;
using simple3d.Sounds;

namespace objects.Weapons
{
    public class FireBall : Bullet
    {
        private enum FireBallState
        {
            Blowing,
            PlayerFollowing
        }

        private readonly Animation movingAnimation;
        private readonly Animation blowingAnimation;
        private readonly ISound blowSound;

        private float lifeTimeMilliseconds;
        private FireBallState state;

        public FireBall(
            Vector2 position,
            Vector2 size,
            float directionAngle,
            float lifeTimeMilliseconds,
            IMapObject target,
            Animation movingAnimation, Animation blowingAnimation, ISound blowSound) : base(position, size, directionAngle)
        {
            this.lifeTimeMilliseconds = lifeTimeMilliseconds;
            Target = target;
            this.movingAnimation = movingAnimation;
            this.blowingAnimation = blowingAnimation;
            this.blowSound = blowSound;
            state = FireBallState.PlayerFollowing;
        }

        public IMapObject Target { get; }

        private const int Damage = 10;

        public override void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            base.OnWorldUpdate(scene, elapsedMilliseconds);

            if (state == FireBallState.Blowing && CurrentAnimation.IsOver)
            {
                scene.RemoveObject(this);
                return;
            }

            if (state == FireBallState.Blowing)
                return;

            lifeTimeMilliseconds -= elapsedMilliseconds;

            if (lifeTimeMilliseconds < 0)
            {
                DoBlow();
                return;
            }

            if (!TryGetDirectionToTarget(scene, out var angle))
            {
                DoBlow();
                return;
            }

            if (ShouldBlow())
            {
                Target.OnShoot(scene, Damage);
                DoBlow();
                return;
            }

            DirectionAngle = angle;
            MoveOnDirection(scene, elapsedMilliseconds, MovingFlags.IgnoreObjects);
        }

        public static FireBall Create(Vector2 position, float lifeTimeMilliseconds, IMapObject target)
        {
            var loader = ResourceCachedLoader.Instance;
            var moving = loader.GetAnimation("./animations/fireball/moving");
            var blow = loader.GetAnimation("./animations/fireball/blow");
            var blowSound = loader.GetSound(MusicResourceHelper.FireBallBlowPath);

            return new FireBall(
                position,
                new Vector2(0f, 0f),
                0,
                lifeTimeMilliseconds,
                target,
                moving,
                blow,
                blowSound);
        }

        private void DoBlow()
        {
            blowSound.Play(0);
            state = FireBallState.Blowing;
        }

        private const float BlowDistance = 2f;
        private const float BlowDistanceSquared = BlowDistance * BlowDistance;

        private bool ShouldBlow()
        {
            return (Position - Target.Position).LengthSquared() < BlowDistanceSquared;
        }

        public override void OnLeftMeleeAttack(Scene scene, int damage)
        {
            //nothing
        }

        public override void OnRightMeleeAttack(Scene scene, int damage)
        {
            //nothing
        }

        public override void OnShoot(Scene scene, int damage)
        {
            //nothing
        }

        private bool TryGetDirectionToTarget(Scene scene, out float angle)
        {
            angle = 0.0f;

            var myPoint = MapPoint.FromVector2(Position);
            Console.WriteLine($"{myPoint}");
            var targetPoint = MapPoint.FromVector2(Target.Position);
            Console.WriteLine($"{targetPoint}");
            var path = PathFinder.FindPath(scene.Map, targetPoint, myPoint);
            
            if (path == null)
                return false;

            Console.WriteLine("before");
            angle = GetAngleToPoint(path[1]);
            Console.WriteLine("after");

            return true;
        }


        protected override int ViewDistance => 20;
        protected override float MoveSpeed => 0.003f;

        public override Animation CurrentAnimation
        {
            get
            {
                return state switch
                {
                    FireBallState.Blowing => blowingAnimation,
                    FireBallState.PlayerFollowing => movingAnimation,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
    }
}