using System;
using System.Linq.Expressions;

namespace EntityQuery
{
    public interface IPropertyAccessorProvider
    {
        bool RegisterAccessor(Type type, LambdaExpression accessor);

        LambdaExpression GetAccessor(Type type);
    }
}
