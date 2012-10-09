using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace EntityQuery
{
    internal sealed class MemberAccessorProvider : IPropertyAccessorProvider
    {
        private ConcurrentDictionary<Type, LambdaExpression> propertyAccessors = new ConcurrentDictionary<Type, LambdaExpression>();

        public MemberAccessorProvider()
        {
        }

        public bool RegisterAccessor(Type type, LambdaExpression accessor)
        {
            return propertyAccessors.TryAdd(type, accessor);
        }

        public LambdaExpression GetAccessor(Type type)
        {
            return propertyAccessors.GetOrAdd(type, t => TypeHelper.GetPropertyAccessor(t));
        }

    }
}
