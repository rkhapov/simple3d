using simple3d.Drawing;
using simple3d.Levels;

namespace objects.Weapons
{
    public class Bow : ShootingWeapon
    {
        private readonly Sprite arrowSprite;
        
        public Bow(
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
    }
}