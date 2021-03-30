using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.DocumentManagement;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Modelling;
using Autodesk.AdvanceSteel.Profiles;
using Autodesk.AdvanceSteel.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvanceSteelCSV
{
    class SteelCommand
    {
        [CommandMethodAttribute("TEST_GROUP", "HelloWorld", "HelloWorld", CommandFlags.Modal)]
        public void SayHelloWorld()
        {
            DocumentManager.LockCurrentDocument();

            using (Transaction tr = TransactionManager.StartTransaction())
            {
                //create column (vertical beam) with a default size
                ProfileName profName = new ProfileName();
                ProfilesManager.GetProfTypeAsDefault("I", out profName);

                Point3d beamEnd = new Point3d(0, 0, 3500);
                Point3d beamStart = Point3d.kOrigin;

                StraightBeam myBeam = new StraightBeam(profName.Name, beamStart, beamEnd, Vector3d.kXAxis);
                myBeam.WriteToDb();

                //create a wide flange column
                beamEnd = new Point3d(0, 3000, 3500);
                beamStart = new Point3d(0, 3000, 0);

                myBeam = new StraightBeam("AISC 14.1 W Shape#@§@#W10x33", beamStart, beamEnd, Vector3d.kXAxis);
                myBeam.WriteToDb();

                //create a curved beam
                beamEnd = new Point3d(0, 3000, 3500);
                beamStart = new Point3d(0, 0, 3500);
                Point3d arcPoint = new Point3d(0, 1500, 5000);

                BentBeam myBentBeam = new BentBeam("HEA  DIN18800-1#@§@#HEA200", Vector3d.kZAxis, beamStart, arcPoint, beamEnd);
                myBentBeam.WriteToDb();

                tr.Commit();                
            }

            DocumentManager.UnlockCurrentDocument();
        }


        [CommandMethodAttribute("ConvertToBeams", "ConvertToBeams", "ConvertToBeams", CommandFlags.Modal)]
        public void ConvertToBeams()
        {
            CSVFile file = new CSVFile();    

            
        }
    }
}
