using System;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Bastion.Contracts;
using Bastion.Managers;

namespace Bastion.Presentation.Utils;

// El manager visto desde el cliente: las mismas operaciones, alcanzadas por
// net.tcp. Un servidor que no contesta es un desenlace mas, no una excepcion
// que la pantalla tenga que atrapar.
public sealed class ServiceAccountManager : IAccountManager
{
    private readonly EndpointAddress _address;

    public ServiceAccountManager(string address)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(address);

        _address = new EndpointAddress(address);
    }

    public Task<RegistrationReceipt> RegisterAsync(
        AccountRegistration registration, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(registration);

        return Task.FromResult(Ask(
            service => service.RegisterAsync(registration).GetAwaiter().GetResult(),
            RegistrationReceipt.Refused(RegistrationOutcome.Unreachable)));
    }

    // Un servidor mudo no se lee como nickname libre, o el formulario
    // invitaria a un nombre que el servidor va a rechazar despues.
    public Task<bool> IsNicknameTakenAsync(string nickname, CancellationToken cancellation)
    {
        return Task.FromResult(Ask(
            service => service.IsNicknameTakenAsync(nickname).GetAwaiter().GetResult(), false));
    }

    public Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellation)
    {
        return Task.FromResult(Ask(
            service => service.IsEmailTakenAsync(email).GetAwaiter().GetResult(), false));
    }

    // Un canal por llamada. Un alta es rara y un canal abierto sobre un
    // enlace caido es peor que abrir otro.
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
