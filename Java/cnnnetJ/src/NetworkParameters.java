
public class NetworkParameters
{
    public int Width = 800;

    public int Height = 600;

    public int NeuronCount = 100;

    public int NeuronInputCount = 10;

    /*
    / <summary>
    / How much does desirability decrease with each iteration
    / </summary>
    */
    public double DesirabilityDecayAmount = 0.005;

    /*
    / <summary>
    / How much does undesirability decrease with each iteration
    / </summary>
    */
    public double UndesirabilityDecayAmount = 0.05;

    /*
    / <summary>
    / Desirability influence distance
    / </summary>
    */
    public int NeuronDesirabilityInfluenceRange = 80;

    /*
    / <summary>
    / Maximum desirability increase
    / </summary>
    */
    public double NeuronDesirabilityMaxInfluence = 0.05;
}
