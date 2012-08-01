namespace dotless.Core.Parser.Functions
{
    using System.Linq;
    using Infrastructure;
    using Infrastructure.Nodes;
    using Tree;
    using Utils;
    using System.Collections.Generic;

    public class MixFunction : Function
    {
        protected override Node Evaluate(Env env)
        {
            Guard.ExpectMinArguments(2, Arguments.Count, this, Index);
            Guard.ExpectMaxArguments(3, Arguments.Count, this, Index);

            var argGuard = new List<Node>(2);
            for (var i = 0; i < 2 && i < Arguments.Count; i++)
            {
                argGuard.Add(Arguments[i]);
            }

            Guard.ExpectAllNodes<Color>(argGuard, this, Index);

            double weight = 50;
            if (Arguments.Count == 3)
            {
                Guard.ExpectNode<Number>(Arguments[2], this, Index);

                weight = ((Number) Arguments[2]).Value;
            }


            var colors = Arguments.Take(2).Cast<Color>().ToArray();

            // Note: this algorithm taken from http://github.com/nex3/haml/blob/0e249c844f66bd0872ed68d99de22b774794e967/lib/sass/script/functions.rb

            var p = weight/100.0;
            var w = p*2 - 1;
            var a = colors[0].Alpha - colors[1].Alpha;

            var w1 = (((w*a == -1) ? w : (w + a)/(1 + w*a)) + 1)/2.0;
            var w2 = 1 - w1;

            var rgb = colors[0].RGB.SelectArray((x, i) => x*w1 + colors[1].RGB[i]*w2);

            var alpha = colors[0].Alpha*p + colors[1].Alpha*(1 - p);

            var color = new Color(rgb[0], rgb[1], rgb[2], alpha);
            return color;
        }
    }
}