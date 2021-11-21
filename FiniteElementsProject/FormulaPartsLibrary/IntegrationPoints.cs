namespace FiniteElementsProject.FormulaPartsLibrary;

public struct IntegrationPoints
{
    public double[] Ksi { get; set; }
    public double[] Eta { get; set; }
    public double[] Scale { get; set; }

    public IntegrationPoints(int points)
    {
        Ksi = new double[points];
        Eta = new double[points];
        Scale = new double[points];
    }
}