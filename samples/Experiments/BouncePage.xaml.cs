using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Glitz;

namespace Experiments {

    /*
    public abstract class Interpolator : IProceduralAnimationInterpolator {
        public enum EdgeBehaviorEnum {
            EaseOut,
            EaseInOut,
            EaseIn
        }

        private EdgeBehaviorEnum _edgeBehavior;

        public EdgeBehaviorEnum EdgeBehavior {
            get {
                return _edgeBehavior;
            }
            set {
                _edgeBehavior = value;
            }
        }

        public bool Easing {
            get;
            set;
        }

        protected abstract double GetEaseInAlpha(double progress);
        protected abstract double GetEaseOutAlpha(double progress);

        protected virtual double GetEaseInOutAlpha(double timeFraction) {
            double returnValue = 0.0;

            if (Easing) {
                // timeFraction = EaseQuadraticInOut(timeFraction);
            }

            // we cut each effect in half by multiplying the time fraction by two and halving the distance.
            if (timeFraction <= 0.5) {
                returnValue = 0.5 * this.GetEaseInAlpha(timeFraction * 2);
            }
            else {
                returnValue = 0.5 + 0.5 * this.GetEaseOutAlpha((timeFraction - 0.5) * 2);
            }
            return returnValue;
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

        #region IProceduralAnimationInterpolator Members

        public double Interpolate(double t) {
            switch (this.EdgeBehavior) {
                case EdgeBehaviorEnum.EaseIn:
                    return this.GetEaseInAlpha(t);
                case EdgeBehaviorEnum.EaseOut:
                    return this.GetEaseOutAlpha(t);
                case EdgeBehaviorEnum.EaseInOut:
                default:
                    return this.GetEaseInOutAlpha(t);
            }
        }

        #endregion
    }

    public class BounceInterpolator : Interpolator {

        private int _bounces;
        private double _bounciness;

        public BounceInterpolator() {
            _bounciness = 3.0;
            _bounces = 1;
        }

        public int Bounces {
            get {
                return _bounces;
            }
            set {
                if (value <= 0)
                    throw new ArgumentException("can't set the bounces to " + value);
                _bounces = value;
            }
        }

        public double Bounciness {
            get {
                return _bounciness;
            }
            set {
                if (value <= 0)
                    throw new ArgumentException("can't set the bounceiness to " + value);
                _bounciness = value;
            }
        }

        protected override double GetEaseOutAlpha(double timeFraction) {
            double returnValue = 0.0;

            // math magic: The cosine gives us the right wave, the timeFraction is the frequency of the wave, 
            // the absolute value keeps every value positive (so it "bounces" off the midpoint of the cosine 
            // wave, and the amplitude (the exponent) makes the sine wave get smaller and smaller at the end.
            returnValue = Math.Abs(Math.Pow((1 - timeFraction), this.Bounciness)
                          * Math.Cos(2 * Math.PI * timeFraction * this.Bounces));
            returnValue = 1 - returnValue;
            return returnValue;
        }

        protected override double GetEaseInAlpha(double timeFraction) {
            double returnValue = 0.0;
            // math magic: The cosine gives us the right wave, the timeFraction is the amplitude of the wave, 
            // the absolute value keeps every value positive (so it "bounces" off the midpoint of the cosine 
            // wave, and the amplitude (the exponent) makes the sine wave get bigger and bigger towards the end.
            returnValue = Math.Abs(Math.Pow((timeFraction), this.Bounciness)
                          * Math.Cos(2 * Math.PI * timeFraction * this.Bounces));
            return returnValue;
        }
    }
    */

    public partial class BouncePage : UserControl {
        public BouncePage() {
            InitializeComponent();
        }
    }
}
