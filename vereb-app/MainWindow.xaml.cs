using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace vereb_app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		DispatcherTimer gameTimer = new DispatcherTimer();

		double birdY = 149;      
		double gravity = 2.2; 
		double jumpForce = 30;
		double speed = 10;

		
		public MainWindow()
		{
			InitializeComponent();

			gameTimer.Interval = System.TimeSpan.FromMilliseconds(20);
			gameTimer.Tick += GameLoop;
			gameTimer.Start();

			this.KeyDown += OnKeyDown;
		}

		private void GameLoop(object sender, System.EventArgs e)
		{
			birdY += gravity; 
			Canvas.SetTop(bird, birdY);
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space)
			{
				birdY -= jumpForce;
			}
		}

		private void generateRectangle()
		{

		}

	}
}