using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AudioTransfer.Configs;
using AudioTransfer.FFMPEG;
using AudioTransfer.FTP;
using AudioTransfer.Utils;
using TempFolder;

namespace AudioTransfer.Logic {
    public class Processor {
        private const string PROCESSED_FILENAME = "processed.txt";
        
        private readonly ProcessorConfig _config;
        private readonly FtpSender _ftpSender;
        private readonly FFmpegWrapper _fFmpegWrapper;

        public Processor(ProcessorConfig config, FtpSender ftpSender, FFmpegWrapper fFmpegWrapper) {
            _config = config;
            _ftpSender = ftpSender;
            _fFmpegWrapper = fFmpegWrapper;
        }

        public async Task ProcessDirectory(string source, string destination) {
            var slim = new SemaphoreSlim(_config.ThreadsCount, _config.ThreadsCount);
            
            var directories = Directory.GetDirectories(source).Select(d => new FileInfo(d));

            await Task.WhenAll(directories.AsParallel().Select(async directory => {
                await slim.WaitAsync();

                try {
                    if (!NeedProcessDirectory(directory, out var filesToProcess)) {
                        return;
                    }

                    using var tempFolder = TempFolderFactory.Create();
                    var outputFile = Path.Combine(tempFolder.Path, directory.Name + "." + _fFmpegWrapper.Config.OutputExtension.ToLower());
                
                    if (await _fFmpegWrapper.Convert(filesToProcess, outputFile)) {
                        var ftpDestination = Path.Combine(destination, directory.Name, Path.GetFileName(outputFile));
                        if (await _ftpSender.Send(ftpDestination, outputFile)) {
                            ConsoleHelper.Success($"{directory.Name} сохранен в ftp://{Path.Combine(_ftpSender.Config.FtpServer, ftpDestination)}");
                            CompleteProcess(directory);
                        }
                    } else {
                        ConsoleHelper.Error($"Не удалось обработать файлы в директории {directory}");
                    }
                } finally {
                    slim.Release();
                }
            }));
        }

        /// <summary>
        /// Создание файлы, что бы понимать, что директория уже обработана и повторная обработка не требуется
        /// </summary>
        /// <param name="directory"></param>
        private static void CompleteProcess(FileSystemInfo directory) {
            var processed = Path.Combine(directory.FullName, PROCESSED_FILENAME);
            if (!File.Exists(processed)) {
                File.Create(processed);
            }
        }
        
        /// <summary>
        /// Проверка на необходимость проверки директории
        /// </summary>
        /// <param name="inputFiles">Файлы из директории</param>
        /// <param name="directory">Директория</param>
        /// <returns></returns>
        private static bool NeedProcessDirectory(FileSystemInfo directory, out List<string> inputFiles) {
            ConsoleHelper.Info($"Начинаем обработку директории {directory.Name}");
            
            inputFiles = Directory.GetFiles(directory.FullName).ToList();
            
            if (inputFiles.Count == 0) {
                ConsoleHelper.Warn($"Директория {directory.FullName} не содержит файлов. Пропускаем");
                return false;
            }

            if (inputFiles.Any(f => new FileInfo(f).Name == PROCESSED_FILENAME)) {
                ConsoleHelper.Warn($"Директория {directory.FullName} содержит файл {PROCESSED_FILENAME}. Пропускаем");
                return false;
            }

            return true;
        }
    }
}
