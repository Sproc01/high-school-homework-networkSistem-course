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
using System.Windows.Shapes;
using System.Numerics;
using RSAwpf;

namespace RSAwpf
{
    /// <summary>
    /// Logica di interazione per WindowInput.xaml
    /// </summary>
    public partial class WindowInput : Window
    {
        RsaClass RSA;
        public RsaClass Val
        {
            get { return RSA; }
        }
        public WindowInput()
        {
            InitializeComponent();
            btnOK.Visibility = Visibility.Hidden;
            btnCalcola.Visibility = Visibility.Hidden;
        }

        private void btnmanuale_Click(object sender, RoutedEventArgs e)
        {
            BigInteger bp;
            BigInteger bq;
            txtPcorrect.Foreground = Brushes.Green;
            txtPcorrect.Text = "✓";
            txtQcorrect.Foreground = Brushes.Green;
            txtQcorrect.Text = "✓";
            if (txtP.Text != "" && txtQ.Text != "" && txtNome.Text!="" && txtCognome.Text!="")
            {
                bool p = BigInteger.TryParse(txtP.Text, out bp);
                bool q = BigInteger.TryParse(txtQ.Text, out bq);
                try
                {
                    if (p & q)
                    {
                        RSA = new RsaClass(bp, bq, txtNome.Text, txtCognome.Text);
                        //rendo visibili i tasti
                        btnCalcola.Visibility = Visibility.Visible;
                    }
                }
                catch (Exception i)
                {
                    char[] s = i.Message.ToCharArray();
                    if (s.Contains('p'))
                    {
                        txtPcorrect.Foreground = Brushes.Red;
                        txtPcorrect.Text = "X";
                    }
                    if (s.Contains('q'))
                    {
                        txtQcorrect.Foreground = Brushes.Red;
                        txtQcorrect.Text = "X";
                    }
                    MessageBox.Show(i.Message);
                }
            }
            else
            {
                string s = "";
                if (txtP.Text == "")
                {
                    s = "Errore p\n";
                    txtPcorrect.Foreground = Brushes.Red;
                    txtPcorrect.Text = "X";
                }
                if (txtQ.Text == "")
                {
                    s += "Errore q";
                    txtQcorrect.Foreground = Brushes.Red;
                    txtQcorrect.Text = "X";
                }
                MessageBox.Show(s);
            }
        }

        private void Btncasuale_Click(object sender, RoutedEventArgs e)
        {
            if (txtNome.Text != "" && txtCognome.Text != "")
            {
                RSA = new RsaClass(txtNome.Text, txtCognome.Text);
                //rendo visibili i tasti dopo aver inserito n
                btnCalcola.Visibility = Visibility.Visible;
                txtP.Text = RSA.P.ToString();
                txtQ.Text = RSA.Q.ToString();
                txtPcorrect.Text = "";
                txtQcorrect.Text = "";
                btnCalcola.Visibility = Visibility.Visible;
            }
            else
                MessageBox.Show("Manca nome e cognome");
           
        }

        private void btnCalcola_Click(object sender, RoutedEventArgs e)
        {
            RSA.CalcoloN();
            RSA.CalcoloE();
            RSA.CalcoloD();
            //mando in output le due chiavi
            txtN.Text += RSA.ChiavePubblica.N;
            txtPriv.Text = RSA.ChiavePrivata.N.ToString() + "," + RSA.ChiavePrivata.esponente.ToString();
            txtPubb.Text = RSA.ChiavePubblica.N.ToString() + "," + RSA.ChiavePubblica.esponente.ToString();
            btnOK.Visibility = Visibility.Visible;
        }

        private void txtP_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //controllo che vieta la scrittura di caratteri diversi dai numeri nella textbox
            char i = e.Text[0];
            e.Handled = !char.IsDigit(i);
        }

        private void txtNome_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //controllo che vieta la scrittura di caratteri diversi da lettere nella textbox
            char i = e.Text[0];
            e.Handled = !char.IsLetter(i);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
