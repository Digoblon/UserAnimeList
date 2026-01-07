namespace UserAnimeList.Application.Services.Mapper;

public interface IAppMapper 
{
    //public Domain.Entities.User MapToUserEntity(RequestRegisterUserJson request);
    TDestination Map<TDestination>(object source);
}