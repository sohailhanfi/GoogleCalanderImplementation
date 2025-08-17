using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Threading;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace ToolsApp
{
    public partial class _Default : Page
    {
        CalendarService calService;

   

        private const string calID = "You@email.com";
        private const string UserId = "You@email.com";

        private static string gFolder = System.Web.HttpContext.Current.Server.MapPath("/App_Data/MyGoogleStorage");
        protected void Page_Load(object sender, EventArgs e)
        {
         
           



        }
        public void Authenticate()
        {
            CalendarService service = null;

            IAuthorizationCodeFlow flow = new GoogleAuthorizationCodeFlow(
                new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = GetClientConfiguration().Secrets,
                    DataStore = new FileDataStore(gFolder),
                    Scopes = new[] { CalendarService.Scope.Calendar }
                });

            var uri = Request.Url.ToString();
            var code = Request["code"];
         
            if (code != null)
            {
                var token = flow.ExchangeCodeForTokenAsync(UserId, code,
                    uri.Substring(0, uri.IndexOf("?")), CancellationToken.None).Result;

                // Extract the right state.
                var oauthState = AuthWebUtility.ExtracRedirectFromState(
                    flow.DataStore, UserId, Request["state"]).Result;
                Response.Redirect(oauthState);
            }
            else
            {
                var result = new AuthorizationCodeWebApp(flow, uri, uri).AuthorizeAsync(UserId,
                    CancellationToken.None).Result;
         
           
                if (result.RedirectUri != null)
                {
                    // Redirect the user to the authorization server.
                    Response.Redirect(result.RedirectUri);
                }
                else
                {
                    // The data store contains the user credential, so the user has been already authenticated.
                    service = new CalendarService(new BaseClientService.Initializer
                    {
                        ApplicationName = "ToolsApp",
                        HttpClientInitializer = result.Credential
                    });
                }
            }

            calService = service;
        }

        public static GoogleClientSecrets GetClientConfiguration()
        {
            using (var stream = new FileStream(gFolder + @"\client_secret.json", FileMode.Open, FileAccess.Read))
            {
                return GoogleClientSecrets.Load(stream);
            }
        }
        public string CreateUpdateEvent(string ExpKey, string ExpVal, string evTitle, string evDate)
        {
            EventsResource er = new EventsResource(calService);
            var queryEvent = er.List(calID);
            queryEvent.SharedExtendedProperty = ExpKey + "=" + ExpVal; //"EventKey=9999"
            var EventsList = queryEvent.Execute();

            Event ev = new Event();
            EventDateTime StartDate = new EventDateTime();
            StartDate.Date = evDate; 
            EventDateTime EndDate = new EventDateTime();
            EndDate.Date = evDate; 

            ev.Start = StartDate;
            ev.End = EndDate;
            ev.Summary = evTitle;

            string FoundEventID = String.Empty;
            foreach (var evItem in EventsList.Items)
            {
                FoundEventID = evItem.Id;
            }

            if (String.IsNullOrEmpty(FoundEventID))
            {
                //If event does not exist, Append Extended Property and create the event
                Event.ExtendedPropertiesData exp = new Event.ExtendedPropertiesData();
                exp.Shared = new Dictionary<string, string>();
                exp.Shared.Add(ExpKey, ExpVal);
                ev.ExtendedProperties = exp;
                return er.Insert(ev, calID).Execute().Summary;
            }
            else
            {
                //If existing, Update the event
                return er.Update(ev, calID, FoundEventID).Execute().Summary;
            }
        }

        public bool DeleteEvents(string summary,string ExpKey, string ExpVal, string evDate)
        {
            EventsResource er = new EventsResource(calService);
            var queryEvent = er.List(calID);

            queryEvent.SharedExtendedProperty = ExpKey + "=" + ExpVal; //"EventKey=9999"
            var EventsList = queryEvent.Execute();
            Event ev = new Event();
            EventDateTime StartDate = new EventDateTime();
            StartDate.Date = evDate;
            EventDateTime EndDate = new EventDateTime();
            EndDate.Date = evDate;

            ev.Start = StartDate;
            ev.End = EndDate;
            ev.Summary = summary;

            string FoundEventID = String.Empty;
            foreach (Event evItem in EventsList.Items)
            {
                FoundEventID = evItem.Id;
                er.Delete(calID, FoundEventID).Execute();
                return true;
            }

            return false;
        }

        protected void getCalander_Click(object sender, EventArgs e)
        {
            Authenticate();
            CreateUpdateEvent("petsAllowed", "yes", "Event for App Testing 2", "2022-06-24");
            //calander.Visible = true;
        }

        protected void DeleteEvent_Click(object sender, EventArgs e)

        {
            Authenticate();
            DeleteEvents("petsAllowed", "yes", "Waqas Birthday", "2022-06-27");
        }
    }
}
