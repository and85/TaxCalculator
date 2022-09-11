using System;
using NUnit.Framework;
using AndriiCo.TaxCalculatorLib.DeductionCalculators;
using AndriiCo.TaxCalculatorLib.Entities;

namespace AndriiCo.Tests.TaxCalculatorLibTests.DeductionCalculatorsTests
{
    /// <summary>
    /// Tests for <see cref="ProgressiveDeductionCalculator"/>
    /// </summary>
    [TestFixture]
    public class ProgressiveDeductionTests
    {
        private readonly Currency _currency = new Currency() { Name = "Eur" };

        [Test]
        public void Calculate_ReturnsCorrectResult_WhenTaxPayerReachedThreshold()
        {
            // arrange
            var expected = new Money(310, _currency);

            var basicRate = new Rate(25);
            var higherRate = new Rate(40);
            var threshold = new Money(600, _currency);
            var grossAmount = new Money(1000, _currency);

            var deduction = new ProgressiveDeductionCalculator("deduction", basicRate, higherRate, threshold);

            // act
            var actual = deduction.CalculateDeduction(grossAmount);

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Calculate_ReturnsCorrectResult_WhenTaxPayerDidntReachThreshold()
        {
            // arrange
            var expected = new Money(125, _currency);

            var basicRate = new Rate(25);
            var higherRate = new Rate(40);
            var threshold = new Money(600, _currency);
            var grossAmount = new Money(500, _currency);

            var deduction = new ProgressiveDeductionCalculator("deduction", basicRate, higherRate, threshold);

            // act
            var actual = deduction.CalculateDeduction(grossAmount);

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Calculate_ThrowsAnException_WhenThresholdAndGrossAmountCurrenciesAreDifferent()
        {
            // arrange
            var basicRate = new Rate(25);
            var higherRate = new Rate(40);
            var threshold = new Money(600, new Currency() { Name = "Eur" });
            var grossAmount = new Money(500, new Currency() { Name = "Usd" });

            var deduction = new ProgressiveDeductionCalculator("deduction", basicRate, higherRate, threshold);

            // act and asser
            Assert.Throws<ArgumentException>(() => deduction.CalculateDeduction(grossAmount));
        }
    }
}
