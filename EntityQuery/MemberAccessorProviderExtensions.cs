using System;
using System.Linq.Expressions;

namespace EntityQuery
{
    public static class MemberAccessorProviderExtensions
    {
        public static bool RegisterAccessor<TEntity, TProperty>(this IPropertyAccessorProvider provider, Expression<Func<TEntity, TProperty>> accessor)
        {
            return provider.RegisterAccessor(typeof(TEntity), accessor);
        }

        public static  LambdaExpression GetAccessor<TEntity>(this IPropertyAccessorProvider provider)
        {
            return provider.GetAccessor(typeof(TEntity));
        }
    }
}
