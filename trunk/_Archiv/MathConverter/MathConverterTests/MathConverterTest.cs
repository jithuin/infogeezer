using System.Globalization;
using IKriv.Wpf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MathConverterTests
{
    [TestClass]
    public class MathConverterTest
    {
        Exception _exception;

        [TestMethod]
        public void SimpleConstant()
        {
            Assert.AreEqual(42m, Convert<decimal>("42"));
        }

        [TestMethod]
        public void FractionalConstant()
        {
            Assert.AreEqual(42.8m, Convert<decimal>("42.8"));
        }

        [TestMethod]
        public void NoLeadingZero()
        {
            Assert.AreEqual(0.3m, Convert<decimal>(".3"));
        }

        [TestMethod]
        public void OneIndexer()
        {
            Assert.AreEqual(29m, Convert<decimal>("{0}", 29));
        }

        [TestMethod]
        public void SimpleSum()
        {
            Assert.AreEqual(5m, Convert<decimal>("{0}+{1}", 2, 3));
        }

        [TestMethod]
        public void Multiplication()
        {
            Assert.AreEqual(6m, Convert<decimal>("{0}+{1}*{2}", 2, 2, 2));
        }

        [TestMethod]
        public void Subexpression()
        {
            Assert.AreEqual(8m, Convert<decimal>("({0}+{1})*{2}", 2, 2, 2));
        }

        [TestMethod]
        public void Subtraction()
        {
            Assert.AreEqual(2m, Convert<decimal>("{0}-8", 10));
        }

        [TestMethod]
        public void UnaryMinus()
        {
            Assert.AreEqual(-6m, Convert<decimal>("{0}*-3", 2));
        }

        [TestMethod]
        public void ComplexSymbolicExpressionXY()
        {
            Assert.AreEqual(9m, Convert<decimal>("(x+1)/(-(-x+1))+y*z/(2*t)", 3,7,8,4));
        }

        [TestMethod]
        public void ComplexSymbolicExpressionAB()
        {
            Assert.AreEqual(4m, Convert<decimal>("(a-b)*(-c+d)", 1,3,7,5));
        }

        [TestMethod]
        public void Spaces()
        {
            Assert.AreEqual(4m, Convert<decimal>("(a - b)* ( - c + d  )   ", 1, 3, 7, 5));
        }

        [TestMethod]
        public void ManyPluses()
        {
            Assert.AreEqual(42.3m, Convert<decimal>("+++++++++++++++++a", 42.3));
        }

        [TestMethod]
        public void ManyMinuses()
        {
            Assert.AreEqual(-42.3m, Convert<decimal>("-----+--a", 42.3));
        }

        [TestMethod]
        public void SameConverterDifferentParameters()
        {
            var converter = new TestMathConverter();
            Assert.AreEqual(3m, converter.Convert(null, typeof(decimal), "2+1", CultureInfo.InvariantCulture));
            Assert.AreEqual(4m, converter.Convert(new object[] {1,3}, typeof(decimal), "a+b", CultureInfo.InvariantCulture));
            Assert.AreEqual(3m, converter.Convert(null, typeof(decimal), "2+1", CultureInfo.InvariantCulture));
            Assert.AreEqual(5m, converter.Convert(new object[] { 2, 3 }, typeof(decimal), "a+b", CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public void IntConvert()
        {
            var result = Convert<int>("1+x", 2);
            Assert.AreEqual(3, result);
            Assert.IsTrue(result is int);
        }

        [TestMethod]
        public void LongConvert()
        {
            var result = Convert<long>("1+x", 29999999999999L);
            Assert.AreEqual(30000000000000L, result);
            Assert.IsTrue(result is long);
        }

        [TestMethod]
        public void StringConvert()
        {
            var result = Convert<string>("1+x", 4);
            Assert.AreEqual("5", result);
            Assert.IsTrue(result is string);
        }

        [TestMethod]
        public void IndexOutOfRange()
        {
            Convert<decimal>("a+b", 1);
            Assert.IsNotNull(_exception);
            Assert.AreEqual("MathConverter: parameter index 1 is out of range. 1 parameter(s) supplied", _exception.Message);
        }

        [TestMethod]
        public void EmptyExpression()
        {
            Convert<decimal>("", 1);
            Assert.IsNotNull(_exception);
            Assert.AreEqual("MathConverter: error parsing expression ''. Unexpected end of text at position 0", _exception.Message);
        }

        [TestMethod]
        public void EmptyParens()
        {
            Convert<decimal>("()", 1);
            Assert.IsNotNull(_exception);
            Assert.AreEqual("MathConverter: error parsing expression '()'. Unexpeted character ')' at position 1", _exception.Message);
        }

        [TestMethod]
        public void UnbalancedParens()
        {
            Convert<decimal>("((3)", 1);
            Assert.IsNotNull(_exception);
            Assert.AreEqual("MathConverter: error parsing expression '((3)'. Expected ')' at position 4", _exception.Message);
        }

        [TestMethod]
        public void HangingPlus()
        {
            Convert<decimal>("2+", 1);
            Assert.IsNotNull(_exception);
            Assert.AreEqual("MathConverter: error parsing expression '2+'. Unexpected end of text at position 2", _exception.Message);
        }

        [TestMethod]
        public void BadNumber()
        {
            Convert<decimal>("2.3.4", 1);
            Assert.IsNotNull(_exception);
            Assert.AreEqual("MathConverter: error parsing expression '2.3.4'. Unexpected character '.' at position 3", _exception.Message);
        }

        [TestMethod]
        public void BadNumber2()
        {
            Convert<decimal>("(1+2.3.4)", 1);
            Assert.IsNotNull(_exception);
            Assert.AreEqual("MathConverter: error parsing expression '(1+2.3.4)'. Expected ')' at position 6", _exception.Message);
        }

        [TestMethod]
        public void ExtraTextAfterExpression()
        {
            Convert<decimal>("(1+2)sometexthere", 1);
            Assert.IsNotNull(_exception);
            Assert.AreEqual("MathConverter: error parsing expression '(1+2)sometexthere'. Unexpected character 's' at position 5", _exception.Message);
        }

        [TestMethod]
        public void InvalidIndex()
        {
            Convert<decimal>("{foo}+1", 1);
            Assert.IsNotNull(_exception);
            Assert.AreEqual("MathConverter: error parsing expression '{foo}+1'. 'foo' is not a valid parameter index at position 1", _exception.Message);
        }


        private object Convert<T>( string str, params object[] args)
        {
            var converter = new TestMathConverter();
            var result = converter.Convert(args, typeof(T), str, CultureInfo.InvariantCulture);
            _exception = converter.Exception;
            return result;
        }

        class TestMathConverter : MathConverter
        {
            public Exception Exception { get; set; }

            protected override void ProcessException(Exception ex)
            {
                this.Exception = ex;
            }
        }
    }
}
