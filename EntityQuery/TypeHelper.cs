using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityQuery
{
    internal static class TypeHelper
    {
        internal static LambdaExpression GetPropertyAccessor(Type type)
        {
            var parameter = Expression.Parameter(type);
            var propertyInfo = type.GetProperties().Where(property => property.IsSimpleProperty()).FirstOrDefault(); ;
            return Expression.Lambda(Expression.Property(parameter, propertyInfo), parameter);
        }

        internal static bool IsNullableSimpleType(Type type)
        {
            Type nullable = Nullable.GetUnderlyingType(type);
            if (nullable != null)
            {
                return IsSimpleType(nullable);
            }
            return IsSimpleType(type);
        }

        internal static bool IsSimpleType(Type type)
        {
            if (((!type.IsPrimitive && !type.Equals(typeof(string)))
                && (!type.Equals(typeof(DateTime)) && !type.Equals(typeof(decimal))))
                && (!type.Equals(typeof(Guid)) && !type.Equals(typeof(DateTimeOffset))))
            {
                return type.Equals(typeof(TimeSpan));
            }
            return true;
        }

        internal static bool IsSimpleProperty(this PropertyInfo property)
        {
            return IsNullableSimpleType(property.PropertyType) || IsSimpleType(property.PropertyType);
        }
    }
}