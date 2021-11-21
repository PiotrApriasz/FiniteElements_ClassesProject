namespace FiniteElementsProject.Mesh;

public struct Grid
{
    public double H { get; set; }
    public double B { get; set; }
    public int nH { get; set; }
    public int nB { get; set; }
    public int nN { get; set; }
    public int nE { get; set; }

    public Node[] Nodes { get; set; }
    public Element[] Elements { get; set; }

    public double[,] GlobalHMatrix { get; set; }

    public Grid(double h, double b, int nh, int nb)
    {
        H = h;
        B = b;
        nH = nh;
        nB = nb;

        nN = nH * nB;
        nE = (nH - 1) * (nB - 1);

        Nodes = new Node[nN];
        Elements = new Element[nE];

        GlobalHMatrix = new double[nH * nB, nH * nB];
    }
}