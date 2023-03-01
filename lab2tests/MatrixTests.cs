using lab2;

namespace lab2tests;

[TestClass]
public class MatrixTests {
    private static readonly double[,] Data = {
        { 0.1, 0.2, 0.3 },
        { 0.4, 0.5, 0.6 }
    };

    [TestMethod]
    public void TestConstructor1() {
        var m = new Matrix(3, 5);
        Assert.AreEqual(3, m.Rows);
        Assert.AreEqual(5, m.Columns);

        for (var i = 0; i < m.Rows; i++) {
            for (var j = 0; j < m.Columns; j++) {
                Assert.AreEqual(0.0, m[i, j]);
            }
        }

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Matrix(-1, 5));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Matrix(3, -1));
    }

    [TestMethod]
    public void TestConstructor2() {
        var m = new Matrix(Data);
        Assert.AreEqual(2, m.Rows);
        Assert.AreEqual(3, m.Columns);

        for (var i = 0; i < m.Rows; i++) {
            for (var j = 0; j < m.Columns; j++) {
                Assert.AreEqual(Data[i, j], m[i, j]);
            }
        }
    }

    [TestMethod]
    public void TestIndexation() {
        var m = new Matrix(Data);

        Assert.AreEqual(m[0, 0], 0.1);
        Assert.AreEqual(m[0, 1], 0.2);
        Assert.AreEqual(m[0, 2], 0.3);
        Assert.AreEqual(m[1, 0], 0.4);
        Assert.AreEqual(m[1, 1], 0.5);
        Assert.AreEqual(m[1, 2], 0.6);

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => m[0, -1]);
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => m[0, 3]);
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => m[-1, 0]);
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => m[3, 0]);

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => m[0, -1] = 3);
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => m[0, 3] = 3);
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => m[-1, 0] = 3);
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => m[3, 0] = 3);

        m[1, 1] = 10;
        Assert.AreEqual(m[1, 1], 10);
    }

    [TestMethod]
    public void TestSize() {
        var a = new Matrix(1, 2);
        var b = new Matrix(2, 2);

        Assert.AreEqual(a.Size, null);
        Assert.AreEqual(b.Size, 4);
    }

    [TestMethod]
    public void TestEmpty() {
        var a = new Matrix(3, 5);
        Assert.AreEqual(a.IsEmpty, true);

        a[1, 3] = 4;
        Assert.AreEqual(a.IsEmpty, false);
    }

    [TestMethod]
    public void TestUnity() {
        var a = new Matrix(3, 3);
        var b = new Matrix(Data);
        Assert.AreEqual(a.IsUnity, false);
        Assert.AreEqual(b.IsUnity, false);

        a[0, 0] = 1;
        a[1, 1] = 1;
        a[2, 2] = 1;
        Assert.AreEqual(a.IsUnity, true);

        a[1, 2] = 1;
        Assert.AreEqual(a.IsUnity, false);
    }

    [TestMethod]
    public void TestExplicitConversion() {
        var m = (Matrix)Data;

        Assert.AreEqual(m[0, 0], 0.1);
        Assert.AreEqual(m[0, 1], 0.2);
        Assert.AreEqual(m[0, 2], 0.3);
        Assert.AreEqual(m[1, 0], 0.4);
        Assert.AreEqual(m[1, 1], 0.5);
        Assert.AreEqual(m[1, 2], 0.6);
    }

    [TestMethod]
    public void TestToString() {
        var m = new Matrix(Data);
        Assert.AreEqual(m.ToString(), "[ 0.1, 0.2, 0.3 ]\n[ 0.4, 0.5, 0.6 ]");
    }

    [TestMethod]
    public void TestGetEmpty() {
        var m = Matrix.GetEmpty(3);

        Assert.AreEqual(m.Rows, 3);
        Assert.AreEqual(m.Columns, 3);

        Assert.IsTrue(m.IsEmpty);
    }

    [TestMethod]
    public void TestGetUnity() {
        var m = Matrix.GetUnity(3);

        Assert.AreEqual(m.Rows, 3);
        Assert.AreEqual(m.Columns, 3);

        Assert.IsTrue(m.IsUnity);
    }

    [TestMethod]
    public void TestSymmetric() {
        var m = (Matrix)new double[,] {
            { 1, 2, 17 },
            { 2, 5, -11 },
            { 17, -11, -7 }
        };
        Assert.IsTrue(m.IsSymmetric);

        m[0, 1] = 3;
        Assert.IsFalse(m.IsSymmetric);

        var n = (Matrix)Data;
        Assert.IsFalse(n.IsSymmetric);
    }

    [TestMethod]
    public void TestOperatorAddition() {
        var a = (Matrix)Data;
        var b = (Matrix)Data;
        var c = new Matrix(3, 3);
        var d = new Matrix(2, 2);

        Assert.ThrowsException<ArgumentException>(() => a + c);
        Assert.ThrowsException<ArgumentException>(() => a + d);

        var e = a + b;
        Assert.AreEqual(e[0, 0], 0.2);
        Assert.AreEqual(e[0, 1], 0.4);
        Assert.AreEqual(e[0, 2], 0.6);
        Assert.AreEqual(e[1, 0], 0.8);
        Assert.AreEqual(e[1, 1], 1.0);
        Assert.AreEqual(e[1, 2], 1.2);
        
        var rnd = new Random();
        var m1 = new Matrix(10, 20);
        var m2 = new Matrix(10, 20);

        for (var i = 0; i < m1.Rows; i++) {
            for (var j = 0; j < m2.Columns; j++) {
                m1[i, j] = rnd.NextDouble() * 2 - 1;
                m2[i, j] = rnd.NextDouble() * 2 - 1;
            }
        }

        var m3 = m1 + m2;
        for (var i = 0; i < m1.Rows; i++) {
            for (var j = 0; j < m2.Columns; j++) {
                Assert.AreEqual(m1[i, j] + m2[i, j], m3[i, j]);
            }
        }
    }

    [TestMethod]
    public void TestSquared() {
        var a = new Matrix(3, 3);
        Assert.IsTrue(a.IsSquared);

        var b = (Matrix)Data;
        Assert.IsFalse(b.IsSquared);
    }

    [TestMethod]
    public void TestParse() {
        var a = Matrix.Parse("1 2 3, 4 5 6, 7 8 9");
        CheckParseResult(a);
        
        a = Matrix.Parse("1 2 3,4 5 6,7 8 9");
        CheckParseResult(a);
        
        a = Matrix.Parse("1 2 3,                          4 5 6,7 8 9");
        CheckParseResult(a);
        
        a = Matrix.Parse("    1    2 3,4 5 6,7    8    9 ");
        CheckParseResult(a);

        Assert.ThrowsException<FormatException>(() => Matrix.Parse("1 2 3, 4 5 6 7, 7 8 9"));
        Assert.ThrowsException<FormatException>(() => Matrix.Parse("1 2 3,, 4 5 6, 7 8 9"));
        Assert.ThrowsException<FormatException>(() => Matrix.Parse(","));

        a = Matrix.Parse("    ");
        Assert.AreEqual(a.Rows, 0);
    }

    private static void CheckParseResult(Matrix m) {
        Assert.AreEqual(m[0, 0], 1);
        Assert.AreEqual(m[0, 1], 2);
        Assert.AreEqual(m[0, 2], 3);
        Assert.AreEqual(m[1, 0], 4);
        Assert.AreEqual(m[1, 1], 5);
        Assert.AreEqual(m[1, 2], 6);
        Assert.AreEqual(m[2, 0], 7);
        Assert.AreEqual(m[2, 1], 8);
        Assert.AreEqual(m[2, 2], 9);
    }

    [TestMethod]
    public void TestTranspose() {
        var a = ((Matrix)Data).Transpose();
        
        Assert.AreEqual(a.Rows, 3);
        Assert.AreEqual(a.Columns, 2);
        
        Assert.AreEqual(a[0, 0], 0.1);
        Assert.AreEqual(a[0, 1], 0.4);
        Assert.AreEqual(a[1, 0], 0.2);
        Assert.AreEqual(a[1, 1], 0.5);
        Assert.AreEqual(a[2, 0], 0.3);
        Assert.AreEqual(a[2, 1], 0.6);
    }
}