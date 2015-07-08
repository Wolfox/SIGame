using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/*using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Models.Markov;*/

namespace ShamanicInterface.State
{

    public class Actions
    {
        private string stateName { get; set; }
        private List<string> actions;

        public Actions(string name) {
            stateName = name;
            actions = new List<string>();
        }

        public void AddAction(string action) {
            actions.Add(action);
        }

        public List<string> GetActions() {
            return actions;
        }
    }
}
