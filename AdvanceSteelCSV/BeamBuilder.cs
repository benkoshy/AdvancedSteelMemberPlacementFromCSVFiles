using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AdvanceSteel.Modelling;
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
            List<ObjectId> objectIds = getLineIds();

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                List<Line> lines = objectIds.Select(id => id.GetObject(OpenMode.ForRead) as Line).ToList();

                Autodesk.AdvanceSteel.DocumentManagement.DocumentManager.LockCurrentDocument();
                using (Autodesk.AdvanceSteel.CADAccess.Transaction steelTr = Autodesk.AdvanceSteel.CADAccess.TransactionManager.StartTransaction())
                {
                    foreach (Line line in lines)
                    {
                        if (fields.Any(row => row.Layer == line.Layer))
                        {
                            CSVField row = fields.First(r => r.Layer == line.Layer);

                            createBeam(row, line);
                        }
                    }

                    steelTr.Commit();
                }
                Autodesk.AdvanceSteel.DocumentManagement.DocumentManager.UnlockCurrentDocument();

                tr.Commit();
            }
        }

        private void createBeam(CSVField row, Line line)
        {
            Autodesk.AdvanceSteel.Geometry.Point3d startPoint = new Autodesk.AdvanceSteel.Geometry.Point3d(line.StartPoint.X, line.StartPoint.Y, line.StartPoint.Z);
            Autodesk.AdvanceSteel.Geometry.Point3d endPoint = new Autodesk.AdvanceSteel.Geometry.Point3d(line.EndPoint.X, line.EndPoint.Y, line.EndPoint.Z);

            string beamFormat = String.Format("{0}#@§@#{1}", row.Table, row.Section);
            // string beamFormat = "AS-NZS SHS - CF C350#@§@#SHS 100x100x3.0";
            StraightBeam myBeam = new StraightBeam(beamFormat, startPoint, endPoint, Autodesk.AdvanceSteel.Geometry.Vector3d.kXAxis);
            myBeam.WriteToDb();
        }

        private List<ObjectId> getLineIds()
        {
            TypedValue[] filterlist = new TypedValue[1];            

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
