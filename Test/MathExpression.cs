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
        public void AlphabeticalSymbolsInInputThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new MathExpression("a * b * c"));
        }

        [Test]
        public void WhiteSpacesGetIgnored()
        {
            Assert.That(new MathExpression("1  +2 + 3").GetAnswerText(), Is.EqualTo("6"));
        }

        // Каждый шаг работы покрыть, всмысле?
        // Всё остальное ещё
        // Не забудь расскоментить валидацию при нем
    }
}