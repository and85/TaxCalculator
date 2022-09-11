using System;
using AndriiCo.TaxCalculatorLib.Entities;

namespace AndriiCo.TaxCalculatorLib.DeductionCalculators
{
    /// <summary>
    /// Deduction calculator for flat tax
    /// </summary>
    public class FlatDeductionCalculator: IDeductionCalculator
    {
        private readonly Rate _rate;

        /// <summary>
        /// Creates intance of <see cref="FlatDeductionCalculator"/>
        /// </summary>
        /// <param name="name">Deduction name</param>
        /// <param name="rate">Basic rate</param>
        public FlatDeductionCalculator(string name, Rate rate)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (rate == null) throw new ArgumentNullException(nameof(rate));

            DeductionName = name;
            _rate = rate;
        }

        /// <summary>
        /// Deduction name
        /// </summary>
        public string DeductionName { get; }

        /// <summary>
        /// Amount of money taken by deduction
        /// </summary>
        /// <param name="grossAmount">Gross salary from which deduction should be calculated</param>
        /// <returns>Returns amount of money taken by deduction</returns>
        public Money CalculateDeduction(Money grossAmount)
        {
            if (grossAmount == null) throw new ArgumentNullException(nameof(grossAmount));

            return grossAmount.MultipyByNumber(_rate.RateValue);
        }
    }
}
