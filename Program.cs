﻿using System;
using System.Windows.Forms;

namespace ProductManagement41 // Proje adın bu. Senin proje adınla aynı olmalı.
{
    static class Program
    {
        /// <summary>
        /// Uygulamanın ana giriş noktası.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1()); // Uygulama Form1'i başlatacak
        }
    }
}