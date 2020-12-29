using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AudioTransfer.Configs;
using AudioTransfer.FFmpeg;
using AudioTransfer.Types;
using AudioTransfer.Utils;

namespace AudioTransfer.Logic.FileProcessor {
    public class ConvertFileProcessor : FileProcessorBase {
        public ConvertFileProcessor(IProcessorConfig config, FFmpegWrapper fFmpegWrapper) : base(config, fFmpegWrapper) { }
        public override async Task Process(FileSystemInfo directory, List<string> inputFiles) {
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
        
        public override string FileName => "conv.txt";

        /// <summary>
        /// Созлание отчета по конвертации файлов
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="inputFiles"></param>
        /// <returns></returns>
        private async Task CreateConvertReport(FileSystemInfo directory, IEnumerable<Tuple<string, string>> inputFiles) {
            var lines = new List<string>();
            
            foreach (var (inputFile, outputFile) in inputFiles) {
                lines.Add($"- Конвертирование {await _fFmpegWrapper.GetFileFormat(inputFile)}");
                lines.Add($"- Результат {await _fFmpegWrapper.GetFileFormat(outputFile)}");
            }

            await File.WriteAllLinesAsync(Path.Combine(directory.FullName, Const.REPORT_FILENAME), lines, Encoding.UTF8);
        }
    }
}
