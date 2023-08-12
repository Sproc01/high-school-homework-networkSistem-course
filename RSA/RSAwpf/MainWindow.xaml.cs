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
using System.Numerics;
using Microsoft.Win32;

namespace RSAwpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RsaClass RSA;
        public MainWindow()
        {
            InitializeComponent();
            //rendo invisibili alcuni tasti
            btnCalcola.Visibility = Visibility.Hidden;
            btnCripta.Visibility = Visibility.Hidden;
            btnDecripta.Visibility= Visibility.Hidden;
            btnSalva.Visibility = Visibility.Hidden;
        }

        private void PulisciText()
        {
            txtCifrare.Clear();
            txtCifrato.Clear();
            txtDecifra.Clear();
            txtPriv.Clear();
            txtPubb.Clear();
            txtN.Clear();
        }
        private void btnPQrandom_Click(object sender, RoutedEventArgs e)
        {
            RSA = new RsaClass();
            //rendo visibili i tasti dopo aver inserito n
            btnCalcola.Visibility = Visibility.Visible;
            btnCripta.Visibility = Visibility.Visible;
            btnDecripta.Visibility = Visibility.Visible;
            PulisciText();
            txtP.Text = RSA.P.ToString();
            txtQ.Text = RSA.Q.ToString();
            txtPcorrect.Text = "";
            txtQcorrect.Text = "";
        }

        private void btnPQinserito_Click(object sender, RoutedEventArgs e)
        {
            BigInteger bp;
            BigInteger bq;
            txtPcorrect.Foreground = Brushes.Green;
            txtPcorrect.Text = "✓";
            txtQcorrect.Foreground = Brushes.Green;
            txtQcorrect.Text = "✓";
            if (txtP.Text != "" && txtQ.Text!="")
            {
                bool p=BigInteger.TryParse(txtP.Text, out bp);
                bool q = BigInteger.TryParse(txtQ.Text, out bq);
                try
                {
                    if (p & q)
                    {
                        RSA = new RsaClass(bp, bq);
                        //rendo visibili i tasti
                        btnCalcola.Visibility = Visibility.Visible;
                        btnCripta.Visibility = Visibility.Visible;
                        btnDecripta.Visibility = Visibility.Visible;
                        PulisciText();
                    }
                }
                catch (Exception i)
                {
                    char[] s = i.Message.ToCharArray();
                    if(s.Contains('p'))
                    {
                        txtPcorrect.Foreground = Brushes.Red;
                        txtPcorrect.Text = "X";
                    }
                    if(s.Contains('q'))
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
                if(txtP.Text=="")
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
        private void btnCalcola_Click(object sender, RoutedEventArgs e)
        {
            //effettuo tutti i calcoli
            RSA.CalcoloN();
            RSA.CalcoloE();
            RSA.CalcoloD();
            PulisciText();
            btnSalva.Visibility = Visibility.Visible;
            //mando in output le due chiavi
            txtN.Text += RSA.ChiavePubblica.N;
            txtPriv.Text = RSA.ChiavePrivata.N.ToString() +","+ RSA.ChiavePrivata.esponente.ToString();
            txtPubb.Text = RSA.ChiavePubblica.N.ToString() +","+ RSA.ChiavePubblica.esponente.ToString();
        }

        private void btnCripta_Click(object sender, RoutedEventArgs e)
        {
            string messaggio = txtCifrare.Text;
            txtCifrato.Clear();
            for (int i = 0; i < messaggio.Length; i++)
            {
                //cripto carattere per carattere,
                //scrivendo il valore numerico criptato seguito da un "|"
                //che serve per determinare che la parte numerica precedente corrisponde ad un carattere
                txtCifrato.Text += RSA.Cifra(messaggio[i])+"|";
            }
        }

        private void btnDecripta_Click(object sender, RoutedEventArgs e)
        {
            string[] messaggio = txtCifrato.Text.Split('|');
            txtDecifra.Clear();
            BigInteger b;
            for (int i = 0; i < messaggio.Length; i++)
            {
                //decripto carattere per carattere
                //sfruttando il fatto che un numero seguito dal "|" corrisponde ad un carattere
                if(messaggio[i]!="")
                {
                    BigInteger.TryParse(messaggio[i], out b);
                    txtDecifra.Text += (char)RSA.Decifra(b);
                }

            }
        }

        private void txtN_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //controllo che vieta la scrittura di caratteri diversi dai numeri nella textbox relativa ad N
            char i = e.Text[0];
            e.Handled = !char.IsDigit(i);
        }

        private void btnSalva_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "(*.rsa)|*.rsa";
            saveFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (saveFile.ShowDialog() == true)
            {
                RSA.Salva(saveFile.FileName);
            }
        }

        private void btnapri_Click(object sender, RoutedEventArgs e)
        {
            PulisciText();
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "(*.rsa) |*.rsa";
            openFile.Multiselect = false;
            openFile.CheckFileExists = true;
            openFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (openFile.ShowDialog() == true)
            {
                RSA = new RsaClass(openFile.FileName);
                //rendo visibili i tasti
                btnCalcola.Visibility = Visibility.Visible;
                btnCripta.Visibility = Visibility.Visible;
                btnDecripta.Visibility = Visibility.Visible;
                btnSalva.Visibility = Visibility.Visible;
                //output valori
                txtN.Text = RSA.ChiavePubblica.N.ToString();
                txtPriv.Text = RSA.ChiavePrivata.N.ToString() + "," + RSA.ChiavePrivata.esponente.ToString();
                txtPubb.Text = RSA.ChiavePubblica.N.ToString() + "," + RSA.ChiavePubblica.esponente.ToString();
                txtP.Text = RSA.P.ToString();
                txtQ.Text = RSA.Q.ToString();
                txtPcorrect.Text = "";
                txtQcorrect.Text = "";
            }

        }
    }
}
