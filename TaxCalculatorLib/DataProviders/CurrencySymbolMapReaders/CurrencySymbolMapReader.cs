using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AndriiCo.TaxCalculatorLib.Entities;
using AndriiCo.TaxCalculatorLib.Exceptions;

namespace AndriiCo.TaxCalculatorLib.DataProviders.CurrencySymbolMapReaders
{
    /// <summary>
    // Reads mapping betweet a currency name and its alternative name from XML file
    /// </summary>
    public class CurrencySymbolMapReader: ICurrencySymbolMapReader
    {
        private const string Currency = "Currency";
        private const string CurrencyName = "name";
        private const string CurrencySymbol = "symbol";

        private readonly string _sourceFile;

        /// <summary>
        /// Creates an instance of <see cref="CurrencySymbolMapReader"/>
        /// </summary>
        /// <param name="sourceFile">Path to XML file</param>
        public CurrencySymbolMapReader(string sourceFile)
        {
            if (string.IsNullOrEmpty(sourceFile)) throw new ArgumentNullException(nameof(sourceFile));

            _sourceFile = sourceFile;
        }

        /// <summary>
        /// String that represents currency in an alternative format
        /// </summary>
        /// <param name="payerCurrency">Currency</param>
        /// <returns>Returns a string that represents currency in an alternative format</returns>
        public string GetCurrencySymbol(Currency payerCurrency)
        {
            if (payerCurrency == null) throw new ArgumentNullException(nameof(payerCurrency));

            XElement xelement = XElement.Load(_sourceFile);

            IEnumerable<XElement> currencies = xelement.Elements(Currency);
            var currency = currencies.SingleOrDefault(x => x.Attribute(CurrencyName)?.Value == payerCurrency.Name);

            if (currency == null)
                throw new EntityNotFoundException(
                    $"Couldn't find alternative representation of currency {payerCurrency.Name}");

            var symbol = currency.Attribute(CurrencySymbol)?.Value;

            return symbol;
        }
    }
}
