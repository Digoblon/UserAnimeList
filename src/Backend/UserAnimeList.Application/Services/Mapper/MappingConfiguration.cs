using Mapster;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Entities;

namespace UserAnimeList.Application.Services.Mapper;

public class MappingConfiguration : IMappingConfiguration
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RequestRegisterUserJson, User>()
            .Ignore(dest => dest.Password);

        config.NewConfig<RequestAnimeJson, Anime>()
            .Ignore(dest => dest.Genres)
            .Ignore(dest => dest.Studios);

        config.NewConfig<Anime, ResponseAnimeJson>()
            .Ignore(dest => dest.Genres)
            .Ignore(dest => dest.Studios);

        config.NewConfig<Anime, ResponseShortAnimeJson>()
            .IgnoreNullValues(true);
    }
}
