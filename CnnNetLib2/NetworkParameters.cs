namespace CnnNetLib2
{
    public class NetworkParameters
    {
        public int NeuronCount = 100;

        /// <summary>
        /// Desirability influence distance
        /// </summary>
        public int NeuronInfluenceRange = 80;

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
        /// </summary>
        public int MaxNeuronMoveDistance = 10;
    }
}
