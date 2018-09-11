﻿/*
 * Copyright 2018 Capnode AB
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace Algoloop.ViewSupport
{
    public class FilenameEditor : ITypeEditor
    {
        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            Grid panel = new Grid();
            panel.ColumnDefinitions.Add(new ColumnDefinition());
            panel.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = GridLength.Auto
            });

            TextBox textBox = new TextBox();
            textBox.BorderBrush = textBox.Background;
            textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            textBox.IsEnabled = !propertyItem.IsReadOnly;
            panel.Children.Add(textBox);

            Binding binding = new Binding("Value"); //bind to the Value property of the PropertyItem
            binding.Source = propertyItem;
            binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);

            if (!propertyItem.IsReadOnly)
            {
                Button button = new Button();
                button.Content = "   . . .   ";
                button.Tag = propertyItem;
                button.Click += button_Click;
                Grid.SetColumn(button, 1);
                panel.Children.Add(button);
            }

            return panel;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            PropertyItem item = ((Button)sender).Tag as PropertyItem;
            if (null == item)
            {
                return;
            }

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            string path = item.Value?.ToString();
            string currentFolder = Directory.GetCurrentDirectory();
            string folder;
            if (path == null)
            {
                folder = currentFolder;
            }
            else
            {
                string fullPath = Path.GetFullPath(path);
                folder = Path.GetDirectoryName(fullPath);
            }

            dlg.InitialDirectory = folder;
            if ((bool)dlg.ShowDialog())
            {
                item.Value = RelativePath(currentFolder, dlg.FileName);
            }
        }

        public static string RelativePath(string absPath, string relTo)
        {
            string[] absDirs = absPath.Split('\\');
            string[] relDirs = relTo.Split('\\');
            // Get the shortest of the two paths 
            int len = absDirs.Length < relDirs.Length ? absDirs.Length : relDirs.Length;
            // Use to determine where in the loop we exited 
            int lastCommonRoot = -1; int index;
            // Find common root 
            for (index = 0; index < len; index++)
            {
                if (absDirs[index] == relDirs[index])
                    lastCommonRoot = index;
                else break;
            }
            // If we didn't find a common prefix then throw 
            if (lastCommonRoot == -1)
            {
                // Paths do not have a common base
                return absPath;
            }
            // Build up the relative path 
            StringBuilder relativePath = new StringBuilder();
            // Add on the .. 
            for (index = lastCommonRoot + 1; index < absDirs.Length; index++)
            {
                if (absDirs[index].Length > 0) relativePath.Append("..\\");
            }
            // Add on the folders 
            for (index = lastCommonRoot + 1; index < relDirs.Length - 1; index++)
            {
                relativePath.Append(relDirs[index] + "\\");
            }
            relativePath.Append(relDirs[relDirs.Length - 1]);
            return relativePath.ToString();
        }
    }
}