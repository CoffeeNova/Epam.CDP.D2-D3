using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionTree._02.Tests
{
    [TestClass]
    public class WeirdMapperTests
    {
        private IWeirdMapper _mapper;

        [TestInitialize]
        public void Init()
        {
            _mapper = new WeirdMapper();
            _mapper.CreateMap<Foo, Bar>();
            _mapper.CreateMap<Bar, Foo>();
        }

        [TestMethod]
        public void TestMethod1()
        {
            //arrange
            var foo1 = new Foo
            {
                Id = 1,
                Name = "testName",
                Date = DateTime.Now
            };

            //act
            var result = _mapper.Map<Foo, Bar>(foo1);

            //assert
            Assert.AreEqual(foo1.Id, result.Id);
            Assert.AreEqual(foo1.Name, result.Name);
            Assert.AreEqual(foo1.Date, result.Date);
        }

        public class Foo
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime Date;

        }

        public class Bar
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime Date;
        }
    }
}
