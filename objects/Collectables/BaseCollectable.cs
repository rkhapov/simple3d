using System.Numerics;
using objects.Monsters;
using simple3d.Drawing;
using simple3d.Levels;
using simple3d.Sounds;

namespace objects.Collectables
{
    public abstract class BaseCollectable : AnimatedObject
    {
        private readonly ISound collectionSound;
        
        public abstract string Name { get; }

        protected BaseCollectable(
            Vector2 position,
            Animation animation,
            ISound collectionSound) : base(position, Vector2.Zero, 0)
        {
            CurrentAnimation = animation;
            this.collectionSound = collectionSound;
        }

        private const float CollectionRadius = 1.3f;
        private const float CollectionRadiusSquared = CollectionRadius * CollectionRadius;

        public override void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            base.OnWorldUpdate(scene, elapsedMilliseconds);

            if ((scene.Player.Position - Position).LengthSquared() < CollectionRadiusSquared)
            {
                OnCollect(scene);
                scene.EventsLogger.AddCollectOfItem(Name);
                collectionSound.Play(0);
                scene.RemoveObject(this);
            }
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

        protected abstract void OnCollect(Scene scene);

        protected override int ViewDistance => 10;
        protected override float MoveSpeed => 0;
        public override Animation CurrentAnimation { get; }
    }
}