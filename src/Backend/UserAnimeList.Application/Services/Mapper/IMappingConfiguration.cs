using Mapster;

namespace UserAnimeList.Application.Services.Mapper;

public interface IMappingConfiguration
{
    void Register(TypeAdapterConfig config);
}