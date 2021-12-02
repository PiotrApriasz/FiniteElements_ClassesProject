using FiniteElementsProject.ConsoleUI;
using FiniteElementsProject.Elements;
using FiniteElementsProject.FormulaLogic;
using FiniteElementsProject.FormulaPartsLibrary;
using FiniteElementsProject.Helpers;
using FiniteElementsProject.Mesh;
using MathNet.Numerics.LinearAlgebra;

// ReSharper disable InconsistentNaming

try
{
    var mesh = new Grid(0.1, 0.1, 4, 4);
    var element4_2D = new Element4_2D(4);
    
    element4_2D.CalculateIntegrationPoints();
    element4_2D.CalculateNValuesSurface();
    element4_2D.CalculateShapeFuncValues();
    
    //element4_2D.ShFunDer_4_2D_ValPrint();
    
    mesh.PerformFiniteElementsCalculations(element4_2D, 100, new double[]{1200, 1200, 1200, 1200});

    //mesh.PrintIds();
    //mesh.PrintNodes();
    //mesh.PrintHMatrix();
    //mesh.PrintHbcMatrix();
    //mesh.PrintGlobalHMatrix();
    
    mesh.PrintFinalValues();

}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}









