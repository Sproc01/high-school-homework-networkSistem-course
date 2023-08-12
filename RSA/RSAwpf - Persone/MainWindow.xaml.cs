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
        List<RsaClass> RSA;
        public MainWindow()
        {
            RSA = new List<RsaClass>();
            RSA.Clear();
            InitializeComponent();
            //rendo invisibili alcuni tasti
            btnCripta.Visibility = Visibility.Hidden;
            btnDecripta.Visibility= Visibility.Hidden;
            btnSalva.Visibility = Visibility.Hidden;
        }
        private void AggiornaCombo()
        {//aggiorna sorgente combobox
            cmb1.ItemsSource = RSA;
            cmb1.SelectedIndex = 0;
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

        private void btnCripta_Click(object sender, RoutedEventArgs e)
        {
            string messaggio = txtCifrare.Text;
            txtCifrato.Clear();
            for (int i = 0; i < messaggio.Length; i++)
            {
                //cripto carattere per carattere,
                //scrivendo il valore numerico criptato seguito da un "|"
                //che serve per determinare che la parte numerica precedente corrisponde ad un carattere
                txtCifrato.Text += RSA[cmb1.SelectedIndex].Cifra(messaggio[i])+"|";
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
                    txtDecifra.Text += (char)RSA[cmb1.SelectedIndex].Decifra(b);
                }

            }
        }

        private void btnSalva_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "(*.rsa)|*.rsa";
            saveFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            MessageBox.Show("Salvare chiavi di " + RSA[cmb1.SelectedIndex]);
            if (saveFile.ShowDialog() == true)
            {
                RSA[cmb1.SelectedIndex].Salva(saveFile.FileName);
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
                RsaClass val = new RsaClass(openFile.FileName);
                if (!Ricerca(val))
                {
                    RSA.Add(val);
                    AggiornaCombo();
                    //rendo visibili i tasti
                    btnCripta.Visibility = Visibility.Visible;
                    btnDecripta.Visibility = Visibility.Visible;
                    btnSalva.Visibility = Visibility.Visible;
                    //output valori
                }
                else
                    MessageBox.Show("Elemento già presente");
                txtN.Text = RSA[cmb1.SelectedIndex].ChiavePubblica.N.ToString();
                txtPriv.Text = RSA[cmb1.SelectedIndex].ChiavePrivata.N.ToString() + "," + RSA[cmb1.SelectedIndex].ChiavePrivata.esponente.ToString();
                txtPubb.Text = RSA[cmb1.SelectedIndex].ChiavePubblica.N.ToString() + "," + RSA[cmb1.SelectedIndex].ChiavePubblica.esponente.ToString();
            }
        }
        private void btninserisci_Click(object sender, RoutedEventArgs e)
        {
            WindowInput WI = new WindowInput();
            WI.ShowDialog();
            if (WI.DialogResult==true)
            {
                if (!Ricerca(WI.Val))
                {
                    RSA.Add(WI.Val);
                    AggiornaCombo();
                    txtN.Text = RSA[cmb1.SelectedIndex].ChiavePubblica.N.ToString();
                    txtPriv.Text = RSA[cmb1.SelectedIndex].ChiavePrivata.N.ToString() + "," + RSA[cmb1.SelectedIndex].ChiavePrivata.esponente.ToString();
                    txtPubb.Text = RSA[cmb1.SelectedIndex].ChiavePubblica.N.ToString() + "," + RSA[cmb1.SelectedIndex].ChiavePubblica.esponente.ToString();
                    btnCripta.Visibility = Visibility.Visible;
                    btnDecripta.Visibility = Visibility.Visible;
                    btnSalva.Visibility = Visibility.Visible;
                }
                else
                    MessageBox.Show("Elemento già presente");
            }
            else
                MessageBox.Show("Operazione annulata");
        }
        private bool Ricerca(RsaClass obj)
        {
            bool a = false ;
            for (int i = 0; i < RSA.Count; i++)
            {
                if(RSA[i].ToString()==obj.ToString())
                {
                    a = true;
                    break;
                }
            }
            return a;
        }

        private void cmb1_DropDownClosed(object sender, EventArgs e)
        {
            if(cmb1.SelectedIndex!=-1)
            {
                txtN.Text = RSA[cmb1.SelectedIndex].ChiavePubblica.N.ToString();
                txtPriv.Text = RSA[cmb1.SelectedIndex].ChiavePrivata.N.ToString() + "," + RSA[cmb1.SelectedIndex].ChiavePrivata.esponente.ToString();
                txtPubb.Text = RSA[cmb1.SelectedIndex].ChiavePubblica.N.ToString() + "," + RSA[cmb1.SelectedIndex].ChiavePubblica.esponente.ToString();
                txtCifrato.Clear();
                txtDecifra.Clear();
            }
        }
    }
}
