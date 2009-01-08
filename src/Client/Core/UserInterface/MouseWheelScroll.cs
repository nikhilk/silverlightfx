// MouseWheelScroll.cs
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
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A behavior that can be associated with an element that supports
    /// vertical scrolling.
    /// </summary>
    public sealed class MouseWheelScroll : Behavior<FrameworkElement> {

        private IScrollProvider _scrollProvider;
        private bool _activated;

        private int _scrollSize;

        /// <summary>
        /// Initializes an instance of a MouseWheelScroll.
        /// </summary>
        public MouseWheelScroll() {
            _scrollSize = 1;
        }

        /// <summary>
        /// Gets or sets the number of scroll units the control should be
        /// scrolled per mouse wheel turn.
        /// </summary>
        public int ScrollSize {
            get {
                return _scrollSize;
            }
            set {
                if (value < 1) {
                    throw new ArgumentOutOfRangeException("value");
                }
                _scrollSize = value;
            }
        }

        private ScrollViewer GetChildScrollViewer(DependencyObject parentObject) {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parentObject); i++) {
                DependencyObject child = VisualTreeHelper.GetChild(parentObject, i);

                ScrollViewer scrollViewer = child as ScrollViewer;
                if (scrollViewer == null) {
                    scrollViewer = GetChildScrollViewer(child);
                }
                if (scrollViewer != null) {
                    return scrollViewer;
                }
            }

            return null;
        }

        /// <internalonly />
        protected override void OnAttach() {
            base.OnAttach();

            if (HtmlPage.IsEnabled == false) {
                return;
            }

            AssociatedObject.MouseEnter += OnMouseEnter;
            AssociatedObject.MouseLeave += OnMouseLeave;
            AssociatedObject.MouseMove += OnMouseMove;
        }

        private void OnBrowserDOMMouseScroll(object sender, HtmlEventArgs e) {
            // Handles DOMMouseScroll on Firefox

            double delta = (double)e.EventObject.GetProperty("detail") / -3;
            UpdateScrollOffset(delta);

            e.PreventDefault();
        }

        private void OnBrowserMouseWheel(object sender, HtmlEventArgs e) {
            // Handles onmousewheel on non-Firefox browsers

            double delta = (double)e.EventObject.GetProperty("wheelDelta") / 120;
            UpdateScrollOffset(delta);

            e.EventObject.SetProperty("returnValue", false);
        }

        /// <internalonly />
        protected override void OnDetach() {
            UpdateDOMEventHandlers(/* activate */ false);

            AssociatedObject.MouseEnter -= OnMouseEnter;
            AssociatedObject.MouseLeave -= OnMouseLeave;
            AssociatedObject.MouseMove -= OnMouseMove;

            base.OnDetach();
        }

        private void OnMouseEnter(object sender, MouseEventArgs e) {
            UpdateDOMEventHandlers(/* activate */ true);
        }

        private void OnMouseLeave(object sender, MouseEventArgs e) {
            UpdateDOMEventHandlers(/* activate */ false);
        }

        private void OnMouseMove(object sender, MouseEventArgs e) {
            UpdateDOMEventHandlers(/* activate */ true);
        }

        private void UpdateDOMEventHandlers(bool activate) {
            if (_activated != activate) {
                _activated = activate;
                if (activate) {
                    IScrollProvider scrollProvider = null;

                    AutomationPeer automationPeer = FrameworkElementAutomationPeer.CreatePeerForElement(AssociatedObject);
                    if (automationPeer != null) {
                        scrollProvider = (IScrollProvider)automationPeer.GetPattern(PatternInterface.Scroll);
                    }

                    if (scrollProvider == null) {
                        ScrollViewer scrollViewer = GetChildScrollViewer(AssociatedObject);
                        if (scrollViewer != null) {
                            automationPeer = FrameworkElementAutomationPeer.CreatePeerForElement(scrollViewer);
                            if (automationPeer != null) {
                                scrollProvider = (IScrollProvider)automationPeer.GetPattern(PatternInterface.Scroll);
                            }
                        }
                    }

                    if ((scrollProvider != null) && scrollProvider.VerticallyScrollable) {
                        _scrollProvider = scrollProvider;

                        HtmlPage.Document.AttachEvent("DOMMouseScroll", OnBrowserDOMMouseScroll);
                        HtmlPage.Document.AttachEvent("onmousewheel", OnBrowserMouseWheel);
                    }
                }
                else {
                    if (_scrollProvider != null) {
                        _scrollProvider = null;

                        HtmlPage.Document.DetachEvent("DOMMouseScroll", OnBrowserDOMMouseScroll);
                        HtmlPage.Document.DetachEvent("onmousewheel", OnBrowserMouseWheel);
                    }
                }
            }
        }

        private void UpdateScrollOffset(double delta) {
            ScrollAmount verticalScroll = delta < 0.0 ? ScrollAmount.SmallIncrement : ScrollAmount.SmallDecrement;
            for (int i = 0; i < _scrollSize; i++) {
                _scrollProvider.Scroll(ScrollAmount.NoAmount, verticalScroll);
            }
        }
    }
}
