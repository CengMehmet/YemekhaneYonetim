using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace RTEvents
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            license Key = new license();
            string key1 = Key.CPUSeriNo();
            string key2 = Key.HDDserino();
            string key3 = Key.AnakartSerino();
            string key = key1 + key2;
            StreamReader sr = new StreamReader("sqlconfig.txt"); string parola = sr.ReadLine();
            if (parola == key)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form2());
            }
            else { MessageBox.Show("Lisans Hatası"); }
        }
    }
}