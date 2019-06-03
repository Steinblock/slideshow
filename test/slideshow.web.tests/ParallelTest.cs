using Microsoft.VisualStudio.TestTools.UnitTesting;
using slideshow.web;
using System.Threading;
using System.Threading.Tasks;

namespace slideshow.web.tests
{
    [TestClass]
    public class ParallelTest
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            await Task.Delay(5000);
        }

        [TestMethod]
        public void TestMethod2()
        {
            Thread.Sleep(5000);
        }

        [TestMethod]
        public void TestMethod3()
        {
            Task.Delay(5000).Wait();
        }
    }
}
