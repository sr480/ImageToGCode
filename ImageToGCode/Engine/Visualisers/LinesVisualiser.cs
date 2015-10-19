using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            foreach (var line in _Presenter.Lines)
            {
                if (line.Pixels.Count < 2)
                    continue;
                Pixel start = line.Pixels[0];
                for(int i = 1; i < line.Pixels.Count - 1; i++)
                {
                    Pixel current = line.Pixels[i];
                    if (start.Intensity != current.Intensity)
                    {
                        Lines.Add(new VisualLine(start, current, 1-start.Intensity));
                        start = current;
                    }
                }
                Lines.Add(new VisualLine(start, line.Pixels[line.Pixels.Count - 1], 1 - start.Intensity));
            }
        }
    }
}
