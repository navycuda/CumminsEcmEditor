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
        public string calibration_versionField {get; set; }
        public string module_nameField {get; set; }
        public string first_prod_cfg_file_versionField {get; set; }
        public string product_idField {get; set; }
        public string module_part_numberField {get; set; }
        public string interface_levelField {get; set; }
        public string creation_dateField {get; set; }
        public string start_boot_loader_versionField {get; set; }
        public string end_boot_loader_versionField {get; set; }
        public string byte_orderField {get; set; }
        public string index_table_addressField {get; set; }
        public string file_descriptorField {get; set; }
        public string harness_key_maskField {get; set; }
        public string harness_key_compatibility_listField {get; set; }
        public string application_tableField {get; set; }
        public string value_1Field {get; set; }
        public string value_2Field {get; set; }
        public string value_3Field {get; set; }
        public string value_4Field { get; set; }

        public string GetXmlString() =>
            "<xml_header>Not Implimented</xml_header>";
    }
}
