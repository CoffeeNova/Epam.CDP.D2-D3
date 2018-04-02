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
            var res = client.SearchFTS<EmployeeEntity>(0, 1, "workstation:(EPBYBREW0165)");

            foreach (var emp in res)
                Console.WriteLine($"{emp.nativeName} {emp.shortStartWorkDate}");
        }

        [TestMethod]
        public void WithoutProviderNonGeneric()
        {
            var client = new E3SQueryClient(User, Password);
            var res = client.SearchFTS(typeof(EmployeeEntity), 0, 10, "workstation:(EPBYBREW0165)");

            foreach (var emp in res.OfType<EmployeeEntity>())
                Console.WriteLine($"{emp.nativeName} {emp.shortStartWorkDate}");
        }


        [TestMethod]
        public void WithProvider()
        {
            var employees = new E3SEntitySet<EmployeeEntity>(User, Password);

            foreach (var emp in employees.Where(e => e.workStation == "EPBYBREW0165"))
                Console.WriteLine($"{emp.nativeName} {emp.shortStartWorkDate}");
        }

        [TestMethod]
        public void E3SEntitySet_Where_Field_Equals_Const()
        {
            //Arrange
            var employees = new E3SEntitySet<EmployeeEntity>(User, Password);
            var query = employees.Where(e => e.workStation == "EPBYBREW0165");

            //Act
            var employee = query.ToList();

            //Assert
            Assert.AreEqual("Игорь Салженицин", employee.FirstOrDefault()?.nativeName);
            Assert.AreEqual("2000-08-01", employee.FirstOrDefault()?.shortStartWorkDate);
        }

        [TestMethod]
        public void E3SEntitySet_Where_Const_Equals_Field()
        {
            //Arrange
            var employees = new E3SEntitySet<EmployeeEntity>(User, Password);
            var query = employees.Where(e => "EPBYBREW0165" == e.workStation);

            //Act
            var employee = query.ToList();

            //Assert
            Assert.AreEqual("Игорь Салженицин", employee.FirstOrDefault()?.nativeName);
            Assert.AreEqual("2000-08-01", employee.FirstOrDefault()?.shortStartWorkDate);
        }

        [TestMethod]
        public void E3SEntitySet_Where_StartsWith()
        {
            //Arrange
            var employees = new E3SEntitySet<EmployeeEntity>(User, Password);
            var query = employees.Where(e => e.workStation.StartsWith("EPBYBREW016"));

            //Act
            var employee = query.ToList();

            //Assert
            Assert.AreEqual("Игорь Салженицин", employee.FirstOrDefault(x => x.workStation == "EPBYBREW0165")?.nativeName);
            Assert.AreEqual("2000-08-01", employee.FirstOrDefault(x => x.workStation == "EPBYBREW0165")?.shortStartWorkDate);
        }

        [TestMethod]
        public void E3SEntitySet_Where_EndsWith()
        {
            //Arrange
            var employees = new E3SEntitySet<EmployeeEntity>(User, Password);
            var query = employees.Where(e => e.workStation.EndsWith("BYBREW0165"));

            //Act
            var employee = query.ToList();

            //Assert
            Assert.AreEqual("Игорь Салженицин", employee.FirstOrDefault()?.nativeName);
            Assert.AreEqual("2000-08-01", employee.FirstOrDefault()?.shortStartWorkDate);
        }

        [TestMethod]
        public void E3SEntitySet_Where_Contains()
        {
            //Arrange
            var employees = new E3SEntitySet<EmployeeEntity>(User, Password);
            var query = employees.Where(e => e.workStation.Contains("BYBREW016"));

            //Act
            var employee = query.ToList();

            //Assert
            Assert.AreEqual("Игорь Салженицин", employee.FirstOrDefault(x => x.workStation == "EPBYBREW0165")?.nativeName);
            Assert.AreEqual("2000-08-01", employee.FirstOrDefault(x => x.workStation == "EPBYBREW0165")?.shortStartWorkDate);
        }

        [TestMethod]
        public void E3SEntitySet_And()
        {
            //Arrange
            var employees = new E3SEntitySet<EmployeeEntity>(User, Password);
            var query = employees.Where(e => e.workStation == "EPBYBREW0165" && e.citySum == "Brest");

            //Act
            var employee = query.ToList();

            //Assert
            Assert.AreEqual("Игорь Салженицин", employee.FirstOrDefault()?.nativeName);
            Assert.AreEqual("2000-08-01", employee.FirstOrDefault()?.shortStartWorkDate);
        }
    }
}
