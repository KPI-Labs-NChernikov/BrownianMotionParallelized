using System.Globalization;
using Cells1;
using Microsoft.Extensions.Configuration;

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
IConfiguration configuration = new ConfigurationBuilder()
    .AddCommandLine(args)
    .Build();

var n = configuration.Parse("n", 5, int.TryParse);
var k = configuration.Parse("k", 10, int.TryParse);
var p = configuration.Parse("p", 0.5d, double.TryParse);
Console.WriteLine($"N = {n}, K = {k}, p = {p}");

var crystal = new Crystal1D(n, k, p);
Presenter.ShowCrystal(crystal);

var cancellationTokenSource = new CancellationTokenSource();
var modellingTime = TimeSpan.FromSeconds(60);
var motionTask = crystal.StartBrownianMotion(modellingTime, cancellationTokenSource.Token);

await HandleShowTimer(crystal, modellingTime);

//cancellationTokenSource.Cancel();     // Necessary only for Timeout.InfiniteTimeSpan
await motionTask;

return;

async Task HandleShowTimer(Crystal1D crystal1D, TimeSpan totalTime)
{
    var period = TimeSpan.FromSeconds(1);
    var timerState = new TimerState();
    var timer = new Timer(
        s =>
        {
            Presenter.ShowCrystal(crystal1D);
            Interlocked.Increment(ref ((TimerState)s!).Counter);
        },
        timerState,
        period,
        period);
    await Task.Delay(totalTime);
    
    timer.Dispose();
}
