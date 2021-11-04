namespace FiniteElementsProject.Elements;

public struct Element4_2D
{
    public double[,] NKsi { get; }
    public double[,] NEta { get; }
    public Func<double, double>[] DerKsi { get; }
    public Func<double, double>[] DerEta { get; }

    public Element4_2D(int points)
    {
        NKsi = new double[4, points];
        NEta = new double[4, points];
        
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
    }
}