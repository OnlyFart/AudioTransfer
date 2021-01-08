using System.Collections.Generic;

namespace AudioTransfer.Configs {
    public interface IProcessorConfig {
        /// <summary>
        /// Количество потоков для обработки файлов
        /// </summary>
        int ThreadsCount { get; set; }
        
        /// <summary>
        /// Исходная директория с файлами для конвертации
        /// </summary>
        string Source { get; set; }
        
        /// <summary>
        /// Результирующая директория для сохранения файлов
        /// </summary>
        string Destination { get; set; }
        
        /// <summary>
        /// Задержка в секундах между итерациями
        /// </summary>
        int DelaySeconds { get; set; }
        
        /// <summary>
        /// Поддерживаемые расширения для обработки
        /// </summary>
        IList<string> SupportExtensions { get; set; }
    }
}
