using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Payroll__System
{

    class DataGridPrinter
    {
        private DataGridView dataGrid;
        private PrintDocument printDocument;
        private DataTable dataTable;

        public int PageNumber { get; set; }
        public int RowCount { get; set; }
        public int TopMargin { get; private set; }
        public object TheDataGrid { get; private set; }

        public DataGridPrinter(DataGridView grid, PrintDocument doc, DataTable table)
        {
            dataGrid = grid;
            printDocument = doc;
            dataTable = table;
            PageNumber = 1;
            RowCount = 0;
        }


        private void DrawHorizontalLines(Graphics g, List<float> Lines)
        {
            foreach (float yPosition in Lines)
            {
                g.DrawLine(Pens.Black, 0, yPosition, g.VisibleClipBounds.Width, yPosition);
            }
        }

        private void DrawVerticalGridLines(Graphics g, Pen pen, int columnWidth, int lastRowBottom, int columnCount)
        {
            int startX = 0;

            for (int i = 0; i <= columnCount; i++)
            {
                g.DrawLine(pen, startX, 0, startX, lastRowBottom);
                startX += columnWidth;
            }
        }


        public bool DrawDataGrid(Graphics g, DataGridView TheDataGrid, DataTable TheTable, int PageWidth, int PageHeight, int TopMargin, int BottomMargin, int RowCount, int PageNumber)
        {
            try
            {
                int lastRowBottom = TopMargin;

                // Create an array to store horizontal gridline positions
                List<float> Lines = new List<float>();

                // Create brushes for text and row backgrounds
                SolidBrush ForeBrush = new SolidBrush(TheDataGrid.ForeColor);
                SolidBrush BackBrush = new SolidBrush(TheDataGrid.BackColor);
                SolidBrush AlternatingBackBrush = new SolidBrush(TheDataGrid.BackColor);

                // Create a pen for gridlines
                Pen TheLinePen = new Pen(TheDataGrid.GridColor, 1);

                // Format for cell text
                StringFormat cellFormat = new StringFormat
                {
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit
                };

                // Calculate column width
                int columnWidth = PageWidth / TheTable.Columns.Count;

                // Set the initial row count for printing
                int initialRowCount = RowCount;

                // Iterate through rows in the DataTable
                for (int i = initialRowCount; i < TheTable.Rows.Count; i++)
                {
                    DataRow dr = TheTable.Rows[i];
                    int startXPosition = TheDataGrid.Location.X;

                    // Calculate row bounds
                    RectangleF RowBounds = new RectangleF(
                        startXPosition,
                        TopMargin + ((RowCount - initialRowCount) + 1) * (TheDataGrid.Font.SizeInPoints + 5), // Vertical offset
                        PageWidth,
                        TheDataGrid.Font.SizeInPoints + 5
                    );

                    // Save row bottom position for horizontal gridlines
                    Lines.Add(RowBounds.Bottom);

                    // Alternate row colors
                    g.FillRectangle((i % 2 == 0) ? BackBrush : AlternatingBackBrush, RowBounds);

                    // Draw each cell in the row
                    for (int j = 0; j < TheTable.Columns.Count; j++)
                    {
                        RectangleF cellBounds = new RectangleF(
                            startXPosition,
                            RowBounds.Y,
                            columnWidth,
                            TheDataGrid.Font.SizeInPoints + 5
                        );

                        if (startXPosition + columnWidth <= PageWidth)
                        {
                            g.DrawString(dr[j].ToString(), TheDataGrid.Font, ForeBrush, cellBounds, cellFormat);
                            lastRowBottom = (int)cellBounds.Bottom;
                        }

                        // Move to the next column
                        startXPosition += columnWidth;
                    }

                    RowCount++;

                    // Check if we've reached the bottom of the page
                    if ((RowCount * (TheDataGrid.Font.SizeInPoints + 5)) > (PageHeight - BottomMargin - TopMargin))
                    {
                        // Draw gridlines before returning
                        DrawHorizontalLines(g, Lines);
                        DrawVerticalGridLines(g, TheLinePen, columnWidth, lastRowBottom, TheTable.Columns.Count);
                        return true;
                    }
                }

                // Draw gridlines when the table is fully printed
                DrawHorizontalLines(g, Lines);
                DrawVerticalGridLines(g, TheLinePen, columnWidth, lastRowBottom, TheTable.Columns.Count);

                return false; // Indicate that printing is complete
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while drawing the DataGrid: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        
    }
}
    

