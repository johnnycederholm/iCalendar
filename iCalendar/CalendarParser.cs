using iCalendar.Exceptions;
using System.Text.RegularExpressions;

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
    }
}
