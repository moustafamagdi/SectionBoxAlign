using System;
using Autodesk.Navisworks.Api.Plugins;
using NApp = Autodesk.Navisworks.Api.Application;
namespace MMTools.Floating
{
    [Plugin("MMTools.FloatingRibbon", "MMTL", DisplayName = "My Navis Tools")]
    [RibbonLayout("FloatingTools.xaml")]
    [RibbonTab("MMTools.Floating.Tab", DisplayName = "My Navis Tools")]
    [Command("MMTools.SmartBox", DisplayName = "Smart Section Box", Icon = "Icons/smart16.png", LargeIcon = "Icons/smart32.png", ToolTip = "Open Smart Section Box for direct face dragging.", LoadForCanExecute = true)]
    [Command("MMTools.ClashAudit", DisplayName = "Clash Test Audit", Icon = "Icons/audit16.png", LargeIcon = "Icons/audit32.png", ToolTip = "Audit clash test selection coverage, overlap and repeated result pairs.", LoadForCanExecute = true)]
    [Command("MMTools.ZoneExplorer", DisplayName = "Zone Explorer", Icon = "Icons/zones16.png", LargeIcon = "Icons/zones32.png", ToolTip = "Explore model zones and review clashes with native Clash Detective display.", LoadForCanExecute = true)]
    [Command("MMTools.SectionBox", DisplayName = "Section Box", Icon = "Icons/section16.png", LargeIcon = "Icons/section32.png", ToolTip = "Fit a section box to selected elements without saving a viewpoint.", LoadForCanExecute = true)]
    [Command("MMTools.SectionSave", DisplayName = "Section Box + Save", Icon = "Icons/save16.png", LargeIcon = "Icons/save32.png", ToolTip = "Fit a section box to selected elements, then name and save the viewpoint.", LoadForCanExecute = true)]
    [Command("MMTools.VerticalPipes", DisplayName = "Vertical Pipes", Icon = "Icons/vertical16.png", LargeIcon = "Icons/vertical32.png", ToolTip = "Select vertical pipes and save a Selection Set.", LoadForCanExecute = true)]
    [Command("MMTools.AlignFace", DisplayName = "Align to Face", Icon = "Icons/alignface16.png", LargeIcon = "Icons/alignface32.png", ToolTip = "Pick a planar face to align the section box. Always vertical; center and size preserved. Esc cancels.", LoadForCanExecute = true)]
    [Command("MMTools.AlignDir", DisplayName = "Align to Dir", Icon = "Icons/aligndir16.png", LargeIcon = "Icons/aligndir32.png", ToolTip = "Pick two points to align the section box X direction horizontally. Esc cancels.", LoadForCanExecute = true)]
    public sealed class FloatingRibbon : CommandHandlerPlugin
    {
        private static string PluginId(string name)
        {
            switch (name)
            {
                case "MMTools.SmartBox": return "SmartSectionBox.SmartSectionBoxAddin.MSSB";
                case "MMTools.ClashAudit": return "HUMAIN.ClashAudit.HUMA";
                case "MMTools.ZoneExplorer": return "HUMAIN.ZoneExplorer.HUMA";
                case "MMTools.SectionBox": return "FastViewpoint.SectionBox.MM Tools";
                case "MMTools.SectionSave": return "FastViewpoint.MM Tools";
                case "MMTools.VerticalPipes": return "VerticalPipes.MMAG";
                case "MMTools.AlignFace": case "MMTools.AlignDir": return "MMTools.SectionBoxAlign.MMSA";
                default: return null;
            }
        }
        public override int ExecuteCommand(string name, params string[] parameters)
        {
            var id = PluginId(name);
            var record = id == null ? null : NApp.Plugins.FindPlugin(id) as AddInPluginRecord;
            if (record == null) return -1;
            return record.Execute(name == "MMTools.AlignFace" ? new[]{"face"} : name == "MMTools.AlignDir" ? new[]{"dir"} : parameters);
        }
        public override CommandState CanExecuteCommand(string name)
        {
            var id = PluginId(name);
            var record = id == null ? null : NApp.Plugins.FindPlugin(id) as AddInPluginRecord;
            return record == null ? new CommandState { IsVisible = true, IsEnabled = false } : record.CanExecute();
        }
    }
}

