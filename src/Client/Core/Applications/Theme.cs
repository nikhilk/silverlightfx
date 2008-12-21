// Theme.cs
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
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Resources;
using System.Xml;

namespace SilverlightFX.Applications {

    /// <summary>
    /// Represents a theme control, whose top-level resource items are to be included
    /// when this theme is selected.
    /// </summary>
    public class Theme : UserControl {

        private string _includes;

        /// <summary>
        /// Any additional themes to include. Each of the additional themes must contain
        /// a UserControl declaration. The list of themes are processed in order after
        /// the current theme's resource items have been included.
        /// </summary>
        public string Includes {
            get {
                return _includes ?? String.Empty;
            }
            set {
                _includes = value;
            }
        }

        private static void ExtractThemeContent(string xml, ThemeInfo theme, bool extractIncludes) {
            XmlReader xmlReader = XmlReader.Create(new StringReader(xml),
                                                   new XmlReaderSettings {
                                                       CheckCharacters = false,
                                                       DtdProcessing = DtdProcessing.Ignore,
                                                       IgnoreComments = true,
                                                       IgnoreProcessingInstructions = true,
                                                       IgnoreWhitespace = true,
                                                   });
            while (xmlReader.Read()) {
                if (xmlReader.NodeType == XmlNodeType.Element) {
                    // Found the first element
                    string tagName = xmlReader.Name;

                    // Extract all the xml namespaces
                    bool hasAttributes = xmlReader.MoveToFirstAttribute();
                    while (hasAttributes) {
                        if (xmlReader.Name.StartsWith("xmlns:")) {
                            string namespaceDeclaration = xmlReader.ReadContentAsString();
                            theme.AddNamespace(xmlReader.Name, namespaceDeclaration);
                        }
                        hasAttributes = xmlReader.MoveToNextAttribute();
                    }

                    if (extractIncludes) {
                        theme.Includes = xmlReader.GetAttribute("Includes");
                    }

                    string resourcesTag = tagName + ".Resources";
                    if (xmlReader.ReadToFollowing(resourcesTag)) {
                        xmlReader.MoveToContent();

                        // TODO: I would have expected MoveToContent to move to the first item
                        //       within the current tag, but apparently the reader is still
                        //       positioned at the current tag, unless a Read is performed.
                        xmlReader.Read();

                        while (xmlReader.EOF == false) {
                            if (xmlReader.NodeType == XmlNodeType.Element) {
                                string key = xmlReader.GetAttribute("x:Key");
                                if ((String.IsNullOrEmpty(key) == false) && (theme.ContainsKey(key) == false)) {
                                    string markup = xmlReader.ReadOuterXml();
                                    theme.AddItem(key, markup);
                                }
                                else {
                                    xmlReader.Skip();
                                }
                            }
                            else {
                                // No more items
                                break;
                            }
                        }
                    }

                    // We only look at the resources of the root tag, and so we're done
                    break;
                }
            }
        }

        private static string GetThemeXaml(string themeName, string fileName, bool lookupShared) {
            StreamResourceInfo resourceInfo = null;

            // First try /Themes/<ThemeName>/<FileName>.xaml
            Uri resourceUri = new Uri("Themes/" + themeName + "/" + fileName + ".xaml", UriKind.Relative);
            resourceInfo = Application.GetResourceStream(resourceUri);

            if ((resourceInfo == null) && lookupShared) {
                // The fallback is one level up, i.e. /Themes/<FileName>.xaml
                resourceUri = new Uri("Themes/" + fileName + ".xaml", UriKind.Relative);
                resourceInfo = Application.GetResourceStream(resourceUri);
            }

            if (resourceInfo == null) {
                throw new InvalidOperationException("The file named '" + fileName + " in the theme named '" + themeName + "' could not be found.");
            }

            StreamReader resourceReader = new StreamReader(resourceInfo.Stream);
            return resourceReader.ReadToEnd();
        }

        internal static void LoadTheme(ResourceDictionary targetResources, string name) {
            ThemeInfo theme = new ThemeInfo();

            string themeXaml = GetThemeXaml(name, "Theme", /* lookupShared */ false);
            try {
                ExtractThemeContent(themeXaml, theme, /* extractIncludes */ true);
                if ((theme.Includes.Length == 0) && (theme.Keys.Count != 0)) {
                    UserControl userControl = (UserControl)XamlReader.Load(themeXaml);
                    ResourceDictionary themeResources = userControl.Resources;

                    foreach (string key in theme.Keys) {
                        targetResources.Add(key, themeResources[key]);
                    }

                    return;
                }
            }
            catch (Exception e) {
                throw new InvalidOperationException("The theme named '" + name + "' contained invalid XAML.", e);
            }

            if (theme.Includes.Length != 0) {
                string[] includes = theme.Includes.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < includes.Length; i++) {
                    string includeXaml = GetThemeXaml(name, includes[i], /* lookupShared */ true);
                    try {
                        ExtractThemeContent(includeXaml, theme, /* extractIncludes */ false);
                    }
                    catch (Exception e) {
                        throw new InvalidOperationException("The include named '" + includes[i] + "' in the theme named '" + name + "' contained invalid XAML.", e);
                    }
                }
            }

            try {
                string mergedXaml = theme.GetXml();
                UserControl userControl = (UserControl)XamlReader.Load(mergedXaml);
                ResourceDictionary themeResources = userControl.Resources;

                foreach (string key in theme.Keys) {
                    targetResources.Add(key, themeResources[key]);
                }
            }
            catch (Exception e) {
                throw new InvalidOperationException("The theme named '" + name + "' contained invalid XAML.", e);
            }
        }


        private sealed class ThemeInfo {

            private StringBuilder _content;
            private List<string> _keys;
            private Dictionary<string, string> _keyMap;
            private Dictionary<string, string> _namespaceMap;
            private string _includes;

            public ThemeInfo() {
                _content = new StringBuilder();

                _keys = new List<string>();
                _keyMap = new Dictionary<string, string>();

                _namespaceMap = new Dictionary<string, string>();
                _namespaceMap["xmlns"] = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
                _namespaceMap["xmlns:x"] = "http://schemas.microsoft.com/winfx/2006/xaml";
                _namespaceMap["xmlns:vsm"] = "clr-namespace:System.Windows;assembly=System.Windows";
            }

            public string Includes {
                get {
                    return _includes ?? String.Empty;
                }
                set {
                    _includes = value;
                }
            }

            public ICollection<string> Keys {
                get {
                    return _keys;
                }
            }

            public void AddItem(string key, string xml) {
                _keyMap[key] = String.Empty;
                _keys.Add(key);

                _content.Append(xml);
            }

            public void AddNamespace(string namespaceName, string namespaceDeclaration) {
                _namespaceMap[namespaceName] = namespaceDeclaration;
            }

            public bool ContainsKey(string key) {
                return _keyMap.ContainsKey(key);
            }

            public string GetXml() {
                string resourceDictionaryFormat =
@"<UserControl {0}>
  <UserControl.Resources>
{1}
  </UserControl.Resources>
</UserControl>";

                StringBuilder nsBuilder = new StringBuilder();
                foreach (KeyValuePair<string, string> nsEntry in _namespaceMap) {
                    string xmlnsDeclaration = nsEntry.Key + "=\"" + nsEntry.Value + "\"";

                    nsBuilder.Append(" ");
                    nsBuilder.Append(xmlnsDeclaration);

                    _content.Replace(xmlnsDeclaration, String.Empty);
                }

                return String.Format(resourceDictionaryFormat, nsBuilder.ToString(), _content.ToString());
            }
        }
    }
}
