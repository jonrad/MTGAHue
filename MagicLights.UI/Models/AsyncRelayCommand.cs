using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MagicLights.UI.Models
{
    public class AsyncRelayCommand : ICommand
    {
        private readonly object syncObject = new object();

        private bool isExecuting = false;

        private readonly Func<object, Task> execute;

        public AsyncRelayCommand(Func<object, Task> execute)
        {
            this.execute = execute;
        }

        public AsyncRelayCommand(Func<Task> execute)
            : this(_ => execute())
        {
        }

        public event EventHandler CanExecuteChanged;

        public bool IsExecuting
        {
            get => isExecuting;
            set
            {
                isExecuting = value;
                CanExecuteChanged?.Invoke(this, new EventArgs());
            }
        }

        public bool CanExecute(object parameter)
        {
            return !isExecuting;
        }

        public void Execute(object parameter)
        {
            lock (syncObject)
            {
                if (isExecuting)
                {
                    return;
                }

                execute(parameter);
            }
        }

        public async Task ExecuteAsync(object parameter)
        {
            try
            {
                isExecuting = true;
                await execute(parameter);
            }
            finally
            {
                isExecuting = false;
            }
        }
    }
}
