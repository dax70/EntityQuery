using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EntityQuery
{
    internal sealed class OrderingFinder : ExpressionVisitor
    {
        private Action<OrderingFinder, MethodCallExpression> state = Start;
        // No op lambda.
        private readonly static Action<OrderingFinder, MethodCallExpression> Finish = (f, m) => { };

        public OrderingFinder()
        {
        }

        internal bool NeedsOrderBy { get; set; }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            state(this, node);
            return base.VisitMethodCall(node);
        }

        internal static bool NeedsOrderMethod(Expression expression)
        {
            OrderingFinder finder = new OrderingFinder
            {
                NeedsOrderBy = false
            };
            finder.Visit(expression);
            return finder.NeedsOrderBy;
        }

        private static string[] methods = new string[] { "Skip", "Take" };

        private static void Start(OrderingFinder finder, MethodCallExpression node)
        {
            if (methods.Any(m => m.Equals(node.Method.Name)))
            {
                finder.NeedsOrderBy = true;
                finder.state = NeedsOrdering;
            }
        }

        private static void NeedsOrdering(OrderingFinder finder, MethodCallExpression node)
        {
            if (QueryableUtility.IsOrderingMethod(node))
            {
                finder.NeedsOrderBy = false;
                finder.state = Finish;
            }
        }

    }
}