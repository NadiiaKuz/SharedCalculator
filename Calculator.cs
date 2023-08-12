namespace SharedCalculator
{
    static class Calculator
    {
        public static double Mul(double a, double b) =>
            a * b;

        public static double Div(double a, double b, out bool isDividedByZero)
        {
            isDividedByZero = false;
            if (b == 0)
            {
                isDividedByZero = true;
                return 0;
            }
            return a / b;
        }

        public static double Add(double a, double b) =>
            a + b;

        public static double Sub(double a, double b) =>
            a - b;
        public static double GetPercent(double a, double percent) =>
            a / 100 * percent;
    }
}
