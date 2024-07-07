using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace HealthCheck;

public class HealthController
{
    [LambdaFunction]
    [RestApi(LambdaHttpMethod.Get, "/")]
    public IHttpResult Check(ILambdaContext context)
    {
        context.Logger.LogInformation("Handling the 'Get' Request");

        return HttpResults.Ok("Hello AWS Serverless");
    }
}