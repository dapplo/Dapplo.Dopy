using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace Dapplo.Dopy.Utils
{

    /// <summary>
    /// Helps to autoscroll a datagrid
    /// </summary>
    public static class DataGridBehavior
    {
        /// <summary>
        /// The AutoscrollProperty
        /// </summary>
        public static readonly DependencyProperty AutoscrollProperty = DependencyProperty.RegisterAttached(
            "Autoscroll", typeof(bool), typeof(DataGridBehavior), new PropertyMetadata(default(bool), AutoscrollChangedCallback));

        private static readonly Dictionary<DataGrid, NotifyCollectionChangedEventHandler> Handlers = new Dictionary<DataGrid, NotifyCollectionChangedEventHandler>();

        private static void AutoscrollChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var dataGrid = dependencyObject as DataGrid;
            if (dataGrid == null)
            {
                throw new InvalidOperationException("Dependency object is not DataGrid.");
            }

            if ((bool)args.NewValue)
            {
                Subscribe(dataGrid);
                dataGrid.Unloaded += DataGridOnUnloaded;
                dataGrid.Loaded += DataGridOnLoaded;
            }
            else
            {
                Unsubscribe(dataGrid);
                dataGrid.Unloaded -= DataGridOnUnloaded;
                dataGrid.Loaded -= DataGridOnLoaded;
            }
        }

        private static void Subscribe(DataGrid dataGrid)
        {
            var handler = new NotifyCollectionChangedEventHandler((sender, eventArgs) => ScrollToEnd(dataGrid));
            Handlers[dataGrid] =  handler;
            ((INotifyCollectionChanged)dataGrid.Items).CollectionChanged += handler;
            ScrollToEnd(dataGrid);
        }

        private static void Unsubscribe(DataGrid dataGrid)
        {
            NotifyCollectionChangedEventHandler handler;
            Handlers.TryGetValue(dataGrid, out handler);
            if (handler == null)
            {
                return;
            }
            ((INotifyCollectionChanged)dataGrid.Items).CollectionChanged -= handler;
            Handlers.Remove(dataGrid);
        }

        private static void DataGridOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var dataGrid = (DataGrid)sender;
            if (GetAutoscroll(dataGrid))
            {
                Subscribe(dataGrid);
            }
        }

        private static void DataGridOnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var dataGrid = (DataGrid)sender;
            if (GetAutoscroll(dataGrid))
            {
                Unsubscribe(dataGrid);
            }
        }

        private static void ScrollToEnd(DataGrid datagrid)
        {
            if (datagrid.Items.Count == 0)
            {
                return;
            }
            datagrid.ScrollIntoView(datagrid.Items[datagrid.Items.Count - 1]);
        }

        /// <summary>
        /// Specifies the Autoscroll value
        /// </summary>
        /// <param name="element">DependencyObject</param>
        /// <param name="value">bool</param>
        public static void SetAutoscroll(DependencyObject element, bool value)
        {
            element.SetValue(AutoscrollProperty, value);
        }

        /// <summary>
        /// Retrieves the Autoscroll value
        /// </summary>
        /// <param name="element">DependencyObject</param>
        /// <returns>bool</returns>
        public static bool GetAutoscroll(DependencyObject element)
        {
            return (bool)element.GetValue(AutoscrollProperty);
        }
    }
}
