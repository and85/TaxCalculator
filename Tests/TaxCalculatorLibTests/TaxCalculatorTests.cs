using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using AndriiCo.TaxCalculatorLib;
using AndriiCo.TaxCalculatorLib.DataProviders;
using AndriiCo.TaxCalculatorLib.DeductionCalculators;
using AndriiCo.TaxCalculatorLib.Entities;

namespace AndriiCo.Tests.TaxCalculatorLibTests
{
    /// <summary>
    /// Tests for <see cref="TaxCalculator"/>
    /// </summary>
    [TestFixture]
    public class TaxCalculatorTests
    {
        [Test]
        public void CalculateGrossAmount_MultipliesHourByRateCorrectly()
        {
            // Arrange
            var currencyActual = new Currency() { Name = "Eur", Symbol = "EurSymbol" };
            var expected = new Money(5000, currencyActual);

            var provider = MockRepository.GenerateStub<IDataProvider>();
            var calculator = new TaxCalculator();
            calculator.DataProvider = provider;

            uint hoursWorked = 10;
            uint hourlyRate = 500;
            var currency = new Currency() {Name = "Eur", Symbol = "EurSymbol"};

            // Act
            var actual = calculator.CalculateGrossAmount(hoursWorked, hourlyRate, currency);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CalculateDeductions_CheckThatCalculateMethodOnDeductionCalculatorIsCalled_Behaviour()
        {
            // Arrange
            var location = new Location();
            var currency = new Currency() { Name = "Eur", Symbol = "EurSymbol" };
            var grossAmount = new Money(5000, currency);

            var deductionCalculator = MockRepository.GenerateMock<IDeductionCalculator>();
            deductionCalculator.Expect(x => x.CalculateDeduction(grossAmount))
                .Repeat.Once()
                .Return(grossAmount);

            var source = new List<IDeductionCalculator>() { deductionCalculator };
            var provider = StubDataProvider(source);

            var calculator = new TaxCalculator();
            calculator.DataProvider = provider;

            // Act
            calculator.CalculateDeductions(grossAmount, location).ToList();

            // Assert
            deductionCalculator.VerifyAllExpectations();
        }

        [Test]
        public void CalculateDeductions_ReturnsCorrectDeduction()
        {
            // Arrange
            var location = new Location();
            var currency = new Currency() { Name = "Eur", Symbol = "EurSymbol" };
            var grossAmount = new Money(5000, currency);
            string deductionName = "Deduction";

            var deductionCalculator = MockRepository.GenerateStub<IDeductionCalculator>();
            deductionCalculator.Stub(x => x.DeductionName)
                .Repeat.Once()
                .Return(deductionName);
            deductionCalculator.Stub(x => x.CalculateDeduction(grossAmount))
                .Repeat.Once()
                .Return(grossAmount);

            var source = new List<IDeductionCalculator>() { deductionCalculator };
            var provider = StubDataProvider(source);

            var calculator = new TaxCalculator();
            calculator.DataProvider = provider;

            // Act
            var deduction = calculator.CalculateDeductions(grossAmount, location).Single();

            // Arrange
            Assert.AreEqual(deductionName, deduction.Name);
            Assert.AreEqual(grossAmount, deduction.Amount);
        }

        private IDataProvider StubDataProvider(List<IDeductionCalculator> calculators)
        {
            var provider = MockRepository.GenerateStub<IDataProvider>();
            provider.Stub(x => x.GetDeductionCalculators(new Location()))
                .WhenCalled(call => call.ReturnValue = calculators)
                .Return(null)
                .Repeat.Any();

            return provider;
        }

        [Test]
        public void CalculateNetAmount_SubstractsDeductionsFromGrossAmountCorrectly()
        {
            // Arrange
            var location = new Location();
            var currency = new Currency() { Name = "Eur", Symbol = "EurSymbol" };
            var grossAmount = new Money(5000, currency);
            var reduceAmount = new Money(1000, currency);
            var expected = new Money(3000, currency);

            var deductionCalculator = MockRepository.GenerateStub<IDeductionCalculator>();
            deductionCalculator.Stub(x => x.CalculateDeduction(grossAmount))
                .Repeat.Twice()
                .Return(reduceAmount);

            var source = new List<IDeductionCalculator>() { deductionCalculator, deductionCalculator };
            var provider = StubDataProvider(source);

            var calculator = new TaxCalculator();
            calculator.DataProvider = provider;

            // Act
            var netAmount = calculator.CalculateNetAmount(grossAmount, location);

            // Arrange
            Assert.AreEqual(expected, netAmount);
        }
    }
}
