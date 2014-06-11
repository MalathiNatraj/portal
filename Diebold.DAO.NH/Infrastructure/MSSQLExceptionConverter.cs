using System;
using NHibernate.Exceptions;
using System.Data.SqlClient;
using NHibernate;

namespace Diebold.DAO.NH.Infrastructure
{
    public class MSSQLExceptionConverter : ISQLExceptionConverter 
    {
        public Exception Convert(AdoExceptionContextInfo adoExceptionContextInfo)
        {
            var sqle = ADOExceptionHelper.ExtractDbException(adoExceptionContextInfo.SqlException) as SqlException;
            if(sqle != null)
            {
                switch (sqle.Number)
                {
                    case 547:
                        return new ConstraintViolationException(adoExceptionContextInfo.Message,
                            sqle.InnerException, adoExceptionContextInfo.Sql, null);
                      case 208:
                          return new SQLGrammarException(adoExceptionContextInfo.Message,
                              sqle.InnerException, adoExceptionContextInfo.Sql);
                      case 3960:
                          return new StaleObjectStateException(adoExceptionContextInfo.EntityName, adoExceptionContextInfo.EntityId);
                }
            }

            throw sqle;
            //return SQLStateConverter.HandledNonSpecificException(adoExceptionContextInfo.SqlException,
            //    adoExceptionContextInfo.Message, adoExceptionContextInfo.Sql);
        }
    }
}
