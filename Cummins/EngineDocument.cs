using CumminsEcmEditor.IntelHex;
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
        public void PrepareDocument()
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
            AddDetails();
        }
        #endregion

        #region Private Document Methods
        private void AddDetails()
        {
            Document.Add("");
            Document.Add($"FilterFileName: {FilterPath}");
            Document.Add("");
            Document.Add($"Report Generated: {DateTime.Now}");
            Document.Add("");
            Document.Add("");
        }
        private void AddDivider(char divider)
        {
            string result = "";
            for (int i = 0; i < D_DividerLength; i++)
                result += divider;
            Document.Add(result);
        }
        private void AddTitle() => Document.Add(D_Title);
        private void AddFilePaths()
        {
            Document.Add($"  Data: {XCalPath}");
            Document.Add($"  Config: {EcfgPath}");
        }
        private void AddParameterString(string name, string value, string units, string itn, string comment, bool marked = false)
        {
            // assign and configure variables
            char mark = marked ? P_ParameterModified : A_ColumnDelineator;
            char d = P_ColumnDelineator;
            name = name.ToPaddedString(P_PaddingName);
            value = value.ToPaddedString(P_PaddingValue);
            units = units.ToPaddedString(P_PaddingUnits);
            itn = itn.ToPaddedString(P_PaddingItn);
            comment = comment.ToDocumentSafe();
            // Add the parameter to the document
            Document.Add($"{mark}{d}{name}{d}{value}{d}{units}{d}{itn}{d}{comment}");
        }
        #endregion
    }
}
