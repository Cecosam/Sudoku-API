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
            if (lastSudokuDifficulty == 0)
            {
                ShowDisabledItems();
            }

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
                textBox.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                textBox.IsEnabled = true;
                if (sudokuAsString[index] != '0')
                {
                    textBox.IsReadOnly = true;                   
                    textBox.Background = new SolidColorBrush(Color.FromRgb(222, 227, 229));
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
                MessageBox.Show("You must first generate a sudoku!");
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
                    textBox.IsEnabled = false;
                    counter++;
                }
            }
            stopWatch.Stop();
        }
        private void ClearSudoku(object sender, RoutedEventArgs e)
        {
            if (lastSudokuDifficulty == 0)
            {
                MessageBox.Show("You must first generate a sudoku!");
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
                MessageBox.Show("You must first generate a sudoku!");
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
                MessageBox.Show("You must first generate a sudoku!");
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

            var rnd = new Random();
            for (int start = 100; start < 600; start += 100)
            {
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

                    if (lastSudokuDifficulty == 1)
                    {
                        sudokuAsString = PredefinedEasySudokus.GetPredefinedSudokuEasy(rnd.Next(0, MAX_PREDEFINED_SUDOKUS));
                    }
                    if (lastSudokuDifficulty == 2)
                    {
                        sudokuAsString = PredefinedMediumSudokus.GetPredefinedSudokuMedium(rnd.Next(0, MAX_PREDEFINED_SUDOKUS));
                    }
                    if (lastSudokuDifficulty == 4)
                    {
                        sudokuAsString = PredefinedHardSudokus.GetPredefinedSudokuHard(rnd.Next(0, MAX_PREDEFINED_SUDOKUS));
                    }
                    if (lastSudokuDifficulty == 8)
                    {
                        sudokuAsString = PredefinedVeryHardSudokus.GetPredefinedSudokuVeryHard(rnd.Next(0, 100));
                    }
                }
                finally
                {
                    connection.Close();
                }


                for (int i = 1; i <= 81; i++)
                {
                    if (sudokuAsString[i - 1] != '0')
                    {
                        document.GetElementById("textBox" + (i + start)).InnerText = sudokuAsString[i - 1].ToString();
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
            var mainWindow = (FindName("MainWindowName") as Window);
            mainWindow.AllowsTransparency = true;
            mainWindow.WindowStyle = WindowStyle.None;
            mainWindow.Background = new SolidColorBrush(Color.FromRgb(237, 240, 242));
            mainWindow.Background.Opacity = 0.95;

            //Buttons Color And Mouse Events
            AddColorAndMouseEventsToButton("buttonEasySudoku");
            AddColorAndMouseEventsToButton("buttonMediumSudoku");
            AddColorAndMouseEventsToButton("buttonHardSudoku");
            AddColorAndMouseEventsToButton("buttonVeryHardSudoku");
            AddColorAndMouseEventsToButton("buttonSolveSudoku");
            AddColorAndMouseEventsToButton("buttonChecker");
            AddColorAndMouseEventsToButton("buttonPrint");
            AddColorAndMouseEventsToButton("buttonClear");
            AddColorAndMouseEventsToButton("MinimizeButton");
            AddColorAndMouseEventsToCloseButton("CloseButton");

            StartAsDisabledButton("buttonSolveSudoku");
            StartAsDisabledButton("buttonChecker");
            StartAsDisabledButton("buttonPrint");
            StartAsDisabledButton("buttonClear");

            //TextBoxes Color
            for (int i = 0; i < 81; i++)
            {
                var textBox = FindName("TextBox" + i) as TextBox;
                textBox.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                textBox.IsEnabled = false;
                textBox.Opacity = 0.7;
                textBox.MouseEnter += new MouseEventHandler(EnterTextBox);
                textBox.MouseLeave += new MouseEventHandler(LeaveTextBox);
            }

            //TimeBoxes
            AddTimeTextBoxesColorsAndDisableThem("textBoxHours");
            AddTimeTextBoxesColorsAndDisableThem("textBoxMinutes");
            AddTimeTextBoxesColorsAndDisableThem("textBoxSeconds");

            DisableLabel("labelHours");
            DisableLabel("labelMinutes");
            DisableLabel("labelSeconds");

            //DisableCheckbox
            DisableCheckbox("checkBoxHints");
        }

        

        private void StartAsDisabledButton(string name)
        {
            var button = FindName(name) as Button;
            //button.Background.Opacity = 0.001;
            //button.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            //button.Foreground.Opacity = 0.001;
            button.Opacity = 0.001;
            button.IsEnabled = false;
        }

        private void AddTimeTextBoxesColorsAndDisableThem(string input)
        {
            TextBox textBox = FindName(input) as TextBox;
            textBox.BorderThickness = new Thickness(0);
            textBox.Background = new SolidColorBrush(Color.FromRgb(222, 228, 229));
            textBox.IsReadOnly = true;
            textBox.Background.Opacity = 0.005;
            textBox.Opacity = 0.005;
            textBox.TextAlignment = TextAlignment.Center;
            textBox.IsEnabled = false;
        }
        private void DisableLabel(string input)
        {
            var label = FindName(input) as Label;
            label.Opacity = 0.001;
        }
        private void DisableCheckbox(string input)
        {
            var checkBox = FindName(input) as CheckBox;
            checkBox.Opacity = 0.001;
            checkBox.IsEnabled = false;
        }

        private void AddColorAndMouseEventsToButton(string buttonName)
        {
            Button button = FindName(buttonName) as Button;
            button.BorderThickness = new Thickness(0);
            button.Background = new SolidColorBrush(Color.FromRgb(222, 227, 229));
            button.MouseEnter += new MouseEventHandler(EnterButton);
            button.MouseLeave += new MouseEventHandler(LeaveButton);

        }

        private void AddColorAndMouseEventsToCloseButton(string buttonName)
        {
            Button button = FindName(buttonName) as Button;
            button.BorderThickness = new Thickness(0);
            button.Background = new SolidColorBrush(Color.FromRgb(222, 227, 229));
            button.MouseEnter += new MouseEventHandler(EnterCloseButton);
            button.MouseLeave += new MouseEventHandler(LeaveButton);
        }


        private void LeaveButton(object sender, MouseEventArgs e)
        {
            ColorAnimation animation = new ColorAnimation();
            var button = sender as Button;

            animation.To = Color.FromRgb(222, 227, 229);
            animation.Duration = new Duration(TimeSpan.FromSeconds(1));
            button.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }

        private void EnterCloseButton(object sender, MouseEventArgs e)
        {
            ColorAnimation animation = new ColorAnimation();
            var button = sender as Button;

            animation.To = Color.FromRgb(226, 29, 29);
            animation.Duration = new Duration(TimeSpan.FromSeconds(1));
            button.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }

        private void EnterButton(object sender, MouseEventArgs e)
        {
            ColorAnimation animation = new ColorAnimation();
            var button = sender as Button;

            animation.To = Color.FromRgb(174, 203, 232);
            animation.Duration = new Duration(TimeSpan.FromSeconds(1));
            button.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }

        private void LeaveTextBox(object sender, MouseEventArgs e)
        {
            ColorAnimation animation = new ColorAnimation();
            var textBox = sender as TextBox;

            if (textBox.IsReadOnly == true)
            {
                animation.To = Color.FromRgb(222, 227, 229);
            }
            else
            {
                animation.To = Color.FromRgb(255, 255, 255);
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
                        toolTip.Background = new SolidColorBrush(Color.FromRgb(255, 250, 252));
                        toolTip.Content = string.Format("You can put those numbers: {0}!", result);
                        textBox.ToolTip = toolTip;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }



            animation.To = Color.FromRgb(227, 232, 234);
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            textBox.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }

        private void ShowDisabledItems()
        {
            for (int i = 0; i < 81; i++)
            {
                var textBox = FindName("TextBox" + i) as TextBox;
                textBox.IsEnabled = true;

            }

            EnableDisabledButton("buttonSolveSudoku");
            EnableDisabledButton("buttonChecker");
            EnableDisabledButton("buttonPrint");
            EnableDisabledButton("buttonClear");

            EnableLabel("labelHours");
            EnableLabel("labelMinutes");
            EnableLabel("labelSeconds");

            EnableTimeTextBoxButton("textBoxHours");
            EnableTimeTextBoxButton("textBoxMinutes");
            EnableTimeTextBoxButton("textBoxSeconds");

            EnableCheckbox("checkBoxHints");
        }
        private void EnableTimeTextBoxButton(string input)
        {
            var textBox = FindName(input) as TextBox;
            textBox.Opacity = 1.0;
            textBox.IsEnabled = true;
        }
        private void EnableLabel(string input)
        {
            var label = FindName(input) as Label;
            Storyboard storyboard = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation();
            animation.To = 1.0;
            animation.Duration = new Duration(TimeSpan.FromSeconds(2.0));
            Storyboard.SetTargetName(animation, label.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Control.OpacityProperty));
            storyboard.Children.Add(animation);
            storyboard.Begin(this);            
        }
        private void EnableDisabledButton(string name)
        {
            var button = FindName(name) as Button;
            Storyboard storyboard = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation();
            animation.To = 1.0;
            animation.Duration = new Duration(TimeSpan.FromSeconds(2.0));
            Storyboard.SetTargetName(animation, button.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Control.OpacityProperty));
            storyboard.Children.Add(animation);
            storyboard.Begin(this);

            button.IsEnabled = true;
        }

        private void EnableCheckbox(string input)
        {
            var checkBox = FindName(input) as CheckBox;
            Storyboard storyboard = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation();
            animation.To = 1.0;
            animation.Duration = new Duration(TimeSpan.FromSeconds(2.0));
            Storyboard.SetTargetName(animation, checkBox.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Control.OpacityProperty));
            storyboard.Children.Add(animation);
            storyboard.Begin(this);

            checkBox.IsEnabled = true;
        }
        #endregion
        #region Window Controls
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        #endregion
    }
}
