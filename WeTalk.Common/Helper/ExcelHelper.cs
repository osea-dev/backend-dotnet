using Aspose.Cells;
using System;
using System.Data;
using System.IO;

namespace WeTalk.Common.Helper
{
	///   <summary >       
	///   Excel操作类       
	///   </summary >       
	public class ExcelHelper
    {
        public string mFilename;
        //public Application app;
        //public Workbooks wbs;
        public Workbook wb;
        //public Worksheets wss;
        public Worksheet ws;
        public ExcelHelper(string Key = "", string FileName = "")
        {
            //       
            //   TODO:   在此处添加构造函数逻辑       
            //
            try
            {
                new Aspose.Cells.License().SetLicense(new MemoryStream(Convert.FromBase64String(Key)));//正版KEY授权
            }
            catch { }

            if (FileName != "")
            {
                mFilename = FileName;
                wb = new Workbook(FileName);
            }
            else
            {
                wb = new Workbook();
            }
        }

        /// <summary>
        /// 创建一个Excel对象
        /// </summary>
        public void Create(string FileName = "")
        {
            //app = new Application();
            //wbs = app.Workbooks;
            //wb = wbs.Add(true);
            if (FileName != "")
            {
                mFilename = FileName;
                wb = new Workbook(FileName);
            }
            else
            {
                wb = new Workbook();
            }
            wb.Worksheets.Clear();
            wb.Worksheets.Add("sheet1");//New Worksheet1是Worksheet的name,ws = wb.Worksheets[0];
        }

        /// <summary>
        /// 获取一个工作表
        /// </summary>
        /// <param name="SheetName">工作表名称</param>
        /// <returns>Excel工作表</returns>
        public Worksheet GetSheet(string SheetName)
        {
            Worksheet s = (Worksheet)wb.Worksheets[SheetName];
            return s;
        }


        /// <summary>
        /// 从指定工作表中获取指定单元格的值，允许获取合并单元格的值
        /// </summary>
        /// <param name="SheetName">工作表名</param>
        /// <param name="row0">开始行</param>
        /// <param name="row1">结束行</param>
        /// <param name="column0">开始列</param>
        /// <param name="column1">结束列</param>
        /// <returns>DataTable</returns>
        public string GetSheetValue(string SheetName, int row0, int row1, int column0, int column1)
        {
            //表达正确的是A.WS.Cells(3,4)代表第3行第4列
            //1.ws.range("D3")
            //2.ws.range(Cells(3,4),Cells(3,4))
            string v = "";
            ws = (Worksheet)wb.Worksheets[SheetName];
            row0 = (row0 <= 0) ? 0 : (row0 - 1);
            row1 = (row1 <= 0) ? 0 : (row1 - 1);
            column0 = (column0 <= 0) ? 0 : (column0 - 1);
            column1 = (column1 <= 0) ? 0 : (column1 - 1);
			Aspose.Cells.Range range = ws.Cells.CreateRange(row0, column0, (row1 - row0 + 1), (column1 - column0 + 1));
            //if (s.Range[s.Cells[row0, column0], s.Cells[row1, column1]].Value != null) v = s.Range[s.Cells[row0, column0], s.Cells[row1, column1]].Value.ToString();
            if (range.Value != null) v = range.Value.ToString();
            return v;
        }

        /// <summary>
        /// 从指定工作表中获取指定单元格的值到DataTable中
        /// </summary>
        /// <param name="SheetName">工作表名</param>
        /// <param name="row0">开始行</param>
        /// <param name="row1">结束行</param>
        /// <param name="column0">开始列</param>
        /// <param name="column1">结束列</param>
        /// <returns>DataTable</returns>
        public DataTable GetSheetTable(string SheetName, int row0, int row1, int column0, int column1)
        {
            //表达正确的是A.WS.Cells(3,4)代表第3行第4列
            //1.ws.range("D3")
            //2.ws.range(Cells(3,4),Cells(3,4))
            Worksheet s = (Worksheet)wb.Worksheets[SheetName];


            DataTable dt = new DataTable();
            DataRow dr;
            string cellContent;
            //int iRowCount = s.UsedRange.Rows.Count;
            //int iColCount = s.UsedRange.Columns.Count;
            //Range range;
            int iRowCount = s.Cells.Rows.Count;
            int iColCount = s.Cells.Columns.Count;
			Aspose.Cells.Range range;

            if (row0 <= 0) row0 = 1;
            if (row1 <= 0) row1 = iRowCount;
            if (column0 <= 0) column0 = 1;
            if (column1 <= 0) column1 = iColCount;

            row0 = (row0 <= 0) ? 0 : (row0 - 1);
            row1 = (row1 <= 0) ? 0 : (row1 - 1);
            column0 = (column0 <= 0) ? 0 : (column0 - 1);
            column1 = (column1 <= 0) ? 0 : (column1 - 1);

            for (int iRow = row0; iRow <= row1; iRow++)
            {
                dr = dt.NewRow();

                for (int iCol = column0; iCol <= column1; iCol++)
                {
                    //range = (Range)s.CreateRange[iRow, iCol];
                    //cellContent = (range.Value2 == null) ? "" : range.Text.ToString();
                    range = s.Cells.CreateRange(iRow, iCol, 1, 1);
                    cellContent = (range.Value == null) ? "" : range.Value.ToString();

                    if (iRow == row0)
                    {
                        dt.Columns.Add(cellContent);
                    }
                    dr[iCol] = cellContent;
                }

                //if (iRow != 1)
                dt.Rows.Add(dr);
            }

            return dt;
        }

        /// <summary>
        /// 添加一个工作表
        /// </summary>
        /// <param name="SheetName">工作表名称</param>
        /// <returns>Excel工作表</returns>
        public Worksheet AddSheet(string SheetName)
        {


            //Worksheet s = (Worksheet)wb.Worksheets.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            //s.Name = SheetName;
            Worksheet s = wb.Worksheets.Add(SheetName);
            s.Name = SheetName;
            return s;
        }

        /// <summary>
        /// 删除一个工作表
        /// </summary>
        /// <param name="SheetName">工作表名称</param>
        public void DelSheet(string SheetName)
        {
            //((Worksheet)wb.Worksheets[SheetName]).Delete();
            Worksheet s = wb.Worksheets[SheetName];
            s.ClearComments();
        }

        /// <summary>
        /// 重命名一个工作表
        /// </summary>
        /// <param name="OldSheetName">要改名的工作表</param>
        /// <param name="NewSheetName">工作表新名称</param>
        /// <returns>工作表</returns>
        public Worksheet ReNameSheet(string OldSheetName, string NewSheetName)
        {
            Worksheet s = (Worksheet)wb.Worksheets[OldSheetName];
            s.Name = NewSheetName;
            return s;
        }

        /// <summary>
        /// 重命名一个工作表
        /// </summary>
        /// <param name="Sheet">Excel工作表实例</param>
        /// <param name="NewSheetName">新命名的工作表</param>
        /// <returns>Excel工作表</returns>
        public Worksheet ReNameSheet(Worksheet Sheet, string NewSheetName)
        {
            Sheet.Name = NewSheetName;
            return Sheet;
        }

        /// <summary>
        /// 设置工作表的值
        /// </summary>
        /// <param name="ws">要设值的工作表</param>
        /// <param name="x">行</param>
        /// <param name="y">列</param>
        /// <param name="value">要设置的值</param>
        public void SetCellValue(Worksheet ws, int x, int y, object value)
        {
            x = (x <= 0) ? 0 : (x - 1);
            y = (y <= 0) ? 0 : (y - 1);
            ws.Cells[x, y].PutValue(value);
        }

        /// <summary>
        /// 设置工作表的值
        /// </summary>
        /// <param name="ws">工作表的名称</param>
        /// <param name="x">行</param>
        /// <param name="y">列</param>
        /// <param name="value">要设置的值</param>
        public void SetCellValue(string ws, int x, int y, object value)
        {
            x = (x <= 0) ? 0 : (x - 1);
            y = (y <= 0) ? 0 : (y - 1);
            GetSheet(ws).Cells[x, y].PutValue(value);
        }

        /// <summary>
        /// 设置工作表的有效性值
        /// </summary>
        /// <param name="ws">工作表的名称</param>
        /// <param name="x">行</param>
        /// <param name="y">列</param>
        /// <param name="value">要设置的值(数据有效性)</param>
        public void SetCellDataValue(string ws, int x, int y, string value)
        {
            x = (x <= 0) ? 0 : (x - 1);
            y = (y <= 0) ? 0 : (y - 1);
			Aspose.Cells.Range r;
            Worksheet WS = (Worksheet)wb.Worksheets[ws];
            //r = WS.Range[WS.Cells[x, y], WS.Cells[x, y]];

            ////单元格已设置数据有效性,只能用代码修改有效性;如果单元格未设置有效性,需要使用 Add 方法
            //r.Validation.Modify(XlDVType.xlValidateList, XlDVAlertStyle.xlValidAlertStop, Type.Missing, value, Type.Missing);
            ////r.Validation.Add(XlDVType.xlValidateList, XlDVAlertStyle.xlValidAlertStop, Type.Missing, value, Type.Missing); 

            r = WS.Cells.CreateRange(x, y, 1, 1);
            r.Value = value;
        }


        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="ws">工作表</param>
        /// <param name="x1">开始的行</param>
        /// <param name="y1">开始的列</param>
        /// <param name="x2">结束的行</param>
        /// <param name="y2">结束的列</param>
        public void UniteCells(Worksheet ws, int x1, int y1, int x2, int y2)
        {
            x1 = (x1 <= 0) ? 0 : (x1 - 1);
            y1 = (y1 <= 0) ? 0 : (y1 - 1);
            x2 = (x2 <= 0) ? 0 : (x2 - 1);
            y2 = (y2 <= 0) ? 0 : (y2 - 1);
            //ws.get_Range(ws.Cells[x1, y1], ws.Cells[x2, y2]).Merge(Type.Missing);
            ws.Cells.Merge(x1, y1, x2 - x1, y2 - y1);//合并单元格 
        }

        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="ws">工作表名称</param>
        /// <param name="x1">开始的行</param>
        /// <param name="y1">开始的列</param>
        /// <param name="x2">结束的行</param>
        /// <param name="y2">结束的列</param>
        public void UniteCells(string ws, int x1, int y1, int x2, int y2)
        {
            x1 = (x1 <= 0) ? 0 : (x1 - 1);
            y1 = (y1 <= 0) ? 0 : (y1 - 1);
            x2 = (x2 <= 0) ? 0 : (x2 - 1);
            y2 = (y2 <= 0) ? 0 : (y2 - 1);
            //GetSheet(ws).get_Range(GetSheet(ws).Cells[x1, y1], GetSheet(ws).Cells[x2, y2]).Merge(Type.Missing);
            GetSheet(ws).Cells.Merge(x1, y1, x2 - x1, y2 - y1);//合并单元格 
        }

        /// <summary>
        /// 将表格插入到Excel的指定工作表指定位置
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="ws">工作表名称</param>
        /// <param name="startX">开始行</param>
        /// <param name="startY">开始列</param>
        public void InsertTable(System.Data.DataTable dt, string ws, int startX, int startY)
        {
            startX = (startX <= 0) ? 0 : (startX - 1);
            startY = (startY <= 0) ? 0 : (startY - 1);
            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                for (int j = 0; j <= dt.Columns.Count - 1; j++)
                {
                    GetSheet(ws).Cells[startX + i, j + startY].PutValue(dt.Rows[i][j].ToString());
                }
            }
        }

        /// <summary>
        /// 将表格插入到Excel的指定工作表指定位置
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="ws">工作表</param>
        /// <param name="startX">开始行</param>
        /// <param name="startY">开始列</param>
        public void InsertTable(System.Data.DataTable dt, Worksheet ws, int startX, int startY)
        {
            startX = (startX <= 0) ? 0 : (startX - 1);
            startY = (startY <= 0) ? 0 : (startY - 1);
            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                for (int j = 0; j <= dt.Columns.Count - 1; j++)
                {
                    ws.Cells[startX + i, j + startY].PutValue(dt.Rows[i][j]);
                }
            }
        }

        /// <summary>
        /// DataTable表格添加到Excel指定工作表的指定位置
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="ws">工作表名称</param>
        /// <param name="startX">开始行</param>
        /// <param name="startY">开始列</param>
        public void AddTable(System.Data.DataTable dt, string ws, int startX, int startY)
        {
            startX = (startX <= 0) ? 0 : (startX - 1);
            startY = (startY <= 0) ? 0 : (startY - 1);
            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                for (int j = 0; j <= dt.Columns.Count - 1; j++)
                {
                    GetSheet(ws).Cells[i + startX, j + startY].PutValue(dt.Rows[i][j]);
                }
            }
        }

        /// <summary>
        /// DataTable表格添加到Excel指定工作表的指定位置
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="ws">工作表</param>
        /// <param name="startX">开始行</param>
        /// <param name="startY">开始列</param>
        public void AddTable(System.Data.DataTable dt, Worksheet ws, int startX, int startY)
        {
            startX = (startX <= 0) ? 0 : (startX - 1);
            startY = (startY <= 0) ? 0 : (startY - 1);
            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                for (int j = 0; j <= dt.Columns.Count - 1; j++)
                {
                    ws.Cells[i + startX, j + startY].PutValue(dt.Rows[i][j]);
                }
            }
        }


        /// <summary>
        /// 将图片插入到工作表中
        /// </summary>
        /// <param name="Filename">图片</param>
        /// <param name="ws">工作表</param>
        public void InsertPictures(string Filename, string ws)
        {
            //GetSheet(ws).Shapes.AddPicture(Filename, MsoTriState.msoFalse, MsoTriState.msoTrue, 10, 10, 150, 150);//后面的数字表示位置       
            GetSheet(ws).Pictures.Add(10, 10, Filename);
        }


        /// <summary>
        /// 保存文档 
        /// </summary>
        /// <returns>是否保存成功</returns>
        public bool Save()
        {
            if (mFilename == " ")
            {
                return false;
            }
            else
            {
                try
                {
                    wb.Save(mFilename);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 文档的另存为
        /// </summary>
        /// <param name="FileName">另存为名称</param>
        /// <returns>是否保存成功</returns>
        public bool SaveAs(object FileName)//文档另存为       
        {
            try
            {
                //wb.SaveAs(FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                wb.Save(FileName.ToString());
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        /// <summary>
        /// 关闭一个Excel对象，销毁对象
        /// </summary>
        public void Close()
        {

            //wb.Close(Type.Missing, Type.Missing, Type.Missing);
            //wbs.Close();
            //app.Quit();
            //wb = null;
            //wbs = null;
            //app = null;
            wb = null;
            GC.Collect();
        }

    }
}
