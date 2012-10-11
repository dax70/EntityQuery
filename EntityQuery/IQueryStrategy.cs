using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EntityQuery
{
    public interface IQueryStrategy
    {
        Expression Visit(IQueryable query);
    }
}
