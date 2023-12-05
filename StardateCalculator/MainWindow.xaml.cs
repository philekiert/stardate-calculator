using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace StardateCalculator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        void Print(Stardate stardate)
        {
            lblTOS.Content = "TOS: " + stardate.FormatStardate(stardate.TOS);
            lblTMP.Content = "TMP: " + stardate.FormatStardate(stardate.TMP);
            lblFilms.Content = "Films: " + stardate.FormatStardate(stardate.Films);
            lblTNG.Content = "TNG: " + stardate.FormatStardate(stardate.TNG);
            lblPrimary.Content = "Primary: " + stardate.FormatStardate(stardate.Primary);
        }

        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (txtStardate.Text != "")
            {
                try
                {
                    double real;
                    double.TryParse(txtStardate.Text, out real);

                    Print(new Stardate(real));

                }
                catch { }
            }
            else
            {
                try
                {
                    int year, month, day, hour, minute;
                    int.TryParse(txtYear.Text, out year);
                    int.TryParse(txtMonth.Text, out month);
                    int.TryParse(txtDay.Text, out day);
                    int.TryParse(txtHour.Text, out hour);
                    int.TryParse(txtMinute.Text, out minute);

                    if (month == 0) month = 1;
                    if (day == 0) day = 1;

                    DateTime dt = new DateTime(year, month, day, hour, minute, 0);
                    Print(new Stardate(dt));
                }
                catch { }
            }
        }
    }
}
