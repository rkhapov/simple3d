using simple3d.Drawing;
using System;

namespace simple3d.Levels
{
    public class MapCell
    {
        public MapCell(MapCellType type, Sprite wallSprite, Sprite floorSprite, Sprite ceilingSprite)
        {
            Type = type;
            WallSprite = wallSprite;
            CeilingSprite = ceilingSprite;
            FloorSprite = floorSprite;
            isAnimation = false;
            tag = "Cell";
            animation = null;
        }

        public MapCell(MapCellType type, Animation animation, Sprite floorSprite, Sprite ceilingSprite, string tag)
        {
            this.animation = animation;
            isAnimation = true;
            this.tag = tag;
            Type = type;
            WallSprite = animation.CurrentFrame;
            CeilingSprite = ceilingSprite;
            FloorSprite = floorSprite;
        }


        public void SpriteUpdate(float time)
        {
            if (isAnimation)
            {
                if (animation.IsOver)
                {
                    isAnimation = false;
                    Type = MapCellType.Empty;
                }
                animation.UpdateFrame(time);
                WallSprite = animation.CurrentFrame;
                
            }
        }

        private Animation animation;
        public string tag;
        private bool isAnimation;
        public MapCellType Type { get; private set; }
        public Sprite WallSprite { get; private set; }
        public Sprite FloorSprite { get; }
        public Sprite CeilingSprite { get;  }
    }
}