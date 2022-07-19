using CumminsEcmEditor.Tools;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CumminsEcmEditor.Cummins
{
    /// <summary>
    /// min_resolution 0 or 1.0 == integer
    /// decimal count is based on the number of digits past the decimal point.
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Engineering_Tool_Config_File", Namespace = "http://www.electronics.cummins.com/eti/I")]
    public class ConfigurationFile
    {
        #region Xml Attributes
        [XmlAttribute("version")]
        public string Version { get; set; }
        [XmlAttribute("description")]
        public string Description { get; set; }
        [XmlAttribute("crc")]
        public string Checksum { get; set; }
        #endregion

        #region Xml Elements
        [XmlElement("compatibility_header")]
        public EcmCompatibilityHeader Header { get; set; }
        [XmlElement("parameter")]
        public EcmParameter[] Parameters { get; set; }
        [XmlElement("group")]
        public EcmGroup Groups { get; set; }
        #endregion

        #region Constructors
        public ConfigurationFile() { }
        public ConfigurationFile(string moduleName, EcmParameter[] parameters)
        {
            Header = new() { module_name = moduleName };
            Parameters = parameters;
        }
        #endregion

        #region Methods
        public EcmParameter[] GetParameters() =>
            Parameters.OrderBy(p => p.GetId()).ToArray();
        public void Save(string filePath) =>
            EcmFiles.Save(this, filePath);
        public void Overwrite(string filePath) =>
            EcmFiles.Save(this, filePath, true);
        public void TESTING_OutputToConsole()
        {
            Console.WriteLine($"{Version} : {Checksum} :: {Description}");
            PropertyInfo[] properties = typeof(EcmCompatibilityHeader).GetProperties();
            Console.WriteLine("Compatibility Header:");
            foreach (PropertyInfo p in properties)
                Console.WriteLine($"\t{p.Name.PadLeft(32)} = {p.GetValue(Header)}");
            Console.WriteLine("Parameters :");
            for (int i = 0; i < 2; i++)
                Console.WriteLine($"\t{Parameters[i].name}");
        }
        public void TESTING_AllItnsToConsole()
        {
            Parameters = Parameters.OrderBy(p => p.GetId()).ToArray();
            foreach (EcmParameter e in Parameters)
            {
                Console.WriteLine($"{e.id} : {e.name}");
            }
        }
        #endregion

        #region Static Methods
        public static ConfigurationFile? Load(string filePath) =>
            EcmFiles.Load<ConfigurationFile>(filePath);
        public static ConfigurationFile TestFile() =>
            new()
            {
                Version = "1.3.0.0",
                Description = "Building a test File",
                Checksum = "1701",
                Header = new()
                {
                    calibration_version = "17.01.00.00",
                    module_name = "BCX",
                    first_prod_cfg_file_version = "first cfg",
                    product_id = "product id",
                    module_part_number = "module pn",
                    interface_level = "3",
                    creation_date = DateTime.Now.ToString(),
                    start_boot_loader_version = "boot1701",
                    end_boot_loader_version = "end1701a",
                    byte_order = "LittleEndian",
                    index_table_address = "8002002C",
                    file_descriptor = "descriptor"
                },
                Parameters = new EcmParameter[]
                {
                    new EcmParameter()
                    {
                        name = "C_ITN_Fixed_Point",
                        id = "00",
                        description = "Fixed Point datapoint",
                        release_status = "released",
                        data_type = new Fixed_Point()
                        {
                            engr_units = "HEX",
                            engr_min = "min",
                            engr_max = "max",
                            min_resolution = "0.1",
                            sign = "-",
                            data_length = "32",
                            scalar_multiplier = "0.250"
                        },
                        accessible_by_id = "accessible by id",
                        access_method = "access method",
                        group_ids = "0"
                    },
                    new EcmParameter()
                    {
                        name = "C_ITN_Floating_Point",
                        id = "01",
                        description = "Floating Point datapoint",
                        release_status = "released",
                        data_type = new Floating_Point()
                        {
                            engr_units = "HEX",
                            engr_min = "min",
                            engr_max = "max",
                            min_resolution = "0.1",
                            data_length = "32",
                        },
                        accessible_by_id = "accessible by id",
                        access_method = "access method",
                        group_ids = "0"
                    },
                    new EcmParameter()
                    {
                        name = "C_ITN_Code",
                        id = "02",
                        description = "Code datapoint",
                        release_status = "released",
                        data_type = new Code()
                        {
                            data_length = "128"
                        },
                        accessible_by_id = "accessible by id",
                        access_method = "access method",
                        group_ids = "0"
                    },
                    new EcmParameter()
                    {
                        name = "C_ITN_Block",
                        id = "03",
                        description = "Block Datapoint",
                        release_status = "released",
                        data_type = new Block()
                        {
                            data_length = "128"
                        },
                        accessible_by_id = "accessible by id",
                        access_method = "access method",
                        group_ids = "0"
                    },
                    new EcmParameter()
                    {
                        name = "C_ITN_X_Axis",
                        id = "04",
                        description = "X_Axis Datapoint",
                        release_status = "released",
                        data_type = new X_Axis()
                        {
                            x_element_type = new Floating_Point()
                            {
                                engr_units = "HEX",
                                engr_min = "min",
                                engr_max = "max",
                                min_resolution = "0.1",
                                data_length = "32",
                            },
                            legacy_style = "legacy style",
                            element_count = "14",
                        },
                        accessible_by_id = "accessible by id",
                        access_method = "access method",
                        group_ids = "0"
                    },
                    new EcmParameter()
                    {
                        name = "C_ITN_Y_Axis",
                        id = "05",
                        description = "Y_Axis Datapoint",
                        release_status = "released",
                        data_type = new Y_Axis()
                        {
                            y_element_type = new Floating_Point()
                            {
                                engr_units = "HEX",
                                engr_min = "min",
                                engr_max = "max",
                                min_resolution = "0.1",
                                data_length = "32",
                            },
                            legacy_style = "legacy style",
                            element_count = "14",
                        },
                        accessible_by_id = "accessible by id",
                        access_method = "access method",
                        group_ids = "0"
                    },
                    new EcmParameter()
                    {
                        name = "C_ITN_Z_Axis",
                        id = "06",
                        description = "Z_Axis Datapoint",
                        release_status = "released",
                        data_type = new Z_Axis()
                        {
                            z_element_type = new Floating_Point()
                            {
                                engr_units = "HEX",
                                engr_min = "min",
                                engr_max = "max",
                                min_resolution = "0.1",
                                data_length = "32",
                            },
                            related_x_axis_id = "00000000",
                            related_y_axis_id = "00000001"
                        },
                        accessible_by_id = "accessible by id",
                        access_method = "access method",
                        group_ids = "0"
                    },
                    new EcmParameter()
                    {
                        name = "C_ITN_Table",
                        id = "07",
                        description = "Table Datapoint",
                        release_status = "released",
                        data_type = new Table()
                        {
                            element_type = new Floating_Point()
                            {
                                engr_units = "HEX",
                                engr_min = "min",
                                engr_max = "max",
                                min_resolution = "0.1",
                                data_length = "32",
                            },
                            legacy_style = "legacy style",
                            element_count = "14",
                        },
                        accessible_by_id = "accessible by id",
                        access_method = "access method",
                        group_ids = "0"
                    },
                    new EcmParameter()
                    {
                        name = "C_ITN_Enumeration",
                        id = "08",
                        description = "Enumeration Datapoint",
                        release_status = "released",
                        data_type = new Enumeration()
                        {
                            data_length = "2",
                            value = new EnumValuePair[]
                            {
                                new(){ numeric_value = "01", symbolic_value = "Liters"},
                                new(){ numeric_value = "02", symbolic_value = "Gallons"},
                            }
                        },
                        accessible_by_id = "accessible by id",
                        access_method = "access method",
                        group_ids = "0"
                    },
                    new EcmParameter()
                    {
                        name = "C_ITN_Integer",
                        id = "09",
                        description = "Integer Datapoint",
                        release_status = "released",
                        data_type = new Integer()
                        {
                            data_length = "128",
                            sign = "U",
                            display_format = "display format"
                        },
                        accessible_by_id = "accessible by id",
                        access_method = "access method",
                        group_ids = "0"
                    },
                    new EcmParameter()
                    {
                        name = "C_ITN_Structure",
                        id = "0A",
                        description = "Structure Datapoint",
                        release_status = "released",
                        data_type = new Structure()
                        {
                            field_parm_ids = "field Ids",
                        },
                        accessible_by_id = "accessible by id",
                        access_method = "access method",
                        group_ids = "0"
                    },
                    new EcmParameter()
                    {
                        name = "C_ITN_String",
                        id = "0B",
                        description = "String Datapoint",
                        release_status = "released",
                        data_type = new String()
                        {
                            max_length = "128"
                        },
                        accessible_by_id = "accessible by id",
                        access_method = "access method",
                        group_ids = "0"
                    },
                    new EcmParameter()
                    {
                        name = "C_ITN_Array",
                        id = "0C",
                        description = "Array Datapoint",
                        release_status = "released",
                        data_type = new Array()
                        {
                            max_num_elements = "64",
                            element_type = new DataType[]
                            {
                                new Floating_Point()
                                {
                                    engr_units = "HEX",
                                    engr_min = "min",
                                    engr_max = "max",
                                    min_resolution = "0.1",
                                    data_length = "32",
                                },
                                new Floating_Point()
                                {
                                    engr_units = "HEX",
                                    engr_min = "min",
                                    engr_max = "max",
                                    min_resolution = "0.1",
                                    data_length = "32",
                                },
                            }
                        },
                        accessible_by_id = "accessible by id",
                        access_method = "access method",
                        group_ids = "0"
                    },
                    new EcmParameter()
                    {
                        name = "C_ITN_Contiguous_Structure",
                        id = "0D",
                        description = "Contiguous_Structure Datapoint",
                        release_status = "released",
                        data_type = new Contiguous_Structure()
                        {
                            name = "A structure, contiguous in nature",
                            contiguous_field = new StructureField[]
                            {
                                new StructureField()
                                {
                                    description = "A structure field entity.  I think",
                                    element_type = new Floating_Point()
                                    {
                                        engr_units = "HEX",
                                        engr_min = "min",
                                        engr_max = "max",
                                        min_resolution = "0.1",
                                        data_length = "32",
                                    }
                                },
                                new StructureField()
                                {
                                    description = "A structure field entity.  I think",
                                    element_type = new Floating_Point()
                                    {
                                        engr_units = "HEX",
                                        engr_min = "min",
                                        engr_max = "max",
                                        min_resolution = "0.1",
                                        data_length = "32",
                                    }
                                },
                            }
                        },
                        accessible_by_id = "accessible by id",
                        access_method = "access method",
                        group_ids = "0"
                    },
                    new EcmParameter()
                    {
                        name = "C_ITN_Date_Time_Stamp",
                        id = "0E",
                        description = "Date_Time_Stamp Datapoint",
                        release_status = "released",
                        data_type = new Date_Time_Stamp()
                        {
                            display_resolution = "s"
                        },
                        accessible_by_id = "accessible by id",
                        access_method = "access method",
                        group_ids = "0"
                    },
                    new EcmParameter()
                    {
                        name = "C_ITN_Dynamic",
                        id = "0F",
                        description = "Dynamic Datapoint",
                        release_status = "released",
                        data_type = new Dynamic() { },
                        accessible_by_id = "accessible by id",
                        access_method = "access method",
                        group_ids = "0"
                    },
                }
            };
        #endregion
    }

    #region Compatibility Header
    [Serializable]
    [XmlRoot(ElementName = "compatibility_header", Namespace = "http://www.electronics.cummins.com/eti/I")]
    public class EcmCompatibilityHeader
    {
        public string calibration_version { get; set; }
        public string module_name { get; set; }
        public string first_prod_cfg_file_version { get; set; }
        public string product_id { get; set; }
        public string module_part_number { get; set; }
        public string interface_level { get; set; }
        public string creation_date { get; set; }
        public string start_boot_loader_version { get; set; }
        public string end_boot_loader_version { get; set; }
        public string byte_order { get; set; }
        public string index_table_address { get; set; }
        public string file_descriptor { get; set; }
    }
    #endregion
    
    #region Groups
    [Serializable]
    [XmlRoot(ElementName = "group", Namespace = "http://www.electronics.cummins.com/eti/I")]
    public class EcmGroup
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "description")]
        public string Description { get; set; }
    }
    #endregion

    #region Parameter
    [Serializable]
    [XmlRoot(ElementName = "parameter", Namespace = "http://www.electronics.cummins.com/eti/I")]
    public class EcmParameter
    {
        [XmlAttribute()]
        public string name { get; set; }
        public string id { get; set; }
        public string description { get; set; }
        public string release_status { get; set; }
        public DataType data_type { get; set; }
        public string accessible_by_id { get; set; }
        public string access_method { get; set; }
        public string group_ids { get; set; }

        public int GetId()
        {
            int result;
            if (int.TryParse(id, out result))
                return result;
            return -1;
        }
    }
    [Serializable]
    [XmlInclude(typeof(Code))]
    [XmlInclude(typeof(Block))]
    [XmlInclude(typeof(Table))]
    [XmlInclude(typeof(Array))]
    [XmlInclude(typeof(X_Axis))]
    [XmlInclude(typeof(Y_Axis))]
    [XmlInclude(typeof(Z_Axis))]
    [XmlInclude(typeof(String))]
    [XmlInclude(typeof(Dynamic))]
    [XmlInclude(typeof(Integer))]
    [XmlInclude(typeof(Structure))]
    [XmlInclude(typeof(Enumeration))]
    [XmlInclude(typeof(Fixed_Point))]
    [XmlInclude(typeof(Floating_Point))]
    [XmlInclude(typeof(Date_Time_Stamp))]
    [XmlInclude(typeof(Contiguous_Structure))]
    [XmlRoot(ElementName = "data_type", Namespace = "http://www.electronics.cummins.com/eti/I")]
    public class DataType { }
    [Serializable]
    [XmlType(Namespace = "http://www.electronics.cummins.com/eti/I")]
    public class Fixed_Point : DataType
    {
        public string engr_units { get; set; }
        public string engr_min { get; set; }
        public string engr_max { get; set; }
        public string min_resolution { get; set; }
        public string sign { get; set; }
        public string data_length { get; set; }
        public string scalar_multiplier { get; set; }

        public float GetScalarMultiplier()
        {
            float output;
            if (float.TryParse(scalar_multiplier, out output))
                return output;
            return 1.0f;
        }
        public int GetDataLength()
        {
            int output;
            if (int.TryParse(data_length, out output))
                return output;
            return 0;
        }
    }
    [Serializable]
    [XmlType(Namespace = "http://www.electronics.cummins.com/eti/I")]
    public class Floating_Point : DataType
    {
        public string engr_units { get; set; }
        public string engr_min { get; set; }
        public string engr_max { get; set; }
        public string min_resolution { get; set; }
        public string data_length { get; set; }
    }
    [Serializable]
    [XmlType(Namespace = "http://www.electronics.cummins.com/eti/I")]
    public class Code : DataType
    {
        public string data_length { get; set; }
    }
    [Serializable]
    [XmlType(Namespace = "http://www.electronics.cummins.com/eti/I")]
    public class Block : DataType
    {
        public string data_length { get; set; }
    }
    [Serializable]
    [XmlType(Namespace = "http://www.electronics.cummins.com/eti/I")]
    public class X_Axis : DataType
    {
        public DataType x_element_type { get; set; }
        public string legacy_style { get; set; }
        public string element_count { get; set; }
    }
    [Serializable]
    [XmlType(Namespace = "http://www.electronics.cummins.com/eti/I")]
    public class Y_Axis : DataType
    {
        public DataType y_element_type { get; set; }
        public string legacy_style { get; set; }
        public string element_count { get; set; }
        public string related_x_axis_id { get; set; }
        public int GetXAxisId()
        {
            int output;
            if (int.TryParse(related_x_axis_id, out output))
                return output;
            return 0;
        }
            
    }
    [Serializable]
    [XmlType(Namespace = "http://www.electronics.cummins.com/eti/I")]
    public class Z_Axis : DataType
    {
        public DataType z_element_type { get; set; }
        public string related_x_axis_id { get; set; }
        public string related_y_axis_id { get; set; }
    }
    [Serializable]
    [XmlType(Namespace = "http://www.electronics.cummins.com/eti/I")]
    public class Table : DataType
    {
        public DataType element_type { get; set; }
        public string legacy_style { get; set; }
        public string element_count { get; set; }
    }
    [Serializable]
    [XmlType(Namespace = "http://www.electronics.cummins.com/eti/I")]
    public class Enumeration : DataType
    {
        public string data_length { get; set; }
        public EnumValuePair[] value { get; set; }
    }
    [Serializable]
    [XmlType(Namespace = "http://www.electronics.cummins.com/eti/I")]
    public class EnumValuePair
    {
        public string numeric_value { get; set; }
        public string symbolic_value { get; set; }
    }
    [Serializable]
    [XmlType(Namespace = "http://www.electronics.cummins.com/eti/I")]
    public class Integer : DataType
    {
        public string sign { get; set; }
        public string data_length { get; set; }
        public string display_format { get; set; }
    }
    [Serializable]
    [XmlType(Namespace = "http://www.electronics.cummins.com/eti/I")]
    public class Structure : DataType
    {
        public string field_parm_ids { get; set; }
    }
    [Serializable]
    [XmlType(Namespace = "http://www.electronics.cummins.com/eti/I")]
    public class String : DataType
    {
        public string max_length { get; set; }
    }
    [Serializable]
    [XmlType(Namespace = "http://www.electronics.cummins.com/eti/I")]
    public class Array : DataType
    {
        public string max_num_elements { get; set; }
        public DataType[] element_type { get; set; }
    }
    [Serializable]
    [XmlType(Namespace = "http://www.electronics.cummins.com/eti/I")]
    public class Contiguous_Structure : DataType
    {
        [XmlAttribute()]
        public string name { get; set; }
        public StructureField[] contiguous_field { get; set; }
    }
    [Serializable]
    [XmlType(Namespace = "http://www.electronics.cummins.com/eti/I")]
    public class StructureField
    {
        public string description { get; set; }
        public DataType element_type { get; set; }
    }
    [Serializable]
    [XmlType(Namespace = "http://www.electronics.cummins.com/eti/I")]
    public class Date_Time_Stamp : DataType
    {
        public string display_resolution { get; set; }
    }
    [Serializable]
    [XmlType(Namespace = "http://www.electronics.cummins.com/eti/I")]
    public class Dynamic : DataType { }
    #endregion


}
