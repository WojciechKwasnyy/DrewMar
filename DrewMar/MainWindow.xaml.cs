using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.ComponentModel;
using System.IO;
using System.Globalization;
using ClosedXML.Excel;

namespace DrewMar
{
    public class WoodenBoard
    {
        public double Length { get; set; }
        public double Width { get; set; }
        public double Thick { get; set; }

        public string Worker { get; set; }
        public double Weight => Length * Width * Thick;

    }
    public class Package
    {
        public double packageCapacity;
        public List<WoodenBoard> listOfCurrentDesks = new List<WoodenBoard>();
        public List<String> listOfDesksMakers = new List<String>();

        public Package(double capacity) => packageCapacity = capacity;


        public double capacity
        {
            get => packageCapacity;
        }
        public void addDeskToList(WoodenBoard woodenBoard)
        {
            listOfCurrentDesks.Add(woodenBoard);
            packageCapacity += woodenBoard.Weight;
            packageCapacity = Math.Round(packageCapacity, 4);
        }
        public void deleteLastDeskFromList(WoodenBoard woodenBoard)
        {
            if (listOfCurrentDesks != null)
            {
                listOfCurrentDesks.RemoveAt(listOfCurrentDesks.Count - 1);
                packageCapacity -= woodenBoard.Weight;
                packageCapacity = Math.Round(packageCapacity, 4);
            }
            else
            {
                AutoClosingMessageBox.Show("Nie można usunąć deski, paczka pusta!", "Caption", 2000);
            }
        }
        public List<String> assignWorkersToPackage(List<String> list)
        {
            List<String> tmp = list.Distinct().ToList();
             
            return tmp;
        }
        public String printAssignedWorkers()
        {
            String s = "";
            foreach (var item in listOfDesksMakers)
            {
                s += item + " ";
            }
            return s;
        }

    }

    class Transport
    {
        public double transportCapacity = 0;
        public int numberOfPackages => listOfCurrentPackages.Count() + 1;
        public List<Package> listOfCurrentPackages = new List<Package>();
        public IXLWorkbook workbook = new XLWorkbook();
        public readonly IXLWorksheet _worksheet = null;

        public int currentExcelCellVerticalPosition = 1;
        public int currentExcelCellHorizontalPosition = 1;

        public Transport(double capacity)
        {
            _worksheet = workbook.Worksheets.Add("paczki");
            transportCapacity = capacity;
        }
        public double capacity
        {
            get => transportCapacity;
        }
        public void addPackageToTransportList(Package package)
        {
            transportCapacity += package.packageCapacity;
            listOfCurrentPackages.Add(package);
        }
        public void updateCurrentExcelCellPosition(int vertical, int horizontal)
        {
            if (currentExcelCellHorizontalPosition + horizontal < 17)
            {
                currentExcelCellVerticalPosition += vertical;
                currentExcelCellHorizontalPosition += horizontal;
            }
            else
            {
                currentExcelCellHorizontalPosition = 2;
                currentExcelCellVerticalPosition += 1;
            }
        }
        public void updateCurrentExcelCellPositionAfterPackage(int numberOfDesks, int startCellVert, int startCellHor)
        {
            if (numberOfDesks < 31)
            {
                currentExcelCellVerticalPosition = startCellVert + 6;
            }
            else
            {
                currentExcelCellVerticalPosition += 2;
            }

            currentExcelCellHorizontalPosition -= (currentExcelCellHorizontalPosition - 1);
        }
    }
    public class AutoClosingMessageBox
    {
        System.Threading.Timer _timeoutTimer;
        string _caption;
        AutoClosingMessageBox(string text, string caption, int timeout)
        {
            _caption = caption;
            _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
                null, timeout, System.Threading.Timeout.Infinite);
            using (_timeoutTimer)
                MessageBox.Show(text, caption);
        }
        public static void Show(string text, string caption, int timeout)
        {
            new AutoClosingMessageBox(text, caption, timeout);
        }
        void OnTimerElapsed(object state)
        {
            IntPtr mbWnd = FindWindow("#32770", _caption);
            if (mbWnd != IntPtr.Zero)
                SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            _timeoutTimer.Dispose();
        }
        const int WM_CLOSE = 0x0010;
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
    }

    public partial class MainWindow : MetroWindow
    {
        private Transport _transport;
        private Package _package;
        private int numberOfTransports;
        private TextBox activeTextBox;

        public MainWindow()
        {
            InitializeComponent();
            _transport = new Transport(0);

            _package = new Package(0);

            comboBox.Items.Add("DUDEK");
            comboBox.Items.Add("ZAWADA");
            comboBox.Items.Add("TEST");

            numberOfTransports = 1;

            activeTextBox = lengthOfADeskTextBox;
            lengthOfADeskTextBox.BorderBrush = System.Windows.Media.Brushes.Black;
            thicknessOfADeskTextBox.BorderBrush = System.Windows.Media.Brushes.Black;
            widthOfADeskTextBox.BorderBrush = System.Windows.Media.Brushes.Black;
            setActiveTextBox();
        }


        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (lengthOfADeskTextBox.Text != "" & thicknessOfADeskTextBox.Text != "" & widthOfADeskTextBox.Text != "")
            {
                var desk = new WoodenBoard();

                desk.Length = Double.Parse(lengthOfADeskTextBox.Text) / 10;
                desk.Width = (0.01) * Double.Parse(widthOfADeskTextBox.Text);
                desk.Thick = (0.001) * Double.Parse(thicknessOfADeskTextBox.Text);
                desk.Worker = comboBox.Text;

                //lengthOfADeskTextBox.Text = "";
                //widthOfADeskTextBox.Text = "";


                desksList.Items.Add("DŁ: " + desk.Length);
                desksList.Items.Add("SZ: " + desk.Width);
                desksList.Items.Add("GR: " + desk.Thick);
                desksList.Items.Add("___________");

                _package.addDeskToList(desk);
                _package.listOfDesksMakers.Add(desk.Worker);
                  
                actualPackageWeightTextBox.Text = Convert.ToString(_package.capacity);
                actualTransportWeightTextBox.Text = Convert.ToString(_transport.capacity + _package.capacity);
                numberOfDesksTextBox.Text = Convert.ToString(_package.listOfCurrentDesks.Count);
                //AutoClosingMessageBox.Show("Poprawnie dodano deske", "Caption", 500);
                setActiveTextBox();

                if (VisualTreeHelper.GetChildrenCount(desksList) > 0)
                {
                    Border border = (Border)VisualTreeHelper.GetChild(desksList, 0);
                    ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                    scrollViewer.ScrollToBottom();
                }

                //this.desksList.ScrollIntoView(this.desksList.Items.Count - 1);

            }
            else
            {
                AutoClosingMessageBox.Show("Błąd dodawania deski - błędne dane", "Caption", 5000);
            }
        }

        private void dodajPaczkeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_package.listOfCurrentDesks.Count != 0)
            {
                _package.listOfDesksMakers = _package.assignWorkersToPackage(_package.listOfDesksMakers);
                _transport.addPackageToTransportList(_package);
                _transport._worksheet.Cell(_transport.currentExcelCellVerticalPosition, _transport.currentExcelCellHorizontalPosition).Value = "Paczka nr. " + _transport.listOfCurrentPackages.Count.ToString();
                _transport._worksheet.Cell(_transport.currentExcelCellVerticalPosition + 1, _transport.currentExcelCellHorizontalPosition).Value = "Grubość = " + _package.listOfCurrentDesks[0].Thick.ToString();
                _transport._worksheet.Cell(_transport.currentExcelCellVerticalPosition + 2, _transport.currentExcelCellHorizontalPosition).Value = "Sztuki = " + _package.listOfCurrentDesks.Count;
                _transport._worksheet.Cell(_transport.currentExcelCellVerticalPosition + 3, _transport.currentExcelCellHorizontalPosition).Value = "Masa paczki = " + _package.packageCapacity;                
                _transport._worksheet.Cell(_transport.currentExcelCellVerticalPosition + 4, _transport.currentExcelCellHorizontalPosition).Value = _package.printAssignedWorkers();
                _transport.updateCurrentExcelCellPosition(0, 1);

                _transport._worksheet.PageSetup.PrintAreas.Clear();
                _transport._worksheet.PageSetup.PrintAreas.Add("A1:P200");



                WoodenBoard last = _package.listOfCurrentDesks.Last();
                int startCellVert = _transport.currentExcelCellVerticalPosition;
                int startCellHor = _transport.currentExcelCellHorizontalPosition;

                foreach (WoodenBoard item in _package.listOfCurrentDesks)
                {
                    _transport._worksheet.Cell(_transport.currentExcelCellVerticalPosition, _transport.currentExcelCellHorizontalPosition).Value = item.Length;
                    _transport.updateCurrentExcelCellPosition(1, 0);
                    _transport._worksheet.Cell(_transport.currentExcelCellVerticalPosition, _transport.currentExcelCellHorizontalPosition).Value = item.Width;

                    if (item == last)
                    {
                        _transport.updateCurrentExcelCellPositionAfterPackage(_package.listOfCurrentDesks.Count, startCellVert, startCellHor);
                        _transport._worksheet.Row(_transport.currentExcelCellVerticalPosition - 1).Height = 8;
                    }
                    else
                    {
                        _transport.updateCurrentExcelCellPosition((-1), 1);
                    }
                }

                _package = new Package(0);
                desksList.Items.Clear();

                numberOfPackagesTextBox.Text = Convert.ToString(_transport.numberOfPackages);
                actualPackageWeightTextBox.Text = "0";
                numberOfDesksTextBox.Text = "0";
                if (_transport.numberOfPackages >= 16)

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
            setActiveTextBox();
        }

        private void wagaTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void dodajDoTransportuButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Czy na pewno chcesz dodać transport ? ", "Dodawanie transportu", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (_transport.listOfCurrentPackages.Count > 0 & result == MessageBoxResult.Yes)
            {

                string path = @"C:\Users\Wojtek\Desktop\Transport" + numberOfTransports + ".txt";
                string pathCatepillar = @"C:\Users\Cat\Desktop\Transporty\Transport" + numberOfTransports + ".txt";

                DateTime dateTime = DateTime.UtcNow.Date;
                DateTime dateWithTime = DateTime.UtcNow.AddHours(1);

                String dateAsString = dateTime.ToString("d");

                string excelPath = @"C:\Users\Wojtek\Desktop\Transporty\Transport - " + numberOfTransports + " " + dateAsString + ".xlsx";
                string excelPathCatepillar = @"C:\Users\Cat\Desktop\Transporty\Transport - " + numberOfTransports + " " + dateAsString + ".xlsx";
                _transport._worksheet.Columns("A:P").AdjustToContents();
                _transport._worksheet.Cell(_transport.currentExcelCellVerticalPosition, _transport.currentExcelCellHorizontalPosition).Value = "Transport skompletowany";
                _transport.updateCurrentExcelCellPosition(1, 0);
                _transport._worksheet.Cell(_transport.currentExcelCellVerticalPosition, _transport.currentExcelCellHorizontalPosition).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                _transport._worksheet.Cell(_transport.currentExcelCellVerticalPosition, _transport.currentExcelCellHorizontalPosition).Value = dateWithTime;

                _transport.updateCurrentExcelCellPosition(2, 0);

                _transport._worksheet.SheetView.ZoomScale = 75;

                _transport._worksheet.PageSetup.AdjustTo(80);

                try
                {
                    var fileInfo = new FileInfo(path);
                    using (var streamWriter = fileInfo.CreateText())


                    {
                        streamWriter.WriteLine("Transport " + numberOfTransports.ToString());
                        int index = 0;
                        String str = "";

                        foreach (var item in _transport.listOfCurrentPackages)
                        {
                            index++;
                            streamWriter.WriteLine("Paczka " + index.ToString() + ": ilosc desek w paczce - " + item.listOfCurrentDesks.Count + " | waga paczki - " + item.packageCapacity);
                            streamWriter.Write("Dlugosc - ");
                            foreach (WoodenBoard itemm in item.listOfCurrentDesks)
                            {
                                str = itemm.Length.ToString();
                                streamWriter.Write(str + " ");
                            }
                            streamWriter.WriteLine();
                            streamWriter.Write("Szerokosc - ");
                            foreach (WoodenBoard itemm in item.listOfCurrentDesks)
                            {
                                streamWriter.Write(itemm.Width.ToString() + " ");
                            }
                            streamWriter.WriteLine();
                            streamWriter.Write("Grubosc - ");
                            foreach (WoodenBoard itemm in item.listOfCurrentDesks)
                            {
                                str = itemm.Thick.ToString();
                                streamWriter.Write(str + " ");
                            }
                            streamWriter.WriteLine();
                            streamWriter.Write("Pracownik - ");
                            foreach (WoodenBoard itemm in item.listOfCurrentDesks)
                            {
                                str = itemm.Worker;
                                streamWriter.Write(str + " ");
                            }

                            streamWriter.WriteLine();
                            streamWriter.WriteLine();
                        }

                        streamWriter.WriteLine("Calkowita waga transportu: " + _transport.transportCapacity);

                    }
                }
                catch (Exception Ex)
                {
                    Console.WriteLine(Ex.ToString());
                }



                _transport.workbook.SaveAs(excelPath);
                numberOfTransports++;
                _transport = new Transport(0);


                numberOfPackagesTextBox.Text = Convert.ToString(_transport.numberOfPackages);
                actualTransportWeightTextBox.Text = Convert.ToString(_transport.transportCapacity);
                AutoClosingMessageBox.Show("Dodano nowy transport", "Caption", 2000);
                setActiveTextBox();
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

        private void buttonDeleteLastDeskClicked(object sender, RoutedEventArgs e)
        {
            if (_package.listOfCurrentDesks.Count > 0)
            {
                _package.deleteLastDeskFromList(_package.listOfCurrentDesks[_package.listOfCurrentDesks.Count - 1]);
                desksList.Items.RemoveAt(desksList.Items.Count - 1);
                desksList.Items.RemoveAt(desksList.Items.Count - 1);
                desksList.Items.RemoveAt(desksList.Items.Count - 1);
                desksList.Items.RemoveAt(desksList.Items.Count - 1);

                _package.listOfDesksMakers.RemoveAt(_package.listOfDesksMakers.Count - 1);

                actualPackageWeightTextBox.Text = Convert.ToString(_package.capacity);
                actualTransportWeightTextBox.Text = Convert.ToString(_transport.capacity + _package.capacity);
                numberOfDesksTextBox.Text = Convert.ToString(_package.listOfCurrentDesks.Count);
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

        private void lenghtOfADeskTextBox_GotFocus(object sender, RoutedEventArgs e)
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

        private void deleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (activeTextBox.Text.Length > 0)
            {
                activeTextBox.Text = activeTextBox.Text.Remove(activeTextBox.Text.Length - 1);
            }

        }
        private void setActiveTextBox()
        {
            activeTextBox = lengthOfADeskTextBox;

            lengthOfADeskTextBox.BorderThickness = new Thickness(3, 3, 3, 3);
            thicknessOfADeskTextBox.BorderThickness = new Thickness(0.5, 0.5, 0.5, 0.5);
            widthOfADeskTextBox.BorderThickness = new Thickness(0.5, 0.5, 0.5, 0.5);
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
