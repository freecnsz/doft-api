using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.Interfaces.RepositoryInterfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.Task
{
    public class DeleteTaskCommand : IRequest<ApiResponse<bool>>
    {
        public int Id { get; set; }

        public DeleteTaskCommand(int id)
        {
            Id = id;
        }
    }

    
    public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, ApiResponse<bool>>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ILogger<DeleteTaskCommandHandler> _logger;

        public DeleteTaskCommandHandler(ITaskRepository taskRepository, ILogger<DeleteTaskCommandHandler> logger)
        {
            _logger = logger;
            _taskRepository = taskRepository;
        }
        public async Task<ApiResponse<bool>> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(request.Id);

            if (task == null) 
            {
                _logger.LogWarning("Task with ID {TaskId} not found", request.Id);
                return ApiResponse<bool>.Error(404, "Task not found", false);
            }

            task.Detail.IsDeleted = true; // Mark the task as deleted
            await _taskRepository.UpdateAsync(task);

            _logger.LogInformation("Task with ID {TaskId} deleted successfully", request.Id);
            return ApiResponse<bool>.Success(200, "Task deleted successfully", true);
        }
    }
}