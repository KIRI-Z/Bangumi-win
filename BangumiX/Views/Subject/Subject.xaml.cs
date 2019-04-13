﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BangumiX.View
{
    public sealed partial class Subject : UserControl
    {
        public ViewModel.SubjectViewModel subjectVM;
        public SubjectSummary subjectSummary;
        public SubjectEpisode subjectEpisode;
        public SubjectCharacter subjectCharacter;
        public SubjectStaff subjectStaff;
        public SubjectComment subjectComment;

        public Subject()
        {
            subjectVM = new ViewModel.SubjectViewModel();
            subjectSummary = new SubjectSummary(ref subjectVM);
            subjectEpisode = new SubjectEpisode(ref subjectVM);
            subjectCharacter = new SubjectCharacter(ref subjectVM);
            subjectStaff = new SubjectStaff(ref subjectVM);
            subjectComment = new SubjectComment();
            this.InitializeComponent();

            pivotSummary.Content = subjectSummary;
            pivotEpisode.Content = subjectEpisode;
            pivotCharacter.Content = subjectCharacter;
            pivotStaff.Content = subjectStaff;
            pivotComment.Content = subjectComment;
        }

        private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (progressGridView.SelectedItem == null) return;
            pivotSubject.SelectedIndex = 1;
            var item = progressGridView.SelectedItem as ViewModel.EpisodeViewModel;
            if (item.Sort == "…") return;
            subjectEpisode.ChangeSelectedEpisodeFromProgress(Convert.ToInt32(item.Sort) - subjectVM.EpsOffset);
        }

        private async void ProgressUpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                subjectProgressRing.IsActive = true;
                await subjectVM.UpdateMultipleProgress();
            }
            finally
            {
                subjectProgressRing.IsActive = false;
                this.IsEnabled = true;
            }
        }
    }
}