using System.Collections.Generic;
using AndriiCo.TaxCalculatorLib.Entities;

namespace AndriiCo.TaxCalculatorLib
{
    /// <summary>
    /// An interface for classes that calculate taxes
    /// </summary>
    public interface ITaxCalculator
    {
        /// <summary>
        /// Gross salary based on hours worked and hourly rate
        /// </summary>
        /// <param name="hoursWorked">Hours worked</param>
        /// <param name="hourlyRate">Hourly rate</param>
        /// <param name="currency">Currency</param>
        /// <returns>Returns gross salary based on hours worked and hourly rate</returns>
        Money CalculateGrossAmount(uint hoursWorked, uint hourlyRate, Currency currency);

        /// <summary>
        /// Enumeration of tax deductions
        /// </summary>
        /// <param name="grossAmount">Gross salary</param>
        /// <param name="payerLocation">Payer location</param>
        /// <returns>Returns enumeration of tax deductions</returns>
        IEnumerable<Deduction> CalculateDeductions(Money grossAmount, Location payerLocation);

        /// <summary>
        /// Amount of money taken home after taxes
        /// </summary>
        /// <param name="grossAmount">Gross salary</param>
        /// <param name="payerLocation">Payer location</param>
        /// <returns>Returns amount of money taken home after taxes</returns>
        Money CalculateNetAmount(Money grossAmount, Location payerLocation);
    }
}
