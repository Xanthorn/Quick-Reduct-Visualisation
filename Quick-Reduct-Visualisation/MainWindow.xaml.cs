﻿using Newtonsoft.Json;
using Quick_Reduct_Visualisation.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace Quick_Reduct_Visualisation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Algorithms algorithms = new();
        public DispatcherTimer myTimer, preductTimer;
        public MainWindow()
        {
            InitializeComponent();
            myTimer = new DispatcherTimer();
            myTimer.Tick += new EventHandler(AutomaticCells);
            myTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            preductTimer = new DispatcherTimer();
            preductTimer.Tick += new EventHandler(CellToPrereduct);
            preductTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);

        }
        public void DataToJson(object sender, RoutedEventArgs e)
        {
            string jsonData = JsonConvert.SerializeObject(algorithms);
            //string programPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //string savePath = programPath + "\\savestate.kek";
            //File.WriteAllText(savePath, jsonData);

            Microsoft.Win32.SaveFileDialog fileDialog = new Microsoft.Win32.SaveFileDialog();
            fileDialog.FileName = "savestate"; 
            fileDialog.DefaultExt = ".kek"; 
            fileDialog.Filter = "kek savestates (*.kek)|*.kek"; 

            bool? result = fileDialog.ShowDialog();

            if (result == true)
            {
                File.WriteAllText(fileDialog.FileName, jsonData);
            }
            
        }
        public void UpdateDataGrid()
        {
            DataTable dt = new DataTable();
            int nbColumns = algorithms.data.dataSets.Count();
            int nbRows = algorithms.data.dataSets.Count();
            for (int i = 0; i < nbColumns + 1; i++)
            {
                if (i == 0)
                {
                    dt.Columns.Add("●");
                    continue;
                }
                dt.Columns.Add($"x{i}");
            }
            for (int row = 0; row < nbRows; row++)
            {
                DataRow dr = dt.NewRow();
                for (int col = 0; col < nbColumns + 1; col++)
                {
                    if (col == 0)
                    {
                        dr[col] = $"x{row + 1}";
                        continue;
                    }
                    dr[col] = algorithms.data.differenceTableResults[row, col - 1];
                }
                dt.Rows.Add(dr);
            }
            dataGrid.ItemsSource = dt.DefaultView;
            DataTable dt2 = new DataTable();
            int nbColumns2 = 2;
            int nbRows2 = algorithms.data.differenceTableCount.Count - 1;
            for (int i = 0; i < nbColumns2; i++)
            {
                if (i == 0)
                {
                    dt2.Columns.Add("●");
                    continue;
                }
                dt2.Columns.Add("Value");
            }
            for (int row = 0; row < nbRows2; row++)
            {
                DataRow dr2 = dt2.NewRow();
                for (int col = 0; col < nbColumns2; col++)
                {
                    if (col == 0)
                    {
                        dr2[col] = $"{algorithms.data.attributes[row]}";
                        continue;
                    }
                    dr2[col] = algorithms.data.differenceTableCount[$"{algorithms.data.attributes[row]}"];
                }
                dt2.Rows.Add(dr2);
            }
            dataCountGrid.ItemsSource = dt2.DefaultView;
        }
        private void ShowResults()
        {
            starter.Visibility = Visibility.Visible;
            stopper.Visibility = Visibility.Hidden;
            oneStep.IsEnabled = false;
            starter.IsEnabled = false;
            stopper.IsEnabled = false;
            prereduct.IsEnabled = false;
            save.IsEnabled = true;
            loadData.IsEnabled = true;
            restart.IsEnabled = true;
            resultText.Text = "(";
            foreach (string s in algorithms.data.reduct)
            {
                resultText.Text += s + ",";
            }
            resultText.Text = resultText.Text.Remove(resultText.Text.Length - 1);
            resultText.Text += ")";
            MessageBox.Show("Reduct is: " + resultText.Text);
            return;
        }
        private void CalculateAndDisplay()
        {
            for (int i = 0; i < algorithms.data.dataSets[i].Length - 1; i++)
            {
                algorithms.CalculateQuickReduct();
            }
            int zeroCount = 0;
            foreach (int i in algorithms.data.differenceTableCount.Values)
            {
                if (i == 0 && algorithms.tryMeNow == true)
                    zeroCount++;
            }
            if (zeroCount == algorithms.data.differenceTableCount.Count)
            {
                algorithms.stopTheCount = true;
            }
            UpdateDataGrid();
            algorithms.k = 0;
        }
        private void CellByCell(object sender, RoutedEventArgs e)
        {
            CalculateAndDisplay();
            if (algorithms.stopTheCount == true)
            {
                ShowResults();
            }
        }
        private void AutomaticCells(object sender, EventArgs e)
        {
            CalculateAndDisplay();
            if (algorithms.stopTheCount == true)
            {
                myTimer.Stop();
                ShowResults();
            }
        }
        private void CellToPrereduct(object sender, EventArgs e)
        {
            CalculateAndDisplay();
            if (algorithms.stopTheCount == true)
            {
                ShowResults();
            }
            if (algorithms.tryMeNow == true)
            {
                preductTimer.Stop();
                starter.IsEnabled = true;
                EnableCommonButtons();
            }
        }
        private void CellToPrereductTimer(object sender, RoutedEventArgs e)
        {
            preductTimer.Start();
            starter.IsEnabled = false;
            DisableCommonButtons();
        }
        private void StopAuto(object sender, RoutedEventArgs e)
        {
            myTimer.Stop();
            stopper.Visibility = Visibility.Hidden;
            starter.Visibility = Visibility.Visible;
            EnableCommonButtons();
        }
        private void StartAuto(object sender, RoutedEventArgs e)
        {
            myTimer.Start();
            starter.Visibility = Visibility.Hidden;
            stopper.Visibility = Visibility.Visible;
            DisableCommonButtons();
        }
        private void EnableCommonButtons()
        {
            oneStep.IsEnabled = true;
            prereduct.IsEnabled = true;
            restart.IsEnabled = true;
            save.IsEnabled = true;
            loadData.IsEnabled = true;
        }
        private void DisableCommonButtons()
        {
            oneStep.IsEnabled = false;
            prereduct.IsEnabled = false;
            restart.IsEnabled = false;
            save.IsEnabled = false;
            loadData.IsEnabled = false;
        }

        private void Restart(object sender, RoutedEventArgs e)
        {
            algorithms.Restart();
            UpdateDataGrid();
            EnableCommonButtons();
            stopper.Visibility = Visibility.Hidden;
            starter.Visibility = Visibility.Visible;
            starter.IsEnabled = true;
            stopper.IsEnabled = true;
        }

        private void LoadData(object sender, RoutedEventArgs e)
        {
            algorithms = new Algorithms();
            algorithms.data.GetData();
            if (algorithms.data.filePath == null)
                return;
            else if (algorithms.data.filePath.Contains(".kek"))
            {
                algorithms = JsonConvert.DeserializeObject<Algorithms>(File.ReadAllText(algorithms.data.filePath));
                UpdateDataGrid();
                if (algorithms.stopTheCount == true)
                    ShowResults();
            }
            else UpdateDataGrid();
            if (algorithms.stopTheCount == true)
                return;
            starter.IsEnabled = true;
            stopper.IsEnabled = true;
            EnableCommonButtons();
        }
    }
}
