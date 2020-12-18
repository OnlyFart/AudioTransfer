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
                    await kernel.Get<Processor>().ProcessDirectory(options.Source, options.Destination);
                });
        }
    }
}