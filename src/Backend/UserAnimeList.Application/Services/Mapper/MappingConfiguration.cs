using Mapster;
using UserAnimeList.Communication.Requests;

namespace UserAnimeList.Application.Services.Mapper;

public class MappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RequestRegisterUserJson, Domain.Entities.User>()
            .Ignore(dest => dest.Password);
    }
}