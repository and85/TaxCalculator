using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Practices.Unity;
using AndriiCo.TaxCalculatorLib.DataProviders.CurrencySymbolMapReaders;
using AndriiCo.TaxCalculatorLib.Entities;
using AndriiCo.TaxCalculatorLib.Exceptions;

namespace AndriiCo.TaxCalculatorLib.DataProviders.LocationCurrencyMapReaderReaders
{
    /// <summary>
    /// Reads XML file that defines mapping between locations and currencies
    /// </summary>
    public class LocationCurrencyMapReader: ILocationCurrencyMapReader
    {
        private const string Location = "Location";
        private const string LocationName = "name";
        private const string Currency = "currency";

        private readonly string _sourceFile;

        /// <summary>
        /// Reader that finds what is an alternative way to represent currency
        /// </summary>
        [Dependency]
        public ICurrencySymbolMapReader CurrencySymbolMapReader { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="LocationCurrencyMapReader"/>
        /// </summary>
        /// <param name="sourceFile">Path to a file that defines what currency is used in different countries</param>
        public LocationCurrencyMapReader(string sourceFile)
        {
            if (string.IsNullOrEmpty(sourceFile)) throw new ArgumentNullException(nameof(sourceFile));

            _sourceFile = sourceFile;
        }

        public Currency GetCurrency(Location payerLocation)
        {
            if (payerLocation == null) throw new ArgumentNullException(nameof(payerLocation));

            XElement xelement = XElement.Load(_sourceFile);

            IEnumerable<XElement> locations = xelement.Elements(Location);
            var location = locations.SingleOrDefault(x => x.Attribute(LocationName)?.Value == payerLocation.Name);

            if (location == null) throw new EntityNotFoundException($"Currency not found for location {payerLocation}");

            var currencyStr = location.Attribute(Currency)?.Value;
            var currency = new Currency {Name = currencyStr};
            currency.Symbol = CurrencySymbolMapReader.GetCurrencySymbol(currency);

            return currency;
        }
    }
}
