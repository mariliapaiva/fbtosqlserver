namespace FB2MSSQL.ViewModel
{
    public class Coluna
    {
        public string Name { get; set; }
        public int Type { get; set; }

        public string RealType
        {
            get
            {
                if (Type == 7)
                {
                    if (SubType == 0)
                        return "SMALLINT";
                    if (SubType == 1)
                        return "NUMERIC";
                    if (SubType == 2)
                        return "DECIMAL";
                }
                else if (Type == 8)
                {
                    if (SubType == 0)
                        return "SMALLINT";
                    if (SubType == 1)
                        return "NUMERIC";
                    if (SubType == 2)
                        return "DECIMAL";
                }
                else if (Type == 9)
                    return "QUAD";
                else if (Type == 10)
                    return "FLOAT";
                else if (Type == 12)
                    return "DATE";
                else if (Type == 13)
                    return "TIME";
                else if (Type == 14)
                    return "CHAR";
                else if (Type == 16)
                {
                    if (SubType == 0)
                        return "BIGINT";
                    if (SubType == 1)
                        return "NUMERIC";
                    if (SubType == 2)
                        return "DECIMAL";
                }
                else if (Type == 27)
                    return "DOUBLE";
                else if (Type == 35)
                    return "TIMESTAMP";
                else if (Type == 37)
                    return "VARCHAR";
                else if (Type == 40)
                    return "CSTRING";
                else if (Type == 45)
                    return "BLOB_ID";
                else if (Type == 261)
                    return "BLOB SUB_TYPE";
                
                return null;
            }
        }

        public int? SubType { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Name, RealType);
        }
    }
}