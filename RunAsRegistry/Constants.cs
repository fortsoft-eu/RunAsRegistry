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

namespace RunAsRegistry {

    /// <summary>
    /// Constants used in many places in the application.
    /// </summary>
    internal static class Constants {

        /// <summary>
        /// The default width of the old AboutForm.
        /// </summary>
        public const int DefaultAboutFormWidth = 420;

        /// <summary>
        /// Windows API constants.
        /// </summary>
        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;
        public const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        public const int MOUSEEVENTF_RIGHTUP = 0x10;
        public const int SC_CLOSE = 0xF060;

        /// <summary>
        /// Characters used in many places in the application code.
        /// </summary>
        public const char BackSlash = '\\';
        public const char CarriageReturn = '\r';
        public const char Colon = ':';
        public const char EnDash = '–';
        public const char Hyphen = '-';
        public const char LineFeed = '\n';
        public const char QuotationMark = '"';
        public const char Slash = '/';
        public const char Space = ' ';
        public const char VerticalTab = '\t';
        public const char Zero = '0';

        /// <summary>
        /// Strings used in many places in the application code.
        /// </summary>
        public const string CommandLineSwitchUA = "-a";
        public const string CommandLineSwitchUH = "-h";
        public const string CommandLineSwitchUI = "-i";
        public const string CommandLineSwitchUO = "-o";
        public const string CommandLineSwitchUQ = "-?";
        public const string CommandLineSwitchUR = "-r";
        public const string CommandLineSwitchUU = "-T";
        public const string CommandLineSwitchUW = "-w";
        public const string CommandLineSwitchWA = "/a";
        public const string CommandLineSwitchWH = "/h";
        public const string CommandLineSwitchWI = "/i";
        public const string CommandLineSwitchWO = "/o";
        public const string CommandLineSwitchWQ = "/?";
        public const string CommandLineSwitchWR = "/r";
        public const string CommandLineSwitchWS = "/s";
        public const string CommandLineSwitchWU = "/T";
        public const string CommandLineSwitchWW = "/w";
        public const string ErrorLogEmptyString = "[Empty String]";
        public const string ErrorLogErrorMessage = "ERROR MESSAGE";
        public const string ErrorLogFileName = "Error.log";
        public const string ErrorLogNull = "[null]";
        public const string ErrorLogTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
        public const string ErrorLogWhiteSpace = "[White Space]";
        public const string ExampleApplicationArguments1 = "msiexec /i \"C:\\Program Files\\Example Application\\example.msi\" INST";
        public const string ExampleApplicationArguments2 = "ALLLEVEL=3 /l* msi.log PROPERTY=\"Embedded \"\"Quotes\"\" White Space\"";
        public const string ExampleApplicationFilePath = "C:\\Program Files\\Example Application\\example.exe";
        public const string ExampleRegFilePath = "C:\\Program Files\\Example Application\\example.reg";
        public const string ExampleWorkingFolderPath = "C:\\Program Files\\Example Application";
        public const string ExtensionExe = ".exe";
        public const string ExtensionLnk = ".lnk";
        public const string ExtensionReg = ".reg";
        public const string NotepadExeFileName = "notepad.exe";
        public const string RegeditExeFileName = "regedit.exe";
    }
}
