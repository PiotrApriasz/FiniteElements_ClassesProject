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























