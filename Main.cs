using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Data.SqlClient;
using System.IO.Ports;
using System.Net;
using System.IO;
using System.Data.OleDb;
using System.Linq;
using System.Net.Sockets;
using System.Web.Services;
using System.Xml;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net.Mail;

namespace RTEvents
{
    //[WebService(Namespace = "http://gateway.megaokul.com/HERYONESMSWebService.asmx/")]

    public partial class Main : Form
    {
        #region TANIMLAMALAR
        DataGridViewPrinter MyDataGridViewPrinter;
        public SqlConnection sqlbag, sqlbag2, sqlbag3;
        SqlCommand k, k2;
        SqlDataReader rd, rd2;
        TextBox[] baslangicsaatleri; TextBox[] bitissaatleri; GroupBox[] gbgunler;
        com.ttmesaj.ws.Service1 smsClient = new com.ttmesaj.ws.Service1();
        string[] giriscikissaatleri = new string[8];//Yemek Başlangıç Bitiş Saatleri
        private string panoip = "";
        private List<string> maildosyayolu = new List<string>();
        #endregion

        public Main()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            sqlconfigoku();
            Helper.Veritabaniguncelle();
            baslangicdegerlerinial();
            timer1.Start();
        }

        #region TEMEL İŞLEMLER

       
        public string sqlconfigoku()
        {
            string yazi, yazi2 = "";
            yazi2 = Helper.XmlOku()[0];
            sqlbag = new SqlConnection(yazi2);
            sqlbag2 = new SqlConnection(yazi2);
            sqlbag3 = new SqlConnection(yazi2);
            panoip = sqlbag.DataSource;
            return yazi2;
        }

        public void baslangicdegerlerinial()
        {
            try
            {
                baslangicsaatleri = new TextBox[] { textBoxSabahBaslama, textBoxOgleBaslama, textBoxAksamBaslama, textBoxaraogunbaslama };
                bitissaatleri = new TextBox[] { textBoxSabahBitis, textBoxOgleBitis, textBoxAksamBitis, textBoxaraogunbitis };


                YemekhaneFonksiyonlar.ComboboxDoldur("sinif").ForEach(p => comboBox4.Items.Add(p));
                YemekhaneFonksiyonlar.ComboboxDoldur("sinif").ForEach(p => comboBox11.Items.Add(p));
                YemekhaneFonksiyonlar.ComboboxDoldur("sinif").ForEach(p => comboBoxMailSinif.Items.Add(p));
                YemekhaneFonksiyonlar.ComboboxDoldur("alan").ForEach(p => comboBox5.Items.Add(p));
                YemekhaneFonksiyonlar.ComboboxDoldur("alan").ForEach(p => comboBox9.Items.Add(p));
                YemekhaneFonksiyonlar.ComboboxDoldur("alan").ForEach(p => comboBoxMailAlan.Items.Add(p));
                YemekhaneFonksiyonlar.ComboboxDoldur("sube").ForEach(p => comboBox6.Items.Add(p));
                comboBoxIstatistikSube.Items.Add("Tümü");
                YemekhaneFonksiyonlar.ComboboxDoldur("sube").ForEach(p => comboBoxIstatistikSube.Items.Add(p));
                comboBoxIstatistikSube.SelectedIndex = 0;
                YemekhaneFonksiyonlar.ComboboxDoldur("sube").ForEach(p => comboBox3.Items.Add(p));
                YemekhaneFonksiyonlar.ComboboxDoldur("sube").ForEach(p => comboBox10.Items.Add(p));
                YemekhaneFonksiyonlar.ComboboxDoldur("sube").ForEach(p => comboBoxMailSube.Items.Add(p));

                Ayarlar ayarlar = new Ayarlar();
                ayarlar.AyarGetir();
                textBoxnetgsmusername.Text = ayarlar.smsusername;
                textBoxnetgsmpass.Text = ayarlar.smspass;
                textBoxnetgsmheader.Text = ayarlar.smsheader;
                richTextBoxgirissms.Text = ayarlar.girissms;
                checkBox1.Checked = Convert.ToBoolean(ayarlar.girdisms);
                checkBox2.Checked = Convert.ToBoolean(ayarlar.bakiyesms);
                checkBox3.Checked = Convert.ToBoolean(ayarlar.bakiyegoster);
                checkBox9.Checked = Convert.ToBoolean(ayarlar.tekrarkontrol);
                checkBox10.Checked = Convert.ToBoolean(ayarlar.girissaatkontrol);
                textBox21.Text = ayarlar.yemekucreti.ToString();
                comboBoxsmsfirma.Text = ayarlar.smsfirma.ToString();
                textBoxMailKullaniciAdi.Text = ayarlar.mailkullaniciadi;
                textBoxMailSifre.Text = ayarlar.mailsifre;
            }
            catch(Exception ex)
            {
                Helper.DosyayaYaz(ex.ToString());
            }

            
        }

        #endregion

        #region HAREKETLER


        private void pictureBox11_Click(object sender, EventArgs e) //Hareket Kayıtlarında Filtrele
        {
            string giristipi = "";
            if (radioButtonSabahYiyenler.Checked == true) { giristipi = "SABAH"; radioButtonSabahYiyenler.Checked = false; }
            else if (radioButtonOglenYiyenler.Checked == true) { giristipi = "OGLE"; radioButtonOglenYiyenler.Checked = false; }
            else if (radioButtonAksamYiyenler.Checked == true) { giristipi = "AKSAM"; radioButtonAksamYiyenler.Checked = false; }
            else if (radioButtonAraOgunYiyenler.Checked == true) { giristipi = "ARA"; radioButtonAraOgunYiyenler.Checked = false; }
            else { giristipi = ""; }
            byte[] gizlenecek = new byte[] { 2, 4,5,6,7, 8, 9, 10,11, 12, 13,14,15,16,17,18,19 };
            if (textBox22.Text != "")
            {
                if (giristipi != "")
                {                   
                    dataGridView2.DataSource = YemekhaneFonksiyonlar.HareketGetir(" WHERE islem='" + giristipi + "' AND ogrencino = '" + textBox22.Text +
                                              "' AND tarih BETWEEN'" + dateTimePicker4.Text + " 00:00" + "' AND '" +
                                              dateTimePicker3.Text + " 23:59'");
                    if (!dataGridView2.Columns.Contains("Sıra"))
                    {
                        dataGridView2.Columns.Add("Sıra", "Sıra");
                    }
                    for (int i = 0; i < dataGridView2.Rows.Count; i++)
                    {
                        dataGridView2.Rows[i].Cells["Sıra"].Value = i + 1;
                    }
                    dataGridView2.Columns["Sıra"].DisplayIndex = 0;
                    
                    foreach (var index in gizlenecek) { dataGridView2.Columns[index].Visible = false; }
                    label44.Text = dataGridView2.Rows.Count.ToString();
                }
                else
                {
                    dataGridView2.DataSource = YemekhaneFonksiyonlar.HareketGetir(" WHERE ogrencino = '" + textBox22.Text + "' AND tarih BETWEEN'" +
                                         dateTimePicker4.Text + " 00:00" + "' AND '" + dateTimePicker3.Text +
                                         " 23:59'");
                    if (!dataGridView2.Columns.Contains("Sıra"))
                    {
                        dataGridView2.Columns.Add("Sıra", "Sıra");
                    }
                    for (int i = 0; i < dataGridView2.Rows.Count; i++)
                    {
                        dataGridView2.Rows[i].Cells["Sıra"].Value = i + 1;
                    }
                    dataGridView2.Columns["Sıra"].DisplayIndex = 0;
                    foreach (var index in gizlenecek) { dataGridView2.Columns[index].Visible = false; }
                    label44.Text = dataGridView2.Rows.Count.ToString();
                }              
            }
            else
            {               
                if (giristipi != "")
                {
                    dataGridView2.DataSource = YemekhaneFonksiyonlar.HareketGetir(" WHERE islem='" + giristipi + "' AND tarih BETWEEN'" + dateTimePicker4.Text +
                                         " 00:00" + "' AND '" + dateTimePicker3.Text + " 23:59' AND islem='" + giristipi + "'");
                    if (!dataGridView2.Columns.Contains("Sıra"))
                    {
                        dataGridView2.Columns.Add("Sıra", "Sıra");
                    }                    
                    for(int i = 0; i < dataGridView2.Rows.Count; i++)
                    {
                        dataGridView2.Rows[i].Cells["Sıra"].Value = i + 1;
                    }
                    dataGridView2.Columns["Sıra"].DisplayIndex = 0;
                    foreach (var index in gizlenecek) { dataGridView2.Columns[index].Visible = false; }
                    label44.Text = dataGridView2.Rows.Count.ToString();

                }
                else
                {
                    dataGridView2.DataSource = YemekhaneFonksiyonlar.HareketGetir(" WHERE tarih BETWEEN'" + dateTimePicker4.Text + " 00:00" + "' AND '" +
                                         dateTimePicker3.Text + " 23:59'");
                    if (!dataGridView2.Columns.Contains("Sıra"))
                    {
                        dataGridView2.Columns.Add("Sıra", "Sıra");
                    }
                    for (int i = 0; i < dataGridView2.Rows.Count; i++)
                    {
                        dataGridView2.Rows[i].Cells["Sıra"].Value = i + 1;
                    }
                    dataGridView2.Columns["Sıra"].DisplayIndex = 0;
                    foreach (var index in gizlenecek) { dataGridView2.Columns[index].Visible = false; }
                    label44.Text = dataGridView2.Rows.Count.ToString();
                }
            }
        }

        private void hareketGetir()
        {
            byte[] gizlenecek = new byte[] {1, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };
            dataGridView2.DataSource =
                YemekhaneFonksiyonlar.HareketGetir(" WHERE tarih BETWEEN'" + dateTimePicker4.Value.ToString("yyyy-MM-dd") + "' AND '" +
                                     dateTimePicker3.Value.ToString("yyyy-MM-dd") + " 23:59' order by tarih ASC");

            if (!dataGridView2.Columns.Contains("Sıra"))
            {
                dataGridView2.Columns.Add("Sıra", "Sıra");
            }
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                dataGridView2.Rows[i].Cells["Sıra"].Value = i + 1;
            }
            dataGridView2.Columns["Sıra"].DisplayIndex = 0;
            foreach (var index in gizlenecek) { dataGridView2.Columns[index].Visible = false; }
            label44.Text = dataGridView2.Rows.Count.ToString();
        }

        private bool SetupThePrintingHareketler()
        {
            string baslik = "*" + dateTimePicker4.Text + " İLE " + dateTimePicker3.Text + " ARASI HAREKETLER LİSTESİ";
            PrintDialog MyPrintDialog = new PrintDialog();
            MyPrintDialog.AllowCurrentPage = false;
            MyPrintDialog.AllowPrintToFile = false;
            MyPrintDialog.AllowSelection = true;
            MyPrintDialog.AllowSomePages = false;
            MyPrintDialog.PrintToFile = false;
            MyPrintDialog.ShowHelp = false;
            MyPrintDialog.ShowNetwork = false;

            //if (MyPrintDialog.ShowDialog() != DialogResult.OK)
            //    return false;

            printDocument2.DocumentName = "HAREKET KAYITLARI";
            printDocument2.PrinterSettings = MyPrintDialog.PrinterSettings;
            printDocument2.DefaultPageSettings = MyPrintDialog.PrinterSettings.DefaultPageSettings;
            printDocument2.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(40, 40, 80, 40);

            if (MessageBox.Show("Raporu sayfaya ortalamak ister misiniz?",
             "Rapor Ortalaması", MessageBoxButtons.YesNo,
             MessageBoxIcon.Question) == DialogResult.Yes)
                MyDataGridViewPrinter = new DataGridViewPrinter(dataGridView2,
                printDocument2, true, true, "*" + dateTimePicker4.Text + " İLE " + dateTimePicker3.Text + " ARASI HAREKETLER LİSTESİ", new Font("Tahoma", 12,
                FontStyle.Bold, GraphicsUnit.Point), System.Drawing.Color.Black, true);
            else
                MyDataGridViewPrinter = new DataGridViewPrinter(dataGridView2,
                printDocument2, false, true, baslik, new Font("Tahoma", 14,
                FontStyle.Bold, GraphicsUnit.Point), System.Drawing.Color.Black, true);

            return true;
        }

        private void pictureBoxHareketYazdir_Click(object sender, EventArgs e)
        {
            if (SetupThePrintingHareketler())
            {
                PrintPreviewDialog MyPrintPreviewDialog = new PrintPreviewDialog();
                MyPrintPreviewDialog.Document = printDocument2;
                ((Form)MyPrintPreviewDialog).WindowState = FormWindowState.Maximized;
                MyPrintPreviewDialog.PrintPreviewControl.Zoom = 1;
                MyPrintPreviewDialog.ShowDialog();
            }
        }

        private void printDocument2_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            bool more = MyDataGridViewPrinter.DrawDataGridView(e.Graphics);
            if (more == true)
                e.HasMorePages = true;
        }
        #endregion


        #region ÖĞRENCİLER


        private void textBox43_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                dataGridView1.DataSource = YemekhaneFonksiyonlar.Listele("where ogrencino LIKE '" + textBox43.Text + "%' order by ogrencino ASC");
                byte[] gizlenecek = new byte[] { 1, 4, 8, 9, 10, 11, 12, 13, 14, 16, 17, 18, 19, 20 };
                foreach (var index in gizlenecek) { dataGridView1.Columns[index].Visible = false; }
            }
        }

        private void button3_Click(object sender, EventArgs e) //Sınıf Öğrencilerini Görüntüle butonu
        {
            if (textBox43.Text == "")
            {

                dataGridView1.DataSource = YemekhaneFonksiyonlar.Listele(
                    "where ogrencisinifi like'" + comboBox4.Text + "%' AND ogrencialani like'" + comboBox5.Text +
                    "%' AND ogrencisubesi like'" + comboBox3.Text + "%'order by ogrencino ASC");

                for (int k = 0; k < dataGridView1.Rows.Count; k++)
                {
                    if (dataGridView1.Rows[k].Cells[15].Value.ToString() == "0")
                    {
                        dataGridView1.Rows[k].Cells[15].Value = "Yemek Yemedi";
                    }
                    else if (dataGridView1.Rows[k].Cells[15].Value.ToString() == "1")
                    {
                        dataGridView1.Rows[k].Cells[15].Value = "Yemek Yedi";
                    }
                }

                byte[] gizlenecek = new byte[] {1, 4, 8, 9, 10, 11, 12, 13, 14, 16, 18, 19, 20};
                foreach (var index in gizlenecek)
                {
                    dataGridView1.Columns[index].Visible = false;
                }
            }
            else
            {
                dataGridView1.DataSource = YemekhaneFonksiyonlar.Listele("where ogrencino='" + textBox43.Text + "'");
                for (int l = 0; l < dataGridView1.Rows.Count; l++)
                {

                    if (dataGridView1.Rows[l].Cells[15].Value.ToString() == "0")
                    {
                        dataGridView1.Rows[l].Cells[15].Value = "Yemek Yemedi";
                    }
                    else if (dataGridView1.Rows[l].Cells[15].Value.ToString() == "1")
                    {
                        dataGridView1.Rows[l].Cells[15].Value = "Yemek Yedi";
                    }
                }
                byte[] gizlenecek = new byte[] { 1, 4, 8, 9, 10, 11, 12, 13, 14, 16, 18, 19, 20 };
                foreach (var index in gizlenecek) { dataGridView1.Columns[index].Visible = false; }
            }

            if (sqlbag.State == ConnectionState.Open) { sqlbag.Close(); }
            k = new SqlCommand("SELECT ogrencino, ogrenciadi, ogrencisoyadi,ogrencisinifi,ogrencialani,ogrencisubesi, ogrencidurum,parmakizi,ogrencibakiye FROM ogrenci where ogrencisinifi like '" + comboBox4.Text + "%' AND ogrencialani like '" + comboBox5.Text + "%' AND ogrencisubesi like '" + comboBox3.Text + "' order by ogrencino ASC", sqlbag);
            sqlbag.Open();
            rd = k.ExecuteReader(); int i = 0, j = 0, m = 0;
            while (rd.Read())
            {
                string okunanogrenci = rd["ogrencino"].ToString();
                i++;
                if (rd["ogrencidurum"].ToString() == "1" || rd["ogrencidurum"].ToString() == "G")
                {
                    j++;
                }
                if (rd["ogrencidurum"].ToString() == "0")
                {
                    m++;
                }
            }
            label38.Text = i.ToString();//Sınıf Mevcudu
            label39.Text = j.ToString();//Gelen Mevcudu
            label41.Text = m.ToString();//Gelmeyen Mevcudu
            sqlbag.Close();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e) //Griddeki elemana Tıklama
        {
            DataGridView dgv = sender as DataGridView;
            if (dgv != null && dgv.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgv.SelectedRows[0];
                if (row != null)
                {
                    textBox16.Text = row.Cells[0].Value.ToString();
                    try
                    {                       
                        pictureBox7.Image = Helper.ResimYukle(textBox16.Text);
                    }
                    catch (Exception)
                    {
                        pictureBox7.Image = null;
                    }
                }
            }

            Ogrenci ogrenci = YemekhaneFonksiyonlar.Getir(textBox16.Text);

            label111.Text = ogrenci.no;
            if (ogrenci.no.Substring(0, 1) == "P" && Helper.XmlOku()[1]=="Girissayi")
            {
                buttonBakiyeGuncelle.Visible = true;
                textBox3.Visible = true;
                button8.Visible = false;
                textBox3.Text = ogrenci.ogrencibakiye.ToString();
            }
            else if(Helper.XmlOku()[1] == "Girissayi")
            {
                buttonBakiyeGuncelle.Visible = false;
                button8.Visible = true;
                textBox3.Visible = false;
            }
            else
            {
                buttonBakiyeGuncelle.Visible = true;
                textBox3.Visible = true;
                button8.Visible = false;
                textBox3.Text = ogrenci.ogrencibakiye.ToString();
            }
            textBox17.Text = ogrenci.TC;
            textBox18.Text = ogrenci.adi;
            textBox19.Text = ogrenci.soyadi;
            textBox26.Text = ogrenci.anneadi;
            textBox20.Text = ogrenci.babaadi;
            maskedTextBoxogrencitel.Text = ogrenci.telefonu;
            maskedTextBoxirtibattel.Text = ogrenci.velitel;
            textBox29.Text = ogrenci.sinifi;
            comboBox6.Text = ogrenci.alani;
            textBox27.Text = ogrenci.subesi;
            textBox23.Text = ogrenci.kartno;
            richTextBox2.Text = ogrenci.adresi;
            textBoxVeliMail.Text = ogrenci.velimail;
                
        }

        private void button6_Click(object sender, EventArgs e) //Öğrencileri Güncelle butonu
        {
            Ogrenci ogrenci = YemekhaneFonksiyonlar.Getir(label111.Text);
            ogrenci.no = textBox16.Text;
            ogrenci.TC = textBox17.Text;
            ogrenci.adi = textBox18.Text;
            ogrenci.soyadi = textBox19.Text;
            ogrenci.sinifi = textBox29.Text;
            ogrenci.subesi = comboBox6.Text;
            ogrenci.alani = textBox27.Text;
            ogrenci.anneadi = textBox26.Text;
            ogrenci.babaadi = textBox20.Text;
            ogrenci.telefonu = maskedTextBoxogrencitel.Text;
            ogrenci.velitel = maskedTextBoxirtibattel.Text;
            ogrenci.adresi = richTextBox2.Text;
            ogrenci.kartno = textBox23.Text;
            ogrenci.velimail = textBoxVeliMail.Text;
            try
            {
                YemekhaneFonksiyonlar.Guncelle(ogrenci,label111.Text);
                MessageBox.Show("Güncelleme Başarılı!", "Öğrenci Güncelleme...");
                ogrencileriyenile();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Güncelleme İşlemi Başarısız! Lütfen Tekrar Deneyiniz!", "İşlem Başarısız!");
            }
            
        }

        private void button13_Click(object sender, EventArgs e)
        {
            DialogResult res =
                MessageBox.Show(label111.Text + " numaralı öğrenci kaydı silinecektir.Onaylıyor Musunuz?",
                    "Öğrenci Silme İşlemi!", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                YemekhaneFonksiyonlar.OgrenciSil(label111.Text);
                ogrencileriyenile();
            }           
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                Form3 yeniogrenci = new Form3();
                yeniogrenci.Show();
            }
            catch (Exception)
            {

            }
        } //Yeni Öğrenci kaydı

        private void textBox24_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ',';
        }

        private void buttonBakiyeGuncelle_Click(object sender, EventArgs e)
        {
            string ogrenciNo = textBox16.Text;
            DialogResult res = MessageBox.Show(ogrenciNo + " nolu '" + textBox18.Text + " " + textBox19.Text + "' isimli öğrencinin bakiyesine " + textBox24.Text + " TL yükleme işlemi gerçekleşecektir.Onaylıyor Musunuz?", "Bakiye Yükleme", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                YemekhaneFonksiyonlar.BakiyeGuncelle(ogrenciNo,Convert.ToDouble(textBox24.Text));
                MessageBox.Show("Yükleme işlemi gerçekleştirilmiştir.");
                textBox24.Clear();
                ogrencileriyenile();
            }
            else
            {
                MessageBox.Show("Yükleme İşlemi İptal Edilmiştir.");
                return;
            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
            string ogrenciNo = textBox16.Text;
            int kalangiris = 0;
            int yuklenecekbakiye = Convert.ToInt32(textBox2.Text);
            DialogResult res = MessageBox.Show(ogrenciNo + " nolu '" + textBox18.Text + " " + textBox19.Text + "' isimli öğrencinin bakiyesine " + yuklenecekbakiye.ToString() + " giriş yükleme işlemi gerçekleşecektir.Onaylıyor Musunuz?", "Giriş Sayısı Yükleme", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                YemekhaneFonksiyonlar.GirisHakkiGuncelle(ogrenciNo,yuklenecekbakiye);
                MessageBox.Show("Yükleme işlemi gerçekleştirilmiştir.");
                textBox2.Clear();
                ogrencileriyenile();
            }
            else
            {
                MessageBox.Show("Yükleme İşlemi İptal Edilmiştir.");
                return;
            }
        }

        #endregion

        #region PROGRAM

        private void programGetir()
        {          
            List<string>yemekProgramiList = YemekhaneFonksiyonlar.YemekProgramiGetir();
            try
            {
                baslangicsaatleri[0].Text = yemekProgramiList[0];
                bitissaatleri[0].Text = yemekProgramiList[1];
                baslangicsaatleri[1].Text = yemekProgramiList[2];
                bitissaatleri[1].Text = yemekProgramiList[3];
                baslangicsaatleri[2].Text = yemekProgramiList[4];
                bitissaatleri[2].Text = yemekProgramiList[5];
                baslangicsaatleri[3].Text = yemekProgramiList[6];
                bitissaatleri[3].Text = yemekProgramiList[7];
            }
            catch (Exception)
            {

            }
        }


        //Programı Görüntüle

        private void button5_Click(object sender, EventArgs e) //YEMEK Programını Kaydet Butonu
        {
            //Yemek Programı Oluştur
            YemekhaneFonksiyonlar.YemekProgramiGuncelle(" " +textBoxSabahBaslama.Text + "', '" + textBoxSabahBitis.Text + "' , '" + textBoxOgleBaslama.Text + "', '" + textBoxOgleBitis.Text + "', '" + textBoxAksamBaslama.Text + "', '" + textBoxAksamBitis.Text + "', '" + textBoxaraogunbaslama.Text + "', '" + textBoxaraogunbitis.Text + "')");
            programGetir();
        }

        #endregion

        #region AYARLAR

        private void pictureBox6_Click(object sender, EventArgs e) //AYARLARI KAYDET
        {
            Ayarlar ayarlar = new Ayarlar();           
            ayarlar.smsusername = textBoxnetgsmusername.Text;
            ayarlar.smspass = textBoxnetgsmpass.Text;
            ayarlar.smsheader = textBoxnetgsmheader.Text;
            ayarlar.girissms = richTextBoxgirissms.Text;
            ayarlar.smsfirma = comboBoxsmsfirma.SelectedItem.ToString();
            ayarlar.girdisms = Convert.ToInt32(checkBox1.Checked);
            ayarlar.bakiyesms = Convert.ToInt32(checkBox2.Checked);
            ayarlar.bakiyegoster = Convert.ToInt32(checkBox3.Checked);
            ayarlar.tekrarkontrol = Convert.ToInt32(checkBox9.Checked);
            ayarlar.girissaatkontrol = Convert.ToInt32(checkBox10.Checked);
            ayarlar.yemekucreti = Convert.ToDecimal(String.Format("{0:0,00}", textBox21.Text.Replace(".",",")));
            ayarlar.mailkullaniciadi = textBoxMailKullaniciAdi.Text;
            ayarlar.mailsifre = textBoxMailSifre.Text;
            ayarlar.AyarGuncelle();
            // random olarak r adında bir dğeişken tanımladık.
            Random r = new Random();
            //255'e kadar rastgele sayı üretecektir.
            int a, b, c;
            a = r.Next(255);
            b = r.Next(255);
            c = r.Next(255);
            label80.BackColor = Color.FromArgb(a, b, c);
        }

        private void buttonTabloDoldur_Click(object sender, EventArgs e)
        {
            YemekhaneFonksiyonlar.NullDoldur();
            MessageBox.Show("Tablo Doldurma İşlemi Tamamlanmıştır.", "Tablo Doldurma");
        }

        private void buttonTumFotoAktar_Click(object sender, EventArgs e)
        {
            var resimKopyalaThread = new Thread(() => {

                string path = @"C:\resimler";
                DirectoryInfo di = new DirectoryInfo(path);
                FileInfo[] rgFiles = di.GetFiles();
                FileInfo fileInfo;
                foreach (string dosya in Directory.GetFiles(@"\\" + panoip + "\\" + "resimler"))
                {
                    try
                    {
                        fileInfo = new FileInfo(dosya);
                        fileInfo.Delete();
                    }
                    catch (Exception) { }
                }
                foreach (FileInfo fi in rgFiles)
                {
                    Invoke(new Action(() =>
                    {
                        File.Copy(@"C:\resimler\" + fi.Name, @"\\" + panoip + "\\" + "resimler" + "\\" + fi.Name);
                    }));

                }
                MessageBox.Show("Fotoğraf aktarma işlemi tamamlandı.", "Fotoğraf Eşzamanlama");
            });


            DialogResult result = MessageBox.Show("Resimler klasöründeki resimler pano resimleri ile eşlenecektir.Devam etmek istiyor musunuz?", "Resim Aktarma İşlemi", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                resimKopyalaThread.Start();
            }
            else
            {
                return;
            }
        }

        private void buttonFarkliFotoAktar_Click(object sender, EventArgs e)
        {
            var resimKopyalaThread = new Thread(() => {

                string path = @"C:\resimler";
                DirectoryInfo di = new DirectoryInfo(path);
                FileInfo[] rgFiles = di.GetFiles();
                foreach (FileInfo fi in rgFiles)
                {
                    Invoke(new Action(() =>
                    {
                        if (!File.Exists(@"\\" + panoip + "\\" + "resimler" + "\\" + fi.Name))
                        {
                            File.Copy(@"C:\resimler\" + fi.Name, @"\\" + panoip + "\\" + "resimler" + "\\" + fi.Name);
                        }                       
                    }));

                }
                MessageBox.Show("Fotoğraf aktarma işlemi tamamlandı.", "Fotoğraf Eşzamanlama");
            });


            DialogResult result = MessageBox.Show("Resimler klasöründeki resimler pano resimleri ile eşlenecektir.Devam etmek istiyor musunuz?", "Resim Aktarma İşlemi", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                resimKopyalaThread.Start();
            }
            else
            {
                return;
            }
        }
        #endregion

        #region SMS
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBoxsmsfirma.SelectedItem.ToString() == "MayaTek")
                {
                    //netgsmgonder(textBox1.Text,richTextBox4.Text); // Numaralar string bir şekilde aralarında virgül olarak gönderilir.
                    var SR = new com.megaokul.gateway.HERYONESMSWebService();

                    //labelsmsdurum.Text = SR.MAYATEK_SMS_SENDER("903327131895", "sms43hmgy", "iPEK YOLU", 1, "5325005024", "işte bu", 0);
                    labelsmsdurum.Text = SR.MAYATEK_SMS_SENDER(textBoxnetgsmusername.Text, textBoxnetgsmpass.Text, textBoxnetgsmheader.Text, 1, textBox1.Text, richTextBox4.Text, 0);
                }
                else if (comboBoxsmsfirma.SelectedItem.ToString() == "NetGSM")
                {
                    netgsmgonder("90" + textBox1.Text, richTextBox4.Text);
                }
                else if (comboBoxsmsfirma.SelectedItem.ToString() == "TTMesaj")
                {
                    ttmesajgonder("90" + textBox1.Text, richTextBox4.Text);
                }
                else if (comboBoxsmsfirma.SelectedItem.ToString() == "Labirent")
                {
                    labirentsmsgonder("0" + textBoxAksamBitis.Text, richTextBox4.Text);
                }
            }
            catch (Exception) { }
        }

        public string DakikSMSMesajGonder(string numaralar, string mesaj)
        {
            // DEĞİŞKENLER OLUŞTURULUYOR
            string kullaniciAdi = "8506766516", sifre = "2kzvy8", baslik = "DEMO";// mesaj = "gönderilecek mesajınız";
            // XML DESENİ YARATILIYOR.
            string xmlDesen = "<SMS><oturum><kullanici>" + kullaniciAdi + "</kullanici><sifre>" + sifre + "</sifre></oturum><mesaj><baslik>" + baslik + "</baslik><metin>" + mesaj + "</metin><alicilar>" + numaralar.ToString() + "</alicilar><tarih></tarih></mesaj></SMS>";
            string ApiAdres = "http://www.dakiksms.com/api/xml_api_ileri.php";
            // APIYE XML DESENİ VE API ADRESİ GÖNDERİLİYOR.
            WebRequest request = WebRequest.Create(ApiAdres);
            request.Method = "POST";
            byte[] byteArray = Encoding.UTF8.GetBytes(xmlDesen);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
            // DÖNEN CEVAP İLGİLİ YERE GÖNDERİLİYOR.
        }

        private string XMLPOST(string PostAddress, string xmlData)
        {
            try
            {
                WebClient wUpload = new WebClient();
                HttpWebRequest request = WebRequest.Create(PostAddress) as HttpWebRequest;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                Byte[] bPostArray = Encoding.UTF8.GetBytes(xmlData);
                Byte[] bResponse = wUpload.UploadData(PostAddress, "POST", bPostArray);
                Char[] sReturnChars = Encoding.UTF8.GetChars(bResponse);
                string sWebPage = new string(sReturnChars);
                return sWebPage;
            }
            catch
            {
                return "-1";
            }
        }



        private void netgsmgonder(string tel, string mesaj)
        {
            string ss = "";
            ss += "<?xml version='1.0' encoding='UTF-8'?>";
            ss += "<mainbody>";
            ss += "<header>";
            ss += "<company dil='TR'>NETGSM</company>";
            ss += "<usercode>" + textBoxnetgsmusername.Text + "</usercode>";//8503028531
            ss += "<password>" + textBoxnetgsmpass.Text + "</password>";//9A8C3Z6
            ss += "<startdate></startdate>";
            ss += "<stopdate></stopdate>";
            ss += "<type>1:n</type>";
            ss += "<msgheader>" + textBoxnetgsmheader.Text + "</msgheader>";
            ss += "</header>";
            ss += "<body>";
            ss += "<msg><![CDATA[" + mesaj + "]]></msg>";
            ss += "<no>" + tel + "</no>";
            ss += "</body>";
            ss += "</mainbody>";
            labelsmsdurum.Text = XMLPOST("http://api.netgsm.com.tr/xmlbulkhttppost.asp", ss);
        }



        public void netgsmkalanbakiye()
        {
            string ss = "";
            ss += "<?xml version='1.0'?>";
            ss += "<mainbody>";
            ss += "<header>";
            ss += "<company>NETGSM</company>";
            ss += "<usercode>Kullaniciadi</usercode>";
            ss += "<password>Sifre</password>";
            ss += "<stip>2</stip>";
            ss += "</header>";
            ss += "</mainbody>";

            labelsmsdurum.Text = XMLPOST("https://api.netgsm.com.tr/xmlpaketkampanya.asp", ss);
        }

        public void ttmesajgonder(string tel, string mesaj)
        {
            try
            {
                smsClient.sendSingleSMS(textBoxnetgsmusername.Text, textBoxnetgsmpass.Text, tel, mesaj, textBoxnetgsmheader.Text, "0", "0");
            }
            catch (Exception ex) { }
        }

        public void labirentsmsgonder(string tel, string mesaj)
        {
            labelsmsdurum.Text = labirentsmspost("<MainmsgBody><UserName>" + textBoxnetgsmusername.Text + "</UserName>" +
                "<PassWord>" + textBoxnetgsmpass.Text + "</PassWord>" +
                "<Action>12</Action>" +
                "<Mesgbody>" + mesaj + "</Mesgbody>" +
                "<Numbers>" + tel + "</Numbers>" +
                "<Originator>" + textBoxnetgsmheader.Text + "</Originator>" +
                "<SDate></SDate></MainmsgBody>");
        }

        public string labirentsmspost(string prmSendData)
        {
            try
            {
                WebClient wUpload = new WebClient();
                wUpload.Proxy = null;
                Byte[] bPostArray = Encoding.UTF8.GetBytes(prmSendData);
                Byte[] bResponse = wUpload.UploadData("http://g.iletimx.com", "POST", bPostArray);
                Char[] sReturnChars = Encoding.UTF8.GetChars(bResponse);
                string sWebPage = new string(sReturnChars);
                return sWebPage;
            }
            catch { return "-1"; }
        }

        #endregion

        #region İKONLAR
        private void pictureBoxbarkod_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private void pictureBoxkayitlar_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
            hareketGetir();
            label44.Text = dataGridView2.Rows.Count.ToString();
        }

        private void pictureBoxogrenciler_Click(object sender, EventArgs e)
        {
            if (sqlbag.State == ConnectionState.Open) { sqlbag.Close(); }
            tabControl1.SelectedIndex = 2;
            ogrencileriyenile();
        }

        public void ogrencileriyenile()
        {
            dataGridView1.DataSource = YemekhaneFonksiyonlar.Listele(" order by ogrencino ASC");

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[15].Value.ToString() == "0")
                {
                    dataGridView1.Rows[i].Cells[15].Value = "Yemek Yemedi";
                }
                else if (dataGridView1.Rows[i].Cells[15].Value.ToString() == "1")
                {
                    dataGridView1.Rows[i].Cells[15].Value = "Yemek Yedi";
                }
            }
            byte[] gizlenecek = new byte[] { 1, 4, 8, 9, 10, 11, 12, 13, 14, 16, 18, 19, 20 };
            foreach (var index in gizlenecek) { dataGridView1.Columns[index].Visible = false; }
        }

        private void pictureBoxprogram_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 3;
            if (sqlbag.State == ConnectionState.Open) { sqlbag.Close(); }
            programGetir();
        }

        private void pictureBoxayarlar_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 4;
            if (sqlbag.State == ConnectionState.Open) { sqlbag.Close(); }
            label80.BackColor = Color.White;
            label80.Visible = true;
        }

        private void pictureBoxcikis_Click(object sender, EventArgs e)
        {
            Application.ExitThread();
            try
            {
                Application.Exit();
                Environment.Exit(1);
            }
            catch (Exception)
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                excelsenkronizasyon senk = new excelsenkronizasyon();
                senk.Show();
            }
            catch (Exception) { }
        }

        private void pictureBoxdevamsizlik_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 5;
            devamsizlikGetir();
        }

        private void RTEventsMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.ExitThread();
            try
            {
                Application.Exit();
                Environment.Exit(1);
            }
            catch (Exception)
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        #endregion

        #region DEVAMSIZLIK
        private void devamsizlikGetir()
        {
            dataGridView4.Columns.Clear();
            string sorgubitistarihi;
            string sorgubaslangictarihi;
            sorgubaslangictarihi = dateTimePicker6.Text;
            sorgubitistarihi = dateTimePicker5.Text;           
            int girensayisi = 0;
            int girmeyensayisi = 0;
            int l = 0;

            List<string> yemekProgram = YemekhaneFonksiyonlar.YemekProgramiGetir();


            if (dataGridView4.Columns.Contains("SIRA") == false) { dataGridView4.Columns.Add("SIRA", "SIRA"); }
            byte[] gizlenecek = new byte[] { 2, 5, 9,10,11,12, 13, 14, 15, 16, 17, 18, 19, 20, 21 };
            dataGridView4.Columns["SIRA"].DisplayIndex = 0;

            var list = YemekhaneFonksiyonlar.Listele("  WHERE ogrencisinifi like'" + comboBox11.Text + "%' AND ogrencialani like'" + comboBox9.Text + "%' AND ogrencisubesi like '" + comboBox10.Text + "%' Order By ogrencino");

            var bindingList = new BindingList<Ogrenci>(list);
            var source = new BindingSource(bindingList, null);
            dataGridView4.DataSource = source;

            foreach (var index in gizlenecek) { dataGridView4.Columns[index].Visible = false; }
            for (int i = 0; i < dataGridView4.Rows.Count; i++)
            {
                dataGridView4.Rows[i].Cells[0].Value = (i + 1).ToString();
            }

            
            if (dataGridView4.Columns.Contains("SABAH") == false) { dataGridView4.Columns.Add("SABAH", "SABAH"); }
            if (dataGridView4.Columns.Contains("OGLE") == false) { dataGridView4.Columns.Add("OGLE", "OGLE"); }
            if (dataGridView4.Columns.Contains("AKSAM") == false) { dataGridView4.Columns.Add("AKSAM", "AKSAM"); }
            if (dataGridView4.Columns.Contains("ARAOGUN") == false) { dataGridView4.Columns.Add("ARAOGUN", "ARAOGUN"); }
            

            for (int i = 0; i < dataGridView4.Rows.Count; i++)
            {
                l = 0;
                List<Ogrenci> hareket = YemekhaneFonksiyonlar.HareketGetir(
                    " where tarih BETWEEN'" + sorgubitistarihi + " " + " 00:00" + "' AND '" + sorgubitistarihi + " " +
                    " 23:59" + "' AND ogrencino = '" + dataGridView4.Rows[i].Cells[1].Value.ToString() +
                    "' ORDER BY tarih DESC");
                if (hareket.Count != 0)
                {
                    DateTime girisSaat = new DateTime();
                    for (int j = 0; j < hareket.Count; j++)
                    {
                        girisSaat = hareket[j].yemekgirisTarih;

                        if (girisSaat < Convert.ToDateTime(yemekProgram[1]) && girisSaat > Convert.ToDateTime(yemekProgram[0]))
                        {
                            try
                            {
                                dataGridView4.Rows[i].Cells[22].Value = Convert.ToDateTime(girisSaat).ToShortTimeString();
                            }
                            catch (Exception) { }
                        }
                        else if (girisSaat < Convert.ToDateTime(yemekProgram[3]) && girisSaat > Convert.ToDateTime(yemekProgram[2]))
                        {
                            try
                            {
                                dataGridView4.Rows[i].Cells[23].Value = Convert.ToDateTime(girisSaat).ToShortTimeString();
                                girensayisi++;
                            }
                            catch (Exception) { }
                        }
                        else if (girisSaat < Convert.ToDateTime(yemekProgram[5]) && girisSaat > Convert.ToDateTime(yemekProgram[4]))
                        {
                            try
                            {
                                dataGridView4.Rows[i].Cells[24].Value = Convert.ToDateTime(girisSaat).ToShortTimeString();
                            }
                            catch (Exception) { }
                        }
                        else if (girisSaat < Convert.ToDateTime(yemekProgram[7]) && girisSaat > Convert.ToDateTime(yemekProgram[6]))
                        {
                            try
                            {
                                dataGridView4.Rows[i].Cells[25].Value = Convert.ToDateTime(girisSaat).ToShortTimeString();
                            }
                            catch (Exception) { }
                        }
                    }
                    l++;
                }
                
                if (dataGridView4.Rows[i].Cells[22].Value== null) { dataGridView4.Rows[i].Cells[22].Value = "Yemedi"; }
                if (dataGridView4.Rows[i].Cells[23].Value== null) { dataGridView4.Rows[i].Cells[23].Value = "Yemedi"; }
                if (dataGridView4.Rows[i].Cells[24].Value== null) { dataGridView4.Rows[i].Cells[24].Value = "Yemedi"; }
                if (dataGridView4.Rows[i].Cells[25].Value == null) { dataGridView4.Rows[i].Cells[25].Value = "Yemedi"; }
            }

            dataGridView4.Columns["SABAH"].DisplayIndex = 9;
            dataGridView4.Columns["OGLE"].DisplayIndex = 10;
            dataGridView4.Columns["AKSAM"].DisplayIndex = 11;
            dataGridView4.Columns["ARAOGUN"].DisplayIndex = 12;

            if (radioButtonSabahYiyen.Checked == true)
            {
                girensayisi = 0;
                dataGridView4.Columns[23].Visible = false; dataGridView4.Columns[24].Visible = false; dataGridView4.Columns[25].Visible = false; radioButtonSabahYiyen.Checked = false;
                
                for (int i = 0; i < dataGridView4.Rows.Count; i++) { if (dataGridView4.Rows[i].Cells[22].Value.ToString() == "Yemedi") { dataGridView4.Rows[i].Selected = true; } else { dataGridView4.Rows[i].Selected = false; } }
                foreach (DataGridViewRow row in dataGridView4.SelectedRows)
                {
                    dataGridView4.Rows.Remove(row);
                }
                for (int i = 0; i < dataGridView4.Rows.Count; i++)
                {
                    try
                    {
                        dataGridView4.Rows[i].Cells[0].Value = (i + 1).ToString();
                        girensayisi++;
                    }
                    catch (Exception) { }
                }
            }
            else if (radioButtonOgleYiyen.Checked == true)
            {
                girensayisi = 0;
                dataGridView4.Columns[22].Visible = false; dataGridView4.Columns[24].Visible = false; dataGridView4.Columns[25].Visible = false; radioButtonOgleYiyen.Checked = false;
                for (int i = 0; i < dataGridView4.Rows.Count; i++) { if (dataGridView4.Rows[i].Cells[23].Value.ToString() == "Yemedi") { dataGridView4.Rows[i].Selected = true; } else { dataGridView4.Rows[i].Selected = false; } }

                foreach (DataGridViewRow row in dataGridView4.SelectedRows)
                {
                    dataGridView4.Rows.Remove(row);
                }
                for (int i = 0; i < dataGridView4.Rows.Count; i++)
                {
                    try
                    {
                        dataGridView4.Rows[i].Cells[0].Value = (i + 1).ToString();
                        girensayisi++;
                    }
                    catch (Exception) { }
                }
            }
            else if (radioButtonAksamYiyen.Checked == true)
            {
                girensayisi = 0;
                dataGridView4.Columns[22].Visible = false; dataGridView4.Columns[23].Visible = false; dataGridView4.Columns[25].Visible = false; radioButtonAksamYiyen.Checked = false;
                for (int i = 0; i < dataGridView4.Rows.Count; i++) { if (dataGridView4.Rows[i].Cells[24].Value.ToString() == "Yemedi") { dataGridView4.Rows[i].Selected = true; } else { dataGridView4.Rows[i].Selected = false; } }

                foreach (DataGridViewRow row in dataGridView4.SelectedRows)
                {
                    dataGridView4.Rows.Remove(row);
                }
                for (int i = 0; i < dataGridView4.Rows.Count; i++)
                {
                    try
                    {
                        dataGridView4.Rows[i].Cells[0].Value = (i + 1).ToString();
                        girensayisi++;
                    }
                    catch (Exception) { }
                }
            }
            else if (radioButtonAraogunYiyen.Checked == true)
            {
                girensayisi = 0;
                dataGridView4.Columns[22].Visible = false; dataGridView4.Columns[23].Visible = false; dataGridView4.Columns[24].Visible = false; radioButtonAraogunYiyen.Checked = false;
                for (int i = 0; i < dataGridView4.Rows.Count; i++) { if (dataGridView4.Rows[i].Cells[25].Value.ToString() == "Yemedi") { dataGridView4.Rows[i].Selected = true; } else { dataGridView4.Rows[i].Selected = false; }}

                foreach (DataGridViewRow row in dataGridView4.SelectedRows)
                {
                    dataGridView4.Rows.Remove(row);
                }
                for (int i = 0; i < dataGridView4.Rows.Count; i++)
                {
                    try
                    {
                        dataGridView4.Rows[i].Cells[0].Value = (i + 1).ToString();
                        girensayisi++;
                    }
                    catch (Exception) { }
                }
            }
            label21.Text = dataGridView4.Rows.Count.ToString(); //Sınıf Mevcudu
            label22.Text = girensayisi.ToString();              //Yemek Yiyen Sayısı
            label19.Text = (Convert.ToInt32(label21.Text) - Convert.ToInt32(label22.Text)).ToString();  //Yemek Yemeyen Sayısı
        }

        string secilendevamsizlikno = "", secilendevamsizlikadi = "";

        private void dataGridView4_SelectionChanged(object sender, EventArgs e) //Griddeki elemana Tıklama
        {
            DataGridView dgv = sender as DataGridView;
            if (dgv != null && dgv.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgv.SelectedRows[0];
                if (row != null)
                {
                    try
                    {
                        secilendevamsizlikno = row.Cells[1].Value.ToString();
                        secilendevamsizlikadi = row.Cells[2].Value.ToString();
                    }
                    catch (Exception) { }
                }
            }
                     
            try
            {
                pictureBox3.Image = Helper.ResimYukle(secilendevamsizlikno);
            }
            catch (Exception)
            {
                pictureBox3.Image = null;
            }
            Ogrenci ogrenci = YemekhaneFonksiyonlar.Getir(secilendevamsizlikno);
            textBox35.Text = ogrenci.TC;
            textBox37.Text = ogrenci.adi;
            textBox36.Text = ogrenci.soyadi;
            textBox38.Text = ogrenci.sinifi;
            textBox39.Text = ogrenci.subesi;
            textBox44.Text = ogrenci.alani;
        }

        private void pictureBoxDevamsizlikYazdir_Click(object sender, EventArgs e)
        {
            if (SetupThePrinting())
            {
                PrintPreviewDialog MyPrintPreviewDialog = new PrintPreviewDialog();
                MyPrintPreviewDialog.Document = printDocument1;
                ((Form)MyPrintPreviewDialog).WindowState = FormWindowState.Maximized;
                MyPrintPreviewDialog.PrintPreviewControl.Zoom = 1;
                MyPrintPreviewDialog.ShowDialog();
            }
        }

        private void textBoxDevamsizlikOgrNoAra_TextChanged(object sender, EventArgs e)
        {
            if (textBoxDevamsizlikOgrNoAra.Text == "")
            {
                devamsizlikGetir();
            }
            else
            {
                if (dataGridView4.Columns.Contains("SIRA") == false) { dataGridView4.Columns.Add("SIRA", "SIRA"); }
                byte[] gizlenecek = new byte[] { 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 };
                dataGridView4.DataSource = YemekhaneFonksiyonlar.Listele(" WHERE ogrencino LIKE'" + textBoxDevamsizlikOgrNoAra.Text + "%'");
                dataGridView4.Columns["SIRA"].DisplayIndex = 0;
                foreach (var index in gizlenecek) { dataGridView4.Columns[index].Visible = false; }
                for (int i = 0; i < dataGridView4.Rows.Count; i++)
                {
                    dataGridView4.Rows[i].Cells[0].Value = (i + 1).ToString();
                }
            }          
        }

        private void buttonSonSinifSil_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Sınıfı 12 ile başlayan tüm öğrenciler silinecektir.Onaylıyor Musunuz?", "Son Sınıf Silme!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res == DialogResult.Yes)
            {
                List<Ogrenci> list = YemekhaneFonksiyonlar.Listele(" WHERE ogrencisinifi LIKE '12%'");
                foreach (Ogrenci ogrenci in list)
                {
                    YemekhaneFonksiyonlar.OgrenciSil(ogrenci.no);
                }
            }        
        }

        

        private bool SetupThePrinting()
        {
            string baslik = "*" + dateTimePicker6.Text + " İLE " + dateTimePicker5.Text + " ARASI '"+"YEMEK"+"' LİSTESİ";
            PrintDialog MyPrintDialog = new PrintDialog();
            MyPrintDialog.AllowCurrentPage = false;
            MyPrintDialog.AllowPrintToFile = false;
            MyPrintDialog.AllowSelection = true;
            MyPrintDialog.AllowSomePages = false;
            MyPrintDialog.PrintToFile = false;
            MyPrintDialog.ShowHelp = false;
            MyPrintDialog.ShowNetwork = false;

            //if (MyPrintDialog.ShowDialog() != DialogResult.OK)
            //    return false;

            printDocument1.DocumentName = "HAREKET KAYITLARI";
            printDocument1.PrinterSettings = MyPrintDialog.PrinterSettings;
            printDocument1.DefaultPageSettings = MyPrintDialog.PrinterSettings.DefaultPageSettings;
            printDocument1.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(20, 20, 40, 20);

            if (MessageBox.Show("Raporu sayfaya ortalamak ister misiniz?",
             "Rapor Ortalaması", MessageBoxButtons.YesNo,
             MessageBoxIcon.Question) == DialogResult.Yes)
                MyDataGridViewPrinter = new DataGridViewPrinter(dataGridView4,
                printDocument1, true, true, "*" + dateTimePicker6.Text + " İLE " + dateTimePicker5.Text + " ARASI '"+"YEMEK" +"' LİSTESİ", new Font("Tahoma", 12,
                FontStyle.Bold, GraphicsUnit.Point), System.Drawing.Color.Black, true);
            else
                MyDataGridViewPrinter = new DataGridViewPrinter(dataGridView4,
                printDocument1, false, true, baslik, new Font("Tahoma", 12,
                FontStyle.Bold, GraphicsUnit.Point), System.Drawing.Color.Black, true);

            return true;
        }

        

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            bool more = MyDataGridViewPrinter.DrawDataGridView(e.Graphics);
            if (more == true)
                e.HasMorePages = true;
        }



        private void button7_Click(object sender, EventArgs e)
        {
            devamsizlikGetir();
        }

        private void dataGridView4_DoubleClick(object sender, EventArgs e)
        {
            devamsizlik devamsizlikform = new devamsizlik();
            devamsizlikform.Show();
            devamsizlikform.doldur(secilendevamsizlikno, secilendevamsizlikadi, dateTimePicker6.Text, dateTimePicker5.Text, textBox24.Text);
        }
        #endregion

        #region TopluMail
        private void pictureBoxTopluMail_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 7;
            maildosyayolu.Clear();
            label124.Text = "";
            mailListeDoldur(" WHERE velimail IS NOT NULL ORDER BY ogrencino ASC");
        }
        private void textBoxMailOgrenciNoAra_TextChanged(object sender, EventArgs e)
        {
            mailListeDoldur(" WHERE ogrencino LIKE '" + textBoxMailOgrenciNoAra.Text + "%' ORDER BY ogrenciadi ASC");
        }
        private void textBoxMailOgrenciAdiAra_TextChanged(object sender, EventArgs e)
        {
            mailListeDoldur(" WHERE ogrenciadi LIKE '" + textBoxMailOgrenciAdiAra.Text + "%' ORDER BY ogrenciadi ASC");
        }

        private void buttonMailFiltrele_Click(object sender, EventArgs e)
        {
            mailListeDoldur(" WHERE velimail IS NOT NULL AND ogrencisinifi LIKE'" + comboBoxMailSinif.Text + "' AND ogrencialani LIKE '" + comboBoxMailAlan.Text + "' AND ogrencisubesi LIKE '" + comboBoxMailSube.Text + "' ORDER BY ogrencino ASC");
        }

        private void buttonMailAktar_Click(object sender, EventArgs e)
        {
            if (checkBoxMailTumunuSec.Checked)
            {
                foreach (ListViewItem item in listViewMail.Items)
                {
                    item.Selected = true;
                }
            }

            for (int i = 0; i < listViewMail.SelectedItems.Count; i++)
            {
                string[] row = { listViewMail.SelectedItems[i].SubItems[0].Text, listViewMail.SelectedItems[i].SubItems[1].Text };
                ListViewItem item = new ListViewItem(row);
                item.Name = listViewMail.SelectedItems[i].SubItems[0].Text;
                if (!listViewMailGonderilecek.Items.ContainsKey(item.Name))
                {
                    listViewMailGonderilecek.Items.Add(item);
                }
            }
        }

        private void buttonMailSeciliSil_Click(object sender, EventArgs e)
        {
            if (checkBoxMailTumunuSec.Checked)
            {
                foreach (ListViewItem item in listViewMailGonderilecek.Items)
                {
                    item.Selected = true;
                }
            }
            for (int i = listViewMailGonderilecek.SelectedIndices.Count - 1; i >= 0; i--)
            {
                listViewMailGonderilecek.Items.RemoveAt(listViewMailGonderilecek.SelectedIndices[i]);
            }
        }

        

        private void mailListeDoldur(string where)
        {
            listViewMail.FullRowSelect = true;
            listViewMail.Items.Clear();
            List<Ogrenci> ogrenci = YemekhaneFonksiyonlar.OgrenciGetir(where);
            for (int i = 0; i < ogrenci.Count; i++)
            {
                if (ogrenci[i].velimail != "" && ogrenci[i].velimail.Contains("@"))
                {
                    string[] row = { ogrenci[i].velimail, ogrenci[i].adi + " " + ogrenci[i].soyadi };
                    ListViewItem item = new ListViewItem(row);
                    listViewMail.Items.Add(item);
                }
            }
        }

        private void buttonMailDosyaEkle_Click(object sender, EventArgs e)
        {
            maildosyayolu.Clear();
            label124.Text = "";
            OpenFileDialog file = new OpenFileDialog();
            file.Multiselect = true;
            file.ShowDialog();
            for (int i = 0; i < file.FileNames.Length; i++)
            {
                maildosyayolu.Add(file.FileNames[i]);
                label124.Text += file.FileNames[i] + "\n";
            }
        }

        private void buttonMailGonder_Click(object sender, EventArgs e)
        {
            string gonderilecekListesi = "";
            for (int i = 0; i < listViewMailGonderilecek.Items.Count; i++)
            {
                gonderilecekListesi += listViewMailGonderilecek.Items[i].SubItems[0].Text + ",";
            }
            gonderilecekListesi.Substring(gonderilecekListesi.Length - 1, 1);
            mailGonder(textBoxMailKullaniciAdi.Text, textBoxMailSifre.Text, gonderilecekListesi, richTextBoxMailMetin.Text, textBoxMailKonusu.Text);
        }

        private void mailGonder(string mailKullaniciAdi, string mailSifre, string mailKisi, string mailMetin, string mailKonu)
        {
            string[] kime = mailKisi.Split(',');
            string metin = mailMetin;
            string kullaniciAdi = mailKullaniciAdi;
            string kullaniciSifre = mailSifre;

            MailMessage ePosta = new MailMessage();

            SmtpClient smtp = new SmtpClient();

            var sendMailThread = new Thread(() =>
            {
                try
                {
                    ePosta.From = new MailAddress(kullaniciAdi);
                    foreach (var iletilecekadres in kime)
                    {
                        if (iletilecekadres.IndexOf("@") != -1)
                        {
                            ePosta.To.Add(iletilecekadres);
                        }
                    }
                    ePosta.Subject = mailKonu;
                    ePosta.Body = mailMetin;
                    ePosta.IsBodyHtml = true;
                    object userState = ePosta;
                    for (int i = 0; i < maildosyayolu.Count; i++)
                    {
                        ePosta.Attachments.Add(new Attachment(maildosyayolu[i]));
                    }

                    smtp.Credentials = new NetworkCredential(kullaniciAdi, kullaniciSifre);

                    string[] mailUzanti = kullaniciAdi.Split('@');

                    if (mailUzanti[1] == "gmail.com")
                    {
                        smtp.EnableSsl = true;
                        smtp.Port = 587;
                        smtp.Host = "smtp.gmail.com";
                    }
                    else
                    {
                        smtp.EnableSsl = true;
                        smtp.Port = 587;
                        smtp.Host = "smtp.live.com";
                    }
                }
                catch (Exception ex) { Helper.DosyayaYaz(ex.ToString()); }

                try
                {
                    smtp.Send(ePosta);
                }
                catch (SmtpException ex) { Helper.DosyayaYaz(ex.ToString()); }
            });
            sendMailThread.Start();
        }

        #endregion



        #region İstatistik
        private void pictureBoxistatistik_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 6;
        }

        private void buttonMailSifreGoster_Click(object sender, EventArgs e)
        {
            if (textBoxMailSifre.PasswordChar.ToString() == "*")
            {
                buttonMailSifreGoster.Text = "Şifre Gizle";
                textBoxMailSifre.PasswordChar = '\0';
            }
            else
            {
                buttonMailSifreGoster.Text = "Şifre Göster";
                textBoxMailSifre.PasswordChar = '*';
            }
            
        }

        private void pictureBoxIstatistikFiltrele_Click(object sender, EventArgs e)
        {
            string giristipi = "";
            if (radioButtonIstatistikSabahYiyen.Checked == true) { giristipi = "SABAH"; radioButtonIstatistikSabahYiyen.Checked = false; }
            else if (radioButtonIstatistikOgleYiyen.Checked == true) { giristipi = "OGLE"; radioButtonIstatistikOgleYiyen.Checked = false; }
            else if (radioButtonIstatistikAksamYiyen.Checked == true) { giristipi = "AKSAM"; radioButtonIstatistikAksamYiyen.Checked = false; }
            else if (radioButtonIstatistikAraOgunYiyen.Checked == true) { giristipi = "ARA"; radioButtonIstatistikAraOgunYiyen.Checked = false; }
            else { giristipi = ""; }

            dataGridViewSayisalIstatistik.Columns.Clear();


            if (giristipi == "" && comboBoxIstatistikSube.SelectedItem.ToString()=="Tümü")
            {
                label53.Text = dateTimePickerIstatistikBaslangic.Text + " tarihi ile " + dateTimePickerIstatistikBitis.Text +
                    " Tarihi Arası Tüm Öğünlerde Yemek Yiyen Kişi Sayıları";
                dataGridViewSayisalIstatistik.AllowUserToAddRows = true;
                dataGridViewSayisalIstatistik.Columns.Add("Tarih", "Tarih");
                dataGridViewSayisalIstatistik.Columns.Add("Gün", "Gün");
                dataGridViewSayisalIstatistik.Columns.Add("Sabah", "Sabah");
                dataGridViewSayisalIstatistik.Columns.Add("Öğle", "Öğle");
                dataGridViewSayisalIstatistik.Columns.Add("Akşam", "Akşam");
                dataGridViewSayisalIstatistik.Columns.Add("Ara Öğün", "Ara Öğün");
                TimeSpan span = Convert.ToDateTime(dateTimePickerIstatistikBitis.Text) - Convert.ToDateTime(dateTimePickerIstatistikBaslangic.Text);

                for (int i = 0; i <= span.TotalDays; i++)
                {
                    string tarih = Convert.ToDateTime(dateTimePickerIstatistikBaslangic.Text).AddDays(i).ToString("yyyy-MM-dd");
                    string gun = new CultureInfo("tr-TR").DateTimeFormat.GetDayName(Convert.ToDateTime(tarih).DayOfWeek).ToString();
                    int sabahSayi = YemekhaneFonksiyonlar.tarihleGiris("WHERE islem='" + "SABAH" + "' AND tarih BETWEEN '" + tarih + " 00:00' AND '" + tarih + " 23:59'");
                    int ogleSayi = YemekhaneFonksiyonlar.tarihleGiris("WHERE islem='" + "OGLE" + "' AND tarih BETWEEN '" + tarih + " 00:00' AND '" + tarih + " 23:59'");
                    int aksamSayi = YemekhaneFonksiyonlar.tarihleGiris("WHERE islem='" + "AKSAM" + "' AND tarih BETWEEN '" + tarih + " 00:00' AND '" + tarih + " 23:59'");
                    int araSayi = YemekhaneFonksiyonlar.tarihleGiris("WHERE islem='" + "ARA" + "' AND tarih BETWEEN '" + tarih + " 00:00' AND '" + tarih + " 23:59'");
                    dataGridViewSayisalIstatistik.Rows.Add(tarih, gun, sabahSayi, ogleSayi, aksamSayi, araSayi);
                }
                int toplamsabah = 0, toplamogle = 0, toplamaksam = 0, toplamara = 0;
                for (int satir = 0; satir < dataGridViewSayisalIstatistik.Rows.Count; satir++)
                {
                    toplamsabah += Convert.ToInt32(dataGridViewSayisalIstatistik.Rows[satir].Cells[2].Value);
                    toplamogle += Convert.ToInt32(dataGridViewSayisalIstatistik.Rows[satir].Cells[3].Value);
                    toplamaksam += Convert.ToInt32(dataGridViewSayisalIstatistik.Rows[satir].Cells[4].Value);
                    toplamara += Convert.ToInt32(dataGridViewSayisalIstatistik.Rows[satir].Cells[5].Value);
                }
                dataGridViewSayisalIstatistik.Rows.Add("Toplam", "Yemek Yiyen", toplamsabah,toplamogle,toplamaksam,toplamara);
            }
            else if (comboBoxIstatistikSube.SelectedItem.ToString() == "Tümü" && giristipi != "")
            {
                label53.Text = dateTimePickerIstatistikBaslangic.Text + " tarihi ile " + dateTimePickerIstatistikBitis.Text +
                    " Tarihi Arası " + giristipi + " Öğününde Yemek Yiyen Kişi Sayıları";
                dataGridViewSayisalIstatistik.AllowUserToAddRows = true;
                dataGridViewSayisalIstatistik.Columns.Add("Tarih", "Tarih");
                dataGridViewSayisalIstatistik.Columns.Add("Gün", "Gün");
                dataGridViewSayisalIstatistik.Columns.Add(comboBoxIstatistikSube.Text, comboBoxIstatistikSube.Text);
                dataGridViewSayisalIstatistik.Columns.Add("Yemek Tipi", "Yemek Tipi");
                TimeSpan span = Convert.ToDateTime(dateTimePickerIstatistikBitis.Text) - Convert.ToDateTime(dateTimePickerIstatistikBaslangic.Text);
                for (int i = 0; i <= span.TotalDays; i++)
                {
                    string tarih = Convert.ToDateTime(dateTimePickerIstatistikBaslangic.Text).AddDays(i).ToString("yyyy-MM-dd");
                    string gun = new CultureInfo("tr-TR").DateTimeFormat.GetDayName(Convert.ToDateTime(tarih).DayOfWeek).ToString();
                    int girensayi = YemekhaneFonksiyonlar.tarihleGiris("WHERE islem='" + giristipi + "' AND tarih BETWEEN '" + tarih + " 00:00' AND '" + tarih + " 23:59'");
                    dataGridViewSayisalIstatistik.Rows.Add(tarih, gun, girensayi, giristipi);
                }
                int toplamkisi = 0;
                for (int satir = 0; satir < dataGridViewSayisalIstatistik.Rows.Count; satir++)
                {
                    toplamkisi += Convert.ToInt32(dataGridViewSayisalIstatistik.Rows[satir].Cells[2].Value);
                }
                dataGridViewSayisalIstatistik.Rows.Add("Toplam", "Yemek Yiyen", toplamkisi, "Kişi");
            }
            else if(comboBoxIstatistikSube.SelectedItem.ToString()!="Tümü"&&giristipi=="")
            {
                label53.Text = dateTimePickerIstatistikBaslangic.Text + " tarihi ile " + dateTimePickerIstatistikBitis.Text +
                    " Tarihi Arası " + comboBoxIstatistikSube.Text + " Şubesinde Yemek Yiyen Kişi Sayıları";
                dataGridViewSayisalIstatistik.AllowUserToAddRows = true;
                dataGridViewSayisalIstatistik.Columns.Add("Tarih", "Tarih");
                dataGridViewSayisalIstatistik.Columns.Add("Gün", "Gün");
                dataGridViewSayisalIstatistik.Columns.Add(comboBoxIstatistikSube.Text + "-SABAH", comboBoxIstatistikSube.Text + "-SABAH");
                dataGridViewSayisalIstatistik.Columns.Add(comboBoxIstatistikSube.Text + "-OGLE", comboBoxIstatistikSube.Text + "-OGLE");
                dataGridViewSayisalIstatistik.Columns.Add(comboBoxIstatistikSube.Text + "-AKSAM", comboBoxIstatistikSube.Text + "-AKSAM");
                dataGridViewSayisalIstatistik.Columns.Add(comboBoxIstatistikSube.Text + "-ARAOGUN", comboBoxIstatistikSube.Text + "-ARAOGUN");
                TimeSpan span = Convert.ToDateTime(dateTimePickerIstatistikBitis.Text) - Convert.ToDateTime(dateTimePickerIstatistikBaslangic.Text);
                for (int i = 0; i <= span.TotalDays; i++)
                {
                    string tarih = Convert.ToDateTime(dateTimePickerIstatistikBaslangic.Text).AddDays(i).ToString("yyyy-MM-dd");
                    string gun = new CultureInfo("tr-TR").DateTimeFormat.GetDayName(Convert.ToDateTime(tarih).DayOfWeek).ToString();
                    int sabahSayi = YemekhaneFonksiyonlar.tarihleGiris("WHERE islem='" + "SABAH" + "' AND ogrenci.ogrencisubesi='" + comboBoxIstatistikSube.SelectedItem.ToString() + "' AND tarih BETWEEN '" + tarih + " 00:00' AND '" + tarih + " 23:59'");
                    int ogleSayi = YemekhaneFonksiyonlar.tarihleGiris("WHERE islem='" + "OGLE" + "' AND ogrenci.ogrencisubesi='" + comboBoxIstatistikSube.SelectedItem.ToString() + "' AND tarih BETWEEN '" + tarih + " 00:00' AND '" + tarih + " 23:59'");
                    int aksamSayi = YemekhaneFonksiyonlar.tarihleGiris("WHERE islem='" + "AKSAM" + "' AND ogrenci.ogrencisubesi='" + comboBoxIstatistikSube.SelectedItem.ToString() + "' AND tarih BETWEEN '" + tarih + " 00:00' AND '" + tarih + " 23:59'");
                    int araSayi = YemekhaneFonksiyonlar.tarihleGiris("WHERE islem='" + "ARA" + "' AND ogrenci.ogrencisubesi='" + comboBoxIstatistikSube.SelectedItem.ToString() + "' AND tarih BETWEEN '" + tarih + " 00:00' AND '" + tarih + " 23:59'");
                    dataGridViewSayisalIstatistik.Rows.Add(tarih, gun, sabahSayi,ogleSayi,aksamSayi,araSayi);
                }
                int toplamsabah = 0, toplamogle = 0, toplamaksam = 0, toplamara = 0;
                for (int satir = 0; satir < dataGridViewSayisalIstatistik.Rows.Count; satir++)
                {
                    toplamsabah += Convert.ToInt32(dataGridViewSayisalIstatistik.Rows[satir].Cells[2].Value);
                    toplamogle += Convert.ToInt32(dataGridViewSayisalIstatistik.Rows[satir].Cells[3].Value);
                    toplamaksam += Convert.ToInt32(dataGridViewSayisalIstatistik.Rows[satir].Cells[4].Value);
                    toplamara += Convert.ToInt32(dataGridViewSayisalIstatistik.Rows[satir].Cells[5].Value);
                }
                dataGridViewSayisalIstatistik.Rows.Add("Toplam", "Yemek Yiyen", toplamsabah, toplamogle, toplamaksam, toplamara);
            }
            else if (comboBoxIstatistikSube.SelectedItem.ToString() != "Tümü" && giristipi != "")
            {
                label53.Text = dateTimePickerIstatistikBaslangic.Text + " tarihi ile " + dateTimePickerIstatistikBitis.Text +
                    " Tarihi Arası " + comboBoxIstatistikSube.Text + " Şubesinde " + giristipi + " Öğününde Yemek Yiyen Kişi Sayıları";
                dataGridViewSayisalIstatistik.AllowUserToAddRows = true;
                dataGridViewSayisalIstatistik.Columns.Add("Tarih", "Tarih");
                dataGridViewSayisalIstatistik.Columns.Add("Gün", "Gün");
                dataGridViewSayisalIstatistik.Columns.Add(comboBoxIstatistikSube.Text, comboBoxIstatistikSube.Text);
                dataGridViewSayisalIstatistik.Columns.Add("Yemek Tipi", "Yemek Tipi");
                TimeSpan span = Convert.ToDateTime(dateTimePickerIstatistikBitis.Text) - Convert.ToDateTime(dateTimePickerIstatistikBaslangic.Text);
                for (int i = 0; i <= span.TotalDays; i++)
                {
                    string tarih = Convert.ToDateTime(dateTimePickerIstatistikBaslangic.Text).AddDays(i).ToString("yyyy-MM-dd");
                    string gun = new CultureInfo("tr-TR").DateTimeFormat.GetDayName(Convert.ToDateTime(tarih).DayOfWeek).ToString();
                    int girensayi = YemekhaneFonksiyonlar.tarihleGiris("WHERE islem='" + giristipi + "' AND ogrenci.ogrencisubesi='" + comboBoxIstatistikSube.SelectedItem.ToString() + "' AND tarih BETWEEN '" + tarih + " 00:00' AND '" + tarih + " 23:59'");
                    dataGridViewSayisalIstatistik.Rows.Add(tarih, gun, girensayi,giristipi);
                }
                int toplamkisi = 0;
                for(int satir = 0; satir < dataGridViewSayisalIstatistik.Rows.Count; satir++)
                {
                    toplamkisi += Convert.ToInt32(dataGridViewSayisalIstatistik.Rows[satir].Cells[2].Value);
                }
                dataGridViewSayisalIstatistik.Rows.Add("Toplam", "Yemek Yiyen", toplamkisi, "Kişi");
            }
        }

        private bool SetupThePrintingIstatistik()
        {
            string baslik = "*" + label53.Text;
            PrintDialog MyPrintDialog = new PrintDialog();
            MyPrintDialog.AllowCurrentPage = false;
            MyPrintDialog.AllowPrintToFile = false;
            MyPrintDialog.AllowSelection = true;
            MyPrintDialog.AllowSomePages = false;
            MyPrintDialog.PrintToFile = false;
            MyPrintDialog.ShowHelp = false;
            MyPrintDialog.ShowNetwork = false;

            //if (MyPrintDialog.ShowDialog() != DialogResult.OK)
            //    return false;

            printDocument3.DocumentName = "Istatistikler";
            printDocument3.PrinterSettings = MyPrintDialog.PrinterSettings;
            printDocument3.DefaultPageSettings = MyPrintDialog.PrinterSettings.DefaultPageSettings;
            printDocument3.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(40, 40, 80, 40);

            if (MessageBox.Show("Raporu sayfaya ortalamak ister misiniz?",
             "Rapor Ortalaması", MessageBoxButtons.YesNo,
             MessageBoxIcon.Question) == DialogResult.Yes)
                MyDataGridViewPrinter = new DataGridViewPrinter(dataGridViewSayisalIstatistik,
                printDocument3, true, true, "*" + label53.Text, new Font("Calibri", 12,
                FontStyle.Bold, GraphicsUnit.Point), System.Drawing.Color.Black, true);
            else
                MyDataGridViewPrinter = new DataGridViewPrinter(dataGridViewSayisalIstatistik,
                printDocument3, false, true, baslik, new Font("Calibri", 12,
                FontStyle.Bold, GraphicsUnit.Point), System.Drawing.Color.Black, true);

            return true;
        }

        private void pictureBoxIstatistikYazdir_Click(object sender, EventArgs e)
        {
            if (SetupThePrintingIstatistik())
            {
                PrintPreviewDialog MyPrintPreviewDialog = new PrintPreviewDialog();
                MyPrintPreviewDialog.Document = printDocument3;
                ((Form)MyPrintPreviewDialog).WindowState = FormWindowState.Maximized;
                MyPrintPreviewDialog.PrintPreviewControl.Zoom = 1;
                MyPrintPreviewDialog.ShowDialog();
            }
        }

        private void printDocument3_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            bool more = MyDataGridViewPrinter.DrawDataGridView(e.Graphics);
            if (more == true)
                e.HasMorePages = true;
        }
        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            labelsaat.Text = DateTime.Now.ToString("HH:mm");
            labeltarih.Text = DateTime.Now.ToString("dd-MM-yyyy");
        }
    }
}