using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PassCracker
{
    public class MainViewModel: INotifyPropertyChanged
    {
        #region Fields
        
        private Model Model;

        #endregion


        #region Properties

        public IEnumerable<PasswordEntry> Passwords
        {
            get
            {
                var e = Model.Entries;
                TotalEntries = e.Count();
                OnPropertyChanged("TotalEntries");
                return e;
            }
        }
        public int TotalEntries { get; set; }

        private PasswordEntry _selectedEntry;
        public PasswordEntry SelectedEntry
        {
            get { return _selectedEntry; }
            set
            {
                if (_selectedEntry != value)
                {
                    _selectedEntry = value;
                    OnPropertyChanged("SelectedEntry");
                }
            }
        }

        private string _tbPassword;
        public string TbPassword
        {
            get { return _tbPassword; }
            set
            {
                if (_tbPassword != value)
                {
                    _tbPassword = value;
                    OnPropertyChanged("TbPassword");
                }
            }
        }

        private int _filterMatches;
        public int FilterMatches
        {
            get { return _filterMatches; }
            set { _filterMatches = value; }
        }


        #endregion


        #region Commands

        public ICommand Add { get; set; }
        public ICommand Remove { get; set; }
        public ICommand RemoveAll { get; set; }
        public ICommand Filter { get; set; }
        public ICommand Clear { get; set; }

        #endregion


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion


        public MainViewModel()
        {
            Model = new PassCracker.Model();

            Add = new Command(AddEntry);
            Remove = new Command<IEnumerable>(RemoveEntries);
            RemoveAll = new Command(RemoveAllEntries);
            Filter = new Command(FilterEntries, () => SelectedEntry != null);
            Clear = new Command(ClearFilter);

            //AddTmpData();
        }

        //private void AddTmpData()
        //{
        //    //var tmp = "observation,interesting,temperature,combination,desperately,examination,infestation,immediately,influential,eliminating,undrinkable,explanation,destructive,unimportant,radioactive,agriculture,recognition,manufacture";
        //    //var tmp = "permanently beckoningly hospitality descendants opportunity persecution examination destructive recognition devastation combination personality reluctantly eliminating destruction reprimanded observation partitioned";
        //    var tmp = "mushroom resemble resulted distance leverage describe absolute decorate destroys assembly generate obsessed";
        //    foreach (var i in tmp.Split(' '))
        //    {
        //        TbPassword = i;
        //        AddEntry();
        //    }
        //}



        private void AddEntry()
        {
            if (string.IsNullOrWhiteSpace(TbPassword)) return;

            Model.Add(TbPassword);
            TbPassword = string.Empty;

            OnPropertyChanged("Passwords");
        }

        private void RemoveEntries(IEnumerable items)
        {
            if (items == null) return;

            new List<PasswordEntry>(items.Cast<PasswordEntry>())
                .ForEach(entry => Model.Remove(entry.Text));

            OnPropertyChanged("Passwords");
        }

        private void RemoveAllEntries()
        {
            Model.Clear();
            OnPropertyChanged("Passwords");
        }

        private void FilterEntries()
        {
            string matchEntry = SelectedEntry.Text;
            int filterMatches = FilterMatches;

            Model.Filter(matchEntry, filterMatches);

            OnPropertyChanged("Passwords");
        }

        private void ClearFilter()
        {
            Model.ClearFilter();

            OnPropertyChanged("Passwords");
        }
    }


    public class Command : ICommand
    {
        private Action _action;
        private Func<bool> _canExecute;

        public Command(Action action) : this(action, true) { }
        public Command(Action action, bool canExecute) : this(action, () => true) { }
        public Command(Action action, Func<bool> canExecute)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (canExecute == null) throw new ArgumentNullException("canExecute");

            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _action();
        }
    }

    public class Command<T> : ICommand
    {
        private Action<T> _action;
        private Func<bool> _canExecute;

        public Command(Action<T> action) : this(action, true) { }
        public Command(Action<T> action, bool canExecute): this(action, () => true) { }
        public Command(Action<T> action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }


        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            if (parameter is T)
                _action((T)parameter);
        }
    }
}
