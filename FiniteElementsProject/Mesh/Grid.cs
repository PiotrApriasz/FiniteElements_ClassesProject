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
    public double[] GlobalPVector { get; set; }

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
        GlobalPVector = new double[nN];

        FiniteElementValues = new DenseVector(nN);
    }

    /// <summary>
    /// Function which executes all necessary operartions to get final
    /// values of searched variable (ex. each nodes temperature or pressure)
    /// </summary>
    /// <param name="element42D">Base element</param>
    /// <param name="bcValues">Values of boundary conditions affecting the mesh edges</param>
    public void PerformFiniteElementsCalculations(Element4_2D element42D, double[] bcValues)
    {
        this.CalculateNodes();
        this.CalculateIds();
        this.CalculateHMatrix(element42D);
        this.CalculateHbcMatrixAndPVector(element42D, bcValues);
        this.CalculateGlobalHMatrixAndGlobalPVector();
        
        var hMatrix = Matrix<double>.Build.DenseOfArray(GlobalHMatrix);
        var pVector = Vector<double>.Build.Dense(GlobalPVector);
        FiniteElementValues = hMatrix.Solve(pVector);
    }
}