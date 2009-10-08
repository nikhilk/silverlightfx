// LinkLabel.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A very simple label control that displays embedded links as
    /// navigatable hyperlinks.
    /// </summary>
    public class LinkLabel : Control {

        /// <summary>
        /// Represents the HyperlinkStyle property.
        /// </summary>
        public static readonly DependencyProperty HyperlinkStyleProperty =
            DependencyProperty.Register("HyperlinkStyle", typeof(Style), typeof(LinkLabel),
                                        new PropertyMetadata(OnContentPropertyChanged));

        /// <summary>
        /// Represents the TargetName property.
        /// </summary>
        public static readonly DependencyProperty TargetNameProperty =
            DependencyProperty.Register("TargetName", typeof(string), typeof(LinkLabel),
                                        new PropertyMetadata(OnContentPropertyChanged));

        /// <summary>
        /// Represents the Text property.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(LinkLabel),
                                        new PropertyMetadata(OnContentPropertyChanged));

        /// <summary>
        /// Represents the TextStyle property.
        /// </summary>
        public static readonly DependencyProperty TextStyleProperty =
            DependencyProperty.Register("TextStyle", typeof(Style), typeof(LinkLabel),
                                        new PropertyMetadata(OnContentPropertyChanged));

        private WrapPanel _wrapPanel;

        /// <summary>
        /// Initializes an instance of a LinkLabel.
        /// </summary>
        public LinkLabel() {
            DefaultStyleKey = typeof(LinkLabel);
        }

        /// <summary>
        /// Gets or sets the style applied to HyperlinkButtons contained within the control.
        /// </summary>
        public Style HyperlinkStyle {
            get {
                return (Style)GetValue(HyperlinkStyleProperty);
            }
            set {
                SetValue(HyperlinkStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the Target to navigate.
        /// </summary>
        public string TargetName {
            get {
                return (string)GetValue(TargetNameProperty);
            }
            set {
                SetValue(TargetNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text of the Label.
        /// </summary>
        public string Text {
            get {
                return (string)GetValue(TextProperty);
            }
            set {
                SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Style to apply to TextBlocks contained within the control.
        /// </summary>
        public Style TextStyle {
            get {
                return (Style)GetValue(TextStyleProperty);
            }
            set {
                SetValue(TextStyleProperty, value);
            }
        }

        /// <internalonly />
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            _wrapPanel = (WrapPanel)GetTemplateChild("rootPanel");
            UpdateText(Text);
        }

        private static void OnContentPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            ((LinkLabel)o).UpdateText((string)e.NewValue);
        }

        private void UpdateText(string text) {
            if (_wrapPanel == null) {
                return;
            }

            _wrapPanel.Children.Clear();

            if (String.IsNullOrEmpty(text)) {
                return;
            }

            Style hyperlinkStyle = HyperlinkStyle;
            Style textStyle = TextStyle;
            string target = TargetName;

            string[] words = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in words) {
                TextBlock textBlock = new TextBlock() {
                    Style = textStyle
                };

                if (s.StartsWith("http://")) {
                    string linkText = s;
                    string linkUrl = s;

                    int dividerIndex = s.IndexOf('|');
                    if (dividerIndex > 0) {
                        linkText = s.Substring(dividerIndex + 1);
                        linkUrl = s.Substring(0, dividerIndex);
                    }

                    HyperlinkButton hyperlink = new HyperlinkButton() {
                        Content = linkText,
                        NavigateUri = new Uri(linkUrl, UriKind.Absolute),
                        TargetName = target,
                        Style = hyperlinkStyle
                    };
                    _wrapPanel.Children.Add(hyperlink);

                    textBlock.Text = " ";
                }
                else {
                    textBlock.Text = s + " ";
                }

                _wrapPanel.Children.Add(textBlock);
            }
        }
    }
}
