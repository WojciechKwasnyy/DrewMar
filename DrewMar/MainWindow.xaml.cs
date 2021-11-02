using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Controls;
using DrewMar.Domain;

namespace DrewMar
{
    public partial class MainWindow : MetroWindow
    {
        private Transport _transport;
        private Package _package;
        private TextBox activeTextBox;

        public MainWindow()
        {
            InitializeComponent();
            _transport = new Transport();
            _package = new Package();

            comboBox.Items.Add("DUDEK");
            comboBox.Items.Add("ZAWADA");
            comboBox.Items.Add("TEST");

            activeTextBox = lengthOfADeskTextBox;
            lengthOfADeskTextBox.BorderBrush = Brushes.Black;
            thicknessOfADeskTextBox.BorderBrush = Brushes.Black;
            widthOfADeskTextBox.BorderBrush = Brushes.Black;
            SetActiveTextBox();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(lengthOfADeskTextBox.Text) 
                || string.IsNullOrEmpty(thicknessOfADeskTextBox.Text) 
                || string.IsNullOrEmpty(widthOfADeskTextBox.Text))
            {
                AutoClosingMessageBox.Show("Błąd dodawania deski - błędne dane", "Caption", 5000);
                return;
            }

            var desk = new WoodenBoard
            {
                Length = double.Parse(lengthOfADeskTextBox.Text) / 10,
                Width = (0.01) * double.Parse(widthOfADeskTextBox.Text),
                Thick = (0.001) * double.Parse(thicknessOfADeskTextBox.Text),
                Worker = comboBox.Text
            };

            if (FeatureFlags.OperationsConfirmationsEnabled)
            {
                widthOfADeskTextBox.Text = "";
            }

            desksList.Items.Add("DŁ: " + desk.Length);
            desksList.Items.Add("SZ: " + desk.Width);
            desksList.Items.Add("GR: " + desk.Thick);
            desksList.Items.Add("___________");

            _package.AddDeskToList(desk);
            _package.listOfDesksMakers.Add(desk.Worker);
                  
            actualPackageWeightTextBox.Text = Convert.ToString(_package.Capacity);
            actualTransportWeightTextBox.Text = Convert.ToString(_transport.Capacity + _package.Capacity);
            numberOfDesksTextBox.Text = Convert.ToString(_package.desks.Count);

            if (FeatureFlags.OperationsConfirmationsEnabled)
            {
                AutoClosingMessageBox.Show("Poprawnie dodano deske", "Caption", 500);
            }
            
            SetActiveTextBox();

            if (VisualTreeHelper.GetChildrenCount(desksList) > 0)
            {
                var border = (Border)VisualTreeHelper.GetChild(desksList, 0);
                var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToBottom();
            }
        }

        private void DodajPaczkeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_package.desks.Count != 0)
            {
                _package.listOfDesksMakers = _package.AssignWorkersToPackage(_package.listOfDesksMakers);
                _transport.AddPackage(_package);

                _package = new Package();
                desksList.Items.Clear();

                numberOfPackagesTextBox.Text = Convert.ToString(_transport.PackagesCount);
                actualPackageWeightTextBox.Text = "0";
                numberOfDesksTextBox.Text = "0";
                if (_transport.PackagesCount >= 16)

                {
                    MessageBox.Show("Limit Paczek! Dodaj Transport!", "Dodawanie paczki", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    AutoClosingMessageBox.Show("Dodano paczke", "Caption", 1000);
                }
            }
            else
            {
                MessageBox.Show("Nie dodano paczki - paczka pusta.", "Błąd dodania paczki", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            SetActiveTextBox();
        }

        private void DodajDoTransportuButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Czy na pewno chcesz dodać transport ? ", "Dodawanie transportu", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (_transport.packages.Count > 0 & result == MessageBoxResult.Yes)
            {
                _transport.Complete();

                var fileDateFormat = "yyyy-dd-M-HH-mm";
                var filePath = $@"C:\Users\Cat\Desktop\Transporty\Transport - {_transport.CompletionTime.ToString(fileDateFormat)}.xlsx";

                if (!FeatureFlags.ProductionModeEnabled)
                {
                    filePath = $@"C:\Users\Wojtek\Desktop\Transporty\Transport - {_transport.CompletionTime.ToString(fileDateFormat)}.xlsx";
                }

                IFileExporter fileExporter = new XlsxFileExporter();
                fileExporter.Export(filePath, _transport);

                _transport = new Transport();

                numberOfPackagesTextBox.Text = Convert.ToString(_transport.PackagesCount);
                actualTransportWeightTextBox.Text = Convert.ToString(_transport.Capacity);
                AutoClosingMessageBox.Show("Dodano nowy transport", "Caption", 2000);
                SetActiveTextBox();
            }
            else if (result == MessageBoxResult.No)
            {
                AutoClosingMessageBox.Show("Anulowano dodawanie transportu", "Caption", 1000);
            }
            else
            {
                MessageBox.Show("Brak paczek, aby dodać transport - nie dodano transportu", "Dodawanie transportu", MessageBoxButton.OK);
            }
        }

        private void ButtonDeleteLastDeskClicked(object sender, RoutedEventArgs e)
        {
            if (_package.desks.Count > 0)
            {
                _package.DeleteLastDeskFromList(_package.desks[_package.desks.Count - 1]);
                desksList.Items.RemoveAt(desksList.Items.Count - 1);
                desksList.Items.RemoveAt(desksList.Items.Count - 1);
                desksList.Items.RemoveAt(desksList.Items.Count - 1);
                desksList.Items.RemoveAt(desksList.Items.Count - 1);

                _package.listOfDesksMakers.RemoveAt(_package.listOfDesksMakers.Count - 1);

                actualPackageWeightTextBox.Text = Convert.ToString(_package.Capacity);
                actualTransportWeightTextBox.Text = Convert.ToString(_transport.Capacity + _package.Capacity);
                numberOfDesksTextBox.Text = Convert.ToString(_package.desks.Count);
                AutoClosingMessageBox.Show("Usunięto ostatnią deskę", "Caption", 1000);
            }
            else
            {
                AutoClosingMessageBox.Show("Paczka pusta, nie można usunąć deski", "Caption", 5000);
            }
        }

        private void OnNumericalButtonClick(object sender, RoutedEventArgs e)
        {
            if (((activeTextBox == lengthOfADeskTextBox || activeTextBox == widthOfADeskTextBox) & activeTextBox.Text.Length < 2)
                || activeTextBox == thicknessOfADeskTextBox & activeTextBox.Text.Length < 3)
            {
                var button = (Button)sender;
                activeTextBox.Text += button.Content.ToString();
            }
        }

        private void LenghtOfADeskTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            activeTextBox = (TextBox)sender;
        }

        private void EnterButtonClick(object sender, RoutedEventArgs e)
        {
            if (activeTextBox == lengthOfADeskTextBox)
            {
                lengthOfADeskTextBox.BorderThickness = new Thickness(0.5, 0.5, 0.5, 0.5);

                activeTextBox = widthOfADeskTextBox;

                widthOfADeskTextBox.BorderThickness = new Thickness(3, 3, 3, 3);
            }
            else if (activeTextBox == widthOfADeskTextBox)
            {
                widthOfADeskTextBox.BorderThickness = new Thickness(0.5, 0.5, 0.5, 0.5);

                activeTextBox = thicknessOfADeskTextBox;

                thicknessOfADeskTextBox.BorderThickness = new Thickness(3, 3, 3, 3);
            }
            else if (activeTextBox == thicknessOfADeskTextBox)
            {
                thicknessOfADeskTextBox.BorderThickness = new Thickness(0.5, 0.5, 0.5, 0.5);

                activeTextBox = lengthOfADeskTextBox;

                lengthOfADeskTextBox.BorderThickness = new Thickness(3, 3, 3, 3);
            }
        }

        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (activeTextBox.Text.Length > 0)
            {
                activeTextBox.Text = activeTextBox.Text.Remove(activeTextBox.Text.Length - 1);
            }

        }
        private void SetActiveTextBox()
        {
            activeTextBox = lengthOfADeskTextBox;

            lengthOfADeskTextBox.BorderThickness = new Thickness(3, 3, 3, 3);
            thicknessOfADeskTextBox.BorderThickness = new Thickness(0.5, 0.5, 0.5, 0.5);
            widthOfADeskTextBox.BorderThickness = new Thickness(0.5, 0.5, 0.5, 0.5);
        }
    }
}
