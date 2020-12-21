using System;
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
        private const string PROCESSED_FILENAME = "join.txt";
        
        private readonly IProcessorConfig _config;
        private readonly FtpSender _ftpSender;
        private readonly FFmpegWrapper _fFmpegWrapper;

        public Processor(IProcessorConfig config, FtpSender ftpSender, FFmpegWrapper fFmpegWrapper) {
            _config = config;
            _ftpSender = ftpSender;
            _fFmpegWrapper = fFmpegWrapper;
        }

        public async Task ProcessDirectory(string source, string destination) {
            var slim = new SemaphoreSlim(_config.ThreadsCount, _config.ThreadsCount);
            
            var directories = Directory.GetDirectories(source).Select(d => new FileInfo(d));

            await Task.WhenAll(directories.Select(async directory => {
                await slim.WaitAsync();

                try {
                    if (!NeedProcessDirectory(directory, out var filesToProcess)) {
                        return;
                    }

                    using var tempFolder = TempFolderFactory.Create();
                    var extension = _fFmpegWrapper.Config.OutputExtension;
                    var outputFile = Path.Combine(tempFolder.Path, $"{directory.Name}_{extension.ToUpper()}WRAP.{extension.ToLower()}");
                
                    if (await _fFmpegWrapper.Convert(filesToProcess, outputFile)) {
                        var ftpDestination = Path.Combine(destination, Path.GetFileName(outputFile));
                        if (await _ftpSender.Send(ftpDestination, outputFile)) {
                            ConsoleHelper.Success($"{directory.Name} сохранен в {new Uri(new Uri("ftp://" + _ftpSender.Config.FtpServer), ftpDestination)}");
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

            if (inputFiles.All(f => new FileInfo(f).Name != PROCESSED_FILENAME)) {
                ConsoleHelper.Warn($"Директория {directory.FullName} не содержит файл {PROCESSED_FILENAME}. Пропускаем");
                return false;
            }

            inputFiles = inputFiles.Where(f => new FileInfo(f).Name != PROCESSED_FILENAME).ToList();

            var badNameFile = inputFiles.FirstOrDefault(f => !int.TryParse(Path.GetFileNameWithoutExtension(f), out _));
            if (badNameFile != null) {
                ConsoleHelper.Warn($"Директория {directory.FullName} содержит файл {badNameFile}, который невозможно отсортировать. Пропускаем");
                return false;
            }

            inputFiles = inputFiles.OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f))).ToList();

            return true;
        }
    }
}
