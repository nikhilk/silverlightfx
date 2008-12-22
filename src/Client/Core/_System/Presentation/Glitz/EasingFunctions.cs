// EasingFunctions.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;

namespace System.Windows.Media.Glitz {

    internal static class EasingFunctions {

        public static double EaseBackIn(double t) {
            double overshoot = 1.70158;
            return t * t * ((overshoot + 1) * t - overshoot);
        }

        public static double EaseBackInOut(double t) {
            double overshoot = 1.70158 * 1.525;

            t = t * 2;
            if (t < 1) {
                return (t * t * ((overshoot + 1) * t - overshoot)) / 2;
            }
            t = t - 2;
            return (t * t * ((overshoot + 1) * t + overshoot)) / 2 + 1;
        }

        public static double EaseBackOut(double t) {
            double overshoot = 1.70158;

            t = t - 1;
            return t * t * ((overshoot + 1) * t + overshoot) + 1;
        }

        public static double EaseBounceIn(double t) {
            return 1 - EaseBounceOut(1 - t);
        }

        public static double EaseBounceInOut(double t) {
            if (t < 0.5) {
                return EaseBounceIn(t * 2) * .5;
            }
            return EaseBounceOut(t * 2 - 1) * .5 + .5;
        }

        public static double EaseBounceOut(double t) {
            if (t < 1 / 2.75) {
                return 7.5625 * t * t;
            }
            if (t < 2 / 2.75) {
                t -= 1.5 / 2.75;
                return 7.5625 * t * t + .75;
            }
            if (t < 2.5 / 2.75) {
                t -= 2.25 / 2.75;
                return 7.5625 * t * t + .9375;
            }
            t -= 2.625 / 2.75;
            return 7.5625 * t * t + .984375;
        }

        public static double EaseElasticIn(double t) {
            if (t == 0) {
                return 0;
            }
            if (t == 1) {
                return 1;
            }

            double period = .3;
            double s = period / 4;

            t = t - 1;
            return -Math.Pow(2, 10 * t) * Math.Sin((t - s) * 2 * Math.PI / period);
        }

        public static double EaseElasticInOut(double t) {
            if (t == 0) {
                return 0;
            }
            if (t == 1) {
                return 1;
            }

            t = t * 2;

            double period = .3 * 1.5;
            double s = period / 4;

            if (t < 1) {
                t = t - 1;
                return -.5 * Math.Pow(2, 10 * t) * Math.Sin((t - s) * 2 * Math.PI / period);
            }
            else {
                t = t - 1;
                return Math.Pow(2, -10 * t) * Math.Sin((t - s) * 2 * Math.PI / period) * .5 + 1;
            }
        }

        public static double EaseElasticOut(double t) {
            if (t == 0) {
                return 0;
            }
            if (t == 1) {
                return 1;
            }

            double period = .3;
            double s = period / 4;

            return Math.Pow(2, -10 * t) * Math.Sin((t - s) * 2 * Math.PI / period) + 1;
        }

        public static double EaseQuadraticIn(double t) {
            return t * t;
        }

        public static double EaseQuadraticInOut(double t) {
            t = t * 2;
            if (t < 1) {
                return t * t / 2;
            }
            return -((--t) * (t - 2) - 1) / 2f;
        }

        public static double EaseQuadraticOut(double t) {
            return -t * (t - 2);
        }
    }
}
