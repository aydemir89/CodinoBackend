// Interfaces
using Codino_UserCredential.Core.Dtos;
using Codino_UserCredential.Core.Dtos.Content;
using Codino_UserCredential.Core.Dtos.Content.Request;
using Codino_UserCredential.Core.Dtos.Content.Response;
using Codino_UserCredential.Repository.Models.Content;
using Codino_UserCredential.Repository.Repositories;

namespace Codino_UserCredential.Business.Concrete.Interfaces;

public interface IContentBusiness
{
    WorldMapResponse GetWorldMap();
    UserProgressResponse GetUserProgress(int userId);
    BiomeResponse GetBiome(int biomeId);
    ToyResponse GetToy(int toyId);
    TasksResponse GetTasks(int toyId);
    TaskSubmissionResponse SubmitTask(TaskSubmissionRequest request);
    
    ApiResponse CreateWorldMap(WorldMapCreateRequest request);
    ApiResponse UpdateWorldMap(int id, WorldMapCreateRequest request);
    ApiResponse DeleteWorldMap(int id);
    
    ApiResponse CreateBiome(BiomeCreateRequest request);
    ApiResponse UpdateBiome(int id, BiomeCreateRequest request);
    ApiResponse DeleteBiome(int id);
    
    ApiResponse CreateToy(ToyCreateRequest request);
    ApiResponse UpdateToy(int id, ToyCreateRequest request);
    ApiResponse DeleteToy(int id);
    
    ApiResponse CreateTask(TaskCreateRequest request);
    ApiResponse UpdateTask(int id, TaskCreateRequest request);
    ApiResponse DeleteTask(int id);
    
    List<WorldMapResponse> GetAllWorldMaps();
    List<BiomeResponse> GetAllBiomes();
    List<ToyResponse> GetAllToys();
    List<TaskDto> GetAllTasks();
    
    TaskSubmissionsResponse GetTaskSubmissions(int userId, int taskId);
    ToyTaskStatusResponse GetToyTaskStatus(int userId, int toyId);
    
    Task<ToyActivationResponse> ActivateToyAsync(ActivateToyRequest request);
    Task<IEnumerable<ToyActivationCodeResponse>> GenerateActivationCodesAsync(GenerateActivationCodesRequest request);
    Task<ToyActivationDetailsResponse> GetToyActivationDetailsAsync(int activationCodeId);
    Task<IEnumerable<ToyActivationSummaryResponse>> GetToyActivationSummaryAsync(int toyId);
}
