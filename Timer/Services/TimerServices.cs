
using System;
using System.Diagnostics;
using System.Windows.Threading;

namespace Timer.Services
{
    public class TimerService
    {
        private readonly DispatcherTimer _timer;
        private readonly Stopwatch _stopwatch;

        private TimeSpan _duration;

        public TimeSpan Remaining { get; private set; }

        public TimeSpan TotalDuration => _duration;

        public bool IsRunning => _timer.IsEnabled;

        public bool IsPaused { get; private set; }

        public event EventHandler<TimeSpan>? Tick;

        public event EventHandler? Completed;

        public TimerService()
        {
            _stopwatch = new Stopwatch();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(250)
            };

            _timer.Tick += OnTimerTick;
        }

        // Запуск таймера
        public void Start(TimeSpan duration)
        {
            if (duration <= TimeSpan.Zero)
                throw new ArgumentException("Время должно быть больше нуля.");

            Cancel();

            _duration = duration;
            Remaining = duration;

            IsPaused = false;

            _stopwatch.Restart();
            _timer.Start();

            Tick?.Invoke(this, Remaining);
        }

        // Изменение оставшегося времени без перезапуска
        public bool AdjustRemaining(TimeSpan adjustment)
        {
            if (!IsRunning && !IsPaused)
                return false;

            TimeSpan currentRemaining = _duration - _stopwatch.Elapsed;
            TimeSpan newRemaining;

            try
            {
                newRemaining = currentRemaining + adjustment;

                if (newRemaining <= TimeSpan.Zero)
                    return false;

                _duration = _stopwatch.Elapsed + newRemaining;
            }
            catch (OverflowException)
            {
                return false;
            }

            Remaining = newRemaining;

            Tick?.Invoke(this, Remaining);

            return true;
        }

        // Пауза
        public void Pause()
        {
            if (!IsRunning)
                return;

            _timer.Stop();
            _stopwatch.Stop();

            IsPaused = true;
        }

        // Продолжение
        public void Resume()
        {
            if (!IsPaused)
                return;

            IsPaused = false;

            _stopwatch.Start();
            _timer.Start();
        }

        // Отмена
        public void Cancel()
        {
            _timer.Stop();
            _stopwatch.Reset();

            Remaining = TimeSpan.Zero;
            _duration = TimeSpan.Zero;
            IsPaused = false;
        }

        // Обновление времени
        private void OnTimerTick(object? sender, EventArgs e)
        {
            Remaining = _duration - _stopwatch.Elapsed;

            if (Remaining <= TimeSpan.Zero)
            {
                Remaining = TimeSpan.Zero;

                _timer.Stop();
                _stopwatch.Stop();

                IsPaused = false;

                Tick?.Invoke(this, Remaining);
                Completed?.Invoke(this, EventArgs.Empty);

                return;
            }

            Tick?.Invoke(this, Remaining);
        }
    }
}
