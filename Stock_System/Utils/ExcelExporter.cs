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

            ws.Cell(1, 1).Value = "ID";
            ws.Cell(1, 2).Value = "Rolo";
            ws.Cell(1, 3).Value = "MM";
            ws.Cell(1, 4).Value = "Metragem Disponível";
            ws.Cell(1, 5).Value = "WIP";

            int row = 2;
            foreach (var r in rolos)
            {
                ws.Cell(row, 1).Value = r.Id;
                ws.Cell(row, 2).Value = r.IdRolo;
                ws.Cell(row, 3).Value = r.Milimetragem;
                ws.Cell(row, 4).Value = r.MetragemDisponivel;
                ws.Cell(row, 5).Value = r.WIP;
                row++;
            }

            ws.Columns().AdjustToContents();

            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            var fileName = Path.Combine(folder, $"Estoque_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            wb.SaveAs(fileName);
            return fileName;
        }
    }
}
