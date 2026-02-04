using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace vereb_app
{
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        DispatcherTimer weatherTimer = new DispatcherTimer();
        Random random = new Random();

        double birdY = 149;
        double birdVelocity = 0;
        double gravity = 1;

        double jumpForce = -12;
        double JumpForce2 = -12;

        double rainJumpForce = -6;
        

        Rectangle fogLayer;
        bool isFogActive = false;
        bool isRaining = false;

        List<PipePair> pipes = new List<PipePair>();
        double pipeSpeed = 5;
        double pipeStartX = 800;
        int pipeGap = 140;

        int score = 0;
        bool gameOver = false;

        public MainWindow()
        {
            InitializeComponent();

            SetupPipes();

            weatherTimer.Interval = TimeSpan.FromSeconds(random.Next(5, 11));
            weatherTimer.Tick += (s, e) =>
            {
                if (random.Next(0, 2) == 0)
                    Rain();
                else
                    Fog();
            };
            weatherTimer.Start();

            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            this.KeyDown += OnKeyDown;
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (gameOver) return;

            birdVelocity += gravity;
            birdY += birdVelocity;
            Canvas.SetTop(bird, birdY);

            MovePipes();
            CheckCollision();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && !gameOver)
                birdVelocity = jumpForce;
            else if (e.Key == Key.Space && gameOver)
                RestartGame();
        }

        private void SetupPipes()
        {
            pipes.Add(new PipePair(columnUp, columnDown, 600));
            pipes.Add(new PipePair(columnUp2, columnDown2, 900));
            pipes.Add(new PipePair(columnUp3, columnDown3, 1200));

            foreach (var pipe in pipes)
                RandomizePipe(pipe);
        }

        private void MovePipes()
        {
            foreach (var pipe in pipes)
            {
                double x = Canvas.GetLeft(pipe.Top);
                x -= pipeSpeed;

                if (x < -80)
                {
                    x = pipeStartX;
                    RandomizePipe(pipe);
                    score++;
                }

                Canvas.SetLeft(pipe.Top, x);
                Canvas.SetLeft(pipe.Bottom, x);
            }
        }

        private void RandomizePipe(PipePair pipe)
        {
            int topHeight = random.Next(-120, -20);
            Canvas.SetTop(pipe.Top, topHeight);
            Canvas.SetTop(pipe.Bottom, topHeight + pipe.Top.Height + pipeGap);
        }

        private void CheckCollision()
        {
            Rect birdRect = new Rect(Canvas.GetLeft(bird), Canvas.GetTop(bird), bird.Width, bird.Height);

            if (birdY < 0 || birdY > canvas.ActualHeight - bird.Height)
                EndGame();

            foreach (var pipe in pipes)
            {
                Rect topRect = new Rect(Canvas.GetLeft(pipe.Top), Canvas.GetTop(pipe.Top), pipe.Top.Width, pipe.Top.Height);
                Rect bottomRect = new Rect(Canvas.GetLeft(pipe.Bottom), Canvas.GetTop(pipe.Bottom), pipe.Bottom.Width, pipe.Bottom.Height);

                if (birdRect.IntersectsWith(topRect) || birdRect.IntersectsWith(bottomRect))
                    EndGame();
            }
        }

        private void EndGame()
        {
            gameOver = true;
            gameTimer.Stop();
            MessageBox.Show("Game Over! Score: " + score);
        }

        private void RestartGame()
        {
            birdY = 149;
            birdVelocity = 0;
            Canvas.SetTop(bird, birdY);

            score = 0;
            gameOver = false;

            jumpForce = JumpForce2;
            isRaining = false;

            isFogActive = false;
            if (fogLayer != null)
                canvas.Children.Remove(fogLayer);

            SetupPipes();
            gameTimer.Start();
        }

        private void Rain()
        {
            if (isRaining) return;

            isRaining = true;
            jumpForce = rainJumpForce;

            DispatcherTimer rainTimer = new DispatcherTimer();
            rainTimer.Interval = TimeSpan.FromSeconds(3);

            rainTimer.Tick += (s, e) =>
            {
                jumpForce = JumpForce2;
                isRaining = false;

                rainTimer.Stop();
                weatherTimer.Interval = TimeSpan.FromSeconds(random.Next(5, 11));
            };

            rainTimer.Start();
        }

        private void Fog()
        {
            if (isFogActive) return;

            isFogActive = true;

            fogLayer = new Rectangle
            {
                Width = canvas.ActualWidth,
                Height = canvas.ActualHeight,
                Fill = System.Windows.Media.Brushes.LightGray,
                Opacity = 0.4
            };

            Canvas.SetLeft(fogLayer, 0);
            Canvas.SetTop(fogLayer, 0);

            canvas.Children.Add(fogLayer);

            DispatcherTimer fogTimer = new DispatcherTimer();
            fogTimer.Interval = TimeSpan.FromSeconds(4);

            fogTimer.Tick += (s, e) =>
            {
                canvas.Children.Remove(fogLayer);
                isFogActive = false;

                fogTimer.Stop();
                weatherTimer.Interval = TimeSpan.FromSeconds(random.Next(5, 11));
            };

            fogTimer.Start();
        }
    }

    class PipePair
    {
        public Rectangle Top { get; }
        public Rectangle Bottom { get; }

        public PipePair(Rectangle top, Rectangle bottom, double startX)
        {
            Top = top;
            Bottom = bottom;
            Canvas.SetLeft(Top, startX);
            Canvas.SetLeft(Bottom, startX);
        }
    }
}
