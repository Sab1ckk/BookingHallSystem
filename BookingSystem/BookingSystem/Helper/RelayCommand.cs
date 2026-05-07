using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookingSystem.Help
{
    /// <summary>
    /// Реализация ICommand для использования в MVVM паттерне.
    /// Позволяет привязывать методы к командам в XAML.
    /// </summary>
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// Метод, выполняемый при вызове команды
        /// </summary>
        private readonly Action<object> _execute;

        /// <summary>
        /// Метод, определяющий возможность выполнения команды
        /// </summary>
        private readonly Predicate<object> _canExecute;

        /// <summary>
        /// Конструктор RelayCommand.
        /// </summary>
        /// <param name="execute">Метод, выполняемый при вызове команды</param>
        /// <param name="canExecute">Метод проверки возможности выполнения (опционально)</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Определяет, может ли команда быть выполнена в текущий момент.
        /// </summary>
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        /// <summary>
        /// Выполняет команду.
        /// </summary>
        public void Execute(object parameter) => _execute(parameter);

        /// <summary>
        /// Событие изменения состояния доступности команды.
        /// Подписывается на CommandManager.RequerySuggested для автоматического обновления UI.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
