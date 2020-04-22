using simple3d.Drawing;
using simple3d.Levels;

namespace objects.Weapons
{
    public class Sword : MeleeWeapon
    {
        public Sword(Animation staticAnimation, Animation attackLeftAnimation, Animation attackRightAnimation,
            Animation blockLeftAnimation, Animation blockRightAnimation, Animation movingAnimation) : base(
            staticAnimation, attackLeftAnimation, attackRightAnimation, blockLeftAnimation, blockRightAnimation,
            movingAnimation)
        {
        }
    }
}