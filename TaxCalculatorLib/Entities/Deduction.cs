namespace AndriiCo.TaxCalculatorLib.Entities
{
    /// <summary>
    /// Class to yax deduction
    /// </summary>
    public class Deduction
    {
        /// <summary>
        /// Deduction name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Amout of money taken by deduction
        /// </summary>
        public Money Amount { get; set; }
    }
}
