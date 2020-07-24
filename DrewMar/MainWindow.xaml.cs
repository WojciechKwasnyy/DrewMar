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

namespace DrewMar
{
    class woodenBoard
    {
        
        public int length { get; set; }
        public int width { get; set; }
        public int thick { get; set; }

        public int weight => length * width * thick;

    }
    public class Package
    {
        public int packageCapacity;
        
        public Package(int capacity) => packageCapacity = capacity;
        IEnumerable<woodenBoard> listOfCurrentDesks;

        public int capacity
        {
            get => packageCapacity;
        }        
    }

    class Transport
    {
        public int transportCapacity;

        public Transport(int capacity) => transportCapacity = capacity;
        IEnumerable<Package> listOfCurrentPackages;

        public int capacity
        {
            get => transportCapacity;
        }
    }
    
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            var transport = new Transport(0);
            var package = new Package(0);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            
            var desk = new woodenBoard(Convert.ToInt32(lenghtOfADeskTextBox.Text), Convert.ToInt32(widthOfADeskTextBox.Text), Convert.ToInt32(thicknessOfADeskTextBox.Text));
            desksList.Items.Add(Convert.ToInt32(lenghtOfADeskTextBox.Text));
            desksList.Items.Add(Convert.ToInt32(widthOfADeskTextBox.Text));
            desksList.Items.Add(Convert.ToInt32(thicknessOfADeskTextBox.Text));
            
        }

        private void dodajPaczkeButton_Click(object sender, RoutedEventArgs e)
        {
            var package = new Package(0);
        }
    }
}
