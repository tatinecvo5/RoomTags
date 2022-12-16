using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomTags
{

    [Transaction(TransactionMode.Manual)]
    public class CreateRoomTag : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;

            List<Level> levels = new FilteredElementCollector(document)
                .OfClass(typeof(Level))
                .WhereElementIsNotElementType()
                .OfType<Level>()
                .ToList();

            int l = 1;
            Transaction transaction = new Transaction(document, "Маркировка комнат");
            transaction.Start();

            foreach (Level level in levels)
            {
                PlanCircuit planCircuit = null;
                PlanTopology planTopology = document.get_PlanTopology(level);
                foreach (PlanCircuit circuit in planTopology.Circuits)
                {
                    if (null != circuit)
                    {
                        planCircuit = circuit;
                        Room room = document.Create.NewRoom(null, planCircuit);
                        room.Name = $"{l}_{room.Number}";
                    }
                }
                l += 1;
            }

            transaction.Commit();

            TaskDialog.Show("Комнаты замаркированы автоматически", "№этажа_№комнаты");
            return Result.Succeeded;
        }
    }
}
