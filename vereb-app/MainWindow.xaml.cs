using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace vereb_app
{
	public partial class MainWindow : Window
	{
		DispatcherTimer gameTimer = new DispatcherTimer();

		double birdY = 149;
		double gravity = 2.2;
		double jumpForce = 30;

		double columnSpeed = 5;
		double columnReset = 300;

		int score = 0;
		bool gameOver = false;
		bool scoreAdded = false;

		public MainWindow()
		{
			InitializeComponent();

			gameTimer.Interval = TimeSpan.FromMilliseconds(20);
			gameTimer.Tick += GameLoop;
			gameTimer.Start();

			this.KeyDown += OnKeyDown;
		}

		private void GameLoop(object sender, System.EventArgs e)
		{
			if (gameOver) return;

			birdY += gravity;
			Canvas.SetTop(bird, birdY);

			MoveColumns();
			CheckCollision();
			AddScore();
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space && !gameOver)
			{
				birdY -= jumpForce;
			}
			else if (e.Key == Key.Space && gameOver)
			{
				RestartGame();
			}
		}

		private void MoveColumns()
		{
			double leftUp = Canvas.GetLeft(columnUp);
			double leftDown = Canvas.GetLeft(columnDown);

			leftUp -= columnSpeed;
			leftDown -= columnSpeed;

			
			if (leftUp < -60)
			{
				leftUp = columnReset;
				leftDown = columnReset;
				scoreAdded = false;
				RandomLocation();
			}

			Canvas.SetLeft(columnUp, leftUp);
			Canvas.SetLeft(columnDown, leftDown);
		}

		private void CheckCollision()
		{
			Rect birdRect = new Rect(
				Canvas.GetLeft(bird),
				Canvas.GetTop(bird),
				bird.Width,
				bird.Height);

			Rect columnUpRect = new Rect(
				Canvas.GetLeft(columnUp),
				Canvas.GetTop(columnUp),
				columnUp.Width,
				columnUp.Height);

			Rect columnDownRect = new Rect(
				Canvas.GetLeft(columnDown),
				Canvas.GetTop(columnDown),
				columnDown.Width,
				columnDown.Height);

			if (birdRect.IntersectsWith(columnUpRect) ||
				birdRect.IntersectsWith(columnDownRect))
			{
				EndGame();
			}
		}

		private void EndGame()
		{
			gameOver = true;
			gameTimer.Stop();
			MessageBox.Show("Game Over! Score: " + score);
		}

		private void RandomLocation()
		{
			Random random = new Random();

			int gapSize = 130;
			int minTop = -100;
			int maxTop = 0;

			int topHeight = random.Next(minTop, maxTop);

			Canvas.SetTop(columnUp, topHeight);
			Canvas.SetTop(columnDown, topHeight + columnUp.Height + gapSize);
		}

		private void RestartGame()
		{
			birdY = 149;
			Canvas.SetTop(bird, birdY);

			Canvas.SetLeft(columnUp, columnReset);
			Canvas.SetLeft(columnDown, columnReset);

			score = 0;
			scoreAdded = false;
			gameOver = false;

			RandomLocation();
			gameTimer.Start();
		}

		private void AddScore()
		{
			if (scoreAdded) return;

			double columnX = Canvas.GetLeft(columnUp);
			double birdX = Canvas.GetLeft(bird);

			if (columnX + columnUp.Width < birdX)
			{
				score++;
				scoreAdded = true;
			}
		}
	}
}
