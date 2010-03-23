// PlayWaveAudio.cs
//

using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using Translate.Services.Audio;
using System.Windows.Interactivity;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Translate {

    public class PlayWaveAudio : TriggerAction<MediaElement> {

        protected override void InvokeAction(EventArgs e) {
            StreamEventArgs streamEvent = (StreamEventArgs)e;
            AssociatedObject.SetSource(new WaveMediaStreamSource(streamEvent.Stream));
        }
    }
}
