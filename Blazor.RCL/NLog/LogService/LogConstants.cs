namespace Blazor.RCL.NLog.LogService
{
    public static class LogConstants
    {
        public static readonly string LogEventProperies = "requestitem|RequestItem,sourceid|SourceId,applicationnameid|applicationnameId,taskid|TaskId";

        public static string AppStatusStarted { get { return "STARTED"; } }
        public static string AppStatusRunning { get { return "RUNNING"; } }
        public static string AppStatusRejectedByValidation { get { return "REJECTED BY VALIDATION"; } }
        public static string AppStatusException { get { return "EXCEPTION"; } }
        public static string AppStatusSuccess { get { return "SUCCESS"; } }
        public static string AppStatusCompleted { get { return "COMPLETED"; } }
        public static string AppStatusDone { get { return "DONE"; } }
        public static string AppStatusRejectedByADComponent { get { return "REJECTED BY AD Component"; } }

        public enum TraceObjectType
        {
            request = 0,
            response = 1,
            other = 3
        }
    }
}
