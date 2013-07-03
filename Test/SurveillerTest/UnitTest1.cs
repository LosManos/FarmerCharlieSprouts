using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FarmerCharlieSprouts.Machinery.Surveiller;

namespace FarmerCharlieSprouts.Test.SurveillerTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var testee = new Surveiller();
			Assert.AreEqual("FarmerCharlieSprouts.Machinery.Surveiller.Surveiller", testee.GetType().ToString());
        }
    }
}
