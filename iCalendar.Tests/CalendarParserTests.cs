using iCalendar.Exceptions;
using NUnit.Framework;

namespace iCalendar.Tests
{
    [TestFixture]
    public class CalendarParserTests
    {
        [Test]
        public void Given_CalendarObject_When_BeginAndEndBlockIsPresent_Then_EmptyCalendarInstanceIsCreated()
        {
            string iCalendarObject = @"BEGIN:VCALENDAR
                                       END:VCALENDAR";

            CalendarParser parser = new CalendarParser(iCalendarObject);
            Calendar calendar = parser.Parse();

            Assert.That(calendar, Is.Not.Null);
            Assert.AreEqual(0, calendar.NumberOfEvents);
        }

        [TestCase("BEGIN:VCALENDAR")]
        [TestCase("END:VCALENDAR")]
        [TestCase("END:VCALENDARBEGIN:VCALENDAR")]
        [ExpectedException(typeof(MalformedFormatException))]
        public void Given_CalendarObject_When_BeginAndEndBlockIsMissing_Then_ExceptionIsThrown(string iCalendarObject)
        {
            CalendarParser parser = new CalendarParser(iCalendarObject);
            parser.Parse();
        }

        [Test]
        [ExpectedException(typeof(MalformedFormatException))]
        public void Given_CalendarObject_When_ObjectContentIsEmpty_Then_ExceptionIsThrown()
        {
            string iCalendarObject = string.Empty;

            CalendarParser parser = new CalendarParser(iCalendarObject);
        }

        [Test]
        public void Given_CalendatObject_When_CalendarNameIsPresent_Then_NameIsParsed()
        {
            string expected = "Name";
            string iCalendarObject = @"BEGIN:VCALENDAR
                                       X-WR-CALNAME:Name
                                       END:VCALENDAR";

            CalendarParser parser = new CalendarParser(iCalendarObject);
            Calendar calendar = parser.Parse();

            StringAssert.AreEqualIgnoringCase(expected, calendar.Name);
        }

        [Test]
        public void Given_CalendatObject_When_CalendarNameIsMissing_Then_NameIsEmpty()
        {
            string expected = string.Empty;
            string iCalendarObject = @"BEGIN:VCALENDAR
                                       END:VCALENDAR";

            CalendarParser parser = new CalendarParser(iCalendarObject);
            Calendar calendar = parser.Parse();

            StringAssert.AreEqualIgnoringCase(expected, calendar.Name);
        }
    }
}
