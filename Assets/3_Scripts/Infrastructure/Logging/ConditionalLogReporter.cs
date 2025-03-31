using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace _3_Scripts.Infrastructure.Logging
{
    public class ConditionalLogReporter : ILogReporter
    {
        
        // TODO Call analytics or monitoring service instead of Debug.Log, add sessionId, playerId, timestamp
        // Add Conditional logic
        public void ReportLog(string message)
        {
            Debug.Log(message);
        }

        public void ReportError(string message)
        {
            Debug.LogError(message);
        }
    }
}