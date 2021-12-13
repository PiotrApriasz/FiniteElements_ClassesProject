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
            element42D.IntegrationPoints[1].Scale[2] = elements.scale[2];
            
            element42D.IntegrationPoints[2].Ksi[0] = elements.values[2];
            element42D.IntegrationPoints[2].Ksi[1] = elements.values[1];
            element42D.IntegrationPoints[2].Ksi[2] = elements.values[0];
            element42D.IntegrationPoints[2].Eta[2] = 1;
            element42D.IntegrationPoints[2].Scale[2] = elements.scale[2];
            
            element42D.IntegrationPoints[3].Ksi[2] = -1;
            element42D.IntegrationPoints[3].Eta[0] = elements.values[2];
            element42D.IntegrationPoints[3].Eta[1] = elements.values[1];
            element42D.IntegrationPoints[3].Eta[2] = elements.values[0];
            element42D.IntegrationPoints[3].Scale[2] = elements.scale[2];
        }
        
    }
    
    public static void CalculateShapeFuncValues(this Element4_2D element42D)
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

                element42D.NValues[j, i] =
                    element42D.ShapeFunctions[i](nodes.ElementAt(j).Item1, nodes.ElementAt(j).Item2);
            }
        }
    }

    public static (double[,] HMatrix, double[,] CMatrix) CalculateOneHAndCMatrix(this Element4_2D element42D, int j, double[,] jacobianInverted,
        double conductivity, double jacobianDet, double heat, double density, (double, double) scales)
    {
        var size1 = element42D.NKsi.GetLength(0);

        var nDerX = new double[size1];
        var nDerY = new double[size1];
        var hMatrix = new double[size1, size1];
        var cMatrix = new double[size1, size1];

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
                hMatrix[k, l] *= scales.Item1 * scales.Item2;
                hMatrix[k, l] *= conductivity;
                hMatrix[k, l] *= jacobianDet;

                cMatrix[k, l] = element42D.NValues[j, k] * element42D.NValues[j, l];
                cMatrix[k, l] *= scales.Item1 * scales.Item2;
                cMatrix[k, l] *= heat;
                cMatrix[k, l] *= density;
                cMatrix[k, l] *= jacobianDet;
            }
        }
        
        return (hMatrix, cMatrix);
    }

    public static (double[,] hbcMatrix, double[] pVector) CalculateOneHbcMatrixAndOnePVector(this Element4_2D element42D,
        int l, double alpha, Node node1, Node node2, double t0)
    {
        var det = ElementHelper.CalculateSideLenght(node1, node2);
        var points = element42D.IntegrationPoints[0].Eta.Length;
        var size1 = element42D.NKsi.GetLength(0);

        var hbcOneMatrix = new double[size1, size1];
        var hbcMatrix = new double[size1, size1];
        var pOneVector = new double[size1];
        var pVector = new double[size1];

        for (int i = 0; i < points; i++)
        {
            for (int j = 0; j < size1; j++)
            {
                for (int k = 0; k < size1; k++)
                {
                    hbcOneMatrix[j ,k] = element42D.ShapeFunctionValuesSurface[l].NValues.ElementAt(j)[i] *
                                         element42D.ShapeFunctionValuesSurface[l].NValues.ElementAt(k)[i];
                    hbcOneMatrix[j ,k] *= element42D.IntegrationPoints[l].Scale[i];
                    hbcOneMatrix[j ,k] *= alpha;
                    hbcOneMatrix[j, k] *= det;
                }

                pOneVector[j] = element42D.ShapeFunctionValuesSurface[l].NValues.ElementAt(j)[i] * t0;
                pOneVector[j] *= element42D.IntegrationPoints[l].Scale[i];
                pOneVector[j] *= alpha;
                pOneVector[j] *= det;
            }

            for (int j = 0; j < size1; j++)
            {
                for (int k = 0; k < size1; k++)
                {
                    hbcMatrix[j, k] += hbcOneMatrix[j, k];
                }

                pVector[j] += pOneVector[j];
            }
        }

        return (hbcMatrix, pVector);
    }

    public static Jacobian CalculateOnePointJacobian(this Element4_2D element42D, int i, int j, Grid grid)
    {
        var nKsiValues = Enumerable.Range(0, element42D.NKsi.GetLength(0))
            .Select(x => element42D.NKsi[x, j])
            .ToArray();
        
        var nEtaValues = Enumerable.Range(0, element42D.NEta.GetLength(0))
            .Select(x => element42D.NEta[x, j])
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
        
        var jacobianNormal = new [,] { { xDerKsi, yDerKsi }, { xDerEta, yDerEta } };
        var jacobianComplement = new [,] { { yDerEta, -yDerKsi }, { -xDerEta, xDerKsi } };
        var jacobianDet = (xDerKsi * yDerEta) - (yDerKsi * xDerEta);

        return new Jacobian(jacobianNormal, jacobianComplement, jacobianDet);
    }

    public static void CalculateNValuesSurface(this Element4_2D element42D)
    {
        var iterator = 0;
        
        foreach (var shapeFunctionValue in element42D.ShapeFunctionValuesSurface)
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