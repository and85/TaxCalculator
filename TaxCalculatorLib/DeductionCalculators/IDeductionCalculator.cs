using AndriiCo.TaxCalculatorLib.Entities;

namespace AndriiCo.TaxCalculatorLib.DeductionCalculators
{
    /// <summary>
    /// Interface for classes that calculate tax deduction
    /// </summary>
    public interface IDeductionCalculator
    {
        /// <summary>
        /// Deduction name
        /// </summary>
        string DeductionName { get; }

        /// <summary>
        /// Amount of money taken by deduction
        /// </summary>
        /// <param name="grossAmount">Gross salary from which deduction should be calculated</param>
        /// <returns>Returns amount of money taken by deduction</returns>
        Money CalculateDeduction(Money grossAmount);
    }
}
