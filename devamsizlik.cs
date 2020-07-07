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
    public partial class devamsizlik : Form
    {
        DataGridViewPrinter MyDataGridViewPrinter;
        string ilktarih, sontarih,ogrencinum,ogrenciadisoyadi;
        public devamsizlik()
        {
            InitializeComponent();
        }

        public void doldur(string ogrencino,string ogrenciadi,string baslangictarihi,string bitistarihi,string girisyoklamasaati)
        {
            label2.Text = YemekhaneFonksiyonlar.Getir(ogrencino).no;
            ilktarih = baslangictarihi;
            sontarih = bitistarihi;
            ogrencinum = ogrencino;
            ogrenciadisoyadi = ogrenciadi;
            label1.Text = ogrencino;
            labelToplamGun.Text = ((Convert.ToDateTime(bitistarihi).Date - Convert.ToDateTime(baslangictarihi).Date).Days).ToString();

            int haftasonu = 0;
            int gelmedigisabah=0,gelmedigiogle=0,gelmedigiaksam=0,gelmedigiaraogun=0;

            List<string> yemekSaatleri = YemekhaneFonksiyonlar.YemekProgramiGetir(); 
            
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("Tarih", "Tarih");
            dataGridView1.Columns.Add("Sabah Yemeği", "Sabah Yemeği");
            dataGridView1.Columns.Add("Öğle Yemeği", "Öğle Yemeği");
            dataGridView1.Columns.Add("Akşam Yemeği", "Akşam Yemeği");
            dataGridView1.Columns.Add("Ara Öğün", "Ara Öğün");


            for (DateTime i = Convert.ToDateTime(baslangictarihi); i < Convert.ToDateTime(bitistarihi); i=i.AddDays(1))
            {
                List<Ogrenci> sabahList = YemekhaneFonksiyonlar.HareketGetir(
                    " where tarih BETWEEN'" + i.ToString("yyyy-MM-dd") + " " + yemekSaatleri[0] + "' AND '" +
                    i.ToString("yyyy-MM-dd") + " " + yemekSaatleri[1] + "'AND ogrencino = '" + ogrencinum +
                    "' ORDER BY tarih DESC");
                string giristarihi = "";
                string[] girissaati = new string[4];
                int l = 0;
                
                try
                {
                    giristarihi = sabahList[0].yemekgirisTarih.ToString();
                    girissaati[0] = Convert.ToDateTime(giristarihi).ToShortTimeString();// ToString("HH:mm");    
                    l++;
                }
                catch (Exception){}
                
                if (l == 0) { girissaati[0] = "Yemek Yemedi";gelmedigisabah++;}
                l = 0;

                List<Ogrenci> ogleList = YemekhaneFonksiyonlar.HareketGetir(
                    " where tarih BETWEEN'" + i.ToString("yyyy-MM-dd") + " " + yemekSaatleri[2] + "' AND '" +
                    i.ToString("yyyy-MM-dd") + " " + yemekSaatleri[3] + "'AND ogrencino = '" + ogrencinum +
                    "' ORDER BY tarih DESC");
                try
                {
                    giristarihi = ogleList[0].yemekgirisTarih.ToString();
                    girissaati[1] = Convert.ToDateTime(giristarihi).ToShortTimeString();// ToString("HH:mm");
                    l++;
                }
                catch (Exception e) { }

                
                if (l == 0) { girissaati[1] = "Yemek Yemedi"; gelmedigiogle++; }
                l = 0;

                List<Ogrenci> aksamList = YemekhaneFonksiyonlar.HareketGetir(
                    " where tarih BETWEEN'" + i.ToString("yyyy-MM-dd") + " " + yemekSaatleri[4] + "' AND '" +
                    i.ToString("yyyy-MM-dd") + " " + yemekSaatleri[5] + "'AND ogrencino = '" + ogrencinum +
                    "' ORDER BY tarih DESC");
                try
                {
                    giristarihi = aksamList[0].ToString();
                    girissaati[2] = Convert.ToDateTime(giristarihi).ToShortTimeString();// ToString("HH:mm");
                    l++;
                }
                catch (Exception){}
                if (l == 0) { girissaati[2] = "Yemek Yemedi"; gelmedigiaksam++; }
                l = 0;

                List<Ogrenci> araOgunList = YemekhaneFonksiyonlar.HareketGetir(
                    " where tarih BETWEEN'" + i.ToString("yyyy-MM-dd") + " " + yemekSaatleri[6] + "' AND '" +
                    i.ToString("yyyy-MM-dd") + " " + yemekSaatleri[7] + "'AND ogrencino = '" + ogrencinum +
                    "' ORDER BY tarih DESC");
                try
                {
                    giristarihi = araOgunList[0].ToString();
                    girissaati[3] = Convert.ToDateTime(giristarihi).ToShortTimeString(); // ToString("HH:mm");
                    l++;
                }
                catch (Exception e){}
               
                if (l == 0) { girissaati[3] = "Yemek Yemedi"; gelmedigiaraogun++; }
                if (i.DayOfWeek == DayOfWeek.Saturday || i.DayOfWeek == DayOfWeek.Sunday)
                {
                    haftasonu++;
                    dataGridView1.Rows.Add(i.ToShortDateString(), "HAFTASONU", "HAFTASONU","HAFTASONU","HAFTASONU");
                    dataGridView1.Rows[dataGridView1.Rows.Count-1].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dataGridView1.Rows.Add(i.ToShortDateString(), girissaati[0], girissaati[1], girissaati[2],girissaati[3]);
                }
            }
            
            labelGelmedigiSabah.Text = (gelmedigisabah-haftasonu).ToString();
            labelGelmedigiOgle.Text = (gelmedigiogle - haftasonu).ToString();
            labelGelmedigiAksam.Text = (gelmedigiaksam - haftasonu).ToString();
            labelgelmedigiAraOgun.Text = (gelmedigiaraogun - haftasonu).ToString();


        }

        private void pictureBoxDevamsizlikYazdir_Click_1(object sender, EventArgs e)
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



        private bool SetupThePrinting()
        {
            string baslik = "*" + ilktarih + " İLE " + sontarih + " ARASI '"+ ogrencinum + "' NUMARALI '" + ogrenciadisoyadi +"' YEMEĞE GELİŞ LİSTESİ";
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
            printDocument1.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(40, 40, 80, 40);

            if (MessageBox.Show("Raporu sayfaya ortalamak ister misiniz?",
             "Rapor Ortalaması", MessageBoxButtons.YesNo,
             MessageBoxIcon.Question) == DialogResult.Yes)
                MyDataGridViewPrinter = new DataGridViewPrinter(dataGridView1,
                printDocument1, true, true, "*" +ilktarih + " İLE " + sontarih + " ARASI '" + ogrencinum + "' NUMARALI '" + ogrenciadisoyadi + "' YEMEĞE GELİŞ LİSTESİ", new Font("Tahoma", 12,
                FontStyle.Bold, GraphicsUnit.Point), System.Drawing.Color.Black, true);
            else
                MyDataGridViewPrinter = new DataGridViewPrinter(dataGridView1,
                printDocument1, false, true, baslik, new Font("Tahoma", 14,
                FontStyle.Bold, GraphicsUnit.Point), System.Drawing.Color.Black, true);

            return true;
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            bool more = MyDataGridViewPrinter.DrawDataGridView(e.Graphics);
            if (more == true)
                e.HasMorePages = true;
        }
    }
}
