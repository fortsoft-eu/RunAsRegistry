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
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace RunAsRegistry {
    internal sealed class LauncherAsRegistry : IDisposable {
        private Process process1, process2;

        internal LauncherAsRegistry() {
            process1 = new Process();
            process2 = new Process();
            process2.StartInfo.FileName = Constants.RegeditExeFileName;
        }

        internal bool OneInstance { get; set; }

        internal string ApplicationFilePath { get; set; }

        internal string Arguments { get; set; }

        internal string RegFilePath { get; set; }

        internal string WorkingFolderPath { get; set; }

        public void Dispose() {
            process1.Dispose();
            process2.Dispose();
        }

        internal void Launch() {
            if (OneInstance) {
                if (SingleInstance.FocusRunning(ApplicationFilePath)) {
                    return;
                }
            }
            process1.StartInfo.FileName = ApplicationFilePath;
            process1.StartInfo.Arguments = Arguments;
            process1.StartInfo.WorkingDirectory = WorkingFolderPath;
            process2.StartInfo.Arguments = new StringBuilder()
                .Append(Constants.CommandLineSwitchWS)
                .Append(Constants.Space)
                .Append(StaticMethods.EscapeArgument(RegFilePath))
                .ToString();
            process2.Start();
            do {
                Thread.Sleep(100);
            } while (!process2.HasExited);
            process1.Start();
        }
    }
}
