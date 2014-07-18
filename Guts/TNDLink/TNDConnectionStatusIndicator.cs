using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NetCams
{
    public class TNDConnectionStatusIndicator : Button
    {
        public TNDConnectionStatusIndicator(TNDLink theConnectionObject)
        {
            mTNDLink = theConnectionObject;
            mTNDLink.TNDLinkEstablished += new TNDLink.TNDLinkEstablishedDelegate(TNDLinkEstablished);
            mTNDLink.TNDLinkDisabled += new TNDLink.TNDLinkDisabledDelegate(TNDLinkDisabled);
            mTNDLink.TNDLinkLost += new TNDLink.TNDLinkLostDelegate(TNDLinkLost);
            this.Click += new EventHandler(OnClick);

            this.AutoSize = true;

            TNDLinkDisabled(mTNDLink);
        }

        void OnClick(object sender, EventArgs e)
        {
            mTNDLink.Connected = true;
        }

        private TNDLink mTNDLink;

        void TNDLinkLost(TNDLink theLink)
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new TNDLink.TNDLinkLostDelegate(TNDLinkLost), new object[] { theLink });
                return;
            }
            this.Text = mTNDLink.Name + " DOWN";
            this.BackColor = Color.Red;
            this.Visible = true;
        }

        void TNDLinkDisabled(TNDLink theLink)
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new TNDLink.TNDLinkDisabledDelegate(TNDLinkDisabled), new object[] { theLink });
                return;
            }
            this.Text = mTNDLink.Name + " disabled";
            this.BackColor = Color.Yellow;
            this.Visible = true;
        }

        void TNDLinkEstablished(TNDLink theLink)
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new TNDLink.TNDLinkEstablishedDelegate(TNDLinkEstablished), new object[] { theLink });
                return;
            }
            this.Text = mTNDLink.Name + " connected";
            this.BackColor = Color.Green;
            this.Visible = false;
        }

    }
}
