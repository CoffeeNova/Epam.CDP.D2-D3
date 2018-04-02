using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ExpressionTree._01;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionTree._01Tests
{
    [TestClass]
    public class WeirdransformerTests
    {
        [TestMethod]
        public void Substitute_Variable_Plus_Minus_One_To_Increment_Or_Decrement_Test()
        {
            // Arrange
            Expression<Func<int, int>> sourceExp = a => a + (a + 1) * (a + 5) * (a - 1);

            // Act
            var resultExp = new WeirdTransformer().VisitAndConvert(sourceExp, "");

            // Assert
            Assert.AreEqual("a => (a + ((Increment(a) * (a + 5)) * Decrement(a)))", resultExp?.ToString());
            Assert.AreEqual(sourceExp.Compile().Invoke(3), resultExp?.Compile().Invoke(3));
        }

        [TestMethod]
        public void SubstituteLambdaParametersTest()
        {
            // Arrange
            Expression<Func<int, string, int, int>> sourceExp = (a, b, c) => a * 2 + b.Length - c;
            var dict = new Dictionary<string, object> { { "a", 1 }, { "b", "qw" } };

            // Act
            var resultExp = (new WeirdTransformer().SubstituteLambdaParameters(sourceExp, dict));

            //Assert
            var sourceExpValue = sourceExp.Compile().Invoke(3, "test", 2);
            var resultExpValue = resultExp.Compile().Invoke(3, "test", 2);

            Assert.AreEqual(8, sourceExpValue);
            Assert.AreEqual(2, resultExpValue);
            Assert.AreEqual("(a, b, c) => (((1 * 2) + \"qw\".Length) - c)", resultExp.ToString());
        }
    }
}