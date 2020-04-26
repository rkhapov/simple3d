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
        }

        public MapCell(MapCellType type, Animation animation, Sprite floorSprite, Sprite ceilingSprite)
        {
            this.animation = animation;
            isAnimation = true;
            Type = type;
            WallSprite = animation.CurrentFrame;
            CeilingSprite = ceilingSprite;
            FloorSprite = floorSprite;
        }

        public void SetTag(string tag)
        {
            Map.AddTaggedCall(tag, this);
        }

        public void StartAnimatiom()
        {
            if (isAnimation)
            {
                animation.Reset();
                isStart = true;

            }
        }
        public void StartAnimatiom(Action invoking)
        {
            if (isAnimation)
            {
                Type = MapCellType.TransparentObj;
                animation.Reset();
                isStart = true;
                this.invoking = invoking;
            }
        }

        public void SpriteUpdate(float time)
        {
            if (isAnimation && isStart)
            {
                animation.UpdateFrame(time);
                WallSprite = animation.CurrentFrame;
                if (animation.IsOver)
                {
                    isStart = false;
                    invoking();
                }
                
                
            }
        }

        private Action invoking = () => { };
        private bool isStart;
        private Animation animation;
       
        private readonly bool isAnimation;
        public MapCellType Type { get;  set; }
        public Sprite WallSprite { get; private set; }
        public Sprite FloorSprite { get; }
        public Sprite CeilingSprite { get;  }
    }
}