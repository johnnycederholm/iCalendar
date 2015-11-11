using iCalendar.Exceptions;
using System.Text.RegularExpressions;

namespace iCalendar
{
    public class CalendarParser
    {
        private string CalendarObject { get; set; }

        public CalendarParser(string calendarObject)
        {
            if (string.IsNullOrWhiteSpace(calendarObject))
                throw new MalformedFormatException();

            CalendarObject = calendarObject;
        }

        public Calendar Parse()
        {
            bool hasStartBlock = Regex.IsMatch(CalendarObject, "^BEGIN:VCALENDAR");
            bool hasEndBlock = Regex.IsMatch(CalendarObject, "END:VCALENDAR$");

            if (hasStartBlock && hasEndBlock)
                return new Calendar();

            throw new MalformedFormatException();
        }
    }
}
