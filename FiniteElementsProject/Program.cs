﻿using FiniteElementsProject.ConsoleUI;
using FiniteElementsProject.Elements;
using FiniteElementsProject.FormulaLogic;
using FiniteElementsProject.FormulaPartsLibrary;
using FiniteElementsProject.Mesh;
// ReSharper disable InconsistentNaming

try
{
    var mesh = new Grid(0.2, 0.1, 5, 4);
    var element4_2D = new Element4_2D(4);

    mesh.CalculateNodes();
    mesh.CalculateIds();
    
    mesh.PrintIds();
    mesh.PrintNodes();

    element4_2D.CalculateShapeFuncDerValues();
    element4_2D.ShFunDer_4_2D_ValPrint();

    mesh.CalculateHMatrix(element4_2D);
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}









