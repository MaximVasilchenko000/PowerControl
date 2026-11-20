using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timer.Services
{
    public class PowerService
    {
        // Запланировать выключение компьютера
        public void ScheduleShutdown(int seconds)
        {
            ExecuteShutdownCommand($"/s /t {seconds}");
        }

        // Запланировать перезагрузку
        public void ScheduleRestart(int seconds)
        {
            ExecuteShutdownCommand($"/r /t {seconds}");
        }

        // Отменить запланированное действие
        public void CancelScheduledAction()
        {
            ExecuteShutdownCommand("/a");
        }

        // Перевести компьютер в спящий режим
        public void SleepComputer()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "rundll32.exe",
                Arguments = "powrprof.dll,SetSuspendState 0,1,0",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process? process = Process.Start(startInfo);

            if (process == null)
            {
                throw new InvalidOperationException(
                    "Не удалось перевести компьютер в спящий режим."
                );
            }
        }

        // Запуск системной команды Windows
        private void ExecuteShutdownCommand(string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "shutdown.exe",
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process? process = Process.Start(startInfo);

            if (process == null)
            {
                throw new InvalidOperationException(
                    "Не удалось запустить системную команду Windows."
                );
            }

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException(
                    $"Команда завершилась с ошибкой. Код: {process.ExitCode}"
                );
            }
        }
    }
}
