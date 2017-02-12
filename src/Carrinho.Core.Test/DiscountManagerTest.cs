using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carrinho.Core;

namespace Carrinho.Core.Test
{
    [TestClass]
    public class DiscountManagerTest
    {
        public DiscountManager discountManager;
        
        [TestInitialize]
        public void Initialize()
        {
            discountManager = new DiscountManager();
        }

        [TestMethod]
        public void GetDiscount_0_Should_Return_Rate_0_And_Value_0()
        {
            var rule = discountManager.GetDiscount(0);
            Assert.AreEqual(0, rule.Rate);
            Assert.AreEqual(0, rule.CalculatedDiscount);
        }

        [TestMethod]
        public void GetDiscount_499_99_Should_Return_Rate_0_And_Value_0()
        {
            var rule = discountManager.GetDiscount(499.99M);
            Assert.AreEqual(0, rule.Rate);
            Assert.AreEqual(0, rule.CalculatedDiscount);
        }

        [TestMethod]
        public void GetDiscount_500M_Should_Return_Rate_5_And_Value_25()
        {
            var rule = discountManager.GetDiscount(500M);
            Assert.AreEqual(.05M, rule.Rate);
            Assert.AreEqual(25, rule.CalculatedDiscount);
        }

        [TestMethod]
        public void GetDiscount_599_99M_Should_Return_Rate_5_And_Value_25()
        {
            var rule = discountManager.GetDiscount(599.99M);
            Assert.AreEqual(.05M, rule.Rate);
            Assert.AreEqual(30M, rule.CalculatedDiscount);
        }

        [TestMethod]
        public void GetDiscount_600M_Should_Return_Rate_10_And_Value_60()
        {
            var rule = discountManager.GetDiscount(600M);
            Assert.AreEqual(.10M, rule.Rate);
            Assert.AreEqual(60M, rule.CalculatedDiscount);
        }

        [TestMethod]
        public void GetDiscount_699_99M_Should_Return_Rate_10_And_Value_70M()
        {
            var rule = discountManager.GetDiscount(699.99M);
            Assert.AreEqual(.10M, rule.Rate);
            Assert.AreEqual(70M, rule.CalculatedDiscount);
        }

        [TestMethod]
        public void GetDiscount_700M_Should_Return_Rate_15_And_Value_105()
        {
            var rule = discountManager.GetDiscount(700M);
            Assert.AreEqual(.15M, rule.Rate);
            Assert.AreEqual(105M, rule.CalculatedDiscount);
        }

        [TestMethod]
        public void GetDiscount_10000M_Should_Return_Rate_15_And_Value_1500()
        {
            var rule = discountManager.GetDiscount(10000M);
            Assert.AreEqual(.15M, rule.Rate);
            Assert.AreEqual(1500M, rule.CalculatedDiscount);
        }
    }
}
