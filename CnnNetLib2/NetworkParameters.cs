namespace CnnNetLib2
{
    public class NetworkParameters
    {
        public int NeuronCount = 100;
        public int NeuronInfluenceRange = 80;
        public double MaxNeuronInfluence = 0.05;
        public double DesirabilityDecayAmount = 0.005;
        public double PercentActiveNeurons = 0.1;
        public int NeuronDesirabilityPlainRange = 10;
        public int MinDistanceBetweenNeurons = 10;
        public int InputNeuronCount = 10;
        public bool InputNeuronsMoveToHigherDesirability = false;
        public int MaxNeuronMoveDistance = 999;
    }
}
