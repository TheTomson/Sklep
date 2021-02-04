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
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;

namespace Sklep
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int kasa;
        public MainWindow()
        {
            InitializeComponent();
            PobierzSklep();
            kasa = 1000;
            kasa_l.Content = kasa;
        }
        public void PobierzSklep()
        {
            using (var connection = new SqliteConnection("Data Source=SklepDB.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT NazwaTowaru,Ilosc,Cena FROM Sklep";
                List<Towary> towary = new List<Towary>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var nazwa = reader.GetString(0);
                        var ilosc = reader.GetString(1);
                        var cena = reader.GetString(2);
                        var towar = new Towary(nazwa, ilosc, cena);
                        towary.Add(towar);
                    }
                }
                sklep.ItemsSource = towary;
            }
        }

        private void KupTowar(object sender, RoutedEventArgs e)
        {
            var kuptowar = new KupTowar();
            kuptowar.Show();
        }
    }
    public class Towary
    {
        public string NazwaTowaru { get; set; }
        public string Ilosc { get; set; }
        public string Cena { get; set; }

        public Towary(string a, string b, string c)
        {
            NazwaTowaru = a;
            Ilosc = b;
            Cena = c;
        }
    }
}
