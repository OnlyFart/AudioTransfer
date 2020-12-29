using AudioTransfer.Configs;
using AudioTransfer.Logic.FileProcessor;
using Ninject.Modules;

namespace AudioTransfer.IoC {
    public class AudioTransferNinject : NinjectModule {
        private readonly Options _options;

        public AudioTransferNinject(Options options) {
            _options = options;
        }
        
        public override void Load() {
            Bind<IFFmpegConfig>().ToConstant((IFFmpegConfig)_options);
            Bind<IProcessorConfig>().ToConstant((IProcessorConfig)_options);
            Bind<FileProcessorBase>().To<ConvertFileProcessor>().InSingletonScope();
            Bind<FileProcessorBase>().To<JoinFileProcessor>().InSingletonScope();
        }
    }
}
