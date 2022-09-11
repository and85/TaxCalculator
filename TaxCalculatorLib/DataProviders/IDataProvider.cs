using System.Collections.Generic;
using AndriiCo.TaxCalculatorLib.DeductionCalculators;
using AndriiCo.TaxCalculatorLib.Entities;

namespace AndriiCo.TaxCalculatorLib.DataProviders
{
    /// <summary>
    /// An interface for data provides that return instances of objects to calculate tax deductions
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// Enumeration of objects that implement <see cref="IDeductionCalculator"/>
        /// </summary>
        /// <returns>Returns enumeration of objects that implement <see cref="IDeductionCalculator"/></returns>
        IEnumerable<IDeductionCalculator> GetDeductionCalculators(Location payerLocation);
    }
}
