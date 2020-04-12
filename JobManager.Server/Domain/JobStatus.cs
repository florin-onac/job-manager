namespace JobManager.Server.Domain
{
    public enum JobStatus
    {
        Ready = 1,
        Started,
        Failed,
        Completed
    }
}