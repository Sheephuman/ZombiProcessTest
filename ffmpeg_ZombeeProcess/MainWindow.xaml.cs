using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

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
            // MainWindowを閉じたときにアプリケーション全体を終了するように設定 ゾンビプロセス化しないために必要



        }
        Thread th1 = null!;
        private void RunSyncButton_Click(object sender, RoutedEventArgs e)
        {
            RunFfmpeg(OutputTextBox);

            th1 = new Thread(() => RunFfmpeg(OutputTextBox));

            th1.IsBackground = false;


            //falseでフォアグラウンド動作。


            // ここでffmpegの実行を行う
            th1.Start();
            //Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            //　わざとShutDownModeを変える　→　Close()でゾンビ化
        }

        private async void RunFfmpegAsync()
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

            process = new Process();

            process.StartInfo = startInfo;
            process.EnableRaisingEvents = true;

            // 標準出力とエラー出力のコールバックを設定



            process.ErrorDataReceived += (s, ev) =>
            {
                if (ev.Data != null)
                {


                    Dispatcher.InvokeAsync(() =>
                   {

                       OutputTextBox.AppendText(ev.Data + Environment.NewLine);
                       OutputTextBox.ScrollToEnd(); // テキストボックスをスクロールして最新の出力を表示
                   });
                    _cts.Cancel(); // キャンセルトークンをキャンセル
                    Task.Delay(100); // 適切な遅延を入れることでUIの更新をスムーズにする
                }
            };

            // Exitedイベント（問題を引き起こす可能性のある実装）
            process.Exited += (s, ev) =>
            {
                // WaitForExitを呼ばない（バッファが残る可能性）
                // 終了を待つ（キャンセルトークンを使用）
                ///ゾンビプロセス化しない
                //_cts.Cancel();



                Dispatcher.InvokeAsync(() => MessageBox.Show("ffmpeg process exited."));
            };

            // プロセス開始
            process.Start();

            process.BeginErrorReadLine();




            try
            {
                await process.WaitForExitAsync(); // キャンセルトークンを使用して非同期に待機


            }
            catch
            {
                // 例外を無視（問題を悪化させる）
            }
        }





        private void RunFfmpeg(TextBox textBox)
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

            //usingブロックを使用するとプロセスがそのScope内で解放されてしまうため、usingは使用しない
            //→ゾンビ化のおそれ
            process = new Process();


            process.StartInfo = startInfo;
            process.EnableRaisingEvents = true;

            // 標準出力とエラー出力のコールバックを設定



            process.ErrorDataReceived += (s, ev) =>
            {
                if (ev.Data != null)
                {

                    //textBoxのインスタンスで遅延実行
                    Dispatcher.InvokeAsync(() =>
                      {

                          textBox.AppendText(ev.Data + Environment.NewLine);
                          textBox.ScrollToEnd();



                          // テキストボックスをスクロールして最新の出力を表示
                      });
                    //    var waitHandle = new ManualResetEvent(false);

                    //  waitHandle.WaitOne(0); // キャンセルトークンをキャンセル


                    //Task.Delay(100); // 適切な遅延を入れることでUIの更新をスムーズにする
                }
            };

            // Exitedイベント（問題を引き起こす可能性のある実装）
            process.Exited += (s, ev) =>
            {
                // WaitForExitを呼ばない（バッファが残る可能性）

                Dispatcher.Invoke(() => MessageBox.Show("ffmpeg process exited."));
            };

            // プロセス開始
            process.Start();

            process.BeginErrorReadLine();



            // 終了を待つ（キャンセルトークンを使用）

        }


        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (process is null)
                    return;




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

        private void AsyncStartButton_Click(object sender, RoutedEventArgs e)
        {
            th1 = new Thread(() => RunFfmpegAsync());
            th1.IsBackground = false;
            //falseでフォアグラウンド動作。


            // ここでffmpegの実行を行う
            th1.Start();

        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            //process.Kill(); // プロセスを強制終了
            //th1.Join(); // スレッドの終了を待機
            Application.Current.Shutdown();
        }
    }
}
