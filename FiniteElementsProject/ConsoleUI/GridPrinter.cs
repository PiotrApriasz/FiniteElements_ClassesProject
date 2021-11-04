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
}