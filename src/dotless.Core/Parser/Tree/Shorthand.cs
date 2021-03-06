﻿namespace dotless.Core.Parser.Tree
{
    using Infrastructure;
    using Infrastructure.Nodes;

    public class Shorthand : Node
    {
        public Node First { get; set; }
        public Node Second { get; set; }

        public Shorthand(Node first, Node second)
        {
            First = first;
            Second = second;
        }

        public override string ToCSS(Env env)
        {
            return First.ToCSS(env) + "/" + Second.ToCSS(env);
        }

        public override Node Copy()
        {
            return new Shorthand(First.Copy(), Second.Copy());
        }
    }
}