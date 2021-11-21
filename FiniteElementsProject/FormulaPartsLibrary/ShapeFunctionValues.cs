namespace FiniteElementsProject.FormulaPartsLibrary;

public struct ShapeFunctionValues
{
    public double[] N1 { get; set; }
    public double[] N2 { get; set; }
    public double[] N3 { get; set; }
    public double[] N4 { get; set; }

    public ShapeFunctionValues(int points)
    {
        N1 = new double[points];
        N2 = new double[points];
        N3 = new double[points];
        N4 = new double[points];
    }
}