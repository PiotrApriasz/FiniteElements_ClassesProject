﻿using FiniteElementsProject.ConsoleUI;
using FiniteElementsProject.Mesh;
using FiniteElementsProject.Elements;
using FiniteElementsProject.FormulaPartsLibrary;

namespace FiniteElementsProject.FormulaLogic;

public static class MeshCalculations
{
    public static void CalculateNodes(this Grid grid, double t0)
    {
        double x;
        double y;

        double dX = grid.B / (grid.nB - 1);
        double dY = grid.H / (grid.nH - 1);

        int counter = 0;

        for (int i = 0; i < grid.nB; i++)
        {
            for (int j = 0; j < grid.nH; j++)
            {
                x = i * dX;
                y = j * dY;

                var node = new Node() { X = x, Y = y, T0 = t0};

                if (x == 0) node.BoundaryCondition = true;
                if (y == 0) node.BoundaryCondition = true;
                if (x == grid.B) node.BoundaryCondition = true;
                if (y == grid.H) node.BoundaryCondition = true;

                grid.Nodes[counter] = node;

                counter++;
            }
        }
    }

    public static void CalculateIds(this Grid grid)
    {
        var k = 1;
        var counter = 0;

        for (int i = 0; i < grid.nB - 1; i++)
        {
            for (int j = 0; j < grid.nH - 1; j++)
            {
                int id1 = k;
                int id2 = k + grid.nH;
                int id3 = id2 + 1;
                int id4 = id1 + 1;
                
                grid.Elements[counter] = new Element(id1, id2, id3, id4);
                counter++;
                k++;
            }

            k++;
        }
    }

    public static void SetNewNodesTemperature(this Grid grid, IEnumerable<double> temperatures)
    {
        var iterator = 0;
        
        foreach (var temperature in temperatures)
        {
            grid.Nodes[iterator].T0 = temperature;
            iterator++;
        }
    }

    public static void CalculateHAndCMatrix(this Grid grid, Element4_2D element42D, double conductivity, 
        double specificHeat, double density)
    {
        var points = element42D.NKsi.GetLength(1);
        var size1 = element42D.NKsi.GetLength(0);
        
        var gausQuadrature = new GaussQuadrature();

        var scales = new List<(double, double)>();

        scales = points == 9 ? gausQuadrature.Get9PointsScales() : gausQuadrature.Get4PointsScales();

        for (int i = 0; i < grid.Elements.Length; i++)
        {
            var hMatrix = new double[size1, size1];
            var cMatrix = new double[size1, size1];
            
            for (int j = 0; j < points; j++)
            {
                var jacobian = element42D.CalculateOnePointJacobian(i, j, grid);
                //jacobian.PrintJacobian(i, j);

                var matrixes = element42D.CalculateOneHAndCMatrix(j, jacobian.JacobianInverted,
                    conductivity, jacobian.JacobianDet, specificHeat, density, scales.ElementAt(j));

                for (int k = 0; k < size1; k++)
                {
                    for (int l = 0; l < size1; l++)
                    {
                        hMatrix[k, l] += matrixes.HMatrix[k, l];
                        cMatrix[k, l] += matrixes.CMatrix[k, l];
                    }
                }
            }

            grid.Elements[i].HMatrix = hMatrix;
            grid.Elements[i].CMatrix = cMatrix;
        }
    }

    public static void CalculateHbcMatrixAndPVector(this Grid grid, Element4_2D element42D, double[] heatingValues, 
        double alpha)
    {
        var size1 = element42D.NKsi.GetLength(0);

        for (var i = 0; i < grid.Elements.Length; i++)
        {
            var hbcMatrix = new double[size1, size1];
            var pVector = new double[size1];

            for (int j = 0; j < 4; j++)
            {
                var node1 = grid.Nodes[grid.Elements[i].ID[j] - 1];
                var node2 = j == 3 ? grid.Nodes[grid.Elements[i].ID[0] - 1] : grid.Nodes[grid.Elements[i].ID[j + 1] - 1];

                if (node1.BoundaryCondition && node2.BoundaryCondition)
                {
                    double[,] oneHbcMatrix;
                    double[] onePVector;
                    
                    (oneHbcMatrix, onePVector) = element42D.CalculateOneHbcMatrixAndOnePVector(j, alpha, node1,
                        node2, heatingValues[j]);

                    for (int k = 0; k < size1; k++)
                    {
                        for (int l = 0; l < size1; l++)
                        {
                            hbcMatrix[k, l] += oneHbcMatrix[k, l];
                        }
                        pVector[k] += onePVector[k];
                    }
                }
            }

            grid.Elements[i].HbcMatrix = hbcMatrix;
            grid.Elements[i].PVector = pVector;
        }
    }

    public static void CalculateGlobalHMatrixAndGlobalPVector(this Grid mesh)
    {
        foreach (var element in mesh.Elements)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    mesh.GlobalHMatrix[element.ID[i] - 1, element.ID[j] - 1] += element.HMatrix[i, j] + element.HbcMatrix[i, j];
                    mesh.GlobalCMatrix[element.ID[i] - 1, element.ID[j] - 1] += element.CMatrix[i, j];
                }

                mesh.GlobalPVector[element.ID[i] - 1] += element.PVector[i];
            }
        }
    }

    public static void CalculateFullHMatrix(this Grid mesh, int timeStep)
    {
        for (int i = 0; i < mesh.GlobalCMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < mesh.GlobalCMatrix.GetLength(1); j++)
            {
                mesh.FullHMatrix[i, j] = mesh.GlobalHMatrix[i, j] + (mesh.GlobalCMatrix[i, j] / timeStep);
            }
        }
    }

    public static void CalculateFullPVector(this Grid mesh, int timeStep)
    {
        
        for (int i = 0; i < mesh.GlobalPVector.Length; i++)
        {
            var element = 0.0;
            
            for (int j = 0; j < mesh.GlobalPVector.Length; j++)
            {
                element += (mesh.GlobalCMatrix[i, j] / timeStep) * mesh.Nodes[j].T0;
            }

            mesh.FullPVector[i] = mesh.GlobalPVector[i] + element;
        }
    }
}























