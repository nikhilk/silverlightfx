// ComponentCollection.cs
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
using System.Collections.ObjectModel;

namespace SilverlightFX.Applications {

    /// <summary>
    /// Represents a list of components. Components can only be added; they cannot
    /// be removed or replaced.
    /// </summary>
    public sealed class ComponentCollection : ObservableCollection<object> {

        /// <internalonly />
        protected override void ClearItems() {
            throw new NotSupportedException();
        }

        /// <internalonly />
        protected override void RemoveItem(int index) {
            throw new NotSupportedException();
        }

        /// <internalonly />
        protected override void SetItem(int index, object item) {
            throw new NotSupportedException();
        }
    }
}
