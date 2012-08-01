﻿namespace dotless.Core.Parser.Tree
{
    using Infrastructure;
    using Infrastructure.Nodes;

    public class Alpha : Call
    {
        public Node Value { get; set; }

        public Alpha(Node value)
        {
            Value = value;
        }

        public override Node Evaluate(Env env)
        {
            Value = Value.Evaluate(env);

            return this;
        }

        public override string ToCSS(Env env)
        {
            return string.Format("alpha(opacity={0})", Value.ToCSS(env));
        }

        public override Node Copy()
        {
            return new Alpha(Value.Copy());
        }
    }
}