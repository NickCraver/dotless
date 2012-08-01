﻿namespace dotless.Core.Parser.Tree
{
    using System.Linq;
    using Infrastructure;
    using Infrastructure.Nodes;
    using Utils;

    public class Expression : Node
    {
        public NodeList Value { get; set; }

        public Expression(NodeList value)
        {
            Value = value;
        }

        public override Node Evaluate(Env env)
        {
            if (Value.Count > 1)
                return new Expression(new NodeList(Value.Select(e => e.Evaluate(env))));

            return Value[0].Evaluate(env);
        }

        public override string ToCSS(Env env)
        {
            var prevWasVerbatim = true;
            return Value.Select(e => {
                var thisIsVerbatim = e is Quoted && ((Quoted)e).Verbatim;
                string space = prevWasVerbatim || thisIsVerbatim ? "" : " ";
                prevWasVerbatim = thisIsVerbatim;
                return space + e.ToCSS(env);
            }).JoinStrings("");
        }

        public override Node Copy()
        {
            return new Expression((NodeList)Value.Copy());
        }
    }
}