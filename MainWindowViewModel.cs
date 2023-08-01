using DevExpress.Mvvm;
using System;
using System.Threading.Tasks;
using System.Globalization;

namespace SharedCalculator
{
    public class MainWindowViewModel : ViewModelBase 
    {
        public IAsyncCommand InputDigitCommand { get; }
        public IAsyncCommand BackToZeroCommand { get; }   
        public IAsyncCommand SignCommand { get; }
        public IAsyncCommand ResultCommand { get; } 
        public IAsyncCommand BackspaceCommand { get; }
        public IAsyncCommand PercentCommand { get; }
        public IAsyncCommand PowCommand { get; }
        public AsyncCommand SqrtCommand { get; }
        public AsyncCommand AddMinusCommand { get; }   
        public AsyncCommand OneDivideCommand { get; }   

        private const string ErrorMessage = "Divide by Zero!";
        private double result = 0;
        private string currentValue = "0";
        private char sign;
        private double? left = null, right = null;
        private bool newInput = true;

        public MainWindowViewModel()
        {
            InputDigitCommand = new AsyncCommand<string>(DigitCommandExecute);
            BackToZeroCommand = new AsyncCommand(BackToZeroExecute);
            SignCommand = new AsyncCommand<char>(SignCommandExecute);
            ResultCommand = new AsyncCommand(ResultCommandExecute, CanResultCalculate);
            BackspaceCommand = new AsyncCommand(BackspaceCommandExecute);
            PercentCommand = new AsyncCommand(PercentCommandExecute, CanResultCalculate);
            PowCommand = new AsyncCommand(PowCommandExecute, UnaryCanExecute);
            SqrtCommand = new AsyncCommand(SqrtCommandExecute, UnaryCanExecute);
            AddMinusCommand = new AsyncCommand(AddMinusCommandExecute, UnaryCanExecute);
            OneDivideCommand = new AsyncCommand(OneDivideCommandExecute /*, UnaryCanExecute*/);
        }      
        
        public string CurrentValue
        {
            get { return currentValue; }
            set 
            {
                if (value == "CE")
                { 
                    currentValue = "0"; 
                }
                else if ((CurrentValue == "0" || newInput) && value != ".")
                {
                    currentValue = value;
                }
                else
                {
                    currentValue += value;
                }

                RaisePropertyChanged(nameof(CurrentValue));
            }
        }

        #region Commands
        Task DigitCommandExecute(string parameter)
        {
            CurrentValue = parameter;
            newInput = false;

            return Task.CompletedTask;
        }

        Task BackToZeroExecute()
        {
            CurrentValue = "CE";
            result = 0;
            left = right = null;

            return Task.CompletedTask;
        }

        Task SignCommandExecute(char parameter)
        {
            left = Double.Parse(CurrentValue, CultureInfo.InvariantCulture);

            sign = parameter;
            newInput = true;

            return Task.CompletedTask;
        }

        Task ResultCommandExecute()
        {
            right = Convert.ToDouble(CurrentValue);
            switch (sign)
            {
                case '+': result = Add (left, right); // TODO: Implement and call adding method
                    break;
                case '-':
                    result = Subtract(left, right); 
                    break;
                case '/':
                    {
                        bool isDividedByZero;
                        result = DividingMethod((double) left, (double) right, out isDividedByZero);
                        if (isDividedByZero)
                        {
                            CurrentValue = ErrorMessage;
                            newInput = true;
                            return Task.CompletedTask;
                        }                                   
                    }
                    break;
                case '*':
                    result = Multiply(left, right); 
                    break;
            }

            newInput = true;
            left = right = null;
            CurrentValue = result.ToString();

            return Task.CompletedTask;
        }

        Task BackspaceCommandExecute()
        {
            if (currentValue == "0")
                return Task.CompletedTask;

            if (currentValue.Length == 1)
            {
                currentValue = "0";
            }
            else
            {
                currentValue = CurrentValue.Remove(CurrentValue.Length - 1);
            }

            RaisePropertyChanged(nameof(CurrentValue));

            return Task.CompletedTask;
        }      

        Task PercentCommandExecute()
        {
            right = Convert.ToDouble(CurrentValue);
            right = GetPercent(left.Value, right.Value);  
            currentValue = right.Value.ToString();

            RaisePropertiesChanged(nameof(CurrentValue));

            return Task.CompletedTask;
        }

        Task PowCommandExecute()
        {
            left = Convert.ToDouble(CurrentValue);
            result = Math.Pow(left.Value, 2); // Використовуємо метод Math.Pow() для піднесення до квадрату
            newInput = true;
            CurrentValue = result.ToString();

            return Task.CompletedTask;
        }


        Task SqrtCommandExecute()
        {
            left = Convert.ToDouble(CurrentValue);

            if ( left >= 0)
            {
                result = Math.Sqrt((double)left);
            }
            else
            {
                currentValue = "Invalid input";
                RaisePropertiesChanged(nameof(CurrentValue));
                return Task.CompletedTask;
            }

            newInput = true;
            CurrentValue = result.ToString();

            return Task.CompletedTask;
        }

        Task AddMinusCommandExecute()
        {
            currentValue = (currentValue[0] == '-') ? 
                  currentValue.Remove(0, 1) 
                : currentValue.Insert(0, "-");

            RaisePropertyChanged(nameof(CurrentValue));

            return Task.CompletedTask;
        }

        Task OneDivideCommandExecute()
        {
            left = Convert.ToDouble(CurrentValue);

            newInput = true;
            var res = DividingMethod(1, left.Value, out bool divByZero); // TODO - Call divide method 1 / left.Value
            CurrentValue = (divByZero) ? ErrorMessage : res.ToString();
            // TODO: Check on dividing method
           //if true current CurrentValue = message 
           // else CurrentValue = res.ToString

            return Task.CompletedTask;
        }

        bool CanResultCalculate() => left.HasValue && newInput == false;

        bool UnaryCanExecute() => currentValue != "0" && !right.HasValue;
        #endregion

        #region Calculator methods
        static double DividingMethod(double left, double right, out bool isDividedByZero)
        {
            isDividedByZero = false;
            if (right == 0)
            {
                isDividedByZero = true;
                return 0;
            }
            return left / right;
        }

        static double GetPercent(double value, double percent)
        {
            return value / 100 * percent;
        }

        double Subtract(double? num1,double? num2)=> 
                (double)(num1 - num2);

        static double Add (double? value1 , double? value2)
        {
            return (double) (value1 + value2);
        }

        double Multiply(double? num1, double? num2) => (double)(num1 * num2);
        
        #endregion
    }
}