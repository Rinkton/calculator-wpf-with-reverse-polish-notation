using BL;

namespace Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void AlphabeticalSymbolsInInputThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => new MathExpression("a * b * c"));
        }

        [Test]
        public void WhiteSpacesGetIgnored()
        {
            Assert.That(new MathExpression("1  +2 + 3").GetAnswerText(), Is.EqualTo("6"));
        }

        [Test]
        public void RPNPrioritiesWorkCorrectly()
        {
            Assert.That(new MathExpression("1 + 5^2 * 6").GetAnswerText(), Is.EqualTo("151"));
        }

        [Test]
        public void PostfixToRPNWorksCorrectly()
        {
            Assert.That(new MathExpression("1 + 2").GetReversePolishNotationText(), Is.EqualTo("1 2 +"));
        }
    }
}