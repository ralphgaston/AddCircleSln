
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcAp = Autodesk.AutoCAD.ApplicationServices.Application;

namespace AddCircle
{
    namespace AddCircle
    {
        public class Commands
        {
            [CommandMethod("DrawCircle")]
            public void Test()
            {
                var doc = AcAp.DocumentManager.MdiActiveDocument;
                var db = doc.Database;
                var ed = doc.Editor;
                PromptPointOptions prmptPtOpt = new PromptPointOptions("Select Point");
                PromptPointResult ptRes = ed.GetPoint(prmptPtOpt);
                Point3d pt = Point3d.Origin;
                if(ptRes.Status == PromptStatus.OK)
                {
                    pt = ptRes.Value;
                }

                PromptDoubleOptions prmptDblOpt = new PromptDoubleOptions("Enter radius");
                PromptDoubleResult dblRes = ed.GetDouble(prmptDblOpt);
                double radius = 0;
                if (dblRes.Status == PromptStatus.OK)
                {
                    radius = dblRes.Value;
                }
                if (radius > 0 && pt != Point3d.Origin)
                {
                    using (var tr = db.TransactionManager.StartTransaction())
                    {
                        // Draw circle
                        Circle circle = new Circle(pt, Vector3d.ZAxis, radius);
                        // Grab the block table
                        var blockTable = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                        // Grab the model space block record from the block table, using the model space objectid
                        var modelSpace 
                            = (BlockTableRecord)tr
                            .GetObject(blockTable[BlockTableRecord.ModelSpace],
                                       OpenMode.ForWrite);
                        // Append the circle to the model space block record.
                        var objectId = modelSpace.AppendEntity(circle);
                        // add the circle to the transaction
                        tr.AddNewlyCreatedDBObject(circle, true);
                        // commit the transaction
                        tr.Commit();
                        
                    }
                }
            }
            [CommandMethod("DrawLine")]
            public void Test2()
            {
                // Grab the top most objects
                var doc = AcAp.DocumentManager.MdiActiveDocument;
                var db = doc.Database;
                var ed = doc.Editor;
                
                // get the start point
                PromptPointOptions prmptPtOpt = new PromptPointOptions("Select start point");
                PromptPointResult ptRes = ed.GetPoint(prmptPtOpt);
                Point3d pt = Point3d.Origin;
                if (ptRes.Status.Equals(PromptStatus.OK))
                {
                    pt = ptRes.Value;
                }

                // get the end point
                prmptPtOpt.Message = "Select end point";
                prmptPtOpt.BasePoint = pt;
                prmptPtOpt.UseDashedLine = true;
                ptRes = ed.GetPoint(prmptPtOpt);
                Point3d endPt = Point3d.Origin;
                if (ptRes.Status.Equals(PromptStatus.OK))
                {
                    endPt = ptRes.Value;
                }

                // start a transaction and draw the line
                using (var tr = db.TransactionManager.StartTransaction())
                {
                    Line line = new Line(pt, endPt);
                    var blockTable = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);

                    // Grab the model space block record from the block table, using the model space objectid
                    var modelSpace
                        = (BlockTableRecord)tr
                        .GetObject(blockTable[BlockTableRecord.ModelSpace],
                                   OpenMode.ForWrite);

                    // Append the line to the model space block record.
                    var objectId = modelSpace.AppendEntity(line);

                    // add the line to the transaction
                    tr.AddNewlyCreatedDBObject(line, true);

                    // commit the transaction
                    tr.Commit();

                }
            }
        }
    }
}
