using System.Collections.Generic;

namespace iCalendar
{
    public class Calendar
    {
        private List<CalendarEvent> events;

        public string Name { get; set; }
        public int NumberOfEvents { get; set; }
        public iCalendarVersion Version { get; set; }

        public List<CalendarEvent> Events
        {
            get { return events; }

            internal set { events = value; }
        }

        public Calendar()
        {
            Name = string.Empty;
            events = new List<CalendarEvent>();
        }


    }
}
