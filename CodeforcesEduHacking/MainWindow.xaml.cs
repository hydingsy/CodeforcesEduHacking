﻿using System;
using System.Collections.Generic;
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
using CodeforcesPlatform;
using Newtonsoft.Json.Linq;

namespace CodeforcesEduHacking
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private CodeforcesAPI codeforcesApi = null;
        private JObject contestList = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async Task LoadContestList()
        {
            contestList = await codeforcesApi.GetContestListAsync();
            if (contestList["status"].ToString() == "OK")
            {
                var list = contestList["result"];
                foreach (var item in list)
                {
                    string now = item["name"].ToString().Trim();
                    if (now.StartsWith("Educational Codeforces Round") || now.EndsWith("(Div. 3)"))
                        contestListComboBox.Items.Add(string.Format("{0,4} {1}", item["id"], item["name"]));
                }
                contestListComboBox.SelectedIndex = list.Count() > 0 ? 0 : -1;
            }
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                codeforcesApi = new CodeforcesAPI();
                await LoadContestList();

                ///
                //for (int i = 444; i < 470; i++)
                //{
                //    contestListComboBox.Items.Add(" " + i.ToString() + " hahahahah");
                //}
                ///

                titleLabel.Content = "请选择一个 Edu Round";
                hackCountButton.IsEnabled = true;
                hackItButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void hackCountButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("正在赶工制作中……");
        }

        private void hackItButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string contestId = contestListComboBox.Text.Substring(0, 4).Trim();
                new SelectedWindow(contestId).Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
