using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnnNetLib2
{
    public partial class CnnNet
    {
        public int NeuronCount = 100;

        /// <summary>
        /// Desirability influence distance
        /// </summary>
        public int NeuronDesirabilityInfluenceRange = 80;

        /// <summary>
        /// Maximum desirability increase
        /// </summary>
        public double NeuronDesirabilityMaxInfluence = 0.05;

        /// <summary>
        /// How much does desirability decrease with each iteration
        /// </summary>
        public double DesirabilityDecayAmount = 0.005;

        /// <summary>
        /// The range around a neuron searched for higher desirability
        /// </summary>
        public int NeuronHigherDesirabilitySearchPlainRange = 10;

        /// <summary>
        /// The minimum distance allowed between neurons
        /// </summary>
        public int MinDistanceBetweenNeurons = 10;

        /// <summary>
        /// How many input neurons we should generate
        /// </summary>
        public int InputNeuronCount = 10;

        /// <summary>
        /// Indicates if input neurons can move to higher desirability
        /// </summary>
        public bool InputNeuronsMoveToHigherDesirability = false;

        /// <summary>
        /// What is the maximum distance that a neuron can move to higher desirability (should be NeuronHigherDesirabilitySearchPlainRange?)
        /// When this distance is reached the neuron automatically sets HasReachedFinalPosition = true
        /// </summary>
        public int MaxNeuronMoveDistance = 50;

        /// <summary>
        /// After how many iterations of neuron stagnation (nouron does not move to new position) HasReachedFinalPosition is set automatically to true
        /// </summary>
        public int NeuronIterationCountBeforeFinalPosition = 5;

        /// <summary>
        /// The range of the neuron dendric tree
        /// </summary>
        public int NeuronDendricTreeRange = 160;
    }
}
