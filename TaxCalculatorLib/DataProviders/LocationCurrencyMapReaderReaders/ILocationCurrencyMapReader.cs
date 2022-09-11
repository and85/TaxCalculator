using AndriiCo.TaxCalculatorLib.Entities;

namespace AndriiCo.TaxCalculatorLib.DataProviders.LocationCurrencyMapReaderReaders
{
    /// <summary>
    /// An interface for readers that find what currency is used in different countries
    /// </summary>
    public interface ILocationCurrencyMapReader
    {
        /// <summary>
        /// Currency used in a defined location
        /// </summary>
        /// <param name="location">Location name</param>
        /// <returns>Returns currency used in a defined location</returns>
        Currency GetCurrency(Location location);
    }
}
