using UserAnimeList.Application.Services.Mapper;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Repositories.Studio;

namespace UserAnimeList.Application.UseCases.Studio.Get.ByName;

public class GetStudioByNameUseCase : IGetStudioByNameUseCase
{
    private readonly IStudioRepository _repository;
    private readonly IAppMapper _mapper;
    public GetStudioByNameUseCase(IStudioRepository repository,
        IAppMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<ResponseStudiosJson> Execute(RequestStudioGetByNameJson request)
    {
        var studios = await _repository.SearchByName(request.Name);
        
        var studiosDto = _mapper.Map<IList<ResponseStudioJson>>(studios);

        return new ResponseStudiosJson
        {
            Studios = studiosDto
        };
    }
}