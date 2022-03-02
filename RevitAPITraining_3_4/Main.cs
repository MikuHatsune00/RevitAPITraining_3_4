using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPITraining_3_4
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            IList<Reference> selectedRefList = uidoc.Selection.PickObjects(ObjectType.Element, "Выберите трубу");
            var elementList = new List<Element>();

            foreach (var selectedElement in selectedRefList)
            {
                Element element = doc.GetElement(selectedElement);
                elementList.Add(element);


                if (element is Pipe)
                {
                    Parameter ODiameterParameter = element.get_Parameter(BuiltInParameter.RBS_PIPE_OUTER_DIAMETER);
                    Parameter IDiameterParameter = element.get_Parameter(BuiltInParameter.RBS_PIPE_INNER_DIAM_PARAM);
                    if (ODiameterParameter.StorageType == StorageType.Double || IDiameterParameter.StorageType == StorageType.Double)
                    {double IDiameterValue = UnitUtils.ConvertFromInternalUnits(IDiameterParameter.AsDouble(), UnitTypeId.Millimeters);
                        double ODiameterValue = UnitUtils.ConvertFromInternalUnits(ODiameterParameter.AsDouble(), UnitTypeId.Millimeters);
                        string ODiameter = Convert.ToString(ODiameterValue);
                        string IDiameter = Convert.ToString(IDiameterValue);
                        string IODiameter = ODiameter + "/" + IDiameter;
                        using (Transaction ts = new Transaction(doc, "Set parameters"))
                        {
                            ts.Start();
                            var FamilyInstance = element as Pipe;
                            Parameter Lparameter = FamilyInstance.LookupParameter("Наименование");
                            Lparameter.Set(IODiameter);
                            ts.Commit();
                        }

                    }
                        


                   

                }


            }
            TaskDialog.Show("Диаметры", "Параметр наименование для труб заполнен");

            return Result.Succeeded;
        }
    }

}
