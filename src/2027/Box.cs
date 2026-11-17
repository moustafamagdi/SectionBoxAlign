using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;
namespace SectionBoxAlign {
 public sealed class Box {
  public readonly Dictionary<string,object> Root;
  readonly Dictionary<string,object> oriented;
  static readonly JavaScriptSerializer Serializer=new JavaScriptSerializer();
  public Box(string json){Root=Serializer.Deserialize<Dictionary<string,object>>(json);object o;
   if(Root==null||!Root.TryGetValue("OrientedBox",out o)||(oriented=o as Dictionary<string,object>)==null)throw new InvalidOperationException("Enable Sectioning and choose Box mode first. Your existing box will be preserved.");
   if(!Root.ContainsKey("Enabled")||!Convert.ToBoolean(Root["Enabled"]))throw new InvalidOperationException("Enable your Section Box first.");
   var bounds=(IList)oriented["Box"];Min=Read(bounds[0]);Max=Read(bounds[1]);Rotation=Read(oriented["Rotation"]);
   for(int i=0;i<3;i++)if(!Finite(Min[i])||!Finite(Max[i])||!Finite(Rotation[i])||Max[i]<=Min[i])throw new InvalidOperationException("The section box has invalid bounds or rotation.");
  }
  static bool Finite(double v){return !double.IsNaN(v)&&!double.IsInfinity(v);}
  static double[] Read(object o){var a=(IList)o;if(a.Count!=3)throw new InvalidOperationException("Unsupported section box format.");return new[]{Convert.ToDouble(a[0]),Convert.ToDouble(a[1]),Convert.ToDouble(a[2])};}
  public double[] Min,Max,Rotation;
  public string WithRotation(double[] r){foreach(double v in r)if(!Finite(v))throw new InvalidOperationException("Invalid calculated rotation.");oriented["Rotation"]=r;return Serializer.Serialize(Root);}
  public void VerifyBounds(Box other){for(int i=0;i<3;i++)if(Math.Abs(Min[i]-other.Min[i])>1e-7||Math.Abs(Max[i]-other.Max[i])>1e-7)throw new InvalidOperationException("Navisworks changed the box bounds; the previous box was restored.");}
 }
}
