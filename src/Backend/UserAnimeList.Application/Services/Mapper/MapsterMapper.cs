using Mapster;
using MapsterMapper;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Domain.Entities;

namespace UserAnimeList.Application.Services.Mapper;

public class MapsterMapper : IAppMapper
{
    private readonly TypeAdapterConfig _config;

    public MapsterMapper(TypeAdapterConfig config)
    {
        _config = config;
    }

    public TDestination Map<TDestination>(object source)
    {
        return source.Adapt<TDestination>(_config);
    }
}
