using System.Timers;
using System.Windows;

namespace TimerZombiee
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


        }

        //string _arguments = "ffmpeg.exe -i \"test.mp4\" -c:v libx264 \"output.mp4\"";


        private System.Timers.Timer _timer = null!;
        private Thread _backgroundThread = null!;
        private bool _isRunning = true;

        private void StartBackgroundWork()
        {
            // バックグラウンドスレッドを開始
            _backgroundThread = new Thread(() =>
            {
                while (_isRunning)
                {
                    Console.WriteLine("Background thread running...");
                    Thread.Sleep(1000);
                }
            });
            _backgroundThread.IsBackground = false; // 意図的にフォアグラウンドスレッドにする
            _backgroundThread.Start();

            // タイマーを開始
            _timer = new System.Timers.Timer(500);
            _timer.Elapsed += TimerElapsed;
            _timer.AutoReset = true;
            _timer.Start();
        }

        private void TimerElapsed(object? sender, ElapsedEventArgs e)
        {
            // タイマーイベントで何か処理（例: ログ出力）
            Console.WriteLine("Timer ticked...");
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 意図的にリソース解放をしない
            // _isRunning = false;
            // _timer?.Stop();
            // _timer?.Dispose();
            //_backgroundThread?.Join();
        }





        private void testButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StartBackgroundWork();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


    }
}