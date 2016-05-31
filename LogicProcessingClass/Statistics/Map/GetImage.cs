/*
 * 修改者：吴鹏(AlphaWu)
 * Blog:Http://AlphaWu.CoM
 * 获取网站缩略图代码
 * 2006 圣诞节
 * C#代码根据该页面“http://www.codeproject.com/useritems/website_screenshot.asp”
 * VB.Net代码修改而来
 * 欢迎修改和传播
 * 最好能保留该信息^_^
 * 也欢迎大家访问我的博客
 * Http://AlphaWu.Cnblogs.CoM
 * 
 * Http://AlphaWu.CoM
 * 
 * 
 */
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Threading;

namespace LogicProcessingClass.Statistics
{
    public class GetImage
    {
        public GetImage()
        {

        }

        public Bitmap GetBitmap(string WebSite,string imgName,string imgUrl,int mapWidth,int mapHeight,string legend)
        {
            WebPageBitmap Shot = new WebPageBitmap(WebSite, imgName, imgUrl, mapWidth, mapHeight, legend);
            Bitmap Pic = Shot.DrawBitmap();
            return Pic;
        }

        public Bitmap GetBitmap(string WebSite, int ScreenWidth, int ScreenHeight, int ImageWidth, int ImageHeight)
        {
            WebPageBitmap Shot = new WebPageBitmap(WebSite,ScreenWidth,ScreenHeight);
            Bitmap Pic = Shot.DrawBitmap(ImageWidth,ImageHeight);
            return Pic;
        }
    }

    public class WebPageBitmap
    {
        WebBrowser MyBrowser;
        string URL;
        int Height;
        int Width;

        public int ImgHeight
        {
            get
            {
                return Height;
            }
            set
            {
                Height = value;
            }
        }

        public int ImgWidth
        {
            get
            {
                return Width;
            }
            set
            {
                Width = value;
            }
        }

        public WebPageBitmap(string url,string imgName,string imgUrl,int mapWidth,int mapHeight,string legend)
        {
            this.URL = url;
            MyBrowser = new WebBrowser();
            MyBrowser.ScrollBarsEnabled = false;

            MyBrowser.Navigate(this.URL);
            WaitWebPageLoad();

            Object[] args = new Object[]{imgName, imgUrl, mapWidth, mapHeight, legend};
            MyBrowser.Document.InvokeScript("showMap", args);
            WaitWebPageLoad();

            var content = MyBrowser.Document.GetElementById("content");
            this.ImgWidth = content.ClientRectangle.Width;
            this.ImgHeight = content.ClientRectangle.Height;
            MyBrowser.Size = new Size(this.ImgWidth, this.ImgHeight);
        }

        public WebPageBitmap(string url, string title, string subtitle, string names, string datas, string dataUnit,int width)
        {

            this.URL = url;
            MyBrowser = new WebBrowser();
            MyBrowser.ScrollBarsEnabled = false;

            MyBrowser.Navigate(this.URL);
            WaitWebPageLoad();

            Object[] args = new Object[] { title,  subtitle, names, datas, dataUnit,width};
            MyBrowser.Document.InvokeScript("showPieChart", args);
            WaitWebPageLoad();

            var content = MyBrowser.Document.GetElementById("floatbox");
            this.ImgWidth = content.ClientRectangle.Width;
            this.ImgHeight = content.ClientRectangle.Height;
            MyBrowser.Size = new Size(this.ImgWidth, this.ImgHeight);
        }

        public WebPageBitmap(string url, int width, int height)
        {
            this.URL = url;
            this.ImgWidth = width;
            this.ImgHeight = height;
            MyBrowser = new WebBrowser();
            MyBrowser.ScrollBarsEnabled = false;
            MyBrowser.Size = new Size(this.ImgWidth, this.ImgHeight);

            MyBrowser.Navigate(this.URL);
            WaitWebPageLoad();
        }

        //等待页面加载
        private bool WaitWebPageLoad()
        {
            //while (MyBrowser.ReadyState != WebBrowserReadyState.Complete)
            //{
            //    Application.DoEvents();
            //}
            int i = 0;
            string sUrl;
            while (true)
            {
                Delay(10);  //系统延迟10毫秒，够少了吧！             
                if (MyBrowser.ReadyState == WebBrowserReadyState.Complete) //先判断是否发生完成事件。
                {
                    if (!MyBrowser.IsBusy) //再判断是浏览器是否繁忙                  
                    {
                        i = i + 1;
                        if (i == 2)   //为什么 是2呢？因为每次加载frame完成时就会置IsBusy为false,未完成就就置IsBusy为false，你想一想，加载一次，然后再一次，再一次...... 最后一次.......
                        {
                            sUrl = MyBrowser.Url.ToString();
                            if (sUrl.Contains("res")) //这是判断没有网络的情况下                           
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        continue;
                    }
                    i = 0;
                }
            }
        }

        //延迟系统时间，但系统又能同时能执行其它任务；
        private void Delay(int Millisecond)
        {
            DateTime current = DateTime.Now;
            while (current.AddMilliseconds(Millisecond) > DateTime.Now)
            {
                Application.DoEvents();//转让控制权            
            }
            return;
        }

        public Bitmap DrawBitmap()
        {
            return DrawBitmap(this.ImgWidth, this.ImgHeight);
        }

        public Bitmap DrawBitmap(int twidth, int theight)
        {
            Bitmap myBitmap = new Bitmap(this.Width, this.Height);
            Rectangle DrawRect = new Rectangle(0, 0, this.Width, this.Height);
            MyBrowser.DrawToBitmap(myBitmap, DrawRect);
            System.Drawing.Image imgOutput = myBitmap;
            System.Drawing.Bitmap oThumbNail = new Bitmap(twidth, theight, imgOutput.PixelFormat);
            Graphics g = Graphics.FromImage(oThumbNail);

            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;

            Rectangle oRectangle = new Rectangle(0, 0, twidth, theight);
            g.DrawImage(imgOutput, oRectangle);

            try
            {
                return oThumbNail;
            }
            catch
            {
                return null;
            }
            finally
            {
                imgOutput.Dispose();
                imgOutput = null;
                MyBrowser.Url = null;
                MyBrowser.Dispose();
                MyBrowser = null;
            }

        }
    }
}
