using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Windows.Input;

namespace ImageToGCode.Tools
{
    class Command : ICommand
    {
        private SynchronizationContext _synchronizationContext = SynchronizationContext.Current;

        readonly Action<object> _executeAction;
        readonly Predicate<object> _canExecute;

        /// <summary>
        /// Инициализация команды
        /// </summary>
        /// <param name="executeAction">Выполняемое действие</param>
        /// <param name="canExecute">Функция, вычисляющая возможность вычисления</param>
        public Command(Action<object> executeAction, Predicate<object> canExecute)
        {
            if (executeAction == null) throw new ArgumentNullException("executeAction", "executeAction is null.");
            if (canExecute == null) throw new ArgumentNullException("canExecute", "canExecute is null.");

            _executeAction = executeAction;
            _canExecute = canExecute;
        }
        public virtual bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (!CanExecute(parameter))
                throw new Exception("Команда недоступна");
            _executeAction(parameter);
        }

        /// <summary>
        /// Запуск извещения об изменении возможности исполнения
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            if (SynchronizationContext.Current != _synchronizationContext)
                RaiseCanExecuteChangedSynced(null);
            else
                _synchronizationContext.Post(RaiseCanExecuteChangedSynced, null);
        }
        private void RaiseCanExecuteChangedSynced(object param)
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }

    }

}
