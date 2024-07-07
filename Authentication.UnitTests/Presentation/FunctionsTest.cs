// using Amazon.Lambda.Annotations.APIGateway;
// using Amazon.Lambda.TestUtilities;
// using Authentication.Presentation.Teste;
// using MediatR;
// using Moq;
// using Shared.Messages;
// using Xunit;
//
// namespace Authentication.UnitTests.Presentation;
//
// public class FunctionsTest
// {
//     public FunctionsTest()
//     {
//     }
//
//     [Fact]
//     public void TestGetMethod()
//     {
//         var iSender = new Mock<ISender>();
//         var iMessageHandlerService = new Mock<IMessageHandlerService>();
//         
//         var context = new TestLambdaContext();
//         var functions = new Functions(iMessageHandlerService.Object, iSender.Object);
//
//         var response = functions.Get(context);
//
//         Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
//
//         var serializationOptions = new HttpResultSerializationOptions { Format = HttpResultSerializationOptions.ProtocolFormat.RestApi };
//         var apiGatewayResponse = new StreamReader(response.Serialize(serializationOptions)).ReadToEnd();
//         Assert.Contains("Hello AWS Serverless", apiGatewayResponse);
//     }
// }
