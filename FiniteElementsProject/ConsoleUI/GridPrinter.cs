using FiniteElementsProject.FormulaPartsLibrary;
using FiniteElementsProject.Mesh;

namespace FiniteElementsProject.ConsoleUI;

public static class GridPrinter
{
    public static void PrintNodes(this Grid grid)
    {
        int substractor = 0;
        int i;

        Console.WriteLine("Nodes table for plate:");
        Console.WriteLine();

        for (i = grid.nH - 1; i >= 0; i--)
        {
            for (int j = (grid.nH - substractor - 1); j < grid.nN - substractor; j += grid.nH)
            {
                Console.Write($"Node[{j + 1}] : X = {grid.Nodes[j].X:0.000}, Y = {grid.Nodes[j].Y:0.000}");
                Console.Write(" | ");
            }

            Console.WriteLine();
            substractor++;
        }

        Console.WriteLine();
    }

    public static void PrintIds(this Grid grid)
    {
        int substractor = 0;
        int i;

        Console.WriteLine("Elements ids table for plate:");
        Console.WriteLine();

        for (i = grid.nH - 2; i >= 0; i--)
        {
            for (int j = (grid.nH - substractor - 2); j < grid.nE - substractor; j += grid.nH - 1)
            {
                Console.Write($"Element[{j + 1}] : ID1 = {grid.Elements[j].ID[0]}, ID2 = {grid.Elements[j].ID[1]}," +
                              $" ID3 = {grid.Elements[j].ID[2]}, ID4 = {grid.Elements[j].ID[3]}");
                Console.Write(" | ");
            }

            Console.WriteLine();
            substractor++;
        }

        Console.WriteLine();
    }

    public static void PrintJacobian(this Jacobian jacobian, int el, int pt)
    {
        Console.WriteLine($"Jacobians for element {el + 1}, point: {pt + 1} ------------------------------");
        Console.WriteLine("Jacobian normal -------------------------------------------------");
        Console.WriteLine();
        
        for (int i = 0; i < jacobian.JacobianNormal.GetLength(0); i++)
        {
            for (int j = 0; j < jacobian.JacobianNormal.GetLength(1); j++)
            {
                Console.Write($"{jacobian.JacobianNormal[i, j]:0.00000000} ");
            }

            Console.WriteLine();
        }

        Console.WriteLine();
        Console.WriteLine($"Jacobian normal Det: {jacobian.JacobianDet:0.00000000}");
        Console.WriteLine("-----------------------------------------------------------------");
        
        Console.WriteLine("Jacobian inverted -----------------------------------------------");
        Console.WriteLine();
        
        for (int i = 0; i < jacobian.JacobianNormal.GetLength(0); i++)
        {
            for (int j = 0; j < jacobian.JacobianNormal.GetLength(1); j++)
            {
                Console.Write($"{jacobian.JacobianInverted[i, j]:0.00000000} ");
            }

            Console.WriteLine();
        }
        
        Console.WriteLine();
        Console.WriteLine($"Jacobian inverted Det: {jacobian.JacobianDetInverted:0.00000000}");
        Console.WriteLine("-----------------------------------------------------------------");
        Console.WriteLine();
    }

    public static void PrintHMatrix(this Grid grid)
    {
        for (int i = 0; i < grid.Elements.Length; i++)
        {
            Console.WriteLine($"H matrix for element {i + 1} -------------------------------------------");
            Console.WriteLine();
            for (int j = 0; j < grid.Elements[i].HMatrix.GetLength(0); j++)
            {
                for (int k = 0; k < grid.Elements[i].HMatrix.GetLength(1); k++)
                {
                    Console.Write($"{grid.Elements[i].HMatrix[j, k]:0.000} ");
                }

                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("-------------------------------------------------------------------");
            Console.WriteLine();
        }
    }
}