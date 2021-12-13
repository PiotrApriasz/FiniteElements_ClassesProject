using FiniteElementsProject.ConsoleUI;
using FiniteElementsProject.Elements;
using FiniteElementsProject.FormulaLogic;
using FiniteElementsProject.FormulaPartsLibrary;
using FiniteElementsProject.Helpers;
using MathNet.Numerics.LinearAlgebra;
using Spectre.Console;
using Grid = FiniteElementsProject.Mesh.Grid;

// ReSharper disable InconsistentNaming

try
{
    var mesh = new Grid(0.1, 0.1, 31, 31);
    var element4_2D = new Element4_2D(9);
    
    element4_2D.CalculateIntegrationPoints();
    element4_2D.CalculateNValuesSurface();
    element4_2D.CalculateShapeFuncValues();

    mesh.PerformFiniteElementsCalculations(element4_2D, 100, new double[]{1200, 1200, 1200, 1200}, 
        100, 1, 300, 700, 25, 7800);
            
    mesh.PrintFinalValues();

}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}









