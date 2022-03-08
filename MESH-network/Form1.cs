using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MESH_network
{
    public partial class Form1 : Form
    {
        int w = 50, h = 50;
        int old_x, old_y;
        List<Circle> lst = new List<Circle>();
        SolidBrush br = new SolidBrush(Color.FromArgb(255, 255, 255));
        string btmu;
        int value;
        int indexRecip, indexTruns;
        int nextID;
        List<string> lstload = new List<string>();

        /// <summary>
        /// Метод кнопки создания узла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_add_Click(object sender, EventArgs e)
        {        
            Circle fig = new Circle();
            fig.name = tb_name.Text;
            fig.description = tb_desc.Text;
            fig.location = tb_loc.Text;
            if (fig == null) return;
            fig.pos_x = 100.0f;
            fig.pos_y = 100.0f;
            lst.Add(fig);
            Pictures.Invalidate();
            Program.circle.Sizearray(lst.ToList());
            tb_desc.Clear();
            tb_loc.Clear();
            tb_name.Clear();
        }           

        /// <summary>
        /// Метод перемещения узла по карте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pictures_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int dx = e.X - old_x;
                int dy = e.Y - old_y;

                foreach (Circle fig in lst)
                {
                    if (fig.selected == false) continue;
                    fig.pos_x += dx;
                    fig.pos_y += dy;
                }
                Pictures.Invalidate();
            }
            old_x = e.X;
            old_y = e.Y;
        }

        /// <summary>
        /// Метод выбора(select) узла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pictures_MouseDown(object sender, MouseEventArgs e)
        {
            Pictures.MouseWheel += new MouseEventHandler(Pictures_MouseWheel);
            if (e.Button == MouseButtons.Left)
            {
                btmu = "left";
                foreach (Circle fig in lst)
                    fig.selected = false;

                for (int i = lst.Count - 1; i >= 0; i--)
                {
                    Circle fig = lst[i];
                    fig.selected |= fig.test(e.X, e.Y);
                    if (fig.selected == true)
                    {
                        indexTruns = i;
                        break;
                    }
                }
            }
            else if(e.Button == MouseButtons.Right)
            {
                btmu = "right";
                foreach (Circle fig in lst)
                    fig.selected = false;

                for (int i = lst.Count - 1; i >= 0; i--)
                {
                    Circle fig = lst[i];
                    fig.selected |= fig.test(e.X, e.Y);
                    if (fig.selected == true)
                    {
                        indexRecip = i;
                        break;
                    }
                }
            }            
            Pictures.Invalidate();
        }

        /// <summary>
        /// Метод изменения размеров круга маршрутизации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pictures_MouseWheel(object sender, MouseEventArgs e)
        {
            foreach (Circle fig in lst)
                fig.selected = false;

            for (int i = lst.Count - 1; i >= 0; i--)
            {
                Circle fig = lst[i];
                fig.selected |= fig.test(e.X, e.Y);
                if (fig.selected == true)
                {
                    if (e.Delta > 10)
                    {
                        value += 2;
                        btmu = "wheelup";
                    }
                    else if (e.Delta < 0)
                    {
                        if (value != 0)
                            value -= 2;
                        btmu = "wheeldown";
                    }
                    break;
                }
            }            
            Pictures.Invalidate();
        }

        /// <summary>
        /// Метод изменения размеров окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Resize(object sender, EventArgs e)
        {
            w = ClientRectangle.Width - 350;
            h = ClientRectangle.Height - Pictures.Top - 10;

            Pictures.Width = w;
            Pictures.Height = h;
        }

        private void bt_save_Click(object sender, EventArgs e)
        {
            nextID = lst.Count + 1;
            Program.circle.RecordInArr(indexRecip, indexTruns, nextID);
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
            {
                foreach(string s in Program.circle.circles2)
                sw.WriteLine(s);
                sw.Close();
            }
        }

        private void bt_load_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "txt files (*.txt)|*.txt";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;


            using (StreamReader sr = new StreamReader(openFileDialog1.FileName))
            {               
                foreach (string line in File.ReadAllLines(openFileDialog1.FileName).Skip(2))
                {       
                    Circle fig = new Circle();
                    string[] s = new string[8];                    
                    for (int i = 0; i < s.Length; i++)
                    {
                        s = line.Split(' ');                        
                    }
                    fig.SetLoadMap(s[0], s[1], s[2], s[3], s[4], s[5], s[6], s[7]);
                    lst.Add(fig);
                }

                foreach (string line3 in File.ReadAllLines(openFileDialog1.FileName).Take(1))
                {
                    Circle fig = new Circle();
                    fig.nextID = Convert.ToInt32(line3);
                }

                string[] s2 = new string[2];
                foreach (string line4 in File.ReadAllLines(openFileDialog1.FileName).Skip(1).Take(1))
                {
                    s2 = line4.Split(' ');
                    Circle fig = new Circle();
                    fig.IDTrunsmitter = Convert.ToInt32(s2[0]);
                    fig.IDRecipient = Convert.ToInt32(s2[1]);
                }                
                sr.Close();                
            }                       

            Pictures.Invalidate();
        }

        /// <summary>
        /// Метод кнопки удаления выбранного узла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_rem_Click(object sender, EventArgs e)
        {            
            int i = 0;
            while (i < lst.Count)
            {
                if (lst[i].selected == true)
                {
                    lst.RemoveAt(i);
                }
                i++;
            }
            Pictures.Invalidate();
        }

        /// <summary>
        /// Метод перерисовки карты при изменениях
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pictures_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(br, 0, 0, Pictures.Width, Pictures.Height);
            foreach (Circle fig in lst)
            {
                fig.draw(e.Graphics, btmu);
                fig.drawTrans(e.Graphics, btmu, value);                
            }            
        }

        public Form1()
        {            
            InitializeComponent();            
        }               

    }
}
