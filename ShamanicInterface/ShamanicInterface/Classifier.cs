using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Models.Markov;

namespace ShamanicInterface.Classifier
{
    public class HMMClassifier
    {

        HiddenMarkovClassifier<MultivariateNormalDistribution> classifer;
        List<HiddenMarkovModel<MultivariateNormalDistribution>> models;
        List<string> names;

        public HMMClassifier() {
            models = new List<HiddenMarkovModel<MultivariateNormalDistribution>>();
            names = new List<string>();
        }

        public HMMClassifier(List<HiddenMarkovModel<MultivariateNormalDistribution>> models, List<string> names) {
            this.models = models;
            this.names = names;
        }

        public void AddModel(HiddenMarkovModel<MultivariateNormalDistribution> model, string name) {
            models.Add(model);
            names.Add(name);
        }

        public void ClearClassifier() {
            models.Clear();
            names.Clear();
        }

        public int ComputeToInt(double[][] sequence) {
            return classifer.Compute(sequence);
        }

        public string ComputeToString(double[][] sequence) {
            return names[ComputeToInt(sequence)];
        }

        public void StartClassifier() {
            classifer = new HiddenMarkovClassifier<MultivariateNormalDistribution>(models.ToArray());
        }

        public double TestModel(int i, double[][] sequence) {
            return models[i].Evaluate(sequence);
        }
    }
}
