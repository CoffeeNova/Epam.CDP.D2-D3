using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerStateManagement;

namespace PowerStateManagementTests
{
    [TestClass()]
    public class PowerStateManagementTests
    {
        private PowerStateManagement.PowerStateManagement _psMgmt;

        [TestInitialize]
        public void Init()
        {
            _psMgmt = new PowerStateManagement.PowerStateManagement();
        }

        [TestMethod()]
        public void GetLastSleepTimeTest()
        {
            //Act
            var result = _psMgmt.GetLastSleepTime();

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void GetLastWakeTimeTest()
        {
            //Act
            var result = _psMgmt.GetLastWakeTime();

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void GetSystemBatteryStateTest()
        {
            //Act
            var result = _psMgmt.GetSystemBatteryState();

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void GetSystemPowerInformationTest()
        {
            //Act
            var result = _psMgmt.GetSystemPowerInformation();

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void SetSleepStateTest()
        {
            //_psMgmt.SetSuspendState(PowerState.Sleep);
        }

        [TestMethod()]
        public void SetHibernateStateTest()
        {
            //_psMgmt.SetSuspendState(PowerState.Hibernate);
        }

        [TestMethod()]
        public void RemoveHibernationFileTest()
        {
            _psMgmt.ReserveHibernationFile(HiberFileAction.Remove);
        }

        [TestMethod()]
        public void ReserveHibernationFileTest()
        {
            _psMgmt.ReserveHibernationFile(HiberFileAction.Reserve);
        }
    }
}