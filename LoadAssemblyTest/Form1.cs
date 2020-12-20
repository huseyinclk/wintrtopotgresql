using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LoadAssemblyTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        object connection = null;

        private void button1_Click(object sender, EventArgs e)
        {
            ConnectNpg();
            System.Data.DataTable dt = FillTableFromDb("SELECT worderacop_id FROM prdt_worderacop ORDER BY worderacop_id DESC LIMIT 50");
            if (dt != null && dt.Rows.Count > 0)
            {
                MessageBox.Show("okunan:" + dt.Rows[0][0].ToString());
            }
        }

        public bool ConnectNpg()
        {
            try
            {
                System.Reflection.Assembly assambly = System.Reflection.Assembly.LoadFile(Application.StartupPath + @"\Npgsql.dll");
                Type connType = assambly.GetType("Npgsql.NpgsqlConnection");
                connection = Activator.CreateInstance(connType);
                System.Reflection.PropertyInfo property = connType.GetProperty("ConnectionString");
                property.SetValue(connection, "Server=192.168.9.74;User Id=postgres;Password=20012001;Database=WHM;Port=5432;Pooling=False;ApplicationName=OPCClient",
                    null);
                System.Reflection.MethodInfo methodInfo = connType.GetMethod("Open");
                object[] parameters = new object[0];
                object r = methodInfo.Invoke(connection, null);

                return ((System.Data.IDbConnection)connection).State == System.Data.ConnectionState.Open;
            }
            catch (Exception exception)
            {
                System.IO.File.WriteAllText(Application.StartupPath + @"\Npgsql_conn.log", exception.Message);
            }

            return false;
        }

        public System.Data.DataTable FillTableFromDb(string sqlquery)
        {
            try
            {
                if (connection != null)
                {
                    System.Reflection.Assembly assambly = System.Reflection.Assembly.LoadFile(Application.StartupPath + @"\Npgsql.dll");
                    Type adapterType = assambly.GetType("Npgsql.NpgsqlDataAdapter");
                    object dataAdapter = Activator.CreateInstance(adapterType);
                    System.Data.IDbCommand command = ((System.Data.IDbConnection)connection).CreateCommand();
                    command.CommandText = sqlquery;
                    ((System.Data.IDbDataAdapter)dataAdapter).SelectCommand = command;
                    System.Data.DataSet ds = new System.Data.DataSet();
                    ((System.Data.IDbDataAdapter)dataAdapter).Fill(ds);
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        return ds.Tables[0];
                    }
                }
            }
            catch (Exception exception)
            {
                System.IO.File.WriteAllText(Application.StartupPath + @"\Npgsql_load.log", exception.Message);
            }

            return null;
        }
    }
}
