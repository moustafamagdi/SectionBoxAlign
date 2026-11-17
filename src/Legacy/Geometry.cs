using System;
namespace SectionBoxAlign {
 public struct V {
  public double X,Y,Z;
  public V(double x,double y,double z){X=x;Y=y;Z=z;}
  public double Length {get{return Math.Sqrt(Dot(this));}}
  public double Dot(V b){return X*b.X+Y*b.Y+Z*b.Z;}
  public V Cross(V b){return new V(Y*b.Z-Z*b.Y,Z*b.X-X*b.Z,X*b.Y-Y*b.X);}
  public V Unit(){double l=Length;if(double.IsNaN(l)||double.IsInfinity(l)||l<1e-10)throw new InvalidOperationException("Pick a valid surface or two distinct points.");return this*(1/l);}
  public static V operator +(V a,V b){return new V(a.X+b.X,a.Y+b.Y,a.Z+b.Z);}
  public static V operator -(V a,V b){return new V(a.X-b.X,a.Y-b.Y,a.Z-b.Z);}
  public static V operator *(V a,double s){return new V(a.X*s,a.Y*s,a.Z*s);}
 }
 public static class Geometry {
  // Native pick normals can contain uninitialized-looking finite values, NaN,
  // infinity or zero. Never normalize those or let them bypass surface sampling.
  public static bool TryPickNormal(V value,out V normal){
   normal=new V();
   if(double.IsNaN(value.X)||double.IsNaN(value.Y)||double.IsNaN(value.Z)||
      Math.Abs(value.X)>1.01||Math.Abs(value.Y)>1.01||Math.Abs(value.Z)>1.01)return false;
   double length=value.Length;if(double.IsNaN(length)||length<0.99||length>1.01)return false;
   normal=value*(1/length);return true;
  }
  const double D=Math.PI/180;
  public static V[] Axes(double[] e){double a=e[0]*D,b=e[1]*D,c=e[2]*D;double ca=Math.Cos(a),sa=Math.Sin(a),cb=Math.Cos(b),sb=Math.Sin(b),cc=Math.Cos(c),sc=Math.Sin(c);
   // OrientedBox JSON is degrees, R = Rx * Ry * Rz (column vectors).
   // Verified against Navisworks 2027 ClipPlanes.GetOrientedBox / Matrix3.
   return new[]{new V(cb*cc,ca*sc+sa*sb*cc,sa*sc-ca*sb*cc),new V(-cb*sc,ca*cc-sa*sb*sc,sa*cc+ca*sb*sc),new V(sb,-sa*cb,ca*cb)};}
  public static double[] Euler(V[] r){double b=Math.Asin(Math.Max(-1,Math.Min(1,r[2].X)));double a,c;
   if(Math.Abs(Math.Cos(b))>1e-8){a=Math.Atan2(-r[2].Y,r[2].Z);c=Math.Atan2(-r[1].X,r[0].X);}else{c=0;a=Math.Atan2(r[1].Z,r[1].Y);}return new[]{a/D,b/D,c/D};}
  // Face alignment treats the normal as an unsigned axis; choose the closest box face.
  public static double[] Align(double[] old,V target,bool face,bool vertical,out int axis){
   target=target.Unit();V[] r=Axes(old);axis=0;
   if(vertical){
    double yaw=Math.Atan2(r[0].Y,r[0].X);if(Math.Sqrt(r[0].X*r[0].X+r[0].Y*r[0].Y)<1e-8)yaw=Math.Atan2(-r[1].X,r[1].Y);
    r=Axes(new[]{0.0,0.0,yaw/D});
    if(face && Math.Abs(target.Z)>1-1e-6){axis=2;return new[]{0.0,0.0,yaw/D};}
    if(face && Math.Abs(target.Z)>1e-6)throw new InvalidOperationException("This face is inclined. Pick a vertical face or use Align to Dir; the box stays vertical.");
    target=new V(target.X,target.Y,0);if(target.Length<1e-8)throw new InvalidOperationException("This direction is vertical. Pick a direction with horizontal separation.");target=target.Unit();
    if(face){axis=Math.Abs(r[1].Dot(target))>Math.Abs(r[0].Dot(target))?1:0;if(r[axis].Dot(target)<0)target=target*(-1);}
    return new[]{0.0,0.0,(Math.Atan2(target.Y,target.X)-axis*Math.PI/2)/D};
   }
   if(face){for(int i=1;i<3;i++)if(Math.Abs(r[i].Dot(target))>Math.Abs(r[axis].Dot(target)))axis=i;if(r[axis].Dot(target)<0)target=target*(-1);}
   V from=r[axis];double cos=Math.Max(-1,Math.Min(1,from.Dot(target)));V cross=from.Cross(target);double sin=cross.Length;
   if(sin<1e-10){if(cos>0)return (double[])old.Clone();cross=r[(axis+1)%3];}else cross=cross*(1/sin);
   for(int i=0;i<3;i++)r[i]=r[i]*cos+cross.Cross(r[i])*sin+cross*(cross.Dot(r[i])*(1-cos));
   return Euler(r);
  }
 }
}
