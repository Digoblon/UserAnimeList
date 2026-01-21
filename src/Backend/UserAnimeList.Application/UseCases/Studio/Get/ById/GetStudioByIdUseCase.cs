using UserAnimeList.Application.Services.Mapper;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Repositories.Studio;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.Studio.Get.ById;

public class GetStudioByIdUseCase : IGetStudioByIdUseCase
{
    private readonly IStudioRepository _repository;
    private readonly IAppMapper _mapper;
    public GetStudioByIdUseCase(IStudioRepository repository,
        IAppMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    public async Task<ResponseStudioJson> Execute(string id)
    {
        var studio = await _repository.GetById(id);

        if (studio is null || !studio.IsActive)
            throw new NotFoundException(ResourceMessagesException.STUDIO_NOT_FOUND);
        
        var response = _mapper.Map<ResponseStudioJson>(studio);
        
        return response;
    }
}