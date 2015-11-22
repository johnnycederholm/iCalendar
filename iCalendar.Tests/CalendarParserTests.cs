using iCalendar.Exceptions;
using NUnit.Framework;
using System;
using System.Linq;

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

        [Test]
        public void Given_CalenderObject_When_VersionIsPresent_Then_VersionNumberIsParsed()
        {
            iCalendarVersion expected = iCalendarVersion.V2;
            string iCalendarObject = @"BEGIN:VCALENDAR
                                       VERSION:2.0
                                       END:VCALENDAR";

            CalendarParser parser = new CalendarParser(iCalendarObject);
            Calendar calendar = parser.Parse();

            Assert.AreEqual(expected, calendar.Version);
        }

        [Test]
        public void Given_CalendarObject_When_VersionIsMissing_Then_VersionIsSetToUnknown()
        {
            iCalendarVersion expected = iCalendarVersion.Unknown;
            string iCalendarObject = @"BEGIN:VCALENDAR
                                       END:VCALENDAR";

            CalendarParser parser = new CalendarParser(iCalendarObject);
            Calendar calendar = parser.Parse();

            Assert.AreEqual(expected, calendar.Version);
        }

        [Test]
        public void Given_CalendarObject_When_OneEventIsPresent_Then_NumberOfEventsIsOne()
        {
            int expected = 1;
            string iCalendarObject = @"BEGIN:VCALENDAR
                                       VERSION:2.0
                                       PRODID:-//hacksw/handcal//NONSGML v1.0//EN
                                       BEGIN:VEVENT
                                       UID:uid1@example.com
                                       DTSTAMP:19970714T170000Z
                                       DTSTART:19970714T170000Z
                                       DTEND:19970715T035959Z
                                       SUMMARY:Bastille Day Party
                                       END:VEVENT
                                       END:VCALENDAR";

            CalendarParser parser = new CalendarParser(iCalendarObject);
            Calendar calendar = parser.Parse();

            Assert.AreEqual(expected, calendar.NumberOfEvents);
        }

        [Test]
        public void Given_CalendarObject_When_TwoEventsIsPresent_Then_NumberOfEventsIsTwo()
        {
            int expected = 2;
            string iCalendarObject = @"BEGIN:VCALENDAR
                                       VERSION:2.0
                                       PRODID:-//hacksw/handcal//NONSGML v1.0//EN
                                       BEGIN:VEVENT
                                       UID:uid1@example.com
                                       DTSTAMP:19970714T170000Z
                                       DTSTART:19970714T170000Z
                                       DTEND:19970715T035959Z
                                       SUMMARY:Bastille Day Party
                                       END:VEVENT
                                       BEGIN:VEVENT
                                       UID:uid1@example.com
                                       DTSTAMP:19970714T170000Z
                                       DTSTART:19970714T170000Z
                                       DTEND:19970715T035959Z
                                       SUMMARY:Bastille Day Party
                                       END:VEVENT
                                       END:VCALENDAR";

            CalendarParser parser = new CalendarParser(iCalendarObject);
            Calendar calendar = parser.Parse();

            Assert.AreEqual(expected, calendar.NumberOfEvents);
        }

        [Test]
        public void Given_CalendarObject_When_EventIsPresent_Then_EventDataIsExtracted()
        {
            int expectedNumberOfEvents = 1;
            CalendarEvent expectedEvent = new CalendarEvent
            {
                Description = string.Empty,
                Start = new DateTime(2015, 12, 01, 7, 0, 0),
                End = new DateTime(2015, 12, 01, 8, 0, 0)
            };

            string iCalendarObject = @"BEGIN:VCALENDAR
                                       PRODID:-//Google Inc//Google Calendar 70.9054//EN
                                       VERSION:2.0
                                       CALSCALE:GREGORIAN
                                       METHOD:PUBLISH
                                       X-WR-CALNAME:Brytarstryrning
                                       X-WR-TIMEZONE:Europe/Stockholm
                                       X-WR-CALDESC:Kalender för att testa brytarstyrning.
                                       BEGIN:VEVENT
                                       DTSTART:20151201T060000Z
                                       DTEND:20151201T070000Z
                                       DTSTAMP:20151122T154644Z
                                       UID:kqd3chl4rvstg7g8g4k3uh22t0@google.com
                                       CREATED:20151111T192501Z
                                       DESCRIPTION:
                                       LAST-MODIFIED:20151111T192501Z
                                       LOCATION:
                                       SEQUENCE:0
                                       STATUS:CONFIRMED
                                       SUMMARY:Test
                                       TRANSP:OPAQUE
                                       END:VEVENT
                                       END:VCALENDAR";

            CalendarParser parser = new CalendarParser(iCalendarObject);
            Calendar calendar = parser.Parse();

            Assert.AreEqual(expectedNumberOfEvents, calendar.NumberOfEvents);
            Assert.AreEqual(expectedEvent, calendar.Events.First());
        }

        [Test]
        public void Given_CalendarObject_When_EventIsOccurringRightNow_Then_OngoingIsTrue()
        {
            string iCalendarObject = $@"BEGIN:VCALENDAR
                                       PRODID:-//Google Inc//Google Calendar 70.9054//EN
                                       VERSION:2.0
                                       CALSCALE:GREGORIAN
                                       METHOD:PUBLISH
                                       X-WR-CALNAME:Brytarstryrning
                                       X-WR-TIMEZONE:Europe/Stockholm
                                       X-WR-CALDESC:Kalender för att testa brytarstyrning.
                                       BEGIN:VEVENT
                                       DTSTART:{DateTime.Now.AddHours(-1).ToString("yyyyMMddTHHmmssZ")}
                                       DTEND:{DateTime.Now.AddHours(2).ToString("yyyyMMddTHHmmssZ")}
                                       DTSTAMP:20151122T154644Z
                                       UID:kqd3chl4rvstg7g8g4k3uh22t0@google.com
                                       CREATED:20151111T192501Z
                                       DESCRIPTION:
                                       LAST-MODIFIED:20151111T192501Z
                                       LOCATION:
                                       SEQUENCE:0
                                       STATUS:CONFIRMED
                                       SUMMARY:Test
                                       TRANSP:OPAQUE
                                       END:VEVENT
                                       END:VCALENDAR";

            CalendarParser parser = new CalendarParser(iCalendarObject);
            Calendar calendar = parser.Parse();

            Assert.IsTrue(calendar.Events.First().Ongoing);
        }
    }
}
