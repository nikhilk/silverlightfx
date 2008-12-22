// ProceduralAnimationController.cs
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
using System.Windows;
using System.Windows.Media.Animation;

namespace System.Windows.Media.Glitz {

    /// <summary>
    /// This class manages the set of active animations and is responsible for
    /// scheduling them, and executing them from start to finish.
    /// </summary>
    internal sealed class ProceduralAnimationController {

        private static readonly DependencyProperty AnimationControllerProperty =
            DependencyProperty.RegisterAttached("AnimationController", typeof(ProceduralAnimationController), typeof(ProceduralAnimationController), null);

        private List<ProceduralAnimation> _activeAnimations;
        private Storyboard _sb;
        private FrameworkElement _fe;

        /// <summary>
        /// Initializes an instance of an AnimationController applying
        /// to the specified FrameworkElement.
        /// </summary>
        /// <param name="fe">The associated FrameworkElement.</param>
        public ProceduralAnimationController(FrameworkElement fe) {
            _sb = new Storyboard();
            _sb.Duration = TimeSpan.FromMilliseconds(0);
            _sb.Completed += OnStoryboardCompleted;

            _fe = fe;
            _fe.Resources.Add("__animationController", _sb);
        }

        public FrameworkElement Element {
            get {
                return _fe;
            }
        }

        private static ProceduralAnimationController GetAnimationController(FrameworkElement fe) {
            ProceduralAnimationController controller = (ProceduralAnimationController)fe.GetValue(AnimationControllerProperty);
            if (controller == null) {
                controller = new ProceduralAnimationController(fe);
                fe.SetValue(AnimationControllerProperty, controller);
            }

            return controller;
        }

        private void OnStoryboardCompleted(object sender, EventArgs e) {
            _sb.Stop();

            if (_activeAnimations.Count == 0) {
                return;
            }

            DateTime timeStamp = DateTime.Now;

            List<ProceduralAnimation> currentAnimations = _activeAnimations;
            List<ProceduralAnimation> newAnimations = new List<ProceduralAnimation>();

            _activeAnimations = null;
            foreach (ProceduralAnimation animation in currentAnimations) {
                bool completed = animation.OnProgress(timeStamp);
                if (completed) {
                    animation.OnStop(/* completed */ true, ProceduralAnimationStopState.Complete);
                }
                else {
                    newAnimations.Add(animation);
                }
            }

            if (newAnimations.Count != 0) {
                _activeAnimations = newAnimations;
                _sb.Begin();
            }
        }

        private void PlayCore(ProceduralAnimation animation) {
            if (_activeAnimations == null) {
                _activeAnimations = new List<ProceduralAnimation>();
            }
            _activeAnimations.Add(animation);

            animation.OnPlay(/* reversed */ false);
            _sb.Begin();
        }

        public static ProceduralAnimationController Play(ProceduralAnimation animation, FrameworkElement associatedElement) {
            ProceduralAnimationController controller = GetAnimationController(associatedElement);
            controller.PlayCore(animation);

            return controller;
        }

        private void StopCore(ProceduralAnimation animation, ProceduralAnimationStopState stopState) {
            animation.OnStop(/* completed */ false, stopState);
            _activeAnimations.Remove(animation);

            if (_activeAnimations.Count == 0) {
                _sb.Stop();
            }
        }

        public static void Stop(ProceduralAnimation animation, ProceduralAnimationStopState stopState) {
            animation.Controller.StopCore(animation, stopState);
        }
    }
}
