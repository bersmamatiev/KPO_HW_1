using System.Text;
using Microsoft.Extensions.DependencyInjection;
using VyshKat;
using VyshKat.Services;

Console.OutputEncoding = Encoding.UTF8;
using var provider = DependencyConfig.Build();
var fleet = provider.GetRequiredService<Fleet>();
var application = new ConsoleApplication(fleet, Console.In, Console.Out);

if (args.Contains("--demo"))
{
    application.RunDemo();
}
else
{
    application.Run();
}
