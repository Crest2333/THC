using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Serilog;
using SkiaSharp;
using System.Configuration;
using System.Data;
using System.Windows;

namespace TWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            LiveCharts.Configure(config => config.HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter('汉')));
            Log.Logger = new LoggerConfiguration().WriteTo.File(";logs/log.txt", retainedFileCountLimit: 7,
                 rollingInterval: RollingInterval.Day)
                .MinimumLevel.Information()
                .CreateLogger();

            base.OnStartup(e);
        }
    }

}
