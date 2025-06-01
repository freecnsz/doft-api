using doft.Application.DTOs;
using doft.Application.DTOs.Schedule;
using doft.Application.Helper;
using doft.Application.Interfaces.RepositoryInterfaces;
using doft.Application.Interfaces.ServiceInterfaces;
using doft.Domain.Entities;
using doft.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace doft.Application.Services.Task
{
    public class TaskSchedulingService : ITaskSchedulingService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IPlannedTaskRepository _plannedTaskRepository;
        private readonly ILogger<TaskSchedulingService> _logger;

        private readonly TimeSpan WORK_DAY_START = new(9, 0, 0);
        private readonly TimeSpan WORK_DAY_END = new(18, 0, 0);
        private readonly TimeSpan MIN_BREAK_DURATION = TimeSpan.FromMinutes(15);
        private readonly TimeSpan MAX_BREAK_DURATION = TimeSpan.FromMinutes(30);

        public TaskSchedulingService(
            ITaskRepository taskRepository,
            IPlannedTaskRepository plannedTaskRepository,
            ILogger<TaskSchedulingService> logger)
        {
            _taskRepository = taskRepository;
            _plannedTaskRepository = plannedTaskRepository;
            _logger = logger;
        }

        public async Task<(bool Success, string? Message, ScheduledTaskResultDto? Result)> ScheduleSingleTaskAsync(
            DoftTask task,
            Consequence consequence,
            DueDateOption dueDateOption,
            Duration duration,
            Urgency urgency,
            CancellationToken cancellationToken)
        {
            try
            {
                // Get all planned tasks for the user
                var existingPlannedTasks = await _plannedTaskRepository.GetPlannedTasksForUserOnDate(task.OwnerId, DateTime.UtcNow.Date);
                
                // Calculate priority score for the new task
                var newTaskScore = PriorityCalculatorHelper.CalculatePriority(consequence, dueDateOption, duration, urgency);
                var newTaskDuration = GetDurationFromEnum(duration);

                // Update task's priority score and priority
                task.PriorityScore = newTaskScore.Score;
                task.Priority = newTaskScore.Priority;
                await _taskRepository.UpdateAsync(task);

                // Create a list of all tasks to be scheduled
                var tasksToSchedule = new List<(DoftTask Task, int PriorityScore, TimeSpan Duration)>
                {
                    (task, (int)newTaskScore.Score, newTaskDuration)
                };

                // Add existing planned tasks to the list
                foreach (var plannedTask in existingPlannedTasks)
                {
                    var existingTask = await _taskRepository.GetByIdAsync(plannedTask.TaskId);
                    if (existingTask != null)
                    {
                        tasksToSchedule.Add((existingTask, (int)existingTask.PriorityScore, plannedTask.Duration));
                    }
                }

                // Sort tasks by priority score (higher score = higher priority)
                tasksToSchedule = tasksToSchedule.OrderByDescending(t => t.PriorityScore).ToList();

                // Clear existing planned tasks
                foreach (var plannedTask in existingPlannedTasks)
                {
                    await _plannedTaskRepository.DeleteAsync(plannedTask);
                }

                // Schedule all tasks
                var startDate = DateTime.UtcNow.Date;
                var maxAttempts = 7; // Try for a week
                var scheduledTasks = new List<PlannedTask>();
                var failedTasks = new List<int>();

                foreach (var (taskToSchedule, priorityScore, taskDuration) in tasksToSchedule)
                {
                    var slot = await FindAvailableTimeSlot(
                        taskToSchedule.OwnerId,
                        startDate,
                        taskDuration,
                        priorityScore > 80); // Consider high priority if score > 80

                    if (slot == null)
                    {
                        failedTasks.Add(taskToSchedule.Id);
                        continue;
                    }

                    var planned = new PlannedTask
                    {
                        TaskId = taskToSchedule.Id,
                        UserId = taskToSchedule.OwnerId,
                        PlannedDate = slot.Value.Date,
                        StartTime = slot.Value.Start,
                        Duration = taskDuration,
                        DueDate = taskToSchedule.DueDate,
                        IsCompleted = false
                    };

                    await _plannedTaskRepository.AddAsync(planned);
                    scheduledTasks.Add(planned);
                }

                // Find the result for the new task
                var newTaskResult = scheduledTasks.FirstOrDefault(p => p.TaskId == task.Id);
                if (newTaskResult == null)
                {
                    _logger.LogWarning("Failed to schedule new task {TaskId}", task.Id);
                    return (false, "Could not find a suitable time slot for the task.", null);
                }

                var result = new ScheduledTaskResultDto
                {
                    TaskId = task.Id,
                    PlannedDate = newTaskResult.PlannedDate,
                    StartTime = newTaskResult.StartTime,
                    Duration = newTaskResult.Duration,
                    DueDate = newTaskResult.DueDate
                };

                if (failedTasks.Any())
                {
                    _logger.LogWarning("Some tasks could not be scheduled: {TaskIds}", string.Join(", ", failedTasks));
                    return (true, "Task scheduled, but some existing tasks could not be rescheduled.", result);
                }

                _logger.LogInformation("All tasks rescheduled successfully");
                return (true, "Task scheduled successfully.", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling task {TaskId}", task.Id);
                return (false, "An error occurred while scheduling the task.", null);
            }
        }

        public async Task<SchedulingResultDto> RescheduleTask(int plannedTaskId, DateTime newDate, TimeSpan newStartTime, CancellationToken cancellationToken)
        {
            var result = new SchedulingResultDto();

            try
            {
                var planned = await _plannedTaskRepository.GetByIdAsync(plannedTaskId);
                if (planned is null)
                {
                    result.Success = false;
                    result.Message = "Planned task not found.";
                    result.Errors.Add(result.Message);
                    return result;
                }

                // Convert newDate to UTC
                var utcNewDate = DateTime.SpecifyKind(newDate.Date, DateTimeKind.Utc);

                // Check if the new time slot is available
                var existingPlans = await _plannedTaskRepository.GetPlannedTasksForUserOnDate(planned.UserId, utcNewDate);
                var isSlotAvailable = !existingPlans.Any(p => 
                    p.Id != plannedTaskId && 
                    p.StartTime <= newStartTime && 
                    p.StartTime + p.Duration > newStartTime);

                if (!isSlotAvailable)
                {
                    result.Success = false;
                    result.Message = "The selected time slot is not available.";
                    result.Errors.Add(result.Message);
                    return result;
                }

                planned.PlannedDate = utcNewDate;
                planned.StartTime = newStartTime;

                await _plannedTaskRepository.UpdateAsync(planned);

                result.Success = true;
                result.Message = "Task rescheduled successfully.";
                result.ScheduledTaskIds.Add(planned.TaskId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rescheduling task {PlannedTaskId}", plannedTaskId);
                result.Success = false;
                result.Message = "An error occurred while rescheduling the task.";
                result.Errors.Add(result.Message);
                return result;
            }
        }

        public async Task<SchedulingResultDto> ScheduleTasksForUser(string userId, CancellationToken cancellationToken)
        {
            var result = new SchedulingResultDto();

            try
            {
                var tasks = await _taskRepository.GetAllTasksForUserAsync(userId);

                var unscheduled = tasks
                    .Where(t => t.Status == DoftTaskStatus.Pending)
                    .Where(t => !_plannedTaskRepository.Exists(t.Id).Result)
                    .OrderByDescending(t => t.Priority) // Schedule high priority tasks first
                    .ToList();

                foreach (var task in unscheduled)
                {
                    // Dışarıdan priority bilgileri alınmadığı için planlanamaz
                    result.UnscheduledTaskIds.Add(task.Id);
                    result.Warnings.Add($"Task {task.Id} skipped — missing priority input.");
                }

                result.Success = true;
                result.Message = "Task scheduling completed.";
    
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling tasks for user {UserId}", userId);
                result.Success = false;
                result.Message = "An error occurred while scheduling tasks.";
                result.Errors.Add(result.Message);
                return result;
            }
        }

        private TimeSpan GetDurationFromEnum(Duration duration) => duration switch
        {
            Duration.LessThan15Min => TimeSpan.FromMinutes(10),
            Duration.Between15And60Min => TimeSpan.FromMinutes(45),
            Duration.MoreThan1Hour => TimeSpan.FromMinutes(90),
            _ => TimeSpan.FromMinutes(30)
        };

        private DoftTaskPriority GetPriorityFromScore(int score) => score switch
        {
            var s when s >= 80 => DoftTaskPriority.High,
            var s when s >= 60 => DoftTaskPriority.Medium,
            _ => DoftTaskPriority.Low
        };

        private async Task<(DateTime Date, TimeSpan Start)?> FindAvailableTimeSlot(
            string userId,
            DateTime date,
            TimeSpan duration,
            bool isHighPriority)
        {
            // Ensure date is in UTC
            var utcDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
            
            var existingPlans = await _plannedTaskRepository.GetPlannedTasksForUserOnDate(userId, utcDate);
            var orderedPlans = existingPlans.OrderBy(p => p.StartTime).ToList();

            var cursor = WORK_DAY_START;

            foreach (var plan in orderedPlans)
            {
                var availableDuration = plan.StartTime - cursor;
                
                // If this is a high priority task, try to fit it in smaller gaps
                if (isHighPriority && availableDuration >= duration)
                {
                    return (utcDate, cursor);
                }
                
                // For normal priority tasks, ensure there's enough break time
                if (availableDuration >= duration + MIN_BREAK_DURATION)
                {
                    // Add a random break duration between MIN and MAX
                    var breakDuration = TimeSpan.FromMinutes(
                        new Random().Next(
                            (int)MIN_BREAK_DURATION.TotalMinutes,
                            (int)MAX_BREAK_DURATION.TotalMinutes
                        )
                    );
                    
                    return (utcDate, cursor + breakDuration);
                }

                cursor = plan.StartTime + plan.Duration;
            }

            // Check if there's enough time at the end of the day
            var remainingTime = WORK_DAY_END - cursor;
            if (remainingTime >= duration + (isHighPriority ? TimeSpan.Zero : MIN_BREAK_DURATION))
            {
                return (utcDate, cursor);
            }

            return null;
        }
    }
}
