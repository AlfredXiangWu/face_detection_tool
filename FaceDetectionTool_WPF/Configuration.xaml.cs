using System;
using System.Windows;
using System.Windows.Controls;
using FDialogResult = System.Windows.Forms.DialogResult;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;
using static FaceDetectionTool_WPF.Properties.Settings;

namespace FaceDetectionTool_WPF
{
    /// <summary>
    /// Configuration.xaml 的交互逻辑
    /// </summary>
    public partial class Configuration : Window
    {
        public Configuration()
        {
            InitializeComponent();
        }

        public IO IO { get; set; }
        public Action<Configuration> Accept { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)(((Button)sender).Tag);
            var fbd = new FolderBrowserDialog();
            fbd.Description = "Select Path";
            if (fbd.ShowDialog() == FDialogResult.OK)
                textBox.Text = fbd.SelectedPath;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (IO == null)
                IO = new IO();
            DataContext = IO;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            SaveConfig();
            if (Accept != null)
                Accept(this);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveConfig()
        {
            Default.ImagePath = IO.FrmImagePath;
            Default.GtFrPath = IO.FrmGtFrPath;
            Default.List = IO.FrmList;
            Default.DetectionFrPath = IO.FrmDetectionFrPath;
            Default.Save();
        }
    }
}
