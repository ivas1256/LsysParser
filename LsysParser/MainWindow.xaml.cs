using NLog;
using LsysParser.Data;
using LsysParser.Data.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LsysParser.Robot.Abstract;
using Microsoft.Win32;
using LsysParser.Robot;

namespace LsysParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = new ProjectViewModel();
                vm.Project.LogEvent += Project_LogEvent;

                DataContext = vm;
            }
            catch (Exception ex)
            {
                MessageProvider.Error(ex, "Не удалось загрузить рабочую область");
                logger.Error(ex);
            }
        }

        private void Project_LogEvent(string message)
        {
            Dispatcher.Invoke(() =>
            {
                tb_log.AppendText(message + Environment.NewLine);
            });
        }

        BasicParser parser;
        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            var prj = (DataContext as ProjectViewModel)?.Project;
            if (prj == null)
                return;

            parser = new Robot.MandersRu_Parser(prj);

            var bgw = new BackgroundWorker();
            bgw.DoWork += (a, b) => parser.Start();

            bgw.RunWorkerAsync();
        }

        private void btn_pause_Click(object sender, RoutedEventArgs e)
        {
            parser?.Pause();
        }

        private void btn_stop_Click(object sender, RoutedEventArgs e)
        {
            parser?.Stop();
        }

        private void btn_showErrors_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            var prj = (DataContext as ProjectViewModel)?.Project;
            if (prj == null)
                return;

            var sfd = new SaveFileDialog();
            sfd.Filter = "Excel файлы|*.xlsx";
            if (!sfd.ShowDialog().Value)
                return;

            try
            {
                Task.Run(() =>
                {
                    var excel = new ExcelSaver(prj);
                    excel.Save(sfd.FileName);

                    MessageProvider.Successfully("Сохранено");
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                MessageProvider.UnknownError(ex);
            }
        }

        private void btn_checkErrors_Click(object sender, RoutedEventArgs e)
        {
            var prj = (DataContext as ProjectViewModel)?.Project;
            if (prj == null)
                return;

            parser = new Robot.MandersRu_Parser(prj);

            var bgw = new BackgroundWorker();
            bgw.DoWork += (a, b) => parser.CheckErrors();

            bgw.RunWorkerAsync();
        }

        private void btn_dwlImages_Click(object sender, RoutedEventArgs e)
        {
            var prj = (DataContext as ProjectViewModel)?.Project;
            if (prj == null)
                return;

            parser = new Robot.MandersRu_Parser(prj);

            var bgw = new BackgroundWorker();
            bgw.DoWork += (a, b) => parser.DownloadImages();

            bgw.RunWorkerAsync();
        }

        private void btn_missed_Click(object sender, RoutedEventArgs e)
        {
            var prj = (DataContext as ProjectViewModel)?.Project;
            if (prj == null)
                return;

            parser = new Robot.MandersRu_Parser(prj);

            var bgw = new BackgroundWorker();
            bgw.DoWork += (a, b) => parser.ParseMissedProducts();

            bgw.RunWorkerAsync();
        }
    }
    class ProjectViewModel
    {
        Project project;
        public Project Project
        {
            get
            {
                return project;
            }

            set
            {
                project = value;
            }
        }

        public ProjectViewModel()
        {
            project = new Project()
            {
                Name = "Новый проект"
            };
        }
    }
}
