using AudioTransfer.Configs;
using AutoMapper;
using Ninject.Modules;

namespace AudioTransfer.IoC {
    public class AudioTransferNinject : NinjectModule {
        private readonly Options _options;

        public AudioTransferNinject(Options options) {
            _options = options;
        }
        
        public override void Load() {
            var mapper = CreateMapper();
            Bind<IMapper>().ToConstant(mapper);
            BindConfig<FFmpegConfig>(mapper, _options);
            BindConfig<ProcessorConfig>(mapper, _options);
            BindConfig<FtpConfig>(mapper, _options);
        }
        
        private static IMapper CreateMapper() {
            var mapperConfig = new MapperConfiguration(cfg => {
                cfg.CreateMap<Options, FFmpegConfig>();
                cfg.CreateMap<Options, ProcessorConfig>();
                cfg.CreateMap<Options, FtpConfig>();
            });
            
            mapperConfig.AssertConfigurationIsValid();
            return mapperConfig.CreateMapper();
        }
        
        private void BindConfig<T>(IMapperBase mapper, Options options) {
            Bind<T>().ToConstant(mapper.Map<T>(options));
        }
    }
}
