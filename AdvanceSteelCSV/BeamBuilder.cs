using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

namespace AdvanceSteelCSV
{
    class BeamBuilder
    {
        List<CSVField> fields;

        Document doc;
        Database db;
        Editor ed;

        public BeamBuilder(List<CSVField> fields)
        {
            this.fields = fields;

            this.doc = Application.DocumentManager.MdiActiveDocument;
            this.db = doc.Database;
            this.ed = doc.Editor;
        }

        public void BuildBeam()
        {
            // get lines from selection
            // separate by layer
            // use the CSV to build the beam
            List<ObjectId> objectIds = getLineIds();


        }

        private List<ObjectId> getLineIds()
        {
            TypedValue[] filterlist = new TypedValue[1];

            //select circle and line

            filterlist[0] = new TypedValue(0, "LINE");            

            SelectionFilter filter = new SelectionFilter(filterlist);
            PromptSelectionResult selRes = ed.SelectAll(filter);
            if (selRes.Status != PromptStatus.OK)
            {

                ed.WriteMessage("\nerror in getting the selectAll");
                return new List<ObjectId>();

            }

            return selRes.Value.GetObjectIds().ToList();
        }
    }
}
