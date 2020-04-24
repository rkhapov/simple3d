using musics;
using simple3d;
using simple3d.Drawing;
using simple3d.Levels;
using simple3d.Sounds;

namespace objects.Weapons
{
    public class Bow : ShootingWeapon
    {
        private readonly Sprite arrowSprite;
        private readonly ISound shootingSound;
        
        private Bow(
            Animation staticAnimation,
            Animation movingAnimation,
            Animation shootingAnimation,
            Sprite arrowSprite,
            ISound shootingSound) : base(staticAnimation, movingAnimation, shootingAnimation)
        {
            this.arrowSprite = arrowSprite;
            this.shootingSound = shootingSound;
        }

        public override void SpawnArrow(Scene scene)
        {
            scene.AddObject(new Arrow(scene.Player.Position, arrowSprite, scene.Player.DirectionAngle));
        }

        public override void MakeShoot(Scene scene)
        {
            if (State == ShootingWeaponState.Shooting)
                return;

            State = ShootingWeaponState.Shooting;

            shootingSound.Play(0);
        }

        public static Bow Create(ResourceCachedLoader loader)
        {
            var @static = loader.GetAnimation("./animations/bow_static");
            var moving = loader.GetAnimation("./animations/bow_moving");
            var shoot = loader.GetAnimation("./animations/bow_shoot");
            var arrowSprite = loader.GetSprite("./sprites/arrow.png");
            var shootSound = loader.GetSound(MusicResourceHelper.BowShootSoundPath);

            return new Bow(
                @static,
                moving,
                shoot,
                arrowSprite,
                shootSound);
        }
    }
}