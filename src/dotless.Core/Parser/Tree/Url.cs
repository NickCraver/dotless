namespace dotless.Core.Parser.Tree
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Infrastructure;
    using Infrastructure.Nodes;
    using Utils;

    public class Url : Node
    {
        public TextNode Value { get; set; }

        public Url(TextNode value, IEnumerable<string> paths)
        {
            // The following block is commented out, because we want to have the path verbatim,
            // not rewritten. Here's why: Say less/trilogy_base.less contains
            //
            //   background-image: url(img/sprites.png)
            //
            // and serverfault/all.less contains
            //
            //   @import "../less/trilogy_base"
            //
            // If relative paths where to be rewritten, the resulting serverfault/all.css would read
            //
            //   background-image: url(../less/img/sprites.png)
            //
            // which is obviously not what we want.

            /*if (!Regex.IsMatch(value.Value, @"^(http:\/)?\/") && paths.Any())
            {
                value.Value = paths.Concat(new[] {value.Value}).AggregatePaths();
            }*/

            Value = value;
        }

        public string GetUrl()
        {
            return Value.Value;
        }

        public override string ToCSS(Env env)
        {
            return "url(" + Value.ToCSS(env) + ")";
        }

        public override Node Copy()
        {
            return new Url((TextNode)Value.Copy(), null);
        }
    }
}