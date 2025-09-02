using ClosedXML.Excel;
using EstoqueRolos.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace EstoqueRolos.Utils
{
    public static class ExcelExporter
    {
        public static string Exportar(IEnumerable<Rolo> rolos, string folder)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Estoque");

            ws.Cell(1, 1).Value = "Código";
            ws.Cell(1, 2).Value = "Descrição";
            ws.Cell(1, 3).Value = "Milimetragem";
            ws.Cell(1, 4).Value = "MOQ";
            ws.Cell(1, 5).Value = "Estoque (m)";
            ws.Cell(1, 6).Value = "Metragem WIP";

            int row = 2;
            foreach (var r in rolos)
            {
                ws.Cell(row, 1).Value = r.Code;
                ws.Cell(row, 2).Value = r.Descricao;
                ws.Cell(row, 3).Value = r.Milimetragem;
                ws.Cell(row, 4).Value = r.MOQ;
                ws.Cell(row, 5).Value = r.Estoque;
                ws.Cell(row, 6).Value = r.MetragemWIP;
                row++;
            }

            ws.Columns().AdjustToContents();

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = Path.Combine(folder, $"Estoque_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            wb.SaveAs(fileName);
            return fileName;
        }
    }
}
