namespace dotless.Test.Specs
{
    using NUnit.Framework;

    public class ScopeFixture : SpecFixtureBase
    {
        [Test]
        public void Scope()
        {
            // Todo: split into separate atomic tests.
            var input =
                @"
@x: blue;
@z: transparent;
@mix: none;

.mixin {
  @mix: #989;
}

.tiny-scope {
  color: @mix; // none
  .mixin;
  color: @mix; // #989
}

.scope1 {
  @y: orange;
  @z: black;
  color: @x; // blue
  border-color: @z; // black
  .hidden {
    @x: #131313;
  }
  .scope2 {
    @y: red;
    color: @x; // blue
    .scope3 {
      @local: white;
      color: @y; // red
      border-color: @z; // black
      background-color: @local; // white
    }
  }
}
";

            var expected =
                @"
.tiny-scope {
  color: none;
  color: #998899;
}
.scope1 {
  color: #0000ff;
  border-color: #000000;
}
.scope1 .scope2 {
  color: #0000ff;
}
.scope1 .scope2 .scope3 {
  color: #ff0000;
  border-color: #000000;
  background-color: #ffffff;
}
";

            AssertLess(input, expected);
        }
    }
}