// ContentView.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Glitz;

using NativeGrid = System.Windows.Controls.Grid;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A derived ContentControl that supports transitions to animate from
    /// initial content to another content.
    /// </summary>
    [TemplatePart(Name = "FrontPresenter", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "BackPresenter", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "AdornerLayer", Type = typeof(Canvas))]
    [ContentProperty("Content")]
    public class ContentView : Control, IAdornableControl {

        /// <summary>
        /// Represents the Content property of a ContentControl.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(ContentView),
                                        new PropertyMetadata(OnContentPropertyChanged));

        /// <summary>
        /// Represents the ContentTemplate property of a ContentControl.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(ContentView), null);

        /// <summary>
        /// Represents the ContentTransition property of a ContentControl.
        /// </summary>
        public static readonly DependencyProperty ContentTransitionProperty =
            DependencyProperty.Register("ContentTransition", typeof(Transition), typeof(ContentView),
                                        new PropertyMetadata(OnContentTransitionPropertyChanged));

        private NativeGrid _contentGrid;
        private Queue<ContentPresenter> _presenterQueue;
        private Canvas _adornerLayer;

        /// <summary>
        /// Initializes an instance of a ContentControl.
        /// </summary>
        public ContentView() {
            DefaultStyleKey = typeof(ContentView);
        }

        /// <summary>
        /// Gets or sets the content to be displayed within the control.
        /// </summary>
        public object Content {
            get {
                return GetValue(ContentProperty);
            }
            set {
                SetValue(ContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets he template used to display the content within the control.
        /// </summary>
        public DataTemplate ContentTemplate {
            get {
                return (DataTemplate)GetValue(ContentTemplateProperty);
            }
            set {
                SetValue(ContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets he template used to display the content within the control.
        /// </summary>
        public Transition ContentTransition {
            get {
                return (Transition)GetValue(ContentTransitionProperty);
            }
            set {
                SetValue(ContentTransitionProperty, value);
            }
        }

        private void AddContent(object content) {
            ContentPresenter presenter = _presenterQueue.Dequeue();

            DataTemplate contentTemplate = ContentTemplate;
            if (contentTemplate != null) {
                presenter.ContentTemplate = contentTemplate;
            }
            presenter.Content = content;

            Panel panel = new NativeGrid();
            panel.Children.Add(presenter);

            _contentGrid.Children.Insert(0, panel);

            // Put the presenter back in the queue; essentially we pick the
            // first presenter in the queue, and then queue it back at the
            // end for a subsequent use
            _presenterQueue.Enqueue(presenter);
        }

        /// <internalonly />
        public override void OnApplyTemplate() {
            ContentPresenter frontPresenter = GetTemplateChild("FrontPresenter") as ContentPresenter;
            if (frontPresenter == null) {
                throw new InvalidOperationException("The template of a ContentControl must contain a ContentControl named FrontPresenter");
            }

            ContentPresenter backPresenter = GetTemplateChild("BackPresenter") as ContentPresenter;
            if (backPresenter == null) {
                throw new InvalidOperationException("The template of a ContentControl must contain a ContentControl named BackPresenter");
            }

            if ((frontPresenter.Parent != backPresenter.Parent) || !(frontPresenter.Parent is NativeGrid)) {
                throw new InvalidOperationException("The FrontPresenter and BackPresenter must be in the same Grid container.");
            }

            _adornerLayer = GetTemplateChild("AdornerLayer") as Canvas;

            _contentGrid = (NativeGrid)frontPresenter.Parent;
            _contentGrid.Children.Clear();

            _presenterQueue = new Queue<ContentPresenter>();
            _presenterQueue.Enqueue(frontPresenter);
            _presenterQueue.Enqueue(backPresenter);

            object content = Content;
            if (content != null) {
                AddContent(content);
            }
        }

        /// <internalonly />
        private void OnContentChanged(object oldContent, object newContent) {
            if (_contentGrid == null) {
                return;
            }

            if (newContent == null) {
                Transition contentTransition = ContentTransition;
                if (contentTransition.IsActive) {
                    contentTransition.StopEffect();
                }

                if (_contentGrid.Children.Count != 0) {
                    Panel panel = (Panel)_contentGrid.Children[0];
                    ContentPresenter contentPresenter = (ContentPresenter)panel.Children[0];

                    panel.Children.Clear();
                    _contentGrid.Children.Clear();
                }

                return;
            }
            else if (_contentGrid.Children.Count == 0) {
                AddContent(newContent);
                return;
            }
            else {
                Transition contentTransition = ContentTransition;

                if (contentTransition == null) {
                    Panel panel = (Panel)_contentGrid.Children[0];
                    ContentPresenter contentPresenter = (ContentPresenter)panel.Children[0];

                    contentPresenter.Content = newContent;
                }
                else {
                    if (contentTransition.IsActive) {
                        contentTransition.StopEffect();
                    }

                    AddContent(newContent);

                    contentTransition.Target = _contentGrid;
                    contentTransition.PlayEffect(EffectDirection.Forward);
                }
            }
        }

        private static void OnContentPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            ((ContentView)o).OnContentChanged(e.OldValue, e.NewValue);
        }

        private void OnContentTransitionChanged(Transition oldTransition, Transition newTransition) {
            if (oldTransition != null) {
                oldTransition.Completed -= OnContentTransitionCompleted;
                ((IAttachedObject)oldTransition).Detach();
            }
            if (newTransition != null) {
                ((IAttachedObject)newTransition).Attach(this);
                newTransition.Completed += OnContentTransitionCompleted;
            }
        }

        private void OnContentTransitionCompleted(object sender, EventArgs e) {
            if ((_contentGrid != null) && (_contentGrid.Children.Count == 2)) {
                Panel panel = (Panel)_contentGrid.Children[1];
                ContentPresenter presenter = (ContentPresenter)panel.Children[0];

                panel.Children.Clear();
                _contentGrid.Children.RemoveAt(1);

                presenter.Content = null;
            }
        }

        private static void OnContentTransitionPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            ((ContentView)o).OnContentTransitionChanged((Transition)e.OldValue, (Transition)e.NewValue);
        }

        #region IAdornableControl Members
        bool IAdornableControl.CanAdorn {
            get {
                ApplyTemplate();
                return (_adornerLayer != null);
            }
        }

        bool IAdornableControl.HasAdornments {
            get {
                ApplyTemplate();
                return (_adornerLayer != null) &&
                       (_adornerLayer.Children.Count != 0);
            }
        }

        void IAdornableControl.AddAdornment(UIElement element) {
            if (element == null) {
                throw new ArgumentNullException("element");
            }

            ApplyTemplate();
            if (_adornerLayer == null) {
                throw new InvalidOperationException("The control cannot be adorned.");
            }

            _adornerLayer.Children.Add(element);
            _adornerLayer.Visibility = Visibility.Visible;
        }

        void IAdornableControl.RemoveAdornment(UIElement element) {
            if (element == null) {
                throw new ArgumentNullException("element");
            }

            ApplyTemplate();
            if (_adornerLayer == null) {
                throw new InvalidOperationException("The control cannot be adorned.");
            }

            _adornerLayer.Children.Remove(element);
            if (_adornerLayer.Children.Count == 0) {
                _adornerLayer.Visibility = Visibility.Collapsed;
            }
        }
        #endregion
    }
}
