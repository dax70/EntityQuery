namespace EntityQuery
{
    #region Using Directives
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    #endregion

    internal class PropertyVisitor : ExpressionVisitor
    {
        private List<PropertyInfo> _properties;

        internal PropertyVisitor(Expression exp)
        {
            this._properties = new List<PropertyInfo>();
            this.Visit(exp);
        }

        public ICollection<PropertyInfo> Properties
        {
            get { return this._properties; }
        }

        public PropertyInfo Property
        {
            get { return this._properties.SingleOrDefault<PropertyInfo>(); }
        }

        public override Expression Visit(Expression exp)
        {
            if (exp == null)
            {
                return exp;
            }
            ExpressionType nodeType = exp.NodeType;
            if (((nodeType != ExpressionType.Lambda) && (nodeType != ExpressionType.MemberAccess)) && (nodeType != ExpressionType.New))
            {
                throw new NotSupportedException("Unsupported ExpressionNode type.");
            }
            return base.Visit(exp);
        }

        protected override Expression VisitLambda<T>(Expression<T> lambda)
        {
            if (lambda == null)
            {
                throw new ArgumentNullException("lambda");
            }
            if (lambda.Parameters.Count != 1)
            {
                throw new InvalidOperationException("Lambda Expression must have exactly one parameter.");
            }
            Expression body = this.Visit(lambda.Body);
            if (body != lambda.Body)
            {
                return Expression.Lambda(lambda.Type, body, lambda.Parameters);
            }
            return lambda;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            PropertyInfo member = node.Member as PropertyInfo;
            if (member == null)
            {
                throw new InvalidOperationException(string.Format("Member Expressions must be properties.", node.Member.ReflectedType.FullName, node.Member.Name));
            }
            if (node.Expression.NodeType != ExpressionType.Parameter)
            {
                throw new InvalidOperationException("Member Expressions must be bound to Lambda parameter.");
            }
            this._properties.Add(member);
            return node;
        }

        public static ICollection<PropertyInfo> GetSelectedProperties(Expression exp)
        {
            return new PropertyVisitor(exp).Properties;
        }

        public static PropertyInfo GetSelectedProperty(Expression exp)
        {
            return new PropertyVisitor(exp).Property;
        }
    }
}
