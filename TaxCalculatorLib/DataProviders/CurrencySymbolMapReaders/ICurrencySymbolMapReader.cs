using AndriiCo.TaxCalculatorLib.Entities;

namespace AndriiCo.TaxCalculatorLib.DataProviders.CurrencySymbolMapReaders
{
    /// <summary>
    /// Interface to read mapping betweet a currency name and its alternative name 
    /// </summary>
    public interface ICurrencySymbolMapReader
    {
        /// <summary>
        /// Symbol that represents currency alternative name
        /// </summary>
        /// <param name="currency">Currency</param>
        /// <returns>Returns a symbol that represents currency alternative name</returns>
        string GetCurrencySymbol(Currency currency);
    }
}
