using Mapster;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Domain.Entities;

namespace UserAnimeList.Application.Services.Mapper;

/*
public class MappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RequestRegisterUserJson, Domain.Entities.User>()
            .Ignore(dest => dest.Password);
    }
}
*/
public class MappingConfiguration : IMappingConfiguration
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RequestRegisterUserJson, User>()
            .Ignore(dest => dest.Password);
    }
}
