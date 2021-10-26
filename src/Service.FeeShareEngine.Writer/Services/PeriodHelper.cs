using System;
using Service.FeeShareEngine.Domain.Models.Models;

namespace Service.FeeShareEngine.Writer.Services
{
    public static class PeriodHelper
    {
        public static (DateTime periodStart, DateTime periodEnd) GetPeriod(DateTime now, PeriodTypes types)
        {
            switch (types)
            {
                case PeriodTypes.Day:
                    return GetDayPeriod(now);
                case PeriodTypes.Week:
                    return GetWeekPeriod(now);
                case PeriodTypes.Month:
                    return GetMonthPeriod(now);
                default:
                    throw new ArgumentOutOfRangeException(nameof(types), types, null);
            }
        }

        private static (DateTime periodStart, DateTime periodEnd) GetDayPeriod(DateTime now)
        {
            var periodStart = now.AddDays(-1).Date;
            var periodEnd = periodStart.AddDays(1).AddSeconds(-1);
            return (periodStart, periodEnd);
        }

        private static (DateTime periodStart, DateTime periodEnd) GetWeekPeriod(DateTime now)
        {           
            var diff = now.DayOfWeek - DayOfWeek.Monday;
            if (diff < 0)
                diff += 7;
            diff += 7;
            var periodStart = now.AddDays(-diff).Date;
            var periodEnd = periodStart.AddDays(7).AddSeconds(-1);
            return (periodStart, periodEnd);
        }

        private static (DateTime periodStart, DateTime periodEnd) GetMonthPeriod(DateTime now)
        {            
            var month = new DateTime(now.Year, now.Month, 1);       
            var periodStart = month.AddMonths(-1);
            var periodEnd = month.AddSeconds(-1);
            return (periodStart, periodEnd);
        }
    }
}