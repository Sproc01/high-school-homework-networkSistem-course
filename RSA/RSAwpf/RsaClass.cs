using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.IO;

namespace RSAwpf
{
    struct Chiave
    {
        public BigInteger N;
        public BigInteger esponente;//e o d
    }
    class RsaClass
    {
        Chiave Cpubblica;
        Chiave Cprivata;
        BigInteger p;
        BigInteger q;
        //proprietà per accedere alla chiave pubblica e privata dal programma
        public Chiave ChiavePubblica
        {
            get { return Cpubblica; }
        }
        public Chiave ChiavePrivata
        {
            get { return Cprivata; }
        }
        public BigInteger P
        {
            get { return p; }
        }
        public BigInteger Q
        {
            get { return q; }
        }
        //3 costruttori:
        //Primo: serve quando si apre il file
        //Secondo: valore random N
        //Terzo: valore N inserito manualmente
        public RsaClass(string p)
        {
            Apri(p);
        }
        public RsaClass()
        {
            //genero numero casuale: generando tot byte casuali che uso per inizializzare il biginteger
            Cpubblica = new Chiave();
            Cprivata = new Chiave();
            byte[] b = new byte[2];
            Random r = new Random();
            BigInteger bip;
            BigInteger biq;
            do
            {
                r.NextBytes(b);
                bip = BigInteger.Abs(new BigInteger(b));
                r.NextBytes(b);
                biq= BigInteger.Abs(new BigInteger(b));
            } while (!(IsPrimeNumber(biq)&& IsPrimeNumber(bip)));
            p = bip;
            q = biq;

        }
        public RsaClass(BigInteger _p, BigInteger _q)
        {
            Cpubblica = new Chiave();
            Cprivata = new Chiave();
            if (IsPrimeNumber(_p) && IsPrimeNumber(_q))
            {
                p = _p;
                q = _q;
            }
            else
            {
                string s = "";
                if (!IsPrimeNumber(_p))
                    s = "Errore p\n";
                if(!IsPrimeNumber(_q))
                    s += "Errore q";
                throw new Exception(s);
            }
        }

        private bool IsPrimeNumber(BigInteger num)//metodo verifica numero sia primo
        {
            bool prime = true;
            if (num == 0 || num == 1)
                prime = false;
            for (int a = 2; a <= num / 2; a++)
            {
                if (num % a == 0)
                    return false;
            }
            return prime;
        }

        private BigInteger MCD(BigInteger a, BigInteger b)//metodo che calcola MCD tra due numeri
        {
            BigInteger resto;
            while (b != 0)
            {
                resto = a % b;
                a = b;
                b = resto;
            }
            return a;
        }

        public void CalcoloN()//calcola n a partire da p e q
        {
            BigInteger n = p * q;
            Cpubblica.N = n;
            Cprivata.N = n;
        }

        public void CalcoloE()//genero in maniera random E, avendo P,Q
        {
            BigInteger ris = (p - 1) * (q - 1);
            BigInteger resto=0;
            BigInteger r=0;
            BigInteger i;
            do
            {
                i = new Random().Next(2,(int)ris);
                r = MCD(ris, i);
                resto = ris % i;
            } while (r != 1 || resto == 0);
            Cpubblica.esponente = i;
        }

        public void CalcoloD()//calcolo D avendo E,P,Q (Metodo Euclide esteso)
        {
            BigInteger[,] matrice = new BigInteger[3, 3];
            matrice[0, 0] = (p - 1) * (q - 1);
            matrice[1, 0] = Cpubblica.esponente;
            matrice[0, 1] = 0;
            matrice[1, 1] = 1;
            matrice[1, 2] = matrice[0, 0] / matrice[1, 0];                     
            matrice[2, 0] = matrice[0, 0] % matrice[1, 0];                      
            matrice[2, 1] = matrice[0, 1] - (matrice[1, 1] * matrice[1,2]);
            while (matrice[2, 0] != 1)
            {
                matrice[0, 0] = matrice[1, 0];
                matrice[1, 0] = matrice[2, 0];
                matrice[0, 1] = matrice[1, 1];
                matrice[1, 1] = matrice[2, 1];
                matrice[1,2] = matrice[0, 0] / matrice[1, 0];                       // (p-1)*(q-1) |0       |
                matrice[2, 0] = matrice[0, 0] % matrice[1, 0];                      //e            |1       |(p-1)*(q-1)/e
                matrice[2, 1] = matrice[0, 1] - (matrice[1, 1] * matrice[1, 2]);    //(p-1)*(q-1)%e|
            }
            if (matrice[2,1] > 0)
                Cprivata.esponente = matrice[2, 1];
            else
                Cprivata.esponente = (p - 1) * (q - 1) + matrice[2, 1];
            if(Cprivata.esponente==Cpubblica.esponente)
            {
                CalcoloE();
                CalcoloD();
            }
        }

        public BigInteger Cifra(int i)//cifra un carattere
        {
            return BigInteger.ModPow(i, Cpubblica.esponente,Cpubblica.N);
        }
        public int Decifra(BigInteger i)//decifra un numero
        {
            return (int)BigInteger.ModPow(i, Cprivata.esponente, Cprivata.N);
        }
        public void Salva(string path)//salva nella stringa di percorso specificata
        {
            FileStream F1 = new FileStream(path, FileMode.OpenOrCreate);
            BinaryWriter bw = new BinaryWriter(F1);
            bw.Write(Cpubblica.N+ "|"+ Cpubblica.esponente+"|");
            bw.Write(Cprivata.N + "|" + Cprivata.esponente + "|");
            bw.Write(p + "|" + q);
            bw.Close();
        }
        public void Apri(string path)//Apre il file corrispondente alla stringa di percorso specificata
        {
            FileStream F1 = new FileStream(path, FileMode.OpenOrCreate);
            BinaryReader br = new BinaryReader(F1);
            string[] s=br.ReadString().Split('|');
            BigInteger.TryParse(s[0], out Cpubblica.N);
            BigInteger.TryParse(s[1], out Cpubblica.esponente);
            s = br.ReadString().Split('|');
            BigInteger.TryParse(s[0], out Cprivata.N);
            BigInteger.TryParse(s[1], out Cprivata.esponente);
            s = br.ReadString().Split('|');
            BigInteger.TryParse(s[0], out p);
            BigInteger.TryParse(s[1], out q);
        }
    }
}
