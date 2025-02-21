using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public class FlappyBird : Form
{
    private const int Width = 600;
    private const int Height = 900;
    private const int Gravity = 5;
    private const int JumpPower = 22;
    private const int PipeWidth = 60;
    private const int PipeGap = 200;
    private int score;
    private int timerCount;
    private bool isGameOver;
    private bool isPlaying;
    private Rectangle bird;
    private int birdVelocity;
    private Timer gameTimer;
    private Timer pipeSpawnTimer;
    private List<Rectangle> pipes;
    private Image birdImage;
    private Image gameNameImage;
    private int pipeSpeed = 5;

    public FlappyBird()
    {
        this.ClientSize = new Size(Width, Height);
        this.Text = "Flappy Bird";
        this.BackColor = Color.LightSkyBlue;
        this.DoubleBuffered = true;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.StartPosition = FormStartPosition.CenterScreen;

        birdImage = Image.FromFile("bird.png.png");
        gameNameImage = Image.FromFile("gamename.png");

        bird = new Rectangle(Width / 4, Height / 2, 40, 40);
        birdVelocity = 0;

        pipes = new List<Rectangle>();

        gameTimer = new Timer();
        gameTimer.Interval = 20;
        gameTimer.Tick += GameTimer_Tick;

        pipeSpawnTimer = new Timer();
        pipeSpawnTimer.Interval = 2000;
        pipeSpawnTimer.Tick += PipeSpawnTimer_Tick;

        isGameOver = false;
        isPlaying = false;

        this.KeyDown += FlappyBird_KeyDown;
        this.MouseClick += FlappyBird_MouseClick;
        this.Paint += FlappyBird_Paint;
        this.Show();
    }

    private void FlappyBird_Paint(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;

        if (!isPlaying)
        {
            int gameNameWidth = gameNameImage.Width;
            int gameNameHeight = gameNameImage.Height;
            g.DrawImage(gameNameImage, Width / 2 - gameNameWidth / 2, 50, gameNameWidth, gameNameHeight);

            DrawMenuButton(g, "Play", 200, 400);
            DrawMenuButton(g, "Credits", 200, 500);
            DrawMenuButton(g, "Quit", 200, 600);
        }
        else
        {
            g.DrawImage(birdImage, bird.X, bird.Y, bird.Width, bird.Height);

            foreach (var pipe in pipes)
            {
                g.FillRectangle(Brushes.DarkGreen, pipe);
            }

            // Score with doubled size
            g.DrawString("Score: " + score, new Font("Arial", 32, FontStyle.Bold), Brushes.Gold, 10, 10);

            // Timer with doubled size
            g.DrawString("Time: " + timerCount / 60 + ":" + (timerCount % 60).ToString("00"), new Font("Arial", 32, FontStyle.Bold), Brushes.Gold, Width - 220, 10);

            if (isGameOver)
            {
                // "Your Score" in red and doubled size
                g.DrawString("Your Score: " + score, new Font("Arial", 36, FontStyle.Bold), Brushes.Red, Width - 380, Height / 2 - 50);
                DrawMenuButton(g, "Retry", Width / 2 - 100, Height / 2 + 50);
                DrawMenuButton(g, "Quit", Width / 2 - 100, Height / 2 + 150);
            }
        }
    }

    private void DrawMenuButton(Graphics g, string text, int x, int y)
    {
        g.FillRectangle(Brushes.Yellow, x, y, 200, 50);
        g.DrawString(text, new Font("Arial", 16), Brushes.Black, x + 50, y + 15);
    }

    private void FlappyBird_KeyDown(object sender, KeyEventArgs e)
    {
        if (!isPlaying)
        {
            if (e.KeyCode == Keys.Enter)
            {
                StartGame();
            }
            else if (e.KeyCode == Keys.C)
            {
                MessageBox.Show("Game Developer: Reha Demircan", "Credits");
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Application.Exit();
            }
        }
        else
        {
            if (e.KeyCode == Keys.Space && !isGameOver)
            {
                birdVelocity = -JumpPower;
            }
            else if (e.KeyCode == Keys.Enter && isGameOver)
            {
                RestartGame();
            }
        }
    }

    private void FlappyBird_MouseClick(object sender, MouseEventArgs e)
    {
        if (!isPlaying)
        {
            if (e.X >= 200 && e.X <= 400 && e.Y >= 400 && e.Y <= 450) // Play button
            {
                StartGame();
            }
            else if (e.X >= 200 && e.X <= 400 && e.Y >= 500 && e.Y <= 550) // Credits button
            {
                MessageBox.Show("Game Developer: Reha Demircan", "Credits");
            }
            else if (e.X >= 200 && e.X <= 400 && e.Y >= 600 && e.Y <= 650) // Quit button
            {
                Application.Exit();
            }
            else if (e.X >= Width / 2 - 100 && e.X <= Width / 2 + 100 && e.Y >= Height / 2 + 50 && e.Y <= Height / 2 + 100) // Retry button
            {
                RestartGame();
            }
            else if (e.X >= Width / 2 - 100 && e.X <= Width / 2 + 100 && e.Y >= Height / 2 + 150 && e.Y <= Height / 2 + 200) // Quit button
            {
                Application.Exit();
            }
        }
    }

    private void GameTimer_Tick(object sender, EventArgs e)
    {
        if (!isGameOver)
        {
            birdVelocity += Gravity;
            bird.Y += birdVelocity;

            for (int i = 0; i < pipes.Count; i++)
            {
                pipes[i] = new Rectangle(pipes[i].X - pipeSpeed, pipes[i].Y, pipes[i].Width, pipes[i].Height);
            }

            CheckCollisions();

            for (int i = 0; i < pipes.Count; i++)
            {
                if (pipes[i].X + pipes[i].Width < bird.X && pipes[i].X + pipes[i].Width + pipeSpeed >= bird.X && !isGameOver)
                {
                    score++;
                }
            }

            if (timerCount % 1200 == 0 && timerCount != 0)
            {
                pipeSpeed++;
            }

            timerCount++;

            this.Invalidate();
        }
    }

    private void PipeSpawnTimer_Tick(object sender, EventArgs e)
    {
        Random rand = new Random();
        int pipeHeight = rand.Next(100, Height - PipeGap - 100);

        pipes.Add(new Rectangle(Width, 0, PipeWidth, pipeHeight));
        pipes.Add(new Rectangle(Width, pipeHeight + PipeGap, PipeWidth, Height - pipeHeight - PipeGap));
    }

    private void StartGame()
    {
        score = 0;
        timerCount = 0;
        bird.Y = Height / 2;
        birdVelocity = 0;
        pipes.Clear();
        pipeSpeed = 5;

        isPlaying = true;
        isGameOver = false;

        gameTimer.Start();
        pipeSpawnTimer.Start();
    }

    private void RestartGame()
    {
        StartGame();
    }

    private void CheckCollisions()
    {
        if (bird.Y + bird.Height >= Height || bird.Y <= 0)
        {
            isGameOver = true;
        }

        foreach (var pipe in pipes)
        {
            if (bird.IntersectsWith(pipe))
            {
                isGameOver = true;
                break;
            }
        }
    }

    public static void Main()
    {
        Application.Run(new FlappyBird());
    }
}
