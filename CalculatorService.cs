using System;

namespace SharedCalculator
{
    public static class CalculatorService
    {
        public static double Add(double a, double b) => a + b;
        public static double Subtraction(double a, double b) => a - b;
        public static double Divide(double a, double b, out bool diveOnZero)
        {
            if (b == 0)
            {
                diveOnZero = true;
                return 0; 
            }

            diveOnZero = false;
            return a / b;
        }
        public static double Muliply(double a, double b) => a * b;
        public static double Pow(double n) => Math.Pow(n, 2);
        public static double Sqrt(double n) => Math.Sqrt(n);   
        public static double GetPercent(double sum, double percent) => sum / 100 * percent; 
    }
}
