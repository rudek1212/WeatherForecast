using Moq;
using Moq.Protected;

public static class HttpClientMockHelper
{
    public static Mock<HttpMessageHandler> CreateMessageHandlerMock(HttpResponseMessage responseMessage)
    {
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage)
            .Verifiable();

        return handlerMock;
    }
}