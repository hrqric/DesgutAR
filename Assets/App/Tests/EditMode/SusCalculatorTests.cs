using NUnit.Framework;

namespace CardapioRA.Tests
{
    public sealed class SusCalculatorTests
    {
        [Test]
        public void AlternatingIdealAnswersProduceOneHundred()
        {
            var answers = new[] { 5, 1, 5, 1, 5, 1, 5, 1, 5, 1 };
            Assert.That(SusCalculator.Calculate(answers), Is.EqualTo(100f));
        }

        [Test]
        public void NeutralAnswersProduceFifty()
        {
            var answers = new[] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };
            Assert.That(SusCalculator.Calculate(answers), Is.EqualTo(50f));
        }
    }
}

