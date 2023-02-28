namespace lab2;

public class Matrix {
    private const double Tolerance = 1e-6;
    private double[,] _data;

    public Matrix(int nRows, int nCols) {
        if (nRows < 0 || nCols < 0) {
            throw new ArgumentOutOfRangeException();
        }

        _data = new double[nRows, nCols];
    }

    public Matrix(double[,] initData) : this(initData.GetLength(0), initData.GetLength(1)) {
        Array.Copy(initData, 0, _data, 0, initData.Length);
    }

    public double this[int i, int j] {
        get {
            if (i < 0 || i >= Rows || j < 0 || j >= Columns) {
                throw new ArgumentOutOfRangeException();
            }

            return _data[i, j];
        }

        set {
            if (i < 0 || i >= Rows || j < 0 || j >= Columns) {
                throw new ArgumentOutOfRangeException();
            }

            _data[i, j] = value;
        }
    }

    public int Rows => _data.GetLength(0);

    public int Columns => _data.GetLength(1);
    
    public int? Size => IsSquared ? _data.Length : null;

    public bool IsSquared => Rows == Columns;

    public bool IsEmpty => _data.Cast<double>().All(num => num == 0);

    public bool IsUnity {
        get {
            if (!IsSquared) {
                return false;
            }

            for (int i = 0; i < Rows; i++) {
                for (int j = 0; j < Columns; j++) {
                    if (i == j) {
                        if (Math.Abs(_data[i, j] - 1) > Tolerance) {
                            return false;
                        }
                    } else if (Math.Abs(_data[i, j]) > Tolerance) {
                        return false;
                    }
                }
            }

            return true;
        }
    }

    public static explicit operator Matrix(double[,] arr) {
        throw new NotImplementedException();
    }

    public override string ToString() {
        return base.ToString();
    }

    public static Matrix GetUnity(int Size) {
        throw new NotImplementedException();
    }
    
    public static Matrix GetEmpty(int Size) {
        throw new NotImplementedException();
    }

    public bool IsSymmetric {
        get {
            throw new NotImplementedException();
        }
    }

    public static Matrix operator +(Matrix m1, Matrix m2) {
        throw new NotImplementedException();
    }

    public Matrix Transpose() {
        throw new NotImplementedException();
    }

    public static Matrix Parse(string s) {
        throw new NotImplementedException();
    }
}