using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timer.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace Timer.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly PowerService _powerService;

        [ObservableProperty]
        private string status = "PowerControl готов к работе";

        public MainViewModel()
        {
            _powerService = new PowerService();
            TimerViewModel = new TimerViewModel();
        }

        [RelayCommand]
        private void ScheduleShutdown()
        {
            try
            {
                _powerService.ScheduleShutdown(60);

                Status = "Выключение запланировано через 60 секунд";
            }
            catch (Exception ex)
            {
                Status = $"Ошибка: {ex.Message}";
            }
        }

        [RelayCommand]
        private void ScheduleRestart()
        {
            try
            {
                _powerService.ScheduleRestart(60);

                Status = "Перезагрузка запланирована через 60 секунд";
            }
            catch (Exception ex)
            {
                Status = $"Ошибка: {ex.Message}";
            }
        }

        [RelayCommand]
        private void CancelAction()
        {
            try
            {
                _powerService.CancelScheduledAction();

                Status = "Запланированное действие отменено";
            }
            catch (Exception ex)
            {
                Status = $"Ошибка: {ex.Message}";
            }
        }
        public TimerViewModel TimerViewModel { get; }

    }
}
