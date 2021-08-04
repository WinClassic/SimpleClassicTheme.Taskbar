using SimpleClassicTheme.Common.Logging;
using SimpleClassicTheme.Taskbar.Helpers;

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SimpleClassicTheme.Taskbar.Forms
{
    public partial class CrashDetailsForm : Form
    {
        private readonly Exception exception;

        public CrashDetailsForm(Exception ex = null)
        {
            InitializeComponent();
            exception = ex;
        }

        private void CrashDetailsForm_Load(object sender, EventArgs e)
        {
            try
            {
                SetFonts();
            }
            catch
            {
            }

            exceptionTextBox.Text = GetException();
            logTextBox.Text = GetLog();
        }

        private string GetException()
        {
            if (exception == null)
            {
                return "Exception was null.";
            }

            return exception.ToString();
        }

        private string GetLog()
        {
            try
            {
                var filePath = Logger.Instance.FilePath;

                if (filePath == null)
                {
                    return "File path provided by logger was null";
                }

                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }
                else
                {
                    return "Log file doesn't exist";
                }
            }
            catch (Exception ex)
            {
                return "Failed to retrieve log file:" + Environment.NewLine + ex.Message;
            }
        }

        private void SetFonts()
        {
            var monospaceFontFamily = FontFamily.GenericMonospace;
            var monospaceFont = new Font(monospaceFontFamily, Font.Size, Font.Unit);

            exceptionTextBox.Font = monospaceFont;
            logTextBox.Font = monospaceFont;
        }
    }
}