using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModulePR;

namespace ModulePR.test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMinDt()
        {
            int[] testmass = new int[] { 100, 15, 12, 10, 20, 101 };
            var i = FindDT.FindMinDt(testmass);
            Assert.AreEqual(4, i);
        }
        [TestMethod]
        public void TestNull()
        {
            int[] testmass = new int[] { 0,1 };
            var i = FindDT.FindMinDt(testmass);
            Assert.AreEqual(0, i);
        }
    }
}
