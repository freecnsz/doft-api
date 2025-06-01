using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Domain.Enums;

namespace doft.Application.DTOs.Schedule
{
    public class CalculationResponseDto
    {
        public double Score { get; set; }
        public DoftTaskPriority Priority { get; set; }
    }
}