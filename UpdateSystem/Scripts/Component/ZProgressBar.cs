using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace UpdateSystem
{
    public partial class ZProgressBar : PictureBox
    {
        public ZProgressBar()
        {
            InitializeComponent();
        }
        
        [Localizable(true)]
        public Image TestImage
        {
            get;set;
        }
    }
}
