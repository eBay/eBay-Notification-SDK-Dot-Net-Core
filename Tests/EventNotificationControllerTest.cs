using System;
using System.Net;
using EbayEventNotificationSDK.Client;
using EbayEventNotificationSDK.Controllers;
using EbayEventNotificationSDK.Models;
using EbayEventNotificationSDK.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EbayEventNotificationSDK.Tests
{
    public class EventNotificationControllerTest
    {
        private readonly EventNotificationController controller;

        private readonly Mock<ILogger<EventNotificationController>> logger = new Mock<ILogger<EventNotificationController>>();

        private readonly ISignatureValidator signatureValidator;

        private readonly IEndPointValidator endpointValidator;

        private readonly Mock<IPublicKeyClient> publicKeyMock = new Mock<IPublicKeyClient>();

        private readonly Mock<IConfig> configMock = new Mock<IConfig>();

        private readonly IPublicKeyCache publicKeyCache;

        private readonly Mock<ILogger<SignatureValidatorImpl>> signatureLoggerMock = new Mock<ILogger<SignatureValidatorImpl>>();

        public EventNotificationControllerTest()
        {
            publicKeyCache = new PublicKeyCacheImpl(publicKeyMock.Object);
            signatureValidator = new SignatureValidatorImpl(publicKeyCache, signatureLoggerMock.Object);
            endpointValidator = new EndpointValidatorImpl(configMock.Object);
            controller = new EventNotificationController(logger.Object, signatureValidator, endpointValidator);
            publicKeyMock.Setup(x => x.getPublicKey(It.IsAny<string>())).Returns(DataProvider.getMockPublicKeyResponse());

        }

        [Fact]
        public void testPayloadProcessingSuccess()
        {
            ActionResult result = controller.process(DataProvider.getMockMessage(), DataProvider.getMockXEbaySignatureHeader());
            StatusCodeResult objectResponse = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.NoContent, objectResponse.StatusCode);
        }

        [Fact]
        public void testPayLoadVerificationFailure()
        {
            ActionResult result = controller.process(DataProvider.getMockTamperedMessage(), DataProvider.getMockXEbaySignatureHeader());
            StatusCodeResult objectResponse = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.PreconditionFailed, objectResponse.StatusCode);

        }

        [Fact]
        public void testVerification()
        {
            string challengeCode = "a8628072-3d33-45ee-9004-bee86830a22d";
            string verificationToken = "71745723-d031-455c-bfa5-f90d11b4f20a";
            string endpoint = "http://www.testendpoint.com/webhook";
            string expectedResult = "ca527df75caa230092d7e90484071e8f05d63068f1317973d6a3a42593734bbb";
            configMock.Setup(x => x.endpoint).Returns(endpoint);
            configMock.Setup(x => x.verificationToken).Returns(verificationToken);
            ActionResult<ChallengeResponse> result = controller.validate(challengeCode);
            Assert.Equal(expectedResult, result.Value.challengeResponse);
        }
    }
}
