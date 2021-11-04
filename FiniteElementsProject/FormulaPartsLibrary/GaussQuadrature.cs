namespace FiniteElementsProject.FormulaPartsLibrary;

public struct GuassElements
{
    public List<double> Xk { get; set; }
    public List<double> Ak { get; set; }
}

public class GaussQuadrature
{
    private List<GuassElements> _guassTabele;

    public GaussQuadrature()
    {
        _guassTabele = new List<GuassElements>()
        {
            new GuassElements()
            {
                Xk = new List<double>() { -1 / Math.Sqrt(3.0), 1 / Math.Sqrt(3.0) },
                Ak = new List<double>() { 1, 1 }
            },
            new GuassElements()
            {
                Xk = new List<double>() { -Math.Sqrt(3.0/5),  0, Math.Sqrt(3.0/5)},
                Ak = new List<double>() { 5.0/9, 8.0/9, 5.0/9 }
            },
            new GuassElements()
            {
                Xk = new List<double>() { -0.861136, -0.339981, 0.861136, 0.339981 },
                Ak = new List<double>() { 0.347855, 0.652145, 0.347855, 0.652145 }
            }
        };
    }

    public GuassElements GetGaussQuadrature(int points) => _guassTabele.ElementAt(points - 4);

    public List<(double, double)> Generate4Nodes(int points)
    {
        var gaussElements = GetGaussQuadrature(points);
        var ksi = gaussElements.Xk.ElementAt(0);
        var eta = gaussElements.Xk.ElementAt(1);
        
        var nodes = new List<(double, double)>();
        
        const int multipier = -1;

        for (var i = 0; i < points; i++)
        {
            if (i is 1 or 3) ksi *= multipier;
            if (i % 2 == 0) eta *= multipier;
            nodes.Add((ksi, eta));
        }

        return nodes;
    }

    public List<(double, double)> Generate9Nodes(int points)
    {
        var gaussElements = GetGaussQuadrature(points);

        var nodeLeft = gaussElements.Xk.ElementAt(0);
        var nodeZero = gaussElements.Xk.ElementAt(1);
        var nodeRight = -gaussElements.Xk.ElementAt(2);

        double ksi = nodeLeft;
        double eta = nodeRight;

        var nodes = new List<(double, double)>();
        
        const int multiplier = -1;

        for (int i = 0; i < points; i++)
        {
            if (i is 1 or 5 or 8)
                ksi = nodeZero;
            if (i is 2 or 6)
            {
                ksi = nodeLeft * multiplier;
                nodeLeft = ksi;
            }
            if (i is 3 or 7 or 8)
            {
                eta = nodeZero;
                nodeRight *= multiplier;
            }
            else if (i > 3)
                eta = nodeRight;
            
            nodes.Add((ksi, eta));
        }

        return nodes;
    }
    
}