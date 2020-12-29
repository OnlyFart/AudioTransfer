using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AudioTransfer.Configs;
using AudioTransfer.Logic.FileProcessor;
using AudioTransfer.Types;
using AudioTransfer.Utils;

namespace AudioTransfer.Logic {
    public class DirectoryProcessor {
        private readonly Dictionary<string, FileProcessorBase> _fileToProcessorMap;

        public readonly IProcessorConfig Config;

        public DirectoryProcessor(IEnumerable<FileProcessorBase> processors, IProcessorConfig config) {
            Config = config;
            _fileToProcessorMap = new Dictionary<string, FileProcessorBase>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var processor in processors) {
                _fileToProcessorMap.Add(processor.FileName, processor);
            }
        }
        
        /// <summary>
        /// Рекурсивная обработка исходной директории
        /// </summary>
        /// <returns></returns>
        public async Task ProcessDirectory() {
            var slim = new SemaphoreSlim(Config.ThreadsCount, Config.ThreadsCount);
            
            var directories = Directory.GetDirectories(Config.Source, "*", SearchOption.AllDirectories).Select(d => new FileInfo(d)).ToList();

            await Task.WhenAll(directories.Select(async directory => {
                await slim.WaitAsync();

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
        }

        /// <summary>
        /// Проверка на необходимость проверки директории
        /// </summary>
        /// <param name="inputFiles">Файлы из директории</param>
        /// <param name="directory">Директория</param>
        /// <param name="fileProcessor">Процессор</param>
        /// <returns></returns>
        private bool NeedProcessDirectory(FileSystemInfo directory, out List<string> inputFiles, out FileProcessorBase fileProcessor) {
            ConsoleHelper.Info($"Начинаем обработку директории {directory.FullName}");
            
            inputFiles = default;
            fileProcessor = default;

            var directoryFiles = Directory.GetFiles(directory.FullName).Select(f => new FileInfo(f)).ToList();

            if (directoryFiles.Count == 0) {
                ConsoleHelper.Warn($"Директория {directory.FullName} не содержит файлов. Пропускаем");
                return false;
            }
            
            if (directoryFiles.Any(f => string.Equals(f.Name, Const.REPORT_FILENAME, StringComparison.InvariantCultureIgnoreCase))) {
                ConsoleHelper.Warn($"Директория {directory.FullName} содержит файл {Const.REPORT_FILENAME}. Пропускаем");
                return false;
            }

            if (!TryGetProcessor(directoryFiles, out fileProcessor)) {
                ConsoleHelper.Warn($"Отсутствуют файлы {string.Join(", ", _fileToProcessorMap.Select(t => t.Key))}. Пропускаем");
                return false;
            }

            ConsoleHelper.Info($"Обрабатываем директорию {directory.FullName} в режиме {fileProcessor.FileName}");

            inputFiles = directoryFiles
                .Where(f => f.Extension == "." + Const.OUTPUT_EXTENSION)
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
            fileProcessor = default;
            
            foreach (var file in directoryFiles) {
                if (_fileToProcessorMap.TryGetValue(file.Name, out fileProcessor)) {
                    return true;
                }
            }

            return false;
        }
    }
}
