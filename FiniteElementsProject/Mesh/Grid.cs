using FiniteElementsProject.Elements;
using FiniteElementsProject.FormulaLogic;
using FiniteElementsProject.Helpers;
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

    public Vector<double> CalculatedTempValues { get; set; }

    public List<FinalResultsHelper> FinalResults { get; set; }

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

        CalculatedTempValues = new DenseVector(nN);

        FinalResults = new List<FinalResultsHelper>();
    }
    
    public void PerformFiniteElementsCalculations(Element4_2D element42D, double t0, double[] heatingValues, 
        int iterations, int timeStep, double alpha, double specificHeat, double conductivity, double density)
    {
        var time = timeStep;
        
        this.CalculateNodes(t0);
        this.CalculateIds();

        for (int i = 0; i < iterations; i++)
        {
            this.CalculateHAndCMatrix(element42D, conductivity, specificHeat, density);
            this.CalculateHbcMatrixAndPVector(element42D, heatingValues, alpha);
            this.CalculateGlobalHMatrixAndGlobalPVector();
            this.CalculateFullHMatrix(timeStep);
            this.CalculateFullPVector(timeStep);
        
            var hMatrix = Matrix<double>.Build.DenseOfArray(FullHMatrix);
            var pVector = Vector<double>.Build.Dense(FullPVector);
            CalculatedTempValues = hMatrix.Solve(pVector);
            
            FinalResults.Add(new FinalResultsHelper()
            {
                Time = time,
                MinTemp = CalculatedTempValues.Min(),
                MaxTemp = CalculatedTempValues.Max()
            });

            time += timeStep;
        
            this.SetNewNodesTemperature(CalculatedTempValues);
        }
        
    }
}