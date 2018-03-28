using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace IQueryable._01
{
    public class ExpressionToFtsRequestTranslator : ExpressionVisitor
    {
        private StringBuilder _resultString;
        private const string Where = nameof(Where);
        private const string StartsWith = nameof(StartsWith);
        private const string EndsWith = nameof(EndsWith);
        private const string Contains = nameof(Contains);

        public string Translate(Expression exp)
        {
            _resultString = new StringBuilder();
            Visit(exp);

            return _resultString.ToString();
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable) && node.Method.Name == Where)
            {
                var predicate = node.Arguments[1];
                Visit(predicate);

                return node;
            }

            //•	Добавить поддержку операций включения (т.е. не точное совпадение со строкой, а частичное). 
            //  При этом в LINQ-нотации они должны выглядеть как обращение к методам класса string: StartsWith, EndsWith, Contains
            if (node.Method.DeclaringType == typeof(string))
            {
                var argument = (ConstantExpression)node.Arguments[0];

                if (node.Method.Name == StartsWith)
                    Visit(Expression.Constant($"{argument.Value }*"));
                else if (node.Method.Name == EndsWith)
                    Visit(Expression.Constant($"*{argument.Value }"));
                else if (node.Method.Name == Contains)
                    Visit(Expression.Constant($"*{argument.Value }*"));
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            switch (node.NodeType)
            {
                //•	Снять текущее ограничение на порядок операндов выражения.
                case ExpressionType.Equal:
                    if (node.Left.NodeType != ExpressionType.MemberAccess && node.Left.NodeType != ExpressionType.Constant)
                        throw new NotSupportedException("Left operand should be property/field or constant");

                    if (node.Right.NodeType != ExpressionType.MemberAccess && node.Right.NodeType != ExpressionType.Constant)
                        throw new NotSupportedException("Right operand should be property/field or constant");

                    if (node.Left.NodeType is ExpressionType.MemberAccess && node.Right.NodeType is ExpressionType.Constant)
                        VisitAndBuild(node.Left, node.Right);

                    else if (node.Left.NodeType is ExpressionType.Constant && node.Right.NodeType is ExpressionType.MemberAccess)
                        VisitAndBuild(node.Right, node.Left);

                    break;

                default:
                    throw new NotSupportedException($"Operation {node.NodeType} is not supported");
            }

            return node;
        }

        private void VisitAndBuild(Expression exp1, Expression exp2)
        {
            Visit(exp1);
            _resultString.Append("(");
            Visit(exp2);
            _resultString.Append(")");
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            _resultString.Append(node.Member.Name).Append(":");

            return base.VisitMember(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            _resultString.Append(node.Value);

            return node;
        }
    }
}
