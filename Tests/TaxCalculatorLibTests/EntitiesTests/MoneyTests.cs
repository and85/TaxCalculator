using System;
using NUnit.Framework;
using AndriiCo.TaxCalculatorLib.Entities;

namespace AndriiCo.Tests.TaxCalculatorLibTests.EntitiesTests
{
    /// <summary>
    /// Tests for <see cref="Money"/>
    /// </summary>
    [TestFixture]
    public class MoneyTests
    {
        [Test]
        [TestCase(3.334, 3.33)]
        [TestCase(3.336, 3.34)]
        public void Constructor_RoundsInputDecimal(decimal input, decimal expected)
        {
            // Act
            var actual = new Money(input, new Currency());

            Assert.AreEqual(expected, actual.Amount);

        }

        [Test]
        public void Constructor_Throws_ArgumentOutOfRangeException_WhenAmountIsNegative()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Money(-1, new Currency()));
        }

        [Test]
        public void Constructor_Throws_ArgumentNullException_WhenCurrencyIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new Money(1, null));
        }

        [Test]
        public void PlusOperator()
        {
            // Arrange
            var currency = new Currency();
            var leftOperand = new Money(1, currency);
            var rightOperand = new Money(2, currency);

            // Act
            var result = leftOperand + rightOperand;

            // Assert
            Assert.AreEqual(3, result.Amount);
        }

        [Test]
        public void MinusOperator()
        {
            // Arrange
            var currency = new Currency();
            var leftOperand = new Money(3, currency);
            var rightOperand = new Money(2, currency);

            // Act
            var result = leftOperand - rightOperand;

            // Assert
            Assert.AreEqual(1, result.Amount);
        }

        [Test]
        [TestCase(1, 2, false)]
        [TestCase(2, 2, true)]
        [TestCase(3, 2, true)]
        public void GreaterOrEqualThanOperator(decimal leftOperand, decimal rightOperand, bool expectedResult)
        {
            // Arrange
            var currency = new Currency();
            var left = new Money(leftOperand, currency);
            var right = new Money(rightOperand, currency);

            // Act
            var actualResult = left >= right;

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        [TestCase(1, 2, true)]
        [TestCase(2, 2, true)]
        [TestCase(3, 2, false)]
        public void LessOrEqualThanOperator(decimal leftOperand, decimal rightOperand, bool expectedResult)
        {
            // Arrange
            var currency = new Currency();
            var left = new Money(leftOperand, currency);
            var right = new Money(rightOperand, currency);

            // Act
            var actualResult = left <= right;

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }


        [Test]
        [TestCase(3, 2, 6)]
        [TestCase(7, 0.333, 2.33)]
        [TestCase(6, 0.333, 2)]
        public void MultipyByNumber(decimal initialAmount, decimal number, decimal expectedResult)
        {
            // Arrange
            var currency = new Currency();
            var money = new Money(initialAmount, currency);

            // Act
            var actual = money.MultipyByNumber(number);

            // Assert
            Assert.AreEqual(expectedResult, actual.Amount);
        }
        

        [Test]
        public void ToString_ContainsCurrencySymbolAndMoney()
        {
            // Arrange
            var currency = new Currency() {Name = "Name", Symbol = "Symbol"};
            var money = new Money(10, currency);

            // Act
            var actual = money.ToString();

            // Asser
            Assert.AreEqual("Symbol10", actual);
        }
    }
}
