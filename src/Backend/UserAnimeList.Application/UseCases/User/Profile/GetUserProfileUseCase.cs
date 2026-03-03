using UserAnimeList.Application.Services.Mapper;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Services.LoggedUser;

namespace UserAnimeList.Application.UseCases.User.Profile;

public class GetUserProfileUseCase : IGetUserProfileUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IAppMapper _mapper;
    
    public GetUserProfileUseCase(ILoggedUser loggedUser, IAppMapper mapper)
    {
        _loggedUser = loggedUser;
        _mapper = mapper;
    }
    
    public async Task<ResponseUserProfileJson> Execute()
    {
        var user = await _loggedUser.User();
        return _mapper.Map<ResponseUserProfileJson>(user);
    }
}