using Authentication.Application.Services.Authentication;

namespace Authentication.UnitTests.Application.Usecases.Authentication.Command.Register;

public class RegisterQueryHandlerTest
{
    private readonly Mock<IAuthenticationService> _mockRepository = new();
    private readonly Mock<IMapper> _mockMapper = new();
}