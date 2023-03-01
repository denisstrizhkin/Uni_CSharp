using System.Text.RegularExpressions;

namespace lab2;

public class Matrix {
    private const double Tolerance = 1e-12;
    private readonly double[,] _data;

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

            for (var i = 0; i < Rows; i++) {
                for (var j = 0; j < Columns; j++) {
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

    public static explicit operator Matrix(double[,] arr) => new(arr);

    public override string ToString() {
        return string.Join('\n', _data.Cast<double>()
            .Select((item, index) => new { item, index })
            .GroupBy(x => x.index / Columns)
            .Select(x => $"[ {string.Join(", ", x.Select(y => y.item))} ]"));
    }

    public static Matrix GetUnity(int size) {
        var m = new Matrix(size, size);

        for (var i = 0; i < size; i++) {
            m[i, i] = 1;
        }

        return m;
    }

    public static Matrix GetEmpty(int size) => new(size, size);

    public bool IsSymmetric {
        get {
            if (!IsSquared) {
                return false;
            }
            
            for (var i = 0; i < Rows; i++) {
                for (var j = i + 1; j < Columns; j++) {
                    if (Math.Abs(_data[i, j] - _data[j, i]) > Tolerance) {
                        return false;
                    }
                }
            }

            return true;
        }
    }

    public static Matrix operator +(Matrix m1, Matrix m2) {
        if (m1.Rows != m2.Rows || m1.Columns != m2.Columns) {
            throw new ArgumentException();
        }

        var r = new Matrix(m1.Rows, m1.Columns);
        for (var i = 0; i < r.Rows; i++) {
            for (var j = 0; j < r.Columns; j++) {
                r[i, j] = m1[i, j] + m2[i, j];
            }
        }

        return r;
    }

    public Matrix Transpose() {
        var r = new Matrix(Columns, Rows);
        
        for (var i = 0; i < Rows; i++) {
            for (var j = 0; j < Columns; j++) {
                r[j, i] = _data[i, j];
            }
        }

        return r;
    }

    public static Matrix Parse(string s) {
        var str = Regex.Replace(s.Trim(), @"[ ]{2,}", " ");
        str = Regex.Replace(str, @"\,[ ]", ",");

        if (str.Length == 0) {
            return GetEmpty(0);
        }

        var strNums = str.Split(',').Select(x => x.Split()).ToList();
        if (strNums.Any(x => x.Length != strNums.ElementAt(0).Length)) {
            throw new FormatException();
        }

        var m = new Matrix(strNums.Count(), strNums.ElementAt(0).Length);
        for (var i = 0; i < m.Rows; i++) {
            for (var j = 0; j < m.Columns; j++) {
                m[i, j] = Convert.ToDouble(strNums.ElementAt(i).ElementAt(j));
            }
        }

        return m;
    }
}