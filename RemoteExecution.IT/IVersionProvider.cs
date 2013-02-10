namespace RemoteExecution.IT
{
    public interface IVersionProvider
    {
        int GetVersion();
    }

    class VersionProvider : IVersionProvider
    {
        #region IVersionProvider Members

        public int GetVersion()
        {
            return 5;
        }

        #endregion
    }
}