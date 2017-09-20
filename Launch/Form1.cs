using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Launch
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            follow = new Launch.Follow();
        }
        Follow follow;
        int t = 0;
        bool isMove = true;
        private void Form1_Load(object sender, EventArgs e)
        {
            progressBar1.Maximum = 50;
            progressBar1.Minimum = 0;
            timer1.Start();
            isMove = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (isMove)
            {
                isMove = false;
                follow.Move();
            }
            if (t <= progressBar1.Maximum * 0.85f && !follow.isMoveOver)
            {
                progressBar1.Value = t++;
            }
            else if (follow.isMoveOver)
            {
                t += 2;
                if (t <= progressBar1.Maximum)
                {
                    progressBar1.Value = t;
                }
                else
                {
                    follow.isMoveOver = false;
                    progressBar1.Value = progressBar1.Maximum;
                    Application.DoEvents();
                    timer1.Stop();
                    follow.Open();
                    Environment.Exit(0);
                }
            }
        }
    }
}
