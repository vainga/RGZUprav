namespace RGZUprav
{
    public partial class Form1 : Form
    {
        private int currentLevelPlayer2 = 0; // ��������� ��������� ������ 2
        private int totalRounds = 25; // ������������ ���������� �����
        private int player2Wins = 0; // ���������� ����� ������ 2
        private int player1Wins = 0; // ���������� ����� ������ 1
        private int totalGamesPlayed = 0; // ���������� ��������� ���
        private double player1BehaviorProbability = 0.5; // ����������� ��������� ������ 1 (0 - �� ������, 1 - � ������)
        private bool[] predefinedSequence = new bool[25]; // ������ ��� �������� ������������������ ������� ��� PredefinedSequence
        private List<int> gameLengths = new List<int>(); // ������ ��� �������� ������ ������ ����

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void startButton_Click(object sender, EventArgs e)
        {
            // ��������� ���������� ��� �� numericUpDown1
            int numberOfGames = (int)numericUpDown1.Value;

            // ����� ����������
            player2Wins = 0;
            player1Wins = 0;
            totalGamesPlayed = 0;

            // �������� ����������� ��������� ������ 1
            if (double.TryParse(winPercent.Text, out double probability) && probability >= 0 && probability <= 1)
            {
                player1BehaviorProbability = probability; // ������ ����������� ��������� ������ 1
            }
            else
            {
                MessageBox.Show("������� ���������� �������� ����������� �� 0 �� 1. ����� �������");
                return; // ��������� ����������, ���� �������� �����������
            }

            // �������� ����������������� ������������������ ��� PredefinedSequence
            if (PredefinedSequenceRadioButton.Checked)
            {
                for (int i = 0; i < 25; i++)
                {
                    predefinedSequence[i] = Controls.Find($"checkBox{i + 1}", true).Cast<CheckBox>().FirstOrDefault()?.Checked ?? false;
                }
            }

            // ���������� ���������� ���
            for (int game = 1; game <= numberOfGames; game++)
            {
                totalGamesPlayed++;

                bool gameOver = false; // ���� ��������� ����
                int roundsPlayed = 0; // ���������� ������� � ������� ����

                for (int round = 1; round <= totalRounds; round++)
                {
                    roundsPlayed++; // ����������� ���������� ��������� �������
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

                    // �������� ������ ������ 2
                    if (currentLevelPlayer2 == 5)
                    {
                        player2Wins++;
                        gameOver = true; // ����� 2 �������, ���� ��������                     
                        gameLengths.Add(round); // ��������� ���������� ������� �� ������
                        //MessageBox.Show("2");
                        break;
                    }

                    // �������� ������ ������ 1
                    if (round == totalRounds)
                    {
                        player1Wins++;
                        gameLengths.Add(round); // ��������� ���������� ������� �� ������ ������ 1
                        //MessageBox.Show("1");
                    }
                }

                // ���� ����� 25 ������� ����� 2 �� �������, �� ������ ����������� ������ 1
                if (!gameOver && totalRounds > 0)
                {
                    player1Wins++;
                    gameLengths.Add(totalRounds); // ���������, ��� ���� ����������� ����� 25 �������
                }
            }

            // ��������� ���������� �� ���������� ���� ���
            UpdateStatistics();

            // ���������� panel1 ����� ������ ����
            panel1.Visible = true;
        }

        private void UpdateStatistics()
        {
            double player2WinPercentage = (double)player2Wins / totalGamesPlayed * 100;
            double player1WinPercentage = (double)player1Wins / totalGamesPlayed * 100;

            // ������� ������� ����� ���� (������� ���������� ������� �� ������)
            double averageRounds = gameLengths.Count > 0 ? gameLengths.Average() : 0;

            // ���������� ���������� �� �����
            Player1WinPercentageLabel.Text = $"������� ����� 1 ������: {player1WinPercentage}%";
            Player2WinPercentageLabel.Text = $"������� ����� 2 ������: {player2WinPercentage}%";
            AverageGameLengthLabel.Text = $"������� ����� ������: {averageRounds:F2} �������";
        }

        private void PredefinedSequenceRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            // ���������� FlowLayoutPanel ������ ��� ������ PredefinedSequenceRadioButton
            if(PredefinedSequenceRadioButton.Checked)
                flowLayoutPanel1.Visible = true;
            else flowLayoutPanel1.Visible = false;

        }

        private bool GetPlayer2Agreement(int round)
        {
            if (AlwaysAgreeRadioButton.Checked)
            {
                return true; // ����� 2 ������ ��������
            }
            else if (AlwaysDisagreeRadioButton.Checked)
            {
                return false; // ����� 2 ������ �� ��������
            }
            else if (RandomDecisionRadioButton.Checked)
            {
                Random rand = new Random();
                return rand.NextDouble() > 0.5; // ��������� �������
            }
            else if (PredefinedSequenceRadioButton.Checked)
            {
                return predefinedSequence[round - 1]; // ���������� ������� ������������ ������������������
            }
            else
            {
                return true; // �� ��������� ���������� (�� ������ ������, ���� �� ������� �����)
            }
        }

        private int GetPlayer2NewMove(int suggestedMove)
        {
            // ���� ����� 2 �� ��������, �������� � ��������������� �������
            if (suggestedMove > currentLevelPlayer2)
            {
                return Math.Max(0, currentLevelPlayer2); // �� ��������� ������
            }
            else if (suggestedMove < currentLevelPlayer2)
            {
                return Math.Min(5, currentLevelPlayer2); // �� ��������� �����
            }
            else
            {
                return currentLevelPlayer2; // �������� �� �����
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
            // ������ �������, ����� ����� �������� ������, ���� �����������
        }

        private void label2_Click(object sender, EventArgs e)
        {
            // ������ �������, ����� ����� �������� ������, ���� �����������
        }
    }
}
