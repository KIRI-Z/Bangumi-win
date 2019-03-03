﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using BangumiX.Common;
using BangumiX.Properties;

namespace BangumiX.Views
{
    /// <summary>
    /// Interaction logic for Collection.xaml
    /// </summary>
    public partial class WatchingCollection : UserControl
    {
        public List<Model.Collection> subject_list;
        public Subject SubjectControl;
        public WatchingCollection()
        {
            InitializeComponent();
            ListViewCollections.SelectionChanged += ListViewCollectionsSelectedIndexChanged;
        }

        public void Switch(ref List<Model.Collection> ref_ordered_collects)
        {
            subject_list = ref_ordered_collects;
            DataContext = subject_list;
            return;
        }

        public async void ListViewCollectionsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (subject_list == null) return;
            var index = ListViewCollections.SelectedIndex;
            if (index == -1) return;
            if (SubjectControl != null) SubjectControl.DataContext = null;
            ApiHelper.SubjectResult subject_result = new ApiHelper.SubjectResult();
            subject_result = await ApiHelper.GetSubject(subject_list[index].subject_id);
            if (subject_result.Status != 1) return;

            var subject = subject_result.Subject;
            if (SubjectControl == null)
            {
                SubjectControl = new Subject();
                Grid.SetColumn(SubjectControl, 1);
                GridMain.Children.Add(SubjectControl);
                SubjectControl.DataContext = subject;
                SubjectControl.buttonSummary.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            else
            {
                SubjectControl.DataContext = subject;
                SubjectControl.buttonSummary.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }

            ApiHelper.ProgressResult progress_result = new ApiHelper.ProgressResult();
            progress_result = await ApiHelper.GetProgress(Settings.Default.UserID, subject.id);
            if (progress_result.Status == 1)
            {
                if (progress_result.SubjectProgress != null)
                {
                    if (progress_result.SubjectProgress.eps != null)
                    {
                        foreach (var ep_src in subject.eps)
                        {
                            foreach (var ep in progress_result.SubjectProgress.eps)
                            {
                                if (ep.id == ep_src.id) ep_src.ep_status = ep.status.id;
                            }
                        }
                    }
                }
            }
            subject.eps_filter();
            SubjectControl.subject_episodes.EpisodeItemsControl.ItemsSource = subject.eps_normal;
            return;
        }

        private async void WishCollectClick(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext;
            var index = ListViewCollections.Items.IndexOf(item);
            var http_result = await ApiHelper.UpdateCollection(subject_list[index].subject_id, "wish");
            if (http_result.Status != 1) Console.WriteLine("UpdateFailed");
        }
        private async void WatchingCollectClick(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext;
            var index = ListViewCollections.Items.IndexOf(item);
            var http_result = await ApiHelper.UpdateCollection(subject_list[index].subject_id, "do");
            if (http_result.Status != 1) Console.WriteLine("UpdateFailed");
        }
        private async void WatchedCollectClick(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext;
            var index = ListViewCollections.Items.IndexOf(item);
            var http_result = await ApiHelper.UpdateCollection(subject_list[index].subject_id, "collect");
            if (http_result.Status != 1) Console.WriteLine("UpdateFailed");
        }
        private async void HoldCollectClick(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext;
            var index = ListViewCollections.Items.IndexOf(item);
            var http_result = await ApiHelper.UpdateCollection(subject_list[index].subject_id, "on_hold");
            if (http_result.Status != 1) Console.WriteLine("UpdateFailed");
        }
        private async void DropCollectClick(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext;
            var index = ListViewCollections.Items.IndexOf(item);
            var http_result = await ApiHelper.UpdateCollection(subject_list[index].subject_id, "dropped");
            if (http_result.Status != 1) Console.WriteLine("UpdateFailed");
        }
    }
}