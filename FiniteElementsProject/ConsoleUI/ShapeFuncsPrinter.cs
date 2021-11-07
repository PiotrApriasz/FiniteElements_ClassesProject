using FiniteElementsProject.Elements;

namespace FiniteElementsProject.ConsoleUI;

public static class ShapeFuncsPrinter
{
    public static void ShFunDer_4_2D_ValPrint(this Element4_2D element42D)
    {
        var points = element42D.NKsi.GetLength(1);
        Console.WriteLine("Shape functions values for dN/dKsi");
        Console.WriteLine();

        for (int i = 0; i < points; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Console.Write($"{element42D.NKsi[j, i]:0.000000} | ");
            }

            Console.WriteLine();
        }

        Console.WriteLine();
        Console.WriteLine("Shape functions values for dN/dEta");
        Console.WriteLine();

        for (int i = 0; i < points; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Console.Write($"{element42D.NEta[j, i]:0.000000} | ");
            }

            Console.WriteLine();
        }

        Console.WriteLine();
        Console.WriteLine();
    }
}