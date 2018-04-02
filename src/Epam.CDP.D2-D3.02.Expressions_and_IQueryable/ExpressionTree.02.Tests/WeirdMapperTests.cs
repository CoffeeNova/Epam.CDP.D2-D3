using System;
using System.Diagnostics.CodeAnalysis;
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
            _mapper.CreateMap<FooSpecial, BarSpecial>();
        }

        [TestMethod]
        public void Should_Map_Correctly()
        {
            // Arrange
            var foo1 = new Foo
            {
                Id = 1,
                Name = "testName",
                Date = DateTime.Now
            };

            // Act
            var result = _mapper.Map<Foo, Bar>(foo1);

            // Assert
            Assert.AreEqual(foo1.Id, result.Id);
            Assert.AreEqual(foo1.Name, result.Name);
            Assert.AreEqual(foo1.Date, result.Date);
        }

        [TestMethod]
        public void Should_Map_Suitable_Members()
        {
            // Arrange
            var foo1 = new FooSpecial
            {
                NormalField = "test",
                NormalProp = "test",
                FieldReadonly = 1,
                PropWithoutSetter = 1,
                PropWithPrivateSetter = 1
            };

            // Act
            var result = _mapper.Map<FooSpecial, BarSpecial>(foo1);

            // Assert
            Assert.AreEqual(foo1.NormalField, result.NormalField);
            Assert.AreEqual(foo1.NormalProp, result.NormalProp);
            Assert.AreNotEqual(foo1.FieldReadonly, result.FieldReadonly);
            Assert.AreNotEqual(foo1.PropWithoutSetter, result.PropWithoutSetter);
            Assert.AreNotEqual(foo1.PropWithPrivateSetter, result.PropWithPrivateSetter);
        }

        [TestMethod]
        public void Should_Throw_WeirdMapperException()
        {
            // Arrange
            var bar1 = new Bar
            {
                Id = 1,
                Name = "testName",
                Date = DateTime.Now
            };

            // Act/Assert
            Assert.ThrowsException<WeirdMapperException>(() => { _mapper.Map<Bar, Foo>(bar1); });
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

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
        [SuppressMessage("ReSharper", "UnassignedReadonlyField")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public class BarSpecial
        {
            public int PropWithoutSetter { get; }
            public int PropWithPrivateSetter { get; private set; }
            public readonly int FieldReadonly;
            public string NormalProp { get; set; }
            public string NormalField;
        }

        public class FooSpecial
        {
            public int PropWithoutSetter { get; set; }
            public int PropWithPrivateSetter { get; set; }
            public int FieldReadonly;
            public string NormalProp { get; set; }
            public string NormalField;
        }
    }
}
