using System.Collections.Generic;

namespace src.doft.Application.DTOs.Task
{
    public class GetTaskStatisticsResponseDto
    {
        public int CompletedTasks { get; set; }
        public int RemainingTasks { get; set; }
    }
} 