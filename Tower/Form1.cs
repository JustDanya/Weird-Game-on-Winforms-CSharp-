using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tower
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int Score;

        int Stability;

        PictureBox CurBrick;

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            //
        }

        private Timer timer1;

        bool dir;
        public void InitTimer()
        {
            timer1 = new Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 100; // in miliseconds
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ShakeBrick();
        }


        private void MakeNewBrick()
        {
            Random rng = new Random();
            int loc = Score == 300 ? 407 : rng.Next(0 , 800);
            PictureBox pb = new PictureBox();
            viewport.Controls.Add(pb);
            pb.Location = new Point(loc, 3);
            var bmp = new Bitmap(Tower.Properties.Resources.brick_aniim_stable01);
            Image img = Tower.Properties.Resources.brick_aniim_stable01;
            pb.Image = img;
            pb.Visible = true;
            pb.Size = new Size(291, 126);
            //if (viewport.Controls.Count > 1) label1.Text = "All right";
            CurBrick = pb;
            InitTimer();
        }

        private void ShakeBrick()
        {
            if (dir)
            {
                if (CurBrick.Left + 20 <= 804) ;
                else dir = false;
            }
            else
            {
                if (CurBrick.Left - 20 >= 0) ;
                else dir = true;
            }
            if (dir) CurBrick.Left += 20;
            else CurBrick.Left -= 20;
            Score -= 5;
            scoreL.Text = Convert.ToString(Score);
            if (Score <= 0)
            {
                OnDead();
                timer1.Stop();
                DialogResult dialog = MessageBox.Show("You Too Slooooooow....", "You LOSE!!!", MessageBoxButtons.OK);
                if (dialog == DialogResult.OK) Application.Exit();
            }
        }

        List<Control> Overlaped;

        private void button1_Click(object sender, EventArgs e)
        {
            int Rel = 0;
            timer1.Stop();
            Overlaped.Clear();
            //int i = 0;
            //while (i < 10)
            //{
            //    CurBrick.Top += 20;
            //    label1.Text = Convert.ToString(CurBrick.Location.X);
            //    i++;
            //}
            int LX = CurBrick.Location.X;
            int RX = CurBrick.Location.X + CurBrick.Size.Width;
            foreach (Control ctrl in viewport.Controls )
            {
                if ( ( (ctrl.Location.X + ctrl.Size.Width) >= LX && (ctrl.Location.X) <= LX) || ((ctrl.Location.X + ctrl.Size.Width) >= RX && (ctrl.Location.X) <= RX) )
                {
                    Overlaped.Add(ctrl);
                }
            }
            if (Overlaped.Count-1 > 0)
            {
                int MY = 0;
                for (int i = 0; i < Overlaped.Count-1; i++)
                {
                    if (Overlaped[i].Location.Y < Overlaped[MY].Location.Y)
                    {
                        MY = i;
                    }
                }
                //CurBrick.Location = new Point(CurBrick.Location.X, Overlaped[MY].Location.Y);
                CurBrick.Top += ( (Overlaped[MY].Location.Y - Overlaped[MY].Size.Height) - (CurBrick.Location.Y - CurBrick.Size.Height) ) - CurBrick.Size.Height;


                if ( (Overlaped[MY].Location.X + Overlaped[MY].Size.Width) >= LX && (Overlaped[MY].Location.X) <= LX)
                {
                    //left corner
                    Rel = (Overlaped[MY].Location.X + Overlaped[MY].Size.Width) - CurBrick.Location.X; 
                }
                else if ((Overlaped[MY].Location.X + Overlaped[MY].Size.Width) >= RX && (Overlaped[MY].Location.X) <= RX)
                {

                    //right corner
                    Rel = Overlaped[MY].Location.X - ( CurBrick.Location.X + Overlaped[MY].Size.Width);
                }

                Rel = Math.Abs(Rel);

                if (Rel > 284 && Rel <=291) //excellent
                {
                    Image img = Tower.Properties.Resources.brick_love;
                    CurBrick.Image = img;

                    label1.ForeColor = Color.Green;
                    label1.Text = "EXCELLENT!!!";
                    Score += 300;
                    scoreL.Text = Convert.ToString(Score);
                    if (Stability < 5)
                    Stability += 1;
                    stabL.Text = Convert.ToString(Stability);
                }
                else if (Rel <= 120 && Rel > 60) // bad
                {
                    Image img = Tower.Properties.Resources.brick_stress;
                    CurBrick.Image = img;

                    label1.ForeColor = Color.Red;
                    label1.Text = "Almost DEAD!!!";
                    Score += 50;
                    scoreL.Text = Convert.ToString(Score);
                    Stability -= 1;
                    stabL.Text = Convert.ToString(Stability);
                }
                else if (Rel < 60) // dead
                {
                    label1.ForeColor = Color.Red;
                    label1.Text = "YOU FALL!!!";
                    OnDead();
                    Bitmap bmp = new Bitmap(CurBrick.Size.Width, CurBrick.Size.Height);

                    //turn the Bitmap into a Graphics object
                    Graphics gfx = Graphics.FromImage(bmp);

                    //now we set the rotation point to the center of our image
                    gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);

                    //now rotate the image
                    gfx.RotateTransform(45);

                    gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);

                    //set the InterpolationMode to HighQualityBicubic so to ensure a high
                    //quality image once it is transformed to the specified size
                    gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    //now draw our new image onto the graphics object
                    //gfx.DrawImage(CurBrick.Image, new Point(0, 0) ) ;//CurBrick.Location.X, CurBrick.Location.Y));
                    //gfx.DrawRectangle(Pens.Black, new Rectangle (CurBrick.Location.X, CurBrick.Location.Y, CurBrick.Size.Width, CurBrick.Size.Height) );
                    //viewport.Controls.Remove(CurBrick);

                    //dispose of our Graphics object
                    gfx.Dispose();
                    //
                    DialogResult dialog = MessageBox.Show("Brick fall from Your Tower", "You LOSE!!!", MessageBoxButtons.OK);
                    if (dialog == DialogResult.OK) Application.Exit();
                }
                else //default
                {
                    label1.ForeColor = Color.Yellow;
                    label1.Text = "Good";
                    Score = Score + 150;
                    scoreL.Text = Convert.ToString(Score);
                }
            }
            else
            {
                CurBrick.Top += 691 ; //694
                if (viewport.Controls.Count > 1)
                {
                    OnDead();
                    DialogResult dialog = MessageBox.Show("Brick fall not on another brick", "You LOSE!!!", MessageBoxButtons.OK);
                    if (dialog == DialogResult.OK) Application.Exit();
                }
            }

            if (Stability <= 0)
            {
                OnDead();
                DialogResult dialog = MessageBox.Show("You Tower Fall..", "You LOSE!!!", MessageBoxButtons.OK);
                if (dialog == DialogResult.OK) Application.Exit();
            }
            if (viewport.Controls.Count >= 6)
            {
                viewport.Controls.Clear();
            }
            //label1.Text = Convert.ToString(Rel) + " " + Convert.ToString(CurBrick.Size.Width); ///Convert.ToString(CurBrick.Location.Y) + " " + Convert.ToString(Overlaped.Count);
            MakeNewBrick();


        }

        void OnDead()
        {
            foreach(Control ctrl in viewport.Controls)
            {
                PictureBox dpb = ctrl as PictureBox;
                Image img = Tower.Properties.Resources.brick_end_game;
                dpb.Image = img;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Score = 300;
            Stability = 3;
            dir = true;
            MakeNewBrick();
            Overlaped = new List<Control>();
            scoreL.Text = Convert.ToString(Score);
            stabL.Text = Convert.ToString(Stability);
        }

        private void viewport_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
