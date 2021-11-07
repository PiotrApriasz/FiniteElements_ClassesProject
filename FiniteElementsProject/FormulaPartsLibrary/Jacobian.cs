namespace FiniteElementsProject.FormulaPartsLibrary;

public struct Jacobian
{
    public double[,] JacobianNormal { get; set; }
    public double[,] JacobianComplement { get; set; }
    public double JacobianDet { get; set; }
    
}