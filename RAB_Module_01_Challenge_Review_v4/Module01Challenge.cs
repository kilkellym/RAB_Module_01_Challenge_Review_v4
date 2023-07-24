#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

#endregion

namespace RAB_Module_01_Challenge_Review_v4
{
    [Transaction(TransactionMode.Manual)]
    public class Module01Challenge : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            // 1. set variables
            int numFloors = 250;
            double currentElev = 0;
            int floorHeight = 15;

            // 5. get titleblock
            FilteredElementCollector tbCollector = new FilteredElementCollector(doc);
            tbCollector.OfCategory(BuiltInCategory.OST_TitleBlocks);
            tbCollector.WhereElementIsElementType();
            ElementId tblockId = tbCollector.FirstElementId();

            // 6. get view family types
            FilteredElementCollector vftCollector = new FilteredElementCollector(doc);
            vftCollector.OfClass(typeof(ViewFamilyType));

            ViewFamilyType fpVFT = null;
            ViewFamilyType cpVFT = null;

            foreach (ViewFamilyType curVFT in vftCollector)
            {
                if (curVFT.ViewFamily == ViewFamily.FloorPlan)
                {
                    fpVFT = curVFT;
                }
                else if (curVFT.ViewFamily == ViewFamily.CeilingPlan)
                {
                    cpVFT = curVFT;
                }
            }

            // 8. create transaction
            Transaction t = new Transaction(doc);
            t.Start("FIZZ BUZZ Challenge");

            // 2. loop through floors and check FIZZBUZZ
            for (int i = 1; i <= numFloors; i++)
            {
                // 3. create level
                Level newLevel = Level.Create(doc, currentElev);
                newLevel.Name = "LEVEL " + i.ToString();

                // 7. increment elevation
                currentElev += floorHeight;

                // 4. check for FIZZBUZZ
                if (i % 3 == 0 && i % 5 == 0)
                {
                    // FIZZBUZZ - create sheet
                    ViewSheet newSheet = ViewSheet.Create(doc, tblockId);
                    newSheet.SheetNumber = i.ToString();
                    newSheet.Name = "FIZZBUZZ_" + i.ToString();

                    // BONUS
                    ViewPlan bonusPlan = ViewPlan.Create(doc, fpVFT.Id, newLevel.Id);
                    bonusPlan.Name = "FIZZBUZZ_" + i.ToString();

                    Viewport newVP = Viewport.Create(doc, newSheet.Id, bonusPlan.Id, new XYZ(1.25, 1, 0));
                }
                else if (i % 3 == 0)
                {
                    // FIZZ - create floor plan
                    ViewPlan newFloorPlan = ViewPlan.Create(doc, fpVFT.Id, newLevel.Id);
                    newFloorPlan.Name = "FIZZ_" + i.ToString();
                }
                else if (i % 5 == 0)
                {
                    // BUZZ - create ceiling plan
                    ViewPlan newCeilingPlan = ViewPlan.Create(doc, cpVFT.Id, newLevel.Id);
                    newCeilingPlan.Name = "BUZZ_" + i.ToString();
                }

                
            }

            t.Commit();
            t.Dispose();

            // 9. alert user
            TaskDialog.Show("Complete", "Created " + numFloors + " levels.");


            return Result.Succeeded;
        }

        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnCommand1";
            string buttonTitle = "Button 1";

            ButtonDataClass myButtonData1 = new ButtonDataClass(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.Blue_32,
                Properties.Resources.Blue_16,
                "This is a tooltip for Button 1");

            return myButtonData1.Data;
        }
    }
}
