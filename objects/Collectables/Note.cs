using System.Numerics;
using musics;
using simple3d;
using simple3d.Drawing;
using simple3d.Levels;
using simple3d.Sounds;

namespace objects.Collectables
{
    public class Note : BaseCollectable
    {
        public Note(Vector2 position, Animation animation, ISound collectionSound) : base(position, animation, collectionSound)
        {
        }

        public override string Name => "Записка";

        protected override void OnCollect(Scene scene)
        {
        }

        public static Note Create(Vector2 position)
        {
            var animation = Animation.FromSingleSprite(
                ResourceCachedLoader.Instance.GetSprite("./sprites/collectables/scroll.png"));
            var sound = ResourceCachedLoader.Instance.GetSound(MusicResourceHelper.CollectionScrollPath);

            return new Note(position, animation, sound);
        }
    }
}