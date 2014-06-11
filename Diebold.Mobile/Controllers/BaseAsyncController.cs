using System;
using System.Web;
using System.Web.Mvc;
using log4net;
using System.Reflection;
/// <summary>
/// Summary description for BaseAsyncController
/// </summary>
/// 
namespace DieboldMobile.Controllers
{
    public class BaseAsyncController : AsyncController, IBaseController
    {
        protected static ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void LogDebug(object message)
        {
            if (logger.IsDebugEnabled)
            {
                logger.Debug(message);
            }
        }

        public void LogError(object message)
        {
            if (logger.IsErrorEnabled)
            {
                logger.Error(message);
            }
        }

        public void LogError(object message, Exception exception)
        {
            if (logger.IsErrorEnabled)
            {
                logger.Error(message, exception);
            }
        }

        public void LogFatal(object message)
        {
            if (logger.IsFatalEnabled)
            {
                logger.Fatal(message);
            }
        }

        public void LogFatal(object message, Exception exception)
        {
            if (logger.IsFatalEnabled)
            {
                logger.Fatal(message, exception);
            }
        }

        public void LogInfo(object message)
        {
            if (logger.IsInfoEnabled)
            {
                logger.Info(message);
            }
        }

        public void LogWarn(object message)
        {
            if (logger.IsWarnEnabled)
            {
                logger.Warn(message);
            }
        }

        public void LogWarn(object message, Exception exception)
        {
            if (logger.IsWarnEnabled)
            {
                logger.Warn(message, exception);
            }
        }
    }
}