
using Timer.Views;
using System;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Timer.Services;

namespace Timer.ViewModels
{
    public partial class TimerViewModel : ObservableObject
    {
        private TimeSpan _totalDuration;
        private double _progressPercentage;

        private readonly TimerService _timerService;
        private readonly PowerService _powerService;

        private int _hours;
        private int _minutes;
        private int _selectedPreset;
        private bool _isUpdatingPreset;

        private string _selectedAction = "Выключение";
        private string _actionToExecute = "Выключение";

        private string _remainingText = "00:00:00";
        private string _status = "Таймер готов к работе";

        private bool _warningShown;
        private bool _notificationShown;

        private int _notificationLeadTimeSeconds = 60;

        public bool IsTimerActive =>_timerService.IsRunning || _timerService.IsPaused;

        public int NotificationLeadTimeSeconds
        {
            get => _notificationLeadTimeSeconds;
            set => SetProperty(ref _notificationLeadTimeSeconds, value);
        }

        public event Action<string, string>? NotificationRequested;

        public int SelectedPreset
        {
            get => _selectedPreset;
            set
            {
                if (!SetProperty(ref _selectedPreset, value))
                    return;

                _isUpdatingPreset = true;

                try
                {
                    Hours = value / 3600;
                    Minutes = (value % 3600) / 60;
                }
                finally
                {
                    _isUpdatingPreset = false;
                }
            }
        }

        public int Hours
        {
            get => _hours;
            set
            {
                if (SetProperty(ref _hours, Math.Max(0, value)))
                {
                    if (!_isUpdatingPreset)
                        SetProperty(ref _selectedPreset, 0, nameof(SelectedPreset));

                    UpdateTimePreview();
                }
            }
        }

        public int Minutes
        {
            get => _minutes;
            set
            {
                if (SetProperty(ref _minutes, Math.Max(0, value)))
                {
                    if (!_isUpdatingPreset)
                        SetProperty(ref _selectedPreset, 0, nameof(SelectedPreset));

                    UpdateTimePreview();
                }
            }
        }

        public string SelectedAction
        {
            get => _selectedAction;
            set => SetProperty(ref _selectedAction, value);
        }

        public string RemainingText
        {
            get => _remainingText;
            set => SetProperty(ref _remainingText, value);
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public double ProgressPercentage
        {
            get => _progressPercentage;
            set => SetProperty(ref _progressPercentage, value);
        }

        public TimerViewModel()
        {
            _timerService = new TimerService();
            _powerService = new PowerService();

            _timerService.Tick += OnTimerTick;
            _timerService.Completed += OnTimerCompleted;
        }

        // Добавить 10 минут

        [RelayCommand]
        private void AddTenMinutes()
        {
            // Если таймер работает или стоит на паузе,
            // добавляем время к текущему отсчёту.
            if (_timerService.IsRunning || _timerService.IsPaused)
            {
                bool success = _timerService.AdjustRemaining(
                    TimeSpan.FromMinutes(10));

                if (success)
                {
                    _totalDuration = _timerService.TotalDuration;
                    Status = "К оставшемуся времени добавлено 10 минут";
                }
                else
                {
                    Status = "Не удалось добавить время";
                }

                return;
            }

            // Если таймер не запущен — меняем время для будущего запуска.
            long totalMinutes = (long)Hours * 60 + Minutes;

            long maximumMinutes = (long)int.MaxValue * 60 + 59;

            if (totalMinutes > maximumMinutes - 10)
                return;

            totalMinutes += 10;

            Hours = (int)(totalMinutes / 60);
            Minutes = (int)(totalMinutes % 60);
        }


        // Убрать 10 минут

        [RelayCommand]
        private void RemoveTenMinutes()
        {
            // Если таймер работает или стоит на паузе,
            // убираем 10 минут из оставшегося времени.
            if (_timerService.IsRunning || _timerService.IsPaused)
            {
                bool success = _timerService.AdjustRemaining(
                    TimeSpan.FromMinutes(-10));

                if (success)
                {
                    _totalDuration = _timerService.TotalDuration;
                    Status = "Из оставшегося времени убрано 10 минут";
                }
                else
                {
                    Status = "Нельзя убрать 10 минут: останется меньше нуля";
                }

                return;
            }

            // Если таймер не запущен — меняем время для будущего запуска.
            long totalMinutes = (long)Hours * 60 + Minutes;

            totalMinutes = Math.Max(0, totalMinutes - 10);

            Hours = (int)(totalMinutes / 60);
            Minutes = (int)(totalMinutes % 60);
        }


        [RelayCommand]
        private void StartTimer()
        {
            if (Minutes > 59)
            {
                Status = "Минуты должны быть от 0 до 59";

                MessageBox.Show(
                    "Минуты не могут быть больше 59.",
                    "Некорректное время",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            TimeSpan duration;

            try
            {
                duration = TimeSpan.FromHours(Hours)
                         + TimeSpan.FromMinutes(Minutes);
            }
            catch (Exception)
            {
                Status = "Слишком большое значение времени";
                return;
            }

            if (duration <= TimeSpan.Zero)
            {
                Status = "Укажи время больше нуля";
                return;
            }

            if (SelectedAction != "Выключение" &&
                SelectedAction != "Перезагрузка" &&
                SelectedAction != "Спящий режим")
            {
                Status = "Выбери корректное действие";
                return;
            }

            string hoursAndMinutes =
                $"{(int)duration.TotalHours:00}:{duration.Minutes:00}";

            NotificationWindow window = new NotificationWindow(
                "ПОДТВЕРЖДЕНИЕ",
                $"Действие: {_selectedAction}\n" +
                $"Время: {hoursAndMinutes}\n\n" +
                "После окончания отсчёта действие будет выполнено.");

            bool? result = window.ShowDialog();

            if (result != true || !window.Confirmed)
            {
                Status = "Запуск отменён пользователем";
                return;
            }

            try
            {
                _warningShown = false;

                _actionToExecute = SelectedAction;

                _totalDuration = duration;
                ProgressPercentage = 100;

                _timerService.Start(duration);
                _notificationShown = false;

                Status = "Таймер запущен";
            }
            catch (Exception ex)
            {
                Status = $"Ошибка запуска: {ex.Message}";
            }
        }

        [RelayCommand]
        private void PauseTimer()
        {
            _timerService.Pause();

            if (_timerService.IsPaused)
                Status = "Таймер приостановлен";
        }

        [RelayCommand]
        private void ResumeTimer()
        {
            _timerService.Resume();

            if (_timerService.IsRunning)
                Status = "Таймер продолжает работу";
        }


        [RelayCommand]
        private void CancelTimer()
        {
            // Останавливаем таймер и отменяем действие.
            _timerService.Cancel();

            // Сбрасываем дополнительные параметры.
            _warningShown = false;
            _notificationShown = false;

            // Очищаем сохранённую продолжительность.
            _totalDuration = TimeSpan.Zero;

            // Очищаем поля ввода.
            Hours = 0;
            Minutes = 0;

            // Сбрасываем быстрый выбор времени.
            SelectedPreset = 0;

            // Очищаем отображение времени и кольцо.
            RemainingText = "00:00:00";
            ProgressPercentage = 0;

            // Обновляем статус.
            Status = "Таймер сброшен";
        }


        // Обновление кольца при вводе времени до запуска таймера
        private void UpdateTimePreview()
        {
            // Если таймер работает или на паузе,
            // не трогаем отображение обратного отсчёта.
            if (_timerService.IsRunning || _timerService.IsPaused)
                return;

            // Проверяем корректность введённых минут.
            if (Minutes > 59)
            {
                RemainingText = "00:00:00";
                ProgressPercentage = 0;
                return;
            }

            try
            {
                TimeSpan duration =
                    TimeSpan.FromHours(Hours) +
                    TimeSpan.FromMinutes(Minutes);

                if (duration <= TimeSpan.Zero)
                {
                    RemainingText = "00:00:00";
                    ProgressPercentage = 0;
                    return;
                }

                // Показываем введённое время внутри кольца.
                RemainingText =
                    $"{(int)duration.TotalHours:00}:" +
                    $"{duration.Minutes:00}:00";

                // Полностью заполняем кольцо.
                ProgressPercentage = 100;
            }
            catch (OverflowException)
            {
                RemainingText = "00:00:00";
                ProgressPercentage = 0;
            }
        }

        private void OnTimerTick(object? sender, TimeSpan remaining)
        {
            RemainingText =
                $"{(int)remaining.TotalHours:00}:" +
                $"{remaining.Minutes:00}:" +
                $"{remaining.Seconds:00}";

            if (remaining.TotalSeconds <= 10 && !_warningShown)
            {
                _warningShown = true;
                Status = "ВНИМАНИЕ! Скоро выполнится действие!";
            }

            if (remaining.TotalSeconds <= NotificationLeadTimeSeconds &&
                !_notificationShown)
            {
                _notificationShown = true;

                NotificationRequested?.Invoke(
                    "PowerControl — предупреждение",
                    $"Через {remaining:hh\\:mm\\:ss} " +
                    $"выполнится действие: {_actionToExecute}.");
            }

            // Берём актуальную длительность непосредственно из TimerService
            TimeSpan currentDuration = _timerService.TotalDuration;

            if (currentDuration.TotalSeconds > 0)
            {
                ProgressPercentage = Math.Clamp(
                    remaining.TotalSeconds /
                    currentDuration.TotalSeconds * 100,
                    0,
                    100);
            }
        }



        private void OnTimerCompleted(object? sender, EventArgs e)
        {
            try
            {
                switch (_actionToExecute)
                {
                    case "Выключение":
                        Status = "Компьютер выключается...";
                        _powerService.ScheduleShutdown(0);
                        break;

                    case "Перезагрузка":
                        Status = "Компьютер перезагружается...";
                        _powerService.ScheduleRestart(0);
                        break;

                    case "Спящий режим":
                        Status = "Компьютер переходит в спящий режим...";
                        _powerService.SleepComputer();
                        break;
                }
            }
            catch (Exception ex)
            {
                Status = $"Ошибка выполнения действия: {ex.Message}";

                MessageBox.Show(
                    ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

    }
}
