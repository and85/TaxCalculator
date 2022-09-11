using System;

namespace AndriiCo.TaxCalculatorLib.Entities
{
    /// <summary>
    /// Class to represent tax rate
    /// </summary>
    public class Rate
    {
        private const int OneHundredPercents = 100;
        private const decimal LowerBound = 0;
        private const decimal HigherBound = 100;

        private readonly decimal _percent;

        /// <summary>
        /// Creates an instance of <see cref="Rate"/>
        /// </summary>
        /// <param name="percent">Representation of rate in percents</param>
        public Rate(decimal percent)
        {
            if (percent < LowerBound || percent > HigherBound)
                throw new ArgumentException($"Percent should be in a range between {LowerBound} and {HigherBound}!");

            _percent = percent;
        }

        /// <summary>
        /// Representation of rate used for calculations
        /// </summary>
        public decimal RateValue => _percent / OneHundredPercents;
    }
}
