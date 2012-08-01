﻿namespace dotless.Core.Parser.Infrastructure.Nodes
{
    public class TextNode : Node
    {
        public string Value { get; set; }

        protected TextNode() { }

        public TextNode(string contents)
        {
            Value = contents;
        }

        public static TextNode operator &(TextNode n1, TextNode n2)
        {
            return n1 != null ? n2 : null;
        }

        public static TextNode operator |(TextNode n1, TextNode n2)
        {
            return n1 ?? n2;
        }

        public override string ToCSS(Env env)
        {
            return env != null && env.Compress ? Value.Trim() : Value;
        }

        public override string ToString()
        {
            return Value;
        }

        public override Node Copy()
        {
            return new TextNode(Value);
        }
    }
}