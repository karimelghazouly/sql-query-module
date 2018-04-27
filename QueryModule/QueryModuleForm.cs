using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QueryModule.QueryParser;

namespace QueryModule
{
    public partial class QueryModuleForm : Form
    {
        public QueryModuleForm()
        {
            InitializeComponent();
            dataGridView1.Visible = false;
            label1.BackColor = Color.Transparent;
            button1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string text = textBox1.Text.Trim();
            if (text == "")
            {
                MessageBox.Show("Please write your SQL query.");
                return;
            }
            List<Token> tokens = lexer.Lex(text);
            ParserResult node = Parser.parse(tokens);
            List<List<string>> data = new List<List<string>>();
            for (int i = 0; i < 3; i++)
            {
                List<string> tt = new List<string>();
                for (int j = 0; j < 3; j++)
                    tt.Add("lol at row " + i.ToString() + " col " + j.ToString());
                data.Add(tt);
            }
            List<string> cols = new List<string>();
            string name = "";
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].tokenType == TokenType.SELECT)
                    continue;
                if (tokens[i].tokenType == TokenType.FROM)
                {
                    cols.Add(name);
                    break;
                }
                if (tokens[i].tokenType == TokenType.COMMA)
                {
                    cols.Add(name);
                    name = "";
                }
                else
                {
                    name += tokens[i].lexeme + " ";
                }
            }
            DataTable table = new DataTable();
            for (int i = 0; i < cols.Count; i++)
                table.Columns.Add(cols[i]);
            for (int i = 0; i < data.Count; i++)
            {
                table.Rows.Add(data[i].ToArray());
            }
            dataGridView1.DataSource = table;
            dataGridView1.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var FD = new System.Windows.Forms.OpenFileDialog();
            if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileToOpen = FD.FileName;
                FileManager.XMLParser.SetDatabasePath(fileToOpen);
                button1.Enabled = true;
            }
        }
    }
}
