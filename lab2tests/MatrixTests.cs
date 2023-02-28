using lab2;

namespace lab2tests;

[TestClass]
public class MatrixTests {
    private static readonly double[,] data = {
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
                Assert.AreEqual(0.0, m[i,j]);
            }
        }

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Matrix(-1, 5));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Matrix(3, -1));
    }

    [TestMethod]
    public void TestConstructor2() {
        var m = new Matrix(data);
        Assert.AreEqual(2, m.Rows);
        Assert.AreEqual(3, m.Columns);

        for (int i = 0; i < m.Rows; i++) {
            for (int j = 0; j < m.Columns; j++) {
                Assert.AreEqual(data[i,j], m[i,j]);
            }
        }
    }

    [TestMethod]
    public void TestIndexation() {
        var m = new Matrix(data);
        
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
        var b = new Matrix(data);
        Assert.AreEqual(a.IsUnity, false);
        Assert.AreEqual(b.IsUnity, false);

        a[0, 0] = 1;
        a[1, 1] = 1;
        a[2, 2] = 1;
        Assert.AreEqual(a.IsUnity, true);

        a[1, 2] = 1;
        Assert.AreEqual(a.IsUnity, false);
    }
}