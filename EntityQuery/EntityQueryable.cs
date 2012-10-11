using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EntityQuery
{
    public class EntityQueryable<T> : IOrderedQueryable<T>, IQueryable<T>, IEnumerable<T>, IOrderedQueryable, IQueryable, IEnumerable
    {
        private readonly Expression _expression;
        private readonly EntityQueryProvider _provider;

        public EntityQueryable(IQueryable source, IQueryStrategy queryStrategy)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (queryStrategy == null)
            {
                throw new ArgumentNullException("ordering");
            }
            this._expression = Expression.Constant(this);
            this._provider = new EntityQueryProvider(source, queryStrategy);
        }

        public EntityQueryable(IQueryable source, Expression expression, IQueryStrategy queryStrategy)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            this._expression = expression;
            this._provider = new EntityQueryProvider(source, queryStrategy);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)this._provider.ExecuteEnumerable(this)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._provider.ExecuteEnumerable(this).GetEnumerator();
        }

        public Type ElementType
        {
            get { return typeof(T); }
        }

        public Expression Expression
        {
            get { return this._expression; }
        }

        public IQueryProvider Provider
        {
            get { return this._provider; }
        }

    }
}
