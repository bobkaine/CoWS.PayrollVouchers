using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;

namespace CoWS.Web.Logging
{
    public enum MyColor { Red, Yellow, Blue };

    [System.Diagnostics.Tracing.EventSource(Name = "CoWS.PayrollVouchers")]
    public class CoWSEventSource : System.Diagnostics.Tracing.EventSource
    {

        private string loggingLevel = System.Configuration.ConfigurationManager.AppSettings["loggingLevel"];

        private static CoWSEventSource _eventSource = new CoWSEventSource();

        public class Keywords
        {
            // Event codes...
            public const EventKeywords Information = (EventKeywords)1;
            public const EventKeywords PerformanceMonitoring = (EventKeywords)2;
            public const EventKeywords AccessDenied = (EventKeywords)4;
            public const EventKeywords ApplicationException = (EventKeywords)8;
            public const EventKeywords CriticalException = (EventKeywords)16;
            public const EventKeywords LoggingFailure = (EventKeywords)32;
        }

        public enum AppSection
        {
            Account,
            Common,
            Manage,
            ChildSupport,
            VouchersPayable,
            Payroll,
            Reports,
            Admin,
            Security,
            Users
        }

        public static bool IsInitialized
        {
            get
            {
                return _eventSource != null;
            }
        }

        public static CoWSEventSource Log
        {
            get
            {
                return _eventSource;
            }
        }

        public void HandleException(int eventID, Exception ex, AppSection appSection)
        {
            var message = "App Section: " + appSection + "\n";
            message += "Message: " + ex.Message;

            if (ex.InnerException != null)
            {
                message += "\nInnerException: " + ex.InnerException.Message;

                if (ex.InnerException.InnerException != null)
                {
                    message += "\nInnerInnerException: " + ex.InnerException.InnerException.Message;
                }
            }

            if (loggingLevel == "verbose")
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException != null)
                    {
                        message += "\nStackTrace: " + ex.InnerException.InnerException.StackTrace;
                    }
                    else
                    {
                        message += "\nStackTrace: " + ex.InnerException.StackTrace;
                    }
                }
                else
                {
                    message += "\nStackTrace: " + ex.StackTrace;
                }
            }

            WriteEventToLog(eventID, message, EventLogEntryType.Error, appSection);
        }

        private void WriteEventToLog(int eventID, string message, EventLogEntryType entryType, AppSection appSection)
        {
            string source = "CoWS.PayrollVouchers";
            string log = "Application";

            if (!EventLog.Exists(log) || !EventLog.SourceExists(source))
            {
                if (EventLog.SourceExists(source))
                {
                    EventLog.DeleteEventSource(source);
                }

                EventLog.CreateEventSource(source, log);
            }

            EventLog eventLog = new EventLog();
            eventLog.Log = log;
            eventLog.Source = source;
            var sectionEventID = Convert.ToInt16(appSection) + eventID;

            eventLog.WriteEntry(message, entryType, sectionEventID);
        }
    }
}
