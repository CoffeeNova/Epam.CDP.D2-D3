using IQueryable._01.E3SClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.Linq;

namespace IQueryable._01.Tests
{
    [TestClass]
    public class E3SProviderTests
    {
        private static readonly string User = ConfigurationManager.AppSettings["user"];
        private static readonly string Password = ConfigurationManager.AppSettings["password"];

        [TestInitialize]
        public void Init()
        {
        }

        [TestMethod]
        public void WithoutProvider()
        {
            var client = new E3SQueryClient(User, Password);
            var res = client.SearchFTS<EmployeeEntity>("workstation:(EPRUIZHW0249)", 0, 1);

            foreach (var emp in res)
                Console.WriteLine($"{emp.nativeName} {emp.shortStartWorkDate}");
        }

        [TestMethod]
        public void WithoutProviderNonGeneric()
        {
            var client = new E3SQueryClient(User, Password);
            var res = client.SearchFTS(typeof(EmployeeEntity), "workstation:(EPRUIZHW0249)", 0, 10);

            foreach (var emp in res.OfType<EmployeeEntity>())
                Console.WriteLine($"{emp.nativeName} {emp.shortStartWorkDate}");
        }


        [TestMethod]
        public void WithProvider()
        {
            var employees = new E3SEntitySet<EmployeeEntity>(User, Password);

            foreach (var emp in employees.Where(e => e.workStation == "EPRUIZHW0249"))
                Console.WriteLine($"{emp.nativeName} {emp.shortStartWorkDate}");
        }

        [TestMethod]
        public void E3SEntitySet_Where_Field_Equals_Const()
        {
            //Arrange
            var employees = new E3SEntitySet<EmployeeEntity>(User, Password);
            var query = employees.Where(e => e.workStation == "EPRUIZHW0249");

            //Act
            var employee = query.FirstOrDefault();

            //Assert
            Assert.AreEqual("", employee?.nativeName);
            Assert.AreEqual("", employee?.shortStartWorkDate);
        }

        [TestMethod]
        public void E3SEntitySet_Where_Const_Equals_Field()
        {
            //Arrange
            var employees = new E3SEntitySet<EmployeeEntity>(User, Password);
            var query = employees.Where(e => "EPRUIZHW0249" == e.workStation);

            //Act
            var employee = query.FirstOrDefault();

            //Assert
            Assert.AreEqual("", employee?.nativeName);
            Assert.AreEqual("", employee?.shortStartWorkDate);
        }

        [TestMethod]
        public void E3SEntitySet_Where_StartsWith()
        {
            //Arrange
            var employees = new E3SEntitySet<EmployeeEntity>(User, Password);
            var query = employees.Where(e => e.workStation.StartsWith("EPRUIZHW006"));

            //Act
            var employee = query.FirstOrDefault();

            //Assert
            Assert.AreEqual("", employee?.nativeName);
            Assert.AreEqual("", employee?.shortStartWorkDate);
        }

        [TestMethod]
        public void E3SEntitySet_Where_EndsWith()
        {
            //Arrange
            var employees = new E3SEntitySet<EmployeeEntity>(User, Password);
            var query = employees.Where(e => e.workStation.EndsWith("IZHW0060"));

            //Act
            var employee = query.FirstOrDefault();

            //Assert
            Assert.AreEqual("", employee?.nativeName);
            Assert.AreEqual("", employee?.shortStartWorkDate);
        }

        [TestMethod]
        public void E3SEntitySet_Where_Contains()
        {
            //Arrange
            var employees = new E3SEntitySet<EmployeeEntity>(User, Password);
            var query = employees.Where(e => e.workStation.Contains("IZHW006")); 

            //Act
            var employee = query.FirstOrDefault();

            //Assert
            Assert.AreEqual("", employee?.nativeName);
            Assert.AreEqual("", employee?.shortStartWorkDate);
        }

        [TestMethod]
        public void E3SEntitySet_And()
        {
            //Arrange
            var employees = new E3SEntitySet<EmployeeEntity>(User, Password);
            var query = employees.Where(e => e.workStation == "EPRUIZHW0249" & e.nativeName == "");

            //Act
            var employee = query.FirstOrDefault();

            //Assert
            Assert.AreEqual("", employee?.nativeName);
            Assert.AreEqual("", employee?.shortStartWorkDate);
        }
    }
}
