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
        [CommandMethodAttribute("ConvertToBeams", "ConvertToBeams", "ConvertToBeams", CommandFlags.Modal)]
        public void ConvertToBeams()
        {
            CSVFile file = new CSVFile();

            BeamBuilder builder = new BeamBuilder(file.GetValidRecords());
            builder.BuildBeam();
        }
    }
}
