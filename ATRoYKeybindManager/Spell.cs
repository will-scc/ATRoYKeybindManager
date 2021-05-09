using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ATRoYKeybindManager
{
    public class Spell : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Spell(int Number, string FixedKey, string Label)
        {
            this.Number = Number;
            this.FixedKey = FixedKey;
            this.Label = Label;
        }

        public int Number { get; }
        public string FixedKey { get; }
        public string Label { get; }
        public string UserKey { get; set; }
        public bool Conflict { get; set; } = false;
    }
}
