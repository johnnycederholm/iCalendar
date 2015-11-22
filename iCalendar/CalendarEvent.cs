using System;

namespace iCalendar
{
    /// <summary>
    /// Representation of an calendar event.
    /// </summary>
    public class CalendarEvent
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }

        public bool Ongoing
        {
            get
            {
                DateTime now = DateTime.Now;

                return Start <= now && End >= now;
            }
        }

        public override bool Equals(object obj)
        {
            CalendarEvent calendarEvent = obj as CalendarEvent;

            if (calendarEvent == null)
            {
                throw new ArgumentException(nameof(obj));
            }

            return Description.Equals(calendarEvent.Description) &&
                   Start.Equals(calendarEvent.Start) &&
                   End.Equals(calendarEvent.End);
        }

        public bool Equals(CalendarEvent calendarEvent)
        {
            return Equals(calendarEvent);
        }
    }
}
