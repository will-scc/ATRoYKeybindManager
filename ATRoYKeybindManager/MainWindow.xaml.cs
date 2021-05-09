using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ATRoYKeybindManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private static readonly Regex regex = new Regex("^[a-zA-Z]+$");

        Spell[] Spells { get; set; } = new Spell[20];

        #region "Spells"
        public Spell FireBall { get; set; } = new Spell(1, "1", "Fireball");
        public Spell LightningBall { get; set; } = new Spell(2, "2", "Lightningball");
        public Spell Flash { get; set; } = new Spell(3, "3", "Flash");
        public Spell Freeze { get; set; } = new Spell(4, "4", "Freeze");
        public Spell Shield { get; set; } = new Spell(5, "5", "Shield");
        public Spell Bless { get; set; } = new Spell(6, "6", "Bless");
        public Spell Heal { get; set; } = new Spell(7, "7", "Heal");
        public Spell WarCry { get; set; } = new Spell(8, "8", "Warcry");
        public Spell Pulse { get; set; } = new Spell(9, "9", "Pulse");
        public Spell FireRing { get; set; } = new Spell(10, "10", "Fire Ring");
        public Spell Special { get; set; } = new Spell(11, "Q", "Special");
        public Spell MapSpell { get; set; } = new Spell(12, "W", "Map Spell");
        public Spell TargetSpell { get; set; } = new Spell(13, "E", "Target Spell");
        public Spell AoeSpell { get; set; } = new Spell(14, "R", "AoE Spell");
        public Spell Barrier { get; set; } = new Spell(15, "T", "Barrier");
        public Spell Curse { get; set; } = new Spell(16, "Y", "Curse");
        public Spell BuffSelf { get; set; } = new Spell(17, "U", "Buff Self");
        public Spell BuffOther { get; set; } = new Spell(18, "I", "Buff Other");
        public Spell Dispel { get; set; } = new Spell(19, "O", "Dispel");
        public Spell AoeDamage { get; set; } = new Spell(20, "P", "AoE Damage");
#endregion
        
        public MainWindow()
        {
            InitializeComponent();

            Spells[FireBall.Number - 1] = FireBall;
            Spells[LightningBall.Number - 1] = LightningBall;
            Spells[Flash.Number - 1] = Flash;
            Spells[Freeze.Number - 1] = Freeze;
            Spells[Shield.Number - 1] = Shield;
            Spells[Bless.Number - 1] = Bless;
            Spells[Heal.Number - 1] = Heal;
            Spells[WarCry.Number - 1] = WarCry;
            Spells[Pulse.Number - 1] = Pulse;
            Spells[FireRing.Number - 1] = FireRing;
            Spells[Special.Number - 1] = Special;
            Spells[MapSpell.Number - 1] = MapSpell;
            Spells[TargetSpell.Number - 1] = TargetSpell;
            Spells[AoeSpell.Number - 1] = AoeSpell;
            Spells[Barrier.Number - 1] = Barrier;
            Spells[Curse.Number - 1] = Curse;
            Spells[BuffSelf.Number - 1] = BuffSelf;
            Spells[BuffOther.Number - 1] = BuffOther;
            Spells[Dispel.Number - 1] = Dispel;
            Spells[AoeDamage.Number - 1] = AoeDamage;
            
            LoadCurrentConfig();
            CheckConflicts();
        }

        private void LoadCurrentConfig()
        {
            try
            {
                string[] lines = File.ReadAllLines("conflict.cfg");
                string UserKeysHex = lines.FirstOrDefault(x => x.StartsWith("user_keys"));
                string UserKeys = ConvertFromHex(UserKeysHex[(UserKeysHex.IndexOf("user_keys=") + 10)..]);

                for (int i = 0; i < UserKeys.Length - 2; i++)
                {
                    Spells[i].UserKey = UserKeys[i].ToString();
                }
            } 
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to load conflict.cfg file:" +
                                $"{Environment.NewLine}{Environment.NewLine}" +
                                $"{ex.Message}", 
                                "Load Config", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Error);
                Close();
            }
        }

        private void SaveCurrentConfig()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < Spells.Length; i++)
            {
                sb.Append(Spells[i].UserKey);
            }
            sb.Append("OP"); //OP are two unused default keys

            string hex = ConvertToHex(sb.ToString());
            
            string user_keys = "user_keys=" + hex;

            string[] lines = File.ReadAllLines("conflict.cfg");
            using (StreamWriter writer = new StreamWriter("conflict.cfg"))
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("user_keys"))
                    {
                        writer.WriteLine(user_keys);
                    }
                    else
                    {
                        writer.WriteLine(lines[i]);
                    }
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

            if (!CheckConflicts())
            {
                SaveCurrentConfig();
            }
            else
            {
                MessageBox.Show("Conflicts detected. Please review");
            }
        }

        private bool CheckConflicts()
        {
            bool anyConflicts = false;

            for(int i = 0; i < Spells.Length; i++)
            {
                bool spellConflict = false;
                for(int j = 0; j < Spells.Length; j++)
                {
                    if (Spells[i].Number != Spells[j].Number)
                    {
                        if (Spells[i].UserKey == Spells[j].UserKey ||
                            Spells[i].UserKey == Spells[j].FixedKey)
                        {
                            spellConflict = true;
                            anyConflicts = true;
                        }
                    }
                }
                Spells[i].Conflict = spellConflict;
            }

            return anyConflicts;
        }

        private string ConvertFromHex(string str)
        {
            StringBuilder sb = new StringBuilder();
            
            for (int i = 0; i < str.Length; i += 2)
            {
                sb.Append(Convert.ToChar(Convert.ToUInt32(str.Substring(i, 2), 16)));
            }

            return sb.ToString();
        }

        private string ConvertToHex(string str)
        {
            var sb = new StringBuilder();

            foreach (char c in str)
            {
                sb.Append(Convert.ToInt32(c).ToString("X"));
            }

            return sb.ToString();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            Spell spell = Spells.FirstOrDefault(x => x.UserKey == tb.Text);

            if (spell != null)
            {
                if (spell.Number >= 1 && spell.Number <= 10)
                {
                    if ("QWERTYUIOP".Contains(tb.Text))
                    {
                        Spell conflictingSpell = Spells.FirstOrDefault(x => x.FixedKey == tb.Text);
                        if (conflictingSpell != null)
                        {
                            MessageBox.Show($"{tb.Text} is the default key for {conflictingSpell.Label}." +
                                            $"{Environment.NewLine}{Environment.NewLine}" +
                                            $"It is not recommended to use Q-P for 1-10 spells.");
                        }
                    }
                }
            }

            CheckConflicts();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
            base.OnPreviewTextInput(e);
        }
    }
}
