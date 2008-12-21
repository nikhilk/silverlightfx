// XGrid.cs
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
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Silverlight.FX.UserInterface {

    /// <summary>
    /// An extended Grid control with more convenient syntax for defining its
    /// rows and columns.
    /// </summary>
    public class XGrid : Grid {

        /// <summary>
        /// Gets or sets the list of columns as a comma separated list of widths.
        /// </summary>
        public string Columns {
            get {
                ColumnDefinitionCollection columns = ColumnDefinitions;
                StringBuilder sb = new StringBuilder();

                bool first = true;
                foreach (ColumnDefinition cd in columns) {
                    if (first == false) {
                        sb.Append(",");
                    }
                    sb.Append(cd.Width.ToString());
                    first = false;
                }

                return sb.ToString();
            }
            set {
                ColumnDefinitionCollection columns = ColumnDefinitions;
                columns.Clear();

                List<GridLength> widths = ParseLengths(value);
                for (int i = 0; i < widths.Count; i++) {
                    ColumnDefinition cd = new ColumnDefinition();
                    cd.Width = widths[i];

                    columns.Add(cd);
                }
            }
        }

        /// <summary>
        /// Gets or sets the list of rows as a comma separated list of heights.
        /// </summary>
        public string Rows {
            get {
                RowDefinitionCollection rows = RowDefinitions;
                StringBuilder sb = new StringBuilder();

                bool first = true;
                foreach (RowDefinition rd in rows) {
                    if (first == false) {
                        sb.Append(",");
                    }
                    sb.Append(rd.Height.ToString());
                    first = false;
                }

                return sb.ToString();
            }
            set {
                RowDefinitionCollection rows = RowDefinitions;
                rows.Clear();

                List<GridLength> heights = ParseLengths(value);
                for (int i = 0; i < heights.Count; i++) {
                    RowDefinition rd = new RowDefinition();
                    rd.Height = heights[i];

                    rows.Add(rd);
                }
            }
        }

        private List<GridLength> ParseLengths(string value) {
            List<GridLength> lengths = new List<GridLength>();

            string[] parts = value.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if ((parts != null) && (parts.Length != 0)) {
                for (int i = 0; i < parts.Length; i++) {
                    string lengthPart = parts[i];

                    if (String.Compare(lengthPart, "Auto", StringComparison.OrdinalIgnoreCase) == 0) {
                        lengths.Add(new GridLength(0, GridUnitType.Auto));
                    }
                    else if (lengthPart.EndsWith("*", StringComparison.Ordinal)) {
                        if (lengthPart.Length == 1) {
                            lengths.Add(new GridLength(1, GridUnitType.Star));
                        }
                        else {
                            double d = Double.Parse(lengthPart.Substring(0, lengthPart.Length - 1));
                            lengths.Add(new GridLength(d, GridUnitType.Star));
                        }
                    }
                    else {
                        double d = Double.Parse(lengthPart);
                        lengths.Add(new GridLength(d, GridUnitType.Pixel));
                    }
                }
            }

            return lengths;
        }
    }
}
