using System;
using Autodesk.Navisworks.Api;
namespace SectionBoxAlign {
 internal static class SurfacePicker {
  static V Position(PickItemResult h){return new V(h.Point.X,h.Point.Y,h.Point.Z);}
  // Some loaders return a zero Normal. Sample actual world-space surface hits,
  // never screen-space directions, and reject edges and nonplanar patches.
  public static V Normal(View view,int x,int y,PickItemResult center){
   if(center.Normal!=null){V normal;var n=new V(center.Normal.X,center.Normal.Y,center.Normal.Z);if(Geometry.TryPickNormal(n,out normal))return normal;}
   Log.Write("Native surface normal unavailable or invalid; using verified surface samples.");
   int[,] offsets={{-1,0},{1,0},{0,-1},{0,1},{-1,-1},{1,1},{-1,1},{1,-1}};
   foreach(int radius in new[]{3,2,1}){
    V[] p=new V[8];bool valid=true;
    for(int i=0;i<8;i++){int px=x+radius*offsets[i,0],py=y+radius*offsets[i,1];if(px<0||py<0||px>=view.Width||py>=view.Height){valid=false;break;}
     var h=view.PickItemFromPoint(px,py,1,true,PickPrimitives.Point|PickPrimitives.Distance|PickPrimitives.CenterExactSurface);
     if(h==null||h.Point==null||h.ModelItem==null||!h.ModelItem.Equals(center.ModelItem)){valid=false;break;}p[i]=Position(h);
    }
    if(!valid)continue;V a=p[1]-p[0],b=p[3]-p[2];double scale=Math.Max(a.Length,b.Length);V cross=a.Cross(b);
    if(scale<1e-8||cross.Length<1e-5*a.Length*b.Length)continue;V normal=cross.Unit(),origin=Position(center);
    foreach(V q in p)if(Math.Abs((q-origin).Dot(normal))>Math.Max(1e-7,scale*1e-5)){valid=false;break;}
    if(valid){Log.Write("Surface normal from verified planar patch; radius="+radius);return normal;}
   }
   throw new InvalidOperationException("Cannot resolve a planar face here. Zoom in and click away from edges, or use Align to Direction.");
  }
 }
}
