/**********************************************************************************
 * 
 * 功能说明:图片操作类
 * 版本:V0.1(C#2.0);时间:2009-04-29
 * *******************************************************************************/
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text.RegularExpressions;

namespace WeTalk.Common.Helper
{
	public class PicHelper
	{
		#region "图片加水印"

		public static string WebPath { get; set; }

		/// <summary> 
		/// 生成缩略图 
		/// </summary> 
		/// <param name="originalImagePath">源图路径</param> 
		/// <param name="thumbnailPath">缩略图路径</param> 
		/// <param name="width">缩略图宽度</param> 
		/// <param name="height">缩略图高度</param> 
		/// <param name="mode">生成缩略图的方式</param> 
		/// <param name="flag">压缩质量1-100</param> 
		/// <param name="processing">加工处理方式：底片:Negative,黑白:Gray,浮雕:Embossment,柔化:Soften，锐化:Sharpen,雾化:Atomization</param> 
		/// <returns>原图大小</returns>
		public static decimal[] MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode, int flag = 100, string processing = "", double v = 1)
		{
			originalImagePath = WebPath + originalImagePath;
			thumbnailPath = WebPath + thumbnailPath;
			decimal[] wh = { 0.00M, 0.00M };
			System.Drawing.Image originalImage = System.Drawing.Image.FromFile(originalImagePath);
			wh[0] = originalImage.Width;
			wh[1] = originalImage.Height;
			ImageFormat imageType = ImageFormat.Jpeg;
			switch (System.IO.Path.GetExtension(originalImagePath).ToLower())
			{
				case ".jpg":
					imageType = ImageFormat.Jpeg;
					break;
				case ".gif":
					imageType = ImageFormat.Gif;
					break;
				case ".png":
					imageType = ImageFormat.Png;
					break;
				case ".bmp":
					imageType = ImageFormat.Bmp;
					break;
				case ".tif":
					imageType = ImageFormat.Tiff;
					break;
				case ".wmf":
					imageType = ImageFormat.Wmf;
					break;
				case ".ico":
					imageType = ImageFormat.Icon;
					break;
				default:
					break;
			}

			int towidth = width;
			int toheight = height;

			int x = 0;
			int y = 0;
			int ow = originalImage.Width;
			int oh = originalImage.Height;
			switch (mode.ToUpper())
			{
				//指定高宽缩放（可能变形）                
				case "HW":

					break;
				case "W":
					//指定宽，高按比例
					if (originalImage.Width < width) return wh;
					toheight = originalImage.Height * width / originalImage.Width;
					break;
				case "H":
					//指定高，宽按比例 
					if (originalImage.Height < height) return wh;
					towidth = originalImage.Width * height / originalImage.Height;
					break;
				case "Cut":
					//指定高宽裁减（不变形）                
					if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
					{
						oh = originalImage.Height;
						ow = originalImage.Height * towidth / toheight;
						y = 0;
						x = (originalImage.Width - ow) / 2;
					}
					else
					{
						ow = originalImage.Width;
						oh = originalImage.Width * height / towidth;
						x = 0;
						y = (originalImage.Height - oh) / 2;
					}
					break;
				default: break;
			}

			//新建一个bmp图片 
			System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(towidth, toheight);

			//新建一个画板 
			System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

			//设置高质量插值法 
			g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

			//设置高质量,低速度呈现平滑程度 
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;

			//清空画布并以透明背景色填充 
			g.Clear(System.Drawing.Color.Transparent);

			//在指定位置并且按指定大小绘制原图片的指定部分 
			g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight), new System.Drawing.Rectangle(x, y, ow, oh), System.Drawing.GraphicsUnit.Pixel);

			//以下代码为保存图片时，设置压缩质量
			EncoderParameters ep = new EncoderParameters();
			long[] qy = new long[1];
			qy[0] = flag;//设置压缩的比例1-100
			EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
			ep.Param[0] = eParam;

			ImageCodecInfo jpegICIinfo = null;
			if (flag > 0)
			{
				ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
				jpegICIinfo = arrayICI.FirstOrDefault(t => t.FormatDescription.Equals("JPEG"));
			}
			//底片:Negative,黑白:Gray,浮雕:Embossment,柔化:Soften，锐化:Sharpen,雾化:Atomization
			switch (processing.ToLower())
			{
				case "negative":
					bitmap = NegativeImage(bitmap);
					break;
				case "gray":
					bitmap = GrayImage(bitmap);
					break;
				case "soften":
					bitmap = SoftenImage(bitmap);
					break;
				case "embossment":
					bitmap = EmbossmentImage(bitmap);
					break;
				case "sharpen":
					bitmap = SharpenImage(bitmap, v);
					break;
				case "atomization":
					bitmap = AtomizationImage(bitmap);
					break;
				default:

					break;
			}

			try
			{
				if (jpegICIinfo != null)
				{
					bitmap.Save(thumbnailPath, jpegICIinfo, ep);//dFile是压缩后的新路径
				}
				else
				{
					bitmap.Save(thumbnailPath, originalImage.RawFormat);
				}
			}
			catch (Exception e)
			{
				throw e;
			}
			finally
			{
				originalImage.Dispose();
				bitmap.Dispose();
				g.Dispose();
			}


			//try
			//{
			//    bitmap.Save(thumbnailPath, imageType);
			//}
			//catch (System.Exception e)
			//{
			//    throw e;
			//}
			//finally
			//{
			//    originalImage.Dispose();
			//    bitmap.Dispose();
			//    g.Dispose();
			//}
			return wh;
		}
		/// <summary>
		/// 根据图片地址无损压缩图片
		/// </summary>
		/// <param name="sFile">原图片地址</param>
		/// <param name="dFile">压缩后保存图片地址</param>
		/// <param name="compressionRatio">压缩质量（数字越小压缩率越高）1-100</param>
		/// <param name="isNarrowResolution">是否缩小分辨率</param>
		/// <returns></returns>
		public static bool ImageCompress(string sFile, string dFile, out int dHeight, out int dWidth, int compressionRatio = 90, bool isNarrowResolution = false)
		{
			Image iSource = Image.FromFile(sFile);
			ImageFormat tFormat = iSource.RawFormat;
			dHeight = 0;
			dWidth = 0;
			if (isNarrowResolution)
			{
				dHeight = iSource.Height / 2;
				dWidth = iSource.Width / 2;
			}
			else
			{
				dHeight = iSource.Height;
				dWidth = iSource.Width;
			}
			int sW, sH;
			//等比例缩放
			Size tem_size = new Size(iSource.Width, iSource.Height);
			if (tem_size.Width > dHeight || tem_size.Width > dWidth)
			{
				if ((tem_size.Width * dHeight) > (tem_size.Width * dWidth))
				{
					sW = dWidth;
					sH = (dWidth * tem_size.Height) / tem_size.Width;
				}
				else
				{
					sH = dHeight;
					sW = (tem_size.Width * dHeight) / tem_size.Height;
				}
			}
			else
			{
				sW = tem_size.Width;
				sH = tem_size.Height;
			}
			//创建点阵图（或位图）
			Bitmap ob = new Bitmap(dWidth, dHeight);
			Graphics g = Graphics.FromImage(ob);
			//设置质量
			g.Clear(Color.WhiteSmoke);
			g.CompositingQuality = CompositingQuality.HighQuality;
			g.SmoothingMode = SmoothingMode.HighQuality;
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			//绘图
			g.DrawImage(iSource, new Rectangle((dWidth - sW) / 2, (dHeight - sH) / 2, sW, sH), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);
			g.Dispose();
			//以下代码为保存图片时，设置压缩质量
			EncoderParameters ep = new EncoderParameters();
			long[] qy = new long[1];
			qy[0] = compressionRatio;//设置压缩的比例1-100
			EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
			ep.Param[0] = eParam;

			try
			{
				ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
				ImageCodecInfo jpegICIinfo = arrayICI.FirstOrDefault(t => t.FormatDescription.Equals("JPEG"));
				if (jpegICIinfo != null)
				{
					ob.Save(dFile, jpegICIinfo, ep);//dFile是压缩后的新路径
				}
				else
				{
					ob.Save(dFile, tFormat);
				}
				return true;
			}
			catch
			{
				return false;
			}
			finally
			{
				iSource.Dispose();
				ob.Dispose();
			}
		}

		//
		/// <summary> 
		/// 在图片上增加文字水印 
		/// </summary> 
		/// <param name="Path">原服务器图片路径</param> 
		/// <param name="Path_sy">生成的带文字水印的图片路径</param> 
		public static void AddWater(string Path, string Path_sy)
		{
			string addText = "优艺客";
			System.Drawing.Image image = System.Drawing.Image.FromFile(Path);
			System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image);
			g.DrawImage(image, 0, 0, image.Width, image.Height);
			System.Drawing.Font f = new System.Drawing.Font("Verdana", 60);
			System.Drawing.SolidBrush b = new System.Drawing.SolidBrush(System.Drawing.Color.Green);

			g.DrawString(addText, f, b, 35, 35);
			g.Dispose();

			image.Save(Path_sy);
			image.Dispose();
		}

		//
		/// <summary> 
		/// 在图片上生成图片水印 
		/// </summary> 
		/// <param name="Path">原服务器图片路径</param> 
		/// <param name="Path_syp">生成的带图片水印的图片路径</param> 
		/// <param name="Path_sypf">水印图片路径</param> 
		public static void AddWaterPic(string Path, string Path_syp, string Path_sypf)
		{
			//HttpContext.Current.Response.Write(Path)
			//HttpContext.Current.Response.End()
			System.Drawing.Image image = System.Drawing.Image.FromFile(Path);
			System.Drawing.Image copyImage = System.Drawing.Image.FromFile(Path_sypf);
			System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image);
			g.DrawImage(copyImage, new System.Drawing.Rectangle(image.Width - copyImage.Width, image.Height - copyImage.Height, copyImage.Width, copyImage.Height), 0, 0, copyImage.Width, copyImage.Height, System.Drawing.GraphicsUnit.Pixel);
			g.Dispose();

			image.Save(Path_syp);
			image.Dispose();
		}

		/// <summary>
		/// 在图片上生成图片水印
		/// </summary>
		/// <param name="bytes">原图流</param>
		/// <param name="Path_syp">生成的带图片水印的图片路径</param> 
		/// <param name="Path_sypf">水印图片路径，要么远程，要么物理</param> 
		public static void AddWaterPic(byte[] bytes, string Path_syp, string Path_sypf)
		{
			System.Drawing.Image copyImage;
			if (Path_sypf.StartsWith("http"))
			{
				//远程下载
				WebRequest wreq = WebRequest.Create(Path_sypf);
				HttpWebResponse wresp = (HttpWebResponse)wreq.GetResponse();
				Stream s = wresp.GetResponseStream();
				copyImage = System.Drawing.Image.FromStream(s);
			}
			else
			{
				copyImage = System.Drawing.Image.FromFile(Path_sypf);
			}
			


			MemoryStream ms = new MemoryStream(bytes);
			System.Drawing.Image image = System.Drawing.Image.FromStream(ms);//原图
			int owidth = (int)(image.Width / 2.2);
			int oheight = (int)(image.Height / 2.2);
			System.Drawing.Image.GetThumbnailImageAbort myCallback = new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback);
			System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image);
			Rectangle rec = new System.Drawing.Rectangle((image.Width - owidth) / 2, (image.Height - oheight) / 2, owidth, oheight);
			var size = new Size(owidth, oheight);
			Bitmap myThumbnail = new Bitmap(size.Width, size.Height);
			using (Graphics g1 = Graphics.FromImage(myThumbnail))
			{
				using (TextureBrush br = new TextureBrush(copyImage, System.Drawing.Drawing2D.WrapMode.Clamp, rec))
				{
					br.ScaleTransform(myThumbnail.Width / (float)rec.Width, myThumbnail.Height / (float)rec.Height);
					g1.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
					g1.FillEllipse(br, new Rectangle(Point.Empty, size));
					myThumbnail.Save("E:\\SVN\\ue_card\\build\\WeTalk.Web\\wwwroot\\Upfile\\Card\\Qrcode\\2022-02-14\\123.png");
					g1.Dispose();


					//var logo = ImageSharp.Image.Load("Image/Logo.png");
					//logo.Mutate(x => x.ConvertToAvatar(new Size(logoWidth, logoWidth), logoWidth / 2));
				}
			}
			//int CornerRadius = 180; //角度
			//using (Graphics g1 = Graphics.FromImage(myThumbnail))
			//{
			//	//g1.Clear(Color.White);
			//	g1.SmoothingMode = SmoothingMode.AntiAlias;
			//	Brush brush = new TextureBrush(copyImage);
			//	GraphicsPath gp = new GraphicsPath();
			//	gp.AddArc(0, 0, CornerRadius, CornerRadius, 180, 90);
			//	gp.AddArc(0 + myThumbnail.Width - CornerRadius, 0, CornerRadius, CornerRadius, 270, 90);
			//	gp.AddArc(0 + myThumbnail.Width - CornerRadius, 0 + myThumbnail.Height - CornerRadius, CornerRadius, CornerRadius, 0, 90);
			//	gp.AddArc(0, 0 + myThumbnail.Height - CornerRadius, CornerRadius, CornerRadius, 90, 90);
			//	g1.FillPath(brush, gp);
			//	//myThumbnail.Save(new_FileName, ImageFormat.Png);
			//	myThumbnail.Save("E:\\SVN\\ue_card\\build\\WeTalk.Web\\wwwroot\\Upfile\\Card\\Qrcode\\2022-02-14\\123.png");
			//}

			g.DrawImage(myThumbnail, rec , 0, 0, myThumbnail.Width, myThumbnail.Height, System.Drawing.GraphicsUnit.Pixel);
			g.Dispose();
			image.Save(Path_syp);
			image.Dispose();

		}


		/// <summary> 
		/// 给图片上水印 
		/// </summary> 
		/// <param name="filePath">原图片物理地址</param> 
		/// <param name="waterFile">水印图片绝对地址</param> 
		public static void MarkWater(string filePaths, string waterFile)
		{
			//GIF不水印 
			//Dim FilePath As String = ConfigurationManager.AppSettings("UserFilesPath")

			int i = filePaths.LastIndexOf(".");
			string ex = filePaths.Substring(i, filePaths.Length - i);
			if (string.Compare(ex, ".gif", true) == 0)
			{
				return;
			}

			string ModifyImagePath = filePaths;
			//要Modify的图像路径 
			int lucencyPercent = 25;
			Image modifyImage = null;
			Image drawedImage = null;
			Graphics g = null;
			try
			{
				//建立图形对象 
				modifyImage = Image.FromFile(ModifyImagePath, true);
				drawedImage = Image.FromFile(waterFile, true);
				g = Graphics.FromImage(modifyImage);
				//获取要绘制图形坐标 
				int x = modifyImage.Width - drawedImage.Width;
				int y = modifyImage.Height - drawedImage.Height;
				//设置颜色矩阵 
				float[][] matrixItems = { new float[] { 1, 0, 0, 0, 0 }, new float[] { 0, 1, 0, 0, 0 }, new float[] { 0, 0, 1, 0, 0 }, new float[] { 0, 0, 0, (float)lucencyPercent / 1f, 0 }, new float[] { 0, 0, 0, 0, 1 } };

				ColorMatrix colorMatrix = new ColorMatrix(matrixItems);
				ImageAttributes imgAttr = new ImageAttributes();
				imgAttr.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
				//绘制阴影图像 
				g.DrawImage(drawedImage, new Rectangle(x, y, drawedImage.Width, drawedImage.Height), 0, 0, drawedImage.Width, drawedImage.Height, GraphicsUnit.Pixel, imgAttr);
				//保存文件 
				string[] allowImageType = { ".jpg", ".gif", ".png", ".bmp", ".tiff", ".wmf", ".ico" };
				FileInfo fi = new FileInfo(ModifyImagePath);
				ImageFormat imageType = ImageFormat.Gif;
				switch (fi.Extension.ToLower())
				{
					case ".jpg":
						imageType = ImageFormat.Jpeg;
						break;
					case ".gif":
						imageType = ImageFormat.Gif;
						break;
					case ".png":
						imageType = ImageFormat.Png;
						break;
					case ".bmp":
						imageType = ImageFormat.Bmp;
						break;
					case ".tif":
						imageType = ImageFormat.Tiff;
						break;
					case ".wmf":
						imageType = ImageFormat.Wmf;
						break;
					case ".ico":
						imageType = ImageFormat.Icon;
						break;
					default:
						break;
				}
				MemoryStream ms = new MemoryStream();
				modifyImage.Save(ms, imageType);
				byte[] imgData = ms.ToArray();
				modifyImage.Dispose();
				drawedImage.Dispose();
				g.Dispose();
				FileStream fs = null;
				File.Delete(ModifyImagePath);
				fs = new FileStream(ModifyImagePath, FileMode.Create, FileAccess.Write);
				if (!(fs == null))
				{
					fs.Write(imgData, 0, imgData.Length);
					fs.Close();
				}
			}
			finally
			{
				try
				{
					drawedImage.Dispose();
					modifyImage.Dispose();
					g.Dispose();
				}
				catch
				{
				}
			}
		}

		#endregion

		//#region "判断文件是否图片"

		////
		///// <summary> 
		///// 判断是否图片 
		///// </summary> 
		///// <param name="upfile">上传控件名</param>  
		////
		//public static bool  ExistsPic(System.Web.UI.WebControls.FileUpload upfile)
		//{
		//    string filetype=upfile.PostedFile.ContentType;
		//    if (filetype.Substring(0, 6) == "image/")
		//    {
		//        return true;
		//    }
		//    else {
		//        return false;
		//    }
		//}
		///// <summary> 

		//#endregion

		#region "远程保存图片"
		//远程保存图片
		/// <summary> 
		/// 远程数据内容【包含图片】 
		/// </summary> 
		/// <param name="content"></param> 
		/// <param name="waterFile">水印文件路径地址</param> 
		/// <returns></returns> 

		public static string FormatContentPic(string content, string waterFile)
		{
			//自动保存远程图片 
			//Dim FilePath As String = ConfigurationManager.AppSettings("UserFilesPath")
			WebClient client = new WebClient();
			//备用Reg:<img.*?src=([\"\'])(http:\/\/.+\.(jpg|gif|bmp|bnp))\1.*?> 
			Regex reg = new Regex("IMG[^>]*?src\\s*=\\s*(?:\"(?<1>[^\"]*)\"|'(?<1>[^']*)')", RegexOptions.IgnoreCase);
			MatchCollection m = reg.Matches(content);


			if (waterFile.Trim() != "") waterFile = WebPath + waterFile;

			foreach (Match math in m)
			{
				string imgUrl = math.Groups[1].Value;

				//取图
				Regex regName = new Regex("\\w+.(?:jpg|gif|bmp|png)", RegexOptions.IgnoreCase);

				//判断是否是已加水印的图
				if (regName.Match(imgUrl).ToString().Substring(0, 3) != "SY_")
				{
					//在原图片名称前加YYMMDD重名名并上传 
					string strNewImgName = "SY_" + DateTime.Now.ToString("yyyymmddhhmmss") + regName.Match(imgUrl).ToString();
					//

					if (strNewImgName.LastIndexOf(".") == -1)
					{
						strNewImgName += ".gif";
					}
					//Try
					//保存图片 
					client.DownloadFile(WebPath + imgUrl, WebPath + imgUrl.Substring(0, imgUrl.LastIndexOf("/")) + strNewImgName);
					if (waterFile.Trim() != "") MarkWater(imgUrl.Substring(0, imgUrl.LastIndexOf("/")) + strNewImgName, waterFile);

					content = content.Replace(imgUrl.Substring(imgUrl.LastIndexOf("/"), imgUrl.Length - 1), strNewImgName);

					//Catch
					//    HttpContext.Current.Response.Write(strNewImgName)
					//    HttpContext.Current.Response.End()
					//Finally
					//End Try
					client.Dispose();
					FileHelper.FileDel(imgUrl.ToString());
				}
			}
			return content;
		}

		/// <summary>
		/// 执行下载并保存到本地
		/// </summary>
		/// <param name="url">图片地址</param>
		/// <param name="dtnow">保存的物理文件名</param>
		public static void DownPic(string url, string dtnow)
		{
			WebRequest wreq = WebRequest.Create(url);
			HttpWebResponse wresp = (HttpWebResponse)wreq.GetResponse();
			Stream s = wresp.GetResponseStream();
			System.Drawing.Image img;
			img = System.Drawing.Image.FromStream(s);
			img.Save(dtnow, ImageFormat.Jpeg);
			MemoryStream ms = new MemoryStream();
			img.Save(ms, ImageFormat.Jpeg);
			img.Dispose();
		}
		#endregion

		#region Base64转图片

		/// <summary>
		/// Base64转图片
		/// </summary>
		/// <param name="filebase64"></param>
		/// <param name="fileurl">完整的物理路径</param>
		/// <returns>数组：filename,width,height,size(kb)</returns>
		public static string[] Base64ToImage(string filebase64, string fileurl)
		{   
			string[] arr = { "", "", "", "" };//filename,width,height,size
			string imgformat = filebase64.Substring(0, filebase64.IndexOf(','));
			string filename = "";
			filebase64 = filebase64.Replace(" ", "+").Substring(filebase64.IndexOf(',') + 1);
			byte[] imageBytes = Convert.FromBase64String(filebase64);
			using (MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
			{
				ms.Write(imageBytes, 0, imageBytes.Length);
				var img = Image.FromStream(ms, true);
				switch (imgformat.ToLower())
				{
					case "data:image/jpg;base64":
						filename =  RandomHelper.GetMd5Code() + DateTime.Now.Ticks + ".jpg";
						img.Save(fileurl + filename, System.Drawing.Imaging.ImageFormat.Jpeg);
						break;
					case "data:image/jpeg;base64":
						filename = RandomHelper.GetMd5Code() + DateTime.Now.Ticks + ".jpeg";
						img.Save(fileurl + filename, System.Drawing.Imaging.ImageFormat.Jpeg);
						break;
					case "data:image/png;base64":
						filename = RandomHelper.GetMd5Code() + DateTime.Now.Ticks + ".png";
						img.Save(fileurl + filename, System.Drawing.Imaging.ImageFormat.Png);
						break;
					case "data:image/gif;base64":
						filename = RandomHelper.GetMd5Code() + DateTime.Now.Ticks + ".gif";
						img.Save(fileurl + filename, System.Drawing.Imaging.ImageFormat.Gif);
						break;
					case "data:image/tiff;base64":
						filename = RandomHelper.GetMd5Code() + DateTime.Now.Ticks + ".tiff";
						img.Save(fileurl + filename, System.Drawing.Imaging.ImageFormat.Tiff);
						break;
					default:
						return arr;
				}
				arr[1] = img.Width.ToString();
				arr[2] = img.Height.ToString();
				arr[3] = (imageBytes.Length / 1024).ToString();//KB
				arr[0] = filename;
			}
			return arr;
		}
		public static string[] Base64ToImage1(string filebase64, string fileurl)
		{
			string[] arr = { "", "", "", "" };//filename,width,height,size
			string imgformat = filebase64.Substring(0, filebase64.IndexOf(','));
			string filename = "";
			filebase64 = filebase64.Substring(filebase64.IndexOf(',') + 1);
			byte[] bytes = Convert.FromBase64String(filebase64);
			using (MemoryStream ms2 = new MemoryStream(bytes))
			{
				System.Drawing.Bitmap bmp2 = new System.Drawing.Bitmap(ms2);
				arr[1] = bmp2.Width.ToString();
				arr[2] = bmp2.Height.ToString();
				arr[3] = (bytes.Length / 1024).ToString();//KB
				switch (imgformat.ToLower())
				{
					case "data:image/jpg;base64":
						filename = filename =RandomHelper.GetMd5Code() + DateTime.Now.Ticks + ".jpg";
						bmp2.Save(fileurl + filename, System.Drawing.Imaging.ImageFormat.Jpeg);
						break;
					case "data:image/jpeg;base64":
						filename = filename =RandomHelper.GetMd5Code() + DateTime.Now.Ticks + ".jpeg";
						bmp2.Save(fileurl + filename, System.Drawing.Imaging.ImageFormat.Jpeg);
						break;
					case "data:image/png;base64":
						filename = filename =RandomHelper.GetMd5Code() + DateTime.Now.Ticks + ".png";
						bmp2.Save(fileurl + filename, System.Drawing.Imaging.ImageFormat.Png);
						break;
					case "data:image/gif;base64":
						filename = filename =RandomHelper.GetMd5Code() + DateTime.Now.Ticks + ".gif";
						bmp2.Save(fileurl + filename, System.Drawing.Imaging.ImageFormat.Gif);
						break;
					case "data:image/tiff;base64":
						filename = filename =RandomHelper.GetMd5Code() + DateTime.Now.Ticks + ".tiff";
						bmp2.Save(fileurl + filename, System.Drawing.Imaging.ImageFormat.Tiff);
						break;
					default:

						break;
				}
				bmp2.Dispose();
			}
			arr[0] = filename;
			return arr;
		}

		/// <summary>
		/// Image 转成 base64
		/// </summary>
		/// <param name="fileFullName">物理路径或网络路径</param>
		public static string ImageToBase64(string fileFullName)
		{
			try
            {
                if (fileFullName.StartsWith("http"))
                {
                    WebClient mywebclient = new WebClient();
                    byte[] Bytes = mywebclient.DownloadData(fileFullName);
                    using (MemoryStream ms = new MemoryStream(Bytes))
                    {
                        byte[] arr = new byte[ms.Length];
                        ms.Position = 0;
                        ms.Read(arr, 0, (int)ms.Length);
                        ms.Close();
                        return "data:image/png;base64," + Convert.ToBase64String(arr);
                    }
                }
                else
                {
                    Bitmap bmp = new Bitmap(fileFullName);
                    MemoryStream ms = new MemoryStream();
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    byte[] arr = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(arr, 0, (int)ms.Length);
                    ms.Close();
                    bmp.Dispose();
                    return "data:image/png;base64," + Convert.ToBase64String(arr);
                }
			}
			catch (Exception ex)
			{
            }
            return "";
        }
		#endregion

		#region 缩略图
		//产生图片缩略图 fixType{0：不固定 1：固定宽度  2：固定高度}
		//当fixType!=0时，若固定高度，则width表示最大宽度值，若固定宽度，则height表示最大高度值，0表示不指定
		/// <summary>
		/// 产生图片缩略图
		/// </summary>
		/// <param name="fileName1">服务器的原图片物理地址</param>
		/// <param name="fileName2">要生成的服务器缩略图物理地址</param>
		/// <param name="width">当fixType!=0时，若固定高度，则width表示最大宽度值</param>
		/// <param name="height">当fixType!=0时，若固定宽度，则height表示最大高度值</param>
		/// <param name="fixType">fixType{0：不固定 1：固定宽度  2：固定高度}</param>
		public static void GetThumbnail(string fileName1, string fileName2, int width, int height, int fixType = 0)
		{
			Hashtable htmimes = new Hashtable();
			htmimes[".gif"] = "image/gif";
			htmimes[".jpeg"] = "image/jpeg";
			htmimes[".jpg"] = "image/jpeg";
			htmimes[".png"] = "image/png";
			htmimes[".tif"] = "image/tiff";
			htmimes[".tiff"] = "image/tiff";
			htmimes[".bmp"] = "image/bmp";

			System.Drawing.Image image = System.Drawing.Image.FromFile(fileName1);
			int owidth = image.Size.Width;
			int oheight = image.Size.Height;

			switch (fixType)
			{
				case 0:
					owidth = width;
					oheight = height;
					break;
				case 1:
					if (owidth <= width)
					{
						//当缩略后的高度超过了最大高度时
						if (height > 0 && oheight > height)
						{
							owidth = Convert.ToInt32((float)owidth * ((float)height / (float)oheight));
							oheight = height;
						}
					}
					else
					{
						oheight = Convert.ToInt32((float)oheight * ((float)width / (float)owidth));
						owidth = width;
						//当缩略后的高度超过了最大高度时
						if (height > 0 && oheight > height)
						{
							owidth = Convert.ToInt32((float)owidth * ((float)height / (float)oheight));
							oheight = height;
						}
					}
					break;
				case 2:
					if (oheight < height)
					{
						//当缩略后的宽度超过了最大宽度时
						if (width > 0 && owidth > width)
						{
							oheight = Convert.ToInt32((float)oheight * ((float)width / (float)owidth));
							owidth = width;
						}
					}
					else
					{

						owidth = Convert.ToInt32((float)owidth * ((float)height / (float)oheight));
						oheight = height;
						//当缩略后的宽度超过了最大宽度时
						if (width > 0 && owidth > width)
						{
							oheight = Convert.ToInt32((float)oheight * ((float)width / (float)owidth));
							owidth = width;
						}
					}
					break;
				default:
					owidth = width;
					oheight = height;
					break;
			}

			string sExt = fileName1.Substring(fileName1.LastIndexOf(".")).ToLower();
			System.Drawing.Image.GetThumbnailImageAbort myCallback = new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback);
			System.Drawing.Image myThumbnail = image.GetThumbnailImage(owidth, oheight, myCallback, IntPtr.Zero);

			if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(fileName2))) System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fileName2));

			System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
			EncoderParameter myEncoderParameter;
			EncoderParameters myEncoderParameters;
			myEncoderParameters = new EncoderParameters(1);
			myEncoderParameter = new EncoderParameter(myEncoder, 80L);
			myEncoderParameters.Param[0] = myEncoderParameter;
			ImageCodecInfo myImageCodecInfo = GetEncoderInfo((string)htmimes[sExt]);

			myThumbnail.Save(fileName2, myImageCodecInfo, myEncoderParameters);
			myThumbnail.Dispose();
			image.Dispose();
		}

		//产生图片缩略图 fixType{0：不固定 1：固定宽度  2：固定高度}
		//当fixType!=0时，若固定高度，则width表示最大宽度值，若固定宽度，则height表示最大高度值，0表示不指定
		/// <summary>
		/// 图片缩略图
		/// </summary>
		/// <param name="imageStream">图片文件流</param>
		/// <param name="sExt">文件扩展名.gif,.jpg</param>
		/// <param name="fileName2">要生成的服务器缩略图物理地址</param>
		/// <param name="width">当fixType!=0时，若固定高度，则width表示最大宽度值</param>
		/// <param name="height">当fixType!=0时，若固定宽度，则height表示最大高度值</param>
		/// <param name="fixType">fixType{0：不固定 1：固定宽度  2：固定高度}</param>
		public static void GetThumbnail(FileStream imageStream,string sExt, string fileName2, int width, int height, int fixType = 0)
		{
			Hashtable htmimes = new Hashtable();
			htmimes[".gif"] = "image/gif";
			htmimes[".jpeg"] = "image/jpeg";
			htmimes[".jpg"] = "image/jpeg";
			htmimes[".png"] = "image/png";
			htmimes[".tif"] = "image/tiff";
			htmimes[".tiff"] = "image/tiff";
			htmimes[".bmp"] = "image/bmp";

			System.Drawing.Image image = System.Drawing.Image.FromStream(imageStream);
			int owidth = image.Size.Width;
			int oheight = image.Size.Height;
			switch (fixType)
			{
				case 0:
					owidth = width;
					oheight = height;
					break;
				case 1:
					if (owidth <= width)
					{
						//当缩略后的高度超过了最大高度时
						if (height > 0 && oheight > height)
						{
							owidth = Convert.ToInt32((float)owidth * ((float)height / (float)oheight));
							oheight = height;
						}
					}
					else
					{
						oheight = Convert.ToInt32((float)oheight * ((float)width / (float)owidth));
						owidth = width;
						//当缩略后的高度超过了最大高度时
						if (height > 0 && oheight > height)
						{
							owidth = Convert.ToInt32((float)owidth * ((float)height / (float)oheight));
							oheight = height;
						}
					}
					break;
				case 2:
					if (oheight < height)
					{
						//当缩略后的宽度超过了最大宽度时
						if (width > 0 && owidth > width)
						{
							oheight = Convert.ToInt32((float)oheight * ((float)width / (float)owidth));
							owidth = width;
						}
					}
					else
					{

						owidth = Convert.ToInt32((float)owidth * ((float)height / (float)oheight));
						oheight = height;
						//当缩略后的宽度超过了最大宽度时
						if (width > 0 && owidth > width)
						{
							oheight = Convert.ToInt32((float)oheight * ((float)width / (float)owidth));
							owidth = width;
						}
					}
					break;
				default:
					owidth = width;
					oheight = height;
					break;
			}
			

			System.Drawing.Image.GetThumbnailImageAbort myCallback = new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback);

			System.Drawing.Image myThumbnail = image.GetThumbnailImage(owidth, oheight, myCallback, IntPtr.Zero);

			if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(fileName2))) System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fileName2));

			System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
			EncoderParameter myEncoderParameter;
			EncoderParameters myEncoderParameters;
			myEncoderParameters = new EncoderParameters(1);
			myEncoderParameter = new EncoderParameter(myEncoder, 80L);
			myEncoderParameters.Param[0] = myEncoderParameter;
			ImageCodecInfo myImageCodecInfo = GetEncoderInfo((string)htmimes[sExt]);

			myThumbnail.Save(fileName2, myImageCodecInfo, myEncoderParameters);
			myThumbnail.Dispose();
			image.Dispose();
		}
		public static void GetThumbnail(Image image, string fileName2, int width, int height, int fixType = 0)
		{
			Hashtable htmimes = new Hashtable();
			htmimes[".gif"] = "image/gif";
			htmimes[".jpeg"] = "image/jpeg";
			htmimes[".jpg"] = "image/jpeg";
			htmimes[".png"] = "image/png";
			htmimes[".tif"] = "image/tiff";
			htmimes[".tiff"] = "image/tiff";
			htmimes[".bmp"] = "image/bmp";
			string sExt = fileName2.Substring(fileName2.LastIndexOf(".")).ToLower();

			int owidth = image.Size.Width;
			int oheight = image.Size.Height;
			switch (fixType)
			{
				case 0:
					owidth = width;
					oheight = height;
					break;
				case 1:
					if (owidth <= width)
					{
						//当缩略后的高度超过了最大高度时
						if (height > 0 && oheight > height)
						{
							owidth = Convert.ToInt32((float)owidth * ((float)height / (float)oheight));
							oheight = height;
						}
					}
					else
					{
						oheight = Convert.ToInt32((float)oheight * ((float)width / (float)owidth));
						owidth = width;
						//当缩略后的高度超过了最大高度时
						if (height > 0 && oheight > height)
						{
							owidth = Convert.ToInt32((float)owidth * ((float)height / (float)oheight));
							oheight = height;
						}
					}
					break;
				case 2:
					if (oheight < height)
					{
						//当缩略后的宽度超过了最大宽度时
						if (width > 0 && owidth > width)
						{
							oheight = Convert.ToInt32((float)oheight * ((float)width / (float)owidth));
							owidth = width;
						}
					}
					else
					{

						owidth = Convert.ToInt32((float)owidth * ((float)height / (float)oheight));
						oheight = height;
						//当缩略后的宽度超过了最大宽度时
						if (width > 0 && owidth > width)
						{
							oheight = Convert.ToInt32((float)oheight * ((float)width / (float)owidth));
							owidth = width;
						}
					}
					break;
				default:
					owidth = width;
					oheight = height;
					break;
			}


			System.Drawing.Image.GetThumbnailImageAbort myCallback = new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback);

			System.Drawing.Image myThumbnail = image.GetThumbnailImage(owidth, oheight, myCallback, IntPtr.Zero);

			if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(fileName2))) System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fileName2));

			System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
			EncoderParameter myEncoderParameter;
			EncoderParameters myEncoderParameters;
			myEncoderParameters = new EncoderParameters(1);
			myEncoderParameter = new EncoderParameter(myEncoder, 80L);
			myEncoderParameters.Param[0] = myEncoderParameter;
			ImageCodecInfo myImageCodecInfo = GetEncoderInfo((string)htmimes[sExt]);

			myThumbnail.Save(fileName2, myImageCodecInfo, myEncoderParameters);
			myThumbnail.Dispose();
			image.Dispose();
		}

		private static bool ThumbnailCallback()
		{
			return false;
		}

		/// <summary>
		/// 获取图像编码解码器的所有相关信息
		/// </summary>
		/// <param name="mimeType">包含编码解码器的多用途网际邮件扩充协议 (MIME) 类型的字符串</param>
		/// <returns>返回图像编码解码器的所有相关信息</returns>
		private static ImageCodecInfo GetEncoderInfo(String mimeType)
		{
			int j;
			ImageCodecInfo[] encoders;
			encoders = ImageCodecInfo.GetImageEncoders();
			for (j = 0; j < encoders.Length; ++j)
			{
				if (encoders[j].MimeType == mimeType)
					return encoders[j];
			}
			return null;
		}

		public static FileStream GetThumbnail(FileStream imageStream,int width, int height, int fixType = 0)
		{
			System.Drawing.Image image = System.Drawing.Image.FromStream(imageStream);
			int owidth = image.Size.Width;
			int oheight = image.Size.Height;
			switch (fixType)
			{
				case 0:
					owidth = width;
					oheight = height;
					break;
				case 1:
					if (owidth <= width)
					{
						//当缩略后的高度超过了最大高度时
						if (height > 0 && oheight > height)
						{
							owidth = Convert.ToInt32((float)owidth * ((float)height / (float)oheight));
							oheight = height;
						}
					}
					else
					{
						oheight = Convert.ToInt32((float)oheight * ((float)width / (float)owidth));
						owidth = width;
						//当缩略后的高度超过了最大高度时
						if (height > 0 && oheight > height)
						{
							owidth = Convert.ToInt32((float)owidth * ((float)height / (float)oheight));
							oheight = height;
						}
					}
					break;
				case 2:
					if (oheight < height)
					{
						//当缩略后的宽度超过了最大宽度时
						if (width > 0 && owidth > width)
						{
							oheight = Convert.ToInt32((float)oheight * ((float)width / (float)owidth));
							owidth = width;
						}
					}
					else
					{

						owidth = Convert.ToInt32((float)owidth * ((float)height / (float)oheight));
						oheight = height;
						//当缩略后的宽度超过了最大宽度时
						if (width > 0 && owidth > width)
						{
							oheight = Convert.ToInt32((float)oheight * ((float)width / (float)owidth));
							owidth = width;
						}
					}
					break;
				default:
					owidth = width;
					oheight = height;
					break;
			}


			System.Drawing.Image.GetThumbnailImageAbort myCallback = new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback);

			System.Drawing.Image myThumbnail = image.GetThumbnailImage(owidth, oheight, myCallback, IntPtr.Zero);
			var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			formatter.Serialize(imageStream, myThumbnail);
			byte[] mBytes = new byte[imageStream.Length];
			imageStream.Position = 0;
			imageStream.Read(mBytes, 0, (int)imageStream.Length);
			imageStream.Close();

			myThumbnail.Dispose();
			image.Dispose();
			return imageStream;
		}

		#endregion

		#region "生成带圆角的图片"
		/// <summary>
		/// 生成带圆角的图片
		/// RoundCorners("c:/123.png","c:/s_123.png" 25, Color.White);
		/// </summary>
		/// <param name="FileName">图片路径,物理路径或网址</param>
		/// <param name="new_FileName">新图路径，物理路径</param>
		/// <param name="CornerRadius">角度</param>
		/// <param name="BackgroundColor">底色</param>
		/// <returns></returns>
		public static void RoundCorners(string FileName,string new_FileName, int CornerRadius, Color BackgroundColor)
		{
			Image StartImage;
			if (FileName.ToLower().StartsWith("http"))
			{
				WebRequest myrequest = WebRequest.Create(FileName);//前台js传的path，可以是远程服务器上的，也可以是本地的
				WebResponse myresponse = myrequest.GetResponse();
				Stream imgstream = myresponse.GetResponseStream();
				StartImage = System.Drawing.Image.FromStream(imgstream);
			}
			else
			{
				StartImage = Image.FromFile(FileName);
			}
			CornerRadius *= 2;
			Bitmap RoundedImage = new Bitmap(StartImage.Width, StartImage.Height);
			using (Graphics g = Graphics.FromImage(RoundedImage))
			{
				g.Clear(BackgroundColor);
				g.SmoothingMode = SmoothingMode.AntiAlias;
				Brush brush = new TextureBrush(StartImage);
				GraphicsPath gp = new GraphicsPath();
				gp.AddArc(0, 0, CornerRadius, CornerRadius, 180, 90);
				gp.AddArc(0 + RoundedImage.Width - CornerRadius, 0, CornerRadius, CornerRadius, 270, 90);
				gp.AddArc(0 + RoundedImage.Width - CornerRadius, 0 + RoundedImage.Height - CornerRadius, CornerRadius, CornerRadius, 0, 90);
				gp.AddArc(0, 0 + RoundedImage.Height - CornerRadius, CornerRadius, CornerRadius, 90, 90);
				g.FillPath(brush, gp); 
				RoundedImage.Save(new_FileName, ImageFormat.Png);
				//return new_FileName;
			}
		}
		#endregion

		#region "旋转90,180,270"
		/// <summary>
		/// 旋转90,180,270
		/// </summary>
		/// <param name="bmp"></param>
		/// <param name="angle">角度</param>
		/// <returns></returns>
		public static Bitmap RotateImage(Bitmap bmp, int angle)
		{
			if (angle != 90 && angle != 180 && angle != 270)
			{
				return null;
			}
			int width = bmp.Width;
			int height = bmp.Height;

			if (angle == 90)
			{
				Bitmap newbmp = new Bitmap(height, width);
				using (Graphics g = Graphics.FromImage(newbmp))
				{
					Point[] destinationPoints = {
						new Point(height, 0), // destination for upper-left point of original
                        new Point(height, width),// destination for upper-right point of original
                        new Point(0, 0)}; // destination for lower-left point of original
					g.DrawImage(bmp, destinationPoints);
				}
				return newbmp;
			}

			if (angle == 180)
			{
				Bitmap newbmp = new Bitmap(width, height);
				using (Graphics g = Graphics.FromImage(newbmp))
				{
					Point[] destinationPoints = {
						new Point(width, height), // destination for upper-left point of original
                        new Point(0, height),// destination for upper-right point of original
                        new Point(width, 0)}; // destination for lower-left point of original
					g.DrawImage(bmp, destinationPoints);
				}
				return newbmp;
			}

			if (angle == 270)
			{
				Bitmap newbmp = new Bitmap(height, width);
				using (Graphics g = Graphics.FromImage(newbmp))
				{
					Point[] destinationPoints = {
						new Point(0, width), // destination for upper-left point of original
                        new Point(0, 0),// destination for upper-right point of original
                        new Point(height, width)}; // destination for lower-left point of original
					g.DrawImage(bmp, destinationPoints);
				}
				return newbmp;
			}
			return null;
		}
		#endregion

		#region "重设大小"
		//重设大小
		public static Bitmap ResizeImage(Bitmap bmp, Size size)
		{
			Bitmap newbmp = new Bitmap(size.Width, size.Height);
			using (Graphics g = Graphics.FromImage(newbmp))
			{
				g.DrawImage(bmp, new Rectangle(Point.Empty, size));
			}
			return newbmp;
		}
		#endregion

		#region "图片特效"
		//底片:Negative
		public static Bitmap NegativeImage(Bitmap bmp)
		{
			int height = bmp.Height;
			int width = bmp.Width;
			Bitmap newbmp = new Bitmap(width, height);

			LockBitmap lbmp = new LockBitmap(bmp);
			LockBitmap newlbmp = new LockBitmap(newbmp);
			lbmp.LockBits();
			newlbmp.LockBits();

			Color pixel;
			for (int x = 1; x < width; x++)
			{
				for (int y = 1; y < height; y++)
				{
					int r, g, b;
					pixel = lbmp.GetPixel(x, y);
					r = 255 - pixel.R;
					g = 255 - pixel.G;
					b = 255 - pixel.B;
					newlbmp.SetPixel(x, y, Color.FromArgb(r, g, b));
				}
			}
			lbmp.UnlockBits();
			newlbmp.UnlockBits();
			return newbmp;
		}

		//
		/// <summary>
		/// 黑白:Gray
		/// </summary>
		/// <param name="bmp"></param>
		/// <param name="type">0平均，1最大值法，2加权平均值法</param>
		/// <returns></returns>
		public static Bitmap GrayImage(Bitmap bmp, int type = 0)
		{
			int height = bmp.Height;
			int width = bmp.Width;
			Bitmap newbmp = new Bitmap(width, height);

			LockBitmap lbmp = new LockBitmap(bmp);
			LockBitmap newlbmp = new LockBitmap(newbmp);
			lbmp.LockBits();
			newlbmp.LockBits();

			Color pixel;
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					pixel = lbmp.GetPixel(x, y);
					int r, g, b, Result = 0;
					r = pixel.R;
					g = pixel.G;
					b = pixel.B;
					switch (type)
					{
						case 0://平均值法
							Result = ((r + g + b) / 3);
							break;
						case 1://最大值法
							Result = r > g ? r : g;
							Result = Result > b ? Result : b;
							break;
						case 2://加权平均值法
							Result = ((int)(0.3 * r) + (int)(0.59 * g) + (int)(0.11 * b));
							break;
					}
					newlbmp.SetPixel(x, y, Color.FromArgb(Result, Result, Result));
				}
			}
			lbmp.UnlockBits();
			newlbmp.UnlockBits();
			return newbmp;
		}

		//浮雕:Embossment
		public static Bitmap EmbossmentImage(Bitmap bmp)
		{
			int height = bmp.Height;
			int width = bmp.Width;
			Bitmap newbmp = new Bitmap(width, height);

			LockBitmap lbmp = new LockBitmap(bmp);
			LockBitmap newlbmp = new LockBitmap(newbmp);
			lbmp.LockBits();
			newlbmp.LockBits();

			Color pixel1, pixel2;
			for (int x = 0; x < width - 1; x++)
			{
				for (int y = 0; y < height - 1; y++)
				{
					int r = 0, g = 0, b = 0;
					pixel1 = lbmp.GetPixel(x, y);
					pixel2 = lbmp.GetPixel(x + 1, y + 1);
					r = Math.Abs(pixel1.R - pixel2.R + 128);
					g = Math.Abs(pixel1.G - pixel2.G + 128);
					b = Math.Abs(pixel1.B - pixel2.B + 128);
					if (r > 255)
						r = 255;
					if (r < 0)
						r = 0;
					if (g > 255)
						g = 255;
					if (g < 0)
						g = 0;
					if (b > 255)
						b = 255;
					if (b < 0)
						b = 0;
					newlbmp.SetPixel(x, y, Color.FromArgb(r, g, b));
				}
			}
			lbmp.UnlockBits();
			newlbmp.UnlockBits();
			return newbmp;
		}

		//柔化:Soften
		public static Bitmap SoftenImage(Bitmap bmp)
		{
			int height = bmp.Height;
			int width = bmp.Width;
			Bitmap newbmp = new Bitmap(width, height);

			LockBitmap lbmp = new LockBitmap(bmp);
			LockBitmap newlbmp = new LockBitmap(newbmp);
			lbmp.LockBits();
			newlbmp.LockBits();

			Color pixel;
			//高斯模板
			int[] Gauss = { 1, 2, 1, 2, 4, 2, 1, 2, 1 };
			for (int x = 1; x < width - 1; x++)
			{
				for (int y = 1; y < height - 1; y++)
				{
					int r = 0, g = 0, b = 0;
					int Index = 0;
					for (int col = -1; col <= 1; col++)
					{
						for (int row = -1; row <= 1; row++)
						{
							pixel = lbmp.GetPixel(x + row, y + col);
							r += pixel.R * Gauss[Index];
							g += pixel.G * Gauss[Index];
							b += pixel.B * Gauss[Index];
							Index++;
						}
					}
					r /= 16;
					g /= 16;
					b /= 16;
					//处理颜色值溢出
					r = r > 255 ? 255 : r;
					r = r < 0 ? 0 : r;
					g = g > 255 ? 255 : g;
					g = g < 0 ? 0 : g;
					b = b > 255 ? 255 : b;
					b = b < 0 ? 0 : b;
					newlbmp.SetPixel(x - 1, y - 1, Color.FromArgb(r, g, b));
				}
			}
			lbmp.UnlockBits();
			newlbmp.UnlockBits();
			return newbmp;
		}

		//锐化:Sharpen
		/// <summary>
		/// 取每个像素点与周边8个点的像素平均值的差值*锐化程度值 ，原点RGB加上这3色的差值形成新点
		/// </summary>
		/// <param name="bmp"></param>
		/// <param name="v">锐化程度-1到1</param>
		/// <returns></returns>
		public static Bitmap SharpenImage(Bitmap bmp, double v = 1)
		{
			int height = bmp.Height;
			int width = bmp.Width;
			Bitmap newbmp = new Bitmap(width, height);

			LockBitmap lbmp = new LockBitmap(bmp);
			LockBitmap newlbmp = new LockBitmap(newbmp);
			lbmp.LockBits();
			newlbmp.LockBits();

			int new_x = 0, new_y = 0;
			Color pixel;
			for (int x = 0; x < width; x++)//边缘八个点像素不变,从1开始最大数-1
			{
				for (int y = 0; y < height; y++)
				{
					double r = 0, g = 0, b = 0, a = 0;
					double r0 = 0, g0 = 0, b0 = 0, a0 = 0;//代表中心点
					int Index = 0;
					for (int col = -1; col <= 1; col++)//取像素本身加上周边9个点
					{
						for (int row = -1; row <= 1; row++)
						{
							new_x = x + row;
							new_y = y + col;
							//左边线
							if (x == 0)
							{
								new_x++;
							}
							//右边线
							if (x == (width - 1))
							{
								new_x--;
							}
							//上边线
							if (y == 0)
							{
								new_y++;
							}
							//下边线
							if (y == (height - 1))
							{
								new_y--;
							}
							pixel = lbmp.GetPixel(new_x, new_y);
							if (col == 0 && row == 0)
							{
								r0 = pixel.R;
								g0 = pixel.G;
								b0 = pixel.B;
								a0 = pixel.A;
							}
							r += pixel.R;//取周边8个点像数R值总和,排除自己
							g += pixel.G;//取周边9个点像数G值总和,排除自己
							b += pixel.B;//取周边9个点像数B值总和,排除自己
							Index++;
						}
					}
					//取周边9个点像素RGB值的平均值 
					r = r0 + v * (r0 - r / 9);
					g = g0 + v * (g0 - g / 9);
					b = b0 + v * (b0 - b / 9);
					a = a0;//透明度不变
						   //处理颜色值溢出
					r = r > 255 ? 255 : r;
					r = r < 0 ? 0 : r;
					g = g > 255 ? 255 : g;
					g = g < 0 ? 0 : g;
					b = b > 255 ? 255 : b;
					b = b < 0 ? 0 : b;
					//newlbmp.SetPixel(x - 1, y - 1, Color.FromArgb(int.Parse(r.ToString("0")), int.Parse(g.ToString("0")), int.Parse(b.ToString("0"))));
					newlbmp.SetPixel(x, y, Color.FromArgb(int.Parse(a.ToString("0")), int.Parse(r.ToString("0")), int.Parse(g.ToString("0")), int.Parse(b.ToString("0"))));
				}
			}
			lbmp.UnlockBits();
			newlbmp.UnlockBits();
			return newbmp;
		}

		//雾化:Atomization
		public static Bitmap AtomizationImage(Bitmap bmp)
		{
			int height = bmp.Height;
			int width = bmp.Width;
			Bitmap newbmp = new Bitmap(width, height);

			LockBitmap lbmp = new LockBitmap(bmp);
			LockBitmap newlbmp = new LockBitmap(newbmp);
			lbmp.LockBits();
			newlbmp.LockBits();

			System.Random MyRandom = new Random();
			Color pixel;
			for (int x = 1; x < width - 1; x++)
			{
				for (int y = 1; y < height - 1; y++)
				{
					int k = MyRandom.Next(123456);
					//像素块大小
					int dx = x + k % 19;
					int dy = y + k % 19;
					if (dx >= width)
						dx = width - 1;
					if (dy >= height)
						dy = height - 1;
					pixel = lbmp.GetPixel(dx, dy);
					newlbmp.SetPixel(x, y, pixel);
				}
			}
			lbmp.UnlockBits();
			newlbmp.UnlockBits();
			return newbmp;
		}
		#endregion

	}

	/// <summary>
	/// 获取或设置Bitmap图片像素颜色方法：GetPixel 和 SetPixel，如果直接对这两个方法进行操作的话速度很慢，这里我们可以通过把数据提取出来操作，然后操作完在复制回去可以加快访问速度
	/// </summary>
	public class LockBitmap
	{
		Bitmap source = null;
		IntPtr Iptr = IntPtr.Zero;
		BitmapData bitmapData = null;

		public byte[] Pixels { get; set; }
		public int Depth { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }

		public LockBitmap(Bitmap source)
		{
			this.source = source;
		}

		/// <summary>
		/// Lock bitmap data
		/// </summary>
		public void LockBits()
		{
			try
			{
				// Get width and height of bitmap
				Width = source.Width;
				Height = source.Height;

				// get total locked pixels count
				int PixelCount = Width * Height;

				// Create rectangle to lock
				Rectangle rect = new Rectangle(0, 0, Width, Height);

				// get source bitmap pixel format size
				Depth = System.Drawing.Bitmap.GetPixelFormatSize(source.PixelFormat);

				// Check if bpp (Bits Per Pixel) is 8, 24, or 32
				if (Depth != 8 && Depth != 24 && Depth != 32)
				{
					throw new ArgumentException("Only 8, 24 and 32 bpp images are supported.");
				}

				// Lock bitmap and return bitmap data
				bitmapData = source.LockBits(rect, ImageLockMode.ReadWrite,
											 source.PixelFormat);

				// create byte array to copy pixel values
				int step = Depth / 8;
				Pixels = new byte[PixelCount * step];
				Iptr = bitmapData.Scan0;

				// Copy data from pointer to array
				Marshal.Copy(Iptr, Pixels, 0, Pixels.Length);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// Unlock bitmap data
		/// </summary>
		public void UnlockBits()
		{
			try
			{
				// Copy data from byte array to pointer
				Marshal.Copy(Pixels, 0, Iptr, Pixels.Length);

				// Unlock bitmap data
				source.UnlockBits(bitmapData);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// Get the color of the specified pixel
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public Color GetPixel(int x, int y)
		{
			Color clr = Color.Empty;

			// Get color components count
			int cCount = Depth / 8;

			// Get start index of the specified pixel
			int i = ((y * Width) + x) * cCount;

			if (i > Pixels.Length - cCount)
				throw new IndexOutOfRangeException();

			if (Depth == 32) // For 32 bpp get Red, Green, Blue and Alpha
			{
				byte b = Pixels[i];
				byte g = Pixels[i + 1];
				byte r = Pixels[i + 2];
				byte a = Pixels[i + 3]; // a
				clr = Color.FromArgb(a, r, g, b);
			}
			if (Depth == 24) // For 24 bpp get Red, Green and Blue
			{
				byte b = Pixels[i];
				byte g = Pixels[i + 1];
				byte r = Pixels[i + 2];
				clr = Color.FromArgb(r, g, b);
			}
			if (Depth == 8)
			// For 8 bpp get color value (Red, Green and Blue values are the same)
			{
				byte c = Pixels[i];
				clr = Color.FromArgb(c, c, c);
			}
			return clr;
		}

		/// <summary>
		/// Set the color of the specified pixel
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="color"></param>
		public void SetPixel(int x, int y, Color color)
		{
			// Get color components count
			int cCount = Depth / 8;

			// Get start index of the specified pixel
			int i = ((y * Width) + x) * cCount;

			if (Depth == 32) // For 32 bpp set Red, Green, Blue and Alpha
			{
				Pixels[i] = color.B;
				Pixels[i + 1] = color.G;
				Pixels[i + 2] = color.R;
				Pixels[i + 3] = color.A;
			}
			if (Depth == 24) // For 24 bpp set Red, Green and Blue
			{
				Pixels[i] = color.B;
				Pixels[i + 1] = color.G;
				Pixels[i + 2] = color.R;
			}
			if (Depth == 8)
			// For 8 bpp set color value (Red, Green and Blue values are the same)
			{
				Pixels[i] = color.B;
			}
		}
	}
}



