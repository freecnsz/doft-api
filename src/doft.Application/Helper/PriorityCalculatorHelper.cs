using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.DTOs.Schedule;
using doft.Domain.Enums;

namespace doft.Application.Helper
{
    public static class PriorityCalculatorHelper
    {
        public static CalculationResponseDto CalculatePriority(
            Consequence consequence,
            DueDateOption dueDate,
            Duration duration,
            Urgency urgency)
        {
            double score = CalculateNormalizedScore(consequence, dueDate, duration, urgency);
            DoftTaskPriority priority = GetPriorityLevel(consequence, dueDate, duration, urgency);

            return new CalculationResponseDto
            {
                Score = score,
                Priority = priority
            };
        }
        private static double CalculateNormalizedScore(
            Consequence consequence,
            DueDateOption dueDate,
            Duration duration,
            Urgency urgency)
        {
            double consequenceScore = (int)consequence / 2.0 * 0.30;
            double dueDateScore = (int)dueDate / 5.0 * 0.30;
            double durationScore = ((int)duration - 1) / 2.0 * 0.10;
            double urgencyScore = (int)urgency / 2.0 * 0.30;

            double totalScore = consequenceScore + dueDateScore + durationScore + urgencyScore;

            return totalScore; // 0.0 to 1.0
        }

        private static DoftTaskPriority GetPriorityLevel(
            Consequence consequence,
            DueDateOption dueDate,
            Duration duration,
            Urgency urgency)
        {
            double score = CalculateNormalizedScore(consequence, dueDate, duration, urgency);

            if (score >= 0.7)
                return DoftTaskPriority.High;
            else if (score >= 0.4)
                return DoftTaskPriority.Medium;
            else
                return DoftTaskPriority.Low;
        }
    }
}
