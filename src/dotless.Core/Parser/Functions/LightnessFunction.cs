﻿namespace dotless.Core.Parser.Functions
{
    using Infrastructure.Nodes;
    using Tree;
    using Utils;

    public class LightnessFunction : HslColorFunctionBase
    {
        protected override Node EvalHsl(HslColor color)
        {
            return color.GetLightness();
        }

        protected override Node EditHsl(HslColor color, Number number)
        {
            color.Lightness += number.Value/100;
            return color.ToRgbColor();
        }
    }
}