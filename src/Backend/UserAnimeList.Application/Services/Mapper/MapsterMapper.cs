using Mapster;
using MapsterMapper;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Domain.Entities;

namespace UserAnimeList.Application.Services.Mapper;

public class MapsterMapper : IAppMapper
{
    //public User MapToUserEntity(RequestRegisterUserJson request) => request.Adapt<User>();
    public TDestination Map<TDestination>(object source) => source.Adapt<TDestination>();
}