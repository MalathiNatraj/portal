using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DieboldMobile.Controllers
{
    interface IBaseController
    {
        void LogDebug(object message);

        void LogError(object message);
        void LogError(object message, Exception exception);

        void LogFatal(object message);
        void LogFatal(object message, Exception exception);

        void LogInfo(object message);

        void LogWarn(object message);
        void LogWarn(object message, Exception exception);
    }
}
