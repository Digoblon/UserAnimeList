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

        config.NewConfig<User, ResponseUserProfileJson>()
            .Map(dest => dest.ImageUrl, source => source.ImagePath);

        config.NewConfig<RequestAnimeJson, Anime>()
            .Ignore(dest => dest.Genres)
            .Ignore(dest => dest.Studios);

        config.NewConfig<Anime, ResponseAnimeJson>()
            .Ignore(dest => dest.Genres)
            .Ignore(dest => dest.Studios)
            .Map(dest => dest.ImageUrl, source => source.ImagePath);
        

        config.NewConfig<Anime, ResponseShortAnimeJson>()
            .IgnoreNullValues(true)
            .Map(dest => dest.ImageUrl, source => source.ImagePath);

        config.NewConfig<RequestUpdateAnimeListEntryJson, AnimeList>()
            .IgnoreNullValues(true)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.UserId)
            .Ignore(dest => dest.AnimeId);
        
        config.NewConfig<AnimeList, ResponseShortAnimeListEntryJson>()
            .Map(dest => dest.Name, src => src.Anime.Name);
    }
}
