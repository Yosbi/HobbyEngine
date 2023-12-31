﻿using System.Windows;
using System.Windows.Controls;

namespace HobbyEditor.GameProject
{
    /// <summary>
    /// Interaction logic for OpenProjectView.xaml
    /// </summary>
    public partial class OpenProjectView : UserControl
    {
        public OpenProjectView()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                var item = (ListBoxItem)projectsListBox.ItemContainerGenerator
                .ContainerFromIndex(projectsListBox.SelectedIndex);

                item?.Focus();
            };
        }

        private void _openProjectButtonClick(object sender, RoutedEventArgs e)
        {
            _openSelectedProject();
        }

        private void _openSelectedProject()
        {
            var project = OpenProject.Open((ProjectData)projectsListBox.SelectedItem);
            if (project != null)
            {
                var window = Window.GetWindow(this);
                window.DataContext = project;
                window.DialogResult = true;
                window.Close();
                
            }
            else
            {
                MessageBox.Show("Failed to open project");
            }
        }
    }
}
