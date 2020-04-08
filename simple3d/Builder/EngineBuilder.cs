namespace simple3d.Builder
{
    public static class EngineBuilder
    {
        public static IEngine BuildEngine(EngineOptions options)
        {
            return Engine.Create(options);
        }
    }
}