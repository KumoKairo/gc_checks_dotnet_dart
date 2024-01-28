class Vector4 {
    private double _x;
    private double _y;
    private double _z;
    private double _w;

    public Vector4(double x, double y, double z, double w) {
        this._x = x;
        this._y = y;
        this._z = z;
        this._w = w;
    }
}

class RefLink {
    public RefLink next;
    public RefLink(RefLink next) {
        this.next = next;
    }
}