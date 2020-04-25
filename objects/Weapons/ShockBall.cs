using System;
using System.Linq;
using System.Numerics;
using musics;
using simple3d;
using simple3d.Drawing;
using simple3d.Levels;
using simple3d.MathUtils;
using simple3d.Sounds;

namespace objects.Weapons
{
    public class ShockBall : Bullet
    {
        private enum ShockBallState
        {
            Moving,
            Blow
        }

        private readonly Animation movingAnimation;
        private readonly Animation blowAnimation;
        private readonly IMapObject master;
        private readonly ISound blowSound;
        private ShockBallState state;

        public ShockBall(Vector2 position, Vector2 size, float directionAngle, Animation movingAnimation, Animation blowAnimation, IMapObject master, ISound blowSound) : base(position, size, directionAngle)
        {
            this.movingAnimation = movingAnimation;
            this.blowAnimation = blowAnimation;
            this.master = master;
            this.blowSound = blowSound;
            state = ShockBallState.Moving;
            movingAnimation.Reset();
            blowAnimation.Reset();
        }

        private const int Damage = 5;

        public override void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            base.OnWorldUpdate(scene, elapsedMilliseconds);

            if (state == ShockBallState.Blow && CurrentAnimation.IsOver)
            {
                scene.RemoveObject(this);
                return;
            }

            if (state == ShockBallState.Blow)
                return;

            var xRayUnit = MathF.Cos(DirectionAngle);
            var yRayUnit = MathF.Sin(DirectionAngle);
            var dx = xRayUnit * MoveSpeed * elapsedMilliseconds;
            var dy = yRayUnit * MoveSpeed * elapsedMilliseconds;
            var newPosition = Position + new Vector2(dx, dy);
            var testX = (int) newPosition.X;
            var testY = (int) newPosition.Y;
            var hit = !scene.Map.InBound(testY, testX) || scene.Map.At(testY, testX).Type != MapCellType.Empty;

            foreach (var obj in scene.Objects.Append(scene.Player))
            {
                if (obj is Bullet || obj is Arrow || obj == master)
                    continue;

                if (!CanDamage(obj))
                    continue;

                obj.OnShoot(scene, Damage);
                hit = true;
            }

            if (hit)
            {
                state = ShockBallState.Blow;
                blowSound.Play(0);
                return;
            }

            Position = newPosition;
        }

        private const float DamageDistance = 1f;
        private const float DamageDistanceSquared = DamageDistance * DamageDistance;

        private bool CanDamage(IMapObject @object)
        {
            return !@object.IsZeroSized() && (@object.Position - Position).LengthSquared() < DamageDistanceSquared;
        } 

        public override void OnLeftMeleeAttack(Scene scene, int damage)
        {
        }

        public override void OnRightMeleeAttack(Scene scene, int damage)
        {
        }

        public override void OnShoot(Scene scene, int damage)
        {
        }

        protected override int ViewDistance => 15;
        protected override float MoveSpeed => 0.006f;

        public override Animation CurrentAnimation
        {
            get
            {
                return state switch
                {
                    ShockBallState.Moving => movingAnimation,
                    ShockBallState.Blow => blowAnimation,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        public static ShockBall Create(Vector2 position, Vector2 size, float directionAngle, IMapObject master)
        {
            var movingAnimation = ResourceCachedLoader.Instance.GetAnimation("./animations/shock/moving");
            var blowAnimation = ResourceCachedLoader.Instance.GetAnimation("./animations/shock/blow");
            var blowSound = ResourceCachedLoader.Instance.GetSound(MusicResourceHelper.ShockBlowPath);

            return new ShockBall(position, size, directionAngle, movingAnimation, blowAnimation, master, blowSound);
        }
    }
}