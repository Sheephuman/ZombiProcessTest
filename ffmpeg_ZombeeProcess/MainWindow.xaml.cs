using System.Diagnostics;
using System.IO;
using System.Windows;

namespace ffmpeg_ZombeeProcess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private CancellationTokenSource _cts = new CancellationTokenSource();
        Process process = null!; // Processオブジェクトをフィールドとして定義

        public MainWindow()
        {
            InitializeComponent();

            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

        }
        Thread th1 = null!;
        private void testButton_Click(object sender, RoutedEventArgs e)
        {
            th1 = new Thread(async () => await RunFfmpegAsync());
            th1.IsBackground = true;

            // ここでffmpegの実行を行う
            th1.Start();



        }
        private async Task RunFfmpegAsync()
        {

            var startInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = "-i test.mp4 -y output.mp4",
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (process = new Process())
            {
                process.StartInfo = startInfo;
                process.EnableRaisingEvents = true;

                // 標準出力とエラー出力のコールバックを設定



                process.ErrorDataReceived += async (s, ev) =>
                {
                    if (ev.Data != null)
                    {


                        await Dispatcher.InvokeAsync(() =>
                        {

                            OutputTextBox.AppendText(ev.Data + Environment.NewLine);
                            OutputTextBox.ScrollToEnd(); // テキストボックスをスクロールして最新の出力を表示
                        });

                        await Task.Delay(100); // 適切な遅延を入れることでUIの更新をスムーズにする
                    }
                };

                // Exitedイベント（問題を引き起こす可能性のある実装）
                process.Exited += async (s, ev) =>
                {
                    // WaitForExitを呼ばない（バッファが残る可能性）

                    await Dispatcher.InvokeAsync(() => MessageBox.Show("ffmpeg process exited."));
                };

                // プロセス開始
                process.Start();

                process.BeginErrorReadLine();



                // 終了を待つ（キャンセルトークンを使用）
                try
                {


                    await process.WaitForExitAsync();
                }
                catch
                {
                    // 例外を無視（問題を悪化させる）
                }
            }
        }



        private void RunFfmpeg()
        {

            var startInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = "-i test.mp4 -y output.mp4",
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (process = new Process())
            {
                process.StartInfo = startInfo;
                process.EnableRaisingEvents = true;

                // 標準出力とエラー出力のコールバックを設定



                process.ErrorDataReceived += async (s, ev) =>
                {
                    if (ev.Data != null)
                    {

                        // バッファに追加（スレッドセーフでない）
                        await Dispatcher.InvokeAsync(() =>
                           {

                               OutputTextBox.AppendText(ev.Data + Environment.NewLine);
                               OutputTextBox.ScrollToEnd(); // テキストボックスをスクロールして最新の出力を表示
                           });

                        await Task.Delay(100); // 適切な遅延を入れることでUIの更新をスムーズにする
                    }
                };

                // Exitedイベント（問題を引き起こす可能性のある実装）
                process.Exited += async (s, ev) =>
                {
                    // WaitForExitを呼ばない（バッファが残る可能性）

                    await Dispatcher.InvokeAsync(() => MessageBox.Show("ffmpeg process exited."));
                };

                // プロセス開始
                process.Start();

                process.BeginErrorReadLine();



                // 終了を待つ（キャンセルトークンを使用）
                try
                {


                    //process.WaitForExitAsync();
                }
                catch
                {
                    // 例外を無視（問題を悪化させる）
                }
            }
            // usingブロックを抜けた後、プロセスが完全に解放されない可能性
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StreamWriter inputWriter = process.StandardInput;


                inputWriter.WriteLine("q");
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Error stopping ffmpeg: {ex.Message}");
            }
            //inputWriter.Dispose();
            //解放させない
        }
    }
}
