using System;
using System.IO;
using Psy.Core.Console;

namespace EpicEdit
{
    public class ConsoleCommands
    {
        private readonly Editor _editor;

        public ConsoleCommands(Editor editor)
        {
            _editor = editor;
        }

        public void Register(BaseConsole console)
        {
            console.CommandBindings.Bind("save", "Save test file", HandleSave);
            console.CommandBindings.Bind("dump", "Dump model debug info", HandleDump);
        }

        private void HandleDump(string[] parameters)
        {
            if (_editor.Model == null)
                return;

            var con = StaticConsole.Console;
            var model = _editor.Model;

            con.AddLine(string.Format("{0} model parts", model.ModelParts.Count));
            foreach (var modelPart in model.ModelParts)
            {
                con.AddLine(string.Format("    ModelPart: {0}", modelPart.Name));
                con.AddLine(string.Format("        Position: {0}", modelPart.Position));
                con.AddLine(string.Format("        Rotation: {0}", modelPart.Rotation));

                con.AddLine(string.Format("        {0} Anchors", modelPart.Anchors.Count));

                foreach (var anchor in modelPart.Anchors)
                {
                    con.AddLine(string.Format("        Anchor: {0}", anchor.Name));    
                    con.AddLine(string.Format("            Position: {0}", anchor.Position));
                }
            }
        }

        private void HandleSave(params string[] parameters)
        {
            var fileStream = new FileStream("model.edm", FileMode.CreateNew);
            var streamWriter = new StreamWriter(fileStream);

            

        }
    }
}