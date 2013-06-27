using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cnnnet.Lib
{
    public partial class CnnNet
    {
        public int NeuronCount = 300;

        /// <summary>
        /// Desirability influence distance
        /// </summary>
        public int NeuronDesirabilityInfluenceRange = 40;

        /// <summary>
        /// Maximum desirability increase
        /// </summary>
        public double NeuronDesirabilityMaxInfluence = 0.3;

        /// <summary>
        /// How much does desirability decrease with each iteration
        /// </summary>
        public double DesirabilityDecayAmount = 0.001;

        /// <summary>
        /// The range around a neuron searched for higher desirability
        /// </summary>
        public int NeuronHigherDesirabilitySearchPlainRange = 10;

        /// <summary>
        /// The minimum distance allowed between neurons
        /// </summary>
        public int MinDistanceBetweenNeurons = 20;

        /// <summary>
        /// How many input neurons we should generate
        /// </summary>
        public int InputNeuronCount = 1;

        /// <summary>
        /// What is the maximum distance that a neuron can move to higher desirability (should be NeuronHigherDesirabilitySearchPlainRange?)
        /// When this distance is reached the neuron automatically sets HasReachedFinalPosition = true
        /// </summary>
        public int MaxNeuronMoveDistance = 50;

        /// <summary>
        /// The range of the neuron dendric tree
        /// </summary>
        public int NeuronDendricTreeRange = 40;

        /// <summary>
        /// Undesirability influence distance
        /// </summary>
        public int NeuronUndesirabilityInfluenceRange = 80;

        /// <summary>
        /// Maximum undesirability increase
        /// </summary>
        public double NeuronUndesirabilityMaxInfluence = 0.03;

        /// <summary>
        /// How much does undesirability decrease with each iteration
        /// </summary>
        public double UndesirabilityDecayAmount = 0.05;

        /// <summary>
        /// After how many iterations of inactivity does the neuron increase
        /// the undesirability with NeuronUndesirabilityMaxInfluence
        /// </summary>
        public int NeuronUndesirabilityMaxIterationsSinceLastActivation = 20;

        /// <summary>
        /// The range around a axon terminal searched according to the guidance forces
        /// </summary>
        public int AxonGuidanceForceSearchPlainRange = 20;

        /// <summary>
        /// The minimum distance allowed between the current found waypoint and previous waypoints
        /// </summary>
        public int AxonMinDistanceToPreviousWaypoints = 1;

        public int AxonGuidanceFrequency = 1;

        public int DesirabilityFrequency = 4;

        public int UndesirabilityFrequency = 4;

        public int NeuronIsActiveMinimumActivityScore = 3;

        public int NeuronActivityScoreDecayAmnount = 1;

        public int NeuronActivityScoreMultiply = 3;
    }
}