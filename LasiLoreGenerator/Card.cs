using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LasiLoreGenerator
{
    //A Card Holds String Information about the text to be printed
    internal class Card
    {
        public string Type;
        public string Name;
        public string Cost;
        public string Power;
        public string Life;
        public string EffectText;

        //Creates a Card
        /// <summary>
        /// type, name, cost, power, life, effectText
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="cost"></param>
        /// <param name="power"></param>
        /// <param name="life"></param>
        /// <param name="effectText"></param>
        public Card (string type, string name, string cost, string power, string life, string effectText)
        {
            Type = type;
            Name = name;
            Cost = cost;
            Power = power;
            Life = life;
            EffectText = effectText;
        }
    }
}
