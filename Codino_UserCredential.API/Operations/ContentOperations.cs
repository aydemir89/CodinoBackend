using Codino_UserCredential.API.Operations.Interfaces;
using Codino_UserCredential.Business.Concrete.Interfaces;
using Codino_UserCredential.Core.Dtos;
using Codino_UserCredential.Core.Dtos.Content;
using Codino_UserCredential.Core.Dtos.Content.Request;
using Codino_UserCredential.Core.Dtos.Content.Response;
using Codino_UserCredential.Repository.Repositories;

namespace Codino_UserCredential.API.Operations;

public class ContentOperations : IContentOperations
{
    private readonly IContentBusiness contentBusiness;
    
    public ContentOperations(IContentBusiness contentBusiness)
    {
        this.contentBusiness = contentBusiness;
    }
    
    public WorldMapResponse GetWorldMap()
    {
        return contentBusiness.GetWorldMap();
    }
    
    public UserProgressResponse GetUserProgress(int userId)
    {
        return contentBusiness.GetUserProgress(userId);
    }
    
    public BiomeResponse GetBiome(int biomeId)
    {
        return contentBusiness.GetBiome(biomeId);
    }
    
    public ToyResponse GetToy(int toyId)
    {
        return contentBusiness.GetToy(toyId);
    }
    
    public TasksResponse GetTasks(int toyId)
    {
        return contentBusiness.GetTasks(toyId);
    }
    
    public TaskSubmissionResponse SubmitTask(TaskSubmissionRequest request)
    {
        return contentBusiness.SubmitTask(request);
    }
    
    // İçerik yönetimi için CRUD metodları
    public ApiResponse CreateWorldMap(WorldMapCreateRequest request)
    {
        return contentBusiness.CreateWorldMap(request);
    }
    
    public ApiResponse UpdateWorldMap(int id, WorldMapCreateRequest request)
    {
        return contentBusiness.UpdateWorldMap(id, request);
    }
    
    public ApiResponse DeleteWorldMap(int id)
    {
        return contentBusiness.DeleteWorldMap(id);
    }
    
    public ApiResponse CreateBiome(BiomeCreateRequest request)
    {
        return contentBusiness.CreateBiome(request);
    }
    
    public ApiResponse UpdateBiome(int id, BiomeCreateRequest request)
    {
        return contentBusiness.UpdateBiome(id, request);
    }
    
    public ApiResponse DeleteBiome(int id)
    {
        return contentBusiness.DeleteBiome(id);
    }
    
    public ApiResponse CreateToy(ToyCreateRequest request)
    {
        return contentBusiness.CreateToy(request);
    }
    
    public ApiResponse UpdateToy(int id, ToyCreateRequest request)
    {
        return contentBusiness.UpdateToy(id, request);
    }
    
    public ApiResponse DeleteToy(int id)
    {
        return contentBusiness.DeleteToy(id);
    }
    
    public ApiResponse CreateTask(TaskCreateRequest request)
    {
        return contentBusiness.CreateTask(request);
    }
    
    public ApiResponse UpdateTask(int id, TaskCreateRequest request)
    {
        return contentBusiness.UpdateTask(id, request);
    }
    
    public ApiResponse DeleteTask(int id)
    {
        return contentBusiness.DeleteTask(id);
    }
    
    // Liste endpointleri
    public List<WorldMapResponse> GetAllWorldMaps()
    {
        return contentBusiness.GetAllWorldMaps();
    }
    
    public List<BiomeResponse> GetAllBiomes()
    {
        return contentBusiness.GetAllBiomes();
    }
    
    public List<ToyResponse> GetAllToys()
    {
        return contentBusiness.GetAllToys();
    }
    
    public List<TaskDto> GetAllTasks()
    {
        return contentBusiness.GetAllTasks();
    }
    
    public TaskSubmissionsResponse GetTaskSubmissions(int userId, int taskId)
    {
        return contentBusiness.GetTaskSubmissions(userId, taskId);
    }
    
    public ToyTaskStatusResponse GetToyTaskStatus(int userId, int toyId)
    {
        return contentBusiness.GetToyTaskStatus(userId, toyId);
    }

    public async Task<IEnumerable<ToyActivationCodeResponse>> GenerateActivationCodesAsync(GenerateActivationCodesRequest request)
    {
        return await contentBusiness.GenerateActivationCodesAsync(request);
    }

    public async Task<ToyActivationDetailsResponse> GetToyActivationDetailsAsync(int activationCodeId)
    {
        return await contentBusiness.GetToyActivationDetailsAsync(activationCodeId);
    }

    public async Task<IEnumerable<ToyActivationSummaryResponse>> GetActivationStatisticsAsync(int? toyId = null)
    {
        if (toyId.HasValue)
            return await contentBusiness.GetToyActivationSummaryAsync(toyId.Value);
    
        var toys = contentBusiness.GetAllToys();
        var result = new List<ToyActivationSummaryResponse>();
    
        foreach (var toy in toys)
        {
            var summary = await contentBusiness.GetToyActivationSummaryAsync(toy.ToyId);
            result.AddRange(summary);
        }

        return result;
    }
}