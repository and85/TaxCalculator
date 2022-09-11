using System;
using System.Collections.Generic;
using System.Text;
using AndriiCo.TaxCalculatorLib.Entities;
using AndriiCo.TaxCalculatorLib.Exceptions;

namespace AndriiCo.TaxCalculatorApp
{
    class Program
    {
        /// <summary>
        /// Path to a file with business rules that define taxation in different countries
        /// </summary>
        const string TaxRulesPath = @".\DataSources\TaxDeductionRules.xml";

        /// <summary>
        /// Path to a file that defines what currency is used in different countries
        /// </summary>
        const string LocationsPath = @".\DataSources\LocationCurrencyMap.xml";

        /// <summary>
        /// Path to a file which defines an alternative way to show a currency
        /// </summary>
        const string CurrenciesPath = @".\DataSources\CurrencySymbolMap.xml";

        private const int SuccessCode = 0;
        private const int FailureCode = 1;

        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            uint hoursWorked = 0;
            uint hourlyRate = 0;
            Location location = new Location();
            ReadInput(ref hoursWorked, ref hourlyRate, ref location);
            
            TaxCalculationFacade service = null;
            try
            {
                service = new TaxCalculationFacade(TaxRulesPath, LocationsPath, CurrenciesPath);
                service.DoCalculations(hoursWorked, hourlyRate, location);
            }
            catch (EntityNotFoundException e)
            {
                Console.WriteLine(e);
                ApplicationEnd(FailureCode);
            }

            if (service != null)
                PrintOutput(location, service.GrossAmount, service.Deductions, service.NetAmount);

            ApplicationEnd(SuccessCode);
        }

        private static void ReadInput(ref uint hoursWorked, ref uint hourlyRate, ref Location location)
        {
            try
            {
                Console.Write("Please enter the hours worked:");
                var hoursWorkedStr = Console.ReadLine();
                hoursWorked = uint.Parse(hoursWorkedStr);

                Console.Write("Please enter the hourly rate:");
                var hourlyRateStr = Console.ReadLine();
                hourlyRate = uint.Parse(hourlyRateStr);

                Console.Write("Please enter the employee’s location:");
                var locationStr = Console.ReadLine();

                location = new Location() {Name = locationStr};
            }
            catch 
            {
                Console.WriteLine("Wrong input!");
                ApplicationEnd(FailureCode);
            }
        }

        private static void PrintOutput(Location location, Money grossAmount, IEnumerable<Deduction> deductions, Money netAmount)
        {
            // change encoding to show euro symbol
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine();
            Console.WriteLine($"Employee location: {location}");
            Console.WriteLine();
            Console.WriteLine($"Gross Amount: {grossAmount}");
            Console.WriteLine();
            Console.WriteLine("Less deductions");
            Console.WriteLine();
            foreach (var deduction in deductions)
            {
                Console.WriteLine($"{deduction.Name}: {deduction.Amount}");
            }
            Console.WriteLine($"Net Amount: {netAmount}");
            Console.WriteLine();
        }

        private static void ApplicationEnd(int exitCode)
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            Environment.Exit(exitCode);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine($"Unhandled exception has happen! Exception: {e.ExceptionObject}");
            ApplicationEnd(FailureCode);
        }
    }
}
