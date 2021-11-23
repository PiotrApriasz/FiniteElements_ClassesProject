using FiniteElementsProject.ConsoleUI;
using FiniteElementsProject.Elements;
using FiniteElementsProject.FormulaPartsLibrary;
using FiniteElementsProject.Helpers;
using FiniteElementsProject.Mesh;

namespace FiniteElementsProject.FormulaLogic;

public static class ElementCalculations
{
    public static void CalculateIntegrationPoints(this Element4_2D element42D)
    {
        var gausQuadrature = new GaussQuadrature();
        var points = element42D.NKsi.GetLength(1);
        
        var elements = points switch
        {
            4 => gausQuadrature.Get2IntegrationPointsElements(),
            9 => gausQuadrature.Get3IntegrationPointsElements(),
            _ => throw new Exception("There is something wrong with element object")
        };
        
        element42D.IntegrationPoints[0].Ksi[0] = elements.values[0];
        element42D.IntegrationPoints[0].Ksi[1] = elements.values[1];
        element42D.IntegrationPoints[0].Eta[0] = -1;
        element42D.IntegrationPoints[0].Eta[1] = -1;
        element42D.IntegrationPoints[0].Scale[0] = elements.scale[0];
        element42D.IntegrationPoints[0].Scale[1] = elements.scale[1];
        
        element42D.IntegrationPoints[1].Ksi[0] = 1;
        element42D.IntegrationPoints[1].Ksi[1] = 1;
        element42D.IntegrationPoints[1].Eta[0] = elements.values[0];
        element42D.IntegrationPoints[1].Eta[1] = elements.values[1];
        element42D.IntegrationPoints[1].Scale[0] = elements.scale[0];
        element42D.IntegrationPoints[1].Scale[1] = elements.scale[1];
        
        element42D.IntegrationPoints[2].Ksi[0] = elements.values[1];
        element42D.IntegrationPoints[2].Ksi[1] = elements.values[0];
        element42D.IntegrationPoints[2].Eta[0] = 1;
        element42D.IntegrationPoints[2].Eta[1] = 1;
        element42D.IntegrationPoints[2].Scale[0] = elements.scale[0];
        element42D.IntegrationPoints[2].Scale[1] = elements.scale[1];
        
        element42D.IntegrationPoints[3].Ksi[0] = -1;
        element42D.IntegrationPoints[3].Ksi[1] = -1;
        element42D.IntegrationPoints[3].Eta[0] = elements.values[1];
        element42D.IntegrationPoints[3].Eta[1] = elements.values[0];
        element42D.IntegrationPoints[3].Scale[0] = elements.scale[0];
        element42D.IntegrationPoints[3].Scale[1] = elements.scale[1];

        if (points == 9)
        {
            element42D.IntegrationPoints[0].Ksi[2] = elements.values[2];
            element42D.IntegrationPoints[0].Eta[2] = -1;
            element42D.IntegrationPoints[0].Scale[2] = elements.scale[2];
            
            element42D.IntegrationPoints[1].Ksi[2] = 1;
            element42D.IntegrationPoints[1].Eta[2] = elements.values[2];
            element42D.IntegrationPoints[0].Scale[2] = elements.scale[2];
            
            element42D.IntegrationPoints[2].Ksi[0] = elements.values[2];
            element42D.IntegrationPoints[2].Ksi[1] = elements.values[1];
            element42D.IntegrationPoints[2].Ksi[2] = elements.values[0];
            element42D.IntegrationPoints[2].Eta[2] = 1;
            element42D.IntegrationPoints[2].Scale[2] = elements.scale[2];
            
            element42D.IntegrationPoints[3].Ksi[2] = -1;
            element42D.IntegrationPoints[3].Eta[0] = elements.values[2];
            element42D.IntegrationPoints[3].Eta[1] = elements.values[1];
            element42D.IntegrationPoints[3].Eta[2] = elements.values[0];
            element42D.IntegrationPoints[2].Scale[2] = elements.scale[2];
        }
        
    }
    
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

    public static double[,] CalculateOneHMatrix(this Element4_2D element42D, int j, double[,] jacobianInverted,
        int kFactor, double jacobianDet)
    {
        var size1 = element42D.NKsi.GetLength(0);
        var size2 = element42D.NKsi.GetLength(1);
        
        var nDerX = new double[size1];
        var nDerY = new double[size1];
        var hMatrix = new double[size1, size1];

        for (int k = 0; k < size1; k++)
        {
            nDerX[k] = jacobianInverted[0, 0] * element42D.NKsi[k, j]
                          + jacobianInverted[0, 1] * element42D.NEta[k, j];
            nDerY[k] = jacobianInverted[1, 0] * element42D.NKsi[k, j]
                          + jacobianInverted[1, 1] * element42D.NEta[k, j];
        }
        
        for (int k = 0; k < size1; k++)
        {
            for (int l = 0; l < size1; l++)
            {
                hMatrix[k, l] = nDerX[k] * nDerX[l] + nDerY[k] * nDerY[l];
                hMatrix[k, l] *= kFactor;
                hMatrix[k, l] *= jacobianDet;
            }
        }
        
        return hMatrix;
    }

    public static double[,] CalculateOneHbcMatrix(this Element4_2D element42D, int l, int kFactor, Node node1, Node node2)
    {
        var det = ElementHelper.CalculateSideLenght(node1, node2);
        var points = element42D.IntegrationPoints[0].Eta.Length;
        var size1 = element42D.NKsi.GetLength(0);

        var hbcMatrix = new double[size1, size1];

        for (int i = 0; i < size1; i++)
        {
            for (int j = 0; j < size1; j++)
            {
                for (int k = 0; k < points; k++)
                {
                    hbcMatrix[i, j] += element42D.ShapeFunctionValues[l].NValues.ElementAt(i)[k] *
                                       element42D.ShapeFunctionValues[l].NValues.ElementAt(j)[k];
                    hbcMatrix[i, j] *= element42D.IntegrationPoints[l].Scale[k];
                }

                hbcMatrix[i, j] *= kFactor;
                hbcMatrix[i, j] *= det;
            }
        }

        return hbcMatrix;
    }

    public static Jacobian CalculateOnePointJacobian(this Element4_2D element42D, int i, Grid grid)
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
            xDerKsi += nKsiValues[k] * currentNodeValues.X; /*tempX[k];*/
            yDerEta += nEtaValues[k] * currentNodeValues.Y; /*tempY[k];*/

            yDerKsi += nKsiValues[k] * currentNodeValues.Y; /*tempY[k];*/
            xDerEta += nEtaValues[k] * currentNodeValues.X; /*tempX[k];*/
        }
        
        var jacobianNormal = new [,] { { xDerKsi, yDerKsi }, { xDerEta, yDerEta } };
        var jacobianComplement = new [,] { { yDerEta, -yDerKsi }, { -xDerEta, xDerKsi } };
        var jacobianDet = (xDerKsi * yDerEta) - (yDerKsi * xDerEta);

        return new Jacobian(jacobianNormal, jacobianComplement, jacobianDet);
    }

    public static void CalculateNValues(this Element4_2D element42D)
    {
        var iterator = 0;
        
        foreach (var shapeFunctionValue in element42D.ShapeFunctionValues)
        {
            var integrationPoint = element42D.IntegrationPoints[iterator];

            for (int i = 0; i < integrationPoint.Eta.Length; i++)
            {
                shapeFunctionValue.N1[i] = element42D.ShapeFunctions[0](integrationPoint.Ksi[i], integrationPoint.Eta[i]);
                shapeFunctionValue.N2[i] = element42D.ShapeFunctions[1](integrationPoint.Ksi[i], integrationPoint.Eta[i]);
                shapeFunctionValue.N3[i] = element42D.ShapeFunctions[2](integrationPoint.Ksi[i], integrationPoint.Eta[i]);
                shapeFunctionValue.N4[i] = element42D.ShapeFunctions[3](integrationPoint.Ksi[i], integrationPoint.Eta[i]);
            }
            
            shapeFunctionValue.NValues.Add(shapeFunctionValue.N1);
            shapeFunctionValue.NValues.Add(shapeFunctionValue.N2);
            shapeFunctionValue.NValues.Add(shapeFunctionValue.N3);
            shapeFunctionValue.NValues.Add(shapeFunctionValue.N4);

            iterator++;
        }
    }
}