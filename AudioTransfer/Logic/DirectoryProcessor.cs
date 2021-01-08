using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AudioTransfer.Configs;
using AudioTransfer.Extensions;
using AudioTransfer.Logic.FileProcessor;
using AudioTransfer.Types;
using AudioTransfer.Utils;

namespace AudioTransfer.Logic {
    /// <summary>
    /// Обработчик директорий
    /// </summary>
    public class DirectoryProcessor {
        private readonly IEnumerable<FileProcessorBase> _processors;

        private readonly IProcessorConfig _config;

        public DirectoryProcessor(IEnumerable<FileProcessorBase> processors, IProcessorConfig config) {
            _config = config;
            _processors = processors;
        }
        
        /// <summary>
        /// Рекурсивная обработка исходной директории
        /// </summary>
        /// <returns></returns>
        public async Task ProcessDirectory(CancellationToken token) {
            while (!token.IsCancellationRequested) {
                ConsoleHelper.Info("Начинаем итерацию");
                var slim = new SemaphoreSlim(_config.ThreadsCount, _config.ThreadsCount);
            
                var directories = Directory.GetDirectories(_config.Source, "*", SearchOption.AllDirectories).Select(d => new FileInfo(d));

                await Task.WhenAll(directories.Select(async directory => {
                    await slim.WaitAsync(token);

                    try {
                        if (NeedProcessDirectory(directory, out var filesToProcess, out var processor)) {
                            await processor.Process(directory, filesToProcess);
                        }
                    } catch (Exception ex) {
                        ConsoleHelper.Error(ex.ToString());
                    } finally {
                        slim.Release();
                    }
                }));
                
                ConsoleHelper.Info($"Итерация закончена. Следующий запуск через {_config.DelaySeconds} секунд");
                await Task.Delay(TimeSpan.FromSeconds(_config.DelaySeconds), token);    
            }
        }

        /// <summary>
        /// Проверка на необходимость проверки директории
        /// </summary>
        /// <param name="inputFiles">Файлы из директории</param>
        /// <param name="directory">Директория</param>
        /// <param name="fileProcessor">Процессор</param>
        /// <returns></returns>
        private bool NeedProcessDirectory(FileSystemInfo directory, out IReadOnlyCollection<string> inputFiles, out FileProcessorBase fileProcessor) {
            ConsoleHelper.Info($"Начинаем обработку директории {directory.FullName}");
            
            inputFiles = default;
            fileProcessor = default;

            var directoryFiles = Directory.GetFiles(directory.FullName).Select(f => new FileInfo(f)).ToList();

            if (directoryFiles.Count == 0) {
                ConsoleHelper.Warn($"Директория {directory.FullName} не содержит файлов. Пропускаем");
                return false;
            }
            
            if (directoryFiles.Any(f => f.FileNameEquals(Const.REPORT_FILENAME))) {
                ConsoleHelper.Warn($"Директория {directory.FullName} содержит файл {Const.REPORT_FILENAME}. Пропускаем");
                return false;
            }

            if (!TryGetProcessor(directoryFiles, out fileProcessor)) {
                ConsoleHelper.Warn($"Директория {directory.FullName} не содержит файлов {string.Join(", ", _processors.Select(t => t.FileName))}. Пропускаем");
                return false;
            }

            ConsoleHelper.Info($"Обрабатываем директорию {directory.FullName} в режиме {fileProcessor.FileName}");

            inputFiles = directoryFiles
                .Where(f => _config.SupportExtensions.Any(f.ExtensionEquals))
                .OrderBy(f => f.Name)
                .Select(t => t.FullName)
                .ToList();

            return true;
        }

        /// <summary>
        /// Получение режма работы процессора
        /// </summary>
        /// <param name="directoryFiles"></param>
        /// <param name="fileProcessor"></param>
        /// <returns></returns>
        private bool TryGetProcessor(IEnumerable<FileInfo> directoryFiles, out FileProcessorBase fileProcessor) {
            fileProcessor = _processors.FirstOrDefault(p => directoryFiles.Any(f => f.FileNameEquals(p.FileName)));
            return fileProcessor != null;
        }
    }
}
