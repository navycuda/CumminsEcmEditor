using CumminsEcmEditor.Tools.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CumminsEcmEditor.IntelHex
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class XmlHeader
    {
        public string calibration_version {get; set; }
        public string module_name {get; set; }
        public string first_prod_cfg_file_version {get; set; }
        public string product_id {get; set; }
        public string module_part_number {get; set; }
        public string interface_level {get; set; }
        public string creation_date {get; set; }
        public string start_boot_loader_version {get; set; }
        public string end_boot_loader_version {get; set; }
        public string byte_order {get; set; }
        public string index_table_address {get; set; }
        public string file_descriptor {get; set; }
        public string harness_key_mask {get; set; }
        public string harness_key_compatibility_list {get; set; }
        public string application_table {get; set; }
        public string value_1 {get; set; }
        public string value_2 {get; set; }
        public string value_3 {get; set; }
        public string value_4 { get; set; }

        public string GetXmlString() =>
            "<xml_header>Not Implimented</xml_header>";
        public int GetTableOfContentsAddress() =>
            index_table_address.HexToInt();
    }
}
