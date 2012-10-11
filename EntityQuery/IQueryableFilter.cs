namespace EntityQuery
{
    using System;
    using System.Linq;

    public interface IQueryableFilter<T>
    {
        IQueryable<T> Filter(IQueryable<T> query);
    }
}
