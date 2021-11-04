using FiniteElementsProject.FormulaPartsLibrary;

namespace FiniteElementsProject.FormulaLogic;

public static class IntegralCalculations
{
    public static double GaussIntegral(Func<double, double> function, int points)
    {
        double sum = 0;
        
        var gausQuadrature = new GaussQuadrature();
        var gaussElements = gausQuadrature.GetGaussQuadrature(points);

        for (int i = 0; i <= points; i++)
        {
            sum += gaussElements.Ak[i] * function(gaussElements.Xk[i]);
        }

        return sum;
    }

    public static double GaussIntegral(Func<double, double, double> function, int points)
    {
        double sum = 0;
        
        var gausQuadrature = new GaussQuadrature();
        var gaussElements = gausQuadrature.GetGaussQuadrature(points);

        for (int i = 0; i <= points; i++)
        {
            for (int j = 0; j <= points; j++)
            {
                sum += gaussElements.Ak[i] * gaussElements.Ak[j] * function(gaussElements.Xk[i], gaussElements.Xk[j]);
            }
        }

        return sum;
    }
}