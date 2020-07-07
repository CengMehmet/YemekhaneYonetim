using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace RTEvents
{
    public partial class Form2 : Form
    {
        public SqlConnection sqlbag, sqlbag2;
        SqlCommand k;
        SqlDataReader rd;
        Main frm;

        public Form2()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            sqlconfigoku();
        }
        public void sqlconfigoku()
        {
            string yazi, yazi2 = "";
            yazi2 = Helper.XmlOku()[0];
            sqlbag = new SqlConnection(yazi2);
            k = new SqlCommand("select pass,adsoyad,yetki from kullanici", sqlbag);
            sqlbag.Open();
            rd = k.ExecuteReader();
            rd.Read();
            sqlbag.Close();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                sifrekontrol();
            }
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            sifrekontrol();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {                   
            Application.ExitThread();
            Application.Exit();
        }
    
        public void sifrekontrol()
        {
            try
            {
                k = new SqlCommand("select pass,adsoyad,yetki from kullanici WHERE kullaniciadi='" + textBox1.Text + "'", sqlbag);
                sqlbag.Open();
                rd = k.ExecuteReader();
                rd.Read();
                string sifre = rd["pass"].ToString();
                if (sifre == textBox2.Text)
                {
                    frm = new Main();
                    string yetkili = rd["adsoyad"].ToString();
                    string yetki = rd["yetki"].ToString();
                    if (yetki == "0")
                    {
                        frm.pictureBoxkayitlar.Enabled = false;
                        frm.pictureBoxogrenciler.Enabled = false;
                        frm.pictureBoxtarifeler.Enabled = false;
                        frm.pictureBoxayarlar.Enabled = false;
                    }
                    else if (yetki == "1") { yetki = "1"; frm.tabControl1.SelectedIndex = 1; }
                    sqlbag.Close();
                    frm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Hatalı Şifre Girişi");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Giriş İşleminde Hata");
            }
        }

        public Screen GetSecondaryScreen()
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
