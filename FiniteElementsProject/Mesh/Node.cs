namespace FiniteElementsProject.Mesh;

public struct Node
{
    public double X { get; set; }
    public double Y { get; set; }
    public double T0 { get; set; }

    public bool BoundaryCondition { get; set; } = false;
}