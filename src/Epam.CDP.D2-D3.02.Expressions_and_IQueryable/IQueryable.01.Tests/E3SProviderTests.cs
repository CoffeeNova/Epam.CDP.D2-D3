using IQueryable._01.E3SClient;
using IQueryable._01.E3SClient.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.Linq;

namespace IQueryable._01.Tests
{
    [TestClass]
    public class E3SProviderTests
    {
        private const string User = nameof(User);
        private const string Password = nameof(Password);

        [TestInitialize]
        public void Init()
        {
        }

        [TestMethod]
        public void WithoutProvider()
        {
            var client = new E3SQueryClient(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);
            var res = client.SearchFts<EmployeeEntity>("workstation:(EPRUIZHW0249)", 0, 1);

            foreach (var emp in res)
                Console.WriteLine($"{emp.NativeName} {emp.StartWorkDate}");
        }

        [TestMethod]
        public void WithoutProviderNonGeneric()
        {
            var client = new E3SQueryClient(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);
            var res = client.SearchFts(typeof(EmployeeEntity), "workstation:(EPRUIZHW0249)", 0, 10);

            foreach (var emp in res.OfType<EmployeeEntity>())
                Console.WriteLine($"{emp.NativeName} {emp.StartWorkDate}");
        }


        [TestMethod]
        public void WithProvider()
        {
            var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

            foreach (var emp in employees.Where(e => e.Workstation == "EPRUIZHW0249"))
                Console.WriteLine($"{emp.NativeName} {emp.StartWorkDate}");
        }

        [TestMethod]
        public void E3SEntitySet_Where_Field_Equals_Const()
        {
            //Arrange
            var employees = new E3SEntitySet<EmployeeEntity>(User, Password);
            var query = employees.Where(e => e.Workstation == "EPRUIZHW0249");

            //Act
            var employee = query.FirstOrDefault();

            //Assert
            Assert.AreEqual("", employee?.NativeName);
            Assert.AreEqual("", employee?.StartWorkDate);
        }

        [TestMethod]
        public void E3SEntitySet_Where_Const_Equals_Field()
        {
            //Arrange
            var employees = new E3SEntitySet<EmployeeEntity>(User, Password);
            var query = employees.Where(e => "EPRUIZHW0249" == e.Workstation);

            //Act
            var employee = query.FirstOrDefault();

            //Assert
            Assert.AreEqual("", employee?.NativeName);
            Assert.AreEqual("", employee?.StartWorkDate);
        }

        [TestMethod]
        public void E3SEntitySet_Where_StartsWith()
        {
            //Arrange
            var employees = new E3SEntitySet<EmployeeEntity>(User, Password);
            var query = employees.Where(e => e.Workstation.StartsWith("EPRUIZHW006"));

            //Act
            var employee = query.FirstOrDefault();

            //Assert
            Assert.AreEqual("", employee?.NativeName);
            Assert.AreEqual("", employee?.StartWorkDate);
        }

        [TestMethod]
        public void E3SEntitySet_Where_EndsWith()
        {
            //Arrange
            var employees = new E3SEntitySet<EmployeeEntity>(User, Password);
            var query = employees.Where(e => e.Workstation.EndsWith("IZHW0060"));

            //Act
            var employee = query.FirstOrDefault();

            //Assert
            Assert.AreEqual("", employee?.NativeName);
            Assert.AreEqual("", employee?.StartWorkDate);
        }

        [TestMethod]
        public void E3SEntitySet_Where_Contains()
        {
            //Arrange
            var employees = new E3SEntitySet<EmployeeEntity>(User, Password);
            var query = employees.Where(e => e.Workstation.Contains("IZHW006"));

            //Act
            var employee = query.FirstOrDefault();

            //Assert
            Assert.AreEqual("", employee?.NativeName);
            Assert.AreEqual("", employee?.StartWorkDate);
        }

        [TestMethod]
        public void E3SEntitySet_And()
        {
            //Arrange
            var employees = new E3SEntitySet<EmployeeEntity>(User, Password);
            var query = employees.Where(e => e.Workstation.Contains("IZHW006"));

            //Act
            var employee = query.FirstOrDefault();

            //Assert
            Assert.AreEqual("", employee?.NativeName);
            Assert.AreEqual("", employee?.StartWorkDate);
        }
    }
}
