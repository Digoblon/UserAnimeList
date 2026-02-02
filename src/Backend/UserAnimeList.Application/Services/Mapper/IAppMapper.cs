using UserAnimeList.Communication.Requests;
using UserAnimeList.Domain.Entities;

namespace UserAnimeList.Application.Services.Mapper;

public interface IAppMapper 
{
    TDestination Map<TDestination>(object source);
    public Anime UpdateToAnime(Anime anime, RequestAnimeJson request);
}