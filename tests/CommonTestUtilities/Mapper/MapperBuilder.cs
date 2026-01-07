using Mapster;
using UserAnimeList.Application.Services.Mapper;

namespace CommonTestUtilities.Mapper;

public class MapperBuilder
{
    public static IAppMapper Build()
    {
        var config = new TypeAdapterConfig();
        new MappingConfiguration().Register(config);

        return new UserAnimeList.Application.Services.Mapper.MapsterMapper(config);
    }
}