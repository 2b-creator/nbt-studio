﻿namespace NbtStudio.UI
{
    partial class EditTagWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.NameLabel = new System.Windows.Forms.Label();
            this.NameBox = new NbtStudio.UI.TagNameTextBox();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.ButtonOk = new System.Windows.Forms.Button();
            this.ValueLabel = new System.Windows.Forms.Label();
            this.ValueBox = new NbtStudio.UI.TagValueTextBox();
            this.MainTable = new System.Windows.Forms.TableLayoutPanel();
            this.ButtonsPanel = new System.Windows.Forms.Panel();
            this.MainTable.SuspendLayout();
            this.ButtonsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.NameLabel.Location = new System.Drawing.Point(10, 10);
            this.NameLabel.Margin = new System.Windows.Forms.Padding(10, 10, 5, 0);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(44, 15);
            this.NameLabel.TabIndex = 1;
            this.NameLabel.Text = "Name:";
            // 
            // NameBox
            // 
            this.NameBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.NameBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.NameBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.NameBox.Location = new System.Drawing.Point(64, 10);
            this.NameBox.Margin = new System.Windows.Forms.Padding(5, 10, 10, 0);
            this.NameBox.Name = "NameBox";
            this.NameBox.Size = new System.Drawing.Size(209, 21);
            this.NameBox.TabIndex = 2;
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.ButtonCancel.Location = new System.Drawing.Point(93, 9);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancel.TabIndex = 7;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // ButtonOk
            // 
            this.ButtonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.ButtonOk.Location = new System.Drawing.Point(12, 9);
            this.ButtonOk.Name = "ButtonOk";
            this.ButtonOk.Size = new System.Drawing.Size(75, 23);
            this.ButtonOk.TabIndex = 6;
            this.ButtonOk.Text = "OK";
            this.ButtonOk.UseVisualStyleBackColor = true;
            this.ButtonOk.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // ValueLabel
            // 
            this.ValueLabel.AutoSize = true;
            this.ValueLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.ValueLabel.Location = new System.Drawing.Point(10, 41);
            this.ValueLabel.Margin = new System.Windows.Forms.Padding(10, 10, 5, 0);
            this.ValueLabel.Name = "ValueLabel";
            this.ValueLabel.Size = new System.Drawing.Size(41, 15);
            this.ValueLabel.TabIndex = 3;
            this.ValueLabel.Text = "Value:";
            // 
            // ValueBox
            // 
            this.ValueBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.ValueBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.ValueBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.ValueBox.Location = new System.Drawing.Point(64, 41);
            this.ValueBox.Margin = new System.Windows.Forms.Padding(5, 10, 10, 0);
            this.ValueBox.Name = "ValueBox";
            this.ValueBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ValueBox.Size = new System.Drawing.Size(209, 21);
            this.ValueBox.TabIndex = 4;
            // 
            // MainTable
            // 
            this.MainTable.AutoSize = true;
            this.MainTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MainTable.ColumnCount = 2;
            this.MainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.MainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.MainTable.Controls.Add(this.NameLabel, 0, 0);
            this.MainTable.Controls.Add(this.NameBox, 1, 0);
            this.MainTable.Controls.Add(this.ValueLabel, 0, 1);
            this.MainTable.Controls.Add(this.ButtonsPanel, 1, 2);
            this.MainTable.Controls.Add(this.ValueBox, 1, 1);
            this.MainTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTable.Location = new System.Drawing.Point(0, 0);
            this.MainTable.Name = "MainTable";
            this.MainTable.RowCount = 3;
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainTable.Size = new System.Drawing.Size(463, 185);
            this.MainTable.TabIndex = 0;
            // 
            // ButtonsPanel
            // 
            this.ButtonsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ButtonsPanel.Controls.Add(this.ButtonOk);
            this.ButtonsPanel.Controls.Add(this.ButtonCancel);
            this.ButtonsPanel.Location = new System.Drawing.Point(283, 141);
            this.ButtonsPanel.Name = "ButtonsPanel";
            this.ButtonsPanel.Size = new System.Drawing.Size(177, 41);
            this.ButtonsPanel.TabIndex = 5;
            // 
            // EditTagWindow
            // 
            this.AcceptButton = this.ButtonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(463, 185);
            this.Controls.Add(this.MainTable);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditTagWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create Tag";
            this.MainTable.ResumeLayout(false);
            this.MainTable.PerformLayout();
            this.ButtonsPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label NameLabel;
        private TagNameTextBox NameBox;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button ButtonOk;
        private System.Windows.Forms.Label ValueLabel;
        private TagValueTextBox ValueBox;
        private System.Windows.Forms.TableLayoutPanel MainTable;
        private System.Windows.Forms.Panel ButtonsPanel;
    }
}