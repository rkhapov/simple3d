using simple3d;
using simple3d.Drawing;
using simple3d.Levels;

namespace objects.Weapons
{
    public class Bow : ShootingWeapon
    {
        private readonly Sprite arrowSprite;
        
        private Bow(
            Animation staticAnimation,
            Animation movingAnimation,
            Animation shootingAnimation,
            Sprite arrowSprite) : base(staticAnimation, movingAnimation, shootingAnimation)
        {
            this.arrowSprite = arrowSprite;
        }

        public override void MakeShoot(Scene scene)
        {
            scene.AddObject(new Arrow(scene.Player.Position, arrowSprite, scene.Player.DirectionAngle));
        }

        public static Bow Create(ResourceCachedLoader loader)
        {
            var @static = loader.GetAnimation("./animations/bow_static");
            var moving = loader.GetAnimation("./animations/bow_moving");
            var shoot = loader.GetAnimation("./animations/bow_shoot");
            var arrowSprite = loader.GetSprite("./sprites/arrow.png");

            return new Bow(
                @static,
                moving,
                shoot,
                arrowSprite);
        }
    }
}