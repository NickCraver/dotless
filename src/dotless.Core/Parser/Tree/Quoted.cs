namespace dotless.Core.Parser.Tree
{
    using System.Text.RegularExpressions;
    using Infrastructure;
    using Infrastructure.Nodes;

    public class Quoted : TextNode
    {
        public char? Quote { get; set; }
        public bool Verbatim { get; set; }

        private Quoted() { }

        public Quoted(string value, string contents)
          : base(contents)
        {
            if (value[0] == '`')
            {
                Quote = null;
                Verbatim = true;
            }
            else
            {
                Quote = value[0];
            }
        }

        public Quoted(string value)
            : this(value, value)
        {
            Quote = null;
        }

        public override string ToCSS(Env env)
        {
            return Quote + Value + Quote;
        }

        private readonly Regex _unescape = new Regex(@"(^|[^\\])\\(.)");

        public string UnescapeContents()
        {
            return _unescape.Replace(Value, @"$1$2");
        }

        public override Node Copy()
        {
            return new Quoted
            {
                Value = Value,
                Quote = Quote,
                Index = Index,
                Verbatim = Verbatim
            };
        }
    }
}