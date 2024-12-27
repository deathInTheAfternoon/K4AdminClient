using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using AdminClient.ViewModels;
using MaterialDesignThemes.Wpf;

namespace AdminClient.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NotNullToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }
            return false;
        }
    }

    // Map TreeView node type to Material Design icon
    public class NodeTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TreeNodeType nodeType)
            {
                return nodeType switch
                {
                    TreeNodeType.Root => PackIconKind.ViewList,
                    TreeNodeType.Organization => PackIconKind.Domain,
                    TreeNodeType.Programs => PackIconKind.FolderMultiple,
                    TreeNodeType.Program => PackIconKind.Folder,
                    TreeNodeType.OperatingUnits => PackIconKind.FolderNetwork,
                    TreeNodeType.OperatingUnit => PackIconKind.FolderAccount,
                    TreeNodeType.BundleDefinitions => PackIconKind.FolderMultiple,
                    TreeNodeType.BundleDefinition => PackIconKind.Package,
                    TreeNodeType.ActivityDefinitions => PackIconKind.FolderCog,
                    TreeNodeType.ActivityDefinition => PackIconKind.FileDocument,
                    _ => PackIconKind.Circle
                };
            }
            return PackIconKind.Circle;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Map TreeView node type to Material Design color
    public class NodeTypeToColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TreeNodeType nodeType)
            {
                return nodeType switch
                {
                    TreeNodeType.Root => new SolidColorBrush(Colors.Gray),
                    TreeNodeType.Organization => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1976D2")), // Material Blue
                    TreeNodeType.Programs => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#388E3C")), // Material Green
                    TreeNodeType.Program => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#43A047")), // Lighter Green
                    TreeNodeType.OperatingUnits => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7B1FA2")), // Material Purple
                    TreeNodeType.OperatingUnit => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8E24AA")), // Lighter Purple
                    TreeNodeType.BundleDefinitions => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F57C00")), // Material Orange
                    TreeNodeType.BundleDefinition => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FB8C00")), // Lighter Orange
                    TreeNodeType.ActivityDefinitions => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C2185B")), // Material Pink
                    TreeNodeType.ActivityDefinition => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D81B60")), // Lighter Pink
                    _ => new SolidColorBrush(Colors.Gray)
                };
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}