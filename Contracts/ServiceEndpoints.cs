namespace Bastion.Contracts;

// Where the account server listens. It lives in the contract because both
// ends need the same address and neither should write it twice.
public static class ServiceEndpoints
{
    public const int DefaultNetTcpPort = 8089;

    public const string AccountPath = "/AccountService";

    public const string DefaultHost = "localhost";

    public static string GetAccountAddress()
    {
        return GetAccountAddress(DefaultHost, DefaultNetTcpPort);
    }

    public static string GetAccountAddress(string host, int port)
    {
        return $"net.tcp://{host}:{port}{AccountPath}";
    }
}
