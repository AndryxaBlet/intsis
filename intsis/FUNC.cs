using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace intsis
{
    static class FUNC
    {
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;

            if (parentObject is T parent)
                return parent;
            else
                return FindParent<T>(parentObject);
        }
        public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                // Если нашли элемент нужного типа, возвращаем его
                if (child is T foundChild)
                    return foundChild;

                // Рекурсивно ищем среди дочерних элементов
                var result = FindChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
