using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;

namespace RTEvents
{
    class YemekhaneFonksiyonlar
    {
        public static string _baglanti;
        public static void NullDoldur()
        {
            _baglanti = Helper.XmlOku()[0];
            string[] sutunAdi = { "TC", "ogrenciadi", "ogrencisoyadi", "cinsiyeti", "ogrencisinifi", "ogrencialani", "ogrencisubesi", "sinifogretmeni", "ogrencidurum", "parmakizi", "anneadi", "babaadi", "ogrencitel", "velitel", "adres", "ogrencibakiye", "kalangiris", "ogrencitip","velimail" };
            for (int i = 0; i < sutunAdi.Length; i++)
            {
                if (sutunAdi[i] == "ogrencidurum")
                {
                    using (var conn = new SqlConnection(_baglanti))
                    {
                        conn.Open();
                        SqlCommand command = new SqlCommand("UPDATE ogrenci SET " + sutunAdi[i] + "='" + "0" + "' WHERE (" + sutunAdi[i] + " IS NULL)", conn);
                        command.ExecuteNonQuery();
                    }
                }
                else if (sutunAdi[i] == "ogrencibakiye" || sutunAdi[i] == "kalangiris")
                {
                    using (var conn = new SqlConnection(_baglanti))
                    {
                        conn.Open();
                        SqlCommand command = new SqlCommand("UPDATE ogrenci SET " + sutunAdi[i] + "='" + 0 + "' WHERE (" + sutunAdi[i] + " IS NULL)", conn);
                        command.ExecuteNonQuery();
                    }
                }
                else if (sutunAdi[i] == "ogrencitip")
                {
                    using (var conn = new SqlConnection(_baglanti))
                    {
                        conn.Open();
                        SqlCommand command = new SqlCommand("UPDATE ogrenci SET " + sutunAdi[i] + "='" + "Bakiye" + "' WHERE (" + sutunAdi[i] + " IS NULL)", conn);
                        command.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(_baglanti))
                    {
                        conn.Open();
                        SqlCommand command = new SqlCommand("UPDATE ogrenci SET " + sutunAdi[i] + "=''" + "" + "WHERE " + sutunAdi[i] + " IS NULL", conn);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        

        public static List<string> ComboboxDoldur(string islem)
        {
            _baglanti = Helper.XmlOku()[0];
            List<string> degerler = new List<string>();
            if (islem == "sinif")
            {
                using (var conn = new SqlConnection(_baglanti)) //HAREKET KAYDINDA BUGÜN ÖĞRENCİ GİRİŞİ VAR MI 
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand("SELECT DISTINCT ogrencisinifi FROM ogrenci", conn);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read()) { degerler.Add(reader["ogrencisinifi"].ToString()); }
                }
            }
            else if (islem == "alan")
            {
                using (var conn = new SqlConnection(_baglanti)) //HAREKET KAYDINDA BUGÜN ÖĞRENCİ GİRİŞİ VAR MI 
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand("SELECT DISTINCT ogrencialani FROM ogrenci", conn);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read()) { degerler.Add(reader["ogrencialani"].ToString()); }
                }
            }
            else if (islem == "sube")
            {
                using (var conn = new SqlConnection(_baglanti)) //HAREKET KAYDINDA BUGÜN ÖĞRENCİ GİRİŞİ VAR MI 
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand("SELECT DISTINCT ogrencisubesi FROM ogrenci", conn);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read()) { degerler.Add(reader["ogrencisubesi"].ToString()); }
                }
            }
            return degerler;
        }

        public static List<Ogrenci> Listele(string where = "")
        {
            _baglanti = Helper.XmlOku()[0];
            List<Ogrenci> ogrencilist = new List<Ogrenci>();
            using (SqlConnection conn = new SqlConnection(_baglanti))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM ogrenci " + where, conn);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Ogrenci ogrenci = new Ogrenci()
                    {
                        no = reader["ogrencino"].ToString(),
                        TC = reader["TC"].ToString(),
                        adi = reader["ogrenciadi"].ToString(),
                        soyadi = reader["ogrencisoyadi"].ToString(),
                        cinsiyeti = reader["cinsiyeti"].ToString(),
                        sinifi = reader["ogrencisinifi"].ToString(),
                        alani = reader["ogrencialani"].ToString(),
                        subesi = reader["ogrencisubesi"].ToString(),
                        durumu = reader["ogrencidurum"].ToString(),
                        ogretmeni = reader["sinifogretmeni"].ToString(),
                        anneadi = reader["anneadi"].ToString(),
                        babaadi = reader["babaadi"].ToString(),
                        velitel = reader["velitel"].ToString(),
                        telefonu = reader["ogrencitel"].ToString(),
                        adresi = reader["adres"].ToString(),
                        ogrencibakiye = Convert.ToDouble(reader["ogrencibakiye"].ToString()),
                        kalangiris = Convert.ToInt32(reader["kalangiris"].ToString()),
                        ogrenciTip = reader["ogrencitip"].ToString(),
                        kartno = reader["parmakizi"].ToString(),
                        velimail = reader["velimail"].ToString()
                    };
                    ogrencilist.Add(ogrenci);
                }
            }
            return ogrencilist;
        }

        public static Ogrenci Getir(string ogrenciNo)
        {
            _baglanti = Helper.XmlOku()[0];
            Ogrenci ogrenci = new Ogrenci();
            using (SqlConnection conn = new SqlConnection(_baglanti))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM ogrenci where ogrencino='" + ogrenciNo + "'", conn);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ogrenci = new Ogrenci()
                    {
                        no = reader["ogrencino"].ToString(),
                        TC = reader["TC"].ToString(),
                        adi = reader["ogrenciadi"].ToString(),
                        soyadi = reader["ogrencisoyadi"].ToString(),
                        cinsiyeti = reader["cinsiyeti"].ToString(),
                        sinifi = reader["ogrencisinifi"].ToString(),
                        alani = reader["ogrencisubesi"].ToString(),
                        subesi = reader["ogrencialani"].ToString(),
                        ogretmeni = reader["sinifogretmeni"].ToString(),
                        anneadi = reader["anneadi"].ToString(),
                        babaadi = reader["babaadi"].ToString(),
                        velitel = reader["velitel"].ToString(),
                        telefonu = reader["ogrencitel"].ToString(),
                        adresi = reader["adres"].ToString(),
                        kartno = reader["parmakizi"].ToString(),
                        durumu = reader["ogrencidurum"].ToString(),
                        ogrencibakiye = Convert.ToDouble(reader["ogrencibakiye"].ToString()),
                        kalangiris = Convert.ToInt32(reader["kalangiris"]),
                        ogrenciTip = reader["ogrenciTip"].ToString(),
                        velimail = reader["velimail"].ToString()
                    };
                }
            }
            return ogrenci;
        }

        public static List<Ogrenci> OgrenciGetir(string where)
        {
            _baglanti = Helper.XmlOku()[0];
            List<Ogrenci> ogrenciList = new List<Ogrenci>();
            using (SqlConnection conn = new SqlConnection(_baglanti))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM ogrenci" + where, conn);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Ogrenci ogrenci = new Ogrenci()
                    {
                        no = reader["ogrencino"].ToString(),
                        TC = reader["TC"].ToString(),
                        adi = reader["ogrenciadi"].ToString(),
                        soyadi = reader["ogrencisoyadi"].ToString(),
                        cinsiyeti = reader["cinsiyeti"].ToString(),
                        sinifi = reader["ogrencisinifi"].ToString(),
                        alani = reader["ogrencisubesi"].ToString(),
                        subesi = reader["ogrencialani"].ToString(),
                        ogretmeni = reader["sinifogretmeni"].ToString(),
                        anneadi = reader["anneadi"].ToString(),
                        babaadi = reader["babaadi"].ToString(),
                        velitel = reader["velitel"].ToString(),
                        telefonu = reader["ogrencitel"].ToString(),
                        adresi = reader["adres"].ToString(),
                        kartno = reader["parmakizi"].ToString(),
                        durumu = reader["ogrencidurum"].ToString(),
                        ogrencibakiye = Convert.ToDouble(reader["ogrencibakiye"].ToString()),
                        kalangiris = Convert.ToInt32(reader["kalangiris"]),
                        ogrenciTip = reader["ogrenciTip"].ToString(),
                        velimail = reader["velimail"].ToString()
                    };
                    ogrenciList.Add(ogrenci);
                }
            }
            return ogrenciList;
        }

        public static void Guncelle(Ogrenci ogrenci,string eskiNo)
        {
            using (SqlConnection conn = new SqlConnection(_baglanti))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("Update ogrenci set ogrencino=@ogrencino,TC=@TC, ogrenciadi=@ogrenciadi, ogrencisoyadi=@ogrencisoyadi, cinsiyeti=@cinsiyeti, ogrencisinifi=@ogrencisinifi, ogrencialani=@ogrencialani, ogrencisubesi=@ogrencisubesi, sinifogretmeni=@sinifogretmeni, ogrencidurum=@ogrencidurum, parmakizi=@parmakizi, anneadi=@anneadi, babaadi=@babaadi, ogrencitel=@ogrencitel, velitel=@velitel, adres=@adres, ogrencibakiye=@ogrencibakiye, kalangiris=@kalangiris, ogrencitip=@ogrencitip,velimail=@velimail WHERE ogrencino='" + eskiNo + "'", conn);
                command.Parameters.AddWithValue("ogrencino", ogrenci.no);
                command.Parameters.AddWithValue("TC", ogrenci.TC);
                command.Parameters.AddWithValue("ogrenciadi", ogrenci.adi);
                command.Parameters.AddWithValue("ogrencisoyadi", ogrenci.soyadi);
                command.Parameters.AddWithValue("cinsiyeti", ogrenci.cinsiyeti);
                command.Parameters.AddWithValue("ogrencisinifi", ogrenci.sinifi);
                command.Parameters.AddWithValue("ogrencisubesi", ogrenci.subesi);
                command.Parameters.AddWithValue("ogrencialani", ogrenci.alani);
                command.Parameters.AddWithValue("sinifogretmeni", ogrenci.ogretmeni);
                command.Parameters.AddWithValue("anneadi", ogrenci.anneadi);
                command.Parameters.AddWithValue("babaadi", ogrenci.babaadi);
                command.Parameters.AddWithValue("velitel", ogrenci.velitel);
                command.Parameters.AddWithValue("ogrencitel", ogrenci.telefonu);
                command.Parameters.AddWithValue("adres", ogrenci.adresi);
                command.Parameters.AddWithValue("parmakizi", ogrenci.kartno);
                command.Parameters.AddWithValue("ogrencidurum", ogrenci.durumu);
                command.Parameters.AddWithValue("ogrencibakiye", ogrenci.ogrencibakiye);
                command.Parameters.AddWithValue("kalangiris", ogrenci.kalangiris);
                command.Parameters.AddWithValue("ogrencitip", ogrenci.ogrenciTip);
                command.Parameters.AddWithValue("velimail", ogrenci.velimail);
                command.ExecuteNonQuery();
            }
        }

        public static void Kaydet(Ogrenci ogrenci)
        {
            using (SqlConnection conn = new SqlConnection(_baglanti))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("INSERT INTO ogrenci " +
                    "(ogrencino, TC, ogrenciadi, ogrencisoyadi, cinsiyeti, ogrencisinifi, ogrencialani, ogrencisubesi, " +
                    "sinifogretmeni, ogrencidurum, parmakizi, anneadi, babaadi, ogrencitel, velitel, adres, ogrencibakiye, " +
                    "kalangiris, ogrencitip,velimail)" +
                    " VALUES (@ogrencino,@TC,@ogrenciadi,@ogrencisoyadi,@cinsiyeti,@ogrencisinifi,@ogrencialani,@ogrencisubesi," +
                    "@sinifogretmeni,@ogrencidurum,@parmakizi,@anneadi,@babaadi,@ogrencitel,@velitel,@adres,@ogrencibakiye," +
                    "@kalangiris,@ogrencitip,@velimail)", conn);
                command.Parameters.AddWithValue("ogrencino", ogrenci.no);
                command.Parameters.AddWithValue("TC", ogrenci.TC);
                command.Parameters.AddWithValue("ogrenciadi", ogrenci.adi);
                command.Parameters.AddWithValue("ogrencisoyadi", ogrenci.soyadi);
                command.Parameters.AddWithValue("cinsiyeti", ogrenci.cinsiyeti);
                command.Parameters.AddWithValue("ogrencisinifi", ogrenci.sinifi);
                command.Parameters.AddWithValue("ogrencisubesi", ogrenci.subesi);
                command.Parameters.AddWithValue("ogrencialani", ogrenci.alani);
                command.Parameters.AddWithValue("sinifogretmeni", ogrenci.ogretmeni);
                command.Parameters.AddWithValue("anneadi", ogrenci.anneadi);
                command.Parameters.AddWithValue("babaadi", ogrenci.babaadi);
                command.Parameters.AddWithValue("velitel", ogrenci.velitel);
                command.Parameters.AddWithValue("ogrencitel", ogrenci.telefonu);
                command.Parameters.AddWithValue("adres", ogrenci.adresi);
                command.Parameters.AddWithValue("parmakizi", ogrenci.kartno);
                command.Parameters.AddWithValue("ogrencidurum", ogrenci.durumu);
                command.Parameters.AddWithValue("ogrencibakiye", ogrenci.ogrencibakiye);
                command.Parameters.AddWithValue("kalangiris", ogrenci.kalangiris);
                command.Parameters.AddWithValue("ogrencitip", ogrenci.ogrenciTip);
                command.Parameters.AddWithValue("velimail", ogrenci.velimail);
                command.ExecuteNonQuery();
            }
        }

        //public static void ExcelEkle(Ogrenci ogrenci2)
        //{
        //    Ogrenci ogrenci = ogrenci2;
        //    string dosya_yolu = "ogrencitablo.xls";
        //    OleDbCommand komut = new OleDbCommand();
        //    OleDbConnection baglanti = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + dosya_yolu + "; Extended Properties = Excel 12.0");
        //    baglanti.Open();
        //    komut.Connection = baglanti;
        //    string sql = "INSERT INTO [Sayfa1$] (ogrencino, TC, ogrenciadi," +
        //        " ogrencisoyadi, cinsiyeti, ogrencisinifi, ogrencialani, " +
        //        "ogrencisubesi, sinifogretmeni, ogrencidurum, parmakizi," +
        //        " anneadi, babaadi, ogrencitel, velitel," +
        //        " adres, bakiye, kalangiris, ogrencitip)" +
        //        " VALUES ('" + ogrenci.no + "','" + ogrenci.TC + "','" + ogrenci.adi + "','" + ogrenci.soyadi + "','" +
        //    ogrenci.cinsiyeti + "','" + ogrenci.sinifi + "','" + ogrenci.alani + "','" + ogrenci.subesi + "','" + ogrenci.ogretmeni + "','" +
        //    ogrenci.durumu + "','" + ogrenci.kartno + "','" + ogrenci.anneadi + "','" + ogrenci.babaadi + "','" + ogrenci.telefonu + "','" + ogrenci.velitel + "','" +
        //    ogrenci.adresi + "','" +  ogrenci.ogrencibakiye + "','" + ogrenci.kalangiris + "','" +
        //    ogrenci.ogrenciTip + "')"; 
        //    komut.CommandText = sql;
        //    komut.ExecuteNonQuery();
        //    MessageBox.Show("Veriler Excel Dosyasına Eklenmiştir.");
        //    baglanti.Close();
        //}

        public static void OgrenciSil(string ogrenciNo)
        {
            using (SqlConnection conn = new SqlConnection(_baglanti))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("DELETE ogrenci WHERE ogrencino='" + ogrenciNo + "'", conn);
                command.ExecuteNonQuery();
            }
        }

        public static void BakiyeGuncelle(string ogrenciNo,double yuklenecekBakiye)
        {
            
            double bakiye = 0.0;
            using (SqlConnection conn = new SqlConnection(_baglanti))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT ogrencino,ogrencibakiye FROM ogrenci where ogrencino='" + ogrenciNo + "'", conn);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    bakiye = Convert.ToDouble(reader["ogrencibakiye"].ToString());
                }
            }
                bakiye += yuklenecekBakiye;
            using (SqlConnection conn = new SqlConnection(_baglanti))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("UPDATE ogrenci SET ogrencibakiye='" + string.Format("{0:0.0000}", bakiye).Replace(",", ".") + "'where ogrencino='" + ogrenciNo + "'", conn);
                command.ExecuteNonQuery();
            }
        }

        public static void GirisHakkiGuncelle(string ogrenciNo, int yuklenecekHak)
        {
            double bakiye = 0.0;
            using (SqlConnection conn = new SqlConnection(_baglanti))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT ogrencino,kalangiris FROM ogrenci where ogrencino='" + ogrenciNo + "'", conn);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    bakiye = Convert.ToDouble(reader["kalangiris"].ToString());
                }
            }
            bakiye += yuklenecekHak;
            using (SqlConnection conn = new SqlConnection(_baglanti))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("UPDATE ogrenci SET kalangiris='" + bakiye + "'where ogrencino='" + ogrenciNo + "'", conn);
                command.ExecuteNonQuery();
            }
        }

        public static List<string> YemekProgramiGetir()
        {
            List<string> yemeksaatleri = new List<string>();
            using (SqlConnection conn = new SqlConnection(_baglanti))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT sabahbaslamasaati,sabahbitissaati,oglebaslamasaati,oglebitissaati,aksambaslamasaati,aksambitissaati,araogunbaslamasaati,araogunbitissaati FROM program2", conn);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    yemeksaatleri.Add(reader["sabahbaslamasaati"].ToString());
                    yemeksaatleri.Add(reader["sabahbitissaati"].ToString());
                    yemeksaatleri.Add(reader["oglebaslamasaati"].ToString());
                    yemeksaatleri.Add(reader["oglebitissaati"].ToString());
                    yemeksaatleri.Add(reader["aksambaslamasaati"].ToString());
                    yemeksaatleri.Add(reader["aksambitissaati"].ToString());
                    yemeksaatleri.Add(reader["araogunbaslamasaati"].ToString());
                    yemeksaatleri.Add(reader["araogunbitissaati"].ToString());
                }
            }
            return yemeksaatleri;
        }

        public static void YemekProgramiGuncelle(string where)
        {
            _baglanti = Helper.XmlOku()[0];
            using (var conn = new SqlConnection(_baglanti))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("DELETE FROM program2", conn);
                command.ExecuteNonQuery();
            }
            using (var conn = new SqlConnection(_baglanti))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("INSERT INTO program2 (sabahbaslamasaati, sabahbitissaati, oglebaslamasaati, oglebitissaati, aksambaslamasaati, aksambitissaati,araogunbaslamasaati,araogunbitissaati) VALUES('" + where, conn);
                command.ExecuteNonQuery();
            }

        }

        public static List<Ogrenci> HareketGetir(string where = "")
        {
            _baglanti = Helper.XmlOku()[0];
            List<Ogrenci> hareketList = new List<Ogrenci>();
            using (SqlConnection conn = new SqlConnection(_baglanti))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT ogrencino,adsoyad,tarih,islem FROM hareketkaydi " + where, conn);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Ogrenci ogrenci = new Ogrenci()
                    {
                        no = reader["ogrencino"].ToString(),
                        adi = reader["adsoyad"].ToString(),
                        yemekgirisTarih = Convert.ToDateTime(reader["tarih"].ToString()),
                        yemekTipi = reader["islem"].ToString(),
                    };
                    hareketList.Add(ogrenci);
                }
            }
            return hareketList;
        }

        

        public static int HareketSayiGetir(string where = "")
        {
            _baglanti = Helper.XmlOku()[0];
            int sayi = 0;
            using (SqlConnection conn = new SqlConnection(_baglanti))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT ogrencino,adsoyad,tarih,islem FROM hareketkaydi " + where, conn);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    sayi++;
                }
            }
            return sayi;
        }



        public static int tarihleGiris(string where)
        {
            _baglanti = Helper.XmlOku()[0];
            int sayi = 0;
            using (SqlConnection conn = new SqlConnection(_baglanti))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT ogrenci.ogrencino, ogrenci.ogrencisubesi, hareketkaydi.tarih FROM ogrenci INNER JOIN hareketkaydi ON ogrenci.ogrencino = hareketkaydi.ogrencino " + where +"", conn);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    sayi++;
                }
            }
            return sayi;
        }
    }
}
