// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="BindStuff.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   A test application for binding.
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

namespace BindingTest
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    // This material is primarily from the MsPL licensed file Properties.cs
    // as described here:
    // <copyright file="Properties.cs" company="Microsoft Corporation">
    // Copyright (c) 2008 All Right Reserved
    // </copyright>
    // <author>Michael S. Scherotter</author>
    // <email>mischero@microsoft.com</email>
    // <date>2009-03-21</date>

    /// <summary>
    /// This is the main support class for the binding test. This particular approach is no longer
    /// necessary.
    /// </summary>
    public class BindStuff
    {
        /// <summary>
        /// The type of this binding class.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess",
            Justification = "Reviewed. Suppression is OK here.")]
        private static readonly Type ClassType = typeof(BindStuff);

        /// <summary>
        /// The ItemsSource property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.RegisterAttached(
                "ItemsSource",
                typeof(IEnumerable),
                ClassType,
                new PropertyMetadata(OnItemsSourceChanged));

        /// <summary>
        /// The ItemTemplate property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.RegisterAttached(
                "ItemTemplate", typeof(DataTemplate), ClassType, new PropertyMetadata(null));

        /// <summary>
        /// Sets the ItemsSourceProperty on the passed in element.
        /// </summary>
        /// <param name="element">
        /// The element on which to set the ItemsSourceProperty.
        /// </param>
        /// <param name="value">
        /// The IEnumerable value to set.
        /// </param>
        public static void SetItemsSource(DependencyObject element, IEnumerable value)
        {
            element.SetValue(ItemsSourceProperty, value);
        }

        /// <summary>
        /// Returns the ItemsSourceProperty associated with the element.
        /// </summary>
        /// <param name="element">
        /// The element from which to extract the ItemsSourceProperty.
        /// </param>
        /// <returns>
        /// An IEnumerable list of items.
        /// </returns>
        public static IEnumerable GetItemsSource(DependencyObject element)
        {
            return (IEnumerable)element.GetValue(ItemsSourceProperty);
        }

        /// <summary>
        /// Sets an element ItemTemplateProperty.
        /// </summary>
        /// <param name="element">
        /// The element whose ItemTemplateProperty is to be set.
        /// </param>
        /// <param name="value">
        /// The DataTemplate value to be set.
        /// </param>
        public static void SetItemTemplate(DependencyObject element, DataTemplate value)
        {
            element.SetValue(ItemTemplateProperty, value);
        }

        /// <summary>
        /// Return the DataTemplate associated with the DependencyObject.
        /// </summary>
        /// <param name="element">
        /// The element with the DataTemplate.
        /// </param>
        /// <returns>
        /// The DataTemplate from the element's ItemTemplateProperty.
        /// </returns>
        public static DataTemplate GetItemTemplate(DependencyObject element)
        {
            return (DataTemplate)element.GetValue(ItemTemplateProperty);
        }

        /// <summary>
        /// Update the items that have changed.
        /// </summary>
        /// <param name="depObject">
        /// The panel object containing the display items.
        /// </param>
        /// <param name="args">
        /// The args component contains a new value which is an IEnumerable list of changed items.
        /// </param>
        private static void OnItemsSourceChanged(DependencyObject depObject, DependencyPropertyChangedEventArgs args)
        {
            var panel = depObject as Panel;
            if (panel == null)
            {
                return;
            }

            var uiElement = depObject as UIElement;
            var itemsSrc = args.NewValue as IEnumerable;
            var notifyCollectionChanged = itemsSrc as INotifyCollectionChanged;
            AddItems(panel, uiElement, itemsSrc);
            if (notifyCollectionChanged != null)
            {
                notifyCollectionChanged.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
                    {
                        AddItems(panel, uiElement, e.NewItems);
                        RemoveItems(panel, e.OldItems);
                    };
            }
        }

        /// <summary>
        /// Remove items from the display panel. The items are the data contexts to be removed.
        /// </summary>
        /// <param name="panel">
        /// The panel containing the items.
        /// </param>
        /// <param name="items">
        /// The items to be removed.
        /// </param>
        private static void RemoveItems(Panel panel, IEnumerable items)
        {
            if (items == null)
            {
                return;
            }

            foreach (object item in items)
            {
                if (item == null)
                {
                    continue;
                }

                object itemOne = item;
                FrameworkElement found =
                    (from child in panel.Children.Cast<FrameworkElement>()
                     where child.DataContext == itemOne
                     select child).FirstOrDefault();
                if (found != null)
                {
                    panel.Children.Remove(found);
                }
            }
        }

        /// <summary>
        /// Set the DataContext using the items for the DependencyObject uiElement.
        /// </summary>
        /// <param name="panel">
        /// The panel which hosts the display elements.
        /// </param>
        /// <param name="uiElement">
        /// The User Interface element that contains the list of display elements.
        /// </param>
        /// <param name="items">
        /// The list of IEnumerable DataContexts.
        /// </param>
        private static void AddItems(Panel panel, DependencyObject uiElement, IEnumerable items)
        {
            if (items == null)
            {
                return;
            }

            DataTemplate template = GetItemTemplate(uiElement);
            if (template == null)
            {
                return;
            }

            foreach (object item in items)
            {
                var newShape = template.LoadContent() as FrameworkElement;
                if (newShape == null)
                {
                    return;
                }

                newShape.DataContext = item;
                panel.Children.Add(newShape);
            }
        }
    }
}