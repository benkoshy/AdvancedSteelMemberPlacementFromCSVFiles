using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using CsvHelper;



namespace AdvanceSteelCSV
{
    class CSVFile
    {        
        Document doc;
        Database db;
        Editor ed;

        string path;

        public CSVFile()
        {
            this.doc = Application.DocumentManager.MdiActiveDocument;
            this.db = doc.Database;
            this.ed = doc.Editor;

            this.path = getPath();

            // this.masterTable = getMasterTable();            
        }

        //private List<CSVField> getMasterTable()
        //{
        //    List<CSVField> fields = new List<CSVField>();

        //    string[] resourceNames =  System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();

        //    using (var reader = new StreamReader(resourceNames.Where(name => name == "MasterSectionTable.csv").First()))
        //    {
        //        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        //        {
        //            fields = csv.GetRecords<CSVField>().ToList();
        //        }

        //        return fields;
        //    }            
        //}

        public List<CSVField> GetValidRecords()
        {
            return getValidRecords(this.path);
        }

        private List<CSVField> getValidRecords(string path)
        {
            List<CSVField> allRecords = getRecords(path);

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                LayerTable layerTable = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;               

                List<CSVField> invalidLayerNames = allRecords.Where(row => ! layerTable.Has(row.Layer)).ToList();
                if (invalidLayerNames.Count > 0)
                {
                    ed.WriteMessage(String.Format("\n The following layernames in the CSV file {0} were invalid: {1}", path, invalidLayerNames.Aggregate((a, b) => a.ToString() + ", " + b.ToString())));
                }

                return allRecords.Where(row => layerTable.Has(row.Layer)).ToList();
            }            
        }

        private string getPath()
        {
            string drawingPath = db.Filename;
            string filePath = Path.GetDirectoryName(drawingPath);
            string pathOfCSV = Path.Combine(filePath, "MEMBER-SCHEDULE.csv");            

            if (!File.Exists(pathOfCSV))
            {
                System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
                fileDialog.Filter = "All Files (*.*)|*.*";
                fileDialog.FilterIndex = 1;
                fileDialog.Multiselect = false;

                if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    return fileDialog.FileName;                    
                }
                else
                {
                    throw new System.Exception("\nPlease check the path of the csv file and it's name should be MEMBER-SCHEDULE.csv  also please save your drawing before running this command. Try again.");
                }                
            }

            return pathOfCSV;
        }

        private List<CSVField> getRecords(string pathOfCSV)
        {
            List<CSVField> fields = new List<CSVField>();

            using (var reader = new StreamReader(pathOfCSV))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    fields = csv.GetRecords<CSVField>().ToList();
                }

                return fields;
            }            
        }
    }
}
