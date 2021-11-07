using FiniteElementsProject.ConsoleUI;
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

    public static void CalculateHMatrix(this Grid grid, Element4_2D element42D)
    {
        var points = element42D.NKsi.GetLength(1);
        var size1 = element42D.NKsi.GetLength(0);
        var size2 = element42D.NKsi.GetLength(1);

        for (int i = 0; i < grid.Elements.Length; i++)
        {
            var hMatrix = new double[size1, size2];
            
            for (int j = 0; j < points; j++)
            {
                var jacobian = CalculateOnePointJacobian(element42D, i, j, grid);
                jacobian.PrintJacobian(i, j);

                var oneHMatrix = CalculateOneHMatrix(element42D, i, j, jacobian.JacobianInverted, 30, jacobian.JacobianDet);

                for (int k = 0; k < size1; k++)
                {
                    for (int l = 0; l < size2; l++)
                    {
                        hMatrix[k, l] += oneHMatrix[k, l];
                    }
                }
            }

            grid.Elements[i].HMatrix = hMatrix;
        }
    }

    private static double[,] CalculateOneHMatrix(Element4_2D element42D, int i, int j, double[,] jacobianInverted,
        int kFactor, double jacobianDet)
    {
        var size1 = element42D.NKsi.GetLength(0);
        var size2 = element42D.NKsi.GetLength(1);
        
        var nDerX = new double[size1];
        var nDerY = new double[size1];
        var hMatrix = new double[size1, size2];

        for (int k = 0; k < size1; k++)
        {
            nDerX[k] = jacobianInverted[0, 0] * element42D.NKsi[k, j]
                          + jacobianInverted[0, 1] * element42D.NEta[k, j];
            nDerY[k] = jacobianInverted[1, 0] * element42D.NKsi[k, j]
                          + jacobianInverted[1, 1] * element42D.NEta[k, j];
        }
        
        for (int k = 0; k < size1; k++)
        {
            for (int l = 0; l < size2; l++)
            {
                hMatrix[k, l] = nDerX[k] * nDerX[l] + nDerY[k] * nDerY[l];
                hMatrix[k, l] *= kFactor;
                hMatrix[k, l] *= jacobianDet;
            }
        }
        
        return hMatrix;
    }

    private static Jacobian CalculateOnePointJacobian(Element4_2D element42D, int i, int j, Grid grid)
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

        var tempX = new double[] { 0, 0.025, 0.025, 0 };
        var tempY = new double[] { 0, 0, 0.025, 0.025 };

        for (int k = 0; k < 4; k++)
        {
            var currentNodeValues = grid.Nodes[currentElement.ID[k] - 1];
            xDerKsi += nKsiValues[k] * currentNodeValues.X; /*tempX[k]*/
            yDerEta += nEtaValues[k] * currentNodeValues.Y; /*tempY[k]*/

            yDerKsi += nKsiValues[k] * currentNodeValues.Y; /*tempY[k]*/
            xDerEta += nEtaValues[k] * currentNodeValues.X; /*tempX[k]*/
        }
        
        var jacobianNormal = new [,] { { xDerKsi, yDerKsi }, { xDerEta, yDerEta } };
        var jacobianComplement = new [,] { { yDerEta, -yDerKsi }, { -xDerEta, xDerKsi } };
        var jacobianDet = (xDerKsi * yDerEta) - (yDerKsi * xDerEta);

        return new Jacobian(jacobianNormal, jacobianComplement, jacobianDet);
    }
}