using ICSharpCode.SharpZipLib.Checksum;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;

namespace WeTalk.Common.Helper
{
	public class ZipHelper
	{
		#region 压缩文件夹,支持递归
		/// <summary>
		///　压缩文件夹
		/// </summary>
		/// <param name="dir">待压缩的文件夹</param>
		/// <param name="targetFileName">压缩后文件路径（包括文件名）</param>
		/// <param name="recursive">是否递归压缩</param>
		/// <returns></returns>
		public static bool Compress(string dir, string targetFileName, bool recursive)
		{
			//如果已经存在目标文件，询问用户是否覆盖
			if (File.Exists(targetFileName))
			{
				throw new Exception("同名文件已经存在！");
			}
			string[] ars = new string[2];
			if (recursive == false)
			{
				ars[0] = dir;
				ars[1] = targetFileName;
				return ZipFileDictory(ars);
			}

			FileStream zipFile;
			ZipOutputStream zipStream;

			//打开压缩文件流
			zipFile = File.Create(targetFileName);
			zipStream = new ZipOutputStream(zipFile);

			if (dir != String.Empty)
			{
				CompressFolder(dir, zipStream, dir);
			}

			//关闭压缩文件流
			zipStream.Finish();
			zipStream.Close();

			if (File.Exists(targetFileName))
				return true;
			else
				return false;
		}

		/// <summary>
		/// 压缩目录
		/// </summary>
		/// <param name="args">数组(数组[0]: 要压缩的目录; 数组[1]: 压缩的文件名)</param>
		public static bool ZipFileDictory(string[] args)
		{
			ZipOutputStream zStream = null;
			try
			{
				string[] filenames = Directory.GetFiles(args[0]);
				Crc32 crc = new Crc32();
				zStream = new ZipOutputStream(File.Create(args[1]));
				zStream.SetLevel(6);
				//循环压缩文件夹中的文件
				foreach (string file in filenames)
				{
					//打开压缩文件
					FileStream fs = File.OpenRead(file);
					byte[] buffer = new byte[fs.Length];
					fs.Read(buffer, 0, buffer.Length);
					ZipEntry entry = new ZipEntry(file);
					entry.DateTime = DateTime.Now;
					entry.Size = fs.Length;
					fs.Close();
					crc.Reset();
					crc.Update(buffer);
					entry.Crc = crc.Value;
					zStream.PutNextEntry(entry);
					zStream.Write(buffer, 0, buffer.Length);
				}
			}
			catch
			{
				throw;
			}
			finally
			{
				zStream.Finish();
				zStream.Close();
			}
			return true;
		}

		/// <summary>
		/// 压缩某个子文件夹
		/// </summary>
		/// <param name="basePath">待压缩路径</param>
		/// <param name="zips">压缩文件流</param>
		/// <param name="zipfolername">待压缩根路径</param>     
		private static void CompressFolder(string basePath, ZipOutputStream zips, string zipfolername)
		{
			if (File.Exists(basePath))
			{
				AddFile(basePath, zips, zipfolername);
				return;
			}
			string[] names = Directory.GetFiles(basePath);
			foreach (string fileName in names)
			{
				AddFile(fileName, zips, zipfolername);
			}

			names = Directory.GetDirectories(basePath);
			foreach (string folderName in names)
			{
				CompressFolder(folderName, zips, zipfolername);
			}

		}

		/// <summary>
		///　压缩某个子文件
		/// </summary>
		/// <param name="fileName">待压缩文件</param>
		/// <param name="zips">压缩流</param>
		/// <param name="zipfolername">待压缩根路径</param>
		private static void AddFile(string fileName, ZipOutputStream zips, string zipfolername)
		{
			if (File.Exists(fileName))
			{
				CreateZipFile(fileName, zips, zipfolername);
			}
		}

		/// <summary>
		/// 压缩单独文件
		/// </summary>
		/// <param name="FileToZip">待压缩文件</param>
		/// <param name="zips">压缩流</param>
		/// <param name="zipfolername">待压缩根路径</param>
		private static void CreateZipFile(string FileToZip, ZipOutputStream zips, string zipfolername)
		{
			try
			{
				FileStream StreamToZip = new FileStream(FileToZip, FileMode.Open, FileAccess.Read);
				string temp = FileToZip;
				string temp1 = zipfolername;
				if (temp1.Length > 0)
				{
					temp = temp.Replace(zipfolername + "\\", "");
				}
				ZipEntry ZipEn = new ZipEntry(temp);

				zips.PutNextEntry(ZipEn);
				byte[] buffer = new byte[16384];
				System.Int32 size = StreamToZip.Read(buffer, 0, buffer.Length);
				zips.Write(buffer, 0, size);
				try
				{
					while (size < StreamToZip.Length)
					{
						int sizeRead = StreamToZip.Read(buffer, 0, buffer.Length);
						zips.Write(buffer, 0, sizeRead);
						size += sizeRead;
					}
				}
				catch (System.Exception ex)
				{
					throw ex;
				}

				StreamToZip.Close();
			}
			catch
			{
				throw;
			}
		}
		#endregion

		#region 解压缩
		/// <summary>   
		/// 功能：解压zip格式的文件。   
		/// </summary>   
		/// <param name="zipFilePath">压缩文件路径</param>   
		/// <param name="unZipDir">解压文件存放路径,为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹</param>   
		/// <returns>解压是否成功</returns>   
		public static void UnZipFile(string zipFilePath, string unZipDir)
		{

			if (zipFilePath == string.Empty)
			{
				throw new Exception("压缩文件不能为空！");
			}
			if (!File.Exists(zipFilePath))
			{
				throw new Exception("压缩文件不存在！");
			}
			//解压文件夹为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹   
			if (unZipDir == string.Empty)
				unZipDir = zipFilePath.Replace(Path.GetFileName(zipFilePath), Path.GetFileNameWithoutExtension(zipFilePath));
			if (!unZipDir.EndsWith("//"))
				unZipDir += "//";
			if (!Directory.Exists(unZipDir))
				Directory.CreateDirectory(unZipDir);

			try
			{
				using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
				{
					ZipEntry theEntry;
					while ((theEntry = s.GetNextEntry()) != null)
					{
						string directoryName = Path.GetDirectoryName(theEntry.Name);
						string fileName = Path.GetFileName(theEntry.Name);
						if (directoryName.Length > 0)
						{
							Directory.CreateDirectory(unZipDir + directoryName);
						}
						if (!directoryName.EndsWith("//"))
							directoryName += "//";
						if (fileName != String.Empty)
						{
							using (FileStream streamWriter = File.Create(unZipDir + theEntry.Name))
							{
								int size = 2048;
								byte[] data = new byte[2048];
								while (true)
								{
									size = s.Read(data, 0, data.Length);
									if (size > 0)
									{
										streamWriter.Write(data, 0, size);
									}
									else
									{
										break;
									}
								}
							}
						}
					}
				}
			}
			catch
			{
				throw;
			}
		}
		#endregion

	}
}
