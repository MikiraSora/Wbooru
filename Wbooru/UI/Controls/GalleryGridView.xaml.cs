﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wbooru.Models.Gallery;
using Wbooru.Settings;

namespace Wbooru.UI.Controls
{
    /// <summary>
    /// GalleryGridView.xaml 的交互逻辑
    /// </summary>
    public partial class GalleryGridView : UserControl
    {
        public event Action<GalleryGridView> RequestMoreItems;
        public event Action<GalleryItem> ClickItemEvent;

        public uint GridItemWidth
        {
            get { return (uint)GetValue(GridItemWidthProperty); }
            set { SetValue(GridItemWidthProperty, value); }
        }

        public static readonly DependencyProperty GridItemWidthProperty =
            DependencyProperty.Register("GridItemWidth", typeof(uint), typeof(GalleryGridView), new PropertyMetadata((uint)150));

        public uint GridItemMarginWidth
        {
            get { return (uint)GetValue(GridItemMarginWidthProperty); }
            set { SetValue(GridItemMarginWidthProperty, value); }
        }

        public static readonly DependencyProperty GridItemMarginWidthProperty =
            DependencyProperty.Register("GridItemMarginWidth", typeof(uint), typeof(GalleryGridView), new PropertyMetadata((uint)10));

        public GalleryItemUIElementWrapper GalleryItemsSource
        {
            get { return (GalleryItemUIElementWrapper)GetValue(GalleryItemsSourceProperty); }
            set { SetValue(GalleryItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty GalleryItemsSourceProperty =
            DependencyProperty.Register("GalleryItemsSource", typeof(GalleryItemUIElementWrapper), typeof(GalleryGridView), new PropertyMetadata(null));

        public GalleryGridView()
        {
            InitializeComponent();

            DataContext = this;

            UpdateSettingForScroller();
        }

        public void UpdateSettingForScroller()
        {
            var scrollbar_visiable = Container.Default.GetExportedValue<SettingManager>().LoadSetting<GlobalSetting>().GalleryListScrollBarVisiable;

            if (scrollbar_visiable)
            {
                ListScrollViewer.MouseMove -= ListScrollViewer_PreviewMouseMove;
                ListScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            }
            else
            {
                ListScrollViewer.MouseMove += ListScrollViewer_PreviewMouseMove;
                ListScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
        }

        private void ListScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var height = (sender as ScrollViewer).ScrollableHeight;
            var at_end = e.VerticalOffset
                                    >= height;

            if (at_end)
                RequestMoreItems?.Invoke(this);
        }

        private void ListScrollViewer_MouseLeave(object sender, MouseEventArgs e)
        {
            drag_action_state = DragActionState.Idle;
            Log.Debug($"{drag_action_state}", "ListScrollViewer_MouseLeave");
        }

        private void ListScrollViewer_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;

            if (drag_action_state == DragActionState.ReadyDrag)
            {
                drag_action_state = DragActionState.Dragging;
                Log.Debug($"{drag_action_state}", "ListScrollViewer_PreviewMouseMove");
            }

            if (DragActionState.Dragging==drag_action_state)
            {
                var y = e.GetPosition(this).Y;
                var offset = prev_y - y;
                prev_y = y;

                ListScrollViewer.ScrollToVerticalOffset(ListScrollViewer.VerticalOffset + offset);
                Log.Debug($"Moving ... {drag_action_state}", "ListScrollViewer_PreviewMouseMove");
            }
        }

        DragActionState drag_action_state = DragActionState.Idle;

        enum DragActionState
        {
            Idle,
            ReadyDrag,
            Dragging
        }

        double prev_y = 0;

        private void ListScrollViewer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            drag_action_state = DragActionState.Idle;
            Log.Debug($"{drag_action_state}", "ListScrollViewer_MouseUp");
        }

        private void ListScrollViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            prev_y = e.GetPosition(this).Y;
            drag_action_state = DragActionState.ReadyDrag;

            Log.Debug($"{drag_action_state}", "ListScrollViewer_PreviewMouseLeftButtonDown");
        }

        private void StackPanel_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (drag_action_state != DragActionState.ReadyDrag || !(((FrameworkElement)sender).DataContext is GalleryItem item))
                return;

            Log.Debug($"click item {item.ID}");
            ClickItemEvent?.Invoke(item);
        }
    }
}
