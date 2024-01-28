class Vector4 {
  double x = 0;
  double y = 0;
  double z = 0;
  double w = 0;

  Vector4(this.x, this.y, this.z, this.w);
}

class RefLink {
  RefLink? next;
  RefLink(this.next);
}
