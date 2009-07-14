// Sprite.cs
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Glitz;

namespace EffectControl {

    [TemplatePart(Name = "ImagePresenter", Type = typeof(Image))]
    public class Sprite : Control {

        private static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(Sprite), null);

        private static readonly DependencyProperty AnimationEffectProperty =
            DependencyProperty.Register("AnimationEffect", typeof(AnimationEffect), typeof(Sprite), null);

        private AnimationEffectDirection _direction;

        public Sprite() {
            DefaultStyleKey = typeof(Sprite);
            MouseLeftButtonDown += OnMouseLeftButtonDown;
        }

        public AnimationEffect AnimationEffect {
            get {
                return (AnimationEffect)GetValue(AnimationEffectProperty);
            }
            set {
                SetValue(AnimationEffectProperty, value);
            }
        }

        public ImageSource Image {
            get {
                return (ImageSource)GetValue(ImageProperty);
            }
            set {
                SetValue(ImageProperty, value);
            }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            AnimationEffect effect = AnimationEffect;
            if (effect != null) {
                if (effect.IsActive) {
                    return;
                }

                IAttachedObject attachedObject = (IAttachedObject)effect;
                if (attachedObject.AssociatedObject != this) {
                    attachedObject.Detach();
                    attachedObject.Attach(this);

                    effect.Target = (Image)GetTemplateChild("ImagePresenter");
                }

                AnimationEffectDirection direction = _direction;
                _direction = _direction == AnimationEffectDirection.Forward ? AnimationEffectDirection.Reverse :
                                                                              AnimationEffectDirection.Forward;

                effect.PlayEffect(direction);
            }
        }
    }
}
