using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NXStartCenter
{
    public partial class DownloadProgressWindow : Window
    {
        public DownloadProgressWindow()
        {
            InitializeComponent();
        }

        public void SetProgress(double value)
        {
            DownloadProgressBar.Value = value;
            ProgressText.Text = $"{value:0}%";
        }
    }
}