using AutoMapper;

namespace Services.AutoMaper
{
    public class Mapper
    {
        private readonly IMapper _mapper;

        public Mapper(Profile profile)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(profile);
            });

            _mapper = config.CreateMapper();
        }

        public IMapper Instance => _mapper;
    }
}
