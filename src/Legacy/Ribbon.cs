using Autodesk.Navisworks.Api.Plugins;
using App=Autodesk.Navisworks.Api.Application;
namespace SectionBoxAlign {
 [Plugin("MMTools.SectionAlignRibbon","MMSA",DisplayName="My Navis Tools")]
 [RibbonLayout("SectionAlign.xaml")]
 [RibbonTab("MMTools.SectionAlign.Legacy.Tab",DisplayName="My Navis Tools")]
 [Command("MMTools.AlignFace",DisplayName="Align to Face",Icon="Icons/alignface16.png",LargeIcon="Icons/alignface32.png",ToolTip="Pick a planar face. Always vertical; center and size preserved.",LoadForCanExecute=true)]
 [Command("MMTools.AlignDir",DisplayName="Align to Dir",Icon="Icons/aligndir16.png",LargeIcon="Icons/aligndir32.png",ToolTip="Pick two points for the horizontal direction. Esc cancels.",LoadForCanExecute=true)]
 public sealed class AlignRibbon:CommandHandlerPlugin {
  public override int ExecuteCommand(string name,params string[] args){return Command.Session.Start(name=="MMTools.AlignFace");}
  public override CommandState CanExecuteCommand(string name){return new CommandState{IsVisible=true,IsEnabled=App.ActiveDocument!=null&&!App.ActiveDocument.IsClear};}
 }
}
