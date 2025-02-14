﻿using simple3d.Drawing;

namespace simple3d.Levels
{
    public abstract class Weapon
    {
        public abstract Sprite Sprite { get; }
        public abstract void UpdateAnimation(float elapsedMilliseconds);
        public abstract bool AnimationIsOver { get; }
        public abstract void GoStatic();
    }
}