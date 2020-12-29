using System;
using System.Threading.Tasks;
using AudioTransfer.Configs;
using AudioTransfer.IoC;
using AudioTransfer.Logic;
using AudioTransfer.Utils;
using CommandLine;
using Ninject;

namespace AudioTransfer {
    class Program {
        private static async Task Main(string[] args) {
            await Parser.Default.ParseArguments<Options>(args)
                .WithParsedAsync(async options => {
                    var kernel = new StandardKernel(new AudioTransferNinject(options));
                    var processor = kernel.Get<DirectoryProcessor>();
                    
                    while (true) {
                        ConsoleHelper.Info("Начинаем итерацию");
                        
                        try {
                            await processor.ProcessDirectory();
                        } catch (Exception ex) {
                            ConsoleHelper.Error(ex.ToString());
                        }
                        
                        ConsoleHelper.Info($"Итерация закончена. Следующий запуск через {processor.Config.DelaySeconds} секунд");
                        await Task.Delay(TimeSpan.FromSeconds(processor.Config.DelaySeconds));
                    }
                });
        }
    }
}