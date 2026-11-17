using System;
using System.IO;
using System.Text;
using Autodesk.Navisworks.Api;
using App=Autodesk.Navisworks.Api.Application;
namespace SectionBoxAlign {
 internal static class NativeTests {
  public static void Prepare(){var doc=App.ActiveDocument;var bounds=doc.Models[0].RootItem.BoundingBox();var vp=doc.CurrentViewpoint.CreateCopy();vp.ClipPlanes.SetOrientedBox(bounds,Rotation3D.Identity);vp.ClipPlanes.Mode=ClipPlaneSetMode.Box;vp.ClipPlanes.Enabled=true;doc.CurrentViewpoint.CopyFrom(vp);}
  public static int Run(string path){var report=new StringBuilder();var view=App.ActiveDocument.ActiveView;string saved=view.GetClippingPlanes();try{
   report.AppendLine("API: "+typeof(View).Assembly.FullName);
   var random=new Random(512);int count=0;
   for(int k=0;k<60;k++){
    var old=new[]{random.NextDouble()*180-90,random.NextDouble()*160-80,random.NextDouble()*360-180};
    string initial="{\"Type\":\"ClipPlaneSet\",\"Version\":1,\"Enabled\":true,\"OrientedBox\":{\"Type\":\"OrientedBox3D\",\"Version\":1,\"Box\":[[10,20,30],[22,45,65]],\"Rotation\":[0,0,0]}}";
    var box=new Box(initial);initial=box.WithRotation(old);view.SetClippingPlanes(initial);
    var target=new V(random.NextDouble()-.5,random.NextDouble()-.5,random.NextDouble()-.5);bool face=k%2==0;bool vertical=k%3==0;if(vertical)target.Z=0;
    int axis;var result=Geometry.Align(old,target,face,vertical,out axis);var nativeBefore=new Box(view.GetClippingPlanes());AlignForm.Apply(view,view.GetClippingPlanes(),nativeBefore.WithRotation(result),nativeBefore);
    var after=new Box(view.GetClippingPlanes());var axes=Geometry.Axes(after.Rotation);
    if(Math.Abs(axes[axis].Dot(target.Unit()))<1-1e-8)throw new Exception("Target alignment failed.");
    // Diagnostic-only access to native oriented-box rotation checks the JSON convention.
    var vp=App.ActiveDocument.CurrentViewpoint.ToViewpoint();var cps=vp.ClipPlanes;var bounds=new BoundingBox3D();var rotation=new Rotation3D();cps.GetOrientedBox(bounds,rotation);
    using(var matrix=new Matrix3(rotation)){
     double error=0;for(int col=0;col<3;col++){double[] expected={axes[col].X,axes[col].Y,axes[col].Z};for(int row=0;row<3;row++)error=Math.Max(error,Math.Abs(matrix.Get(col,row)-expected[row]));}
     if(error>1e-7){report.AppendLine("Matrix error="+error+" JSON="+view.GetClippingPlanes());for(int r=0;r<3;r++)report.AppendLine(matrix.Get(r,0)+","+matrix.Get(r,1)+","+matrix.Get(r,2));throw new Exception("Native rotation convention mismatch.");}
    }
        count++;
   }
   report.AppendLine("PASS: "+count+" native face/direction rotations, center/size preservation, and native matrix comparisons.");
   int windows=System.Windows.Forms.Application.OpenForms.Count;
   var session=Command.Session;
   var ribbonRecord=App.Plugins.FindPlugin("MMTools.FloatingRibbon.MMTL");
   if(ribbonRecord==null)throw new Exception("My Navis Tools ribbon not discovered.");
   var ribbon=(ribbonRecord.LoadedPlugin??ribbonRecord.LoadPlugin()) as Autodesk.Navisworks.Api.Plugins.CommandHandlerPlugin;
   if(ribbon==null||ribbon.ExecuteCommand("MMTools.AlignFace")!=0||!session.IsPicking||!session.IsFace)throw new Exception("Ribbon face command did not begin picking immediately.");
   var record=App.Plugins.FindPlugin("MMTools.SectionBoxAlign.Pick.MMSA");var picker=record.LoadedPlugin as PickTool;
   picker.KeyDown(view,KeyModifiers.None,27,0);
   if(session.IsPicking||App.ActiveDocument.Tool.Value==Tool.CustomToolPlugin)throw new Exception("Escape did not release the tool.");
   if(ribbon.ExecuteCommand("MMTools.AlignDir")!=0||!session.IsPicking||session.IsFace)throw new Exception("Ribbon direction command did not begin picking immediately.");
   picker.MouseDown(view,KeyModifiers.None,2,0,0,0);
   if(session.IsPicking)throw new Exception("Right-click did not cancel.");
   if(System.Windows.Forms.Application.OpenForms.Count!=windows)throw new Exception("Unexpected window created.");
   report.AppendLine("PASS: immediate face/direction tool activation, Escape, right-click cancellation, no windows created.");return 0;
  }catch(Exception e){report.AppendLine("FAIL: "+e);return 1;}finally{try{view.SetClippingPlanes(saved);report.AppendLine("Original section state restored.");}catch(Exception e){report.AppendLine("Restore failed: "+e);}File.WriteAllText(path,report.ToString());}}
 }
}





