using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace UpdateSystem.Scripts.Component
{
    public partial class ZButtn : PictureBox
    {
        public ZButtn()
        {
            InitializeComponent();
        }
        [Localizable(true)]
        public Image OnMouseEnterBG
        {
            get;set;
        }
        [Localizable(true)]
        public Image OnMouseLeaveBG
        {
            get; set;
        }
        [Localizable(true)]
        public Image OnMouseDownBG
        {
            get; set;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (OnMouseEnterBG != null)
            {
                this.Image = OnMouseEnterBG;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (OnMouseLeaveBG != null)
            {
                this.Image = OnMouseLeaveBG;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (OnMouseDownBG!=null)
            {
                this.Image = OnMouseDownBG;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (OnMouseEnterBG != null)
            {
                this.Image = OnMouseEnterBG;
            }
        }
    }
}
