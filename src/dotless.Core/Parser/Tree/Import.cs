namespace dotless.Core.Parser.Tree
{
    using System.Text.RegularExpressions;
    using Importers;
    using Infrastructure;
    using Infrastructure.Nodes;
    using Utils;

    public class Import : Directive
    {
        public Importer Importer { get; set; }
        public string Path { get; set; }
        protected Node OriginalPath { get; set; }
        protected bool Css { get; set; }
        public Ruleset InnerRoot { get; set; }

        private Import() { }

        public Import(Quoted path, Importer importer)
            : this(path.Value, importer)
        {
            OriginalPath = path;
        }

        public Import(Url path, Importer importer)
            : this(path.GetUrl(), importer)
        {
            OriginalPath = path;
        }

        private Import(string path, Importer importer)
        {
            Importer = importer;
            var regex = new Regex(@"\.(le|c)ss$");

            Path = regex.IsMatch(path) ? path : path + ".less";

            Css = Path.EndsWith("css");

            if(!Css)
                Importer.Import(this);
        }

        protected override string ToCSS(Env env, Context context)
        {
            return base.ToCSS(env); // should throw InvalidOperationException
        }

        public override Node Evaluate(Env env)
        {
            if (Css)
                return new NodeList(new TextNode("@import " + OriginalPath.ToCSS(env) + ";\n"));

            NodeHelper.ExpandNodes<Import>(env, InnerRoot.Rules);
            InnerRoot.InvalidRulesetCache();

            return new NodeList(InnerRoot.Rules);
        }

        public override Node Copy()
        {
            return
                new Import
                {
                     Importer = Importer,
                     Path = Path,
                     OriginalPath = OriginalPath != null ? OriginalPath.Copy() : null,
                     Css = Css,
                     InnerRoot = InnerRoot != null ? (Ruleset)InnerRoot.Copy() : null
                };
        }
    }
}