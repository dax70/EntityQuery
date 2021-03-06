﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EntityQuery
{
    internal class EntityQueryProvider : ExpressionVisitor, IQueryProvider
    {
        private IQueryable _source;
        private readonly IQueryStrategy queryStrategy;

        private class ParameterReplacer : ExpressionVisitor
        {
            private readonly Type _parameterType;

            public ParameterReplacer(Type parameterType)
            {
                this._parameterType = parameterType;
            }

            //protected override Expression VisitLambda<T>(Expression<T> node)
            //{
            //    ParameterExpression parameter = (ParameterExpression)replaceWith;
            //    Expression body = Visit(node.Body);
            //    return Expression.Lambda(body, new ParameterExpression[] { parameter });

            //    //return base.VisitLambda<T>(node);
            //}

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (node.Type != this._parameterType)
                {
                    return Expression.Parameter(this._parameterType, node.Name);
                }
                return base.VisitParameter(node);
            }
        }

        public EntityQueryProvider(IQueryable source, IQueryStrategy queryStrategy)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (queryStrategy == null)
            {
                throw new ArgumentNullException("ordering");
            }
            this.queryStrategy = queryStrategy;
            this._source = source;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            return new EntityQueryable<TElement>(this._source, expression, this.queryStrategy);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            Type type = expression.Type.GetGenericArguments().First<Type>();
            return (IQueryable)Activator.CreateInstance(typeof(EntityQueryable<>).MakeGenericType(new Type[] { type }), new object[] { this._source, expression, this.queryStrategy });
        }

        public TResult Execute<TResult>(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            return (TResult)this.Execute(expression);
        }

        public object Execute(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            Expression expression2 = this.VisitAll(expression);
            return this._source.Provider.Execute(expression2);
        }

        internal IEnumerable ExecuteEnumerable<T>(EntityQueryable<T> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            Expression expression2 = this.VisitAll(this.queryStrategy.Visit(query));
            return this._source.Provider.CreateQuery(expression2);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            //if (node.Type.IsGenericType && (node.Type.GetGenericTypeDefinition() == typeof(EntityQueryable<>)))
            //{
            //    return this._source.Expression;
            //}

            if (typeof(IQueryable).IsAssignableFrom(node.Type))
            {
                Type type = node.Type.GetGenericArguments()[0];
                Type rootType = this._source.Expression.Type.GetGenericArguments()[0];
                if (type.IsSubclassOf(rootType))
                {
                    this._source = this._source.OfType(type);
                }
                return this._source.Expression;
            }

            return base.VisitConstant(node);
        }

        private Expression VisitAll(Expression expression)
        {
            return this.Visit(expression);
        }
    }
}
