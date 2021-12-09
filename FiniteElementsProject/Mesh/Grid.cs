using FiniteElementsProject.Elements;
using FiniteElementsProject.FormulaLogic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace FiniteElementsProject.Mesh;

public class Grid
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
    public double[,] GlobalCMatrix { get; set; }
    public double[] GlobalPVector { get; set; }

    public double[,] FullHMatrix { get; set; }
    public double[] FullPVector { get; set; }

    public Vector<double> FiniteElementValues { get; set; }

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
        GlobalCMatrix = new double[nH * nB, nH * nB];
        GlobalPVector = new double[nN];
        
        FullHMatrix = new double[nH * nB, nH * nB];
        FullPVector = new double[nN];

        FiniteElementValues = new DenseVector(nN);
    }
    
    public void PerformFiniteElementsCalculations(Element4_2D element42D, double t0, double[] bcValues)
    {
        this.CalculateNodes(t0);
        this.CalculateIds();
        this.CalculateHAndCMatrix(element42D);
        this.CalculateHbcMatrixAndPVector(element42D, bcValues);
        this.CalculateGlobalHMatrixAndGlobalPVector();
        this.CalculateFullHMatrix(50);
        this.CalculateFullPVector(50);
        
        var hMatrix = Matrix<double>.Build.DenseOfArray(FullHMatrix);
        var pVector = Vector<double>.Build.Dense(FullPVector);
        FiniteElementValues = hMatrix.Solve(pVector);
    }
}