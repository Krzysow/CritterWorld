using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AFunnyNamespace
{
    public partial class BlankSettings : Form
    {
        Blank critter;

        public BlankSettings(Blank blank)
        {
            critter = blank;

            InitializeComponent();

            trackBarNominalspeed.Value = critter.speed;
            labelNominalSpeedShown.Text = trackBarNominalspeed.Value.ToString();
        }

        private void TrackBarNominalSpeed_ValueChanged(object sender, EventArgs e)
        {
            labelNominalSpeedShown.Text = trackBarNominalspeed.Value.ToString();
        }
        
        private void BtnOk_Click(object sender, EventArgs e)
        {
            critter.speed = int.Parse(labelNominalSpeedShown.Text);
            critter.SaveSettings();
            Dispose();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
