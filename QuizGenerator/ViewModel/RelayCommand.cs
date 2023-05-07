using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuizGenerator.ViewModel
{
    class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (canExecute != null) CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (canExecute != null) CommandManager.RequerySuggested -= value;
            }
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        private Action<object> execute;
        public void Execute(object? parameter)
        {
            execute(parameter);
        }

        private Predicate<object> canExecute;
        public bool CanExecute(object? parameter)
        {
            return canExecute == null ? true : canExecute(parameter);
        }
    }
}
