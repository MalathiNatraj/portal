namespace Diebold.Platform.Proxies.REST.Enums
{
    #region Common

    public enum ContentFormat {
        Xml,
        Json,
        Unsupported
    }

    #endregion

    #region Request

    public enum RequestMethod { GET, POST, PUT, DELETE }

    #endregion

    #region Response

    public enum ResponseStatus
    {
        Aborted,
        Completed,
        Error,
        None,
        TimedOut
    }

    #endregion
}
