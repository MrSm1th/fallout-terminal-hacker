using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassCracker
{
    public class PasswordEntry
    {
        public string Text { get; set; }
        public int[] Matches { get; set; }

        public int Rank
        {
            get { return Matches.Where(m => m > 0).Count(); }
        }

        public PasswordEntry(string text): this(text, new int[0]) { }
        public PasswordEntry(string text, int[] matches)
        {
            Text = text;
            Matches = matches;
        }

        public override string ToString()
        {
            return string.Format("{0:00} {1} [{2}]", Rank, Text, string.Join(" ", Matches));
        }
    }

    public class Model
    {
        private List<string> _textEntries;
        private Dictionary<string, int[]> _entries;
        public IEnumerable<PasswordEntry> Entries
        {
            get
            {
                return _entries
                    .Select(e => new PasswordEntry(e.Key, e.Value))
                    .OrderByDescending(e => e.Rank)
                    .ThenBy(e => e.Text);
            }
        }

        public Model()
        {
            _entries = new Dictionary<string, int[]>();
            _textEntries = new List<string>();
        }

        public void Add(string entry)
        {
            if (!_entries.ContainsKey(entry))
            {
                _entries.Add(entry, null);
                _textEntries.Add(entry);
                UpdateEntriesMatches();
            }
        }

        public void Remove(string entry)
        {
            _entries.Remove(entry);
            _textEntries.Remove(entry);
            UpdateEntriesMatches();
        }

        public void Clear()
        {
            _textEntries.Clear();
            _entries.Clear();
        }

        public void Filter(string entry, int matches)
        {
            if (!_entries.ContainsKey(entry)) return;

            _entries.Remove(entry);
            foreach (var e in _entries.Keys.ToArray())
            {
                if (Compare(e, entry) != matches)
                    _entries.Remove(e);
            }
            UpdateEntriesMatches();
        }

        public void ClearFilter()
        {
            var tmpCopy = _textEntries.ToList();
            _textEntries.Clear();
            _entries.Clear();
            tmpCopy.ForEach(e => Add(e));
        }


        private void UpdateEntriesMatches()
        {
            foreach (var e in _entries.Keys.ToArray())
            {
                _entries[e] = CompareToOthers(e);
            }
        }

        private int[] CompareToOthers(string variant)
        {
            var matches = new int[variant.Length];
            foreach (var v in _entries.Keys)
            {
                if (v.Equals(variant)) continue;

                var match = Compare(v, variant);
                matches[match]++;
            }

            return matches.Skip(1).ToArray();
        }

        private int Compare(string s1, string s2)
        {
            int n = 0;
            for (int i = 0; i < s1.Length && i < s2.Length; i++)
            {
                if (s1[i] == s2[i]) n++;
            }

            return n;
        }
    }
}
