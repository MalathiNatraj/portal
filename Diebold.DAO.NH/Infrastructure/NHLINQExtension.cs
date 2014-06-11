using System;
using System.Linq;
using NHibernate;
using System.Linq.Expressions;
using NHibernate.Linq;

namespace Diebold.DAO.NH.Infrastructure
{
    public static class NHLINQExtension
    {
        public static IFutureValue<TResult> ToFutureValue<TSource, TResult>(
            this IQueryable<TSource> source, Expression<Func<IQueryable<TSource>, TResult>> selector) where TResult : struct
        {
            var provider = (INhQueryProvider)source.Provider;
            var method = ((MethodCallExpression)selector.Body).Method;
            var expression = Expression.Call(null, method, source.Expression);
            return (IFutureValue<TResult>)provider.ExecuteFuture(expression);
        }
    }
}
