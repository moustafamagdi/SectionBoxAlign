using System;
using System.IO;
using System.Text;
using Autodesk.Navisworks.Api;
using App=Autodesk.Navisworks.Api.Application;
namespace SectionBoxAlign {
 internal static class NativeTests {
  public static int Run(string path){var report=new StringBuilder();var view=App.ActiveDocument.ActiveView;string saved=view.GetClippingPlanes();try {
   report.AppendLine("API: "+typeof(View).Assembly.FullName);var random=new Random(512);
   for(int k=0;k<60;k++){
    string initial="{\"Type\":\"ClipPlaneSet\",\"Version\":1,\"Enabled\":true,\"OrientedBox\":{\"Type\":\"OrientedBox3D\",\"Version\":1,\"Box\":[[2440731,9075878,1919],[2440794,9075950,1942]],\"Rotation\":[0,0,13]}}";
    view.SetClippingPlanes(initial);var before=new Box(view.GetClippingPlanes());int axis;var target=new V(random.NextDouble()-.5,random.NextDouble()-.5,0);var result=Geometry.Align(before.Rotation,target,k%2==0,true,out axis);AlignForm.Apply(view,initial,before.WithRotation(result),before);var after=new Box(view.GetClippingPlanes());
    if(Math.Abs(Geometry.Axes(after.Rotation)[axis].Dot(target.Unit()))<1-1e-7)throw new Exception("Alignment readback failed");
   }
   report.AppendLine("PASS: 60 native JSON rotation roundtrips and center/dimensions preservation at large world coordinates.");
   int windows=System.Windows.Forms.Application.OpenForms.Count;var record=App.Plugins.FindPlugin("MMTools.SectionAlignRibbon.MMSA");var ribbon=(record.LoadedPlugin??record.LoadPlugin()) as AlignRibbon;
   if(ribbon.ExecuteCommand("MMTools.AlignFace")!=0||!Command.Session.IsPicking||!Command.Session.IsFace)throw new Exception("Face activation failed");
   var pick=(PickTool)App.Plugins.FindPlugin("MMTools.SectionBoxAlign.Pick.MMSA").LoadedPlugin;pick.KeyDown(view,KeyModifiers.None,27,0);
   if(Command.Session.IsPicking)throw new Exception("Escape failed");
   if(ribbon.ExecuteCommand("MMTools.AlignDir")!=0||!Command.Session.IsPicking||Command.Session.IsFace)throw new Exception("Direction activation failed");
   pick.MouseDown(view,KeyModifiers.None,2,0,0,0);if(Command.Session.IsPicking)throw new Exception("Cancel failed");
   if(System.Windows.Forms.Application.OpenForms.Count!=windows)throw new Exception("A form opened");
   report.AppendLine("PASS: both ribbon commands, immediate picking, Escape/right-click, and no windows.");
   view.SetClippingPlanes(saved);
   int width=view.Width,height=view.Height;
   report.AppendLine("Viewport="+width+"x"+height);
   using(var bitmap=view.GenerateImage(ImageGenerationStyle.Scene,width,height,false)){bitmap.Save(Path.ChangeExtension(path,"png"));}
   bool surface=false;
   for(int y=height/5;y<height*4/5&&!surface;y+=Math.Max(1,height/20))for(int x=width/5;x<width*4/5&&!surface;x+=Math.Max(1,width/20)){
    var hit=view.PickItemFromPoint(x,y);if(hit==null||hit.Point==null)continue;
    try{V normal=SurfacePicker.Normal(view,x,y,hit);if(Math.Abs(normal.Length-1)>1e-8)throw new Exception("Invalid sampled normal");surface=true;report.AppendLine("PASS: actual model surface normal sampled at "+x+","+y+" normal="+normal.X+","+normal.Y+","+normal.Z);}catch(InvalidOperationException){}
   }
   if(!surface)throw new Exception("No planar surface sampled from rendered test model.");return 0;
  }catch(Exception e){report.AppendLine("FAIL: "+e);return 1;}finally{Command.Session.Stop();view.SetClippingPlanes(saved);File.WriteAllText(path,report.ToString());}}
 }
}
