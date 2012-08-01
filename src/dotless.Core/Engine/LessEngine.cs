namespace dotless.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Exceptions;
    using Loggers;
    using Parser.Infrastructure;

    public class LessEngine : ILessEngine
    {
        public Parser.Parser Parser { get; set; }
        public ILogger Logger { get; set; }
        public bool Compress { get; set; }

        public LessEngine(Parser.Parser parser, ILogger logger, bool compress)
        {
            Parser = parser;
            Logger = logger;
            Compress = compress;
        }

        public LessEngine(Parser.Parser parser)
            : this(parser, new ConsoleLogger(LogLevel.Error), false)
        {
        }

        public LessEngine(string curDir)
            : this(new Parser.Parser(curDir))
        {
        }

        public LessEngine(string curDir, bool compress, bool noCache = false)
            : this(new Parser.Parser(curDir, noCache: noCache), new ConsoleLogger(LogLevel.Error), compress)
        {

        }

        public string TransformToCss(string source, string fileName)
        {
            try
            {
                var tree = Parser.Parse(source, fileName);

                var env = new Env { Compress = Compress };

                var css = tree.ToCSS(env);

                return css;
            }
            catch (ParserException e)
            {
                Logger.Error(e.Message);
                throw;
            }

            return "";
        }

        public IEnumerable<string> GetImports()
        {
            return Parser.Importer.Imports.Distinct();
        }

        public void ResetImports()
        {
            Parser.Importer.Imports.Clear();
        }

    }
}