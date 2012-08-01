namespace dotless.Core.Parser.Functions
{
    using System;
    using System.Linq;
    using Infrastructure;
    using Infrastructure.Nodes;
    using Tree;

    public class FormatStringFunction : Function
    {
        protected override Node Evaluate(Env env)
        {
            if (Arguments.Count == 0)
                return new Quoted("");

            Func<Node, string> unescape = n => n is Quoted ? ((Quoted) n).UnescapeContents() : n.ToCSS(env);

            var format = unescape(Arguments[0]);

            var args = new string[Arguments.Count - 1];
            for (var i = 1; i < Arguments.Count; i++)
            {
                args[i - 1] = unescape(Arguments[i]);
            }

            var result = string.Format(format, args);

            return new Quoted(result);
        }
    }
}