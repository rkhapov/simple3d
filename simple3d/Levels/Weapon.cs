using System;
using simple3d.Drawing;

namespace simple3d.Levels
{
    public abstract class Weapon
    {
        private readonly Animation staticAnimation;
        private readonly Animation attackLeftAnimation;
        private readonly Animation attackRightAnimation;
        private readonly Animation movingAnimation;
        private readonly Animation blockLeftAnimation;
        private readonly Animation blockRightAnimation;

        private WeaponAnimationState weaponAnimationState;

        public WeaponAnimationState AnimationState
        {
            get => weaponAnimationState;
            set
            {
                if (weaponAnimationState == value)
                    return;

                staticAnimation.Reset();
                attackLeftAnimation.Reset();
                movingAnimation.Reset();
                attackRightAnimation.Reset();
                blockLeftAnimation.Reset();
                blockRightAnimation.Reset();

                weaponAnimationState = value;
            }
        }

        protected Weapon(Animation staticAnimation,
            Animation attackLeftAnimation,
            Animation attackRightAnimation,
            Animation blockLeftAnimation,
            Animation blockRightAnimation,
            Animation movingAnimation)
        {
            this.staticAnimation = staticAnimation;
            this.attackLeftAnimation = attackLeftAnimation;
            this.movingAnimation = movingAnimation;
            this.blockLeftAnimation = blockLeftAnimation;
            this.blockRightAnimation = blockRightAnimation;
            this.attackRightAnimation = attackRightAnimation;
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
                WeaponAnimationState.AttackLeft => attackLeftAnimation,
                WeaponAnimationState.AttackRight => attackRightAnimation,
                WeaponAnimationState.BlockLeft => blockLeftAnimation,
                WeaponAnimationState.BlockRight => blockRightAnimation,
                _ => throw new NotImplementedException($"Animation for {AnimationState} are not implemented")
            };
        }
    }
}