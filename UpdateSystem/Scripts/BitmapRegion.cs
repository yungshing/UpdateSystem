using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;

    class BitmapRegion
    {
        /// <summary> 
        /// 创建支持位图区域的控件（目前有button和form）
        /// </summary> 
        /// <param name="control">控件</param> 
        /// <param name="bitmap">位图</param> 
        public static void CreateControlRegion(Control control, Bitmap bitmap)
        {
            //判断是否存在控件和位图
            if (control == null || bitmap == null)
                return;

            //设置控件大小为位图大小
            control.Width = bitmap.Width;
            control.Height = bitmap.Height;

            //当控件是form时
            if (control is System.Windows.Forms.Form)
            {
                //强制转换为FORM
                Form form = (Form)control;
                //当FORM的边界FormBorderStyle不为NONE时，应将FORM的大小设置成比位图大小稍大一点
                form.Width = control.Width;
                form.Height = control.Height;
                //没有边界
                form.FormBorderStyle = FormBorderStyle.None;
                //将位图设置成窗体背景图片
                form.BackgroundImage = bitmap;
                //计算位图中不透明部分的边界
                GraphicsPath graphicsPath = CalculateControlGraphicsPath(bitmap);
                //应用新的区域
                form.Region = new Region(graphicsPath);
            }

            //当控件是button时
            else if (control is System.Windows.Forms.Button)
            {
                //强制转换为 button
                Button button = (Button)control;
                //不显示button text
                button.Text = "";
                //改变 cursor的style
                button.Cursor = Cursors.Hand;
                //设置button的背景图片
                button.BackgroundImage = bitmap;
                //计算位图中不透明部分的边界
                GraphicsPath graphicsPath = CalculateControlGraphicsPath(bitmap);
                //应用新的区域
                button.Region = new Region(graphicsPath);
            }
        }
        /// <summary> 
        /// //计算位图中不透明部分的边界
        /// </summary> 
        /// <param name="bitmap">位图</param> 
        /// <returns>计算得出的图片路径集</returns> 
        private static GraphicsPath CalculateControlGraphicsPath(Bitmap bitmap)
        {
            //创建 GraphicsPath
            GraphicsPath graphicsPath = new GraphicsPath();
            //使用左上角的一点的颜色作为我们透明色
            Color colorTransparent = bitmap.GetPixel(0, 0);
            //第一个找到点的X
            int colOpaquePixel = 0;
            // 偏历所有行（Y方向）
            for (int row = 0; row < bitmap.Height; row++)
            {
                //重设
                colOpaquePixel = 0;
                //偏历所有列（X方向）
                for (int col = 0; col < bitmap.Width; col++)
                {
                    //如果是不需要透明处理的点则标记，然后继续偏历
                    if (bitmap.GetPixel(col, row) != colorTransparent)
                    {
                        //记录当前
                        colOpaquePixel = col;

                        //建立新变量来记录当前点
                        int colNext = col;

                        ///从找到的不透明点开始，继续寻找不透明点,一直到找到或则达到图片宽度 
                        for (colNext = colOpaquePixel; colNext < bitmap.Width; colNext++)
                            if (bitmap.GetPixel(colNext, row) == colorTransparent)
                                break;

                        //将不透明点加到graphics path
                        graphicsPath.AddRectangle(new Rectangle(colOpaquePixel, row, colNext - colOpaquePixel, 1));
                        col = colNext;
                    }
                }
            }
            
            return graphicsPath;
        }
    }
