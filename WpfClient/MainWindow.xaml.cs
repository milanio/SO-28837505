using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfClient.ServiceReference1;

namespace WpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WpfTextBoxLogger _logger;


        public class WpfTextBoxLogger : ITextBoxLogger
        {
            private readonly TextBox _textBox;

            public WpfTextBoxLogger(TextBox textBox)
            {
                _textBox = textBox;
            }

            public void WriteLine(string format, params object[] args)
            {
                var value = string.Format(format, args);
                if (string.IsNullOrEmpty(_textBox.Text)) _textBox.Text = value + "\r\n";
                else
                {
                    _textBox.Text += value + "\r\n";
                }

            }
        }

        class SchedulerServiceCallback : ISchedulerServiceCallback
        {
            private readonly ITextBoxLogger _textBoxLogger;

            public SchedulerServiceCallback(ITextBoxLogger textBoxLogger)
            {
                _textBoxLogger = textBoxLogger;
            }

            public void StatusUpdate(SchedulerStatus status)
            {
                Thread.Sleep(1000);
                _textBoxLogger.WriteLine("Thread {0} received callback at {2} ", Thread.CurrentThread.ManagedThreadId, status, GetCurrentTime());
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            _logger = new WpfTextBoxLogger(TbLog);
        }

        private async void ButtonAsyncStop_Click(object sender, RoutedEventArgs e)
        {

            var callback = new SchedulerServiceCallback(_logger);
            var ssc = new SchedulerServiceClient(new InstanceContext(callback));
            try
            {
                _logger.WriteLine("Thread {0} - Calling SubscribeStatusUpdate at {1}",
                    Thread.CurrentThread.ManagedThreadId, GetCurrentTime());

                ssc.SubscribeStatusUpdate();
                _logger.WriteLine("Thread {0} - Calling Stop at {1}", Thread.CurrentThread.ManagedThreadId,
                    GetCurrentTime());

                await ssc.StopAsync();

                _logger.WriteLine("Thread {0} - Stop processing has finished at {1}.",
                    Thread.CurrentThread.ManagedThreadId, GetCurrentTime());
            }
            finally
            {
                ssc.Close();
            }

        }

        static string GetCurrentTime()
        {
            return DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
        }

        private void ButtonTask_Click(object sender, RoutedEventArgs e)
        {
            var callback = new SchedulerServiceCallback(_logger);
            var ssc = new SchedulerServiceClient(new InstanceContext(callback));
            _logger.WriteLine("Thread {0} - Calling SubscribeStatusUpdate at {1}",
                Thread.CurrentThread.ManagedThreadId, GetCurrentTime());

            ssc.SubscribeStatusUpdate();
            _logger.WriteLine("Thread {0} - Calling Stop at {1}", Thread.CurrentThread.ManagedThreadId,
                GetCurrentTime());

            ssc.StopAsync().ContinueWith(t => Dispatcher.BeginInvoke(new Action(() =>
            {
                _logger.WriteLine("Thread {0} - Stop processing has finished at {1}.",
                    Thread.CurrentThread.ManagedThreadId, GetCurrentTime());
                ssc.Close();
            })));

        }

        private void ButtonSyncStop_Click(object sender, RoutedEventArgs e)
        {
            var callback = new SchedulerServiceCallback(_logger);
            var ssc = new SchedulerServiceClient(new InstanceContext(callback));
            try
            {
                _logger.WriteLine("Thread {0} - Calling SubscribeStatusUpdate at {1}",
                    Thread.CurrentThread.ManagedThreadId, GetCurrentTime());

                ssc.SubscribeStatusUpdate();
                _logger.WriteLine("Thread {0} - Calling Stop at {1}", Thread.CurrentThread.ManagedThreadId,
                    GetCurrentTime());

                ssc.Stop();
                _logger.WriteLine("Thread {0} - the call to Stop has finished at {1}. ",
                    Thread.CurrentThread.ManagedThreadId, GetCurrentTime());
            }
            catch (Exception exc)
            {
                _logger.WriteLine("Probable Deadlock : {0} at {1}", exc,
                Thread.CurrentThread.ManagedThreadId, GetCurrentTime());
            }

        }


    }

    public interface ITextBoxLogger
    {
        void WriteLine(string format, params object[] args);

    }
}
