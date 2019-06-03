using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.MethodLevel)]

namespace slideshow.web.tests
{

    [TestClass]
    public class AppInit
    {
    }

}
