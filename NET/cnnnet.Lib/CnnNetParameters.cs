namespace cnnnet.Lib
{
    public partial class CnnNet
    {
        /// <summary>
        /// The minimum distance to any margin from a generated neuron
        /// </summary>
        public int NeuronGenerationMargin = 40;

        /// <summary>
        /// The minimum distance between a processing neuron and any input /output neuron
        /// </summary>
        public int NeuronProcessMarginFromInputAndOutputNeurons = 10;

        /// <summary>
        /// The minimum distance between input neurons
        /// </summary>
        public int NeuronInputMarginBetween = 50;



        public int NeuronCount = 60;

        /// <summary>
        /// How many input neurons we should generate
        /// </summary>
        public int InputNeuronCount = 6;

        /// <summary>
        /// How many output neurons we should generate
        /// </summary>
        public int OutputNeuronCount = 6;

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
        public int SomaGuidanceForceSearchPlainRange = 20;

        /// <summary>
        /// The minimum distance allowed between neurons
        /// </summary>
        public int MinDistanceBetweenNeurons = 15;

        public int InputNeuronDelayIterationsBeforeExtendingAxon = 2;

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
        public double UndesirabilityDecayAmount = 0.1;

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
        /// The minimum distance allowed between the current found waypoint and other neuron waypoints
        /// </summary>
        public int AxonMinDistanceToOtherWayPoints = 5;
        
        public int NeuronIsActiveMinimumActivityScore = 3;

        /// <summary>
        /// For how many iterations do we hold the neuronal activity.
        /// This also means that only the last 'NeuronActivityHistoryLength' iterations are going to be used 
        /// for neuronal plasticity
        /// </summary>
        public int NeuronActivityHistoryLength = 5;

        /// <summary>
        /// For how many iterations after the neuron fires is the neuron capable of firing again
        /// </summary>
        public int NeuronActivityIdleIterations = 3;

        /// <summary>
        /// The minimum strength required for a synapse to pass the signal from an 
        /// PreSynaptic neuron to a PostSynaptic neuron
        /// </summary>
        public double NeuronSynapseConnectedMinimumStrength = 0.2;

        /// <summary>
        /// The amount that a synapses strength can change because of plasticity
        /// </summary>
        public double NeuronSynapseStrengthChangeAmount = 0.1;
    }
}