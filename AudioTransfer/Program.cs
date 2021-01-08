using System.Threading;
using System.Threading.Tasks;
using AudioTransfer.Configs;
using AudioTransfer.IoC;
using AudioTransfer.Logic;
using CommandLine;
using Ninject;

namespace AudioTransfer {
    class Program {
        private static async Task Main(string[] args) {
            await Parser.Default.ParseArguments<Options>(args)
                .WithParsedAsync(async options => {
                    var kernel = new StandardKernel(new AudioTransferNinject(options));
                    
                    var cts = new CancellationTokenSource();
                    await kernel.Get<DirectoryProcessor>().ProcessDirectory(cts.Token);
                });
        }
    }
}