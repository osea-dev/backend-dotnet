using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Drawing.Processing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace WeTalk.Common.Helper
{
    public static class PicCoreHelper
    {
        /// <summary>
        /// 生成分享图
        /// </summary>
        /// <param name="templatePath">原图路径</param>
        /// <param name="logoPath">圆形LOGO图</param>
        /// <param name="position">位置</param>
        /// <param name="logoWidth">水印宽度</param>
        /// <param name="cornerRadius">圆角角度</param>
        /// <returns></returns>
        public static Image ShareImg(String templatePath, String logoPath, Point position, int logoWidth, int cornerRadius)
        {

            var outputImage = Image.Load(templatePath);
            if (outputImage.Size().Width != 1080)
            {
                outputImage.Mutate(x => x.Resize(1080, 1920));
            }

            var logo = Image.Load(logoPath);

            logo.Mutate(x => x.ConvertToAvatar(new Size(logoWidth, logoWidth), cornerRadius));
            outputImage.Mutate(x => x.DrawImage(logo, position, 1));
            return outputImage;
        }

        public static void ShareQrCode(byte[] templateImg, String logoPath,string filename)
        {
            var outputImage = Image.Load(templateImg);
            //if (outputImage.Size().Width != 1080)
            //{
            //    outputImage.Mutate(x => x.Resize(1080, 1920));
            //}
            Image logo;
            var pngEncoder = new SixLabors.ImageSharp.Formats.Png.PngEncoder();
            pngEncoder.ColorType = SixLabors.ImageSharp.Formats.Png.PngColorType.Rgb;
            if (!string.IsNullOrEmpty(logoPath))
            {
                if (logoPath.ToLower().StartsWith("http"))
                {
                    var logoBytes = DownPic(logoPath);
                    logo = Image.Load<Rgba32>(logoBytes); //注意,需使用Rgba32，否则图片如果是JPG时，栽切后会出现黑底
                }
                else
                {
                    logo = Image.Load<Rgba32>(logoPath);
                }
                int owidth = (int)(outputImage.Width / 2.2);
                int oheight = (int)(outputImage.Height / 2.2);
                Point position = new Point((outputImage.Width - owidth) / 2, (outputImage.Height - oheight) / 2);
                logo.Mutate(x => x.ConvertToAvatar(new Size(owidth, oheight), owidth / 2));
                outputImage.Mutate(x => x.DrawImage(logo, position, 1));
            }
            Stream stream = new System.IO.FileStream(filename, FileMode.Create);
            outputImage.SaveAsPng(stream, pngEncoder);
            stream.Dispose();
        }

        public static Image ShareImg(String templatePath, byte[] logoByteData, Point position, int logoWidth, int cornerRadius)
        {

            var outputImage = Image.Load(templatePath);

            if (outputImage.Size().Width != 1080)
            {
                outputImage.Mutate(x => x.Resize(1080, 1920));
            }

            var logo = Image.Load(logoByteData);

            logo.Mutate(x => x.ConvertToAvatar(new Size(logoWidth, logoWidth), cornerRadius));
            outputImage.Mutate(x => x.DrawImage(logo, position, 1));
            return outputImage;
        }


        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="url">图片地址</param>
        public static byte[] DownPic(string url)
        {
            WebRequest wreq = WebRequest.Create(url);
            HttpWebResponse wresp = (HttpWebResponse)wreq.GetResponse();
            Stream s = wresp.GetResponseStream();
            return StreamToBytes(s);
        }

        ///将数据流转为byte[]  
        private static byte[] StreamToBytes(Stream stream)
        {
            List<byte> bytes = new List<byte>();
            int temp = stream.ReadByte();
            while (temp != -1)
            {
                bytes.Add((byte)temp);
                temp = stream.ReadByte();
            }
            return bytes.ToArray();
        }

        // Implements a full image mutating pipeline operating on IImageProcessingContext
        private static IImageProcessingContext ConvertToAvatar(this IImageProcessingContext processingContext, Size size, float cornerRadius)
        {
            return processingContext.Resize(new ResizeOptions
            {
                Size = size,
                Mode = ResizeMode.Crop
            }).ApplyRoundedCorners(cornerRadius);
        }

        // This method can be seen as an inline implementation of an `IImageProcessor`:
        // (The combination of `IImageOperations.Apply()` + this could be replaced with an `IImageProcessor`)
        private static IImageProcessingContext ApplyRoundedCorners(this IImageProcessingContext ctx, float cornerRadius)
        {
            Size size = ctx.GetCurrentSize();
            IPathCollection corners = BuildCorners(size.Width, size.Height, cornerRadius);
            ctx.SetGraphicsOptions(new GraphicsOptions()
            {
                Antialias = true,
                AlphaCompositionMode = PixelAlphaCompositionMode.DestOut // enforces that any part of this shape that has color is punched out of the background
            });

			// mutating in here as we already have a cloned original
			// use any color (not Transparent), so the corners will be clipped
			foreach (var c in corners)
			{
				ctx = ctx.Fill(Color.Red, c);
			}
			return ctx;
        }

        private static IPathCollection BuildCorners(int imageWidth, int imageHeight, float cornerRadius)
        {
            // first create a square
            var rect = new RectangularPolygon(-0.5f, -0.5f, cornerRadius, cornerRadius);

            // then cut out of the square a circle so we are left with a corner
            IPath cornerTopLeft = rect.Clip(new EllipsePolygon(cornerRadius - 0.5f, cornerRadius - 0.5f, cornerRadius));

            // corner is now a corner shape positions top left
            //lets make 3 more positioned correctly, we can do that by translating the original around the center of the image

            float rightPos = imageWidth - cornerTopLeft.Bounds.Width + 1;
            float bottomPos = imageHeight - cornerTopLeft.Bounds.Height + 1;

            // move it across the width of the image - the width of the shape
            IPath cornerTopRight = cornerTopLeft.RotateDegree(90).Translate(rightPos, 0);
            IPath cornerBottomLeft = cornerTopLeft.RotateDegree(-90).Translate(0, bottomPos);
            IPath cornerBottomRight = cornerTopLeft.RotateDegree(180).Translate(rightPos, bottomPos);

            return new PathCollection(cornerTopLeft, cornerBottomLeft, cornerTopRight, cornerBottomRight);
        }
    }

}