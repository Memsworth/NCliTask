using CliFx;
using CliFx.Binding;
using CliFx.Infrastructure;
using CliWrap;
using CliWrap.Buffered;

[Command("Install", Description = "Create your web apis")]
public partial class InstallCommand : ICommand
{
    [CommandOption("manual", 'm', Description = "Set service startup type to Manual (default is Automatic).")]
    public bool Manual { get; set; } = false;

    [CommandOption("path1", 'p', Description = "First web api")]
    public required string Path1 { get; set; }
    [CommandOption("path2", 'q', Description = "Second web api")]
    public required string Path2 { get; set; }

    [CommandOption("display-name1", 'd', Description = "service name for the first webapi")]
    public string ServiceName1 { get; set; } = "NAPI1";

    [CommandOption("display-name2", 'e', Description = "service name for the first webapi")]
    public string ServiceName2 { get; set; } = "NAPI2";
    public async ValueTask ExecuteAsync(IConsole console)
    {
        var webApiFile1 = Directory.GetFiles(Path1, "*.exe", SearchOption.TopDirectoryOnly);
        var webApiFile2 = Directory.GetFiles(Path2, "*.exe", SearchOption.TopDirectoryOnly);

        if (webApiFile1.Length == 0 || webApiFile2.Length == 0)
        {
            throw new CliFx.CommandException($"No executable found in '{Path1}' or in {Path2}", exitCode: 1);
        }

        var exe1 = webApiFile1[0];
        var exe2 = webApiFile2[0];

        var isServiceName1ExistQuery1 = await SingleExcuteQuery(ServiceName1, "query");
        var isServiceName1ExistQuery2 = await SingleExcuteQuery(ServiceName2, "query");

        if (isServiceName1ExistQuery1 is null || isServiceName1ExistQuery2 is null)
        {
            throw new CliFx.CommandException($"No executable found in '{Path1}' or in {Path2}", exitCode: 1);
        }

        var serviceNameOutput = isServiceName1ExistQuery1.StandardOutput + isServiceName1ExistQuery1.StandardError + isServiceName1ExistQuery2.StandardOutput + isServiceName1ExistQuery2.StandardError;
        var isServiceExist = !(serviceNameOutput).Contains("FAILED 1060", StringComparison.OrdinalIgnoreCase);

        if (isServiceExist)
        {
            var stopServiceOneResult = await SingleExcuteQuery(ServiceName1, "stop");
            var stopServiceTwoResult = await SingleExcuteQuery(ServiceName2, "stop");
            var deleteServiceOneResult = await SingleExcuteQuery(ServiceName1, "delete");
            var deleteServiceTwoResult = await SingleExcuteQuery(ServiceName2, "delete");

            if (stopServiceOneResult is null || stopServiceTwoResult is null || deleteServiceOneResult is null || deleteServiceTwoResult is null)
            {
                throw new CliFx.CommandException($"No executable found in '{Path1}' or in {Path2}", exitCode: 1);
            }
        }
        var startupType = Manual ? "demand" : "auto";
        var serviceOneCreateResult = await SingleExcuteQuery(ServiceName1, exe1, startupType);
        var serviceTwoCreateResult = await SingleExcuteQuery(ServiceName2, exe2, startupType);

        if (serviceOneCreateResult is null || serviceTwoCreateResult is null)
        {
            throw new CliFx.CommandException($"No executable found in '{Path1}' or in {Path2}", exitCode: 1);
        }

        if (serviceOneCreateResult.ExitCode != 0 || serviceTwoCreateResult != 0)
        {
            throw new CliFx.CommandException($"No executable found in '{Path1}' or in {Path2}", exitCode: 1);
        }

        var startServiceOneResult = await SingleExcuteQuery(ServiceName1, "start");
        var startServiceTwoResult = await SingleExcuteQuery(ServiceName2, "start");
    }



    private async Task<BufferedCommandResult?> SingleExcuteQuery
        (string serviceName, string path, string startupType, string query = "create")
    {
        return await Cli.Wrap("sc").WithArguments(
        $"{query} {serviceName} binPath= \"{path}\" DisplayName= \"{serviceName}\" start= {startupType.ToLower()}")
        .WithValidation(CommandResultValidation.None).ExecuteBufferedAsync();
    }
    private async Task<BufferedCommandResult?> SingleExcuteQuery(string serviceName, string query)
    {
        return await Cli.Wrap("sc").WithArguments([query, serviceName])
        .WithValidation(CommandResultValidation.None).ExecuteBufferedAsync();
    }
}