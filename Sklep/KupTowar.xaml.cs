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
        private bool obliczone;
        private int kasa;
        private Dictionary<string, SokInformacje> tabela_sokow;
        private MainWindow parent_window;

        public KupTowar(MainWindow parent, ref int k, ref Dictionary<string, SokInformacje> ts)
        {
            InitializeComponent();
            kasa = k;
            tabela_sokow = ts;
            obliczone = false;
            parent_window = parent;
            PobierzSoki();
        }
        public void PobierzSoki()
        {
            List<string> soki = new List<string>();
            foreach (var sok in tabela_sokow)
            {
                soki.Add(sok.Key);
            }
            lista_towar.ItemsSource = soki;
        }

        private void wyznacz_cene(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(ilosc_sokow.Text) > 0)
            {
                var sok = Convert.ToString(lista_towar.SelectedItem);
                var cena_za_szt = tabela_sokow[sok].Cena;
                var cena_calkowita = cena_za_szt * Convert.ToInt32(ilosc_sokow.Text);
                do_zaplatyy.Content = Convert.ToString(cena_calkowita);
                obliczone = true;
            }
            else
            {
                produkt_kupiony.Content = "ilość musi byc większa od zera";
            }
        }

        private void kup_sokii(object sender, RoutedEventArgs e)
        {
            if (obliczone)
            {
                var koszt = Convert.ToInt32(do_zaplatyy.Content);
                if (koszt <= kasa)
                {
                    var sok = Convert.ToString(lista_towar.SelectedItem);
                    var ilosc = Convert.ToInt32(ilosc_sokow.Text);
                    tabela_sokow[sok].Ilosc += ilosc;
                    kasa -= koszt;
                    produkt_kupiony.Content = String.Format("Zakupiłes {0} w ilości  {1}", lista_towar.Text, ilosc_sokow.Text);
                    obliczone = false;
                    parent_window.AktualizujTabeleSokow();
                    parent_window.kasa_l.Content = kasa;
                }
                else
                {
                    produkt_kupiony.Content = "Masz za mało pieniędzy!";
                }
            }
            else
            {
                produkt_kupiony.Content = "Przed zakupem należy obliczyć cenę!";
            }
        }
    }
}
