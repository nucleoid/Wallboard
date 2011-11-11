using AutoMapper;
using MbUnit.Framework;
using Wallboard.Automapper;

namespace LoggingServer.Tests.Interface.Automapper
{
    [TestFixture]
    public class AutomapperConfigTest
    {
        [Test]
        public void TestMappings()
        {
            //Act
            AutomapperConfig.Setup();

            //Assert
            Mapper.AssertConfigurationIsValid();
        }
    }
}
