using NUnit.Framework;
using AndriiCo.TaxCalculatorLib.DeductionCalculators;
using AndriiCo.TaxCalculatorLib.Entities;

namespace AndriiCo.Tests.TaxCalculatorLibTests.DeductionCalculatorsTests
{
    /// <summary>
    /// Tests for <see cref="FlatDeductionCalculator"/>
    /// </summary>
    [TestFixture]
    public class FlatDeductionTests
    {
        [Test]
        public void Calculate_ReturnsCorrectResult()
        {
            // arrange
            var currency = new Currency() {Name = "Eur"};
            var expected = new Money(250, currency);

            var rate = new Rate(25);
            var grossAmount = new Money(1000, currency);

            var deduction = new FlatDeductionCalculator("deduction", rate);

            // act
            var actual = deduction.CalculateDeduction(grossAmount);

            // assert
            Assert.AreEqual(expected, actual);
        }
    }
}
