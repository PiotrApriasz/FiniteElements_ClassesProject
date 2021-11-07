namespace FiniteElementsProject.FormulaPartsLibrary;

public class Jacobian
{
    public double[,] JacobianNormal { get; set; }
    public double JacobianDet { get; set; }
    public double JacobianDetInverted { get; set; }
    public double[,] JacobianInverted { get; set; }
    
    private double[,] JacobianComplement { get; set; }

    public Jacobian(double[,] jacobianNormal, double[,] jacobianComplement, double jacobianDet)
    {
        JacobianNormal = jacobianNormal;
        JacobianComplement = jacobianComplement;
        JacobianDet = jacobianDet;
        
        JacobianDetInverted = 1 / JacobianDet;

        JacobianInverted = new double[JacobianComplement.GetLength(0), JacobianComplement.GetLength(1)];
        
        for (int i = 0; i < JacobianComplement.GetLength(0); i++)
        {
            for (int j = 0; j < JacobianComplement.GetLength(1); j++)
            {
                JacobianInverted[i,j] = JacobianComplement[i, j] * JacobianDetInverted;
            }
        }
    }
    
}