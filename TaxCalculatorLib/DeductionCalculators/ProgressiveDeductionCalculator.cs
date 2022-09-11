using System;
using AndriiCo.TaxCalculatorLib.Entities;

namespace AndriiCo.TaxCalculatorLib.DeductionCalculators
{
    /// <summary>
    /// Deduction calculator for progressive tax
    /// </summary>
    public class ProgressiveDeductionCalculator: IDeductionCalculator
    {
        private readonly Rate _basicRate;
        private readonly Rate _higherRate;
        private readonly Money _threshold;

        /// <summary>
        /// Creates an instance of <see cref="ProgressiveDeductionCalculator"/>
        /// </summary>
        /// <param name="name">Deduction name</param>
        /// <param name="basicRate">Basic rate</param>
        /// <param name="higherRate">Higher rate</param>
        /// <param name="threshold">Threshold after which higher rate is applied</param>
        public ProgressiveDeductionCalculator(string name, Rate basicRate, Rate higherRate, Money threshold)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (basicRate == null) throw new ArgumentNullException(nameof(basicRate));
            if (higherRate == null) throw new ArgumentNullException(nameof(higherRate));
            if (threshold == null) throw new ArgumentNullException(nameof(threshold));

            DeductionName = name;
            _basicRate = basicRate;
            _higherRate = higherRate;
            _threshold = threshold;
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

            if (!Equals(_threshold.Currency, grossAmount.Currency))
                throw new ArgumentException("Can't operate with two different currencies!");

            Money totalDeduction;
            if (_threshold >= grossAmount)
            {
                totalDeduction = grossAmount.MultipyByNumber(_basicRate.RateValue);
            }
            else
            {
                var firstPart = _threshold.MultipyByNumber(_basicRate.RateValue);
                var secondPart = (grossAmount - _threshold).MultipyByNumber(_higherRate.RateValue);
                totalDeduction = firstPart + secondPart;
            }

            return totalDeduction;
        }
    }
}
