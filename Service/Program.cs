using CoreWCF;
using CoreWCF.Configuration;
using Bastion.Contracts;
using Bastion.DataAccess;
using Bastion.Managers;
using Bastion.Service;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("Bastion")
    ?? throw new InvalidOperationException("appsettings.json has no Bastion connection string.");
int port = builder.Configuration.GetValue("Endpoint:NetTcpPort", ServiceEndpoints.DefaultNetTcpPort);

builder.WebHost.UseNetTcp(port);
builder.Services.AddServiceModelServices();
builder.Services.AddSingleton<ISqlConnectionFactory>(new SqlConnectionFactory(connectionString));
builder.Services.AddSingleton<IAccountDao, AccountDao>();
builder.Services.AddSingleton<IAccountManager, AccountManager>();

// PerCall: CoreWCF asks the container for one service per message.
builder.Services.AddTransient<AccountService>();

WebApplication app = builder.Build();

app.UseServiceModel(serviceModel =>
{
    serviceModel.AddService<AccountService>();
    serviceModel.AddServiceEndpoint<AccountService, IAccountService>(
        new NetTcpBinding(SecurityMode.None), ServiceEndpoints.AccountPath);
});

app.Run();
