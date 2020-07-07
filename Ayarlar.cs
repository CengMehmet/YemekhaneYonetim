using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RTEvents
{
    class Ayarlar
    {
        private static string _baglanti = "";
        private static string _haberlesme = "";

        public Ayarlar()
        {
        }

        public string smsusername { get; set; }
        public string smspass { get; set; }
        public string smsheader { get; set; }
        public string smsfirma { get; set; }
        public string girissms { get; set; }
        public int girdisms { get; set; }
        public int bakiyesms { get; set; }
        public int bakiyegoster { get; set; }
        public int turnikedevrede { get; set; }
        public int tekrarkontrol { get; set; }
        public int girissaatkontrol { get; set; }
        public decimal yemekucreti { get; set; }

        public string mailkullaniciadi { get; set; }
        public string mailsifre { get; set; }



        public void AyarGetir()
        {
            _baglanti = Helper.XmlOku()[0];
            using (SqlConnection conn = new SqlConnection(_baglanti))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM ayarlar", conn);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    smsusername = reader["smsusername"].ToString();
                    smspass = reader["smspass"].ToString();
                    smsheader = reader["smsheader"].ToString();
                    smsfirma = reader["smsfirma"].ToString();
                    girissms = reader["girissms"].ToString();
                    girdisms = Convert.ToInt32(reader["girdisms"].ToString());
                    bakiyesms = Convert.ToInt32(reader["bakiyesms"].ToString());
                    bakiyegoster = Convert.ToInt32(reader["bakiyegoster"].ToString());
                    turnikedevrede = Convert.ToInt32(reader["turnikedevrede"].ToString());
                    tekrarkontrol = Convert.ToInt32(reader["tekrarkontrol"].ToString());
                    girissaatkontrol = Convert.ToInt32(reader["girissaatkontrol"].ToString());
                    yemekucreti = Convert.ToDecimal(reader["yemekucreti"].ToString());
                    mailkullaniciadi = reader["mailkullaniciadi"].ToString();
                    mailsifre = reader["mailsifre"].ToString();
                }
            }
        }

        public void AyarGuncelle()
        {
            _baglanti = Helper.XmlOku()[0]; ;
            using (SqlConnection conn = new SqlConnection(_baglanti))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("UPDATE ayarlar SET " +
                                                    "smsusername='" + smsusername + "', " +
                                                    "smspass='" + smspass + "', " +
                                                    "smsheader='" + smsheader + "', " +
                                                    "girissms='" + girissms + "', " +
                                                    "smsfirma='" + smsfirma + "', " +
                                                    "girdisms='" + girdisms + "', " +
                                                    "yemekucreti=@yemekucreti , " +
                                                    "bakiyesms='" + bakiyesms + "', " +
                                                    "bakiyegoster='" + bakiyegoster + "', " +
                                                    "turnikedevrede='" + turnikedevrede + "', " +
                                                    "tekrarkontrol='" + tekrarkontrol + "', " +
                                                    "girissaatkontrol='" + girissaatkontrol + "', " +
                                                    "mailkullaniciadi='" + mailkullaniciadi + "', " +
                                                    "mailsifre='" + mailsifre +
                                                    "'", conn);
                command.Parameters.AddWithValue("@yemekucreti", yemekucreti);
                command.ExecuteNonQuery();
            }
        }
    }
}
