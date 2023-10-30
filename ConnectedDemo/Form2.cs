
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ConnectedDemo
{
    public partial class Form2 : Form
    {
        SqlConnection con;
        SqlDataAdapter da;
        SqlCommandBuilder builder;
        DataSet ds;

        SqlCommand cmd;
        SqlDataReader reader;
        public Form2()
        {
            InitializeComponent();
            con = new SqlConnection(ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString);

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            try
            {
                //Write a query 
                string qry = "select * from Dept";//name of table in DB
                                                  //Assign query to adapter--> Will fire the query
                da = new SqlDataAdapter(qry, con);
                //Create object of dataSet
                ds = new DataSet();
                //Fill() will fire the select query and load data in the ds
                //Dept is a name given to the table in DatSet
                da.Fill(ds, "Dept");
                cmbDept.DataSource = ds.Tables["Dept"];
                cmbDept.DisplayMember = "dname";
                cmbDept.ValueMember = "did";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }
        private DataSet GetEmployees()
        {
            // String qry = "Select * from Employee";
            string qry = "select e.*, d.dname from Employee e inner join dept d on d.did = e.did";

            //assing the query 
            da = new SqlDataAdapter(qry, con);
            //When app load the in DataSet ,we need to manage the PK also
            da.MissingSchemaAction=MissingSchemaAction.AddWithKey;
            //
            builder = new SqlCommandBuilder(da);
            ds=new DataSet();
            da.Fill(ds, "Employee");
            return ds;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                ds = GetEmployees();
                // create new row to add recrod
                DataRow row = ds.Tables["Employee"].NewRow();
                // assign value to the row
                row["name"] = txtName.Text;
                row["email"] = txtEmail.Text;
                row["age"] = txtAge.Text;
                row["salary"] = txtSalary.Text;
                row["did"] = cmbDept.SelectedValue;
                // attach this row in DataSet table
                ds.Tables["Employee"].Rows.Add(row);
                // update the changes from DataSet to DB
                int result = da.Update(ds.Tables["Employee"]);
                if (result >= 1)
                {
                    MessageBox.Show("Record inserted");
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            GetAllEmps();

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                ds = GetEmployees();
                // find the row
                DataRow row = ds.Tables["Employee"].Rows.Find(txtid.Text);
                if (row != null)
                {
                    row["name"] = txtName.Text;
                    row["email"] = txtEmail.Text;
                    row["age"] = txtAge.Text;
                    row["salary"] = txtSalary.Text;
                    row["did"] = cmbDept.SelectedValue;
                    // update the changes from DataSet to DB
                    int result = da.Update(ds.Tables["Employee"]);
                    if (result >= 1)
                    {
                        MessageBox.Show("Record updated");
                        ClearFields();
                    }
                }
                else
                {
                    MessageBox.Show("Id not matched");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            GetAllEmps();

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                ds = GetEmployees();
                // find the row
                DataRow row = ds.Tables["Employee"].Rows.Find(txtid.Text);
                if (row != null)
                {
                    // delete the current row from DataSet table
                    row.Delete();
                    // update the changes from DataSet to DB
                    int result = da.Update(ds.Tables["Employee"]);
                    if (result >= 1)
                    {
                        MessageBox.Show("Record deleted");
                        ClearFields();
                    }
                }
                else
                {
                    MessageBox.Show("Id not matched");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            GetAllEmps();

        }

        private void btnShowAll_Click(object sender, EventArgs e)
        {
            try
            {

                ds = GetEmployees();
                dataGridView1.DataSource = ds.Tables["Employee"];
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string qry = "select e.*, d.dname from Employee e inner join dept d on d.did = e.did";
                da = new SqlDataAdapter(qry, con);
                da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                ds = new DataSet();
                da.Fill(ds, "emp");
                //find method can only seach the data if PK is applied in the DataSet table
                DataRow row = ds.Tables["emp"].Rows.Find(txtid.Text);
                if (row != null)
                {
                    txtName.Text = row["name"].ToString();
                    txtEmail.Text = row["email"].ToString();
                    txtAge.Text = row["age"].ToString();
                    txtSalary.Text = row["salary"].ToString();
                    cmbDept.Text = row["dname"].ToString();
                }
                else
                {
                    MessageBox.Show("Record not found");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }
        private void GetAllEmps()
        {
            string qry = "select e.*, d.dname from Employee e inner join dept d on d.did = e.did";
            cmd = new SqlCommand(qry, con);
            con.Open();
            reader = cmd.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            dataGridView1.DataSource = table;
            con.Close();
        }

        private void ClearFields()
        {
            txtid.Clear();
            txtName.Clear();
            txtSalary.Clear();
            txtAge.Clear();
            txtEmail.Clear();
            cmbDept.ResetText();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtid.Text = dataGridView1.CurrentRow.Cells["id"].Value.ToString();
            txtName.Text = dataGridView1.CurrentRow.Cells["name"].Value.ToString();
            txtAge.Text = dataGridView1.CurrentRow.Cells["age"].Value.ToString();
            txtEmail.Text = dataGridView1.CurrentRow.Cells["Email"].Value.ToString();
            txtSalary.Text = dataGridView1.CurrentRow.Cells["salary"].Value.ToString();
            cmbDept.Text = dataGridView1.CurrentRow.Cells["dname"].Value.ToString();
        }
    }
}
