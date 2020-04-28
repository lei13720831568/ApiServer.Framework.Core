using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace ApiServer.Framework.Core.Vaild
{
    public class CaptchaFactory:IDependency
    {
        public ICaptchaStore CaptchaStore { get; set; }
        /// <summary>
        /// 绘制干扰线
        /// </summary>
        /// <param name="random"></param>
        /// <param name="bitmap"></param>
        /// <param name="graphics"></param>
        /// <param name="lineCount"></param>
        /// <param name="pointCount"></param>
        public static void Disturb(Random random, Bitmap bitmap, Graphics graphics, int lineCount, int pointCount)
        {

            var colors = new List<Color>
            {
                Color.AliceBlue,
                Color.Azure,
                Color.CadetBlue,
                Color.Beige,
                Color.Chartreuse
            };

            //干扰线
            for (var i = 0; i < lineCount; i++)
            {
                var x1 = random.Next(bitmap.Width);
                var x2 = random.Next(bitmap.Width);
                var y1 = random.Next(bitmap.Height);
                var y2 = random.Next(bitmap.Height);

                //Pen 类 定义用于绘制直线和曲线的对象。
                var pen = new Pen(colors[random.Next(0, colors.Count - 1)]);

                graphics.DrawLine(pen, x1, y1, x2, y2);
            }

            //干扰点
            for (var i = 0; i < pointCount; i++)
            {
                var x = random.Next(bitmap.Width);
                var y = random.Next(bitmap.Height);
                bitmap.SetPixel(x, y, Color.FromArgb(random.Next()));
            }
        }

        private List<char> _characters;

        public CaptchaFactory()
        {
            //排除的内容
            var exts = new List<char> { '0', '1', 'l', 'L', 'o', 'O' };

            _characters = new List<char>();
            for (var c = '0'; c <= '9'; c++)
            {
                if (exts.Contains(c))
                {
                    continue;
                }
                _characters.Add(c);
            }
            for (var c = 'a'; c < 'z'; c++)
            {
                if (exts.Contains(c))
                {
                    continue;
                }
                _characters.Add(c);
            }
        }

        public (Captcha,byte[]) Create(int charCount = 4, int width = 85, int height = 40)
        {
            var chars = new char[charCount];
            var len = _characters.Count;
            var random = new Random();
            for (var i = 0; i < chars.Length; i++)
            {
                var r = random.Next(len);
                chars[i] = _characters[r];
            }

            var fontNames = new List<string>
            {
                "Helvetica","Arial","Lucida Family","Verdana","Tahoma","Trebuchet MS","Georgia","Times"
            };

            //Bitmap 类 封装 GDI+ 包含图形图像和其属性的像素数据的位图。 一个 Bitmap 是用来处理图像像素数据所定义的对象。
            //Bitmap 类 继承自 抽象基类 Image 类
            using (var bitmap = new Bitmap(width, height))
            {
                //Graphics 类 封装一个 GDI+ 绘图图面。
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    //填充背景色 白色
                    graphics.Clear(Color.White);

                    //绘制干扰线和干扰点
                    Disturb(random, bitmap, graphics, width / 2, height);

                    //添加灰色边框
                    var pen = new Pen(Color.Silver);
                    graphics.DrawRectangle(pen, 0, 0, width - 1, height - 1);

                    var x = 1;
                    const int y = 5;

                    var rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

                    var color = Color.FromArgb(random.Next(100, 122), random.Next(100, 122), random.Next(100, 122));

                    foreach (var c in chars)
                    {
                        //随机选择字符 字体样式和大小
                        var fontName = fontNames[random.Next(0, fontNames.Count - 1)];
                        var font = new Font(fontName, random.Next(15, 20));
                        //淡化字符颜色 
                        using (var brush = new LinearGradientBrush(rectangle, color, color, 90f, true))
                        {
                            brush.SetSigmaBellShape(0.5f);
                            graphics.DrawString(c.ToString(), font, brush, x + random.Next(-2, 2), y + random.Next(-5, 5));
                            x = x + width / charCount;
                        }
                    }

                    using (var memoryStream = new MemoryStream())
                    {;
                        bitmap.Save(memoryStream, ImageFormat.Png);

                        var byteArray = memoryStream.ToArray();
                        var captcha=new Captcha
                        {
                            Id = System.Guid.NewGuid().ToString("N"),
                            Code = new string(chars),
                            LimitKey = "",
                            Type = "image",BizId = ""
                        };
                        CaptchaStore.Store(captcha, 180);
                        return (captcha, byteArray);
                    }
                }
            }
        }


        public bool VaildCaptcha(string id, string input) {
            (var result,_)= CaptchaStore.Vaild(id, "image", input);
            return result;
        }

    }

}
