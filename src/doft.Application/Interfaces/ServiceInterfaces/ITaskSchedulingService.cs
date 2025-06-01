using doft.Application.DTOs;
using doft.Application.DTOs.Schedule;
using doft.Domain.Entities;
using doft.Domain.Enums;

namespace doft.Application.Interfaces.ServiceInterfaces
{
    public interface ITaskSchedulingService
    {
        Task<SchedulingResultDto> ScheduleTasksForUser(string userId, CancellationToken cancellationToken);

        Task<SchedulingResultDto> RescheduleTask(int plannedTaskId, DateTime newDate, TimeSpan newStartTime, CancellationToken cancellationToken);

        Task<(bool Success, string? Message, ScheduledTaskResultDto? Result)> ScheduleSingleTaskAsync(
            DoftTask task,
            Consequence consequence,
            DueDateOption dueDateOption,
            Duration duration,
            Urgency urgency,
            CancellationToken cancellationToken);
    }
}
