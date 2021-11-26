namespace FiniteElementsProject.Mesh;

public struct Element
{
    public int[] ID { get; set; }
    public double[,] HMatrix { get; set; }
    public double[,] HbcMatrix { get; set; }
    public double[] PVector { get; set; }

    public Element(int id1, int id2, int id3, int id4)
    {
        ID = new[] { id1, id2, id3, id4 };
        HMatrix = new double[4,4];
        HbcMatrix = new double[4,4];
        PVector = new double[4];
    }
}