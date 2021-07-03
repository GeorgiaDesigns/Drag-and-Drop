using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.IO;

namespace DragDrop1
{
    public partial class Form1 : Form
    {
        SqlConnection myConnection;
        string conStr;
        byte[] vetorImagem;
        Image imagem;

        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            pb.DoDragDrop(pb.Image, DragDropEffects.Copy | DragDropEffects.Move);
        }

        private void pbDestino_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Bitmap))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void pbDestino_DragDrop(object sender, DragEventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            pb.Image = (Bitmap)e.Data.GetData(DataFormats.Bitmap);
        }

        private void btnConexao_Click(object sender, EventArgs e)
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["DragDrop1.Properties.Settings.PRConnectionString"];

            if (settings == null)
            {
                MessageBox.Show("Não encontrou connection string.");
            }
            else
            {
                conStr = settings.ConnectionString;
            }

            if (myConnection == null)
            {
                myConnection = new SqlConnection(conStr);
            }

            try
            {
                if (myConnection.State == ConnectionState.Closed)
                {
                    myConnection.Open();
                    label1.Text = "Aberto";
                }
                else
                {
                    myConnection.Close();
                    label1.Text = "Fechado";
                }
            }
            catch(SqlException erro)
            {
                MessageBox.Show("Erro no SQL.");
            }
        }

        private void btnMostrarDados_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("select id, descricao from PV_Coisas", myConnection);
            cmd.CommandType = CommandType.Text;

            try
            {
                SqlDataReader rdr = cmd.ExecuteReader();
                while (!rdr.IsClosed)
                {
                    DataTable dt = new DataTable();
                    dt.Load(rdr);
                    dataGridView1.DataSource = dt;
                }

                rdr.Close();
            }
            catch(Exception erro)
            {
                MessageBox.Show("Erro no SQL!");
            }
        }

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            textBox1.Text = e.Data.GetData(DataFormats.Text).ToString();
        }

        private void textBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Bitmap))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            PictureBox pictureBox1 = (PictureBox)sender;
            pictureBox1.Image = (Bitmap)e.Data.GetData(DataFormats.Bitmap);
        }

        private void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Bitmap))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        public static Bitmap byteToImage(byte[] blob)
        {
            MemoryStream mStream = new MemoryStream();
            byte[] pData = blob;
            mStream.Write(pData, 0, Convert.ToInt32(pData.Length));
            Bitmap bm = new Bitmap(mStream, false);
            mStream.Dispose();
            return bm;
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView dataGridView1 = (DataGridView)sender;
            Bitmap bmp = null;
            try
            {
                SqlCommand cmd = new SqlCommand("select foto from PV_Coisas where id = @id", myConnection);
                cmd.Parameters.Add("@id", System.Data.SqlDbType.Int, 4);
                cmd.Parameters["@id"].Value = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);
                vetorImagem = (byte[])cmd.ExecuteScalar();
                if (vetorImagem == null)
                {
                    MessageBox.Show("Valor de ID inválido.");
                    return;
                }

                bmp = byteToImage(vetorImagem);
            }
            catch(Exception erro)
            {
                MessageBox.Show("Erro no SQL.");
            }

            dataGridView1.DoDragDrop(bmp, DragDropEffects.All);
        }
    }
}
