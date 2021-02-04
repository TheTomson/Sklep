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

namespace Sklep
{
    /// <summary>
    /// Logika interakcji dla klasy KupTowar.xaml
    /// </summary>
    public partial class KupTowar : Window
    {
        private int kasa;
        private Dictionary<string, int> tabela_sokow;

        public KupTowar(int k)
        {
            InitializeComponent();
            kasa = k;
            tabela_sokow = new Dictionary<string, int>();
            PobierzSoki();
        }
        public void PobierzSoki()
        {
            using (var connection = new SqliteConnection("Data Source=SklepDB.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT NazwaTowaru,Cena FROM Hurtownia";
                List<string> soki = new List<string>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var nazwa = reader.GetString(0);
                        var cena = Convert.ToInt32(reader.GetString(1));
                        soki.Add(nazwa);
                        tabela_sokow.Add(nazwa, cena);
                    }
                }
                lista_towar.ItemsSource = soki;
            }
        }

        private void wyznacz_cene(object sender, RoutedEventArgs e)
        {
            var sok = Convert.ToString(lista_towar.SelectedItem);
            var cena_za_szt = tabela_sokow[sok];
            var cena_calkowita = cena_za_szt * Convert.ToInt32(ilosc_sokow.Text);
            do_zaplaty.Text = Convert.ToString(cena_calkowita);
        }
    }
}
