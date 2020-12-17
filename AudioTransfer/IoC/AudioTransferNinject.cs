using AudioTransfer.Configs;
using AutoMapper;
using Ninject.Modules;

namespace AudioTransfer.IoC {
    public class AudioTransferNinject : NinjectModule {
        public override void Load() {
            Bind<IMapper>().ToConstant(CreateMapper());
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
    }
}
