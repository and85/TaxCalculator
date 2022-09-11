using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using AndriiCo.TaxCalculatorLib.DataProviders;
using AndriiCo.TaxCalculatorLib.Entities;

namespace AndriiCo.TaxCalculatorLib
{
    /// <summary>
    /// Calculates taxes
    /// </summary>
    public class TaxCalculator: ITaxCalculator
    {
        /// <summary>
        /// Data provider that returns tax deductions for a specific location
        /// </summary>
        [Dependency]
        public IDataProvider DataProvider { get; set; }

        /// <summary>
        /// Gross salary based on hours worked and hourly rate
        /// </summary>
        /// <param name="hoursWorked">Hours worked</param>
        /// <param name="hourlyRate">Hourly rate</param>
        /// <param name="currency">Currency</param>
        /// <returns>Returns gross salary based on hours worked and hourly rate</returns>
        public Money CalculateGrossAmount(uint hoursWorked, uint hourlyRate, Currency currency)
        {
            if (currency == null) throw new ArgumentNullException(nameof(currency));

            return new Money(hoursWorked * hourlyRate, currency);
        }

        /// <summary>
        /// Enumeration of calculated tax deductions
        /// </summary>
        /// <param name="grossAmount">Gross salary</param>
        /// <param name="payerLocation">Payer location</param>
        /// <returns>Returns enumeration of calculated tax deductions</returns>
        public IEnumerable<Deduction> CalculateDeductions(Money grossAmount, Location payerLocation)
        {
            if (grossAmount == null) throw new ArgumentNullException(nameof(grossAmount));
            if (payerLocation == null) throw new ArgumentNullException(nameof(payerLocation));

            foreach (var calculator in DataProvider.GetDeductionCalculators(payerLocation))
            {
                var deduction = new Deduction()
                {
                    Name = calculator.DeductionName,
                    Amount = calculator.CalculateDeduction(grossAmount)
                };

                yield return deduction;
            } 
        }

        /// <summary>
        /// Amount of money taken home after taxes
        /// </summary>
        /// <param name="grossAmount">Gross salary</param>
        /// <param name="payerLocation">Payer location</param>
        /// <returns>Returns amount of money taken home after taxes</returns>
        public Money CalculateNetAmount(Money grossAmount, Location payerLocation)
        {
            if (grossAmount == null) throw new ArgumentNullException(nameof(grossAmount));
            if (payerLocation == null) throw new ArgumentNullException(nameof(payerLocation));

            var deductionAmount = new Money(0, grossAmount.Currency);
            foreach (var calculator in DataProvider.GetDeductionCalculators(payerLocation))
            {
                deductionAmount += calculator.CalculateDeduction(grossAmount);
            }

            var netAmount = grossAmount - deductionAmount;
            return netAmount;
        }
    }
}
