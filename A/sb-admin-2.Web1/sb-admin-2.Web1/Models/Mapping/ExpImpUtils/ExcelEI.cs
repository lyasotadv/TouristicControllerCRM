using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Excel = Microsoft.Office.Interop.Excel;

namespace sb_admin_2.Web1.Models.Mapping.ExpImpUtils
{
    public class ExcelEI
    {
        public enum Direction { down, right }

        private enum Status { active, closed }
        private Status status { get; set; }

        private Excel.Application apl;
        private Excel.Sheets sheets;
        private Excel.Worksheet wsheets;
        private Excel.Workbook book;
        private Excel.Workbooks books;

        private string _listname;

        private string fname { get; set; }

        public string listname 
        {
            get { return _listname; }
            set 
            {
                if (value != _listname)
                {
                    _listname = value;
                    UpdateWorkSheet();
                }
            }
        }

        public string startrange { get; set; }

        public int h { get; set; }
        public int w { get; set; }

        public string[,] Data { get; private set; }

        private void UpdateWorkSheet()
        {
            if (string.IsNullOrEmpty(listname))
                wsheets = (Excel.Worksheet)sheets.get_Item(1);
            else
                wsheets = (Excel.Worksheet)sheets.get_Item(listname);
        }

        public ExcelEI(string FileName)
        {
            status = Status.active;

            fname = FileName;
            Init();
            UpdateWorkSheet();

            h = 0;
            w = 0;

            listname = null;
            startrange = null;

            Data = null;
        }

        ~ExcelEI()
        {
            Close();
        }

        public void AutoRange(string StartRange, Direction direction)
        {
            int dX = 0;
            int dY = 0;

            switch (direction)
            {
                case Direction.down:
                    {
                        dX = 0;
                        dY = 1;
                        h = 0;
                        break;
                    }
                case Direction.right:
                    {
                        dX = 1;
                        dY = 0;
                        w = 0;
                        break;
                    }
            }

            Excel.Range cells = wsheets.get_Range(StartRange, Type.Missing);
            string sStr = string.Empty;
            while (true)
            {
                h += dY;
                w += dX;
                cells = cells.get_Offset(dY, dX);
                sStr = Convert.ToString(cells.Value2);
                if (sStr == null)
                    break;
            }
        }

        private void Init()
        {
            if (status == Status.closed)
                throw new ObjectDisposedException("Aplication is closed. Create new object");

            if (string.IsNullOrEmpty(fname))
                throw new ArgumentNullException("File name have not been set");

            apl = new Excel.Application();
            books = apl.Workbooks;
            book = apl.Workbooks.Open(fname,
             Type.Missing, Type.Missing, Type.Missing, Type.Missing,
             Type.Missing, Type.Missing, Type.Missing, Type.Missing,
             Type.Missing, Type.Missing, Type.Missing, Type.Missing,
             Type.Missing, Type.Missing);
            sheets = book.Worksheets;
        }

        public void Close()
        {
            if (status != Status.closed)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(sheets);
                book.Close();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(book);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(books);
                apl.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(apl);
                status = Status.closed;
            }
        }

        public void Read()
        {
            if (string.IsNullOrEmpty(startrange))
                startrange = "A1";

            Data = new string[h, w];
            for (int n = 0; n < h; n++)
                for (int m = 0; m < w; m++)
                {
                    Excel.Range cells = wsheets.get_Range(startrange, Type.Missing);
                    cells = cells.get_Offset(n, m);
                    string sStr = Convert.ToString(cells.Value2);
                    Data[n, m] = sStr;
                }
        }
    }
}