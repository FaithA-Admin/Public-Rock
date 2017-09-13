using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.OData;
using DDay.iCal;
using Rock;
using Rock.Rest;
using Rock.Rest.Filters;
using org.willowcreek.Model;
using Rock.Model;

namespace org.willowcreek.Rest
{
    /// <summary>
    /// WillowEventItemOccurrences REST API
    /// </summary>
    public partial class WillowEventItemOccurrencesController : ApiController<WillowEventItemOccurrence>
    {
        public WillowEventItemOccurrencesController() : base(new WillowEventItemOccurrenceService(new WillowContext())) { }

        /// <summary>
        /// Gets a list of Willow calendar events.
        /// </summary>
        ///<param name="year">The calendar year.</param>
        /// <returns>
        /// A list of WillowCalendarEvent items.
        /// </returns>
        [Authenticate, Secured]
        [EnableQuery]
        [HttpGet]
        [System.Web.Http.Route("api/WillowCalendarEvents/{year}")]
        public List<WillowCalendarEvents> GetCalendarEvents(int year)
        {
            // return a 400 if year is invalid.
            if (!IsValidDate(year, 1, 1))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var willowContext = this.Service.Context as WillowContext;
            var willowEventItemMinistryService = new WillowEventItemMinistryService(willowContext);
            var willowEventItemOccurrenceService = new WillowEventItemOccurrenceService(willowContext);
            List<WillowCalendarEvents> willowEventsList = new List<WillowCalendarEvents>();
            var eventOccurrences = willowEventItemOccurrenceService.Queryable().ToList<WillowEventItemOccurrence>();
            var eventMinistries = willowEventItemMinistryService.Queryable().ToList<WillowEventItemMinistry>();

            try
            {

                foreach (var eventOccurrence in eventOccurrences)
                {
                    if (eventOccurrence.EventiCalendarContent == null)
                    {
                        throw new Exception("Calendar content not found.");
                    }

                    Event calendarEvent = ScheduleICalHelper.GetCalenderEvent(eventOccurrence.EventiCalendarContent);

                    if (calendarEvent.DTStart != null)
                    {
                        DateTime startDate = new DateTime(year, 1, 1);
                        DateTime endDate = new DateTime(year + 1, 1, 1).AddTicks(-1);
                        IList<Occurrence> dates = ScheduleICalHelper.GetOccurrences(calendarEvent, startDate, endDate);

                        foreach (var date in dates)
                        {
                            WillowCalendarEvents willowEvent = new WillowCalendarEvents();
                            willowEvent.EventOccurrenceId = eventOccurrence.Id;
                            willowEvent.EventItemId = eventOccurrence.EventItemId;
                            willowEvent.EventName = eventOccurrence.Name;
                            willowEvent.Summary = eventOccurrence.Summary;
                            willowEvent.Description = eventOccurrence.Description;
                            willowEvent.DetailsURL = eventOccurrence.DetailsURL;
                            willowEvent.CampusId = eventOccurrence.CampusId;
                            willowEvent.CampusName = eventOccurrence.CampusName;
                            willowEvent.Location = eventOccurrence.Location;
                            willowEvent.Ministries = eventMinistries.Where(mn => mn.EventItemId == eventOccurrence.EventItemId).Select(mn => mn.Ministry).ToList<string>();
                            willowEvent.ContactName = eventOccurrence.ContactName;
                            willowEvent.ContactPhone = eventOccurrence.ContactPhone;
                            willowEvent.ContactEmail = eventOccurrence.ContactEmail;
                            willowEvent.Photo = eventOccurrence.Photo;
                            willowEvent.RegistrationLinkURL = eventOccurrence.RegistrationLinkURL;
                            willowEvent.EventiCalendarContent = eventOccurrence.EventiCalendarContent;
                            willowEvent.ScheduleId = eventOccurrence.ScheduleId;
                            willowEvent.EventStartDateTime = DateTime.Parse(date.Period.StartTime.ToString());
                            willowEvent.EventEndDateTime = DateTime.Parse(date.Period.EndTime.ToString());
                            willowEvent.RegistrationId = eventOccurrence.RegistrationId;
                            willowEvent.RegistrationStartDateTime = eventOccurrence.RegistrationStartDateTime;
                            willowEvent.RegistrationEndDateTime = eventOccurrence.RegistrationEndDateTime;
                            //Include event to the list.
                            willowEventsList.Add(willowEvent);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogService.LogException(ex, System.Web.HttpContext.Current);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            return willowEventsList;
        }

        private bool IsValidDate(int year, int month, int day)
        {
            if (year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year)
                return false;

            if (month < 1 || month > 12)
                return false;

            return day > 0 && day <= DateTime.DaysInMonth(year, month);
        }
    }

    public class WillowCalendarEvents
    {
        /// <summary>
        /// Gets or sets the event occurence identifier.
        /// </summary>
        /// <value>
        /// The event occurrence identifier.
        /// </value>
        public int EventOccurrenceId { get; set; }

        /// <summary>
        /// Gets or sets the event item identifier.
        /// </summary>
        /// <value>
        /// The event item identifier.
        /// </value>
        public int EventItemId { get; set; }

        /// <summary>
        /// Gets or sets the event name.
        /// </summary>
        /// <value>
        /// The event name.
        /// </value>
        public string EventName { get; set; }

        /// <summary>
        /// Gets or sets the summary.
        /// </summary>
        /// <value>
        /// The summary.
        /// </value>
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the details URL.
        /// </summary>
        /// <value>
        /// The details URL.
        /// </value>
        public string DetailsURL { get; set; }

        /// <summary>
        /// Gets or sets the campus identifier.
        /// </summary>
        /// <value>
        /// The campus identifier.
        /// </value>
        public int? CampusId { get; set; }

        private string _campusName;
        /// <summary>
        /// Gets or sets the campus.
        /// </summary>
        /// <value>
        /// The campus.
        /// </value>
        public string CampusName
        {
            get
            {
                if (this.CampusId.HasValue == false)
                {
                    return "All";
                }
                else
                {
                    return this._campusName;
                }
            }
            set
            {
                this._campusName = value;
            }
        }

        /// <summary>
        /// Gets or sets the Location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the ministries.
        /// </summary>
        /// <value>
        /// The ministries.
        /// </value>
        public List<String> Ministries { get; set; }

        /// <summary>
        /// Gets or sets the contact name.
        /// </summary>
        /// <value>
        /// The contact name.
        /// </value>
        public string ContactName { get; set; }

        /// <summary>
        /// Gets or sets the contact phone.
        /// </summary>
        /// <value>
        /// The contact phone.
        /// </value>
        public string ContactPhone { get; set; }

        /// <summary>
        /// Gets or sets the contact email.
        /// </summary>
        /// <value>
        /// The contact email.
        /// </value>
        public string ContactEmail { get; set; }

        /// <summary>
        /// Gets or sets the photo.
        /// </summary>
        /// <value>
        /// The photo.
        /// </value>
        public string Photo { get; set; }

        /// <summary>
        /// Gets or sets the registration link URL.
        /// </summary>
        /// <value>
        /// The registration link URL.
        /// </value>
        public string RegistrationLinkURL { get; set; }

        /// <summary>
        /// Gets or sets the schedule identifier.
        /// </summary>
        /// <value>
        /// The schedule identifier.
        /// </value>
        public int ScheduleId { get; set; }

        /// <summary>
        /// Gets or sets the event iCalendar content.
        /// </summary>
        /// <value>
        /// The event iCalendar content.
        /// </value>
        public string EventiCalendarContent { get; set; }

        /// <summary>
        /// Gets or sets the event start date time.
        /// </summary>
        /// <value>
        /// THe event start date time.
        /// </value>
        public DateTime? EventStartDateTime { get; set; }

        /// <summary>
        /// Gets or sets the event end date time.
        /// </summary>
        /// <value>
        /// The event end date time.
        /// </value>
        public DateTime? EventEndDateTime { get; set; }

        /// <summary>
        /// Gets or sets the registration identifier.
        /// </summary>
        /// <value>
        /// The registration identifier.
        /// </value>
        public int? RegistrationId { get; set; }

        /// <summary>
        /// Gets or sets the registration start date time.
        /// </summary>
        /// <value>
        /// The registration start date time.
        /// </value>
        public DateTime? RegistrationStartDateTime { get; set; }

        /// <summary>
        /// Gets or sets the registration end date time.
        /// </summary>
        /// <value>
        /// The registration end date time.
        /// </value>
        public DateTime? RegistrationEndDateTime { get; set; }

    }
}
