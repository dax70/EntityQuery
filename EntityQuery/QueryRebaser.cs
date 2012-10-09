using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EntityQuery
{
    public class QueryRebaser : ExpressionVisitor
    {
        public static IQueryable Rebase(IQueryable source, IQueryable query)
        {
            Expression expression = new Visitor(source.Expression).Visit(query.Expression);
            return source.Provider.CreateQuery(expression);
        }

        public static Expression Rebase(Expression sourceExpression, Expression queryExpression)
        {
            return new Visitor(sourceExpression).Visit(queryExpression);
        }

        private class Visitor : ExpressionVisitor
        {
            private Expression _root;

            public Visitor(Expression root)
            {
                this._root = root;
            }

            protected override Expression VisitMethodCall(MethodCallExpression m)
            {
                if (((m.Arguments.Count > 0)
                    && (m.Arguments[0].NodeType == ExpressionType.Constant))
                    && ((((ConstantExpression)m.Arguments[0]).Value != null)
                    && (((ConstantExpression)m.Arguments[0]).Value is IQueryable)))
                {
                    List<Expression> list = new List<Expression> { this._root };
                    list.AddRange(m.Arguments.Skip<Expression>(1));
                    return Expression.Call(m.Method, list.ToArray());
                }
                return base.VisitMethodCall(m);
            }
        }
    }
}
