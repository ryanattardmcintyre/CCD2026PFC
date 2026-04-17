using Google.Api;
using Google.Cloud.Logging.Type;
using Google.Cloud.Logging.V2;
using Google.Protobuf.WellKnownTypes;
using Microsoft.CodeAnalysis;

namespace WebApplication2.Repositories
{
    public class LogsRepository
    {
        public static void WriteLogEntry(string projectId,  LogSeverity severity, string message, Exception? ex, string logId = "class_demo")
        {
            //LogSeverity = error, info, verbose, warning, debug ...

            var client = LoggingServiceV2Client.Create();
            LogName logName = new LogName(projectId, logId);

            var jsonPayload = new Struct()
            {
                Fields =
                {
                    { "message", Value.ForString(message) },
                    { "exception", Value.ForString(ex?.ToString() ?? "") }
                }
            };

            severity = ex != null ? LogSeverity.Error : severity;

            LogEntry logEntry = new LogEntry
            {
                LogNameAsLogName = logName,
                Severity = severity,
                JsonPayload = jsonPayload
            };
            
            MonitoredResource resource = new MonitoredResource { Type = "global" };
            IDictionary<string, string> entryLabels = new Dictionary<string, string>
    {
       
    };
            client.WriteLogEntries(logName, resource, entryLabels,
                new[] { logEntry });

        }
    }
}
