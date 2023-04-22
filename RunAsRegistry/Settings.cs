/**
 * This is open-source software licensed under the terms of the MIT License.
 *
 * Copyright (c) 2020-2023 Petr Červinka - FortSoft <cervinka@fortsoft.eu>
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 **
 * Version 1.3.1.0
 */

using FortSoft.Tools;
using System;
using System.Collections;
using System.Text;

namespace RunAsRegistry {

    /// <summary>
    /// This is an implementation of using the PersistentSettings class for
    /// RunAsRegistry.
    /// </summary>
    public sealed class Settings : IDisposable {

        /// <summary>
        /// Field.
        /// </summary>
        private PersistentSettings persistentSettings;

        /// <summary>
        /// Occurs on successful saving all application settings into the Windows
        /// registry.
        /// </summary>
        public event EventHandler Saved;

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings() {
            persistentSettings = new PersistentSettings();
            Load();
        }

        /// <summary>
        /// Represents the setting if the application should check for updates.
        /// The default value is true.
        /// </summary>
        public bool CheckForUpdates { get; set; } = true;

        /// <summary>
        /// Represents whether visual styles will be used when rendering
        /// application windows. The default value is false.
        /// </summary>
        public bool DisableThemes { get; set; }

        /// <summary>
        /// An example of software application setting that will be stored in the
        /// Windows registry.
        /// </summary>
        public bool OneInstance { get; set; }

        /// <summary>
        /// Represents the printing setting, whether to use soft margins (larger)
        /// or hard margins (smaller). This setting does not apply to the
        /// embedded Chromium browser. The default value is true.
        /// </summary>
        public bool PrintSoftMargins { get; set; } = true;

        /// <summary>
        /// Represents the setting if the application should inform the user
        /// about available updates in the status bar only. If not, a pop-up
        /// window will appear. The default value is false.
        /// </summary>
        public bool StatusBarNotifOnly { get; set; }

        /// <summary>
        /// An example of software application setting that will be stored in the
        /// Windows registry.
        /// </summary>
        public string ApplicationFilePath { get; set; }

        /// <summary>
        /// An example of software application setting that will be stored in the
        /// Windows registry.
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// An example of software application setting that will be stored in the
        /// Windows registry.
        /// </summary>
        public string RegFilePath { get; set; }

        /// <summary>
        /// An example of software application setting that will be stored in the
        /// Windows registry.
        /// </summary>
        public string ShortcutName { get; set; }

        /// <summary>
        /// An example of software application setting that will be stored in the
        /// Windows registry.
        /// </summary>
        public string WorkingFolderPath { get; set; }

        /// <summary>
        /// Loads the software application settings from the Windows registry.
        /// </summary>
        private void Load() {
            IntToBitSettings(persistentSettings.Load("BitSettings", BitSettingsToInt()));
            ApplicationFilePath = persistentSettings.Load("Path", ApplicationFilePath);
            Arguments = persistentSettings.Load("Arguments", Arguments);
            WorkingFolderPath = persistentSettings.Load("Folder", WorkingFolderPath);
            RegFilePath = persistentSettings.Load("RegFile", RegFilePath);
            ShortcutName = persistentSettings.Load("Shortcut", ShortcutName);
        }

        /// <summary>
        /// Saves the software application settings into the Windows registry.
        /// </summary>
        public void Save() {
            persistentSettings.Save("BitSettings", BitSettingsToInt());
            persistentSettings.Save("Path", ApplicationFilePath);
            persistentSettings.Save("Arguments", Arguments);
            persistentSettings.Save("Folder", WorkingFolderPath);
            persistentSettings.Save("RegFile", RegFilePath);
            persistentSettings.Save("Shortcut", ShortcutName);
            Saved?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Expands an integer value into some boolean settings.
        /// </summary>
        private void IntToBitSettings(int i) {
            BitArray bitArray = new BitArray(new int[] { i });
            bool[] bitSettings = new bool[bitArray.Count];
            bitArray.CopyTo(bitSettings, 0);
            i = bitSettings.Length - 27;

            OneInstance = bitSettings[--i];
            DisableThemes = bitSettings[--i];
            PrintSoftMargins = bitSettings[--i];
            StatusBarNotifOnly = bitSettings[--i];
            CheckForUpdates = bitSettings[--i];
        }

        /// <summary>
        /// Compacts some boolean settings into an integer value.
        /// </summary>
        private int BitSettingsToInt() {
            StringBuilder stringBuilder = new StringBuilder(string.Empty.PadRight(27, Constants.Zero))
                .Append(OneInstance ? 1 : 0)
                .Append(DisableThemes ? 1 : 0)
                .Append(PrintSoftMargins ? 1 : 0)
                .Append(StatusBarNotifOnly ? 1 : 0)
                .Append(CheckForUpdates ? 1 : 0);
            return Convert.ToInt32(stringBuilder.ToString(), 2);
        }

        /// <summary>
        /// This setting will not be directly stored in the Windows registry.
        /// </summary>
        public bool RenderWithVisualStyles { get; set; }

        /// <summary>
        /// Clears the software application values from the Windows registry.
        /// </summary>
        public void Clear() => persistentSettings.Clear();

        /// <summary>Clean up any resources being used.</summary>
        public void Dispose() => persistentSettings.Dispose();
    }
}
