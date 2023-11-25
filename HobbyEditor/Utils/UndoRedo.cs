﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public UndoRedoAction(string name)
        {
            Name = name;
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
            cmd.Undo();
            _redoList.Insert(0, cmd);
        }

        public void Redo()
        {
            if (_redoList.Count == 0) return;

            var cmd = _redoList.First();
            _redoList.RemoveAt(0);
            cmd.Redo();
            _undoList.Add(cmd);
        }

        public void Add(IUndoRedo cmd)
        {
            _undoList.Add(cmd);
            _redoList.Clear();
        }
    }
}
