// FlashBulb.cs
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Glitz;

namespace EffectControl {

    public class FlashBulb : Effect {

        protected override ProceduralAnimation CreateEffectAnimation(EffectDirection direction) {
            FrameworkElement target = GetTarget();

            ScaleTransform scaleTransform = target.RenderTransform as ScaleTransform;
            if (scaleTransform == null) {
                scaleTransform = new ScaleTransform();
                target.RenderTransform = scaleTransform;
                target.RenderTransformOrigin = new Point(0.5, 0.5);
            }

            TimeSpan halfDuration = TimeSpan.FromMilliseconds(Duration.TotalMilliseconds / 2);
            TweenInterpolation interpolation = GetEffectiveInterpolation();

            // A FlashBulb effect basically grows the element and makes it transparent
            // half way and then restores the element to its initial state during the
            // 2nd half of the animation.
            // This is accomplished with three animations that auto-reverse.
            // As a result the animations are the same regardless of effect direction.
            DoubleAnimation opacityAnimation =
                new DoubleAnimation(target, UIElement.OpacityProperty, halfDuration, 0.25);
            DoubleAnimation scaleXAnimation =
                new DoubleAnimation(scaleTransform, ScaleTransform.ScaleXProperty, halfDuration, 1.1);
            DoubleAnimation scaleYAnimation =
                new DoubleAnimation(scaleTransform, ScaleTransform.ScaleYProperty, halfDuration, 1.1);

            opacityAnimation.Interpolation = interpolation;
            scaleXAnimation.Interpolation = interpolation;
            scaleYAnimation.Interpolation = interpolation;

            // Create a composite animation that plays all three in parallel
            ProceduralAnimation animation =
                new ProceduralAnimationSet(opacityAnimation, scaleXAnimation, scaleYAnimation);
            animation.AutoReverse = true;

            return animation;
        }
    }
}
