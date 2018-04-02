using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionTree._01
{
    public class WeirdTransformer : ExpressionVisitor
    {
        protected override Expression VisitBinary(BinaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Add:
                    return SubstituteOperation(node, Expression.Increment);
                case ExpressionType.Subtract:
                    return SubstituteOperation(node, Expression.Decrement);

                default:
                    return base.VisitBinary(node);
            }
        }

        private Expression SubstituteOperation(BinaryExpression node, Func<Expression, Expression> operand)
        {
            var parameter = node.Left.NodeType == ExpressionType.Parameter
                ? (ParameterExpression)node.Left
                : null;
            var constant = node.Right.NodeType == ExpressionType.Constant
                ? (ConstantExpression)node.Right
                : null;
            

            if (parameter == null || constant == null || constant.Value is int == false || (int)constant.Value != 1)
                return base.VisitBinary(node);

            var changedPar = VisitParameter(parameter);
            return operand(changedPar);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="parametersToChange">Contains dictionary of parameters necessary for changing, where key is name of parameter and value it's constant value.</param>
        /// <returns></returns>
        public T SubstituteLambdaParameters<T>(T expression, IDictionary<string, object> parametersToChange)
            where T : Expression
        {
            _parametersToChange = parametersToChange ?? throw new ArgumentNullException(nameof(parametersToChange));

            return VisitAndConvert(expression, "");
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if (node.NodeType != ExpressionType.Lambda)
                return base.VisitLambda(node);

            return Expression.Lambda(Visit(node.Body), node.Parameters);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (_parametersToChange == null || !_parametersToChange.Any())
                return base.VisitParameter(node);
            if (node.NodeType != ExpressionType.Parameter)
                return base.VisitParameter(node);

            if (_parametersToChange.TryGetValue(node.Name, out var value) && value?.GetType() == node.Type)
                return Expression.Constant(value);

            return base.VisitParameter(node);
        }


        private IDictionary<string, object> _parametersToChange;
    }
}
