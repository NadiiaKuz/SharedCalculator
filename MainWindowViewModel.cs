using DevExpress.Mvvm;
using System;
using System.Threading.Tasks;

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

        double result = 0;
        string currentValue = "0";
        char sign;
        double? left = null, right = null;
        bool newInput = true;

        public MainWindowViewModel()
        {
            InputDigitCommand = new AsyncCommand<string>(DigitCommandExecute);
            BackToZeroCommand = new AsyncCommand(BackToZeroExecute);
            SignCommand = new AsyncCommand<char>(SignCommandExecute);
            ResultCommand = new AsyncCommand(ResultCommandExecute, CanResultCalculate);
            BackspaceCommand = new AsyncCommand(BackspaceCommandExecute);
            PercentCommand = new AsyncCommand(PercentCommandExecute);
            PowCommand = new AsyncCommand(PowCommandExecute, UnarCanExecute);
            SqrtCommand = new AsyncCommand(SqrtCommandExecute, UnarCanExecute);
            AddMinusCommand = new AsyncCommand(AddMinusCommandExecute, UnarCanExecute);
            OneDivideCommand = new AsyncCommand(OneDivideCommandExecute, UnarCanExecute);
        }      

        public string CurrentValue
        {
            get => currentValue;
            set 
            {
                if (value == "CE")
                { 
                    currentValue = "0"; 
                }
                else if (CurrentValue == "0" || newInput)
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
            left = Convert.ToDouble(CurrentValue);
            sign = parameter;
            newInput = true;

            return Task.CompletedTask;
        }

        Task ResultCommandExecute()
        {
            right = Convert.ToDouble(CurrentValue);
            switch (sign)
            {
                case '+': result = CalculatorService.Add(left.Value, right.Value);
                    break;
                case '-':
                    result = CalculatorService.Subtraction(left.Value, right.Value);
                    break;
                case '/':
                    {
                        var res = CalculatorService.Divide(left.Value, right.Value, out bool divedeOnZero);
                        if (divedeOnZero)
                        {
                            newInput = true;
                            CurrentValue = "Divide by zero!";
                            return Task.CompletedTask; ;
                        }
                        else
                            result = res;
                    }
                    break;
                case '*':
                    result = CalculatorService.Multiply(left.Value, right.Value);
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
            result = CalculatorService.GetPercent(left.Value, right.Value);
            newInput = true;
            CurrentValue = result.ToString();

            return Task.CompletedTask;
        }

        Task PowCommandExecute()
        {
           left = Convert.ToDouble(CurrentValue);  
           result = CalculatorService.Pow(left.Value);
           newInput = true;
           CurrentValue = result.ToString();

            return Task.CompletedTask;
        }

        Task SqrtCommandExecute()
        {
            left = Convert.ToDouble(CurrentValue);
            result = CalculatorService.Sqrt(left.Value);
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
            var res = CalculatorService.Divide(1, left.Value, out bool divideByZero);
            newInput = true;
            
            if (divideByZero)
            {
                CurrentValue = "Divide by zero!";
                return Task.CompletedTask;
            }
            else
            {
                CurrentValue = res.ToString();
            }

            return Task.CompletedTask;
        }

        bool CanResultCalculate() => left.HasValue && newInput == false;

        bool UnarCanExecute() => currentValue != "0" && !right.HasValue;  
        #endregion
    }
}