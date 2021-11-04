namespace FiniteElementsProject.Mesh;

public struct Element
{
    public int[] ID { get; set; }

    public Element(int id1, int id2, int id3, int id4)
    {
        ID = new[] { id1, id2, id3, id4 };
    }
}