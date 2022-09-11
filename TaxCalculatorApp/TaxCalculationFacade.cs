using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using AndriiCo.TaxCalculatorLib;
using AndriiCo.TaxCalculatorLib.DataProviders;
using AndriiCo.TaxCalculatorLib.DataProviders.CurrencySymbolMapReaders;
using AndriiCo.TaxCalculatorLib.DataProviders.LocationCurrencyMapReaderReaders;
using AndriiCo.TaxCalculatorLib.Entities;

namespace AndriiCo.TaxCalculatorApp
{
    /// <summary>
    /// Facade which provides single entry point for all calculations
    /// </summary>
    public class TaxCalculationFacade
    {
        private readonly string _taxRulesPath;
        private readonly string _locationsPath;
        private readonly string _currenciesPath;

        /// <summary>
        /// Creates an instance of <see cref="TaxCalculationFacade"/>
        /// </summary>
        /// <param name="taxRulesPath">Path to a file with business rules that define taxation in different countries</param>
        /// <param name="locationsPath">Path to a file that defines what currency is used in different countries</param>
        /// <param name="currenciesPath">Path to a file which defines an alternative way to show a currency</param>
        public TaxCalculationFacade(string taxRulesPath, string locationsPath, string currenciesPath)
        {
            _taxRulesPath = taxRulesPath;
            _locationsPath = locationsPath;
            _currenciesPath = currenciesPath;
        }

        private IUnityContainer RegisterTypes()
        {
            IUnityContainer unitycontainer = new UnityContainer();

            unitycontainer.RegisterTypes(
                AllClasses.FromLoadedAssemblies(),
                WithMappings.FromMatchingInterface,
                WithName.Default);

            unitycontainer.RegisterType<ICurrencySymbolMapReader, CurrencySymbolMapReader>(
                new InjectionConstructor(_currenciesPath));

            unitycontainer.RegisterType<ILocationCurrencyMapReader, LocationCurrencyMapReader>(
                new InjectionConstructor(_locationsPath));

            unitycontainer.RegisterType<IDataProvider, DataProvider>(
                new InjectionConstructor(_taxRulesPath));
            

            return unitycontainer;
        }

        /// <summary>
        /// Gross salary
        /// </summary>
        public Money GrossAmount { get; private set; }

        /// <summary>
        /// List of tax deductions
        /// </summary>
        public IEnumerable<Deduction> Deductions { get; private set; }

        /// <summary>
        /// Take home salary
        /// </summary>
        public Money NetAmount { get; private set; }

        /// <summary>
        /// Does all required calculations for TaxCulculatorApp
        /// </summary>
        /// <param name="hoursWorked">Hours worked</param>
        /// <param name="hourlyRate">Hourly rate</param>
        /// <param name="location">Tax payer location</param>
        public void DoCalculations(uint hoursWorked, uint hourlyRate, Location location)
        {
            if (location == null)
                throw new ArgumentNullException(nameof(location));

            var unityContainer = RegisterTypes();

            var locationCurrencyMapReader = unityContainer.Resolve<ILocationCurrencyMapReader>();
            var taxCalculator = unityContainer.Resolve<ITaxCalculator>();

            var currency = locationCurrencyMapReader.GetCurrency(location);

            GrossAmount = taxCalculator.CalculateGrossAmount(hoursWorked, hourlyRate, currency);
            Deductions = taxCalculator.CalculateDeductions(GrossAmount, location);
            NetAmount = taxCalculator.CalculateNetAmount(GrossAmount, location);
        }
    }
}
