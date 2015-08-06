using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using FDialogResult = System.Windows.Forms.DialogResult;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;

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

        public ImagePath ImagePath { get; set; }

        private ObservableCollection<ImagePath> pathList;
        public Action<Configuration> Accept { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)(((Button)sender).Tag);
            var fbd = new FolderBrowserDialog();
            fbd.Description = "Select Path";
            if (fbd.ShowDialog() == FDialogResult.OK)
                textBox.Text = fbd.SelectedPath;
        }

        private void File_Click(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)(((Button)sender).Tag);
            var ofd = new OpenFileDialog();
            ofd.Title = "Select File";
            if (ofd.ShowDialog() == true)
                textBox.Text = ofd.FileName;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadOrSave(true);
            DataContext = ImagePath;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (Accept != null)
                Accept(this);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cbSelection.Text))
            { MessageBox.Show("请输入名称"); return; }
            if (pathList.SingleOrDefault(p => p.Name == cbSelection.Text.Trim()) != null)
            { MessageBox.Show("请输入新名称"); return; }
            pathList.Add(new ImagePath() { Name = cbSelection.Text });
            cbSelection.SelectedIndex = pathList.Count - 1;
        }

        private void Del_Click(object sender, RoutedEventArgs e)
        {
            if (cbSelection.SelectedItem != null)
            {
                pathList.Remove(cbSelection.SelectedItem as ImagePath);
                cbSelection.SelectedIndex = pathList.Count - 1;
            }
        }

        private void cbSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbSelection.SelectedItem != null)
            {
                ImagePath = cbSelection.SelectedItem as ImagePath;
                DataContext = ImagePath;
            }
        }

        private void LoadOrSave(bool isLoad)
        {
            var file = new FileInfo(Properties.Settings.Default.PathsFileName);
            var xml = new XmlSerializer(typeof(ImagePath[]));
            if (isLoad)
            {
                if (!file.Exists)
                {
                    pathList = new ObservableCollection<ImagePath>();
                    pathList.Add(new ImagePath() { Name = "Default" });
                }
                else
                {
                    var s = File.OpenRead(file.FullName);
                    pathList = new ObservableCollection<ImagePath>((ImagePath[])xml.Deserialize(s));
                    s.Close();
                }
                cbSelection.ItemsSource = pathList;
                cbSelection.SelectedIndex = 0;
            }
            else
            {
                if (file.Exists)
                    file.Delete();
                if (pathList.Count == 0)
                    return;
                var s = File.OpenWrite(file.FullName);
                xml.Serialize(s, pathList.ToArray());
                s.Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LoadOrSave(false);
        }
    }
}
