using System;
using System.Linq;
using DDay.iCal;
using MbUnit.Framework;
using Wallboard.Tasks;

namespace Wallboard.Tests.Tasks
{
    [TestFixture]
    public class CalendarTasksTest
    {
        private CalendarTasks _tasks;

        [SetUp]
        public void Setup()
        {
            _tasks = new CalendarTasks();
        }

        [Test]
        public void LoadOccurences_With_End_Time()
        {
            //Act
            var events = _tasks.LoadEvents(GenerateCalendars(), DateTime.Parse("11/14/2011"), DateTime.Parse("11/16/2011"));

            //Assert
            Assert.AreEqual(1, events.Count());
        }

        [Test]
        public void LoadOccurences_Without_End_Time()
        {
            //Act
            var events = _tasks.LoadEvents(GenerateCalendars(), DateTime.Parse("11/14/2011"), null);

            //Assert
            Assert.AreEqual(2, events.Count());
        }

        [Test]
        public void LoadOccurences_Orders_By_Start()
        {
            //Act
            var events = _tasks.LoadEvents(GenerateCalendars(), DateTime.Parse("11/14/2011"), null);

            //Assert
            var first = events.ElementAt(0);
            var second = events.ElementAt(1);
            Assert.LessThan(first.Start.Date, second.Start.Date);
        }

        private IICalendarCollection GenerateCalendars()
        {
            var calendar = new iCalendar();

            var evt = calendar.Create<Event>();
            evt.Start = new iCalDateTime(DateTime.Parse("11/14/2011")).AddDays(5);
            evt.End = evt.Start.AddDays(1);
            evt.IsAllDay = true;

            var evt2 = calendar.Create<Event>();
            evt2.Start = new iCalDateTime(DateTime.Parse("11/14/2011")).AddHours(8);
            evt2.End = evt2.Start.AddHours(18);          
            evt2.Description = "Holiday Party!";
            evt2.Location = "Some office";

            return new iCalendarCollection {calendar};
        }
    }
}
