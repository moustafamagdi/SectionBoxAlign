using System;
using System.IO;
using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Plugins;
using App=Autodesk.Navisworks.Api.Application;
[assembly:System.Reflection.AssemblyVersion("1.1.0.0")]
namespace SectionBoxAlign {
 internal static class Log {
  public static string Folder=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),"SectionBoxAlign Logs");
  public static void Write(string text){try{Directory.CreateDirectory(Folder);File.AppendAllText(Path.Combine(Folder,"SectionBoxAlign-"+DateTime.Now.ToString("yyyyMMdd")+".log"),DateTime.Now.ToString("O")+" "+text+Environment.NewLine);}catch{}}
 }
 [Plugin("MMTools.SectionBoxAlign","MMSA",DisplayName="Section Box Align")]
 [AddInPlugin(AddInLocation.None)]
 public sealed class Command:AddInPlugin {
  internal static readonly AlignForm Session=new AlignForm();
  public override int Execute(params string[] parameters){
   if(parameters.Length>0&&parameters[0]=="selftest")return NativeTests.Run(parameters[1]);
   return Session.Start(parameters.Length==0||parameters[0]!="dir");
  }
 }
 // A windowless interaction controller. The historical name is retained for test compatibility.
 internal sealed class AlignForm {
  const string ToolId="MMTools.SectionBoxAlign.Pick.MMSA";
  readonly System.Windows.Forms.Timer timer=new System.Windows.Forms.Timer{Interval=250};
  PickTool tool;Document document;Model model;string file;bool picking,face;Tool previous;V? first;
  internal string Prompt;
  internal bool IsPicking {get{return picking;}}
  internal bool IsFace {get{return face;}}
  public AlignForm(){timer.Tick+=(s,e)=>{if(picking&&(!SameDocument()||document.Tool.Value!=Tool.CustomToolPlugin||document.Tool.CustomToolPluginId!=ToolId))Stop();};}
  bool SameDocument(){try{return document==App.ActiveDocument&&!document.IsClear&&document.FileName==file&&document.Models.Count>0&&document.Models[0]==model;}catch{return false;}}
  public int Start(bool isFace){Stop();try{
   document=App.ActiveDocument;if(document==null||document.IsClear||document.Models.Count==0)return 1;
   file=document.FileName;model=document.Models[0];face=isFace;first=null;
   var record=App.Plugins.FindPlugin(ToolId);if(record==null)throw new InvalidOperationException("Picking tool unavailable; restart Navisworks.");
   tool=record.LoadedPlugin as PickTool??record.LoadPlugin() as PickTool;if(tool==null)return 1;
   previous=document.Tool.Value;if(previous==Tool.CustomToolPlugin)previous=Tool.Select;
   tool.Owner=this;picking=true;document.Tool.SetCustomToolPlugin(tool);timer.Start();
   Prompt=face?"Align to Face: pick a planar face. Always vertical. Esc cancels.":"Align to Dir: pick point 1 of 2. Always vertical. Esc cancels.";
   new Box(document.ActiveView.GetClippingPlanes());Redraw();return 0;
  }catch(Exception e){Prompt=e.Message+"  Esc cancels.";Log.Write(e.ToString());Redraw();return 1;}}
  void Redraw(){try{if(SameDocument())document.ActiveView.RequestDelayedRedraw(ViewRedrawRequests.OverlayRender);}catch{}}
  internal void Stop(){timer.Stop();if(tool!=null)tool.Owner=null;try{if(picking&&SameDocument()&&document.Tool.Value==Tool.CustomToolPlugin&&document.Tool.CustomToolPluginId==ToolId)document.Tool.Set(previous);}catch(Exception e){Log.Write(e.ToString());}picking=false;first=null;Prompt=null;Redraw();}
  internal void Pick(View view,int x,int y){if(!picking)return;try{
   if(!SameDocument()||view!=document.ActiveView){Stop();return;}
   var hit=view.PickItemFromPoint(x,y,1,true,face?(PickPrimitives.Point|PickPrimitives.Distance):(PickPrimitives.Vertex|PickPrimitives.LineVertex|PickPrimitives.Point|PickPrimitives.Distance));
   if(hit==null||hit.Point==null){Prompt="No surface here. Pick a visible model surface. Esc cancels.";Redraw();return;}
   V point=new V(hit.Point.X,hit.Point.Y,hit.Point.Z),target;
   if(face)target=SurfacePicker.Normal(view,x,y,hit);
   else {if(!first.HasValue){first=point;Prompt="Align to Dir: pick point 2 of 2. Esc cancels.";Redraw();return;}target=point-first.Value;}
   string original=view.GetClippingPlanes();var box=new Box(original);int axis;
   var rotation=Geometry.Align(box.Rotation,target,face,true,out axis);
   Apply(view,original,box.WithRotation(rotation),box);
   Log.Write("v1.1.0 Applied "+(face?"Face":"Direction")+" AlwaysVertical=true BEFORE="+original+" AFTER="+view.GetClippingPlanes());Stop();
  }catch(Exception e){Log.Write(e.ToString());Prompt=e.Message+"  Pick again or Esc to cancel.";Redraw();}}
  internal static void Apply(View view,string original,string updated,Box box){try{if(!view.TrySetClippingPlanes(updated))throw new InvalidOperationException("Navisworks rejected the new box rotation.");box.VerifyBounds(new Box(view.GetClippingPlanes()));view.RequestDelayedRedraw(ViewRedrawRequests.All);}catch{if(!view.TrySetClippingPlanes(original))Log.Write("ERROR restoring original clipping data: "+original);throw;}}
 }
 [Plugin("MMTools.SectionBoxAlign.Pick","MMSA",DisplayName="Section Box Align Pick")]
 public sealed class PickTool:ToolPlugin {
  internal AlignForm Owner;
  public override bool MouseDown(View view,KeyModifiers modifiers,ushort button,int x,int y,double timeOffset){if(Owner==null)return false;if(button==2){Owner.Stop();return true;}if(button!=1)return false;Owner.Pick(view,x,y);return true;}
  public override bool KeyDown(View view,KeyModifiers modifiers,ushort key,double timeOffset){if(key==27&&Owner!=null){Owner.Stop();return true;}return false;}
  public override void OverlayRender(View view,Graphics graphics){if(Owner==null||string.IsNullOrEmpty(Owner.Prompt))return;try{using(var font=new TextFontInfo("Arial",14,0,false,false)){graphics.Color(new Color(0,0,0),1);graphics.Text2D(font,Owner.Prompt,new Point2D(17,29),0,0);graphics.Color(new Color(1,1,1),1);graphics.Text2D(font,Owner.Prompt,new Point2D(16,28),0,0);}}catch(Exception e){Log.Write("Overlay: "+e.Message);}}
 }
}
