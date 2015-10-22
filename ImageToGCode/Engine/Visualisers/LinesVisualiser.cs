using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToGCode.Engine.Visualisers
{
    class LinesVisualiser
    {
        private readonly ImageByLinesPresenter _Presenter;
        private readonly ObservableCollection<VisualLine> _Lines;

        public ObservableCollection<VisualLine> Lines { get { return _Lines; } }

        public LinesVisualiser(ImageByLinesPresenter presenter)
        {
            _Presenter = presenter;
            _Lines = new ObservableCollection<VisualLine>();
        }
        public void Visualise()
        {
            Lines.Clear();
            int lpos = 0;
            int lSimpl = GetSimplificationK();
            foreach (var line in _Presenter.Lines)
            {
                lpos++;
                if (lpos % lSimpl != 0)
                    continue;

                if (line.Pixels.Count < 2)
                    continue;
                Pixel start = line.Pixels[0];
                for (int i = 1; i < line.Pixels.Count - 1; i++)
                {
                    Pixel current = line.Pixels[i];
                    if (start.Intensity != current.Intensity)
                    {
                        Lines.Add(new VisualLine(start, current, 1 - start.Intensity));
                        start = current;
                    }
                }
                Lines.Add(new VisualLine(start, line.Pixels[line.Pixels.Count - 1], 1 - start.Intensity));
            }
        }
        private int GetSimplificationK()
        {
            if (_Presenter.LineResolution < 4)
                return (int)(4/_Presenter.LineResolution);
            return 1;
        }

        //public Bitmap Render()
        //{
        //    //Bitmap bm = new Bitmap(_Presenter.Image.Width, _Presenter.Image.Height);
        //    //Graphics gr = Graphics.FromImage(bm);
        //    //gr.InterpolationMode = InterpolationMode.Bilinear;
        //    //gr.CompositingQuality = CompositingQuality.HighQuality;
        //    //gr.SmoothingMode = SmoothingMode.AntiAlias;

        //    //Visualise();

        //    //foreach(var ln in Lines)
        //    //{
        //    //    Point p1 = new Point((int)Math.Round(ln.V1.X), bm.Height - (int)Math.Round(ln.V1.Y) - 1);
        //    //    Point p2 = new Point((int)Math.Round(ln.V2.X), bm.Height - (int)Math.Round(ln.V2.Y) - 1);
        //    //    byte gcIntence = (byte)(255 - 255 * ln.Intensity);
        //    //    gr.DrawLine(new Pen(new SolidBrush(Color.FromArgb(gcIntence, gcIntence, gcIntence)), 1), p1, p2);
        //    //}

        //    //gr.Dispose();
        //    //return bm;
        //}
    }
}
