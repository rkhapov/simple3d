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
        private readonly string text;

        public Note(Vector2 position, Animation animation, ISound collectionSound, string text) : base(position, animation, collectionSound)
        {
            this.text = text;
        }

        public override string Name => "Записка";

        protected override void OnCollect(Scene scene)
        {
            scene.Player.DoReadText(text);
        }

        public static Note Create(Vector2 position, string text)
        {
            var animation = Animation.FromSingleSprite(
                ResourceCachedLoader.Instance.GetSprite("./sprites/collectables/scroll.png"));
            var sound = ResourceCachedLoader.Instance.GetSound(MusicResourceHelper.CollectionScrollPath);

            return new Note(position, animation, sound, text);
        }
    }
}