using System.Text.RegularExpressions;

namespace Sql2Poco
{
    public class ResultFieldDetails
    {
        public string ColumnName { get; set; }
        public int ColumnOrdinal { get; set; }
        public int ColumnSize { get; set; }
        public int NumericPrecision { get; set; }
        public int NumericScale { get; set; }
        public bool IsUnique { get; set; }
        public string BaseColumnName { get; set; }
        public string BaseTableName { get; set; }
        public string DataType { get; set; }
        public bool AllowDBNull { get; set; }
        public int ProviderType { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsAutoIncrement { get; set; }
        public bool IsRowVersion { get; set; }
        public bool IsLong { get; set; }
        public bool IsReadOnly { get; set; }
        public string ProviderSpecificDataType { get; set; }
        public string DataTypeName { get; set; }
        public string UdtAssemblyQualifiedName { get; set; }
        public int NewVersionedProviderType { get; set; }
        public bool IsColumnSet { get; set; }
        public string RawProperties { get; set; }
        public int NonVersionedProviderType { get; set; }
        public object FieldValue { get; set; }
        public string CSType
        {
            get
            {
                string notUsed;
                return TypeMapping.DBType2CSType(DataTypeName, out notUsed , AllowDBNull);
            }
        }
        public string CSColumnName
        {
            get
            {
                if (Regex.Match((ColumnName.Substring(0, 1)), "[0-9]").Success)
                    return "_" + ColumnName;
                else
                    return ColumnName;
            }
        }


    }

}
