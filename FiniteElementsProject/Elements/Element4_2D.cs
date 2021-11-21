using FiniteElementsProject.FormulaPartsLibrary;

namespace FiniteElementsProject.Elements;

public struct Element4_2D
{
    public double[,] NKsi { get; }
    public double[,] NEta { get; }
    public IntegrationPoints[] IntegrationPoints { get; set; }
    public ShapeFunctionValues[] ShapeFunctionValues { get; set; }
    public Func<double, double>[] DerKsi { get; }
    public Func<double, double>[] DerEta { get; }
    public Func<double, double, double>[] ShapeFunctions { get; }

    public Element4_2D(int points)
    {
        NKsi = new double[4, points];
        NEta = new double[4, points];

        IntegrationPoints = new IntegrationPoints[]
        {
            new IntegrationPoints((int)Math.Sqrt(points)),
            new IntegrationPoints((int)Math.Sqrt(points)),
            new IntegrationPoints((int)Math.Sqrt(points)),
            new IntegrationPoints((int)Math.Sqrt(points)),
        };

        ShapeFunctionValues = new[]
        {
            new ShapeFunctionValues((int)Math.Sqrt(points)),
            new ShapeFunctionValues((int)Math.Sqrt(points)),
            new ShapeFunctionValues((int)Math.Sqrt(points)),
            new ShapeFunctionValues((int)Math.Sqrt(points)),
        };

        DerKsi = new List<Func<double, double>>()
        {
            eta => (-1 / 4.0) * (1 - eta),
            eta => (1 / 4.0) * (1 - eta),
            eta => (1 / 4.0) * (1 + eta),
            eta => (-1 / 4.0) * (1 + eta)
        }.ToArray();
        
        DerEta = new List<Func<double, double>>()
        {
            ksi => (-1 / 4.0) * (1 - ksi),
            ksi => (-1 / 4.0) * (1 + ksi),
            ksi => (1 / 4.0) * (1 + ksi),
            ksi => (1 / 4.0) * (1 - ksi)
        }.ToArray();

        ShapeFunctions = new List<Func<double, double, double>>()
        {
            (ksi, eta) => 0.25 * (1 - ksi) * (1 - eta),
            (ksi, eta) => 0.25 * (1 + ksi) * (1 - eta),
            (ksi, eta) => 0.25 * (1 + ksi) * (1 + eta),
            (ksi, eta) => 0.25 * (1 - ksi) * (1 + eta),
        }.ToArray();
    }
}