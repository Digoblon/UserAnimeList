using Mapster;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Domain.Entities;

namespace UserAnimeList.Application.Services.Mapper;

public static class AnimeMapping
{
    public static TypeAdapterConfig UpdateConfig()
    {
        var config = new TypeAdapterConfig();

        config.NewConfig<RequestAnimeJson, Anime>()
            .IgnoreNullValues(true)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Synopsis, src => src.Synopsis ?? string.Empty)
            .Map(dest => dest.Episodes, src => src.Episodes)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Source, src => src.Source)
            .Map(dest => dest.Type, src => src.Type)
            .Map(dest => dest.AiredFrom, src => src.AiredFrom)
            .Map(dest => dest.AiredUntil, src => src.AiredUntil)
            
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedOn)
            
            .Ignore(dest => dest.DeletedOn!)
            .Ignore(dest => dest.IsActive)
            
            .Ignore(dest => dest.NameNormalized)

            .Ignore(dest => dest.Genres)
            .Ignore(dest => dest.Studios);

        return config;
    }
}