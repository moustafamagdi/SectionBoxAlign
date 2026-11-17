using System;
using SectionBoxAlign;
class MathTests {
 static int checks;
 static void Check(bool ok,string m){checks++;if(!ok)throw new Exception(m);}
 static void Reject(Action a){try{a();}catch(InvalidOperationException){checks++;return;}throw new Exception("Expected rejection");}
 static void Main(){var random=new Random(79);int axis;V normalized;
  foreach(var invalid in new[]{new V(0,0,6.5296554895875689e235),new V(0,0,0),new V(double.NaN,0,0),new V(0,double.PositiveInfinity,0),new V(0,0,double.NegativeInfinity),new V(0,0,1e-200),new V(1,1,1)})Check(!Geometry.TryPickNormal(invalid,out normalized),"Invalid native normal must fall back");
  foreach(var valid in new[]{new V(0,0,1),new V(-1,0,0),new V(0.6,0.8,0)}){Check(Geometry.TryPickNormal(valid,out normalized),"Valid native normal");Check(Math.Abs(normalized.Length-1)<1e-12,"Normalized native normal");}
  for(int k=0;k<2000;k++){
   var old=new[]{random.NextDouble()*360-180,random.NextDouble()*360-180,random.NextDouble()*360-180};var t=new V(random.NextDouble()-.5,random.NextDouble()-.5,random.NextDouble()-.5).Unit();
   foreach(bool face in new[]{false,true})foreach(bool vertical in new[]{false,true}){
    var target=vertical?new V(t.X,t.Y,0).Unit():t;var e=Geometry.Align(old,target,face,vertical,out axis);var a=Geometry.Axes(e);
    Check((face?Math.Abs(a[axis].Dot(target)):a[axis].Dot(target))>1-1e-8,"Alignment");Check(Math.Abs(a[0].Dot(a[1]))<1e-8&&a[0].Cross(a[1]).Dot(a[2])>1-1e-8,"Orthonormal frame");if(vertical)Check(a[2].Z>1-1e-8,"Keep Vertical");
   }
  }
  foreach(double pitch in new[]{90.0,-90.0}){var a=Geometry.Axes(new[]{37.0,pitch,-61.0});var b=Geometry.Axes(Geometry.Euler(a));for(int i=0;i<3;i++)Check((a[i]-b[i]).Length<1e-7,"Gimbal lock roundtrip");}
  var zero=new[]{0.0,0.0,0.0};var reverse=Geometry.Axes(Geometry.Align(zero,new V(-1,0,0),false,false,out axis));Check(reverse[0].X<-.999999,"Antiparallel direction");
  Reject(()=>Geometry.Align(zero,new V(0,0,0),false,false,out axis));Reject(()=>Geometry.Align(zero,new V(0,0,1),false,true,out axis));Reject(()=>Geometry.Align(zero,new V(1,0,1),true,true,out axis));
  var top=Geometry.Axes(Geometry.Align(new[]{12.0,23.0,41.0},new V(0,0,1),true,true,out axis));Check(axis==2&&top[2].Z>.999999,"Horizontal face");
  string json="{\"Enabled\":true,\"Metadata\":\"keep\",\"OrientedBox\":{\"Box\":[[101,202,303],[105,209,312]],\"Rotation\":[15,25,35]}}";var box=new Box(json);string changed=box.WithRotation(new[]{37.0,-23.0,71.0});box.VerifyBounds(new Box(changed));Check(changed.Contains("keep"),"Metadata preserved");
  Console.WriteLine("PASS: "+checks+" checks; 8000 alignments, Euler singularities, rejection paths, bounds and metadata.");
 }
}
