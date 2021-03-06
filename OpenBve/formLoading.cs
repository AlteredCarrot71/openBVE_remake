﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace OpenBve
{
    internal partial class formLoading : Form
    {
        internal formLoading()
        {
            InitializeComponent();
        }

        // members
        private bool AllowClosing = false;
        private bool QueryCancel = false;

        // show loading dialog
        internal static bool ShowLoadingDialog()
        {
            formLoading Dialog = new formLoading();
            DialogResult Result = Dialog.ShowDialog();
            Dialog.Dispose();
            return Result == DialogResult.OK;
        }

        // load
        private void formLoading_Load(object sender, EventArgs e)
        {
            this.MinimumSize = this.Size;
            try
            {
                string f = Interface.GetCombinedFileName(Program.FileSystem.GetDataFolder("Menu"), "banner.png");
                pictureboxBanner.Image = Image.FromFile(f);
            }
            catch { }
            labelRoute.Text = Interface.GetInterfaceString("loading_loading_route");
            labelTrain.Text = Interface.GetInterfaceString("loading_loading_train");
            labelAlmostTitle.Text = Interface.GetInterfaceString("loading_almost");
            labelFilesNotFoundCaption.Text = Interface.GetInterfaceString("loading_almost_filesnotfound");
            buttonIssues.Text = Interface.GetInterfaceString("loading_almost_show");
            labelProblemsTitle.Text = Interface.GetInterfaceString("loading_problems");
            listviewProblems.Columns[0].Text = Interface.GetInterfaceString("loading_problems_type");
            listviewProblems.Columns[1].Text = Interface.GetInterfaceString("loading_problems_description");
            buttonSave.Text = Interface.GetInterfaceString("loading_save");
            buttonIgnore.Text = Interface.GetInterfaceString("loading_ignore");
            buttonCancel.Text = Interface.GetInterfaceString("loading_cancel");
            /* 
			 * For some reasons, the Ignore and Save Report buttons do not show
			 * up later on Mac OS X if initially set to invisible.
			 * */
            if (Program.CurrentPlatform != Program.Platform.Mac)
            {
                buttonIgnore.Visible = false;
                buttonSave.Visible = false;
            }
        }

        // form closing
        private void formLoading_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!AllowClosing)
            {
                e.Cancel = true;
            }
        }

        // update
        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            {
                int a = (int)Math.Floor(100.0 * Loading.RouteProgress);
                if (a < 0) a = 0; else if (a > 100) a = 100;
                labelRoutePercentage.Text = a.ToString(System.Globalization.CultureInfo.InvariantCulture) + "%";
                progressbarRoute.Value = a;
            }
            {
                int a = (int)Math.Floor(100.0 * Loading.TrainProgress);
                if (a < 0) a = 0; else if (a > 100) a = 100;
                labelTrainPercentage.Text = a.ToString(System.Globalization.CultureInfo.InvariantCulture) + "%";
                progressbarTrain.Value = a;
            }
            if (Loading.Complete)
            {
                timerUpdate.Enabled = false;
                AllowClosing = true;
                bool critical = false;
                if (QueryCancel)
                {
                    this.DialogResult = DialogResult.Cancel;
                }
                else if (Interface.MessageCount == 0)
                {
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    int errors = 0;
                    for (int i = 0; i < Interface.MessageCount; i++)
                    {
                        if (Interface.Messages[i].FileNotFound) errors++;
                        if (Interface.Messages[i].Type == Interface.MessageType.Critical) critical = true;
                    }
                    if (errors == 0)
                    {
                        labelFilesNotFoundCaption.Visible = false;
                        labelFilesNotFoundValue.Visible = false;
                        labelHelp.Text = Interface.GetInterfaceString("loading_almost_help_general");
                    }
                    else
                    {
                        labelFilesNotFoundValue.Text = errors.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        labelHelp.Text = Interface.GetInterfaceString("loading_almost_help_filesnotfound");
                    }
                    panelAlmost.Visible = true;
                    panelLoading.Visible = false;
                    if (critical)
                    {
                        ShowMessages();
                        buttonSave.Visible = true;
                        panelProblems.Visible = true;
                        buttonIgnore.Visible = false;
                        panelAlmost.Visible = false;
                    }
                    else
                    {
                        buttonIgnore.Visible = true;
                        buttonIgnore.BringToFront();
                        buttonIgnore.Focus();
                    }
                }
            }
        }

        // save
        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (Loading.Complete)
            {
                // prepare
                System.Text.StringBuilder Builder = new System.Text.StringBuilder();
                for (int i = 0; i < Interface.MessageCount; i++)
                {
                    Builder.AppendLine(Interface.Messages[i].Text);
                }
                // save
                SaveFileDialog Dialog = new SaveFileDialog();
                Dialog.Filter = Interface.GetInterfaceString("dialog_textfiles") + "|*.txt|" + Interface.GetInterfaceString("dialog_allfiles") + "|*";
                if (Dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        System.IO.File.WriteAllText(Dialog.FileName, Builder.ToString(), System.Text.Encoding.UTF8);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                }
            }
        }

        // ignore
        private void buttonIgnore_Click(object sender, EventArgs e)
        {
            if (Loading.Complete)
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        // cancel
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (AllowClosing)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else
            {
                Loading.Cancel = true;
                QueryCancel = true;
                buttonCancel.Enabled = false;
            }
        }

        // issues
        private void buttonIssues_Click(object sender, EventArgs e)
        {
            ShowMessages();
            buttonSave.Visible = true;
            panelProblems.Visible = true;
            panelAlmost.Visible = false;
        }

        // show messages
        private void ShowMessages()
        {
            listviewProblems.SmallImageList = new ImageList();
            string Folder = Program.FileSystem.GetDataFolder("Menu");
            try
            {
                listviewProblems.SmallImageList.Images.Add("information", Image.FromFile(Interface.GetCombinedFileName(Folder, "icon_information.png")));
            }
            catch { }
            try
            {
                listviewProblems.SmallImageList.Images.Add("warning", Image.FromFile(Interface.GetCombinedFileName(Folder, "icon_warning.png")));
            }
            catch { }
            try
            {
                listviewProblems.SmallImageList.Images.Add("error", Image.FromFile(Interface.GetCombinedFileName(Folder, "icon_error.png")));
            }
            catch { }
            try
            {
                listviewProblems.SmallImageList.Images.Add("critical", Image.FromFile(Interface.GetCombinedFileName(Folder, "icon_critical.png")));
            }
            catch { }
            /* 
			 * Show critical errors
			 * */
            for (int i = 0; i < Interface.MessageCount; i++)
            {
                if (Interface.Messages[i].Type == Interface.MessageType.Critical)
                {
                    ListViewItem a = listviewProblems.Items.Add("Critical", "critical");
                    a.SubItems.Add(Interface.Messages[i].Text);
                }
            }
            /*
			 * Show informational messages
			 * */
            for (int i = 0; i < Interface.MessageCount; i++)
            {
                if (Interface.Messages[i].Type == Interface.MessageType.Information)
                {
                    ListViewItem a = listviewProblems.Items.Add("Information", "information");
                    a.SubItems.Add(Interface.Messages[i].Text);
                }
            }
            /*
			 * Show errors
			 * */
            for (int i = 0; i < Interface.MessageCount; i++)
            {
                if (Interface.Messages[i].Type == Interface.MessageType.Error)
                {
                    ListViewItem a = listviewProblems.Items.Add("Error", "error");
                    a.SubItems.Add(Interface.Messages[i].Text);
                }
            }
            /*
			 * Show warnings
			 * */
            for (int i = 0; i < Interface.MessageCount; i++)
            {
                if (Interface.Messages[i].Type == Interface.MessageType.Warning)
                {
                    ListViewItem a = listviewProblems.Items.Add("Warning", "warning");
                    a.SubItems.Add(Interface.Messages[i].Text);
                }
            }
            listviewProblems.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

    }
}