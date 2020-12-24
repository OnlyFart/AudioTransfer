using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AudioTransfer.Configs;
using AudioTransfer.FFMPEG;
using AudioTransfer.Types;
using AudioTransfer.Utils;

namespace AudioTransfer.Logic {
    public class Processor {
        private const string JOIN_FILENAME = "join.txt";
        private const string CONVERTED_FILENAME = "conv.txt";
        private const string REPORT_FILENAME = "report.txt";

        private const string OUTPUT_EXTENSION = "mp3";

        public readonly IProcessorConfig Config;
        private readonly FFmpegWrapper _fFmpegWrapper;

        public Processor(IProcessorConfig config, FFmpegWrapper fFmpegWrapper) {
            Config = config;
            _fFmpegWrapper = fFmpegWrapper;
        }

        public async Task ProcessDirectory() {
            var slim = new SemaphoreSlim(Config.ThreadsCount, Config.ThreadsCount);
            
            var directories = Directory.GetDirectories(Config.Source, "*", SearchOption.AllDirectories).Select(d => new FileInfo(d)).ToList();

            await Task.WhenAll(directories.Select(async directory => {
                await slim.WaitAsync();

                try {
                    if (!NeedProcessDirectory(directory, out var filesToProcess, out var mode)) {
                        return;
                    }

                    await Process(directory, mode, filesToProcess);
                } catch (Exception ex) {
                    ConsoleHelper.Error(ex.ToString());
                } finally {
                    slim.Release();
                }
            }));
        }

        private async Task Process(FileSystemInfo directory, ProcessorMode mode, List<string> inputFiles) {
            if (mode == ProcessorMode.Join) {
                var outputFileName = GetOutputFileName(directory);
                if (await _fFmpegWrapper.Convert(inputFiles, outputFileName)) {
                    ConsoleHelper.Success($"Успешно обработали каталог {directory}");
                    await CreateJoinReport(directory, inputFiles, outputFileName);
                } else {
                    ConsoleHelper.Error($"Не удалось обработать файлы в директории {directory}");
                }
            } else if (mode == ProcessorMode.Convert) {
                var files = new List<Tuple<string, string>>();
                
                foreach (var inputFile in inputFiles) {
                    var outputFile = GetOutputFileName(inputFile, directory);
                    if (await _fFmpegWrapper.Convert(inputFile, outputFile)) {
                        files.Add(Tuple.Create(inputFile, outputFile));
                        ConsoleHelper.Success($"Успешно обработали файл {inputFile}");
                    } else {
                        ConsoleHelper.Error($"Не удалось обработать файл {inputFile}");
                    }
                }

                await CreateConvertReport(directory, files);
            }
        }

        private  async Task CreateJoinReport(FileSystemInfo directory, IEnumerable<string> inputFiles, string outputFile) {
            var lines = new List<string> {"Слияние:"};
            foreach (var file in inputFiles) {
                var inputInfo = await GetFileInfo(file);
                lines.Add($"- {inputInfo}");
            }
            
            lines.Add("Результат:");
            var outputInfo = await GetFileInfo(outputFile);
            lines.Add($"- {outputInfo}");
            
            await File.WriteAllLinesAsync(Path.Combine(directory.FullName, REPORT_FILENAME), lines, Encoding.UTF8);
        }

        private async Task CreateConvertReport(FileSystemInfo directory, IEnumerable<Tuple<string, string>> inputFiles) {
            var lines = new List<string>();
            
            foreach (var (inputFile, outputFile) in inputFiles) {
                var inputInfo = await GetFileInfo(inputFile);
                var outputInfo = await GetFileInfo(outputFile);

                lines.Add($"- Конвертирование {inputInfo}");
                lines.Add($"- Результат {outputInfo}");
            }

            await File.WriteAllLinesAsync(Path.Combine(directory.FullName, REPORT_FILENAME), lines, Encoding.UTF8);
        }

        private async Task<AudioFileInfo> GetFileInfo(string path) {
            return (await _fFmpegWrapper.GetFileFormat(path)).Format;
        }

        /// <summary>
        /// Получение результируюшего пути для сохранения файлов с созданием дерева каталогов
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        private string GetOutputFileName(FileSystemInfo directory) {
            var outputDirectory = GetOutputDirectory(directory);
            var outputFile = $"{directory.Name}_{OUTPUT_EXTENSION.ToUpper()}WRAP.{OUTPUT_EXTENSION.ToLower()}";

            return Path.Combine(outputDirectory, outputFile);
        }

        /// <summary>
        /// Получение результируюшего пути для сохранения файлов с созданием дерева каталогов
        /// </summary>
        /// <param name="file"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        private string GetOutputFileName(string file, FileSystemInfo directory) {
            var outputDirectory = GetOutputDirectory(directory);
            var outputFile = $"{Path.GetFileNameWithoutExtension(file)}.{OUTPUT_EXTENSION.ToLower()}";

            return Path.Combine(outputDirectory, outputFile);
        }
        
        /// <summary>
        /// Создание результирующей папки
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        private string GetOutputDirectory(FileSystemInfo directory) {
            var outputDirectory = Path.Combine(Config.Destination, Path.GetRelativePath(Config.Source, directory.FullName));
            if (!Directory.Exists(outputDirectory)) {
                Directory.CreateDirectory(outputDirectory);
            }

            return outputDirectory;
        }

        /// <summary>
        /// Проверка на необходимость проверки директории
        /// </summary>
        /// <param name="inputFiles">Файлы из директории</param>
        /// <param name="directory">Директория</param>
        /// <param name="mode">Режим работы процессора</param>
        /// <returns></returns>
        private static bool NeedProcessDirectory(FileSystemInfo directory, out List<string> inputFiles, out ProcessorMode mode) {
            ConsoleHelper.Info($"Начинаем обработку директории {directory.FullName}");
            
            inputFiles = default;
            mode = default;

            var directoryFiles = Directory.GetFiles(directory.FullName).Select(f => new FileInfo(f)).ToList();

            if (directoryFiles.Count == 0) {
                ConsoleHelper.Warn($"Директория {directory.FullName} не содержит файлов. Пропускаем");
                return false;
            }
            
            if (directoryFiles.Any(f => string.Equals(f.Name, REPORT_FILENAME, StringComparison.InvariantCultureIgnoreCase))) {
                ConsoleHelper.Warn($"Директория {directory.FullName} содержит файл {REPORT_FILENAME}. Пропускаем");
                return false;
            }
            
            var isConvert = directoryFiles.Any(f => string.Equals(f.Name, CONVERTED_FILENAME, StringComparison.InvariantCultureIgnoreCase));
            var isJoin = directoryFiles.Any(f => string.Equals(f.Name, JOIN_FILENAME, StringComparison.InvariantCultureIgnoreCase));

            if (!isConvert && !isJoin) {
                ConsoleHelper.Warn($"Отсутствуют файлы {CONVERTED_FILENAME} и {JOIN_FILENAME}. Пропускаем");
                return false;
            }

            mode = isJoin ? ProcessorMode.Join : ProcessorMode.Convert;
            ConsoleHelper.Info($"Обрабатываем директорию {directory.FullName} в режиме {mode}");

            inputFiles = directoryFiles
                .Where(f => f.Extension == "." + OUTPUT_EXTENSION)
                .OrderBy(f => f.Name)
                .Select(t => t.FullName)
                .ToList();

            return true;
        }
    }
}
