using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using AndriiCo.TaxCalculatorApp;
using AndriiCo.TaxCalculatorLib.Entities;
using AndriiCo.TaxCalculatorLib.Exceptions;

namespace AndriiCo.Tests.TaxCalculatorIntegrationTests
{
    /// <summary>
    /// Integration tests for TaxCalculator application. 
    /// Involves testing of business rules defined in xml files and covers xml file readers
    /// </summary>
    [TestFixture]
    public class TaxCalculatorIntegrationTests
    {
        private string _taxRulesPath = @"..\..\..\..\TaxCalculatorApp\DataSources\TaxDeductionRules.xml";
        private string _lcationsPath = @"..\..\..\..\TaxCalculatorApp\DataSources\LocationCurrencyMap.xml";
        private string _currenciesPath = @"..\..\..\..\TaxCalculatorApp\DataSources\CurrencySymbolMap.xml";

        [OneTimeSetUp]
        public void Init()
        {
            // repoint tests to a correct path, otherwise we would need to turn off shadow copying in Resharper
            _taxRulesPath = Path.Combine(AssemblyLocation(), _taxRulesPath);
            _lcationsPath = Path.Combine(AssemblyLocation(), _lcationsPath);
            _currenciesPath = Path.Combine(AssemblyLocation(), _currenciesPath);
        }

        [Test]
        [TestCase("Ireland", "Eur", "€", 1600, 160, 10)]
        [TestCase("Italy", "Eur", "€", 900, 100, 9)]
        [TestCase("Germany", "Eur", "€", 1440, 160, 9)]
        public void DoCalculations_CalculateGrossAmount(
            string payerLocation, 
            string expectedCurrency, 
            string expectedCurrencySymbol, 
            decimal expectedGross,
            int hoursWorked,
            int hourlyRate)
        {
            // Arrange
            var expected = new Money(expectedGross, new Currency() {Name = expectedCurrency, Symbol = expectedCurrencySymbol });
            
            var location = new Location() {Name = payerLocation };
            var service = new TaxCalculationFacade(_taxRulesPath, _lcationsPath, _currenciesPath);

            // Act
            service.DoCalculations((uint)hoursWorked, (uint)hourlyRate, location);

            var actual = service.GrossAmount;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("Ireland", 3, 160, 10)]
        [TestCase("Italy", 2, 160, 10)]
        [TestCase("Germany", 2, 160, 10)]
        public void DoCalculations_AllDeductionsDefined(
            string payerLocation,
            int expectedDeductionsCount,
            int hoursWorked,
            int hourlyRate)
        {
            // Arrange
            var location = new Location() { Name = payerLocation };
            var service = new TaxCalculationFacade(_taxRulesPath, _lcationsPath, _currenciesPath);

            // Act
            service.DoCalculations((uint)hoursWorked, (uint)hourlyRate, location);

            var actual = service.Deductions.Count();

            // Assert
            Assert.AreEqual(expectedDeductionsCount, actual);
        }

        [Test, TestCaseSource(typeof(TaxCalculatorIntegrationTests), nameof(NetAmountTestCases))]
        public void DoCalculations_CalculateNetAmount(
            string payerLocation,
            string expectedCurrency,
            string expectedCurrencySymbol,
            decimal expectedGross,
            int hoursWorked,
            int hourlyRate)
        {
            // Arrange
            var expected = new Money(expectedGross, new Currency() { Name = expectedCurrency, Symbol = expectedCurrencySymbol });

            var location = new Location() { Name = payerLocation };
            var service = new TaxCalculationFacade(_taxRulesPath, _lcationsPath, _currenciesPath);

            // Act
            service.DoCalculations((uint)hoursWorked, (uint)hourlyRate, location);

            var actual = service.NetAmount;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DoCalculations_ThrowsEntityNotFoundException()
        {
            // Arrange
            string payerLocation = "DreamLand";
            uint hoursWorked = 20;
            uint hourlyRate = 30;

            var location = new Location() { Name = payerLocation };
            var service = new TaxCalculationFacade(_taxRulesPath, _lcationsPath, _currenciesPath);

            // Act and Assert
            Assert.Throws<EntityNotFoundException>(() => service.DoCalculations(hoursWorked, hourlyRate, location));
        }

        public static IEnumerable NetAmountTestCases
        {
            get
            {
                yield return new TestCaseData("Ireland", "Eur", "€", 320m, 50, 10)
                    .SetName("Calculate take home money for Ireland when gross amount is less than threshold");

                yield return new TestCaseData("Ireland", "Eur", "€", 431m, 70, 10)
                    .SetName("Calculate take home money for Ireland when gross amount is greater than threshold");

                yield return new TestCaseData("Italy", "Eur", "€", 658.10m, 50, 20)
                    .SetName("Calculate take home money for Italy");

                yield return new TestCaseData("Germany", "Eur", "€", 219m, 30, 10)
                    .SetName("Calculate take home money for Germany when gross amount is less than threshold");

                yield return new TestCaseData("Germany", "Eur", "€", 358m, 50, 10)
                    .SetName("Calculate take home money for Germany when gross amount is greater than threshold");                
            }
        }

        private string AssemblyLocation()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var codebase = new Uri(assembly.CodeBase);
            var path = Path.GetDirectoryName(codebase.LocalPath);
            
            return path;
        }

    }
}
