using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace RTEvents
{
    public static class Helper
    {
        private static string _baglanti;

        static Helper()
        {
            _baglanti = XmlOku()[0];
        }

        public static List<string> XmlOku()
        {
            List<string> xmList = new List<string>();
            XmlTextReader oku = new XmlTextReader("config.xml");
            while (oku.Read())
            {
                if (oku.NodeType == XmlNodeType.Element)
                {
                    switch (oku.Name)
                    {
                        case "SqlBaglanti":
                            _baglanti = oku.ReadString();
                            xmList.Add(_baglanti);
                            break;
                        case "calisma":
                            xmList.Add(oku.ReadString());
                            break;
                    }
                }
            }
            oku.Close();
            return xmList;
        }

        public static void Veritabaniguncelle()
        {
            using (SqlConnection conn = new SqlConnection(_baglanti))
            {
                conn.Open();
                try { SqlCommand k = new SqlCommand("ALTER TABLE AYARLAR ADD mailkullaniciadi nvarchar(50) NULL DEFAULT 'a@mail.com' WITH VALUES", conn); k.ExecuteNonQuery(); } catch (Exception ex) { }
                try { SqlCommand k = new SqlCommand("ALTER TABLE AYARLAR ADD mailsifre nvarchar(50) DEFAULT '123' WITH VALUES", conn); k.ExecuteNonQuery(); } catch (Exception) { }
                try { SqlCommand k = new SqlCommand("ALTER TABLE ogrenci ADD velimail nvarchar(50)", conn); k.ExecuteNonQuery(); } catch (Exception) { }
            }
        }

        public static void KomutGonder(string komut)
        {
            int Port = 8888;
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            byte[] sendbuf = Encoding.UTF8.GetBytes(komut);
            IPEndPoint ep = new IPEndPoint(IPAddress.Broadcast, Port);
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            s.SendTo(sendbuf, ep);
        }

        public static void HandleException(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                DosyayaYaz(ex.ToString());
            }
        }

        public static string Sqlconfigoku()
        {
            StreamReader oku;
            oku = File.OpenText("sqlconfig.txt");
            string yazi, yazi2 = "";
            while ((yazi = oku.ReadLine()) != null)
            {
                yazi2 = yazi;
            }
            oku.Close();
            SqlConnection sqlbag = new SqlConnection(yazi2);

            try
            {
                sqlbag.Open();
            }
            catch (Exception)
            {
                
            }

            return yazi2;
        }

        public static void DosyayaYaz(string log)
        {
            try
            {
                string dosya_yolu = @"hatalog.txt";
                using (FileStream fs = new FileStream(dosya_yolu, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(DateTime.Now);
                        sw.WriteLine(log);
                        sw.Flush();
                    }
                }
            }
            catch (Exception) { }
        }

        public static Image ResimYukle(string ogrencino)
        {
            Image image1 = null;
            string adres = "c:/resimler/" + ogrencino + ".jpg";
            try
            {
                if (File.Exists(adres))
                {
                    using (FileStream stream = new FileStream("c:/resimler/" + ogrencino + ".jpg", FileMode.Open))
                    {
                        image1 = Image.FromStream(stream);
                        stream.Flush();
                        stream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                DosyayaYaz(ex.ToString());
            }
            return image1;
        }

        public static bool InternetKontrol()
        {
            try
            {
                System.Net.Sockets.TcpClient kontrol_client = new System.Net.Sockets.TcpClient("www.google.com.tr", 80);
                kontrol_client.SendTimeout = 500;
                kontrol_client.Close();
                return true;
            }
            catch (Exception ex)
            {
                Helper.DosyayaYaz(ex.ToString());
                return false;
            }
        }

        public static string[] loadPorts()
        {
            string[] ports = SerialPort.GetPortNames();
            //foreach (string port in ports){comboBoxport.Items.Add(port);}
            return ports;
        }

        public static Screen GetSecondaryScreen()
        {
            if (Screen.AllScreens.Length == 1)
            {
                return null;
            }
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.Primary == false)
                {
                    return screen;
                }
            }
            return null;
        }
    }
}
