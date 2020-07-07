using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace RTEvents
{
    public partial class Form3 : Form
    {
        Main main;
        private FilterInfoCollection webcam;//webcam isminde tanımladığımız değişken bilgisayara kaç kamera bağlıysa onları tutan bir dizi. 
        private VideoCaptureDevice cam;//cam ise bizim kullanacağımız aygıt.
        Bitmap ogrenciresim;
        public Form3()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            webcam = new FilterInfoCollection(FilterCategory.VideoInputDevice);//webcam dizisine mevcut kameraları dolduruyoruz.
            //cam.DesiredFrameSize = new Size(600,600);
            main = ((Main)Application.OpenForms["RTEventsMain"]);

            YemekhaneFonksiyonlar.ComboboxDoldur("sube").ForEach(p => comboBox6.Items.Add(p));
            comboBoxcinsiyet.SelectedIndex = 0;
            comboBox6.SelectedIndex = 0;
            radioButton2.Select();
        }



        private void pictureBox3_Click(object sender, EventArgs e)
        {
            string ogrencitip = "";
            Ogrenci ogrenci = YemekhaneFonksiyonlar.Getir(textBox16.Text);
            if (radioButton1.Checked) { ogrencitip = "Bakiye"; }
            else if (radioButton2.Checked) { ogrencitip = "Girişsayı"; }
            ogrenci.TC = textBox17.Text;
            ogrenci.adi = textBox18.Text;
            ogrenci.soyadi = textBox19.Text;
            ogrenci.cinsiyeti = comboBoxcinsiyet.Text;
            ogrenci.ogretmeni = "";
            ogrenci.anneadi = textBox20.Text;
            ogrenci.babaadi = textBox21.Text;
            ogrenci.telefonu = maskedTextBoxogrencitel.Text;
            ogrenci.velitel = maskedTextBoxirtibattel.Text;
            ogrenci.sinifi = textBox24.Text;
            ogrenci.alani = textBox25.Text;
            ogrenci.subesi = comboBox6.Text;
            ogrenci.kartno = textBox26.Text;
            ogrenci.adresi = richTextBox2.Text;
            ogrenci.durumu = "0";
            ogrenci.ogrencibakiye = 0;
            ogrenci.kalangiris = 0;
            ogrenci.ogrenciTip = ogrencitip;
            ogrenci.velimail = textBoxVeliMail.Text;
            if (ogrenci.no == null)
            {
                try
                {
                    ogrenci.no = textBox16.Text;                    
                    YemekhaneFonksiyonlar.Kaydet(ogrenci);
                    MessageBox.Show("Öğrenci Kaydı Tamamlanmıştır.", "Öğrenci Kayıt");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Kayıt Tamamlanamadı.Lütfen Tekrar Deneyiniz.", "Hata!");
                }              
            }
            else
            {
                ogrenci.no = textBox16.Text;
                YemekhaneFonksiyonlar.Guncelle(ogrenci,ogrenci.no);
            }
            try
            {
                if (textBox16.Text != "")
                {
                    if (!Directory.Exists(@"C:/resimler"))
                    {
                        Directory.CreateDirectory(@"C:/resimler");
                    }
                    //ogrenciresim.Save(@"C:/resimler/" + textBox16.Text + "y.jpg", ImageFormat.Jpeg);
                    if(!File.Exists(@"C:/resimler/" + textBox16.Text + ".jpg")) { pictureBox5.Image.Save(@"C:/resimler/" + textBox16.Text + ".jpg", ImageFormat.Jpeg); }
                    else { File.Delete(@"C:/resimler/" + textBox16.Text + ".jpg"); pictureBox5.Image.Save(@"C:/resimler/" + textBox16.Text + ".jpg", ImageFormat.Jpeg); }
                }
            }
            catch (Exception) { }
            try
            {
                main.ogrencileriyenile();
            }
            catch (Exception)
            {
                
            }
            this.Close();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox16.Text != "")
                {
                    try
                    {
                        if (cam.IsRunning) { cam.Stop(); }
                    }
                    catch (Exception) { }
                    cam = new VideoCaptureDevice(webcam[0].MonikerString);
                    cam.NewFrame += new NewFrameEventHandler(cam_NewFrame);
                    cam.Start();//kamerayı başlatıyoruz.
                }
                else { MessageBox.Show("Önce Öğrenci Numarasını Giriniz"); }
            }
            catch (Exception)
            {
                
            }
            
        }

        void cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Bitmap bit = (Bitmap)eventArgs.Frame.Clone();
                pictureBox5.Image = bit;
            }
            catch (Exception)
            {
                
            }
            
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox16.Text != "")
                {
                    cam.Stop();
                }
            }
            catch (Exception)
            {
                
            }
        }

        private void textBox16_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                Ogrenci ogrenci = YemekhaneFonksiyonlar.Getir(textBox16.Text);
                
                textBox17.Text = ogrenci.TC;
                textBox18.Text = ogrenci.adi;
                textBox19.Text = ogrenci.soyadi;
                comboBoxcinsiyet.Text = ogrenci.cinsiyeti;
                textBox20.Text =ogrenci.anneadi;
                textBox21.Text = ogrenci.babaadi;
                maskedTextBoxogrencitel.Text = ogrenci.telefonu;
                maskedTextBoxirtibattel.Text = ogrenci.velitel;
                textBox24.Text = ogrenci.sinifi;
                comboBox6.Text = ogrenci.alani;
                textBox25.Text = ogrenci.subesi;
                textBox26.Text = ogrenci.kartno;
                richTextBox2.Text = ogrenci.adresi;
                string ogrencitip = ogrenci.ogrenciTip;
                textBoxVeliMail.Text = ogrenci.velimail;
                if (ogrencitip == "Bakiye") { radioButton1.Checked = true; }
                else if (ogrencitip == "Girişsayı") { radioButton2.Checked = true; }
                try
                {
                    pictureBox5.Image = Helper.ResimYukle(ogrenci.no);
                }
                catch (Exception){ }
            }
        } //numara ile öğrenci arama

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (cam != null)
                {
                    cam.Stop();
                }               
            }
            catch (Exception)
            {
            }
           
        }
    }
}
