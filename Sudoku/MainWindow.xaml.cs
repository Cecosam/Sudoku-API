using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Sudoku
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer dispatcherTimer = null;
        private Stopwatch stopWatch = new Stopwatch();
        private SqlConnection connection;
        private int lastSudokuDifficulty = 0;
        public const int MAX_PREDEFINED_SUDOKUS = 1000;
        public const string CONNECTION_STRING = @"workstation id = SudokuSolver.mssql.somee.com; packet size = 4096; user id = Cecosam_SQLLogin_1; pwd = zok2ocsnoo; data source = SudokuSolver.mssql.somee.com; persist security info = False; initial catalog = SudokuSolver";
        public MainWindow()
        {
            InitializeComponent();
            AddColors();

            //Endless tooltips
            ToolTipService.ShowDurationProperty.OverrideMetadata(
    typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));

            connection = new SqlConnection();
            connection.ConnectionString = CONNECTION_STRING;            
        }

        private void CheckIfDigit(object sender, TextCompositionEventArgs e)
        {
            try
            {
                if (!char.IsDigit(e.Text, e.Text.Length - 1))
                {
                    e.Handled = true;
                    return;
                }
                else
                {
                    (sender as TextBox).Text = e.Text;
                }


                var sudokuToSolve = new SudokuCell[9, 9];
                var sudokuValidator = new SudokuSolver();

                var counter = 0;
                var numbersCount = 0;
                for (int row = 0; row < 9; row++)
                {
                    for (int col = 0; col < 9; col++)
                    {
                        var textBox = this.FindName("TextBox" + counter) as TextBox;
                        sudokuToSolve[row, col] = new SudokuCell(row, col);
                        if (textBox.Text != string.Empty)
                        {
                            sudokuToSolve[row, col].Value = int.Parse(textBox.Text);
                            numbersCount++;
                        }
                        counter++;
                    }
                }

                if (numbersCount == 81)
                {
                    if (!sudokuValidator.CheckIfSudokuIsValid(sudokuToSolve))
                    {
                        MessageBox.Show("This sudoku is not valid! Please check for errors!", "Warning");
                        return;
                    }
                    MessageBox.Show("Congratulations! You solved this sudoku for: " + GetTime() + "!");
                }
            }
            catch (Exception)
            {

            }
        }

        private string GetTime()
        {
            var seconds = stopWatch.Elapsed.Seconds;
            var minutes = stopWatch.Elapsed.Minutes;
            var hours = stopWatch.Elapsed.Hours;
            stopWatch.Stop();

            return string.Format("{0}{1}{2}",
                hours == 0 ? "" : hours + " hours, ",
                minutes == 0 ? "" : minutes + " minutes and ",
                seconds + " seconds");
        }
        private void GetSudoku(object sender, RoutedEventArgs e)
        {
            var sudokuAsString = string.Empty;
            var difficulty = 0;

            switch (((Button)sender).Name[6])
            {
                case 'E':
                    difficulty = 1;
                    lastSudokuDifficulty = 1;
                    break;
                case 'M':
                    difficulty = 2;
                    lastSudokuDifficulty = 2;
                    break;
                case 'H':
                    difficulty = 4;
                    lastSudokuDifficulty = 4;
                    break;
                case 'V':
                    difficulty = 8;
                    lastSudokuDifficulty = 8;
                    break;
                default:
                    break;
            }

            try
            {
                connection.Open();
                var queryString = string.Format(string.Format("SELECT TOP 1 SudokuValues,Difficulty FROM Sudoku where Difficulty = {0} ORDER BY NEWID()", difficulty));
                SqlCommand query = new SqlCommand(queryString, connection);
                SqlDataReader reader = query.ExecuteReader();
                reader.Read();
                sudokuAsString = reader[0] as string;
            }
            catch (Exception)
            {
                MessageBox.Show("Could not connect to the server! An offline sudoku will be generated for you!", "Warning!");
                var rnd = new Random();
                if (difficulty == 1)
                {
                    sudokuAsString = PredefinedEasySudokus.GetPredefinedSudokuEasy(rnd.Next(0, MAX_PREDEFINED_SUDOKUS));
                }
                if (difficulty == 2)
                {
                    sudokuAsString = PredefinedMediumSudokus.GetPredefinedSudokuMedium(rnd.Next(0, MAX_PREDEFINED_SUDOKUS));
                }
                if (difficulty == 4)
                {
                    sudokuAsString = PredefinedHardSudokus.GetPredefinedSudokuHard(rnd.Next(0, MAX_PREDEFINED_SUDOKUS));
                }
                if (difficulty == 8)
                {
                    sudokuAsString = PredefinedVeryHardSudokus.GetPredefinedSudokuVeryHard(rnd.Next(0, 100));
                }
            }
            finally
            {
                connection.Close();
            }

            var counter = 0;

            for (int index = 0; index < 81; index++)
            {
                var textBox = this.FindName("TextBox" + counter) as TextBox;
                textBox.Text = string.Empty;
                textBox.IsReadOnly = false;
                textBox.Background = new SolidColorBrush(Color.FromRgb(237, 249, 240));
                if (sudokuAsString[index] != '0')
                {
                    textBox.IsReadOnly = true;
                    textBox.Background = new SolidColorBrush(Color.FromRgb(229, 247, 215));
                    textBox.Text = sudokuAsString[index].ToString();
                }
                counter++;
            }

            StartDispatchTimer();

            stopWatch.Restart();
        }

        private void StartDispatchTimer()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(StartClock);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void StartClock(object sender, EventArgs e)
        {
            (FindName("textBoxHours") as TextBox).Text = stopWatch.Elapsed.Hours.ToString();
            (FindName("textBoxMinutes") as TextBox).Text = stopWatch.Elapsed.Minutes.ToString();
            (FindName("textBoxSeconds") as TextBox).Text = stopWatch.Elapsed.Seconds.ToString();
        }
        private void SolveSudoku(object sender, RoutedEventArgs e)
        {
            var sudokuSolver = new SudokuDifficulty();
            var sudokuValidator = new SudokuSolver();
            var sudokuToSolve = new SudokuCell[9, 9];

            var counter = 0;
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    var textBox = this.FindName("TextBox" + counter) as TextBox;
                    sudokuToSolve[row, col] = new SudokuCell(row, col);
                    if (textBox.Text != string.Empty)
                    {
                        sudokuToSolve[row, col].Value = int.Parse(textBox.Text);
                    }
                    counter++;
                }
            }

            if (lastSudokuDifficulty == 0)
            {
                MessageBox.Show("First you must generate sudoku!");
                return;
            }

            if (!sudokuValidator.CheckIfSudokuIsValid(sudokuToSolve))
            {
                MessageBox.Show("This sudoku is not valid! Please check for errors!", "Warning");
                return;
            }

            try
            {
                sudokuToSolve = sudokuSolver.SolveSudoku(sudokuToSolve);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("This sudoku is not valid! Please check for errors!", "Warning");
                return;
            }

            counter = 0;

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    var textBox = this.FindName("TextBox" + counter) as TextBox;
                    textBox.Text = string.Empty;
                    textBox.Text = sudokuToSolve[row, col].Value.ToString();
                    counter++;
                }
            }
            stopWatch.Stop();
        }
        private void ClearSudoku(object sender, RoutedEventArgs e)
        {
            if (lastSudokuDifficulty == 0)
            {
                MessageBox.Show("First you must generate sudoku!");
                return;
            }

            var counter = 0;
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    var textBox = this.FindName("TextBox" + counter) as TextBox;
                    if (textBox.IsReadOnly != true)
                    {
                        textBox.Text = string.Empty;
                    }
                    counter++;
                }
            }
            stopWatch.Restart();
        }
        private void CheckIfSudokuIsValid(object sender, RoutedEventArgs e)
        {
            var sudokuSolver = new SudokuDifficulty();
            var sudokuValidator = new SudokuSolver();
            var sudokuToSolve = new SudokuCell[9, 9];

            var counter = 0;
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    var textBox = this.FindName("TextBox" + counter) as TextBox;
                    sudokuToSolve[row, col] = new SudokuCell(row, col);
                    if (textBox.Text != string.Empty)
                    {
                        sudokuToSolve[row, col].Value = int.Parse(textBox.Text);
                    }
                    counter++;
                }
            }

            if (lastSudokuDifficulty == 0)
            {
                MessageBox.Show("First you must generate sudoku!");
                return;
            }

            if (!sudokuValidator.CheckIfSudokuIsValid(sudokuToSolve))
            {
                MessageBox.Show("You have some errors!");
                return;
            }

            try
            {
                sudokuToSolve = sudokuSolver.SolveSudoku(sudokuToSolve);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("You have some errors!");
                return;
            }

            MessageBox.Show("You are doing great!");
        }

        private void PrintSudoku(object sender, EventArgs e)
        {
            if (lastSudokuDifficulty == 0)
            {
                MessageBox.Show("First you must generate sudoku!");
                return;
            }
            System.Windows.Forms.WebBrowser browser = new System.Windows.Forms.WebBrowser();
            browser.DocumentText = PrintSudokuTemplate.SUDOKUTEMPLATE;

            browser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(IsDocumentCompleted);
        }

        private void IsDocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            var browser = (System.Windows.Forms.WebBrowser)sender;
            var document = browser.Document;
            var isOnline = true;

            var counter = 0;
            for (int i = 1; i <= 81; i++)
            {
                var element = this.FindName("TextBox" + counter) as TextBox;
                int number;
                var isParsed = int.TryParse(element.Text, out number);
                if (isParsed)
                {
                    document.GetElementById("textBox" + i).InnerText = element.Text;
                }
                counter++;
            }

            var sudokuAsString = string.Empty;

            try
            {
                connection.Open();
                var queryString = string.Format(string.Format("SELECT TOP 1 SudokuValues,Difficulty FROM Sudoku where Difficulty = {0} ORDER BY NEWID()", lastSudokuDifficulty));
                SqlCommand query = new SqlCommand(queryString, connection);
                SqlDataReader reader = query.ExecuteReader();
                reader.Read();
                sudokuAsString = reader[0] as string;
            }
            catch (Exception)
            {
                isOnline = false;
            }
            finally
            {
                connection.Close();
            }

            if (isOnline)
            {
                for (int i = 1; i <= 81; i++)
                {
                    if (sudokuAsString[i - 1] != '0')
                    {
                        document.GetElementById("textBox" + (i + 100)).InnerText = sudokuAsString[i - 1].ToString();
                    }
                }
            }

            if (browser.ReadyState.Equals(System.Windows.Forms.WebBrowserReadyState.Complete))
            {
                browser.ShowPrintDialog();
            }
        }

        private void MainWindowName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.E))
            {
                sender = FindName("buttonEasySudoku");
                GetSudoku(sender, e);
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.M))
            {
                sender = FindName("buttonMediumSudoku");
                GetSudoku(sender, e);
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.H))
            {
                sender = FindName("buttonHardSudoku");
                GetSudoku(sender, e);
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.Y))
            {
                sender = FindName("buttonVeryHardSudoku");
                GetSudoku(sender, e);
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.S))
            {
                sender = FindName("buttonSolveSudoku");
                SolveSudoku(sender, e);
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.W))
            {
                sender = FindName("buttonChecker");
                CheckIfSudokuIsValid(sender, e);
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.P))
            {
                sender = FindName("buttonPrint");
                PrintSudoku(sender, e);
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.L))
            {
                sender = FindName("buttonClear");
                ClearSudoku(sender, e);
            }
        }


        private void LoginUser(object sender, RoutedEventArgs e)
        {

        }

        private void CreateUser(object sender, RoutedEventArgs e)
        {

        }

        private void LogoutUser(object sender, RoutedEventArgs e)
        {

        }

        #region Front-End
        private void AddColors()
        {
            //Grid Color
            (FindName("MainGrid") as Grid).Background = new SolidColorBrush(Color.FromRgb(224, 250, 252));

            //Buttons Color And Mouse Events
            AddColorAndMouseEventsToButton("buttonEasySudoku");
            AddColorAndMouseEventsToButton("buttonMediumSudoku");
            AddColorAndMouseEventsToButton("buttonHardSudoku");
            AddColorAndMouseEventsToButton("buttonVeryHardSudoku");
            AddColorAndMouseEventsToButton("buttonSolveSudoku");
            AddColorAndMouseEventsToButton("buttonChecker");
            AddColorAndMouseEventsToButton("buttonPrint");
            AddColorAndMouseEventsToButton("buttonClear");

            //TextBoxes Color
            for (int i = 0; i < 81; i++)
            {
                var textBox = FindName("TextBox" + i) as TextBox;
                textBox.Background = new SolidColorBrush(Color.FromRgb(237, 249, 240));
                textBox.MouseEnter += new MouseEventHandler(EnterTextBox);
                textBox.MouseLeave += new MouseEventHandler(LeaveTextBox);
            }

            TimeTextBoxesColors("textBoxHours");
            TimeTextBoxesColors("textBoxMinutes");
            TimeTextBoxesColors("textBoxSeconds");
        }

        private void TimeTextBoxesColors(string input)
        {
            TextBox textBox = FindName(input) as TextBox;
            textBox.BorderThickness = new Thickness(0);
            textBox.Background = new SolidColorBrush(Color.FromRgb(224, 250, 252));
            textBox.TextAlignment = TextAlignment.Center;
        }

        private void AddColorAndMouseEventsToButton(string buttonName)
        {
            Button button = FindName(buttonName) as Button;
            button.Background = new SolidColorBrush(Color.FromRgb(237, 249, 249));
            button.MouseEnter += new MouseEventHandler(EnterButton);
            button.MouseLeave += new MouseEventHandler(LeaveButton);

        }

        private void LeaveButton(object sender, MouseEventArgs e)
        {
            ColorAnimation animation = new ColorAnimation();
            var button = sender as Button;

            animation.To = Color.FromRgb(237, 249, 249);
            animation.Duration = new Duration(TimeSpan.FromSeconds(1));
            button.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }

        private void EnterButton(object sender, MouseEventArgs e)
        {
            ColorAnimation animation = new ColorAnimation();
            var button = sender as Button;

            animation.To = Color.FromRgb(252, 237, 232);
            animation.Duration = new Duration(TimeSpan.FromSeconds(1));
            button.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }

        private void LeaveTextBox(object sender, MouseEventArgs e)
        {
            ColorAnimation animation = new ColorAnimation();
            var textBox = sender as TextBox;

            if (textBox.IsReadOnly == true)
            {
                animation.To = Color.FromRgb(229, 247, 215);
            }
            else
            {
                animation.To = Color.FromRgb(237, 249, 240);
            }

            textBox.ToolTip = null;
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            textBox.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }

        private void EnterTextBox(object sender, MouseEventArgs e)
        {
            ColorAnimation animation = new ColorAnimation();
            var textBox = sender as TextBox;

            var sudokuDifficulty = new SudokuDifficulty();
            var sudoku = new SudokuCell[9, 9];

            if ((FindName("checkBoxHints") as CheckBox).IsChecked == true)
            {
                if (textBox.IsReadOnly == false && lastSudokuDifficulty != 0 && textBox.Text == string.Empty)
                {
                    try
                    {
                        var counter = 0;
                        var inputRow = 0;
                        var inputCol = 0;
                        for (int row = 0; row < 9; row++)
                        {
                            for (int col = 0; col < 9; col++)
                            {
                                var box = this.FindName("TextBox" + counter) as TextBox;
                                sudoku[row, col] = new SudokuCell(row, col);
                                if (box.Text != string.Empty)
                                {
                                    sudoku[row, col].Value = int.Parse(box.Text);
                                }
                                if ("TextBox" + counter == textBox.Name)
                                {
                                    inputCol = col;
                                    inputRow = row;
                                }
                                counter++;
                            }
                        }
                        var outputList = sudokuDifficulty.GetAllPossibleNumbers(inputRow, inputCol, sudoku).PossibleValues;

                        var result = string.Join(" ", outputList);

                        var toolTip = new ToolTip();
                        toolTip.Background = new SolidColorBrush(Color.FromRgb(224, 250, 252));
                        toolTip.Content = string.Format("You can put those numbers: {0}!", result);
                        textBox.ToolTip = toolTip;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            
            
            
            animation.To = Color.FromRgb(252, 237, 232);
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            textBox.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }
        #endregion
    }
}
