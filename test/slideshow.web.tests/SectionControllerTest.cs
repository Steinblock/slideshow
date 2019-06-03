using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using slideshow.core.Models;
using slideshow.core.Repository;
using slideshow.web.Controllers;
using slideshow.web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace slideshow.web.tests
{
    [TestClass]
    public class SectionControllerTest
    {
        private Mock<ISectionRepository> repo;
        private SectionController controller;

        [TestInitialize]
        public void MyTestInitialize()
        {
            this.repo = new Mock<ISectionRepository>();
            this.controller = new SectionController(repo.Object);
        }

        [TestMethod]
        public void CanGetIndex()
        {

            var section1 = new Mock<ISection>()
                .SetupAllProperties();
            var section2 = new Mock<ISection>()
                .SetupAllProperties();
            var sections = new[] { section1.Object, section2.Object }.AsQueryable();

            repo.Setup(x => x.GetAllSections()).Returns(sections);

            var result = this.controller.Index() as OkObjectResult;
            Assert.IsNotNull(result);

            var model = result.Value as PagedModel<SectionViewModel>;

            Assert.IsNotNull(model);

            Assert.AreEqual(2, model.total);
                
        }
    }
}
