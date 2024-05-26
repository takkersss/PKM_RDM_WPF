using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKM_RDM_WPF.model
{
    public class Move
    {
        private string name;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private int? accuracy;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private int? power;
        private int pp;
        private int priority;
        private NameUrl damage_class;
        private NameUrl type;

        public Move() { }
        public Move(string name, int? accuracy, int? power, int pp, int priority, NameUrl damage_class)
        {
            this.Name = name;
            this.Accuracy = accuracy;
            this.Power = power;
            this.Pp = pp;
            this.Priority = priority;
            this.Damage_class = damage_class;
        }

        public string Name { get => name; set => name = value; }
        public int? Accuracy { get => accuracy; set => accuracy = value; }
        public int? Power { get => power; set => power = value; }
        public int Pp { get => pp; set => pp = value; }
        public int Priority { get => priority; set => priority = value; }
        public NameUrl Damage_class { get => damage_class; set => damage_class = value; }
        public NameUrl Type { get => type; set => type = value; }

        public override string? ToString()
        {
            return utils.Utils.ToNiceString(this.Name);
        }

        public string GetDamageClass() // Physical, Special, Status
        {
            return this.Damage_class.Name.ToLower();
        }

        public bool isAnAttack() // Is this an attack ? Else a status
        {
            return isPhysical() || isSpecial();
        }

        public bool isPhysical() // Is this an attack ? Else a status
        {
            return GetDamageClass() == "physical";
        }

        public bool isSpecial()
        {
            return GetDamageClass() == "special";
        }

        // Attack power >= 65 + bullet seed etc + heavy slam etc
        public bool isAGoodAttack()
        {
            return (isAnAttack() || (isAnAttack() && this.Power == null)) && (this.Power >= 65 || this.Power == 55);
        }

        public string GetType()
        {
            return this.Type.Name.ToLower();
        }
    }
}
