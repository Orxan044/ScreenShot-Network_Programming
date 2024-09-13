using System.Net.Sockets;
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.IO;

namespace Client;

public partial class MainWindow : Window
{

    public BitmapImage PhotoSource
    {
        get { return (BitmapImage)GetValue(PhotoSourceProperty); }
        set { SetValue(PhotoSourceProperty, value); }
    }

    public Dispatcher Dispatcher { get; set; } = Dispatcher.CurrentDispatcher;

    public static readonly DependencyProperty PhotoSourceProperty =
        DependencyProperty.Register("ImageBox", typeof(BitmapImage), typeof(MainWindow));

    public UdpClient server = new();
    public IPEndPoint connectEp = new(IPAddress.Loopback, 27001); 

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        BtnStart.IsEnabled = false;
        Task.Run(() => 
        {
            while (true)
            {
                try
                {

                    var bytes = server.Receive(ref connectEp);
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource.Read(bytes);
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    Dispatcher.Invoke(() => PhotoSource = bitmapImage);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                Task.Delay(5000).Wait();
            }
        
        
        });
    }
}











//public RelayCommand StartCommand
//{
//    get => new RelayCommand(() =>
//    {
//        Task.Run(() =>
//        {
//            while (true)
//            {
//                try
//                {
//                    Client = new();
//                    Client.Connect(IpAdressEndPoint);
//                    if (Client.Connected)
//                    {
//                        try
//                        {
//                            using (NetworkStream networkStream = Client.GetStream())
//                            {
//                                byte[] imageData = new byte[4096];
//                                int bytesRead;
//                                using (MemoryStream memoryStream = new MemoryStream())
//                                {

//                                    while ((bytesRead = networkStream.Read(imageData, 0, imageData.Length)) > 0)
//                                    {
//                                        memoryStream.Write(imageData, 0, bytesRead);
//                                    }


//                                    memoryStream.Seek(0, SeekOrigin.Begin);
//                                    BitmapImage bitmapImage = new BitmapImage();
//                                    bitmapImage.BeginInit();
//                                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
//                                    bitmapImage.StreamSource = memoryStream;
//                                    bitmapImage.EndInit();
//                                    bitmapImage.Freeze();

//                                    Dispatcher.Invoke(() => PhotoSource = bitmapImage);
//                                }
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            // Handle exceptions if any
//                            MessageBox.Show("Error loading the image: " + ex.Message);
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show(ex.Message);
//                }
//                Task.Delay(5000).Wait();
//            }
//        }
//        );
//    })
//    {
//    };
//}
//    }