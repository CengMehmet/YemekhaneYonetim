using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace RTEvents
{
    public partial class excelsenkronizasyon : Form
    {
        private static string _baglanti;

        OpenFileDialog openfileDialog1 = new OpenFileDialog();
        SqlConnection sqlbag;
        SqlCommand k;
        SqlDataReader rd;
        string sqlbagstring;

        public excelsenkronizasyon()
        {
            InitializeComponent();
            sqlconfigoku();
        }

        public void sqlconfigoku()
        {
            StreamReader oku;
            oku = File.OpenText("sqlconfig.txt");
            string yazi, yazi2 = "";
            while ((yazi = oku.ReadLine()) != null)
            {
                yazi2 = yazi;
            }
            oku.Close();
            sqlbag = new SqlConnection(yazi2);
            sqlbagstring = yazi2;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Equals("EXCEL"))
                {
                    clsProcess.Kill();
                    break;
                }
            }
        }

        private void excelgoster()
        {
            string dosya_yolu = "ogrencitablo.xls";

            OleDbConnection baglanti = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + dosya_yolu + "; Extended Properties = Excel 12.0");
            baglanti.Open();
            string sorgu = "select * from [Sayfa1$] ";
            OleDbDataAdapter data_adaptor = new OleDbDataAdapter(sorgu, baglanti);
            baglanti.Close();

            System.Data.DataTable dt = new System.Data.DataTable();
            data_adaptor.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        public void exceldenguncelle()
        {
            _baglanti = Helper.XmlOku()[0];
            for (int l = 0; l < dataGridView1.Rows.Count - 1; l++)
            {
                int i = 0;
                using (SqlConnection conn = new SqlConnection(_baglanti))
                {
                    conn.Open(); 
                    SqlCommand command = new SqlCommand("Select ogrencino, ogrenciadi,ogrencisoyadi from ogrenci where ogrencino='" + dataGridView1.Rows[l].Cells[0].Value.ToString() + "'", conn);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        i++;
                    }
                }

                

                if (i == 0)
                {
                    using (var conn = new SqlConnection(_baglanti))
                    {
                        conn.Open();
                        SqlCommand command = new SqlCommand("INSERT INTO ogrenci (ogrencino,TC,ogrenciadi,ogrencisoyadi,cinsiyeti,ogrencisinifi,ogrencialani,ogrencisubesi,sinifogretmeni,ogrencidurum,parmakizi,anneadi,babaadi,ogrencitel,velitel,adres,ogrencibakiye,kalangiris,ogrencitip,velimail)VALUES('" +
                         dataGridView1.Rows[l].Cells[0].Value.ToString() + "', '" + //ogrencino
                         dataGridView1.Rows[l].Cells[1].Value.ToString() + "', '" + //TC
                         dataGridView1.Rows[l].Cells[2].Value.ToString() + "', '" + //ogrenciadi
                         dataGridView1.Rows[l].Cells[3].Value.ToString() + "', '" + //ogrencisoyadi
                         dataGridView1.Rows[l].Cells[4].Value.ToString() + "', '" + //cinsiyeti
                         dataGridView1.Rows[l].Cells[5].Value.ToString() + "', '" + //ogrencisinifi
                         dataGridView1.Rows[l].Cells[6].Value.ToString() + "', '" + //ogrencialani
                         dataGridView1.Rows[l].Cells[7].Value.ToString() + "', '" + //ogrencisubesi 
                         dataGridView1.Rows[l].Cells[8].Value.ToString() + "', '" + "0" + "', '" + //sinifogretmeni+ogrencidurum
                         dataGridView1.Rows[l].Cells[10].Value.ToString() + "', '" + //parmakizi
                         dataGridView1.Rows[l].Cells[11].Value.ToString() + "', '" + //anneadi
                         dataGridView1.Rows[l].Cells[12].Value.ToString() + "', '" + //babaadi
                         dataGridView1.Rows[l].Cells[13].Value.ToString() + "', '" + //ogrencitel
                         dataGridView1.Rows[l].Cells[14].Value.ToString() + "', '" + //velitel
                         dataGridView1.Rows[l].Cells[15].Value.ToString() + "', '" + //adres
                         dataGridView1.Rows[l].Cells[16].Value.ToString() + "', '" + //ogrencibakiye
                         dataGridView1.Rows[l].Cells[17].Value.ToString() + "', '" + //kalangiriş
                         dataGridView1.Rows[l].Cells[18].Value.ToString() + "', '" +//ogrencitipi
                         dataGridView1.Rows[l].Cells[19].Value.ToString() +
                         "')", conn);
                        command.ExecuteNonQuery();
                    }
                }
                else if (i == 1)
                {
                    string a = dataGridView1.Rows[l].Cells[16].Value.ToString().Replace(",", ".");
                    using (var conn = new SqlConnection(_baglanti))
                    {
                        conn.Open();
                        SqlCommand command = new SqlCommand("UPDATE ogrenci SET TC='" + dataGridView1.Rows[l].Cells[1].Value.ToString() + "', " +
                        "ogrenciadi='" + dataGridView1.Rows[l].Cells[2].Value.ToString() + "', " +
                        "ogrencisoyadi='" + dataGridView1.Rows[l].Cells[3].Value.ToString() + "', " +
                        "cinsiyeti='" + dataGridView1.Rows[l].Cells[4].Value.ToString() + "', " +
                        "ogrencisinifi='" + dataGridView1.Rows[l].Cells[5].Value.ToString() + "', " +
                        "ogrencialani='" + dataGridView1.Rows[l].Cells[6].Value.ToString() + "', " +
                        "ogrencisubesi='" + dataGridView1.Rows[l].Cells[7].Value.ToString() + "', " +
                        "sinifogretmeni='" + dataGridView1.Rows[l].Cells[8].Value.ToString() + "', " +
                        "ogrencidurum='" + dataGridView1.Rows[l].Cells[9].Value.ToString() + "', " +
                        "parmakizi='" + dataGridView1.Rows[l].Cells[10].Value.ToString() + "', " +
                        "anneadi='" + dataGridView1.Rows[l].Cells[11].Value.ToString() + "', " +
                        "babaadi='" + dataGridView1.Rows[l].Cells[12].Value.ToString() + "', " +
                        "ogrencitel='" + dataGridView1.Rows[l].Cells[13].Value.ToString() + "', " +
                        "velitel='" + dataGridView1.Rows[l].Cells[14].Value.ToString() + "', " +
                        "adres='" + dataGridView1.Rows[l].Cells[15].Value.ToString() + "', " +
                        "ogrencibakiye='" + string.Format("{0:0.0000}", dataGridView1.Rows[l].Cells[16].Value.ToString()).Replace(",", ".") + "', " +
                        "kalangiris='" + dataGridView1.Rows[l].Cells[17].Value + "', " +
                        "ogrencitip='" + dataGridView1.Rows[l].Cells[18].Value.ToString() + "', " + 
                        "velimail='" + dataGridView1.Rows[l].Cells[19].Value.ToString() +
                        "' WHERE ogrencino='" + dataGridView1.Rows[l].Cells[0].Value.ToString() + "'", conn);
                        command.ExecuteNonQuery();
                    }
                }

            }
            string[] sutunAdi = { "TC", "ogrenciadi", "ogrencisoyadi", "cinsiyeti", "ogrencisinifi", "ogrencialani", "ogrencisubesi", "sinifogretmeni", "ogrencidurum", "parmakizi", "anneadi", "babaadi", "ogrencitel", "velitel", "adres", "kalangiris", "ogrencitip" , "ogrencino","velimail" };
            for (int m = 0; m < sutunAdi.Length; m++)
            {
                if (sutunAdi[m] == "ogrencino")
                {
                    if (sqlbag.State == ConnectionState.Open) { sqlbag.Close(); }
                    sqlbag.Open();
                    k = new SqlCommand("DELETE FROM ogrenci WHERE " + sutunAdi[m] + "='" + "" + "'", sqlbag);
                    k.ExecuteNonQuery();
                    sqlbag.Close();
                }
                else if (sutunAdi[m] == "kalangiris")
                {
                    if (sqlbag.State == ConnectionState.Open) { sqlbag.Close(); }
                    sqlbag.Open();
                    k = new SqlCommand("UPDATE ogrenci SET " + sutunAdi[m] + "='" + 0 + "' WHERE (" + sutunAdi[m] + " IS NULL)", sqlbag);
                    k.ExecuteNonQuery();
                    sqlbag.Close();
                }
                else if (sutunAdi[m] == "ogrencidurum")
                {
                    if (sqlbag.State == ConnectionState.Open) { sqlbag.Close(); }
                    sqlbag.Open();
                    k = new SqlCommand("UPDATE ogrenci SET " + sutunAdi[m] + "='" + "DIŞARIDA" + "' WHERE (" + sutunAdi[m] + " IS NULL)", sqlbag);
                    k.ExecuteNonQuery();
                    sqlbag.Close();
                }
                else
                {
                    if (sqlbag.State == ConnectionState.Open) { sqlbag.Close(); }
                    sqlbag.Open();
                    k = new SqlCommand("UPDATE ogrenci SET " + sutunAdi[m] + "=''" + "" + "WHERE " + sutunAdi[m] + " IS NULL", sqlbag);
                    k.ExecuteNonQuery();
                    sqlbag.Close();
                }

            }
            excelgoster();
            MessageBox.Show("Tablo Doldurma İşlemi Tamamlanmıştır.", "Tablo Doldurma");

        }

        private void buttonExcelGoster_Click(object sender, EventArgs e)
        {
            excelgoster();
        }

        private void buttonExcelGuncelle_Click(object sender, EventArgs e)
        {
            exceldenguncelle();
        }
    }
}
