namespace dotless.Core
{
    using configuration;
    using System.IO;

    public static class Less
    {
        public static string Parse(string less)
        {
            return Parse(less, DotlessConfiguration.Default);
        }

        public static string Parse(string less, DotlessConfiguration config)
        {
            var dir = Path.GetDirectoryName(less);
            return new EngineFactory(config).GetEngine(dir, true).TransformToCss(less, null);
        }
    }
}