using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Collections.Specialized;
using System.Collections;
using System.Data;            // System.Data.dll  
using System.Data.SqlClient;  // System.Data.dll  
using System.Web.UI.WebControls;

// https://docs.microsoft.com/fr-fr/sql/connect/ado-net/step-3-proof-of-concept-connecting-to-sql-using-ado-net?view=sql-server-2017

namespace parking
{
    public class DBConnect
    {
        SqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        public bool opened = false;
        public int lastIdInserted = 0;
        public string lastError = "";
        public List<string> lastRowsNames;
        public ArrayList lastRows;

        //Constructor
        public DBConnect(string srv, string db, string user, string pass)
        {
            server = srv;
            database = db;
            uid = user;
            password = pass;
            string connectionString = "Server=tcp:" + srv + ",1433;Initial Catalog=" + db + ";Persist Security Info=False;User ID=" + user + ";Password=" + pass + ";MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";
            connection = new SqlConnection(connectionString);
        }

        public void DBConnectString(string srv, string db, string user, string pass)
        {
            string connectionString = "Server=tcp:" + srv + ",1433;Initial Catalog=" + db + ";Persist Security Info=False;User ID=" + user + ";Password=" + pass + ";MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";
            connection.ConnectionString = connectionString;
        }

        // Open connection to database
        public bool OpenConnection()
        {
            // already opened
            if (opened) return true;
            try
            {
                connection.Open();
                opened = true;
                return true;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
                opened = false;
                return false;
            }
        }

        // Close connection
        public bool CloseConnection()
        {
            if (!opened) return true;
            try
            {
                connection.Close();
                opened = false;
                return true;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        // Insert with no image (image type require a specific process
        public int noSelect( string query)
        {
            lastError = "";
            if (opened)
            {
                // Create command and assign the query and connection from the constructor
                var command = new SqlCommand();
                command.Connection = connection;  
                command.CommandType = CommandType.Text;
                command.CommandText = query;
                if (query.Trim() == "")
                {
                    lastError = "Empty command";
                    return 0;
                }
                else
                {
                    try
                    {
                        command.ExecuteScalar();
                    }
                    catch (SqlException ex)
                    {
                        lastError = ex.Message;
                        return 0;
                    }
                }
                return 1;
            }
            return 0;
        }

        // Insert, update etc ... with an image
        public int noSelect(string query, string dataname, byte[] imagedata)
        {
            // The query should contain @image in request to be replace by imagedata
            int newid = 0;
            lastError = "";
            if (opened)
            {
                // Create command and assign the query and connection from the constructor
                var command = new SqlCommand();
                SqlParameter parameter;

                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = query;
                if (query.IndexOf(dataname) >= 0)
                {
                    parameter = new SqlParameter(dataname, SqlDbType.Image);
                    parameter.Value = imagedata;
                    command.Parameters.Add(parameter);
                }
                try
                {
                    newid = (int)command.ExecuteScalar();
                    lastIdInserted = newid;
                }
                catch (SqlException ex)
                {
                    newid = 0;
                    lastError = ex.Message;
                }
                
                return newid;
            }
            return 0;
        }

        private string Bin2Hex(byte[] buffer)
        {
            var hex = new StringBuilder(buffer.Length * 2);
            foreach (byte b in buffer)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public byte[] Hex2Bin(string hexString)
        {
            int bytesCount = (hexString.Length) / 2;
            byte[] bytes = new byte[bytesCount];
            for (int x = 0; x < bytesCount; ++x)
            {
                bytes[x] = Convert.ToByte(hexString.Substring(x * 2, 2), 16);
            }

            return bytes;
        }
        // Select statement
        // Results are stored in an array list, results[i] = object (cast needed)
        public ArrayList Select(string query)
        {
            ArrayList list = new ArrayList();
            lastError = "";
            if (!opened) return list;

            var command = new SqlCommand();
 
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = query;

            // Create a data reader and Execute the command
            try
            {
                SqlDataReader dataReader = command.ExecuteReader();

                // Names of columns 
                List<string> columns = new List<string>();
                for (int i = 0; i < dataReader.FieldCount; i++)
                    columns.Add(dataReader.GetName(i));
                lastRowsNames = columns;

                // Put a row in a associative array collection : row["colname"] = value
                while (dataReader.Read())
                {
                    NameValueCollection row = new NameValueCollection();
                    int size = 0;
                    for (int c = 0; c < columns.Count; c++)
                    {
                        row[columns[c]] = dataReader[columns[c]] + "";
                        if (columns[c] == "sdata" || columns[c] == "enter_sdata" || columns[c] == "exit_sdata")
                            size = (row[columns[c]] == "") ? 0 : Int32.Parse(row[columns[c]]);
                        if (row[columns[c]] == "System.Byte[]" && size != 0) // blob
                        {
                            byte[] rawData = new byte[size];
                            dataReader.GetBytes(c, 0, rawData, 0, (Int32)size);
                            row[columns[c]] = Bin2Hex(rawData);
                            size = 0;
                        }
                    }
                    list.Add(row);
                }

                // Close Data Reader
                dataReader.Close();
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
            }
            lastRows = list;
            return list;
        }

        // Results are stored in datagrid
        public void Select(string query, System.Web.UI.WebControls.Label datagrid)
        {
            //BindingSource bindingSource1 = new BindingSource();
            //ArrayList list = new ArrayList();
            lastError = "";
            if (!opened) return;
            ArrayList results = Select(query);
            string result = @"<table class=""table table-striped table-hover""><thead class=""thead-dark""> ";
            // Col name
            result += "<tr></thead><tbody> ";
            for (int c = 0; c < lastRowsNames.Count; c++)
                result += "<th>"+ lastRowsNames[c]+"</th>";
            result += "</tr>";
            for (int l = 0; l < results.Count; l++)
            {
                result += "<tr>";
                System.Collections.Specialized.NameValueCollection row = (NameValueCollection)results[l];
                for (int c = 0; c < lastRowsNames.Count; c++)
                {
                    string value = row[lastRowsNames[c]];
                    if (row[lastRowsNames[c]] != "" && (lastRowsNames[c] == "enter_picture" || lastRowsNames[c] == "exit_picture" || lastRowsNames[c] == "photoname"))
                    {
                        value = @"<img width=""140"" src=""images/"+ row[lastRowsNames[c]] + @""" />";
                        //byte[] img = Hex2Bin(row[lastRowsNames[c]]);
                        //value = @"<img width=""140"" src=""data:image/jpeg;base64," + Convert.ToBase64String(img) + @"""/>";
                    }
                    result += "<td>" + value + "</td>";
                }
                result += "</tr>";
            }
            result += "</tbody></table>";
            datagrid.Text = result;
        }

        //Count statement
        public int Count( string query)
        {
            //string query = "SELECT Count(*) FROM tableinfo";
            int Count = -1;

            if (opened)
            {
                /*
                // Create Mysql Command
                MySqlCommand cmd = new MySqlCommand(query, connection);

                //ExecuteScalar will return one value
                Count = int.Parse(cmd.ExecuteScalar()+"");
                 * */
            }

            return Count;
        }

    }
}
