using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MESH_network
{
    internal class Circle
    {        
        Guid id;                                //id узла
        public float radius;                    //радиус узла
        public float radius_squared;            //площадь узла
        public float diametr;                   //диаметр узла
        public string name;                     //имя узла
        public string description;              //описание узла
        public string location;                 //местоположение узла
        public float diametr_trunsmitter;       //диаметр круга маршрутизации
        public float pos_x, pos_y;              //начальные координаты отрисовки узлов и кругов маршрутизации
        public bool selected;                   //выделение узла
        public float x0new, y0new, diametrnew;  //новые коориднаты и диаметр отрисовки узла маршрутизации
        public float dx, dy;
        public int nextID;
        public int IDTrunsmitter;
        public int IDRecipient;


        public List<string> circles2 = new List<string>(2);
        List<Circle> circles = new List<Circle>();
        
        /// <summary>
        /// Метод созднания id узла и назначене радиуса узла
        /// </summary>
        public Circle()
        {
            id = Guid.NewGuid();
            setRadius(25.0f);
        }

        /// <summary>
        /// Метод назначения размерности массива
        /// </summary>
        /// <param name="count">Переменная размера массива</param>
        public void Sizearray(List<Circle> lst)
        {
            circles = lst.ToList();
        }

        /// <summary>
        /// Метод выделения узла
        /// </summary>
        /// <param name="x">X мыши</param>
        /// <param name="y">Y мыши</param>
        /// <returns></returns>
        public bool test(float x, float y)
        {
            dx = x - pos_x;
            dy = y - pos_y;

            if (dx * dx + dy * dy <= radius_squared) return true;
            return false;
        }

        /// <summary>
        /// Метод отрисовки узла
        /// </summary>
        /// <param name="g">переменная свойства Graphics, для отрисовки</param>
        /// <param name="buttonmouse">Какая кнопка мыши была нажата</param>
        public void draw(Graphics g, string buttonmouse)
        {            
            float x0 = pos_x - radius;
            float y0 = pos_y - radius;
            Pen p = Pens.Blue;

            switch (buttonmouse)
            {
                case "left":
                    if (selected == true)
                        p = Pens.Green;
                    break;
                case "right":
                    if (selected == true)
                        p = Pens.Pink;
                    break;
            }                       
            g.DrawEllipse(p, x0, y0, diametr, diametr);
        }

        /// <summary>
        /// Метод отрисовки круга маршрутизации у узлов
        /// </summary>
        /// <param name="circle">переменная свйоства Graphics для отрисовки круга маршрутизации</param>
        /// <param name="buttonmouse">Какая кнопка мыши была нажата</param>
        /// <param name="value">переменная скейлинга радиуса круга маршрутизации</param>
        public void drawTrans(Graphics circle, string buttonmouse, int value)
        {            
            changedlocation(pos_x, pos_y, value, diametr_trunsmitter);
            Pen t = Pens.Black;
            switch (buttonmouse)
            {
                case "left":
                    if (selected == true)
                        circle.DrawEllipse(t = Pens.Black, x0new, y0new, diametrnew, diametrnew);
                    break;
                case "right":                    
                    break;
                case "wheelup":                    
                    if (selected == true)
                        circle.DrawEllipse(t = Pens.Black, x0new, y0new, diametrnew, diametrnew);
                    break;
                case "wheeldown":
                    if (selected == true)
                        circle.DrawEllipse(t = Pens.Black, x0new, y0new, diametrnew, diametrnew);
                    break;
            }            
        }

        /// <summary>
        /// Метод сбора информации об узлах
        /// </summary>
        /// <param name="indexRecepient">индекс получателя</param>
        /// <param name="indexTrunsmitter">индекс отправителя</param>
        /// <param name="nextID">индекс следующего узла</param>
        public void RecordInArr(int indexRecepient, int indexTrunsmitter, int nextID)
        {
            circles2.Clear();
            circles2.Add(nextID.ToString());
            circles2.Add(indexTrunsmitter.ToString() + " " + indexRecepient.ToString());
            for (int i = 0; i < circles.Count; i++)
            {
                circles2.Add(circles[i].pos_x + " " + circles[i].pos_y + " " + circles[i].diametrnew + " " + circles[i].radius + " " + circles[i].id + " " + circles[i].name + " " + circles[i].description + " " + circles[i].location);
            }                        
        }

        /// <summary>
        /// Метод расчета точки отрисовки окружностей маршрутизации и их размеров
        /// </summary>
        /// <param name="pos_x">Начальная точка отрисовки по оси OX (Левый верхний угол)</param>
        /// <param name="pos_y">Начальная точка отрисовки по оси OY (Левый верхний угол)</param>
        /// <param name="value">переменная скейлинга радиуса и координат круга маршрутизации</param>
        /// <param name="diametr_trusmitter">диаметр круга маршрутизации</param>
        private void changedlocation(float pos_x,  float pos_y, int value,float diametr_trusmitter)
        {
            x0new = pos_x - diametr - value;
            y0new = pos_y - diametr - value;
            diametrnew = diametr_trunsmitter + value * 2;
        }

        /// <summary>
        /// Метод подсчета радиуса, площади и диаметра узла
        /// </summary>
        /// <param name="new_radius">переменная с заданным радиусом</param>
        public void setRadius(float new_radius)
        {
            radius = new_radius;
            radius_squared = radius * radius;
            diametr = radius * 2.0f;
            diametr_trunsmitter = radius * 4.0f;
        }

        public void SetLoadMap(string s0, string s1, string s2, string s3, string s4, string s5, string s6, string s7)
        {
            pos_x = Convert.ToInt32(s0);
            pos_y = Convert.ToInt32(s1);
            diametrnew = Convert.ToInt32(s2);
            radius = Convert.ToInt32(s3);
            id = new Guid (s4);
            name = s5;
            description = s6;
            location = s7;            
        }        
    }
}
