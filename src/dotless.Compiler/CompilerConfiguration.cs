namespace dotless.Compiler
{
    using Core.configuration;

    internal class CompilerConfiguration : DotlessConfiguration
    {
        public CompilerConfiguration(DotlessConfiguration config)
        {
            LessSource = config.LessSource;
            LogLevel = config.LogLevel;
            MinifyOutput = config.MinifyOutput;
            Optimization = config.Optimization;

            CacheEnabled = false;
            Web = false;
        }

        public bool Help { get; set; }
        public bool Recurse { get; set; }
    }
}