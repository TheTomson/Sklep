using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Sklep
{
    /// <summary>
    /// Logika interakcji dla klasy Klienci.xaml
    /// </summary>                                   
    public partial class Klienci : Window
    {
        public Klienci()
        {
            InitializeComponent();
        }
        public void DodajKlienci(object sender, RoutedEventArgs e)
        {
            var nazwaKlienta = nazwa_klient.Text;
            var adresKlienta = adres_klient.Text;
            var telefonKlienta = telefon_klient.Text;
            var nipKlienta = nip_kient.Text;

            var query = String.Format("INSERT INTO Klienci VALUES (NULL,'{0}','{1}','{2}','{3}')", nazwa_klient.Text, adres_klient.Text, telefon_klient.Text, nip_kient.Text);
            using (var connection = new SqliteConnection("Data Source=SklepDB.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = query;
                command.ExecuteScalar();
                udane_klient.Content = "Udało się! Dodałeś nowego klienta do bazy danych!";
            }
        }
    }
}
