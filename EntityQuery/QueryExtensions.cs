#region Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web; 
#endregion

namespace EntityQuery
{
    public static class QueryExtensions
    {
        public static IQueryable OfType(this IQueryable source, Type type)
        {
            return source.Provider.CreateQuery(
                    Expression.Call(null, 
                        typeof(Queryable).GetMethod("OfType").MakeGenericMethod(new Type[] { type }), 
                        new Expression[] { source.Expression })
                    );
        }

        public static IQueryable OrderBy(this IQueryable source, LambdaExpression ordering)
        {
            return source.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable),
                        "OrderBy",
                        new Type[] { source.ElementType, ordering.Body.Type /* Should be property Type*/ },
                        new Expression[] { Expression.Constant(source), Expression.Quote(ordering) }
                    ));
        }

        public static IQueryable Take(this IQueryable source, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Take", new Type[] { source.ElementType }, new Expression[] { source.Expression, Expression.Constant(count) }));
        }
    }
}