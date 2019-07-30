using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Rhisis.Core.Common;
using Rhisis.Core.Structures.Configuration;
using Rhisis.Database;
using Rhisis.Database.Entities;
using Rhisis.Database.Repositories;
using Rhisis.Login.Client;
using Rhisis.Login.Core;
using Rhisis.Login.Packets;
using Rhisis.Network.Packets;
using Rhisis.Network.Packets.Login;
using Sylver.HandlerInvoker;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Rhisis.Login.Tests
{
    public class LoginHandlerTest
    {
        private const string BuildVersion = "20072019";
        private const string IncorrectBuildVersion = "23462378";
        private readonly LoginConfiguration _loginConfiguration = new LoginConfiguration
        {
            BuildVersion = BuildVersion
        };
        private readonly LoginClient _loginClient = new LoginClient();

        private readonly Mock<ILogger<LoginHandler>> _loggerMock;
        private readonly Mock<ILoginServer> _loginServerMock;
        private readonly Mock<ICoreServer> _coreServerMock;
        private readonly Mock<IDatabase> _database;
        private readonly Mock<IUserRepository> _databaseUserRepository;
        private readonly Mock<ILoginPacketFactory> _loginPacketFactoryMock;
        private readonly Mock<IOptions<LoginConfiguration>> _loginConfigurationMock;
        private readonly LoginHandler _loginHandler;

        private readonly List<DbUser> _users;

        public LoginHandlerTest()
        {
            this._users = new UserGenerator().Generate(100);

            this._loggerMock = new Mock<ILogger<LoginHandler>>();
            this._loginServerMock = new Mock<ILoginServer>();
            this._coreServerMock = new Mock<ICoreServer>();
            this._loginPacketFactoryMock = new Mock<ILoginPacketFactory>();

            this._loginConfigurationMock = new Mock<IOptions<LoginConfiguration>>();
            this._loginConfigurationMock.Setup(x => x.Value).Returns(this._loginConfiguration);

            this._databaseUserRepository = new Mock<IUserRepository>();
            this._databaseUserRepository.Setup(x => x.GetUser(It.IsAny<string>()))
                .Returns<string>(x => this._users.AsQueryable().FirstOrDefault(y => y.Username == x));

            this._database = new Mock<IDatabase>();
            this._database.SetupGet(x => x.Users).Returns(this._databaseUserRepository.Object);

            this._loginHandler = new LoginHandler(
                this._loggerMock.Object,
                this._loginConfigurationMock.Object,
                this._loginServerMock.Object,
                this._database.Object,
                this._coreServerMock.Object,
                this._loginPacketFactoryMock.Object);

            this._loginClient.Initialize(this._loginServerMock.Object,
                new Mock<ILogger<LoginClient>>().Object,
                new Mock<IHandlerInvoker>().Object,
                this._loginPacketFactoryMock.Object);
        }

        [Fact]
        public void OnCertifyFailedWithIncorrectBuildVersionTest()
        {
            var certifyPacket = new Mock<CertifyPacket>(this._loginConfigurationMock.Object);
            certifyPacket.SetupGet(x => x.BuildVersion).Returns(IncorrectBuildVersion);

            this._loginHandler.OnCertify(this._loginClient, certifyPacket.Object);

            this._loginPacketFactoryMock.Verify(x => x.SendLoginError(this._loginClient, ErrorType.ILLEGAL_VER), Times.Once());
        }

        public static IEnumerable<object[]> UserData => new List<object[]>
        {
            new object[] { "Rhisis", "CorrectPassword", AuthenticationResult.Success, ErrorType.DEFAULT },
            new object[] { "Shade", "password", AuthenticationResult.AccountDeleted, ErrorType.ILLEGAL_ACCESS },
            new object[] { "Ivillis", "randompassword", AuthenticationResult.BadUsername, ErrorType.FLYFF_ACCOUNT },
            new object[] { "Bubble", "incorrect password", AuthenticationResult.BadPassword, ErrorType.FLYFF_PASSWORD },
        };

        [Theory]
        [MemberData(nameof(UserData))]
        public void OnCertifyTest(string username, string password, AuthenticationResult authenticationResult, ErrorType errorType)
        {
            var certifyPacket = new Mock<CertifyPacket>(this._loginConfigurationMock.Object);
            certifyPacket.SetupGet(x => x.BuildVersion).Returns(BuildVersion);
            certifyPacket.SetupGet(x => x.Username).Returns(username);
            certifyPacket.SetupGet(x => x.Password).Returns(password);

            this._loginHandler.OnCertify(this._loginClient, certifyPacket.Object);

            if (authenticationResult == AuthenticationResult.Success)
            {
                DbUser user = this._users.FirstOrDefault(x => x.Username == username && x.Password == password);

                this._databaseUserRepository.Verify(x => x.Update(user), Times.Once());
                this._database.Verify(x => x.Complete());

                Assert.NotNull(this._loginClient.Username);
                Assert.Equal(username, this._loginClient.Username);
            }
            else
            {
                Assert.Null(this._loginClient.Username);
                this._loginPacketFactoryMock.Verify(x => x.SendLoginError(this._loginClient, errorType), Times.Once());
            }
        }

        public static IEnumerable<object[]> PingPacketData => new List<object[]>
        {
            new object[] { 42, false },
            new object[] { 24, true }
        };

        [Theory]
        [MemberData(nameof(PingPacketData))]
        public void OnPingTest(int time, bool timeout)
        {
            var pingPacketMock = new Mock<PingPacket>();
            pingPacketMock.SetupGet(x => x.Time).Returns(time);
            pingPacketMock.SetupGet(x => x.IsTimeOut).Returns(timeout);

            this._loginHandler.OnPing(this._loginClient, pingPacketMock.Object);

            if (!timeout)
            {
                this._loginPacketFactoryMock.Verify(x => x.SendPong(this._loginClient, pingPacketMock.Object.Time));
            }
        }
    }
}
