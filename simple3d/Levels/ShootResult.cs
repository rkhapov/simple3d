namespace simple3d.Levels
{
    public class ShootResult
    {
        public ShootResult(bool hit, IMapObject hitObject)
        {
            Hit = hit;
            HitObject = hitObject;
        }

        public bool Hit { get; }
        public IMapObject HitObject { get; }
    }
}