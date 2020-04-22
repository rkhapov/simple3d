using System;
using simple3d.Drawing;

namespace simple3d.Levels
{
    public abstract class MeleeWeapon : Weapon
    {
        private readonly Animation staticAnimation;
        private readonly Animation attackLeftAnimation;
        private readonly Animation attackRightAnimation;
        private readonly Animation movingAnimation;
        private readonly Animation blockLeftAnimation;
        private readonly Animation blockRightAnimation;

        private MeleeWeaponState state;

        public MeleeWeaponState State
        {
            get => state;
            set
            {
                if (state == value)
                    return;

                staticAnimation.Reset();
                attackLeftAnimation.Reset();
                movingAnimation.Reset();
                attackRightAnimation.Reset();
                blockLeftAnimation.Reset();
                blockRightAnimation.Reset();

                state = value;
            }
        }

        protected MeleeWeapon(Animation staticAnimation,
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
            state = MeleeWeaponState.Static;
        }

        public override Sprite Sprite => GetCurrentAnimation().CurrentFrame;

        public override void UpdateAnimation(float elapsedMilliseconds)
        {
            GetCurrentAnimation().UpdateFrame(elapsedMilliseconds);
        }

        private Animation GetCurrentAnimation()
        {
            return State switch
            {
                MeleeWeaponState.Static => staticAnimation,
                MeleeWeaponState.Moving => movingAnimation,
                MeleeWeaponState.AttackLeft => attackLeftAnimation,
                MeleeWeaponState.AttackRight => attackRightAnimation,
                MeleeWeaponState.BlockLeft => blockLeftAnimation,
                MeleeWeaponState.BlockRight => blockRightAnimation,
                _ => throw new NotImplementedException($"Animation for {State} are not implemented")
            };
        }

        public void DoLeftAttack(Scene scene)
        {
            State = MeleeWeaponState.AttackLeft;
            var player = scene.Player;

            foreach (var obj in scene.Objects)
            {
                if ((obj.Position - player.Position).Length() < 1)
                {
                    obj.OnLeftMeleeAttack(scene, this);
                }
            }
        }

        public void DoRightAttack(Scene scene)
        {
            throw new NotImplementedException();
        }

        public override bool AnimationIsOver => GetCurrentAnimation().IsOver;
        
        public override void GoStatic()
        {
            State = MeleeWeaponState.Static;
        }
    }
}