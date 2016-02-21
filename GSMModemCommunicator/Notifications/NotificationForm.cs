using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GSMModemCommunicator.Notifications
    {
    public partial class NotificationForm : Form
        {
        private static volatile bool showed;
        private Action optionAction;

        public NotificationForm()
            {
            InitializeComponent();
            }

        public static bool Showed
            {
            get { return showed; }
            }

        private void NotificationForm_Load(object sender, EventArgs e)
            {
            showed = true;
            }

        private void NotificationForm_FormClosed(object sender, FormClosedEventArgs e)
            {
            showed = false;
            }

        internal void SetOption(string actionOption, Action optionAction)
            {
            this.optionAction = optionAction;

            if (string.IsNullOrEmpty(actionOption) ||
                optionAction == null)
                {
                optionButton.Hide();
                }
            else
                {
                optionButton.Text = actionOption;
                optionButton.Show();
                }
            }

        private void optionButton_Click(object sender, EventArgs e)
            {
            if (optionAction != null)
                {
                optionAction();
                Close();
                }
            }
        }
    }
