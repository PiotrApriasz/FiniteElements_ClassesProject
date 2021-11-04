namespace FiniteElementsProject.FormulaPartsLibrary;

public struct Jacobian
{
    public double[,] JacobianNormal { get; set; }
    public double[,] JacobianInverted { get; set; }
    public double JacobianDet { get; set; }
    
}