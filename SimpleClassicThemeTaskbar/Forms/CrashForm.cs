using Sentry;

using SimpleClassicThemeTaskbar.Helpers;

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar.Forms
{
    public partial class CrashForm : Form
    {
        private readonly Exception exception;

        public CrashForm()
        {
            InitializeComponent();
        }

        public CrashForm(Exception ex) : this()
        {
            exception = ex;
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (exception == null)
            {
                MessageBox.Show(
                    this,
                    "No report could be sent because the application didn't provide an exception.",
                    "Report failed to send",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            else
            {
                using (SentrySdk.Init(o =>
                {
                    o.Dsn = "https://eadebff4c83e42e4955d85403ff0cc7c@o925637.ingest.sentry.io/5874762";
                    o.MaxBreadcrumbs = 50;
#if DEBUG
                    o.Debug = true;
#endif
                }))
                {
                    SentrySdk.ConfigureScope(scope =>
                    {
                        scope.Level = SentryLevel.Fatal;

                        if (submitLogCheckBox.Checked)
                        {
                            scope.AddAttachment(Logger.FilePath, AttachmentType.Default);
                        }

                        SentrySdk.CaptureException(exception);
                    });
                }
            }
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                MessageBox.Show(
                    this,
                    "Report has been sent successfully.",
                    "Report sent",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    this,
                    "We failed to send the error report. Please check the application logs and explain this occurrance over at GitHub.",
                    "This is embarrassing...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            Close();
        }

        private void CrashForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker.IsBusy)
            {
                e.Cancel = true;
            }
            else
            {
                Environment.Exit(1);
            }
        }

        private void DescriptionLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(this,
                @"The currently known data collected by error reports sent in debug builds is:

• Current application log
    • OS version
    • SCT and application version
    • Depending on the problem, this might also include information about open windows

• Stack trace (information about where the program crashed)

• System Environment
    • OS version (e.g. Microsoft Windows 10.0.22000)
    • .NET runtime version
    • SCT application version
    • Senty SDK version (Error reporting SDK version)
    • Loaded assemblies (i.e. libraries)

• Locale information
    • Calendar type
    • Timezone
    • Locale name (e.g. English, en-US)",
                "Data collected",
                MessageBoxButtons.OK,
                MessageBoxIcon.None);
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            sendButton.Text = "Sending...";

            // Disable UI
            Enabled = false;
            ControlBox = false;

            backgroundWorker.RunWorkerAsync();
        }

        private void viewDetailsButton_Click(object sender, EventArgs e)
        {
            using (var detailsForm = new CrashDetailsForm(exception))
            {
                detailsForm.ShowDialog();
            }
        }
    }
}