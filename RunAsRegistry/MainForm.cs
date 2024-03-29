﻿/**
 * This is open-source software licensed under the terms of the MIT License.
 *
 * Copyright (c) 2020-2024 Petr Červinka - FortSoft <cervinka@fortsoft.eu>
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
 * Version 1.3.2.1
 */

using FostSoft.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace RunAsRegistry {
    public partial class MainForm : Form {
        private Form dialog;
        private int textBoxClicks;
        private Point location;
        private Settings settings;
        private string workingFolderPathTemp, shortcutNameTemp;
        private Timer textBoxClicksTimer;

        public MainForm(Settings settings) {
            Icon = Properties.Resources.Icon;
            Text = Application.ProductName;

            textBoxClicksTimer = new Timer();
            textBoxClicksTimer.Interval = SystemInformation.DoubleClickTime;
            textBoxClicksTimer.Tick += new EventHandler((sender, e) => {
                textBoxClicksTimer.Stop();
                textBoxClicks = 0;
            });

            InitializeComponent();

            this.settings = settings;
            textBox1.Text = settings.ApplicationFilePath;
            textBox2.Text = settings.Arguments;
            textBox3.Text = settings.WorkingFolderPath;
            textBox4.Text = settings.RegFilePath;
            textBox5.Text = settings.ShortcutName;
            checkBox.Checked = settings.OneInstance;
        }

        private List<string> BuildArguments() {
            List<string> arguments = new List<string>();
            string applicationFilePath = textBox1.Text;
            if (!string.IsNullOrWhiteSpace(applicationFilePath) && !File.Exists(applicationFilePath)) {
                throw new ApplicationException(Properties.Resources.MessageApplicationNotFound);
            }
            if (!string.IsNullOrWhiteSpace(applicationFilePath)) {
                arguments.Add(Constants.CommandLineSwitchWI);
                arguments.Add(StaticMethods.EscapeArgument(applicationFilePath));
            } else {
                throw new ApplicationException(Properties.Resources.MessageApplicationNotSet);
            }
            if (!string.IsNullOrWhiteSpace(textBox2.Text)) {
                arguments.Add(Constants.CommandLineSwitchWA);
                arguments.Add(StaticMethods.EscapeArgument(textBox2.Text));
            }
            if (Directory.Exists(textBox3.Text)) {
                arguments.Add(Constants.CommandLineSwitchWW);
                arguments.Add(StaticMethods.EscapeArgument(textBox3.Text));
            }
            if (checkBox.Checked) {
                arguments.Add(Constants.CommandLineSwitchWO);
            }
            string regFilePath = textBox4.Text;
            if (!string.IsNullOrWhiteSpace(regFilePath) && !File.Exists(regFilePath)) {
                throw new ApplicationException(Properties.Resources.MessageRegFileNotFound);
            }
            if (!string.IsNullOrWhiteSpace(regFilePath)) {
                arguments.Add(Constants.CommandLineSwitchWR);
                arguments.Add(StaticMethods.EscapeArgument(regFilePath));
            } else {
                throw new ApplicationException(Properties.Resources.MessageRegFileNotSet);
            }
            return arguments;
        }

        private void Close(object sender, EventArgs e) => Close();

        private void CreateShortcut(object sender, EventArgs e) {
            if (string.IsNullOrWhiteSpace(textBox5.Text)) {
                dialog = new MessageForm(this, Properties.Resources.MessageShortcutNameNotSet, null, MessageForm.Buttons.OK,
                    MessageForm.BoxIcon.Exclamation);
                dialog.HelpRequested += new HelpEventHandler(OpenHelp);
                dialog.ShowDialog(this);
                textBox5.Focus();
                textBox5.SelectAll();
                return;
            }
            try {
                string shortcutFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), textBox5.Text);
                if (!shortcutFilePath.EndsWith(Constants.ExtensionLnk, StringComparison.OrdinalIgnoreCase)) {
                    shortcutFilePath += Constants.ExtensionLnk;
                }
                if (File.Exists(shortcutFilePath)) {
                    dialog = new MessageForm(this, Properties.Resources.MessageShortcutAlreadyExists, null, MessageForm.Buttons.YesNo,
                        MessageForm.BoxIcon.Warning, MessageForm.DefaultButton.Button2);
                    dialog.HelpRequested += new HelpEventHandler(OpenHelp);
                    if (!dialog.ShowDialog(this).Equals(DialogResult.Yes)) {
                        textBox5.Focus();
                        textBox5.SelectAll();
                        return;
                    }
                }
                List<string> arguments = BuildArguments();
                ProgramShortcut programShortcut = new ProgramShortcut() {
                    ShortcutFilePath = shortcutFilePath,
                    TargetPath = Application.ExecutablePath,
                    WorkingDirectory = Application.StartupPath,
                    Arguments = string.Join(Constants.Space.ToString(), arguments),
                    IconLocation = textBox1.Text
                };
                programShortcut.Create();
            } catch (ApplicationException exception) {
                ShowApplicationException(exception);
            } catch (Exception exception) {
                ShowException(exception);
            }
        }

        private void EditRegFile(object sender, EventArgs e) {
            if (string.IsNullOrWhiteSpace(textBox4.Text)
                    || !textBox4.Text.EndsWith(Constants.ExtensionReg, StringComparison.OrdinalIgnoreCase)) {

                dialog = new MessageForm(this, Properties.Resources.MessageRegFileNotSet, null, MessageForm.Buttons.OK,
                    MessageForm.BoxIcon.Exclamation);
                dialog.HelpRequested += new HelpEventHandler(OpenHelp);
                dialog.ShowDialog(this);
                textBox4.Focus();
                textBox4.SelectAll();
                return;
            }
            if (!File.Exists(textBox4.Text)) {
                dialog = new MessageForm(this, Properties.Resources.MessageRegFileNotFound, null, MessageForm.Buttons.OK,
                    MessageForm.BoxIcon.Exclamation);
                dialog.HelpRequested += new HelpEventHandler(OpenHelp);
                dialog.ShowDialog(this);
                textBox4.Focus();
                textBox4.SelectAll();
                return;
            }
            try {
                Process process = new Process();
                process.StartInfo.FileName = Constants.NotepadExeFileName;
                process.StartInfo.Arguments = StaticMethods.EscapeArgument(textBox4.Text);
                process.Start();
            } catch (Exception exception) {
                ShowException(exception);
            }
        }

        private void Launch(object sender, EventArgs e) {
            try {
                List<string> arguments = BuildArguments();
                Process process = new Process();
                process.StartInfo.FileName = Application.ExecutablePath;
                process.StartInfo.Arguments = string.Join(Constants.Space.ToString(), arguments);
                process.StartInfo.WorkingDirectory = Application.StartupPath;
                process.Start();
                SaveSettings();
            } catch (ApplicationException exception) {
                ShowApplicationException(exception);
            } catch (Exception exception) {
                ShowException(exception);
            }
        }

        private void OnDragEnter(object sender, DragEventArgs e) {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop, false) ? DragDropEffects.All : DragDropEffects.None;
        }

        private void OnDragDrop(object sender, DragEventArgs e) {
            try {
                string filePath = ((string[])e.Data.GetData(DataFormats.FileDrop, false))[0];
                if (filePath.EndsWith(Constants.ExtensionReg, StringComparison.OrdinalIgnoreCase)) {
                    textBox4.Text = filePath;
                    textBox4.Focus();
                    textBox4.SelectAll();
                } else {
                    if (string.IsNullOrWhiteSpace(textBox3.Text) || !Directory.Exists(textBox3.Text)
                            || textBox3.Text.Equals(workingFolderPathTemp)) {

                        textBox3.Text = Path.GetDirectoryName(filePath);
                        textBox3.SelectAll();
                        workingFolderPathTemp = textBox3.Text;
                    }
                    if (string.IsNullOrWhiteSpace(textBox5.Text) || textBox5.Text.Equals(shortcutNameTemp)) {
                        textBox5.Text = Application.ProductName + Constants.Space + Path.GetFileNameWithoutExtension(filePath);
                        textBox5.SelectAll();
                        shortcutNameTemp = textBox5.Text;
                    }
                    textBox1.Text = filePath;
                    textBox1.Focus();
                    textBox1.SelectAll();
                }
            } catch (Exception exception) {
                ShowException(exception);
            }
        }

        private void OnFormActivated(object sender, EventArgs e) {
            if (dialog != null) {
                dialog.Activate();
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e) => SaveSettings();

        private void OnKeyDown(object sender, KeyEventArgs e) {
            if (e.Control && e.KeyCode.Equals(Keys.A)) {
                e.SuppressKeyPress = true;
                if (sender is TextBox) {
                    ((TextBox)sender).SelectAll();
                }
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e) {
            if (!e.Button.Equals(MouseButtons.Left)) {
                textBoxClicks = 0;
                return;
            }
            TextBox textBox = (TextBox)sender;
            textBoxClicksTimer.Stop();
            if (textBox.SelectionLength > 0) {
                textBoxClicks = 2;
            } else if (textBoxClicks.Equals(0) || Math.Abs(e.X - location.X) < 2 && Math.Abs(e.Y - location.Y) < 2) {
                textBoxClicks++;
            } else {
                textBoxClicks = 0;
            }
            location = e.Location;
            if (textBoxClicks.Equals(3)) {
                textBoxClicks = 0;
                NativeMethods.MouseEvent(Constants.MOUSEEVENTF_LEFTUP, Cursor.Position.X, Cursor.Position.Y, 0, 0);
                Application.DoEvents();
                if (textBox.Multiline) {
                    char[] chars = textBox.Text.ToCharArray();
                    int selectionEnd = Math.Min(
                        Array.IndexOf(chars, Constants.CarriageReturn, textBox.SelectionStart),
                        Array.IndexOf(chars, Constants.LineFeed, textBox.SelectionStart));
                    if (selectionEnd < 0) {
                        selectionEnd = textBox.TextLength;
                    }
                    selectionEnd = Math.Max(textBox.SelectionStart + textBox.SelectionLength, selectionEnd);
                    int selectionStart = Math.Min(textBox.SelectionStart, selectionEnd);
                    while (--selectionStart > 0
                        && !chars[selectionStart].Equals(Constants.LineFeed)
                        && !chars[selectionStart].Equals(Constants.CarriageReturn)) { }
                    textBox.Select(selectionStart, selectionEnd - selectionStart);
                } else {
                    textBox.SelectAll();
                }
                textBox.Focus();
            } else {
                textBoxClicksTimer.Start();
            }
        }

        private void OpenHelp(object sender, HelpEventArgs hlpevent) {
            try {
                StringBuilder url = new StringBuilder()
                    .Append(Properties.Resources.Website.TrimEnd(Constants.Slash).ToLowerInvariant())
                    .Append(Constants.Slash)
                    .Append(Application.ProductName.ToLowerInvariant())
                    .Append(Constants.Slash);
                Process.Start(url.ToString());
            } catch (Exception exception) {
                ShowException(exception);
            }
        }

        private void OpenRegedit(object sender, EventArgs e) {
            try {
                Process process = new Process();
                process.StartInfo.FileName = Constants.RegeditExeFileName;
                process.Start();
            } catch (Exception exception) {
                ShowException(exception);
            }
        }

        private void SaveSettings() {
            settings.ApplicationFilePath = textBox1.Text;
            settings.Arguments = textBox2.Text;
            settings.WorkingFolderPath = textBox3.Text;
            settings.RegFilePath = textBox4.Text;
            settings.ShortcutName = textBox5.Text;
            settings.OneInstance = checkBox.Checked;
            settings.Save();
        }

        private void SelectApplication(object sender, EventArgs e) {
            try {
                if (!string.IsNullOrEmpty(textBox1.Text)) {
                    string directoryPath = Path.GetDirectoryName(textBox1.Text);
                    if (Directory.Exists(directoryPath)) {
                        openFileDialog1.InitialDirectory = directoryPath;
                    }
                    if (File.Exists(textBox1.Text)) {
                        openFileDialog1.FileName = Path.GetFileName(textBox1.Text);
                    }
                }
            } catch (Exception exception) {
                Debug.WriteLine(exception);
                ErrorLog.WriteLine(exception);
            }
            try {
                if (openFileDialog1.ShowDialog(this).Equals(DialogResult.OK)) {
                    textBox1.Text = openFileDialog1.FileName;
                    if (string.IsNullOrWhiteSpace(textBox3.Text) || !Directory.Exists(textBox3.Text)
                            || textBox3.Text.Equals(workingFolderPathTemp)) {

                        textBox3.Text = Path.GetDirectoryName(textBox1.Text);
                        textBox3.SelectAll();
                        workingFolderPathTemp = textBox3.Text;
                    }
                    if (string.IsNullOrWhiteSpace(textBox5.Text) || textBox5.Text.Equals(shortcutNameTemp)) {
                        textBox5.Text = Application.ProductName + Constants.Space + Path.GetFileNameWithoutExtension(textBox1.Text);
                        textBox5.SelectAll();
                        shortcutNameTemp = textBox5.Text;
                    }
                }
            } catch (Exception exception) {
                ShowException(exception);
            } finally {
                textBox1.Focus();
                textBox1.SelectAll();
            }
        }

        private void SelectFolder(object sender, EventArgs e) {
            try {
                if (!string.IsNullOrEmpty(textBox3.Text)) {
                    if (Directory.Exists(textBox3.Text)) {
                        folderBrowserDialog.SelectedPath = textBox3.Text;
                    }
                }
            } catch (Exception exception) {
                Debug.WriteLine(exception);
                ErrorLog.WriteLine(exception);
            }
            try {
                if (folderBrowserDialog.ShowDialog(this).Equals(DialogResult.OK)) {
                    if (!textBox3.Text.Equals(folderBrowserDialog.SelectedPath)) {
                        textBox3.Text = folderBrowserDialog.SelectedPath;
                        workingFolderPathTemp = textBox3.Text;
                    }
                }
            } catch (Exception exception) {
                ShowException(exception);
            } finally {
                textBox3.Focus();
                textBox3.SelectAll();
            }
        }

        private void SelectRegFile(object sender, EventArgs e) {
            try {
                if (!string.IsNullOrEmpty(textBox4.Text)) {
                    string directoryPath = Path.GetDirectoryName(textBox4.Text);
                    if (Directory.Exists(directoryPath)) {
                        openFileDialog2.InitialDirectory = directoryPath;
                    }
                    if (File.Exists(textBox4.Text)) {
                        openFileDialog2.FileName = Path.GetFileName(textBox4.Text);
                    }
                }
            } catch (Exception exception) {
                Debug.WriteLine(exception);
                ErrorLog.WriteLine(exception);
            }
            try {
                if (openFileDialog2.ShowDialog(this).Equals(DialogResult.OK)) {
                    textBox4.Text = openFileDialog2.FileName;
                }
            } catch (Exception exception) {
                ShowException(exception);
            } finally {
                textBox4.Focus();
                textBox4.SelectAll();
            }
        }

        private void ShowAbout(object sender, EventArgs e) {
            dialog = new AboutForm();
            dialog.HelpRequested += new HelpEventHandler(OpenHelp);
            dialog.ShowDialog(this);
        }

        private void ShowApplicationException(ApplicationException exception) {
            Debug.WriteLine(exception);
            ErrorLog.WriteLine(exception);
            dialog = new MessageForm(this, exception.Message, null, MessageForm.Buttons.OK, MessageForm.BoxIcon.Exclamation);
            dialog.HelpRequested += new HelpEventHandler(OpenHelp);
            dialog.ShowDialog(this);
        }

        private void ShowException(Exception exception) => ShowException(exception, null);

        private void ShowException(Exception exception, string statusMessage) {
            Debug.WriteLine(exception);
            ErrorLog.WriteLine(exception);
            StringBuilder title = new StringBuilder()
                .Append(Program.GetTitle())
                .Append(Constants.Space)
                .Append(Constants.EnDash)
                .Append(Constants.Space)
                .Append(Properties.Resources.CaptionError);
            dialog = new MessageForm(this, exception.Message, title.ToString(), MessageForm.Buttons.OK, MessageForm.BoxIcon.Error);
            dialog.HelpRequested += new HelpEventHandler(OpenHelp);
            dialog.ShowDialog(this);
        }
    }
}
