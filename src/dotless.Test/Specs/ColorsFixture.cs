namespace dotless.Test.Specs
{
    using NUnit.Framework;

    public class ColorsFixture : SpecFixtureBase
    {
        [Test]
        public void Colors()
        {
            AssertExpressionUnchanged("#fea");
            AssertExpressionUnchanged("#ffeeaa");
            AssertExpressionUnchanged("rgba(255, 238, 170, 0.1)");
            AssertExpressionUnchanged("#00f");
            AssertExpressionUnchanged("#0000ff");
            AssertExpressionUnchanged("rgba(0, 0, 255, 0.1)");
        }
        
        [Test]
        public void Overflow()
        {
            AssertExpression("#000000", "#111111 - #444444");
            AssertExpression("#ffffff", "#eee + #fff");
            AssertExpression("#ffffff", "#aaa * 3");
            AssertExpression("#00ff00", "#00ee00 + #009900");
        }

        [Test]
        public void Gray()
        {
            AssertExpression("#c8c8c8", "rgb(200, 200, 200)");
            AssertExpression("#808080", "hsl(50, 0, 50)");
        }
    }
}