using System;
using System.ServiceModel;
using Bastion.Contracts;

namespace Bastion.Presentation.Utils;

// Talks to the account server over net.tcp. The calls block the frame they
// are made on, which is fine for the three presses of a registration and
// would not be for a move: ASR-01 puts a move on its own path.
public sealed class ServiceAccountGateway : IAccountGateway
{
    private readonly EndpointAddress _address;

    public ServiceAccountGateway(string address)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(address);

        _address = new EndpointAddress(address);
    }

    public RegistrationAnswer Register(NewAccountRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var message = new RegisterRequest
        {
            Nickname = request.Nickname,
            Email = request.Email,
            Password = request.Password,
            BirthDate = request.BirthDate,
            Language = Resources.Language.Current.Name,
            AcceptsTerms = request.AcceptsTerms
        };

        return Ask(service => Translate(service.RegisterAsync(message).GetAwaiter().GetResult().Outcome));
    }

    public bool IsNicknameTaken(string nickname)
    {
        // A server that does not answer must not be read as a free nickname,
        // or the form would invite a name the server will refuse later.
        return Ask(service => service.IsNicknameTakenAsync(nickname).GetAwaiter().GetResult(), whenUnreachable: false);
    }

    public bool IsEmailTaken(string email)
    {
        return Ask(service => service.IsEmailTakenAsync(email).GetAwaiter().GetResult(), whenUnreachable: false);
    }

    private static RegistrationAnswer Translate(RegisterOutcome outcome)
    {
        return outcome switch
        {
            RegisterOutcome.Registered => RegistrationAnswer.Registered,
            RegisterOutcome.NicknameTaken => RegistrationAnswer.NicknameTaken,
            RegisterOutcome.EmailTaken => RegistrationAnswer.EmailTaken,
            RegisterOutcome.UnderageRejected => RegistrationAnswer.Underage,
            RegisterOutcome.TermsNotAccepted => RegistrationAnswer.TermsNotAccepted,
            _ => RegistrationAnswer.Rejected
        };
    }

    private RegistrationAnswer Ask(Func<IAccountService, RegistrationAnswer> call)
    {
        return Ask(call, RegistrationAnswer.Unreachable);
    }

    // One channel per call. A registration is rare and a channel left open
    // over a dropped link is worse than opening another one.
    private TAnswer Ask<TAnswer>(Func<IAccountService, TAnswer> call, TAnswer whenUnreachable)
    {
        var factory = new ChannelFactory<IAccountService>(new NetTcpBinding(SecurityMode.None), _address);

        try
        {
            return call(factory.CreateChannel());
        }
        catch (CommunicationException)
        {
            return whenUnreachable;
        }
        catch (TimeoutException)
        {
            return whenUnreachable;
        }
        finally
        {
            factory.Abort();
        }
    }
}
