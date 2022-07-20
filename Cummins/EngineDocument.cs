using CumminsEcmEditor.IntelHex;
using CumminsEcmEditor.Tools;
using CumminsEcmEditor.Tools.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CumminsEcmEditor.Cummins
{
    public class EngineDocument
    {
        #region Static Character Properties
        // P_ = for a single parameter but can be used in arrays
        // A_ = array of parameters (table, y axis, z axis)
        private static char P_ColumnDelineator = ' ';
        private static char P_ParameterModified = '~';
        private static char A_ColumnDelineator = '|';
        private static char D_HeaderLineDelineator = '=';
        private static char A_RowDelineator = '-';
        #endregion

        #region Static Parameter Properties
        // ~ UL_ITN_0x00000001                                001A002C     HEX        0x00000001 An imaginary itn to help
        //   UL_ITN_0x00000002                                5.00         Deg/C      0x00000002 demonstrate the document
        // ▲ ▲                                              ▲ ▲          ▲ ▲        ▲ ▲        ▲ ▲
        // │ ├──────────────────────────────────────────────┤ ├──────────┤ ├────────┤ ├────────┤ ├────────────▶
        // │ │ Parameter Name                               │ │UserValue │ │UnitType│ │Itn #   │ │Comment
        // │ │ 48 char                                      │ │12 char   │ │10 char │ │10 char │ │Remove special characters
        // │ ╰──────────────────────────────────────────────╯ ╰──────────╯ ╰────────╯ ╰────────╯ ╰────────────▶
        // ╰── Parameter Marked as Modified with '~'
        // One ' ' between each column.
        private static int P_PaddingName = 48;
        private static int P_PaddingValue = 12;
        private static int P_PaddingUnits = 10;
        private static int P_PaddingItn = 10;
        private static string D_GroupColumnHeaders
        {
            get
            {
                char d = P_ColumnDelineator;
                return
                    "Name".ToPaddedString(P_PaddingName) + d +
                    "Value".ToPaddedString(P_PaddingValue) + d +
                    "Units".ToPaddedString(P_PaddingUnits) + d +
                    "Itn".ToPaddedString(P_PaddingItn) + d +
                    "Comment";
            }
        }
        private static int D_DividerLength => D_GroupColumnHeaders.Length;
        private static string D_Title = "CumminsEcmEditor :: Calibration Document Report";
        #endregion

        #region Array Properties
        public static int A_PaddingCell = 12;
        #endregion

        #region Document Properties
        private Calibration XCal { get; set; }
        private List<string> Document { get; set; }
        private string XCalPath => XCal.GetXCalPath();
        private string EcfgPath => XCal.TableOfContents.GetEcfgPath();
        private string FilterPath => "NonFunctioning";
        #endregion

        #region Constructor
        public EngineDocument(Calibration xCal)
        {
            // Reference the calibration
            XCal = xCal;
        }
        #endregion

        #region Document Methods
        public string Save()
        {
            // Generate the Document
            GenerateDocument();
            // Save the Document
            // set the documentPath, alongside the .xcal
            string docPath = XCal.GetXCalPath();
            if (docPath.Contains(".XCAL"))
              docPath = docPath.Replace(".XCAL", "_documented.txt");
            else if (docPath.Contains(".xcal"))
              docPath = docPath.Replace(".xcal", "_documented.txt");
            else
              docPath += ".documented.txt";
            EcmFiles.Save(docPath, Document.ToArray(), true);
            // Clear the document to save memory
            Document.Clear();
            return docPath;
        } 
        #endregion

        #region Private Document Methods
        private void GenerateDocument()
        {
            // Generates the Header, (re)sets List<string> Document
            PrepareDocument();
            // Documents and adds the Itns to the document.
            DocumentItns();
        }
        private void PrepareDocument()
        {
            // Start a new Document
            Document = new List<string>();

            // Assemble the document
            AddDivider(D_HeaderLineDelineator);
            // Document title
            AddTitle();
            // data and config file paths
            AddFilePaths();
            AddDivider(D_HeaderLineDelineator);
            // report details
            // what additional details could go here?
            // what is the filter file
            AddReportDetails();
            AddDivider(D_HeaderLineDelineator);
            // Parameter Report Area
            AddParameterReport();
            AddDivider(D_HeaderLineDelineator);
            // SubFile Area
            // What is a subfile number?  
            AddSubFile(8);
            AddDivider(D_HeaderLineDelineator);
            // Temporary... Add one Titled divider as a legend.
            // Eventually the divider will be used as part of
            // the sort methodology.
            AddTitledDivider("Itn Legend");
        }
        private void DocumentItns()
        {
            Itn[] itns = XCal.TableOfContents.GetAllItns(SortItnsBy.None);
            // Currently only adding single value itns.
            foreach (Itn itn in itns)
                if (itn.IsSingleValue())
                    AddParameterString(
                        itn.GetName(),
                        itn.GetSingleValue(),
                        itn.GetUnits(),
                        itn.GetHexId(),
                        itn.GetComment() + $" {itn.GetEcmParameterType()}",
                        itn.IsModified
                        );
                else
                {
                    // Temp disable
                    // AddDimensionalParameters(itn);
                }

            // Add short divider and record count
            AddDivider(A_RowDelineator, 31);
            Document.Add($"Total Records = {itns.Length}");
        }
        #endregion

        #region Private Document Add Methods
        private void AddDimensionalParameters(Itn itn)
        {
            DataType dT = itn.Parameter.data_type;
            if (dT is X_Axis x)
                AddXAxis(x);
            else if (dT is Y_Axis y)
                AddYAxis(y);
            else if (dT is Z_Axis z)
                AddZAxis(z);
            else if (dT is Table t)
                AddTable(t);
        }
        private void AddXAxis(X_Axis x)
        {

        }
        private void AddYAxis(Y_Axis y)
        {

        }
        private void AddZAxis(Z_Axis z)
        {

        }
        private void AddTable(Table t)
        {

        }
        private DataType GetElementDataType(DataType dT)
        {
            if (dT is Floating_Point fP)
                return fP;
            return (Fixed_Point)dT;
        }
        private void AddTitledDivider(string title) =>
            Document.AddRange(new string[]{"",title,D_GroupColumnHeaders,GetDivider(A_RowDelineator),""});
        private void AddSubFile(int number) => Document.Add($"Subfile: {number}");
        private void AddParameterReport()
        {
            Document.Add("Parameter Report");
            Document.Add("");
            Document.Add($"");
            Document.Add($"");
            Document.Add($"");
            Document.Add($"");
            Document.Add($"");
            Document.Add($"");
            Document.Add($"");
        }
        private void AddReportDetails()
        {
            Document.Add("");
            Document.Add($"FilterFileName: {FilterPath}");
            Document.Add("");
            Document.Add($"Report Generated: {DateTime.Now}");
            Document.Add("");
            Document.Add("");
        }
        private void AddDivider(char divider) =>
            Document.Add(GetDivider(divider));
        private void AddDivider(char divider, int length) =>
            Document.Add(GetDivider(divider, length));
        private void AddTitle() => Document.Add(D_Title);
        private void AddFilePaths()
        {
            Document.Add($"  Data: {XCalPath}");
            Document.Add($"  Config: {EcfgPath}");
        }
        private void AddParameterString(string name, string value, string units, string itn, string comment, bool marked = false)
        {
            // assign and configure variables
            char mark = marked ? P_ParameterModified : P_ColumnDelineator;
            char d = P_ColumnDelineator;
            name = name.ToPaddedString(P_PaddingName);
            value = value.ToPaddedString(P_PaddingValue);
            units = units.ToPaddedString(P_PaddingUnits);
            itn = itn.ToPaddedString(P_PaddingItn);
            comment = comment.ToDocumentSafe();
            // Add the parameter to the document
            Document.Add($"{d}{mark}{name}{d}{value}{d}{units}{d}{itn}{d}{comment}");
        }
        #endregion

        #region Private Document Get Methods
        private string GetDivider(char divider)
        {
            string result = "";
            for (int i = 0; i < D_DividerLength; i++)
                result += divider;
            return result;
        }
        private string GetDivider(char divider, int length)
        {
            string result = "";
            for (int i = 0; i < length; i++)
                result += divider;
            return result;
        }
        #endregion
    }
}
