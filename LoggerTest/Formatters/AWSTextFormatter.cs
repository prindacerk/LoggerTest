using System.IO;
using Serilog.Events;
using Serilog.Formatting;

namespace LoggerTest.Formatters
{
    public class AwsTextFormatter : ITextFormatter
    {
        public void Format(LogEvent logEvent, TextWriter output)
        {
            output.Write($"Timestamp - {logEvent.Timestamp} | Level - {logEvent.Level} | Message {logEvent.MessageTemplate} {output.NewLine}");
            if (logEvent.Exception != null)
            {
                output.Write($"Exception - {logEvent.Exception} {output.NewLine}");
            }
            output.Write($"Properties - {string.Join(", ", logEvent.Properties)}");
        }
    }
}
