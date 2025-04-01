namespace _3_Scripts.Infrastructure.Logging
{
    public interface ILogReporter
    {
        void ReportLog(string message);
        void ReportError(string message);  
    }
}