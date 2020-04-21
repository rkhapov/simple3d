using System;
using System.Numerics;
using simple3d.Drawing;
using simple3d.MathUtils;

namespace simple3d.Levels
{
    public abstract class ShootingWeapon : Weapon
    {
        private readonly Animation staticAnimation;
        private readonly Animation movingAnimation;
        private readonly Animation shootingAnimation;

        private ShootingWeaponState state;

        protected ShootingWeapon(
            Animation staticAnimation,
            Animation movingAnimation,
            Animation shootingAnimation)
        {
            this.staticAnimation = staticAnimation;
            this.movingAnimation = movingAnimation;
            this.shootingAnimation = shootingAnimation;

            state = ShootingWeaponState.Static;
        }

        public ShootingWeaponState State
        {
            get => state;
            set
            {
                if (state == value)
                    return;

                staticAnimation.Reset();
                movingAnimation.Reset();
                shootingAnimation.Reset();

                state = value;
            }
        }

        private Animation GetCurrentAnimation()
        {
            return state switch
            {
                ShootingWeaponState.Static => staticAnimation,
                ShootingWeaponState.Shooting => shootingAnimation,
                ShootingWeaponState.Moving => movingAnimation,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public override Sprite Sprite => GetCurrentAnimation().CurrentFrame;

        public override void UpdateAnimation(float elapsedMilliseconds)
        {
            GetCurrentAnimation().UpdateFrame(elapsedMilliseconds);
        }

        public virtual void MakeShoot(Scene scene)
        {
            throw new NotImplementedException("default shooting are not implemented");
            var player = scene.Player;
            var xRayUnit = MathF.Sin(player.DirectionAngle);
            var yRayUnit = MathF.Cos(player.DirectionAngle);
            var rayStep = 0.01f;
            var playerX = player.Position.X;
            var playerY = player.Position.Y;
            var currentX = playerX;
            var currentY = playerY;
            var currentXStep = xRayUnit * rayStep;
            var currentYStep = yRayUnit * rayStep;
            var map = scene.Map;

            while (true)
            {
                currentX += currentXStep;
                currentY += currentYStep;
                var testX = (int) currentX;
                var testY = (int) currentY;

                if (!map.InBound(testY, testX) || map.At(testY, testX).Type == MapCellType.Wall)
                    return;

                var p = new Vector2(currentX, currentY);

                foreach (var obj in scene.Objects)
                {
                    if (!obj.InRadius(p))
                        continue;

                    if (obj.Contains(p))
                        return;
                }
            }
        }
    }
}