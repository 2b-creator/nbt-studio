﻿using fNbt;
using NbtStudio.SNBT;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public partial class UpdateWindow : Form
    {
        private readonly AvailableUpdate AvailableUpdate;

        public UpdateWindow(AvailableUpdate update)
        {
            InitializeComponent();

            AvailableUpdate = update;
            this.Icon = Properties.Resources.app_icon_16;
            CurrentVersionValue.Text = Updater.GetCurrentVersion().ToString(false);
            AvailableVersionValue.Text = update.Version.ToString(false);
            ChangelogBox.Text = update.Changelog;
            ButtonOk.Select();
        }

        private void Confirm()
        {
            if (TryUpdate())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private bool TryUpdate()
        {
            try
            {
                var path = Application.ExecutablePath;
                AvailableUpdate.Update();
                Process.Start(path);
                Application.Exit();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(Util.ExceptionMessage(ex), "Update failed!");
                return false;
            }
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            Confirm();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UpdateWindow_Load(object sender, EventArgs e)
        {
            CenterToParent();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}