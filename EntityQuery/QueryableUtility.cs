using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace EntityQuery
{
    internal class QueryableUtility
    {
        private static readonly string[] _orderMethods;
        private static readonly MethodInfo[] _methods;

        public static bool IsQueryableMethod(Expression expression, string method)
        {
            return _methods.Where(m => m.Name == method).Contains(GetQueryableMethod(expression));
        }

        static QueryableUtility()
        {
            _orderMethods = new string[] { "OrderBy", "ThenBy", "OrderByDescending", "ThenByDescending" };
            _methods = typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static);
        }

        public static bool IsOrderingMethod(Expression expression)
        {
            return _orderMethods.Any<string>(method => IsQueryableMethod(expression, method));
        }

        private static MethodInfo GetQueryableMethod(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Call)
            {
                MethodCallExpression methodExpression = (MethodCallExpression)expression;
                if (methodExpression.Method.IsStatic && (methodExpression.Method.DeclaringType == typeof(Queryable)))
                {
                    return methodExpression.Method.GetGenericMethodDefinition();
                }
            }
            return null;
        }

    }
}