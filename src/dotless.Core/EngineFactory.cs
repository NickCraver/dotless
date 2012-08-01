namespace dotless.Core
{
    using configuration;

    public class EngineFactory
    {
        public DotlessConfiguration Configuration { get; set; }

        public EngineFactory(DotlessConfiguration configuration)
        {
            Configuration = configuration;
        }
        public EngineFactory() : this(DotlessConfiguration.Default)
        {
        }

        public ILessEngine GetEngine(string curDir, bool noCache = false)
        {
            return new LessEngine(curDir, Configuration.MinifyOutput, noCache: noCache);
        }
    }
}