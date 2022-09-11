using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Practices.Unity;
using AndriiCo.TaxCalculatorLib.DataProviders.LocationCurrencyMapReaderReaders;
using AndriiCo.TaxCalculatorLib.DeductionCalculators;
using AndriiCo.TaxCalculatorLib.Entities;
using AndriiCo.TaxCalculatorLib.Enums;
using AndriiCo.TaxCalculatorLib.Exceptions;

namespace AndriiCo.TaxCalculatorLib.DataProviders
{
    /// <summary>
    /// Data provides that returns instances of objects to calculate tax deductions defined in XML files
    /// </summary>
    public class DataProvider : IDataProvider
    {
        private const string DeductionName = "name";
        private const string DeductioType = "type";
        private const string RatePercent = "ratePercent";
        private const string BasicRatePercent = "basicRatePercent";
        private const string HigherRatePercent = "higherRatePercent";
        private const string Threshold = "threshold";
        private const string Location = "Location";
        private const string LocationName = "name";
        private const string TaxDeductions = "TaxDeductions";

        private readonly string _sourceFile;

        /// <summary>
        /// Instance of the class that implements <see cref="ILocationCurrencyMapReader"/>
        /// </summary>
        [Dependency]
        public ILocationCurrencyMapReader LocationCurrencyMapReader { get; set; }

        /// <summary>
        /// Creates an instance of XmlDeductionDataProvider
        /// </summary>
        /// <param name="sourceFile">Path to XML file that defines tax rules</param>
        
        public DataProvider(string sourceFile)
        {
            if (string.IsNullOrEmpty(sourceFile)) throw new ArgumentNullException(nameof(sourceFile));

            _sourceFile = sourceFile;
        }

        /// <summary>
        /// Enumeration of objects that implement <see cref="IDeductionCalculator"/>
        /// </summary>
        /// <returns>Returns enumeration of objects that implement <see cref="IDeductionCalculator"/></returns>
        public IEnumerable<IDeductionCalculator>  GetDeductionCalculators(Location payerLocation)
        {
            if (payerLocation == null) throw new ArgumentNullException(nameof(payerLocation));

            var currency = LocationCurrencyMapReader.GetCurrency(payerLocation); 
            var taxDeductions = GetTaxDeductionsPerLocation(payerLocation);
            if (taxDeductions != null)
            {
                foreach (var deduction in taxDeductions)
                {
                    string nameStr = deduction.Attribute(DeductionName)?.Value;
                    string typeStr = deduction.Attribute(DeductioType)?.Value;

                    TaxDeductionType deductionType;
                    Enum.TryParse(typeStr, out deductionType);
                    switch (deductionType)
                    {
                        case TaxDeductionType.FlatDeduction:
                            var ratePercent = GetDecimalValue(deduction, RatePercent);
                            yield return new FlatDeductionCalculator(nameStr, new Rate(ratePercent));
                            break;
                        case TaxDeductionType.ProgressiveDeduction:
                            var basicRatePercent = GetDecimalValue(deduction, BasicRatePercent);
                            var higherRatePercent = GetDecimalValue(deduction, HigherRatePercent);
                            var threshold = GetDecimalValue(deduction, Threshold);
                            yield return new ProgressiveDeductionCalculator(nameStr, new Rate(basicRatePercent),
                                new Rate(higherRatePercent), new Money(threshold, currency));
                            break;
                        default:
                            throw new NotImplementedException($"{deductionType} is not implemented!");
                    }
                }
            }
        }

        private IEnumerable<XElement> GetTaxDeductionsPerLocation(Location payerLocation)
        {
            var xelement = XElement.Load(_sourceFile);
            var locations = xelement.Elements(Location);
            var location = locations.SingleOrDefault(x => x.Attribute(LocationName)?.Value == payerLocation.Name);

            if (location == null) throw new EntityNotFoundException($"Couldn't find tax rules for {payerLocation}");

            return location.Element(TaxDeductions)?.Elements();
        }

        private decimal GetDecimalValue(XElement deduction, string valueName)
        {
            decimal value = 0;

            var basicRateStr = deduction.Attribute(valueName)?.Value;
            if (basicRateStr != null)
            {
                value = decimal.Parse(basicRateStr);
            }

            return value;
        }
    }
}
