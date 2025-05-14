using System.Security.Claims;
using Codino_UserCredential.Business.Concrete.Interfaces;
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
    
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IServiceProvider serviceProvider;

    public ContentBusiness(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        localizer = serviceProvider.GetService<IStringLocalizer<ContentBusiness>>();
        worldMapRepository = serviceProvider.GetService<IWorldMapRepository>();
        biomeRepository = serviceProvider.GetService<IBiomeRepository>();
        toyRepository = serviceProvider.GetService<IToyRepository>();
        taskRepository = serviceProvider.GetService<ITaskRepository>();
        taskSubmissionRepository = serviceProvider.GetService<ITaskSubmissionRepository>();
        unitOfWork = serviceProvider.GetService<IUnitOfWork<CodinoDbContext>>();
        httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
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
            
            // Gönderimi kaydet
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
    
    // İçerik yönetimi için CRUD metodları
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
                CreaUserId = 1 // Aktif kullanıcı ID'si alınmalı
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
}