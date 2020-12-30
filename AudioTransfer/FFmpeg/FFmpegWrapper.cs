using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioTransfer.Configs;
using AudioTransfer.Extensions;
using AudioTransfer.Types;
using AudioTransfer.Utils;
using Newtonsoft.Json;

namespace AudioTransfer.FFmpeg {
    /// <summary>
    /// Обертка на ffmpeg
    /// </summary>
    public class FFmpegWrapper {
        private readonly IFFmpegConfig _config;

        public FFmpegWrapper(IFFmpegConfig config) {
            _config = config;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Convert(string inputFile, string outputFile) {
            return await Convert(new List<string>{inputFile}, outputFile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFiles"></param>
        /// <param name="outputFile"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Convert(IReadOnlyCollection<string> inputFiles, string outputFile) {
            try {
                if (inputFiles.Count == 0) {
                    return false;
                }

                var arguments = "-y -loglevel error"
                    .AppendThroughWhitespace(string.Join(" ", inputFiles.Select(f => $"-i {f.CoverQuotes()}")))
                    .AppendThroughWhitespace("-filter_complex")
                    .AppendThroughWhitespace(string.Join(string.Empty, inputFiles.Select((_, i) => $"[{i}:a]")) + $"concat=n={inputFiles.Count}:v=0:a=1")
                    .AppendThroughWhitespace("-vn")
                    .AppendThroughWhitespace($"-codec:a {_config.Codec}")
                    .AppendThroughWhitespace($"-q:a {_config.Quality}")
                    .AppendThroughWhitespace(outputFile.CoverQuotes());

                var info = new ProcessStartInfo {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    FileName = _config.FFmpegPath,
                    Arguments = arguments
                };
                
                var tcs = new TaskCompletionSource<int>();

                var process = new Process {
                    StartInfo = info,
                    EnableRaisingEvents = true
                };
                
                process.Exited += (sender, args) => {
                    tcs.SetResult(process.ExitCode);
                    process.Dispose();
                };
                
                process.Start();

                return await tcs.Task == 0;
            } catch (Exception ex) {
                ConsoleHelper.Error($"Не удалось обработать файл {outputFile} {ex}");
                return false;
            }
        }

        /// <summary>
        /// Получение информации об аудиофайле
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<AudioFileInfo> GetFileFormat(string file) {
            var result = new AudioFileFormat();
            
            try {
                var arguments = $"-i {file.CoverQuotes()} -print_format json -show_format -v quiet";

                var info = new ProcessStartInfo {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    FileName = _config.FFprobePath,
                    Arguments = arguments,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    StandardErrorEncoding = Encoding.UTF8,
                    StandardOutputEncoding = Encoding.UTF8,
                };

                using var process = new Process {
                    StartInfo = info,
                    EnableRaisingEvents = true
                };
                process.Start();
                
                var sb = new StringBuilder();

                while (!process.StandardOutput.EndOfStream) {
                    sb.Append(await process.StandardOutput.ReadLineAsync());
                }

                result = JsonConvert.DeserializeObject<AudioFileFormat>(sb.ToString());
                process.WaitForExit();
                return result.Format;
            } catch (Exception ex) {
                ConsoleHelper.Error($"Не удалось обработать файл {ex}");
                return result.Format;
            }
        }
    }
}