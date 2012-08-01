namespace dotless.Test.Specs.Functions
{
    using NUnit.Framework;

    public class GrayscaleFixture : SpecFixtureBase
    {
        [Test]
        public void TestGrayscale()
        {
            AssertExpression("#bbbbbb", "grayscale(#abc)");
            AssertExpression("#808080", "grayscale(#f00)");
            AssertExpression("#808080", "grayscale(#00f)");
            AssertExpression("#ffffff", "grayscale(#fff)");
            AssertExpression("#000000", "grayscale(#000)");
        }

        [Test]
        public void TestGrayscaleTestsTypes()
        {
            AssertExpressionError("Expected color in function 'grayscale', found \"foo\"", 10, "grayscale(\"foo\")");
        }
    }
}