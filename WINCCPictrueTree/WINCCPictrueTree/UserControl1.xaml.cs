//#define Debug
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WINCCPictrueTree.Models;

namespace WINCCPictrueTree
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class CatalogueControl : UserControl
    {
        #region 依赖属性
        public bool Edit
        {
            get { return (bool)GetValue(EditProperty); }
            set 
            {
                var item = ToolBox.Items[0] as MenuItem;
                if (value)
                {
                    item.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xB4, 0xDB, 0xE6));
                }
                else
                {
                    item.Background = null;
                }
                SetValue(EditProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Edit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditProperty =
            DependencyProperty.Register("Edit", typeof(bool), typeof(CatalogueControl), null);

        public bool Move
        {
            get { return (bool)GetValue(MoveProperty); }
            set 
            {
                var item = ToolBox.Items[1] as MenuItem;
                if (value)
                {
                    item.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xB4, 0xDB, 0xE6));
                }
                else
                {
                    item.Background = null;
                }
                SetValue(MoveProperty, value); 
            }
        }

        // Using a DependencyProperty as the backing store for Move.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MoveProperty =
            DependencyProperty.Register("Move", typeof(bool), typeof(CatalogueControl), null);

        public string SelectedPictrue
        {
            get { return (string)GetValue(SelectedPictrueProperty); }
            set { SetValue(SelectedPictrueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedPictrue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedPictrueProperty =
            DependencyProperty.Register("SelectedPictrue", typeof(string), typeof(CatalogueControl), null);
        #endregion

        #region 字段
        private ObservableCollection<CatalogueItemClass> CatalogueItems;
        #endregion

        #region 注册路由事件
        //1、声明并注册路由事件，使用冒泡策略
        public static readonly RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent("Selected",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CatalogueControl));

        //2、通过.NET事件包装路由事件
        public event RoutedEventHandler Selected
        {
            add
            {
                AddHandler(SelectedEvent, value);
            }
            remove
            {
                RemoveHandler(SelectedEvent, value);
            }
        }
        #endregion

        public CatalogueControl()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            CatalogueItems = CatalogueItemClass.Init();
            listBox.ItemsSource = CatalogueItems;
        }

        #region 目录事件
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var item = GetItem(sender);
            CatalogueItems.Insert(CatalogueItems.IndexOf(item) + 1, new CatalogueItemClass() { Name = "未命名", PictrueName = "未命名", Children = new ObservableCollection<CatalogueItemClass>() });
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var item = GetItem(sender);
            var result = MessageBox.Show($"请确认是否删除{item.Name}目录!", "注意", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                CatalogueItems.Remove(item);
            }
        }
        #endregion

        #region 拖动事件
        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Move)
            {
                return;
            }
            var dragSource = sender as ListBox;
            var selected = GetDataFromListBox(dragSource, e.GetPosition(dragSource));
            if (selected != null)
            {
                var dataObj = new DataObject(selected);
                var item = GetItem(sender);
                dataObj.SetData("Item", item);
                DragDrop.DoDragDrop(dragSource, dataObj, DragDropEffects.Move);
            }
        }

        private void Expander_Drop(object sender, DragEventArgs e)
        {
            var distExp = sender as Expander;
            var dist = distExp.DataContext as CatalogueItemClass;
            var source = e.Data.GetData(typeof(CatalogueItemClass)) as CatalogueItemClass;
            var node = e.Data.GetData("Item") as CatalogueItemClass;
            if (dist == node)
            {
                return;
            }
            node.Children.Remove(source);

            dist.Children.Add(source);
        }
        #endregion

        #region 菜单栏事件
        private void Move_Click(object sender, RoutedEventArgs e)
        {
            Move = !Move;
            Edit = false;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            CatalogueItemClass.Save(CatalogueItems);
            Edit = false;
            Move = false;
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Edit = !Edit;
            Move = false;
        }
        #endregion
        private object GetDataFromListBox(ListBox source, Point point)
        {
            UIElement element = source.InputHitTest(point) as UIElement;
            if (element != null)
            {
                object data = DependencyProperty.UnsetValue;
                while (data == DependencyProperty.UnsetValue)
                {
                    data = source.ItemContainerGenerator.ItemFromContainer(element);

                    if (data == DependencyProperty.UnsetValue)
                    {
                        element = VisualTreeHelper.GetParent(element) as UIElement;
                    }

                    if (element == source)
                    {
                        return null;
                    }
                }

                if (data != DependencyProperty.UnsetValue)
                {
                    return data;
                }
            }

            return null;
        }

        private CatalogueItemClass GetItem(object sender)
        {
            CatalogueItemClass item = null;
            if (sender is Button)
            {
                var btn = sender as Button;
                var panel = btn.Parent as StackPanel;
                var grid = panel.Parent as Grid;
                var header = (grid.Children[0] as Label).Content.ToString();
                item = CatalogueItems.First((c) => c.Name == header);

            }
            else if (sender is ListBox)
            {
                var list = sender as ListBox;
                var expander = list.Parent as Expander;
                item = expander.DataContext as CatalogueItemClass;
            }
            return item;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Move || Edit)
                return;
            var selected = ((ListBox)sender).SelectedItem as CatalogueItemClass;
            if (selected == null)
                return;
#if Debug
            MessageBox.Show(selected.PictrueName);
#endif
            SelectedPictrue = selected.PictrueName;
            RoutedEventArgs arg = new RoutedEventArgs();
            arg.RoutedEvent = SelectedEvent;
            RaiseEvent(arg);
        }
    }
}
