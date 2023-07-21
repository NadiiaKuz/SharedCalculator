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
            PercentCommand = new AsyncCommand(PercentCommandExecute, CanResultCalculate);
            PowCommand = new AsyncCommand(PowCommandExecute, UnaryCanExecute);
            SqrtCommand = new AsyncCommand(SqrtCommandExecute, UnaryCanExecute);
            AddMinusCommand = new AsyncCommand(AddMinusCommandExecute, UnaryCanExecute);
            OneDivideCommand = new AsyncCommand(OneDivideCommandExecute, UnaryCanExecute);
        }      
        // qwdswded edfewd
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
                case '+': result = 0; // TODO: Implement and call adding method
                    break;
                case '-':
                    result = 0; // TODO: Implement and call subtracting method 
                    break;
                case '/':
                    {
                        result = 0; // TODO: Implement and call dividing method.
                                    // TODO: Check on zero - if divide by zero need message Divide by Zero!
                    }
                    break;
                case '*':
                    result = 0; // TODO: Implement and call multiply method
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
            right = 0; // TODO: Implement and call get percent method with right value form left.Value
            currentValue = right.Value.ToString();

            RaisePropertiesChanged(nameof(CurrentValue));

            return Task.CompletedTask;
        }

        Task PowCommandExecute()
        {
           left = Convert.ToDouble(CurrentValue);  
           result = 0; // TODO: Implement and call pow method
           newInput = true;
           CurrentValue = result.ToString();

            return Task.CompletedTask;
        }

        Task SqrtCommandExecute()
        {
            left = Convert.ToDouble(CurrentValue);
            result = 0; // TODO: Implement and call get sqrt method
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
            var res = 0; // TODO - Call divide method 1 / left.Value
            newInput = true;
            // TODO: Check on dividing method
           //if true current value = message 
           // else res.ToString

            return Task.CompletedTask;
        }

        bool CanResultCalculate() => left.HasValue && newInput == false;

        bool UnaryCanExecute() => currentValue != "0" && !right.HasValue;
        #endregion

        #region Calculator methods

        #endregion
    }
}