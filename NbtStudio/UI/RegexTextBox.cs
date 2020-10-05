﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public class RegexTextBox : ConvenienceTextBox
    {
        // cache results of regex parsing, so calling IsMatch repeatedly doesn't pointlessly re-parse
        private Regex LastRegex = null;
        private bool _RegexMode = false;
        public bool RegexMode
        {
            get => _RegexMode;
            set
            {
                _RegexMode = value;
                if (value)
                    SetColor(CheckRegexInternal(out _));
                else
                {
                    RestoreBackColor();
                    HideTooltip();
                }
            }
        }
        public RegexTextBox()
        {
            this.TextChanged += TagNameTextBox_TextChanged;
        }

        private void TagNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (RegexMode)
                SetColor(CheckRegexInternal(out LastRegex));
        }

        private void SetColor(Exception exception)
        {
            if (exception == null)
                RestoreBackColor();
            else
                SetBackColor(Color.FromArgb(255, 230, 230));
        }

        private void ShowTooltip(Exception exception)
        {
            if (exception != null)
            {
                string message = exception.Message;
                string redundant = $"\"{this.Text}\" - ";
                int index = message.IndexOf(redundant);
                if (index != -1)
                    message = message.Substring(index + redundant.Length);
                ShowTooltip("Regex Parsing Error", message, TimeSpan.FromSeconds(3));
            }
        }

        public bool IsMatch(string input)
        {
            if (this.Text == "")
                return true;
            if (input == null)
                return false;
            if (RegexMode)
            {
                if (LastRegex == null)
                    CheckRegexInternal(out LastRegex);
                if (LastRegex == null)
                    return false;
                return LastRegex.IsMatch(input);
            }
            else
                return input.IndexOf(this.Text, StringComparison.OrdinalIgnoreCase) != -1;
        }

        public Regex ReparseRegex()
        {
#if DEBUG
            Console.WriteLine($"Parsing new regex: \"{this.Text}\"");
#endif
            return new Regex(this.Text, RegexOptions.IgnoreCase);
        }

        private Exception CheckRegexInternal(out Regex regex)
        {
            regex = null;
            try
            { regex = ReparseRegex(); }
            catch (Exception ex) { return ex; }
            return null;
        }

        public bool CheckRegex(out Regex regex)
        {
            if (!RegexMode)
            {
                regex = null;
                return true;
            }
            var error = CheckRegexInternal(out regex);
            bool valid = error == null;
            SetColor(error);
            if (!valid)
            {
                ShowTooltip(error);
                this.Select();
            }
            return valid;
        }
    }
}
