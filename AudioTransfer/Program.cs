using System.Threading.Tasks;
using AudioTransfer.Configs;
using AudioTransfer.IoC;
using AudioTransfer.Logic;
using AutoMapper;
using CommandLine;
using Ninject;

namespace AudioTransfer {
    class Program {
        private static readonly StandardKernel _kernel = new StandardKernel(new AudioTransferNinject());

        private static void BindConfig<T>(IMapperBase mapper, Options options) {
            _kernel.Bind<T>().ToConstant(mapper.Map<T>(options));
        }

        private static async Task Main(string[] args) {
            await Parser.Default.ParseArguments<Options>(args)
                .WithParsedAsync(async options => {
                    var mapper = _kernel.Get<IMapper>();

                    BindConfig<FFmpegConfig>(mapper, options);
                    BindConfig<ProcessorConfig>(mapper, options);
                    BindConfig<FtpConfig>(mapper, options);

                    await _kernel.Get<Processor>().ProcessDirectory(options.Source, options.Destination);
                });
        }
    }
}