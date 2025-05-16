using System.Security.Claims;
using Codino_UserCredential.Business.Concrete.Interfaces;
using Codino_UserCredential.Business.Services;
using Codino_UserCredential.Business.Validators;
using Codino_UserCredential.Core.Dtos;
using Codino_UserCredential.Core.Dtos.Content;
using Codino_UserCredential.Core.Dtos.Content.Request;
using Codino_UserCredential.Core.Dtos.Content.Response;
using Codino_UserCredential.Core.Enums;
using Codino_UserCredential.Repository.Context;
using Codino_UserCredential.Repository.Models.Content;
using Codino_UserCredential.Repository.Repositories;
using Codino_UserCredential.Repository.Repositories.Content;
using Codino_UserCredential.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Codino_UserCredential.Business.Concrete;

public class ContentBusiness : IContentBusiness
{
    private readonly IWorldMapRepository worldMapRepository;
    private readonly IBiomeRepository biomeRepository;
    private readonly IToyRepository toyRepository;
    private readonly ITaskRepository taskRepository;
    private readonly ITaskSubmissionRepository taskSubmissionRepository;
    private readonly IStringLocalizer localizer;
    private readonly IUnitOfWork<CodinoDbContext> unitOfWork;
    private readonly IToyActivationCodeRepository toyActivationCodeRepository;
    private readonly IToyAvatarRepository toyAvatarRepository;
    private readonly IUserToyRepository userToyRepository;
    private readonly IUserRepository userRepository;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IServiceProvider serviceProvider;
    private readonly QrCodeGenerationService _qrCodeGenerationService;
    private readonly ToyActivatorValidator _toyActivatorValidator;
    public ContentBusiness(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        _qrCodeGenerationService = serviceProvider.GetService<QrCodeGenerationService>();
        localizer = serviceProvider.GetService<IStringLocalizer<ContentBusiness>>();
        worldMapRepository = serviceProvider.GetService<IWorldMapRepository>();
        biomeRepository = serviceProvider.GetService<IBiomeRepository>();
        toyRepository = serviceProvider.GetService<IToyRepository>();
        taskRepository = serviceProvider.GetService<ITaskRepository>();
        taskSubmissionRepository = serviceProvider.GetService<ITaskSubmissionRepository>();
        unitOfWork = serviceProvider.GetService<IUnitOfWork<CodinoDbContext>>();
        httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>(); 
        toyActivationCodeRepository = serviceProvider.GetService<IToyActivationCodeRepository>();
        toyAvatarRepository = serviceProvider.GetService<IToyAvatarRepository>();
        userToyRepository = serviceProvider.GetService<IUserToyRepository>();
        userRepository = serviceProvider.GetService<IUserRepository>();
        _toyActivatorValidator = serviceProvider.GetService<ToyActivatorValidator>();
    }
    
    public WorldMapResponse GetWorldMap()
    {
        var response = new WorldMapResponse();
        try
        {
            var worldMap = worldMapRepository.GetQuery(w => w.StatusId == Status.Valid).FirstOrDefault();
            if (worldMap == null)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = localizer.GetString("WorldMapNotFound");
                return response;
            }
            response.Id = worldMap.id;
            response.Name = worldMap.Name;
            response.BackgroundImageUrl = worldMap.BackgroundImageUrl;
            response.Code = (int)ResponseCode.Success;
            response.Message = localizer.GetString("Success");
        }
        catch (Exception ex)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = ex.Message;
        }
        
        return response;
    }

    public UserProgressResponse GetUserProgress(int userId)
    {
        var response = new UserProgressResponse();
        try
        {
            var biomes = biomeRepository.GetQuery(b => b.StatusId == Status.Valid).ToList();
            var biomeProgressList = new List<BiomeProgress>();
            foreach (var biome in biomes)
            {
                var toysInBiome = toyRepository.GetQuery(t => t.BiomeId == biome.id && t.StatusId == Status.Valid)
                    .ToList();
                var tasksIds = new List<int>();
                foreach (var toy in toysInBiome)
                {
                    var toyTasks = taskRepository.GetQuery(t => t.ToyId == toy.id && t.StatusId == Status.Valid)
                        .Select(t => t.id).ToList();
                    tasksIds.AddRange(toyTasks);
                }

                var completedTasks = taskSubmissionRepository.GetQuery(ts => ts.UserId == userId &&
                                                                             tasksIds.Contains(ts.TaskId) &&
                                                                             ts.IsCorrect).Select(ts => ts.TaskId)
                    .Distinct().Count();
                var totalTasks = tasksIds.Count();
                var completionRate = totalTasks > 0 ? (double)completedTasks / totalTasks * 100 : 0;
                biomeProgressList.Add(new BiomeProgress
                {
                    BiomeId = biome.id,
                    BiomeName = biome.Name,
                    CompletedTaskCount = completedTasks,
                    TotalTaskCount = totalTasks,
                    CompletionRate = (int)completionRate
                });
            }
            response.BiomeProgresses = biomeProgressList;
            response.Code = (int)ResponseCode.Success;
            response.Message = localizer.GetString("Success");
        }
        catch (Exception ex)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = ex.Message;
        }
        return response;
    }

    public BiomeResponse GetBiome(int biomeId)
    {
        var response = new BiomeResponse();
        
        try
        {
            var biome = biomeRepository.GetById(biomeId);
            
            if (biome == null || biome.StatusId != Status.Valid)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = localizer.GetString("BiomeNotFound");
                return response;
            }
            
            var toys = toyRepository.GetQuery(t => t.BiomeId == biomeId && t.StatusId == Status.Valid).ToList();
            var toyList = new List<ToyDto>();
            
            foreach (var toy in toys)
            {
                // Oyuncağa ait görevleri say
                var totalTasks = taskRepository.GetQuery(t => t.ToyId == toy.id && t.StatusId == Status.Valid).Count();
                
                toyList.Add(new ToyDto
                {
                    Id = toy.id,
                    Name = toy.Name,
                    IconImageUrl = toy.IconImageUrl,
                    Description = toy.Description,
                    TaskCount = totalTasks
                });
            }
            
            response.BiomeId = biome.id;
            response.Name = biome.Name;
            response.BackgroundImageUrl = biome.BackgroundImageUrl;
            response.Toys = toyList;
            response.Code = (int)ResponseCode.Success;
            response.Message = localizer.GetString("Success");
        }
        catch (Exception ex)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = ex.Message;
        }
        
        return response;
    }

    public ToyResponse GetToy(int toyId)
    {
        var response = new ToyResponse();
        
        try
        {
            var toy = toyRepository.GetById(toyId);
            
            if (toy == null || toy.StatusId != Status.Valid)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = localizer.GetString("ToyNotFound");
                return response;
            }
            
            response.ToyId = toy.id;
            response.Name = toy.Name;
            response.Description = toy.Description;
            response.IconImageUrl = toy.IconImageUrl;
            response.Code = (int)ResponseCode.Success;
            response.Message = localizer.GetString("Success");
        }
        catch (Exception ex)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = ex.Message;
        }
        
        return response;
    }

    public TasksResponse GetTasks(int toyId)
    {
        var response = new TasksResponse();
        
        try
        {
            var toy = toyRepository.GetById(toyId);
            
            if (toy == null || toy.StatusId != Status.Valid)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = localizer.GetString("ToyNotFound");
                return response;
            }
            
            var tasks = taskRepository.GetQuery(t => t.ToyId == toyId && t.StatusId == Status.Valid).ToList();
            var taskList = new List<TaskDto>();
            
            foreach (var task in tasks)
            {
                taskList.Add(new TaskDto
                {
                    Id = task.id,
                    Title = task.Title,
                    Description = task.Description
                });
            }
            
            response.ToyId = toyId;
            response.ToyName = toy.Name;
            response.Tasks = taskList;
            response.Code = (int)ResponseCode.Success;
            response.Message = localizer.GetString("Success");
        }
        catch (Exception ex)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = ex.Message;
        }
        
        return response;
    }

    public TaskSubmissionResponse SubmitTask(TaskSubmissionRequest request)
    {
        var response = new TaskSubmissionResponse();
        
        try
        {
            var task = taskRepository.GetById(request.TaskId);
            
            if (task == null || task.StatusId != Status.Valid)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = localizer.GetString("TaskNotFound");
                return response;
            }
            
            bool isCorrect = IsCodeCorrect(request.SubmittedCode, task.ExpectedPattern);
            
            var submission = new TaskSubmission
            {
                UserId = request.UserId,
                TaskId = request.TaskId,
                SubmittedCode = request.SubmittedCode,
                IsCorrect = isCorrect,
                Timestamp = DateTime.UtcNow,
                StatusId = Status.Valid,
                creaDateTime = DateTime.UtcNow,
                CreaUserId = request.UserId
            };
            
            taskSubmissionRepository.Insert(submission);
            unitOfWork.SaveChanges();
            
            response.IsCorrect = isCorrect;
            response.Code = (int)ResponseCode.Success;
            response.Message = isCorrect ? 
                localizer.GetString("TaskCompletedSuccessfully") : 
                localizer.GetString("TaskNotCompletedCorrectly");
        }
        catch (Exception ex)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = ex.Message;
        }
        
        return response;
    }
    
    private bool IsCodeCorrect(string submittedCode, string expectedPattern)
    {
        return submittedCode.Contains(expectedPattern);
    }
    
    public ApiResponse CreateWorldMap(WorldMapCreateRequest request)
    {
        var response = new ApiResponse();
        
        try
        {
            var newWorldMap = new WorldMap
            {
                Name = request.Name,
                BackgroundImageUrl = request.BackgroundImageUrl,
                StatusId = Status.Valid,
                creaDateTime = DateTime.UtcNow,
                CreaUserId = 1 // Aktif kullanıcı ID'si alınmalı
            };
            
            worldMapRepository.Insert(newWorldMap);
            unitOfWork.SaveChanges();
            
            response.Code = (int)ResponseCode.Success;
            response.Message = localizer.GetString("WorldMapCreatedSuccessfully");
        }
        catch (Exception ex)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = ex.Message;
        }
        
        return response;
    }

    public ApiResponse UpdateWorldMap(int id, WorldMapCreateRequest request)
    {
        var response = new ApiResponse();
        
        try
        {
            var worldMap = worldMapRepository.GetById(id);
            
            if (worldMap == null || worldMap.StatusId != Status.Valid)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = localizer.GetString("WorldMapNotFound");
                return response;
            }
            
            worldMap.Name = request.Name;
            worldMap.BackgroundImageUrl = request.BackgroundImageUrl;
            
            worldMapRepository.Update(worldMap);
            unitOfWork.SaveChanges();
            
            response.Code = (int)ResponseCode.Success;
            response.Message = localizer.GetString("WorldMapUpdatedSuccessfully");
        }
        catch (Exception ex)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = ex.Message;
        }
        
        return response;
    }

    public ApiResponse DeleteWorldMap(int id)
    {
        var response = new ApiResponse();
        
        try
        {
            var worldMap = worldMapRepository.GetById(id);
            
            if (worldMap == null || worldMap.StatusId != Status.Valid)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = localizer.GetString("WorldMapNotFound");
                return response;
            }
            
            // Soft delete
            worldMap.StatusId = Status.InValid;
            
            worldMapRepository.Update(worldMap);
            unitOfWork.SaveChanges();
            
            response.Code = (int)ResponseCode.Success;
            response.Message = localizer.GetString("WorldMapDeletedSuccessfully");
        }
        catch (Exception ex)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = ex.Message;
        }
        
        return response;
    }

    public List<WorldMapResponse> GetAllWorldMaps()
    {
        var response = new List<WorldMapResponse>();
        
        try
        {
            var worldMaps = worldMapRepository.GetQuery(w => w.StatusId == Status.Valid).ToList();
            
            foreach (var worldMap in worldMaps)
            {
                response.Add(new WorldMapResponse
                {
                    Id = worldMap.id,
                    Name = worldMap.Name,
                    BackgroundImageUrl = worldMap.BackgroundImageUrl,
                    Code = (int)ResponseCode.Success,
                    Message = localizer.GetString("Success")
                });
            }
        }
        catch (Exception)
        {
            // Log exception
        }
        
        return response;
    }

    // Biome CRUD
    public ApiResponse CreateBiome(BiomeCreateRequest request)
    {
        var response = new ApiResponse();
    
        try
        {
            // WorldMap kontrolü
            var worldMap = worldMapRepository.GetById(request.WorldMapId);
        
            if (worldMap == null || worldMap.StatusId != Status.Valid)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = localizer.GetString("WorldMapNotFound");
                return response;
            }
        
            var newBiome = new Biome
            {
                Name = request.Name,
                WorldMapId = request.WorldMapId, // Entity sınıfımızda güncelledik, bu artık "WorldMapid" sütununa eşlenecek
                BackgroundImageUrl = request.BackgroundImageUrl,
                StatusId = Status.Valid,
                creaDateTime = DateTime.UtcNow,
                CreaUserId = 1 // Aktif kullanıcı ID'si alınmalı
            };
        
            biomeRepository.Insert(newBiome);
            unitOfWork.SaveChanges();
        
            response.Code = (int)ResponseCode.Success;
            response.Message = localizer.GetString("BiomeCreatedSuccessfully");
        }
        catch (Exception ex)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = ex.Message;
        }
    
        return response;
    }
    public ApiResponse UpdateBiome(int id, BiomeCreateRequest request)
    {
        var response = new ApiResponse();
        
        try
        {
            var biome = biomeRepository.GetById(id);
            
            if (biome == null || biome.StatusId != Status.Valid)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = localizer.GetString("BiomeNotFound");
                return response;
            }
            
            // WorldMap kontrolü
            var worldMap = worldMapRepository.GetById(request.WorldMapId);
            
            if (worldMap == null || worldMap.StatusId != Status.Valid)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = localizer.GetString("WorldMapNotFound");
                return response;
            }
            
            biome.Name = request.Name;
            biome.WorldMapId = request.WorldMapId;
            biome.BackgroundImageUrl = request.BackgroundImageUrl;
            
            biomeRepository.Update(biome);
            unitOfWork.SaveChanges();
            
            response.Code = (int)ResponseCode.Success;
            response.Message = localizer.GetString("BiomeUpdatedSuccessfully");
        }
        catch (Exception ex)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = ex.Message;
        }
        
        return response;
    }

    public ApiResponse DeleteBiome(int id)
    {
        var response = new ApiResponse();
        
        try
        {
            var biome = biomeRepository.GetById(id);
            
            if (biome == null || biome.StatusId != Status.Valid)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = localizer.GetString("BiomeNotFound");
                return response;
            }
            
            // Soft delete
            biome.StatusId = Status.InValid;
            
            biomeRepository.Update(biome);
            unitOfWork.SaveChanges();
            
            response.Code = (int)ResponseCode.Success;
            response.Message = localizer.GetString("BiomeDeletedSuccessfully");
        }
        catch (Exception ex)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = ex.Message;
        }
        
        return response;
    }

    public List<BiomeResponse> GetAllBiomes()
    {
        var response = new List<BiomeResponse>();
        
        try
        {
            var biomes = biomeRepository.GetQuery(b => b.StatusId == Status.Valid).ToList();
            
            foreach (var biome in biomes)
            {
                var toys = toyRepository.GetQuery(t => t.BiomeId == biome.id && t.StatusId == Status.Valid).ToList();
                var toyList = new List<ToyDto>();
                
                foreach (var toy in toys)
                {
                    // Oyuncağa ait görevleri say
                    var totalTasks = taskRepository.GetQuery(t => t.ToyId == toy.id && t.StatusId == Status.Valid).Count();
                    
                    toyList.Add(new ToyDto
                    {
                        Id = toy.id,
                        Name = toy.Name,
                        IconImageUrl = toy.IconImageUrl,
                        Description = toy.Description,
                        TaskCount = totalTasks
                    });
                }
                
                response.Add(new BiomeResponse
                {
                    BiomeId = biome.id,
                    Name = biome.Name,
                    BackgroundImageUrl = biome.BackgroundImageUrl,
                    Toys = toyList,
                    Code = (int)ResponseCode.Success,
                    Message = localizer.GetString("Success")
                });
            }
        }
        catch (Exception)
        {
            // Log exception
        }
        
        return response;
    }

    // Toy CRUD
    public ApiResponse CreateToy(ToyCreateRequest request)
    {
        var response = new ApiResponse();
        
        try
        {
            // Biome kontrolü
            var biome = biomeRepository.GetById(request.BiomeId);
            
            if (biome == null || biome.StatusId != Status.Valid)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = localizer.GetString("BiomeNotFound");
                return response;
            }
            
            var newToy = new Toy
            {
                Name = request.Name,
                BiomeId = request.BiomeId, 
                IconImageUrl = request.IconImageUrl,
                Description = request.Description,
                StatusId = Status.Valid,
                creaDateTime = DateTime.UtcNow,
                CreaUserId = GetCurrentUserId() 
            };
            
            toyRepository.Insert(newToy);
            unitOfWork.SaveChanges();
            
            response.Code = (int)ResponseCode.Success;
            response.Message = localizer.GetString("ToyCreatedSuccessfully");
        }
        catch (Exception ex)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = ex.Message;
        }
        
        return response;
    }

    public ApiResponse UpdateToy(int id, ToyCreateRequest request)
    {
        var response = new ApiResponse();
        
        try
        {
            var toy = toyRepository.GetById(id);
            
            if (toy == null || toy.StatusId != Status.Valid)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = localizer.GetString("ToyNotFound");
                return response;
            }
            
            // Biome kontrolü
            var biome = biomeRepository.GetById(request.BiomeId);
            
            if (biome == null || biome.StatusId != Status.Valid)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = localizer.GetString("BiomeNotFound");
                return response;
            }
            
            toy.Name = request.Name;
            toy.BiomeId = request.BiomeId;
            toy.IconImageUrl = request.IconImageUrl;
            toy.Description = request.Description;
            
            toyRepository.Update(toy);
            unitOfWork.SaveChanges();
            
            response.Code = (int)ResponseCode.Success;
            response.Message = localizer.GetString("ToyUpdatedSuccessfully");
        }
        catch (Exception ex)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = ex.Message;
        }
        
        return response;
    }

    public ApiResponse DeleteToy(int id)
    {
        var response = new ApiResponse();
        
        try
        {
            var toy = toyRepository.GetById(id);
            
            if (toy == null || toy.StatusId != Status.Valid)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = localizer.GetString("ToyNotFound");
                return response;
            }
            
            // Soft delete
            toy.StatusId = Status.InValid;
            
            toyRepository.Update(toy);
            unitOfWork.SaveChanges();
            
            response.Code = (int)ResponseCode.Success;
            response.Message = localizer.GetString("ToyDeletedSuccessfully");
        }
        catch (Exception ex)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = ex.Message;
        }
        
        return response;
    }

    public List<ToyResponse> GetAllToys()
    {
        var response = new List<ToyResponse>();
        
        try
        {
            var toys = toyRepository.GetQuery(t => t.StatusId == Status.Valid).ToList();
            
            foreach (var toy in toys)
            {
                response.Add(new ToyResponse
                {
                    ToyId = toy.id,
                    Name = toy.Name,
                    Description = toy.Description,
                    IconImageUrl = toy.IconImageUrl,
                    Code = (int)ResponseCode.Success,
                    Message = localizer.GetString("Success")
                });
            }
        }
        catch (Exception)
        {
            // Log exception
        }
        
        return response;
    }

    // Task CRUD
    public ApiResponse CreateTask(TaskCreateRequest request)
    {
        var response = new ApiResponse();
        
        try
        {
            // Toy kontrolü
            var toy = toyRepository.GetById(request.ToyId);
            
            if (toy == null || toy.StatusId != Status.Valid)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = localizer.GetString("ToyNotFound");
                return response;
            }
            var currentUserId = GetCurrentUserId();
            var newTask = new ProgrammingTask
            {
                ToyId = request.ToyId,
                Title = request.Title,
                Description = request.Description,
                ExpectedPattern = request.ExpectedPattern,
                StatusId = Status.Valid,
                creaDateTime = DateTime.UtcNow,
                CreaUserId = currentUserId 
            };
            
            taskRepository.Insert(newTask);
            unitOfWork.SaveChanges();
            
            response.Code = (int)ResponseCode.Success;
            response.Message = localizer.GetString("TaskCreatedSuccessfully");
        }
        catch (Exception ex)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = ex.Message;
        }
        
        return response;
    }
    
    private int GetCurrentUserId()
    {
        try
        {
            var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
        
            if (httpContextAccessor?.HttpContext == null)
                return 0; 
        
            var userIdClaim = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return 0; 
        
            return userId;
        }
        catch
        {
            return 0; 
        }
    }
    
    public ApiResponse UpdateTask(int id, TaskCreateRequest request)
    {
        var response = new ApiResponse();
        
        try
        {
            var task = taskRepository.GetById(id);
            
            if (task == null || task.StatusId != Status.Valid)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = localizer.GetString("TaskNotFound");
                return response;
            }
            
            // Toy kontrolü
            var toy = toyRepository.GetById(request.ToyId);
            
            if (toy == null || toy.StatusId != Status.Valid)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = localizer.GetString("ToyNotFound");
                return response;
            }
            
            task.ToyId = request.ToyId;
            task.Title = request.Title;
            task.Description = request.Description;
            task.ExpectedPattern = request.ExpectedPattern;
            
            taskRepository.Update(task);
            unitOfWork.SaveChanges();
            
            response.Code = (int)ResponseCode.Success;
            response.Message = localizer.GetString("TaskUpdatedSuccessfully");
        }
        catch (Exception ex)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = ex.Message;
        }
        
        return response;
    }

    public ApiResponse DeleteTask(int id)
    {
        var response = new ApiResponse();
        
        try
        {
            var task = taskRepository.GetById(id);
            
            if (task == null || task.StatusId != Status.Valid)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = localizer.GetString("TaskNotFound");
                return response;
            }
            
            // Soft delete
            task.StatusId = Status.InValid;
            
            taskRepository.Update(task);
            unitOfWork.SaveChanges();
            
            response.Code = (int)ResponseCode.Success;
            response.Message = localizer.GetString("TaskDeletedSuccessfully");
        }
        catch (Exception ex)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = ex.Message;
        }
        
        return response;
    }

    public List<TaskDto> GetAllTasks()
    {
        var response = new List<TaskDto>();
        
        try
        {
            var tasks = taskRepository.GetQuery(t => t.StatusId == Status.Valid).ToList();
            
            foreach (var task in tasks)
            {
                response.Add(new TaskDto
                {
                    Id = task.id,
                    Title = task.Title,
                    Description = task.Description
                });
            }
        }
        catch (Exception)
        {
            // Log exception
        }
        
        return response;
    }

    // Kullanıcı görev takibi
    public TaskSubmissionsResponse GetTaskSubmissions(int userId, int taskId)
    {
        var response = new TaskSubmissionsResponse();
        
        try
        {
            var task = taskRepository.GetById(taskId);
            
            if (task == null || task.StatusId != Status.Valid)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = localizer.GetString("TaskNotFound");
                return response;
            }
            
            var submissions = taskSubmissionRepository.GetQuery(ts => 
                ts.UserId == userId && 
                ts.TaskId == taskId)
                .OrderByDescending(ts => ts.Timestamp)
                .ToList();
            
            var submissionDtos = new List<TaskSubmissionDto>();
            
            foreach (var submission in submissions)
            {
                submissionDtos.Add(new TaskSubmissionDto
                {
                    Id = submission.id,
                    SubmittedCode = submission.SubmittedCode,
                    IsCorrect = submission.IsCorrect,
                    Timestamp = submission.Timestamp
                });
            }
            
            response.TaskId = taskId;
            response.TaskTitle = task.Title;
            response.Submissions = submissionDtos;
            response.Code = (int)ResponseCode.Success;
            response.Message = localizer.GetString("Success");
        }
        catch (Exception ex)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = ex.Message;
        }
        
        return response;
    }

    public ToyTaskStatusResponse GetToyTaskStatus(int userId, int toyId)
    {
        var response = new ToyTaskStatusResponse();
        
        try
        {
            var toy = toyRepository.GetById(toyId);
            
            if (toy == null || toy.StatusId != Status.Valid)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = localizer.GetString("ToyNotFound");
                return response;
            }
            
            var tasks = taskRepository.GetQuery(t => 
                t.ToyId == toyId && 
                t.StatusId == Status.Valid)
                .ToList();
            
            var taskStatusDtos = new List<TaskStatusDto>();
            
            foreach (var task in tasks)
            {
                // Kullanıcının bu görevi tamamlayıp tamamlamadığını kontrol et
                var successfulSubmission = taskSubmissionRepository.GetQuery(ts => 
                    ts.UserId == userId && 
                    ts.TaskId == task.id && 
                    ts.IsCorrect)
                    .OrderByDescending(ts => ts.Timestamp)
                    .FirstOrDefault();
                
                taskStatusDtos.Add(new TaskStatusDto
                {
                    TaskId = task.id,
                    Title = task.Title,
                    Description = task.Description,
                    IsCompleted = successfulSubmission != null,
                    CompletionDate = successfulSubmission?.Timestamp
                });
            }
            
            response.ToyId = toyId;
            response.ToyName = toy.Name;
            response.Tasks = taskStatusDtos;
            response.Code = (int)ResponseCode.Success;
            response.Message = localizer.GetString("Success");
        }
        catch (Exception ex)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = ex.Message;
        }
        
        return response;
    }

    public async Task<ToyActivationResponse> ActivateToyAsync(ActivateToyRequest request)
    {
        var validationResult = _toyActivatorValidator.ValidateActivateToyRequest(request);
        if (validationResult.Code != (int)ResponseCode.Success)
        {
            return new ToyActivationResponse
            {
                Code = validationResult.Code,
                Message = validationResult.Message
            };
        }
        
        var response = new ToyActivationResponse();
        try
        {
            var user = await Task.Run(() => userRepository.GetById(request.UserId));
            if (user == null || user.StatusId != Status.Valid)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = localizer.GetString("UserNotFound");
                return response;
            }

            var activationCode = await Task.Run((() =>
                toyActivationCodeRepository
                    .GetQuery(tac => tac.ActivationCode == request.ActivationCode && tac.StatusId == Status.Valid)
                    .FirstOrDefault()));
            if (activationCode == null)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = localizer.GetString("InvalidActivationCode");
                return response;
            }
            var toy = await Task.Run(() => toyRepository.GetById(activationCode.ToyId));
        
            if (toy == null || toy.StatusId != Status.Valid)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = localizer.GetString("ToyNotFound");
                return response;
            }
            if (activationCode.IsActivated)
            {
                if (activationCode.ActivatedByUserId == request.UserId)
                {
                    response.Code = (int)ResponseCode.Success;
                    response.Message = localizer.GetString("ToyAlreadyActivatedByYou");
                    response.ToyId = toy.id;
                    response.ToyName = toy.Name;
                    response.ToyImageUrl = toy.IconImageUrl;
                    response.ActivationCode = activationCode.ActivationCode;
                    response.IsNewlyActivated = false;
                
                    var toyAvatars = await Task.Run(() => 
                        toyAvatarRepository.GetQuery(ta => 
                                ta.ToyId == toy.id && 
                                ta.StatusId == Status.Valid)
                            .ToList());
                
                    foreach (var avatar in toyAvatars)
                    {
                        response.UnlockedAvatars.Add(new ToyAvatarDto
                        {
                            Id = avatar.id,
                            Name = avatar.Name,
                            AvatarUrl = avatar.AvatarUrl,
                            RequiredToyLevel = avatar.RequiredToyLevel,
                            RequiredUserXp = avatar.RequiredUserXp
                        });
                    }
                
                    return response;
                }
                else
                {
                    response.Code = (int)ResponseCode.Error;
                    response.Message = localizer.GetString("ActivationCodeAlreadyUsed");
                    return response;
                }
            }

            activationCode.IsActivated = true;
            activationCode.ActivatedByUserId = request.UserId;
            activationCode.ActivationDate = DateTime.UtcNow;
            await Task.Run(() => toyActivationCodeRepository.Update(activationCode));

            var userToy = new UserToy
            {
                UserId = request.UserId,
                ToyId = activationCode.ToyId,
                UnlockDate = DateTime.UtcNow,
                StatusId = Status.Valid,
                creaDateTime = DateTime.UtcNow,
                CreaUserId = request.UserId
            };
            await Task.Run((() => userToyRepository.Insert(userToy)));
            await Task.Run((() => unitOfWork.SaveChanges()));
            
            response.Code = (int)ResponseCode.Success;
            response.Message = localizer.GetString("ToyActivatedSuccessfully");
            response.ToyId = toy.id;
            response.ToyName = toy.Name;
            response.ToyImageUrl = toy.IconImageUrl;
            response.ActivationCode = activationCode.ActivationCode;
            response.IsNewlyActivated = true;
            
            var unlockedAvatars = await Task.Run(() => 
                toyAvatarRepository.GetQuery(ta => 
                        ta.ToyId == toy.id && 
                        ta.StatusId == Status.Valid)
                    .ToList());
        
            foreach (var avatar in unlockedAvatars)
            {
                response.UnlockedAvatars.Add(new ToyAvatarDto
                {
                    Id = avatar.id,
                    Name = avatar.Name,
                    AvatarUrl = avatar.AvatarUrl,
                    RequiredToyLevel = avatar.RequiredToyLevel,
                    RequiredUserXp = avatar.RequiredUserXp
                });
            }
        }
        catch (Exception e)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = e.Message;
        }

        return response;
    }

    public async Task<IEnumerable<ToyActivationCodeResponse>> GenerateActivationCodesAsync(GenerateActivationCodesRequest request)
    {
        var response = new List<ToyActivationCodeResponse>();
        try
        {
            var toy = await Task.Run((() => toyRepository.GetByIdAsync(request.ToyId)));
            if (toy == null || toy.StatusId != Status.Valid)
            {
                var errorResponse = new ToyActivationCodeResponse
                {
                    Code = (int)ResponseCode.Error,
                    Message = localizer.GetString("ToyNotFound")
                };
                response.Add(errorResponse);
                return response;
            }

            for (int i = 0; i < request.Count; i++)
            {
                string activationCode = await _qrCodeGenerationService.GenerateUniqueActivationCodeAsync();
                    //await GenerateUniqueActivationCodeAsync();
                
                var newActivationCode = new ToyActivationCode
                {
                    ActivationCode = activationCode,
                    ToyId = request.ToyId,
                    IsActivated = false,
                    StatusId = Status.Valid,
                    creaDateTime = DateTime.UtcNow,
                    CreaUserId = GetCurrentUserId()
                };
                await Task.Run(() => toyActivationCodeRepository.InsertAsync(newActivationCode));
                var qrImageUrl = await _qrCodeGenerationService.GenerateQrCodeImageAsync(
                    $"https://codino.com/activate?code={activationCode}",
                    activationCode
                );
                response.Add(new ToyActivationCodeResponse
                {
                    Id = newActivationCode.id,
                    ToyId = toy.id,
                    ToyName = toy.Name,
                    ToyImageUrl = toy.IconImageUrl,
                    ActivationCode = activationCode,
                    QrCodeImageUrl = qrImageUrl,
                    ExpirationDate = request.ExpirationDate,
                    IsActive = true,
                    Code = (int)ResponseCode.Success,
                    Message = localizer.GetString("ActivationCodeGeneratedSuccessfully")
                });
            }

            await Task.Run((() => unitOfWork.SaveChangesAsync()));
        }
        catch (Exception e)
        {
            var errorResponse = new ToyActivationCodeResponse
            {
                Code = (int)ResponseCode.Error,
                Message = e.Message
            };
            response.Add(errorResponse);
        }

        return response;
    }

    /*private async Task<string> GenerateUniqueActivationCodeAsync()
    {
        string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; 
        Random random = new Random();

        while (true)
        {
            char[] code = new char[8];
            for (int i = 0; i < 8; i++)
            {
                code[i] = chars[random.Next(chars.Length)];
            }

            string activationCode = new string(code);

            var existingCode = await Task.Run(() => toyActivationCodeRepository.GetQuery(tac =>
                    tac.ActivationCode == activationCode &&
                    tac.StatusId == Status.Valid)
                .FirstOrDefault());

            if (existingCode == null)
            {
                return activationCode;
            }
        }
    }*/

    public async Task<ToyActivationDetailsResponse> GetToyActivationDetailsAsync(int activationCodeId)
    {
        var response = new ToyActivationDetailsResponse();
        try
        {
            var activationCode = await Task.Run(() => toyActivationCodeRepository.GetById(activationCodeId));
        
            if (activationCode == null || activationCode.StatusId != Status.Valid)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = localizer.GetString("ActivationCodeNotFound");
                return response;
            }
            var toy = await Task.Run(() => toyRepository.GetById(activationCode.ToyId));
        
            if (toy == null || toy.StatusId != Status.Valid)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = localizer.GetString("ToyNotFound");
                return response;
            }
            response.Id = activationCode.id;
            response.ActivationCode = activationCode.ActivationCode;
            response.CreationDate = activationCode.creaDateTime;
            
            response.ToyId = toy.id;
            response.ToyName = toy.Name;
            response.ToyDescription = toy.Description;
            response.ToyImageUrl = toy.IconImageUrl;
            
            response.IsActivated = activationCode.IsActivated;
            response.ActivationDate = activationCode.ActivationDate;
            if (activationCode.IsActivated && activationCode.ActivatedByUserId.HasValue)
            {
                var user = await Task.Run(() => userRepository.GetById(activationCode.ActivatedByUserId.Value));
            
                if (user != null)
                {
                    response.UserInfo = new UserActivationInfoDto
                    {
                        UserId = user.id,
                        FullName = $"{user.Name} {user.Surname}",
                        Email = user.Email,
                        RegistrationDate = user.creaDateTime,
                        Level = user.Level,
                        Xp = user.Xp,
                        CurrentAvatarUrl = user.AvatarImage
                    };
                }
            }
            var avatars = await Task.Run(() => toyAvatarRepository.GetQuery(ta => 
                    ta.ToyId == toy.id && 
                    ta.StatusId == Status.Valid)
                .ToList());
        
            foreach (var avatar in avatars)
            {
                response.Avatars.Add(new ToyAvatarDto
                {
                    Id = avatar.id,
                    Name = avatar.Name,
                    AvatarUrl = avatar.AvatarUrl,
                    RequiredToyLevel = avatar.RequiredToyLevel,
                    RequiredUserXp = avatar.RequiredUserXp
                });
            }
        
            response.Code = (int)ResponseCode.Success;
            response.Message = localizer.GetString("Success");
        }
        catch (Exception e)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = e.Message;
        }

        return response;
    }

    public async Task<IEnumerable<ToyActivationSummaryResponse>> GetToyActivationSummaryAsync(int toyId)
    {
        var response = new List<ToyActivationSummaryResponse>();
        try
        {
            var toy = await Task.Run(() => toyRepository.GetById(toyId));
        
            if (toy == null || toy.StatusId != Status.Valid)
            {
                var errorResponse = new ToyActivationSummaryResponse
                {
                    Code = (int)ResponseCode.Error,
                    Message = localizer.GetString("ToyNotFound")
                };
                response.Add(errorResponse);
                return response;
            }
            var activationCodes = await Task.Run(() => toyActivationCodeRepository.GetQuery(tac => 
                    tac.ToyId == toyId && 
                    tac.StatusId == Status.Valid)
                .ToList());
        
            var now = DateTime.UtcNow;
            var last24Hours = now.AddDays(-1);
            var last7Days = now.AddDays(-7);
            var last30Days = now.AddDays(-30);
            
            var totalCodes = activationCodes.Count;
            var activatedCodes = activationCodes.Count(ac => ac.IsActivated);
            var activationsLast24Hours = activationCodes.Count(ac => 
                ac.IsActivated && 
                ac.ActivationDate.HasValue && 
                ac.ActivationDate.Value >= last24Hours);
        
            var activationsLast7Days = activationCodes.Count(ac => 
                ac.IsActivated && 
                ac.ActivationDate.HasValue && 
                ac.ActivationDate.Value >= last7Days);
        
            var activationsLast30Days = activationCodes.Count(ac => 
                ac.IsActivated && 
                ac.ActivationDate.HasValue && 
                ac.ActivationDate.Value >= last30Days);
        
            var recentActivations = activationCodes
                .Where(ac => ac.IsActivated && ac.ActivationDate.HasValue)
                .OrderByDescending(ac => ac.ActivationDate)
                .Take(10)
                .ToList();
        
            var recentActivationDtos = new List<RecentActivationDto>();
        
            foreach (var activation in recentActivations)
            {
                var user = await Task.Run(() => 
                    userRepository.GetById(activation.ActivatedByUserId ?? 0));
            
                if (user != null)
                {
                    recentActivationDtos.Add(new RecentActivationDto
                    {
                        Id = activation.id,
                        ActivationCode = activation.ActivationCode,
                        UserId = user.id,
                        UserName = $"{user.Name} {user.Surname}",
                        ActivationDate = activation.ActivationDate.Value
                    });
                }
            }
            var activationSummary = new ToyActivationSummaryResponse
            {
                ToyId = toy.id,
                ToyName = toy.Name,
                TotalActivationCodes = totalCodes,
                ActiveCodesCount = totalCodes - activatedCodes,
                ActivatedCodesCount = activatedCodes,
                ExpiredCodesCount = 0, 
                ActivationsLast24Hours = activationsLast24Hours,
                ActivationsLast7Days = activationsLast7Days,
                ActivationsLast30Days = activationsLast30Days,
                RecentActivations = recentActivationDtos,
                Code = (int)ResponseCode.Success,
                Message = localizer.GetString("Success")
            };
        
            response.Add(activationSummary);
        }
        catch (Exception e)
        {
            var errorResponse = new ToyActivationSummaryResponse
            {
                Code = (int)ResponseCode.Error,
                Message = e.Message
            };
            response.Add(errorResponse);
        }

        return response;
    }
}