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

        private double result = 0;
        private string currentValue = "0";
        private char sign;
        private double? left = null, right = null;
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
                    currentValue = "0"; 
                else
                if (CurrentValue == "0" || newInput)
                    currentValue = value;
                else
                    currentValue += value;         

                RaisePropertyChanged(nameof(CurrentValue));
            }
        }

        #region Commands
        async Task DigitCommandExecute(string parameter)
        {
            CurrentValue = parameter;
            newInput = false; 
        }

        async Task BackToZeroExecute()
        {
            CurrentValue = "CE";
            result = 0;
            left = right = null;
        }

        async Task SignCommandExecute(char parameter)
        {
            left = Convert.ToDouble(CurrentValue);
            sign = parameter;
            newInput = true;
        }

        async Task ResultCommandExecute()
        {
            right = Convert.ToDouble(CurrentValue);
            switch (sign)
            {
                case '+': result = CalculatorService.Add(left.Value, right.Value);
                    break;
                case '-':
                    result = CalculatorService.Substraction(left.Value, right.Value);
                    break;
                case '/':
                    {
                        var res = CalculatorService.Divide(left.Value, right.Value, out bool divedeOnZero);
                        if (divedeOnZero)
                        {
                            newInput = true;
                            CurrentValue = "Divide by zero!";
                            return;
                        }
                        else
                            result = res;
                    }
                    break;
                case '*':
                    result = CalculatorService.Muliply(left.Value, right.Value);
                    break;
            }

            newInput = true;
            left = right = null;
            CurrentValue = result.ToString();          
        }

        async Task BackspaceCommandExecute()
        {
            if (currentValue == "0")
                return;

            if (currentValue.Length == 1)
            {
                currentValue = "0";
            }
            else
            {
                currentValue = CurrentValue.Remove(CurrentValue.Length - 1);
            }

            RaisePropertyChanged(nameof(CurrentValue));
        }


        async Task PercentCommandExecute()
        {
            right = Convert.ToDouble(CurrentValue);
            result = CalculatorService.GetPercent(left.Value, right.Value);
            newInput = true;
            CurrentValue = result.ToString();           
        }

        async Task PowCommandExecute()
        {
           left = Convert.ToDouble(CurrentValue);  
           result = CalculatorService.Pow(left.Value);
           newInput = true;
           CurrentValue = result.ToString();      
        }

        async Task SqrtCommandExecute()
        {
            left = Convert.ToDouble(CurrentValue);
            result = CalculatorService.Sqrt(left.Value);
            newInput = true;
            CurrentValue = result.ToString();
        }

        async Task AddMinusCommandExecute()
        {
            if (currentValue[0] == '-')
               currentValue = currentValue.Remove(0, 1);
            else
               currentValue = currentValue.Insert(0, "-");

            RaisePropertyChanged(nameof(CurrentValue));
        }

        async Task OneDivideCommandExecute()
        {
            left = Convert.ToDouble(CurrentValue);
            var res = CalculatorService.Divide(1, left.Value, out bool divideByZero);
            newInput = true;
            if (divideByZero)
            {           
                CurrentValue = "Divide by zero!";
                return;
            }
            else
                CurrentValue = res.ToString();  
        }

        bool CanResultCalculate() => left.HasValue && newInput == false;

        bool UnarCanExecute() => currentValue != "0" && !right.HasValue;  
        #endregion
    }
}