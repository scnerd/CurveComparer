/*
 * Right click in the left panel to paint. Left click to erase.
 * You can add, remove, and change hidden layers to the network using the list box and control buttons beneath it.
 * When ready, click the button with a number in it to train the neural net and see its results.
 *    Note that the net is trying to replicate on the right panel what you've painted on the left
 * Blue dots indicate the points on your image used for the net's training data. Also used to calculate error.
 *    Dots can be moved by adjusting the dial at the bottom right of the panel (bottom left of control console)
 * To train more than one epoch at a time, change the dial below the training button (the button with the number in it)
 * 
 * And that's basically it. Play and enjoy. And learn too. Learning's good.
 * 
 * David Maxson
 * scnerd@gmail.com
 * 12/4/13
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CurveComparer
{
    public partial class Form1 : Form
    {
        int width, height;

        Color
            back_color = Color.LightGray,
            vert_color = Color.DarkBlue,
            line_color = Color.Black,
            old_vert_color = Color.LightBlue,
            old_line_color = Color.LightSalmon;
        Pen vert_pen, line_pen, old_vert_pen, old_line_pen;
        int vert_sz = 4, vert_radius = 10, line_sz = 2;

        bool mouse_down = false;
        bool mouse_button = false; // false == left, true == right

        List<Point> left_verts = new List<Point>();

        Bitmap bmpMain, bmpOut;

        private Bitmap MainImg
        {
            get { return bmpMain; }
            set { picMain.Image = bmpMain = value; }
        }

        private Bitmap OutImg
        {
            get { return bmpOut; }
            set { picOut.Image = bmpOut = value; }
        }

        public Form1()
        {
            InitializeComponent();
            width = picMain.Width;
            height = picMain.Height;
            MainImg = new Bitmap(width, height);
            OutImg = new Bitmap(width, height);

            vert_pen = new Pen(vert_color, vert_sz);
            line_pen = new Pen(line_color, line_sz);
            old_vert_pen = new Pen(old_vert_color, vert_sz);
            old_line_pen = new Pen(old_line_color, line_sz);

            ResetAlgorithmsList();
        }

        private void ResetAlgorithmsList()
        {
            lstLayers.Items.Add(new Bezier());
            lstLayers.Items.Add(new Hermite());
            lstLayers.Items.Add(new HermiteCopyEdges());
            lstLayers.Items.Add(new HermiteAutoLoop());
            lstLayers.Items.Add(new CornerCut());
            lstLayers.Items.Add(new BSpline());
            lstLayers.Items.Add(new BSplineAutoLoop());
            lstLayers.SelectedIndex = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MarkModified();
        }

        private void picMain_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private void picMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                left_verts.Add(e.Location);
            else
                left_verts.Clear();

            MarkModified();
        }

        private void lstLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            MarkModified();
        }

        private void numIterations_ValueChanged(object sender, EventArgs e)
        {
            MarkModified();
        }

        private void MarkModified()
        {
            DrawLeftImage();
            var sol = SolveRightImage();
            DrawRightImage(sol);
        }

        private void DrawLeftImage()
        {
            var drawer = Graphics.FromImage(MainImg);
            drawer.Clear(back_color);
            if (left_verts.Count > 0)
            {
                if (chkShowVerts.Checked)
                    drawer.DrawEllipse(vert_pen, GetRectFromRadius(left_verts[0], vert_radius));
                for (int i = 1; i < left_verts.Count; i++)
                {
                    if (chkShowVerts.Checked)
                        drawer.DrawEllipse(vert_pen, GetRectFromRadius(left_verts[i], vert_radius));
                    drawer.DrawLine(line_pen, left_verts[i - 1], left_verts[i]);
                }
            }
            picMain.Refresh();
        }

        private Rectangle GetRectFromRadius(Point p, int radius)
        {
            return new Rectangle(p.X - radius, p.Y - radius, 2 * radius, 2 * radius);
        }

        private Point[] SolveRightImage()
        {
            //TODO: Implement using some other algorithm

            return ((CurveAlgorithm)lstLayers.SelectedItem).MakeCurve(left_verts.Select(p => new PointF(p.X, p.Y)).ToArray(), (int)numIterations.Value).Select(pf => new Point((int)pf.X, (int)pf.Y)).ToArray();
        }

        private void DrawRightImage(Point[] solution)
        {
            var drawer = Graphics.FromImage(OutImg);
            drawer.Clear(back_color);
            if (left_verts.Count > 0)
            {
                if (chkShowVerts.Checked)
                    drawer.DrawEllipse(old_vert_pen, GetRectFromRadius(left_verts[0], vert_radius));
                for (int i = 1; i < left_verts.Count; i++)
                {
                    if (chkShowVerts.Checked)
                        drawer.DrawEllipse(old_vert_pen, GetRectFromRadius(left_verts[i], vert_radius));
                    drawer.DrawLine(old_line_pen, left_verts[i - 1], left_verts[i]);
                }
            }

            if (solution.Length > 0)
            {
                if (chkShowVerts.Checked)
                    drawer.DrawEllipse(vert_pen, GetRectFromRadius(solution[0], vert_radius));
                for (int i = 1; i < solution.Length; i++)
                {
                    if (chkShowVerts.Checked)
                        drawer.DrawEllipse(vert_pen, GetRectFromRadius(solution[i], vert_radius));
                    drawer.DrawLine(line_pen, solution[i - 1], solution[i]);
                }
            }
            picOut.Refresh();
        }

        private void chkShowVerts_CheckedChanged(object sender, EventArgs e)
        {
            MarkModified();
        }
    }
}