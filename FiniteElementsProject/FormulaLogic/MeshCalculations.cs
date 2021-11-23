using FiniteElementsProject.ConsoleUI;
using FiniteElementsProject.Mesh;
using FiniteElementsProject.Elements;
using FiniteElementsProject.FormulaPartsLibrary;

namespace FiniteElementsProject.FormulaLogic;

public static class MeshCalculations
{
    public static void CalculateNodes(this Grid grid)
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

                var node = new Node() { X = x, Y = y };

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

    public static void CalculateHMatrix(this Grid grid, Element4_2D element42D)
    {
        var points = element42D.NKsi.GetLength(1);
        var size1 = element42D.NKsi.GetLength(0);
        var size2 = element42D.NKsi.GetLength(1);

        for (int i = 0; i < grid.Elements.Length; i++)
        {
            var hMatrix = new double[size1, size1];
            
            for (int j = 0; j < points; j++)
            {
                var jacobian = element42D.CalculateOnePointJacobian(i, grid);
                jacobian.PrintJacobian(i, j);

                var oneHMatrix = element42D.CalculateOneHMatrix(j, jacobian.JacobianInverted, 25, jacobian.JacobianDet);

                for (int k = 0; k < size1; k++)
                {
                    for (int l = 0; l < size1; l++)
                    {
                        hMatrix[k, l] += oneHMatrix[k, l];
                    }
                }
            }

            grid.Elements[i].HMatrix = hMatrix;
        }
    }

    public static void CalculateHbcMatrix(this Grid grid, Element4_2D element42D)
    {
        var size1 = element42D.NKsi.GetLength(0);

        var nodesTemp = new[]
        {
            new Node() { X = 0.0, Y = 0.0, BoundaryCondition = true },
            new Node() { X = 0.025, Y = 0.0, BoundaryCondition = true },
            new Node() { X = 0.025, Y = 0.025 },
            new Node() { X = 0, Y = 0.025, BoundaryCondition = true }
        };
        
        foreach (var element in grid.Elements)
        {
            var hbcMatrix = new double[size1, size1];

            for (int j = 0; j < 4; j++)
            {
                var node1 = grid.Nodes[element.ID[j] - 1];
                var node2 = j == 3 ? grid.Nodes[element.ID[0] - 1] : grid.Nodes[element.ID[j + 1] - 1]; 
                //var node1 = nodesTemp[j];
                //var node2 = j == 3 ? nodesTemp[0] : nodesTemp[j + 1];

                var oneHbcMatrix = new double[4, 4];
                
                if (node1.BoundaryCondition && node2.BoundaryCondition)
                {
                    oneHbcMatrix = element42D.CalculateOneHbcMatrix(j, 25, node1, node2);
                }
                
                element.HbcMatrix.Add(oneHbcMatrix);
            }
        }
    }

    /*public static void CalculateGlobalHMatrix(this Grid mesh)
    {
        var tempXMatrix = new int[mesh.Elements[0].HMatrix.GetLength(0),
            mesh.Elements[0].HMatrix.GetLength(1)];
        
        var tempYMatrix = new int[mesh.Elements[0].HMatrix.GetLength(0),
            mesh.Elements[0].HMatrix.GetLength(1)];

        foreach (var element in mesh.Elements)
        {
            for (int i = 0; i < element.ID.Length; i++)
            {
                for (int j = 0; j < element.ID.Length; j++)
                {
                    tempXMatrix[i, j] =  
                }
            }
        }
    }*/
}























