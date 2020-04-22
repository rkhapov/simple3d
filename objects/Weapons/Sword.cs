using simple3d;
using simple3d.Drawing;
using simple3d.Levels;

namespace objects.Weapons
{
    public class Sword : MeleeWeapon
    {
        private Sword(Animation staticAnimation, Animation attackLeftAnimation, Animation attackRightAnimation,
            Animation blockLeftAnimation, Animation blockRightAnimation, Animation movingAnimation) : base(
            staticAnimation, attackLeftAnimation, attackRightAnimation, blockLeftAnimation, blockRightAnimation,
            movingAnimation)
        {
        }

        public static Sword Create(ResourceCachedLoader loader)
        {
            var @static = loader.GetAnimation("./animations/sword_static");
            var leftAttack = loader.GetAnimation("./animations/sword_left_attack");
            var rightAttack = loader.GetAnimation("./animations/sword_right_attack");
            var leftBlock = loader.GetAnimation("./animations/sword_left_block");
            var rightBlock = loader.GetAnimation("./animations/sword_right_block");
            
            return new Sword(
                @static,
                leftAttack,
                rightAttack,
                leftBlock,
                rightBlock,
                @static);
        }
    }
}