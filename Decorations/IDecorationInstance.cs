using System;
using System.Drawing;
using System.Text;

namespace NetCams
{
    public interface IDecorationInstance : IObjectInstance
    {
        void Draw(Graphics g, PictureBoxScale scale);
    }
}
