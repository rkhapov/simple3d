using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using simple3d.Drawing;
using simple3d.Levels;

namespace objects
{
    public class BaseAnimatedMapObj : IMapObject
    {
        public Vector2 Position { get; }
        public Vector2 Size { get; }
        public float DirectionAngle { get; }
        public Sprite Sprite { get; private set; }

        private Animation animation;

        public BaseAnimatedMapObj(Vector2 position, Vector2 size, float directionAngle, Animation animation)
        {
            Position = position;
            Size = size;
            DirectionAngle = directionAngle;
            Sprite = animation.CurrentFrame;
            this.animation = animation;
        }

        public void OnLeftMeleeAttack(Scene scene, int damage)
        {
           
        }

        public void OnRightMeleeAttack(Scene scene, int damage)
        {
            
        }

        public void OnShoot(Scene scene, int damage)
        {
           
        }

        public void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            animation.UpdateFrame(elapsedMilliseconds);
            Sprite = animation.CurrentFrame; // fix mb
        }
    }
}
