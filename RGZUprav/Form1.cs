namespace RGZUprav
{
    public partial class Form1 : Form
    {
        private int currentLevelPlayer2 = 0; // Начальное положение Игрока 2
        private int totalRounds = 25; // Максимальное количество ходов
        private int player2Wins = 0; // Количество побед Игрока 2
        private int player1Wins = 0; // Количество побед Игрока 1
        private int totalGamesPlayed = 0; // Количество сыгранных игр
        private double player1BehaviorProbability = 0.5; // Вероятность поведения Игрока 1 (0 - от центра, 1 - к центру)
        private bool[] predefinedSequence = new bool[25]; // Массив для хранения последовательности решений для PredefinedSequence
        private List<int> gameLengths = new List<int>(); // Список для хранения длинны каждой игры

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void startButton_Click(object sender, EventArgs e)
        {
            // Считываем количество игр из numericUpDown1
            int numberOfGames = (int)numericUpDown1.Value;

            // Сброс статистики
            player2Wins = 0;
            player1Wins = 0;
            totalGamesPlayed = 0;

            // Получаем вероятность поведения Игрока 1
            if (double.TryParse(winPercent.Text, out double probability) && probability >= 0 && probability <= 1)
            {
                player1BehaviorProbability = probability; // Задаем вероятность поведения игрока 1
            }
            else
            {
                MessageBox.Show("Введите допустимое значение вероятности от 0 до 1. Через запятую");
                return; // Прерываем выполнение, если значение недопустимо
            }

            // Получаем предустановленную последовательность для PredefinedSequence
            if (PredefinedSequenceRadioButton.Checked)
            {
                for (int i = 0; i < 25; i++)
                {
                    predefinedSequence[i] = Controls.Find($"checkBox{i + 1}", true).Cast<CheckBox>().FirstOrDefault()?.Checked ?? false;
                }
            }

            // Проведение нескольких игр
            for (int game = 1; game <= numberOfGames; game++)
            {
                totalGamesPlayed++;

                bool gameOver = false; // Флаг окончания игры
                int roundsPlayed = 0; // Количество раундов в текущей игре

                for (int round = 1; round <= totalRounds; round++)
                {
                    roundsPlayed++; // Увеличиваем количество сыгранных раундов
                    int suggestedMove = Player1Move();

                    bool player2Agrees = GetPlayer2Agreement(round);

                    if (player2Agrees)
                    {
                        currentLevelPlayer2 = MovePlayer2(suggestedMove);
                    }
                    else
                    {
                        int newMove = GetPlayer2NewMove(suggestedMove);
                        currentLevelPlayer2 = MovePlayer2(newMove);
                    }

                    // Проверка победы Игрока 2
                    if (currentLevelPlayer2 == 5)
                    {
                        player2Wins++;
                        gameOver = true; // Игрок 2 победил, игра окончена                     
                        gameLengths.Add(round); // Добавляем количество раундов до победы
                        //MessageBox.Show("2");
                        break;
                    }

                    // Проверка победы Игрока 1
                    if (round == totalRounds)
                    {
                        player1Wins++;
                        gameLengths.Add(round); // Добавляем количество раундов до победы Игрока 1
                        //MessageBox.Show("1");
                    }
                }

                // Если после 25 раундов Игрок 2 не победил, то победу засчитываем Игроку 1
                if (!gameOver && totalRounds > 0)
                {
                    player1Wins++;
                    gameLengths.Add(totalRounds); // Добавляем, что игра закончилась после 25 раундов
                }
            }

            // Обновляем статистику по завершению всех игр
            UpdateStatistics();

            // Показываем panel1 после старта игры
            panel1.Visible = true;
        }

        private void UpdateStatistics()
        {
            double player2WinPercentage = (double)player2Wins / totalGamesPlayed * 100;
            double player1WinPercentage = (double)player1Wins / totalGamesPlayed * 100;

            // Подсчет средней длины игры (среднее количество раундов до победы)
            double averageRounds = gameLengths.Count > 0 ? gameLengths.Average() : 0;

            // Обновление статистики на форме
            Player1WinPercentageLabel.Text = $"Процент побед 1 игрока: {player1WinPercentage}%";
            Player2WinPercentageLabel.Text = $"Процент побед 2 игрока: {player2WinPercentage}%";
            AverageGameLengthLabel.Text = $"Средняя длина партии: {averageRounds:F2} раундов";
        }

        private void PredefinedSequenceRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            // Показываем FlowLayoutPanel только при выборе PredefinedSequenceRadioButton
            if(PredefinedSequenceRadioButton.Checked)
                flowLayoutPanel1.Visible = true;
            else flowLayoutPanel1.Visible = false;

        }

        private bool GetPlayer2Agreement(int round)
        {
            if (AlwaysAgreeRadioButton.Checked)
            {
                return true; // Игрок 2 всегда согласен
            }
            else if (AlwaysDisagreeRadioButton.Checked)
            {
                return false; // Игрок 2 всегда не согласен
            }
            else if (RandomDecisionRadioButton.Checked)
            {
                Random rand = new Random();
                return rand.NextDouble() > 0.5; // Случайное решение
            }
            else if (PredefinedSequenceRadioButton.Checked)
            {
                return predefinedSequence[round - 1]; // Используем заранее определенную последовательность
            }
            else
            {
                return true; // По умолчанию соглашение (на всякий случай, если не выбрана опция)
            }
        }

        private int GetPlayer2NewMove(int suggestedMove)
        {
            // Если игрок 2 не согласен, движется в противоположную сторону
            if (suggestedMove > currentLevelPlayer2)
            {
                return Math.Max(0, currentLevelPlayer2); // Не двигается вперед
            }
            else if (suggestedMove < currentLevelPlayer2)
            {
                return Math.Min(5, currentLevelPlayer2); // Не двигается назад
            }
            else
            {
                return currentLevelPlayer2; // Остается на месте
            }
        }

        private int Player1Move()
        {
            Random rand = new Random();
            double randomValue = rand.NextDouble();

            if (currentLevelPlayer2 == 0)
            {
                return randomValue <= player1BehaviorProbability ? 1 : 0;
            }
            else if (currentLevelPlayer2 == 1)
            {
                return randomValue <= player1BehaviorProbability ? 2 : 1;
            }
            else
            {
                return randomValue <= player1BehaviorProbability ? currentLevelPlayer2 + 1 : currentLevelPlayer2 - 1;
            }
        }

        private int MovePlayer2(int move)
        {
            return move >= 0 && move <= 5 ? move : currentLevelPlayer2;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            // Пустая функция, здесь можно добавить логику, если потребуется
        }

        private void label2_Click(object sender, EventArgs e)
        {
            // Пустая функция, здесь можно добавить логику, если потребуется
        }
    }
}
