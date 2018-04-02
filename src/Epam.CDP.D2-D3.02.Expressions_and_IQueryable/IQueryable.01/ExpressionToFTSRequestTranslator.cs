using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using IQueryable._01.E3SClient;

namespace IQueryable._01
{
    public class ExpressionToFtsRequestTranslator : ExpressionVisitor
    {
        private StringBuilder _resultString;
        private const string Where = nameof(Where);

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
                if (node.Method.Name == Enum.GetName(typeof(Search), Search.StartsWith))
                    _constantModifier = Search.StartsWith;

                else if (node.Method.Name == Enum.GetName(typeof(Search), Search.EndsWith))
                    _constantModifier = Search.EndsWith;

                else if (node.Method.Name == Enum.GetName(typeof(Search), Search.Contains))
                    _constantModifier = Search.Contains;
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
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    VisitAnd(node);
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

        private void VisitAnd(BinaryExpression exp)
        {
            Visit(exp.Left);
            _resultString.Append(Constants.QUERY_AND_DELIMITER);
            Visit(exp.Right);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            _resultString.Append(node.Member.Name).Append(":");

            return base.VisitMember(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (_constantModifier == Search.EndsWith || _constantModifier == Search.Contains)
                _resultString.Append("*");
            _resultString.Append(node.Value);
            if (_constantModifier == Search.StartsWith || _constantModifier == Search.Contains)
                _resultString.Append("*");

            return node;
        }

        private Search _constantModifier = Search.Nope;

        private enum Search
        {
            Nope,
            EndsWith,
            StartsWith,
            Contains
        }
    }
}
