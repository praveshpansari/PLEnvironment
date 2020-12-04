﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AssignmentASE
{
    /// <summary>
    /// Circle Shape class inherited from <see cref="Shape"/>
    /// </summary>
    /// <remarks>
    /// This class allows to set and draw a circle of specified radius
    /// </remarks>
    class Circle : Shape
    {
        // The radius of the circle
        int radius;

        public override void set(params int[] list)
        {
            //list[0] is x, list[1] is y, list[2] is radius
            base.set(list[0], list[1]);
            this.radius = list[2];
        }

        public override void draw(Graphics g, bool fill, Pen p, Brush b)
        {
            // iF fill is on draw filled else draw outlined
            if (fill)
                g.FillEllipse(b, x - radius, y - radius, radius * 2, radius * 2);
            else
                g.DrawEllipse(p, x - radius, y - radius, radius * 2, radius * 2);
        }
    }
}


