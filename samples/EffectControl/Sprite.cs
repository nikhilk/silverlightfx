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

        private static readonly DependencyProperty EffectProperty =
            DependencyProperty.Register("Effect", typeof(Effect), typeof(Sprite), null);

        private EffectDirection _direction;

        public Sprite() {
            DefaultStyleKey = typeof(Sprite);
            MouseLeftButtonDown += OnMouseLeftButtonDown;
        }

        public Effect Effect {
            get {
                return (Effect)GetValue(EffectProperty);
            }
            set {
                SetValue(EffectProperty, value);
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
            Effect effect = Effect;
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

                EffectDirection direction = _direction;
                _direction = _direction == EffectDirection.Forward ? EffectDirection.Reverse : EffectDirection.Forward;

                effect.PlayEffect(direction);
            }
        }
    }
}
