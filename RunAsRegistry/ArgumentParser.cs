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

using System;
using System.Collections.Generic;
using System.Text;

namespace RunAsRegistry {
    internal class ArgumentParser {
        private bool argumentsSet;
        private bool expectingArguments;
        private bool expectingFilePath;
        private bool expectingFolderPath;
        private bool expectingRegFilePath;
        private bool filePathSet;
        private bool folderPathSet;
        private bool hasArguments;
        private bool helpSet;
        private bool oneInstanceSet;
        private bool regFilePathSet;
        private bool thisTestSet;
        private List<string> arguments;
        private string applicationArguments;
        private string applicationFilePath;
        private string argumentString;
        private string regFilePath;
        private string workingFolderPath;

        internal ArgumentParser() {
            Reset();
        }

        internal bool HasArguments => hasArguments;

        internal bool IsHelp => helpSet;

        internal bool IsThisTest => thisTestSet;

        internal bool OneInstance => oneInstanceSet;

        internal string ApplicationArguments => applicationArguments;

        internal string ApplicationFilePath => applicationFilePath;

        internal string ArgumentString {
            get {
                if (string.IsNullOrEmpty(argumentString) && arguments.Count > 0) {
                    return string.Join(Constants.Space.ToString(), arguments);
                }
                return argumentString;
            }
            set {
                Reset();
                argumentString = value;
                arguments = Parse(argumentString);
                try {
                    Evaluate();
                } catch (Exception exception) {
                    Reset();
                    throw exception;
                }
            }
        }

        internal string RegFilePath => regFilePath;

        internal string WorkingFolderPath => workingFolderPath;

        internal string[] Arguments {
            get {
                return arguments.ToArray();
            }
            set {
                Reset();
                arguments = new List<string>(value.Length);
                arguments.AddRange(value);
                try {
                    Evaluate();
                } catch (Exception exception) {
                    Reset();
                    throw exception;
                }
            }
        }

        private void Evaluate() {
            foreach (string arg in arguments) {
                string argument = arg;
                hasArguments = true;

                // Input file path: Application to launch.
                if (argument.Equals(Constants.CommandLineSwitchUI) || argument.Equals(Constants.CommandLineSwitchWI)) {
                    if (filePathSet || expectingFilePath) {
                        throw new ApplicationException(Properties.Resources.ExceptionMessageI);
                    }
                    if (expectingArguments || expectingFolderPath || expectingRegFilePath || helpSet || thisTestSet) {
                        throw new ApplicationException(Properties.Resources.ExceptionMessageM);
                    }
                    expectingFilePath = true;

                    // Arguments passed to the launched application.
                } else if (argument.Equals(Constants.CommandLineSwitchUA) || argument.Equals(Constants.CommandLineSwitchWA)) {
                    if (argumentsSet || expectingArguments) {
                        throw new ApplicationException(Properties.Resources.ExceptionMessageA);
                    }
                    if (expectingFilePath || expectingFolderPath || expectingRegFilePath || helpSet || thisTestSet) {
                        throw new ApplicationException(Properties.Resources.ExceptionMessageM);
                    }
                    expectingArguments = true;

                    // Working folder path.
                } else if (argument.Equals(Constants.CommandLineSwitchUW) || argument.Equals(Constants.CommandLineSwitchWW)) {
                    if (folderPathSet || expectingFolderPath) {
                        throw new ApplicationException(Properties.Resources.ExceptionMessageW);
                    }
                    if (expectingFilePath || expectingArguments || expectingRegFilePath || helpSet || thisTestSet) {
                        throw new ApplicationException(Properties.Resources.ExceptionMessageM);
                    }
                    expectingFolderPath = true;

                    // Registry file path.
                } else if (argument.Equals(Constants.CommandLineSwitchUR) || argument.Equals(Constants.CommandLineSwitchWR)) {
                    if (regFilePathSet || expectingRegFilePath) {
                        throw new ApplicationException(Properties.Resources.ExceptionMessageR);
                    }
                    if (expectingFilePath || expectingArguments || expectingFolderPath || helpSet || thisTestSet) {
                        throw new ApplicationException(Properties.Resources.ExceptionMessageM);
                    }
                    expectingRegFilePath = true;

                    // Allows only one instance.
                } else if (argument.Equals(Constants.CommandLineSwitchUO) || argument.Equals(Constants.CommandLineSwitchWO)) {
                    if (oneInstanceSet || helpSet || thisTestSet || expectingFilePath || expectingArguments || expectingFolderPath
                            || expectingRegFilePath) {

                        throw new ApplicationException(Properties.Resources.ExceptionMessageM);
                    }
                    oneInstanceSet = true;

                    // Will show help.
                } else if (argument.Equals(Constants.CommandLineSwitchUH) || argument.Equals(Constants.CommandLineSwitchWH)
                        || argument.Equals(Constants.CommandLineSwitchUQ) || argument.Equals(Constants.CommandLineSwitchWQ)) {

                    if (filePathSet || argumentsSet || folderPathSet || oneInstanceSet || helpSet || thisTestSet || expectingFilePath
                            || expectingArguments || expectingRegFilePath || expectingFolderPath) {

                        throw new ApplicationException(Properties.Resources.ExceptionMessageM);
                    }
                    helpSet = true;

                    // Test mode (ArgumentParser test).
                } else if (argument.Equals(Constants.CommandLineSwitchUU) || argument.Equals(Constants.CommandLineSwitchWU)) {
                    if (filePathSet || argumentsSet || folderPathSet || oneInstanceSet || helpSet || thisTestSet || expectingFilePath
                            || expectingArguments || expectingRegFilePath || expectingFolderPath) {

                        throw new ApplicationException(Properties.Resources.ExceptionMessageM);
                    }
                    thisTestSet = true;
                } else if (expectingFilePath) {
                    applicationFilePath = argument;
                    expectingFilePath = false;
                    filePathSet = true;
                } else if (expectingArguments) {
                    applicationArguments = argument;
                    expectingArguments = false;
                    argumentsSet = true;
                } else if (expectingFolderPath) {
                    workingFolderPath = argument;
                    expectingFolderPath = false;
                    folderPathSet = true;
                } else if (expectingRegFilePath) {
                    regFilePath = argument;
                    expectingRegFilePath = false;
                    regFilePathSet = true;
                } else if (argument.StartsWith(Constants.Hyphen.ToString()) || argument.StartsWith(Constants.Slash.ToString())) {
                    throw new ApplicationException(Properties.Resources.ExceptionMessageU);
                } else {
                    throw new ApplicationException(Properties.Resources.ExceptionMessageM);
                }
            }
            if (expectingFilePath || expectingArguments || expectingFolderPath || expectingRegFilePath) {
                throw new ApplicationException(Properties.Resources.ExceptionMessageM);
            }
        }

        private void Reset() {
            applicationArguments = string.Empty;
            applicationFilePath = string.Empty;
            argumentsSet = false;
            expectingArguments = false;
            expectingFilePath = false;
            expectingFolderPath = false;
            expectingRegFilePath = false;
            filePathSet = false;
            folderPathSet = false;
            hasArguments = false;
            helpSet = false;
            oneInstanceSet = false;
            regFilePath = string.Empty;
            regFilePathSet = false;
            thisTestSet = false;
            workingFolderPath = string.Empty;
        }

        private static List<string> Parse(string str) {
            char[] c = str.ToCharArray();
            List<string> arguments = new List<string>();
            StringBuilder stringBuilder = new StringBuilder();
            bool e = false, d = false, s = false;
            for (int i = 0; i < c.Length; i++) {
                if (!s) {
                    if (c[i].Equals(Constants.Space)) {
                        continue;
                    }
                    d = c[i].Equals(Constants.QuotationMark);
                    s = true;
                    e = false;
                    if (d) {
                        continue;
                    }
                }
                if (d) {
                    if (c[i].Equals(Constants.BackSlash)) {
                        if (i + 1 < c.Length && c[i + 1].Equals(Constants.QuotationMark)) {
                            stringBuilder.Append(c[++i]);
                        } else {
                            stringBuilder.Append(c[i]);
                        }
                    } else if (c[i].Equals(Constants.QuotationMark)) {
                        if (i + 1 < c.Length && c[i + 1].Equals(Constants.QuotationMark)) {
                            stringBuilder.Append(c[++i]);
                        } else {
                            d = false;
                            e = true;
                        }
                    } else {
                        stringBuilder.Append(c[i]);
                    }
                } else if (s) {
                    if (c[i].Equals(Constants.Space)) {
                        s = false;
                        arguments.Add(e ? stringBuilder.ToString() : stringBuilder.ToString().TrimEnd(Constants.Space));
                        stringBuilder = new StringBuilder();
                    } else if (!e) {
                        stringBuilder.Append(c[i]);
                    }
                }
            }
            if (stringBuilder.Length > 0) {
                arguments.Add(stringBuilder.ToString());
            }
            return arguments;
        }
    }
}
