using FiniteElementsProject.Elements;
using FiniteElementsProject.FormulaPartsLibrary;
using FiniteElementsProject.Mesh;

namespace FiniteElementsProject.FormulaLogic;

public static class ElementCalculations
{
    public static void CalculateShapeFuncDerValues(this Element4_2D element42D)
    {
        var points = element42D.NKsi.GetLength(1);
        var gausQuadrature = new GaussQuadrature();

        var nodes = points switch
        {
            4 => gausQuadrature.Generate4Nodes(points),
            9 => gausQuadrature.Generate9Nodes(points),
            _ => throw new Exception("There is something wrong with element object")
        };

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < points; j++)
            {
                element42D.NKsi[i, j] = element42D.DerKsi[i](nodes.ElementAt(j).Item2);
                element42D.NEta[i, j] = element42D.DerEta[i](nodes.ElementAt(j).Item1);
            }
        }
    }

    public static void CalculateJacobian(this Element4_2D element42D, Jacobian jacobian, Grid grid)
    {
        var points = element42D.NKsi.GetLength(1);

        for (int i = 0; i < grid.Elements.Length; i++)
        {
            for (int j = 0; j < points; j++)
            {
                CalculateOnePointJacobian(element42D, i, j, ref jacobian, grid);
                var jacSize1 = jacobian.JacobianInverted.GetLength(0);
                var jacSize2 = jacobian.JacobianInverted.GetLength(1);
            
                var detInverted = 1 / jacobian.JacobianDet;
                var jacobianTransposed = new double[jacSize1, jacSize2];

                for (int k = 0; k < jacSize1; k++)
                {
                    for (int l = 0; l < jacSize2; l++)
                    {
                        jacobianTransposed[k,l] = jacobian.JacobianInverted[k, l] * detInverted;
                        Console.Write($"{jacobianTransposed[k, l]:0.00000000} ");
                    }

                    Console.WriteLine();
                }

                Console.WriteLine();
                Console.WriteLine($"Inverted det = {detInverted:0.00000000}");
                Console.WriteLine();
            }
        }
    }

    private static void CalculateOnePointJacobian(Element4_2D element42D, int i, int j, ref Jacobian jacobian, Grid grid)
    {
        var nKsiValues = Enumerable.Range(0, element42D.NKsi.GetLength(0))
            .Select(x => element42D.NKsi[x, 0])
            .ToArray();
        
        var nEtaValues = Enumerable.Range(0, element42D.NEta.GetLength(0))
            .Select(x => element42D.NEta[x, 0])
            .ToArray();

        var xDerKsi = 0.0;
        var yDerEta = 0.0;

        var yDerKsi = 0.0;
        var xDerEta = 0.0;

        var currentElement = grid.Elements[i];

        for (int k = 0; k < 4; k++)
        {
            var currentNodeValues = grid.Nodes[currentElement.ID[k] - 1];
            xDerKsi += nKsiValues[k] * currentNodeValues.X;
            yDerEta += nEtaValues[k] * currentNodeValues.Y;

            yDerKsi += nKsiValues[k] * currentNodeValues.Y;
            xDerEta += nEtaValues[k] * currentNodeValues.X;
        }
        
        jacobian.JacobianNormal = new [,] { { xDerKsi, yDerKsi }, { xDerEta, yDerEta } };
        jacobian.JacobianInverted = new [,] { { yDerEta, -yDerKsi }, { -xDerEta, xDerKsi } };
        jacobian.JacobianDet = (xDerKsi * yDerEta) - (yDerKsi * xDerEta);
    }
}