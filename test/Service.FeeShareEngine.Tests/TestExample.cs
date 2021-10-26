using System;
using NUnit.Framework;
using Service.FeeShareEngine.Domain.Models.Models;
using Service.FeeShareEngine.Writer.Services;

namespace Service.FeeShareEngine.Tests
{
    public class TestExample
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void DayTests()
        {
            var (start, end) = PeriodHelper.GetPeriod(new DateTime(2021, 10, 11,12,22,10), PeriodTypes.Day); 
            Assert.AreEqual(new DateTime(2021, 10,10), start);
            Assert.AreEqual(new DateTime(2021, 10,10,23,59,59), end);
            
            (start, end) = PeriodHelper.GetPeriod(new DateTime(2021, 10, 11,23,59,59), PeriodTypes.Day); 
            Assert.AreEqual(new DateTime(2021, 10,10), start);
            Assert.AreEqual(new DateTime(2021, 10,10,23,59,59), end);
            
            (start, end) = PeriodHelper.GetPeriod(new DateTime(2021, 10, 11), PeriodTypes.Day); 
            Assert.AreEqual(new DateTime(2021, 10,10), start);
            Assert.AreEqual(new DateTime(2021, 10,10,23,59,59), end);
        }
        
        [Test]
        public void WeekTests()
        {
            var (start, end) = PeriodHelper.GetPeriod(new DateTime(2021, 10, 25,12,22,10), PeriodTypes.Week); 
            Assert.AreEqual(new DateTime(2021, 10,18), start);
            Assert.AreEqual(new DateTime(2021, 10,24,23,59,59), end);
            
            (start, end) = PeriodHelper.GetPeriod(new DateTime(2021, 10, 31,23,59,59), PeriodTypes.Week); 
            Assert.AreEqual(new DateTime(2021, 10,18), start);
            Assert.AreEqual(new DateTime(2021, 10,24,23,59,59), end);
            
            (start, end) = PeriodHelper.GetPeriod(new DateTime(2021, 10, 26), PeriodTypes.Week); 
            Assert.AreEqual(new DateTime(2021, 10,18), start);
            Assert.AreEqual(new DateTime(2021, 10,24,23,59,59), end);
        }
        [Test]
        public void MonthsTests()
        {
            var (start, end) = PeriodHelper.GetPeriod(new DateTime(2021, 10, 25,12,22,10), PeriodTypes.Month); 
            Assert.AreEqual(new DateTime(2021, 9,1), start);
            Assert.AreEqual(new DateTime(2021, 9,30,23,59,59), end);
            
            (start, end) = PeriodHelper.GetPeriod(new DateTime(2021, 10, 31,23,59,59), PeriodTypes.Month); 
            Assert.AreEqual(new DateTime(2021, 9,1), start);
            Assert.AreEqual(new DateTime(2021, 9,30,23,59,59), end);
            
            (start, end) = PeriodHelper.GetPeriod(new DateTime(2021, 3, 26), PeriodTypes.Month); 
            Assert.AreEqual(new DateTime(2021, 2,1), start);
            Assert.AreEqual(new DateTime(2021, 2,28,23,59,59), end);
        }
    }
}
