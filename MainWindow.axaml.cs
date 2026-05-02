using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace YT_DLP_GuiWrapper;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        if (OperatingSystem.IsWindows()) 
        {
            WindowsMenu.IsVisible = true;
            this.Height = 450;
        }

        Task.Run(GetInstalledVersion);
    }

    private string? ytdlpPath;

    private static string? getYTDLP()
    {
        if (OperatingSystem.IsMacOS())
        {
            string[] pathes =
            {
                "/usr/local/bin/yt-dlp", //Intel
                "/opt/homebrew/bin/yt-dlp" //Apple Silicon
            };

            foreach (string path in pathes)
                if (File.Exists(path))
                    return path;
        }
        else if (OperatingSystem.IsWindows())
        {
            string[] pathes =
            {
                @"C:\Program Files\yt-dlp\yt-dlp.exe",
                @"C:\ProgramData\chocolatey\bin\yt-dlp.exe",
                "yt-dlp.exe"
            };

            foreach (string path in pathes)
                if (File.Exists(path))
                    return path;
        }

        return null;
    }

    private static string? getFFMPEG()
    {
        if (OperatingSystem.IsMacOS())
        {
            string[] pathes =
            {
                "/usr/local/bin/ffmpeg", //Intel
                "/opt/homebrew/bin/ffmpeg" //Apple Silicon
            };

            foreach (string path in pathes)
                if (File.Exists(path))
                    return path;
        }
        else if (OperatingSystem.IsWindows())
        {
            string[] pathes =
            {
                @"C:\Program Files\ffmpeg\bin\ffmpeg.exe",
                @"C:\ProgramData\chocolatey\bin\ffmpeg.exe",
                "ffmpeg.exe"
            };

            foreach (string path in pathes)
                if (File.Exists(path))
                    return path;
        }

        return null;
    }

    private void GetInstalledVersion()
    {
        ytdlpPath = getYTDLP();

        if (ytdlpPath != null)
        {
            var psi = new ProcessStartInfo
            {
                FileName               = ytdlpPath,
                Arguments              = "--version",
                UseShellExecute        = false,
                RedirectStandardOutput = true,
                CreateNoWindow         = true,
            };

            using var p = Process.Start(psi)!;
            var version = p.StandardOutput.ReadToEndAsync();
            p.WaitForExitAsync();

            Dispatcher.Invoke(() =>
            {
                YtdlpVersion.Text = "yt-dlp " + version.Result.Trim();
                Link.IsEnabled = true;
                foreach (RadioButton RB in OutputFormatContainer.Children)
                    RB.IsEnabled = true;
                foreach (RadioButton RB in OutputBitrateContainer.Children)
                    RB.IsEnabled = true;
                MyButton.IsEnabled = true;
            });

            if (getFFMPEG() == null)
            {
                Dispatcher.Invoke(async () =>
                {
                    await ShowMessageAsync("Sorry", "FFMPEG not installed !\n\nbrew install ffmpeg");
                });
            }
        }
        else
        {
            Dispatcher.Invoke(async () =>
            {
                await ShowMessageAsync("Sorry", "YT-DLP not installed !\n\nbrew install yt-dlp");
            });
        }        
    }

    public async Task ShowMessageAsync(string title, string message)
    {
        var dialog = new Window
        {
            Title                 = title,
            Width                 = 300,
            Height                = 130,
            CanResize             = false,
            ShowInTaskbar         = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content               = new StackPanel
            {
                Margin   = new Avalonia.Thickness(20),
                Spacing  = 16,
                Children =
                {
                    new TextBlock { Text = message, TextWrapping = Avalonia.Media.TextWrapping.Wrap },
                    new Button    { Content = "OK", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center }
                }
            }
        };

        var btn = ((StackPanel)dialog.Content).Children[1] as Button;
        btn!.Click += (s, e) => {
            if (title == "Sorry")
                Environment.Exit(0);
            else
                dialog.Close();
        };

        await dialog.ShowDialog(this);
    }

    public void showLogs()
    {
        LogsPanel.IsVisible = !LogsPanel.IsVisible;

        SizeToContent = LogsPanel.IsVisible
            ? SizeToContent.Height
            : SizeToContent.Manual;

        if (!LogsPanel.IsVisible && OperatingSystem.IsWindows())
            this.Height = 450;
        else
            this.Height = 420;
    }

    private string DefaultFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
    private CancellationTokenSource? _cts;
    private string? SelectedFormat;
    private string? SelectedBitrate; 

    private async void Button_Click(object? sender, RoutedEventArgs e)
    {
        if ((string?)MyButton.Tag != "downloading")
        {
            _cts = new CancellationTokenSource();

            if (string.IsNullOrEmpty(Link.Text) || string.IsNullOrWhiteSpace(Link.Text))
                return;

            
            MyButton.Tag = "downloading";

            ProgressLabel.Text = "Fetching data...";

            string downloadPath;

            /*if (!string.IsNullOrEmpty(FolderName.Text) && !string.IsNullOrWhiteSpace(FolderName.Text))
                downloadPath = Path.Combine(DefaultFolder, FolderName.Text);
            else
                downloadPath = DefaultFolder;*/

            //DownloadPath

            int totalTracks = await CountTracksAsync(Link.Text, _cts.Token);
            //Console.WriteLine($"Number of tracks : {totalTracks}");

            if (totalTracks > 1)
            {
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName               = ytdlpPath,
                        Arguments              = $"--print \"%(playlist_title)s\" --print \"%(artist)s\" --playlist-items 1 \"{Link.Text}\"",
                        UseShellExecute        = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError  = true,
                        CreateNoWindow         = true,
                    };

                    using var process = new Process { StartInfo = psi };
                    process.Start();

                    var output = await process.StandardOutput.ReadToEndAsync(_cts.Token);
                    await process.WaitForExitAsync(_cts.Token);

                    var lines = output.Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries);

                    downloadPath = System.IO.Path.Combine(DefaultFolder, lines[1].Trim() + " - " + lines[0].Trim().Replace("Album - ", ""));

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    ProgressLabel.Text = "An error occured";
                    downloadPath = DefaultFolder;
                }
            }
            else
            {
                downloadPath = DefaultFolder;
            }
        
            SelectedFormat = FormatFlac.IsChecked == true ? "flac" : FormatWav.IsChecked  == true ? "wav"  : "mp3";

            SelectedBitrate = Bitrate128.IsChecked == true ? "128k" : Bitrate320.IsChecked == true ? "320k" : "192k";

            DownloadState.Value = 0;

            var progress = new Progress<double>(pct =>
            {
                DownloadState.Value = pct;
                ProgressLabel.Text = $"{pct:0.0}%";
            });

            try
            {
                DownloadState.Value = 1;
                ProgressLabel.Text = "Starting...";

                await DownloadAudioAsync(
                    url:          Link.Text,
                    outputFolder: downloadPath,
                    format:       SelectedFormat,
                    bitrate:      SelectedBitrate,
                    tracks:       totalTracks,
                    progress:     progress,
                    ct:           _cts.Token);

                DownloadState.Value = 100;
                ProgressLabel.Text = "Finished";
            }
            catch (OperationCanceledException)
            {
                DownloadState.Value = 0;
                ProgressLabel.Text = "Canceled";
                MyButton.Tag = null;
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.ToString());
                AppendLog(ex.ToString());
                DownloadState.Value = 0;
                ProgressLabel.Text = "An error occured";
            }
            finally
            {
                MyButton.Tag = null;
            }
        }
        else
        {
            _cts?.Cancel();
            MyButton.Tag = null;
        }
    }

    private async Task DownloadAudioAsync(
    string url,
    string outputFolder,
    string format,
    string bitrate,
    int tracks,
    IProgress<double> progress,
    CancellationToken ct = default)
    {
        var template = System.IO.Path.Combine(outputFolder, "%(artist)s - %(title)s.%(ext)s");
        var args = $"-x --audio-format {format} --audio-quality {bitrate} " +
                $"--newline -o \"{template}\" \"{url}\"";

        var psi = new ProcessStartInfo
        {
            FileName               = ytdlpPath,
            Arguments              = args,
            UseShellExecute        = false,
            RedirectStandardOutput = true,
            RedirectStandardError  = true,
            CreateNoWindow         = true,
        };

        using var process = new Process { StartInfo = psi };

        int  currentTrack = 1; //single title
        bool inFfmpeg     = false;

        ct.Register(() =>
        {
            try { process.Kill(entireProcessTree: true); }
            catch { /* process already killed */ }
        });

        process.OutputDataReceived += (s, e) => 
        {
            if (e.Data is null) return;
            var line = e.Data;

            AppendLog(line);

            //Console.WriteLine($"[yt-dlp] {line}");

            if (line.Contains("Downloading item"))
            {
                var m = System.Text.RegularExpressions.Regex.Match(
                    line, @"Downloading item (\d+) of (\d+)");
                if (m.Success)
                {
                    currentTrack = int.Parse(m.Groups[1].Value);
                    tracks       = int.Parse(m.Groups[2].Value);
                    inFfmpeg     = false;
                }
                return;
            }

            if (line.Contains("[ExtractAudio]") || line.Contains("[ffmpeg]"))
            {
                if (!inFfmpeg)
                {
                    inFfmpeg = true;
                    progress.Report(TrackProgress(currentTrack, tracks, 90));
                }
                return;
            }

            if (!inFfmpeg)
            {
                var pct = ParsePercent(line);
                if (pct.HasValue)
                    progress.Report(TrackProgress(currentTrack, tracks, pct.Value * 0.85));
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync(ct);

        if (process.ExitCode != 0 && !ct.IsCancellationRequested)
            throw new Exception($"yt-dlp failed (exit {process.ExitCode})");
    }

    private void AppendLog(string line)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            LogsContent.Text += line + "\n";

            LogsScroll.ScrollToEnd();
        });
    }

    private static double TrackProgress(int currentTrack, int total, double trackPct)
    {
        if (total <= 0) return trackPct;
        double trackSize  = 100.0 / total;
        double baseOffset = (currentTrack - 1) * trackSize;
        return baseOffset + trackPct / 100.0 * trackSize;
    }

    /// Get number of track
    private async Task<int> CountTracksAsync(string url, CancellationToken ct)
    {
        var psi = new ProcessStartInfo
        {
            FileName               = ytdlpPath,
            Arguments              = $"--flat-playlist --print id \"{url}\"",
            UseShellExecute        = false,
            RedirectStandardOutput = true,
            RedirectStandardError  = true,
            CreateNoWindow         = true,
        };

        using var process = new Process { StartInfo = psi };
        process.Start();

        var output = await process.StandardOutput.ReadToEndAsync(ct);
        await process.WaitForExitAsync(ct);

        var count = output.Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries).Length;
        return Math.Max(count, 1);
    }

    private static double? ParsePercent(string line)
    {
        if (!line.Contains("[download]")) return null;

        var match = System.Text.RegularExpressions.Regex.Match(line, @"(\d+\.?\d*)%");
        if (match.Success && double.TryParse(
                match.Groups[1].Value,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out var pct))
            return pct;

        return null;
    }
    
    private void Menu_Quit_Click(object? sender, RoutedEventArgs e)
    => Environment.Exit(0);

    private async void Menu_About_Click(object? sender, RoutedEventArgs e)
        => await ShowMessageAsync("About", "YT-DLP GuiWrapper\n\nMade by AxlRocket");

    private void Menu_Logs_Click(object? sender, RoutedEventArgs e)
        => showLogs();

}