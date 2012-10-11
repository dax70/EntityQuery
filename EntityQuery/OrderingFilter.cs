using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EntityQuery
{
    internal class OrderingFilter<T>: IQueryStrategy
    {
        private IPropertyAccessorProvider propertyAccessor;

        public OrderingFilter(IPropertyAccessorProvider propertyAccessor)
        {
            this.propertyAccessor = propertyAccessor;
        }

        public Expression Visit(IQueryable query)
        {
            if (OrderingFinder.NeedsOrderMethod(query.Expression))
            {
                // Create Ordered at root and combine both queries.
                var orderedQuery = query.OrderBy(this.propertyAccessor.GetAccessor<T>());
                return QueryComposer.Compose(orderedQuery, query).Expression;
            }
            return query.Expression;
        }
    }
}
