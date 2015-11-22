using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using iCalendar.Exceptions;

namespace iCalendar
{
    public class CalendarParser
    {
        private string CalendarObject { get; set; }
        private Calendar calendar;

        public CalendarParser(string calendarObject)
        {
            if (string.IsNullOrWhiteSpace(calendarObject))
                throw new MalformedFormatException();

            CalendarObject = calendarObject;
        }

        public Calendar Parse()
        {
            ContainsCalendarBlock();

            calendar = new Calendar();

            FindCalendarName();
            FindVersionNumber();
            FindEvents();

            return calendar;
        }

        /// <summary>
        /// Ensures the existens of required VCalendar blocks.
        /// </summary>
        private void ContainsCalendarBlock()
        {
            bool hasStartBlock = Regex.IsMatch(CalendarObject, "^BEGIN:VCALENDAR");
            bool hasEndBlock = Regex.IsMatch(CalendarObject, "END:VCALENDAR$");

            if (!hasStartBlock || !hasEndBlock)
                throw new MalformedFormatException();
        }

        /// <summary>
        /// Populate calendar name.
        /// </summary>
        private void FindCalendarName()
        {
            Match match = Regex.Match(CalendarObject, "X-WR-CALNAME:([A-Öa-ö]*)");

            if (match.Success)
            {
                calendar.Name = match.Groups[1].Value;
            }
        }

        /// <summary>
        /// Find and populate version number.
        /// </summary>
        private void FindVersionNumber()
        {
            Match match = Regex.Match(CalendarObject, "VERSION:\\s*([12].0)");

            if (match.Success)
            {
                switch (match.Groups[1].Value)
                {
                    case "1.0":
                        calendar.Version = iCalendarVersion.V1;
                        break;
                    case "2.0":
                        calendar.Version = iCalendarVersion.V2;
                        break;
                    default:
                        calendar.Version = iCalendarVersion.Unknown;
                        break;
                }
            }
        }

        /// <summary>
        /// Find and populate events.
        /// </summary>
        private void FindEvents()
        {
            MatchCollection matches = Regex.Matches(CalendarObject, "BEGIN:VEVENT([\\s\\S]*?)END:VEVENT", RegexOptions.Multiline);
            bool noEvents = matches.Count == 0;

            if (noEvents)
            {
                return;
            }
            
            int numberOfEvents = 0;
            List<CalendarEvent> events = new List<CalendarEvent>();
                
            foreach (Match match in matches)
            {
                numberOfEvents++;

                CalendarEvent calendarEvent = ParseEvent(match.Groups[0].Value);
                events.Add(calendarEvent);                
            }

            calendar.NumberOfEvents = numberOfEvents;
            calendar.Events = events;
        }

        /// <summary>
        /// Parse VEVENT data and construct an CalendarEvent from it.
        /// </summary>
        private CalendarEvent ParseEvent(string eventData)
        {
            string summary = Regex.Match(eventData, "(?<=SUMMARY:)[A-Öa-ö ]*").Value;
            string description = Regex.Match(eventData, "(?<=DESCRIPTION:)[A-Öa-ö ]*").Value;
            string start = Regex.Match(eventData, "(?<=DTSTART:)[0-9TZ]*").Value;
            string end = Regex.Match(eventData, "(?<=DTEND:)[0-9TZ]*").Value;
            CalendarEvent calendarEvent = new CalendarEvent();
            calendarEvent.Description = description;
            calendarEvent.Summary = summary;

            DateTime startTime;

            if (DateTime.TryParseExact(start, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime))
            {
                calendarEvent.Start = startTime;
            }

            DateTime endTime;

            if (DateTime.TryParseExact(end, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture, DateTimeStyles.None, out endTime))
            {
                calendarEvent.End = endTime;
            }

            return calendarEvent;
        }
    }
}
