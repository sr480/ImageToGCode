using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ImageToGCode.Engine.Visualisers
{
    interface IRenderable
    {
        DrawingGroup Drawing { get; }
        void Render();
        event EventHandler UpdateNeeded;
    }
}
