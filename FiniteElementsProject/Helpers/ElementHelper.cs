using FiniteElementsProject.Mesh;

namespace FiniteElementsProject.Helpers;

public static class ElementHelper
{
    public static double CalculateSideLenght(Node node1, Node node2)
    {
        return (Math.Sqrt(Math.Pow((node2.X - node1.X), 2) + Math.Pow((node2.Y - node1.Y), 2))) / 2;
    }
}