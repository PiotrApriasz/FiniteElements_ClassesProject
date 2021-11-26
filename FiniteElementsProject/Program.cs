using FiniteElementsProject.ConsoleUI;
using FiniteElementsProject.Elements;
using FiniteElementsProject.FormulaLogic;
using FiniteElementsProject.FormulaPartsLibrary;
using FiniteElementsProject.Helpers;
using FiniteElementsProject.Mesh;
// ReSharper disable InconsistentNaming

try
{
    var mesh = new Grid(0.1, 0.1, 4, 4);
    var element4_2D = new Element4_2D(4);
    element4_2D.CalculateIntegrationPoints();
    element4_2D.CalculateNValues();

    mesh.CalculateNodes();
    mesh.CalculateIds();
    
    mesh.PrintIds();
    mesh.PrintNodes();

    element4_2D.CalculateShapeFuncDerValues();
    element4_2D.ShFunDer_4_2D_ValPrint();

    mesh.CalculateHMatrix(element4_2D);
    mesh.PrintHMatrix();
    
    mesh.CalculateHbcMatrixAndPVector(element4_2D, 1200);
    mesh.PrintHbcMatrix();
    
    mesh.CalculateGlobalHMatrix();
    mesh.PrintGlobalHMatrix();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}









