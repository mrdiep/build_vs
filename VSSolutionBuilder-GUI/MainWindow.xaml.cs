using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using VSSolutionBuilder;

namespace VSSolutionBuilderGui
{
    public partial class MainWindow
    {
        private ObservableCollection<string> files = new ObservableCollection<string>();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            FileSlnTextBox.ItemsSource = files;
        }
        string _fileNameWithoutExtension;

        string FileNameWithoutExtension => _fileNameWithoutExtension ?? (_fileNameWithoutExtension = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (FileNameWithoutExtension.EndsWith("saved", StringComparison.Ordinal))
            {
                File.WriteAllText("source.txt", string.Join(Environment.NewLine, files));
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (FileNameWithoutExtension.EndsWith("saved", StringComparison.Ordinal) && File.Exists("source.txt"))
            {
                File.ReadLines("source.txt").ToList().ForEach(x => files.Add(x));
            }
        }

        private void FileSlnTextBox_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            var _files = (string[])e.Data.GetData(DataFormats.FileDrop);
            _files?.Where(x => x.EndsWith("sln", StringComparison.Ordinal) && !files.Contains(x)).ToList().ForEach(x => files.Add(x));
        }

        private void BuildAll_Click(object sender, RoutedEventArgs e) => new SlnBuilder().Build(files.ToArray());

        private void BuildSelectedSolution_Click(object sender, RoutedEventArgs e) => new SlnBuilder().Build(FileSlnTextBox.SelectedItems.Cast<string>().ToArray());

        private void RemoveFile_Clicked(object sender, RoutedEventArgs e) => FileSlnTextBox.SelectedItems.Cast<string>().ToList().ForEach(x => files.Remove(x));

        private void MoveUp_Clicked(object sender, RoutedEventArgs e) => Move("up");

        private void MoveTop_Clicked(object sender, RoutedEventArgs e) => Move("top");

        private void MoveBottom_Clicked(object sender, RoutedEventArgs e) => Move("bottom");

        private void MoveDown_Clicked(object sender, RoutedEventArgs e) => Move("down");

        public void Move(string type)
        {
            var selectedFiles = FileSlnTextBox.SelectedItems.Cast<string>().ToList();
            if (selectedFiles.Count != 1)
            {
                MessageBox.Show("Pls choose one solution", "Err");
                return;
            }

            var selectedFile = selectedFiles.Single();

            switch (type)
            {
                case "up":
                    if (selectedFile != files.First())
                    {
                        var newIndex = files.IndexOf(selectedFile);
                        files.RemoveAt(newIndex);
                        files.Insert(newIndex - 1, selectedFile);
                        FileSlnTextBox.SelectedItem = selectedFile;
                    }
                    break;

                case "down":
                    if (selectedFile != files.Last())
                    {
                        var currentIndex = files.IndexOf(selectedFile);

                        files.RemoveAt(currentIndex);
                        files.Insert(currentIndex + 1, selectedFile);
                        FileSlnTextBox.SelectedItem = selectedFile;
                    }
                    break;

                case "top":
                    if (selectedFile != files.First())
                    {
                        var newIndex = files.IndexOf(selectedFile);
                        files.RemoveAt(newIndex);
                        files.Insert(0, selectedFile);
                        FileSlnTextBox.SelectedItem = selectedFile;
                    }
                    break;

                case "bottom":
                    if (selectedFile != files.Last())
                    {
                        var currentIndex = files.IndexOf(selectedFile);

                        files.RemoveAt(currentIndex);
                        files.Add(selectedFile);
                        FileSlnTextBox.SelectedItem = selectedFile;
                    }
                    break;
            }
        }
    }
}