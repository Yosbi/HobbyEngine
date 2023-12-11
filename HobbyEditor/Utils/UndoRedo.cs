using System.Collections.ObjectModel;
using System.Diagnostics;

namespace HobbyEditor.Utils
{
    public interface IUndoRedo
    {
        string Name { get; }
        void Undo();
        void Redo();
    }

    public class UndoRedoAction : IUndoRedo
    {
        public string Name { get; private set; }
        private Action _undoAction;
        private Action _redoAction;

        public UndoRedoAction(string name, string property, object instance, object undoValue, object redoValue)
        {
            Name = name;

            _undoAction = () => instance.GetType().GetProperty(property)?.SetValue(instance, undoValue);
            _redoAction = () => instance.GetType().GetProperty(property)?.SetValue(instance, redoValue);
        }

        public UndoRedoAction(string name, Action undo, Action redo)
        {
            Name = name;

            Debug.Assert(undo != null && redo != null);
            _undoAction = undo;
            _redoAction = redo;
        }

        public void Undo()
        {
            _undoAction();
        }

        public void Redo()
        {
            _redoAction();
        }
    }   

    public class UndoRedo
    {
        private readonly ObservableCollection<IUndoRedo> _redoList = new ObservableCollection<IUndoRedo>();
        private readonly ObservableCollection<IUndoRedo> _undoList = new ObservableCollection<IUndoRedo>();
        public ReadOnlyObservableCollection<IUndoRedo> RedoList { get; private set; }
        public ReadOnlyObservableCollection<IUndoRedo> UndoList { get; private set; }

        private bool _enableAdd = true;

        public UndoRedo()
        {
            RedoList = new ReadOnlyObservableCollection<IUndoRedo>(_redoList);
            UndoList = new ReadOnlyObservableCollection<IUndoRedo>(_undoList);
        }

        public void Reset()
        {
            _redoList.Clear();
            _undoList.Clear();
        }

        public void Undo()
        {
            if (_undoList.Count == 0) return;

            var cmd = _undoList.Last();
            _undoList.RemoveAt(_undoList.Count - 1);
            _enableAdd = false;
            cmd.Undo();
            _enableAdd = true;
            _redoList.Insert(0, cmd);
        }

        public void Redo()
        {
            if (_redoList.Count == 0) return;

            var cmd = _redoList.First();
            _redoList.RemoveAt(0);
            _enableAdd = false;
            cmd.Redo();
            _enableAdd = true;
            _undoList.Add(cmd);
        }

        public void Add(IUndoRedo cmd)
        {
            if (!_enableAdd) return;

            _undoList.Add(cmd);
            _redoList.Clear();
        }
    }
}
