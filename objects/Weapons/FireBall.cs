using System.Numerics;
using objects.Monsters;
using objects.Monsters.Algorithms;
using simple3d.Drawing;
using simple3d.Levels;

namespace objects.Weapons
{
    public class FireBall : AnimatedObject
    {
        private float lifeTime;

        public FireBall(
            Vector2 position,
            Vector2 size,
            float directionAngle,
            float lifeTime,
            Animation animation,
            IMapObject target) : base(position, size, directionAngle)
        {
            this.lifeTime = lifeTime;
            CurrentAnimation = animation;
            Target = target;
        }
        
        public IMapObject Target { get; }

        public override void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            base.OnWorldUpdate(scene, elapsedMilliseconds);

            if (!TryGetDirectionToTarget(scene, out var angle))
            {
                scene.RemoveObject(this);
                return;
            }

            if (ShouldBlow())
            {
                Target.OnShoot(scene, 100500);
                scene.RemoveObject(this);
                return;
            }

            DirectionAngle = angle;
            MoveOnDirection(scene, elapsedMilliseconds);
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
            var targetPoint = MapPoint.FromVector2(Target.Position);
            var path = PathFinder.FindPath(scene.Map, targetPoint, myPoint);

            if (path == null)
                return false;

            angle = GetAngleToPoint(path[1]);

            return true;
        }


        protected override int ViewDistance => 20;
        protected override float MoveSpeed => 0.006f;
        public override Animation CurrentAnimation { get; }
    }
}