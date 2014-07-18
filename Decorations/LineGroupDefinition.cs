using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace NetCams
{
    /// <summary>
    /// This is intended to define lines that are generated in variable numbers.  e.g. FindEdges tool can create a variable
    /// number of EdgeInstances (lines), so there isn't the normal 1:1 mapping of instance:definition relationship.  This
    /// defines the attributes of all of those created lines.
    /// </summary>
    /// 
    /* COMMENTED OUT "TEMPORARILLY" 4/16/09 SO I CAN COMPILE A NEW TOOL.  I QUIT WORKING ON THIS FINDEDGES STUFF A FEW WEEKS AGO WHEN I GOT STUCK AND RAN OUT OF TIME
    public class LineGroupDefinition : DecorationDefinition
    {
        public LineGroupDefinition(TestSequence testSequence)
            : base(testSequence)
		{
        }

        public override void Dispose_UVision()
        {
            base.Dispose_UVision();
        }

        public Color Color
        {
            get { return mColor; }
            set 
            {
                HandlePropertyChange(this, "Color", mColor, value);
                mColor = value;
            }
        }

        protected Color mColor = System.Drawing.Color.Magenta;
    }
     */
}
