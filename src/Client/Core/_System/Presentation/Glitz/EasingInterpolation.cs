// EasingInterpolation.cs
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

    // Math based on the interpolations sample at http://msdn2.microsoft.com/en-us/library/ms771715.aspx.

    /// <summary>
    /// An interpolation implementation that eases the linear interpolation at the start,
    /// end or both start and end of the animation.
    /// </summary>
    public abstract class EasingInterpolation : TweenInterpolation {

        /// <summary>
        /// A default EasingInterpolation that accelerates the animation at the start
        /// and decelerates the animation at the end.
        /// </summary>
        public static readonly EasingInterpolation Default = new SmoothInterpolation();

        private EasingInterpolationMode _mode;

        /// <summary>
        /// Gets or sets how the easing should be done by the interpolation.
        /// </summary>
        public EasingInterpolationMode Mode {
            get {
                return _mode;
            }
            set {
                if ((value < EasingInterpolationMode.EaseInOut) ||
                    (value > EasingInterpolationMode.EaseOut)) {
                    throw new ArgumentOutOfRangeException("value");
                }
                _mode = value;
            }
        }

        /// <internalonly />
        public sealed override double Interpolate(double t) {
            double tInterpolated = t;

            switch (_mode) {
                case EasingInterpolationMode.EaseIn:
                    tInterpolated = InterpolateIn(t);
                    break;
                case EasingInterpolationMode.EaseOut:
                    tInterpolated = InterpolateOut(t);
                    break;
                case EasingInterpolationMode.EaseInOut:
                    if (t <= 0.5) {
                        tInterpolated = 0.5 * InterpolateIn(t * 2);
                    }
                    else {
                        tInterpolated = 0.5 + 0.5 * InterpolateOut((t - 0.5) * 2);
                    }
                    break;
            }

            return tInterpolated;
        }

        #region Back Interpolation
        /// <summary>
        /// Performs back interpolation when the selected easing mode is set to EaseIn
        /// or during the first half of an EaseInOut interpolation.
        /// </summary>
        /// <param name="t">The current progress value based on linear interpolation.</param>
        /// <param name="amplitude">The relative distance of the backing.</param>
        /// <param name="suppression">How quickly the backing is suppressed.</param>
        /// <returns>The modified value to use as the new progress value.</returns>
        protected static double InterpolateBackIn(double t, double amplitude, double suppression) {
            // Math Magic
            // The sine gives us the right wave, the timeFraction * 0.5 (frequency) gives us only half 
            // of the full wave, the amplitude gives us the relative height of the peak, and the
            // exponent makes the effect drop off more quickly by the "suppression" factor. 
            //
            double frequency = 0.5;
            return Math.Pow((1 - t), suppression) * amplitude * Math.Sin(2 * Math.PI * t * frequency) * -1 + t;
        }

        /// <summary>
        /// Performs back interpolation when the selected easing mode is set to EaseOut
        /// or during the second half of an EaseInOut interpolation.
        /// </summary>
        /// <param name="t">The current progress value based on linear interpolation.</param>
        /// <param name="amplitude">The relative distance of the backing.</param>
        /// <param name="suppression">How quickly the backing is suppressed.</param>
        /// <returns>The modified value to use as the new progress value.</returns>
        protected static double InterpolateBackOut(double t, double amplitude, double suppression) {
            // Math Magic
            // The sine gives us the right wave, the timeFraction * 0.5 (frequency) gives us only half 
            // of the full wave, the amplitude gives us the relative height of the peak, and the
            // exponent makes the effect drop off more quickly by the "suppression" factor. 
            //
            double frequency = 0.5;
            return Math.Pow(t, suppression) * amplitude * Math.Sin(2 * Math.PI * t * frequency) + t;
        }
        #endregion

        #region Bounce Interpolation
        /// <summary>
        /// Performs bouncing interpolation when the selected easing mode is set to EaseIn
        /// or during the first half of an EaseInOut interpolation.
        /// </summary>
        /// <param name="t">The current progress value based on linear interpolation.</param>
        /// <param name="bounces">The number of bounces to introduce.</param>
        /// <param name="bounciness">Determines the height of the bounces.</param>
        /// <returns>The modified value to use as the new progress value.</returns>
        protected static double InterpolateBounceIn(double t, int bounces, double bounciness) {
            // Math Magic
            // The cosine gives us the right wave, the t is the frequency of the wave, 
            // the absolute value keeps every value positive (so it "bounces" off the
            // midpoint of the cosine wave, and the amplitude (the exponent) makes the sine
            // wave get smaller and smaller at the end.
            //
            return Math.Abs(Math.Pow(t, bounciness) * Math.Cos(2 * Math.PI * t * bounces));
        }

        /// <summary>
        /// Performs bouncing interpolation when the selected easing mode is set to EaseOut
        /// or during the second half of an EaseInOut interpolation.
        /// </summary>
        /// <param name="t">The current progress value based on linear interpolation.</param>
        /// <param name="bounces">The number of bounces to introduce.</param>
        /// <param name="bounciness">Determines the height of the bounces.</param>
        /// <returns>The modified value to use as the new progress value.</returns>
        protected static double InterpolateBounceOut(double t, int bounces, double bounciness) {
            // Math Magic
            // The cosine gives us the right wave, the t is the frequency of the wave, 
            // the absolute value keeps every value positive (so it "bounces" off the
            // midpoint of the cosine wave, and the amplitude (the exponent) makes the sine
            // wave get smaller and smaller at the end.
            //
            return 1 - Math.Abs(Math.Pow((1 - t), bounciness) * Math.Cos(2 * Math.PI * t * bounces));
        }
        #endregion

        #region Elastic Interpolation
        /// <summary>
        /// Performs elastic interpolation when the selected easing mode is set to EaseIn
        /// or during the first half of an EaseInOut interpolation.
        /// </summary>
        /// <param name="t">The current progress value based on linear interpolation.</param>
        /// <param name="oscillations">The number of oscillations to introduce.</param>
        /// <param name="springiness">Determines the strength of the oscillations.</param>
        /// <returns>The modified value to use as the new progress value.</returns>
        protected static double InterpolateElasticIn(double t, int oscillations, double springiness) {
            // Math Magic
            // The cosine gives us the right wave, the t * the # of oscillations is the 
            // frequency of the wave, and the amplitude (the exponent) makes the wave get smaller
            // at the end by the "springiness" factor. This is extremely similar to the bounce equation.
            //
            return Math.Pow(t, springiness) * Math.Cos(2 * Math.PI * t * oscillations);
        }

        /// <summary>
        /// Performs elastic interpolation when the selected easing mode is set to EaseOut
        /// or during the second half of an EaseInOut interpolation.
        /// </summary>
        /// <param name="t">The current progress value based on linear interpolation.</param>
        /// <param name="oscillations">The number of oscillations to introduce.</param>
        /// <param name="springiness">Determines the strength of the oscillations.</param>
        /// <returns>The modified value to use as the new progress value.</returns>
        protected static double InterpolateElasticOut(double t, int oscillations, double springiness) {
            // Math Magic
            // The cosine gives us the right wave, the t * the # of oscillations is the 
            // frequency of the wave, and the amplitude (the exponent) makes the wave get smaller
            // at the end by the "springiness" factor. This is extremely similar to the bounce equation.
            //
            return 1 - Math.Pow((1 - t), springiness) * Math.Cos(2 * Math.PI * t * oscillations);
        }
        #endregion

        #region Exponential Interpolation
        /// <summary>
        /// Performs exponential interpolation when the selected easing mode is set to EaseIn
        /// or during the first half of an EaseInOut interpolation.
        /// </summary>
        /// <param name="t">The current progress value based on linear interpolation.</param>
        /// <param name="power">The amount of exponential growth.</param>
        /// <returns>The modified value to use as the new progress value.</returns>
        protected static double InterpolateExponentialIn(double t, double power) {
            // Math Magic
            // Simple exponential growth
            //
            return Math.Pow(t, power);
        }

        /// <summary>
        /// Performs exponential interpolation when the selected easing mode is set to EaseOut
        /// or during the second half of an EaseInOut interpolation.
        /// </summary>
        /// <param name="t">The current progress value based on linear interpolation.</param>
        /// <param name="power">The amount of exponential decay.</param>
        /// <returns>The modified value to use as the new progress value.</returns>
        protected static double InterpolateExponentialOut(double t, double power) {
            // Math Magic
            // Simple exponential decay
            //
            return Math.Pow(t, 1 / power);
        }
        #endregion

        /// <summary>
        /// Performs the interpolation when the selected easing mode is set to EaseIn
        /// or during the first half of an EaseInOut interpolation.
        /// </summary>
        /// <param name="t">The current progress value based on linear interpolation.</param>
        /// <returns>The modified value to use as the new progress value.</returns>
        protected abstract double InterpolateIn(double t);

        /// <summary>
        /// Performs the interpolation when the selected easing mode is set to EaseOut
        /// or during the second half of an EaseInOut interpolation.
        /// </summary>
        /// <param name="t">The current progress value based on linear interpolation.</param>
        /// <returns>The modified value to use as the new progress value.</returns>
        protected abstract double InterpolateOut(double t);

        #region Quadratic Interpolation
        /// <summary>
        /// Performs quadratic interpolation when the selected easing mode is set to EaseIn
        /// or during the first half of an EaseInOut interpolation.
        /// </summary>
        /// <param name="t">The current progress value based on linear interpolation.</param>
        /// <returns>The modified value to use as the new progress value.</returns>
        protected static double InterpolateQuadraticIn(double t) {
            return t * t;
        }

        /// <summary>
        /// Performs quadratic interpolation when the selected easing mode is set to EaseOut
        /// or during the second half of an EaseInOut interpolation.
        /// </summary>
        /// <param name="t">The current progress value based on linear interpolation.</param>
        /// <returns>The modified value to use as the new progress value.</returns>
        protected static double InterpolateQuadraticOut(double t) {
            return -t * (t - 2);
        }
        #endregion


        private sealed class SmoothInterpolation : EasingInterpolation {

            public SmoothInterpolation() {
                Mode = EasingInterpolationMode.EaseInOut;
            }

            protected override double InterpolateIn(double t) {
                return InterpolateQuadraticIn(t);
            }

            protected override double InterpolateOut(double t) {
                return InterpolateQuadraticOut(t);
            }
        }
    }
}
