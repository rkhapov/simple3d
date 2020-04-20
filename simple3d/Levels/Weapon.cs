using System;
using simple3d.Drawing;

namespace simple3d.Levels
{
    public abstract class Weapon
    {
        private readonly Animation staticAnimation;
        private readonly Animation attackAnimation;
        private readonly Animation movingAnimation;

        private WeaponAnimationState weaponAnimationState;

        public WeaponAnimationState AnimationState
        {
            get => weaponAnimationState;
            set
            {
                staticAnimation.Reset();
                attackAnimation.Reset();
                movingAnimation.Reset();

                weaponAnimationState = value;
            }
        }

        protected Weapon(Animation staticAnimation, Animation attackAnimation, Animation movingAnimation)
        {
            this.staticAnimation = staticAnimation;
            this.attackAnimation = attackAnimation;
            this.movingAnimation = movingAnimation;
            weaponAnimationState = WeaponAnimationState.Static;
        }

        public Sprite Sprite => GetCurrentAnimation().CurrentFrame;

        public void UpdateAnimation(float elapsedMilliseconds)
        {
            GetCurrentAnimation().UpdateFrame(elapsedMilliseconds);
        }

        private Animation GetCurrentAnimation()
        {
            return AnimationState switch
            {
                WeaponAnimationState.Static => staticAnimation,
                WeaponAnimationState.Moving => movingAnimation,
                WeaponAnimationState.Attack => attackAnimation,
                _ => throw new NotImplementedException($"Animation for {AnimationState} are not implemented")
            };
        }
    }
}