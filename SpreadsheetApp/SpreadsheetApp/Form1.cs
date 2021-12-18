using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Threading;
using System.IO;


namespace SpreadSheetApp
{
    public partial class Form1 : Form
    {
        SharableSpreadSheet spreadsheet = new SharableSpreadSheet(0,0);
        public Form1()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e) //save
        {
 
            spreadsheet.save(textBox10.Text);
            MessageBox.Show("Saving...");
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)// Load button
        {
            if (!File.Exists(textBox9.Text))
            {
                MessageBox.Show("The file not exists in current path - please enter full path.");
                return;
            }
            spreadsheet.load(textBox9.Text);
            int rows = spreadsheet.getRows();
            int cols = spreadsheet.getCols();
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            for (int i = 0; i < cols; i++)
            {
                dataGridView1.Columns.Add("col name", "col" + i);
            }
            for (int j = 0; j < rows; j++)
            {
                dataGridView1.Rows.Add("row name", "row" + j);
            }
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    String data = spreadsheet.getCell(j, i);
                    dataGridView1[i,j].Value = data;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e) //add row
        {
            MessageBox.Show("Please enter value in the text boxes");
        }

        private void button5_Click(object sender, EventArgs e) // set cell
        {

            MessageBox.Show("Please enter values in the text boxes");
        }
        
        
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)//value for row number in set cell
        {
         
           
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e) // get cell
        {

            MessageBox.Show("Please enter values in the text boxes");

        }

        private void button2_Click(object sender, EventArgs e) //add col
        {
            MessageBox.Show("Please enter value in the text boxes");
        }

        private void textBox2_TextChanged(object sender, EventArgs e)//value for col number in set cell
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)//value for col number in get cell
        {

        }

        private void button7_Click(object sender, EventArgs e) // set cell OK 
        {
            //int row = Int32.Parse(textBox1.Text);
            //int col = Int32.Parse(textBox2.Text);

            int row;
            int col;
            bool isNumRow = int.TryParse(textBox1.Text, out row);
            bool isNumCol= int.TryParse(textBox2.Text, out col);
            if (!isNumRow || !isNumCol)
            {
                MessageBox.Show("incorrect value - must be integer");
                return;
            }
            if (row == 0 || col ==0)
            {
                MessageBox.Show("incorrect value - indexes start from 1");
                return;
            }
            if (row > spreadsheet.getRows())
            {
                MessageBox.Show("incorrect value - must be "+ spreadsheet.getRows() + " or less");
                return;
            }
            if(col > spreadsheet.getCols())
            {
                MessageBox.Show("incorrect value - must be " + spreadsheet.getCols() + " or less");
                return;
            }
            row -= 1;
            col -= 1;
            String str = textBox3.Text;
            spreadsheet.setCell(row, col, str);
            dataGridView1[col, row].Value = str;
            MessageBox.Show("The cell " + textBox1.Text + "," + textBox2.Text + "\nwas set with the string " + "'" + str + "'");

        }

        private void textBox6_TextChanged(object sender, EventArgs e)//value for row number in get cell
        {

        }

        private void button8_Click(object sender, EventArgs e) // get cell OK
        {
            //int row = Int32.Parse(textBox6.Text);
            //int col = Int32.Parse(textBox5.Text);
            int row;
            int col;
            bool isNumRow = int.TryParse(textBox6.Text, out row);
            bool isNumCol = int.TryParse(textBox5.Text, out col);
            if (!isNumRow || !isNumCol)
            {
                MessageBox.Show("incorrect value - must be integer");
                return;
            }

            if (row == 0 || col == 0)
            {
                MessageBox.Show("incorrect value - indexes start from 1");
                return;
            }
            if (row > spreadsheet.getRows())
            {
                MessageBox.Show("incorrect value - must be " + spreadsheet.getRows() + " or less");
                return;
            }
            if (col > spreadsheet.getCols())
            {
                MessageBox.Show("incorrect value - must be " + spreadsheet.getCols() + " or less");
                return;
            }
            row -= 1;
            col -= 1;
            String str = spreadsheet.getCell(row, col); ;
            MessageBox.Show("The string in cell " + textBox6.Text + "," + textBox5.Text + " is " + "'"+str+"'");
        }

        private void textBox7_TextChanged(object sender, EventArgs e)//column number for add row
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e) //add row OK button
        {
            //int row = Int32.Parse(textBox7.Text);
            int row;
            bool isNum = int.TryParse(textBox7.Text, out row);
            if(!isNum)
            {
                MessageBox.Show("incorrect value - must be integer");
                return;
            }
            if (row == 0) { 
                MessageBox.Show("incorrect value - indexes start from 1");
                return;
            }
            if (row > spreadsheet.getRows())
            {
                MessageBox.Show("incorrect value - must be "+ spreadsheet.getRows() + " or less");
                return;
            }
            row -= 1;
            spreadsheet.addRow(row);
            int rows = spreadsheet.getRows();
            int cols = spreadsheet.getCols();
            dataGridView1.Columns.Clear();
            dataGridView1.Refresh();
            dataGridView1.Rows.Clear();
            for (int i = 0; i < cols; i++)
            {
                dataGridView1.Columns.Add("col name", "col" + i);
            }
            for (int j = 0; j < rows; j++)
            {
                dataGridView1.Rows.Add("row name", "row" + j);
            }
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    String data = spreadsheet.getCell(j, i);
                    dataGridView1[i, j].Value = data;
                }
            }
            MessageBox.Show("A new row was added after row number  " + textBox7.Text );


        }

        private void button10_Click(object sender, EventArgs e) //add column OK button
        {
            //int col = Int32.Parse(textBox8.Text);
            int col;
            bool isNum = int.TryParse(textBox8.Text, out col);
            if (!isNum)
            {
                MessageBox.Show("incorrect value - must be integer");
                return;
            }
            if (col == 0)
            {
                MessageBox.Show("incorrect value - indexes start from 1");
                return;
            }
            if(col > spreadsheet.getCols())
            {
                MessageBox.Show("incorrect value - must be 16 or less  "+spreadsheet.getCols());
                return;
            }
            col -= 1;
            spreadsheet.addCol(col);
            int rows = spreadsheet.getRows();
            int cols = spreadsheet.getCols();
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            for (int i = 0; i < cols; i++)
            {
                dataGridView1.Columns.Add("col name", "col" + i);
            }
            for (int j = 0; j < rows; j++)
            {
                dataGridView1.Rows.Add("row name", "row" + j);
            }
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    String data = spreadsheet.getCell(j, i);
                    dataGridView1[i, j].Value = data;
                }
            }
            MessageBox.Show("A new column was added after column number  " + textBox8.Text);


        }

        private void textBox8_TextChanged(object sender, EventArgs e) //column number for add col
        {

        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}

