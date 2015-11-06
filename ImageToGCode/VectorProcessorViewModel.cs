using ImageToGCode.Engine.GCodeGeneration;
using ImageToGCode.Engine.GCodeGeneration.VectorProcessor;
using ImageToGCode.Tools;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace ImageToGCode
{
    class VectorProcessorViewModel : INotifyPropertyChanged
    {
        private VPathGroup _SelectedGroup;
        public ObservableCollection<Engine.GCodeGeneration.VectorProcessor.VPathGroup> PathGroups { get; private set; }
        public Command OpenSvg { get; private set; }
        public Command MoveUp { get; private set; }
        public Command MoveDown { get; private set; }
        public VPathGroup SelectedGroup
        {
            get
            {
                return _SelectedGroup;
            }
            set
            {
                if (_SelectedGroup == value)
                    return;
                _SelectedGroup = value;
                RaisePropertyChanged("SelectedGroup");
                MoveUp.RaiseCanExecuteChanged();
                MoveDown.RaiseCanExecuteChanged();
            }
        }
        public VectorProcessorViewModel()
        {
            PathGroups = new ObservableCollection<Engine.GCodeGeneration.VectorProcessor.VPathGroup>();
            OpenSvg = new Command((x) => OpenSvgAction(), (x) => true);
            MoveUp = new Command((x) => MoveUpAction(), (x) => SelectedGroup != null);
            MoveDown = new Command((x) => MoveDownAction(), (x) => SelectedGroup != null);
        }

        private void OpenSvgAction()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "SVG file (*.svg)|*.svg";
            if (openFileDialog.ShowDialog() == true)
            {
                //try { 
                PathGroups.Clear();
                var doc = Svg.SvgDocument.Open(openFileDialog.FileName);

                var gcg = new VPathGroupSVGGenerator(doc);
                foreach (var vPath in gcg.GenerateVPathGroups())
                {
                    PathGroups.Add(vPath);
                }
                //}
                //catch (Exception e)
                //{
                //    System.Windows.MessageBox.Show("Ошибка открытия файла", "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                //}
            }
        }

        private void MoveUpAction()
        {
            if (SelectedGroup == null)
                throw new Exception("SelectedGroup is null");

            //var groupToMove = SelectedGroup;
            var idx = PathGroups.IndexOf(SelectedGroup);
            if (idx <= 0)
                return;

            PathGroups.Move(idx, idx - 1);
        }
        private void MoveDownAction()
        {
            if (SelectedGroup == null)
                throw new Exception("SelectedGroup is null");

            //var groupToMove = SelectedGroup;
            var idx = PathGroups.IndexOf(SelectedGroup);
            if (idx >= PathGroups.Count - 1)
                return;

            PathGroups.Move(idx, idx + 1);
        }
        public IEnumerable<BaseGCode> Generate()
        {
            yield return new BaseGCode("G21");
            yield return new BaseGCode("G90");
            yield return new BaseGCode("M3 S0");

            foreach (var grp in PathGroups)
                foreach (var gc in grp.Generate())
                    yield return gc;

            yield return new BaseGCode("M5");
            yield return new BaseGCode("%");
        }
        #region IPropertyChanged
        private SynchronizationContext _synchronizationContext = SynchronizationContext.Current;
        private void OnPropertyChanged(string propertyName)
        {
            if (SynchronizationContext.Current != _synchronizationContext)
                RaisePropertyChanged(propertyName);
            else
                _synchronizationContext.Post(RaisePropertyChanged, propertyName);
        }
        private void RaisePropertyChanged(object param)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs((string)param));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
