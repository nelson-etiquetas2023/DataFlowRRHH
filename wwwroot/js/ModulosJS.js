//Funciones necesarias.
function addCell(tr, content, colSpan = 1) {
    let td = document.createElement("th");
    td.colSpan = colSpan;
    td.innerText = content;
    tr.appendChild(td);
}
function getDataSrc(dt) {
    // Return the RowGroup dataSrc
    //var dataSrc = dt.rowGroup().dataSrc();
    var dataSrc = 1;

    // If multi level use only the first level
    if (Array.isArray(dataSrc)) {
        dataSrc = dataSrc[0];
    }
    return dataSrc;
}
function updateSheet1(xlsx, groupName, title, button, dt) {
    
    // Get number of columns to remove last hidden index column.
    var numColumns = dt.columns().header().count();
    
    var newSheet =
        '<?xml version="1.0" encoding="UTF-8" standalone="yes"?>' +
        '<worksheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships" xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" xmlns:x14ac="http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac" mc:Ignorable="x14ac">' +
        getTableData(groupName, title, button, dt) +
        "</worksheet>";

    


    // Get sheet.
    var sheet = (xlsx.xl.worksheets["sheet1.xml"] = $.parseXML(newSheet));
    

    // if (Array.isArray(groupName)) {
    //   setSheetName(
    //     xlsx,
    //     button.sheetName
    //       ? button.sheetName
    //       : document.getElementsByTagName("title")[0].innerHTML
    //   );
    // } else {
    //   setSheetName(xlsx, groupName);
    // }
}
function getTableData(groupName, title, button, dt) {
    // Processes Datatable row data to build sheet.
    //Params:
    //  dt: Datatable API.
    //  title: Title displayed at top of SS or empty str for no title.
    //Returns:
    //  String of XML formatted worksheet.
    //console.log('getTableData', groupName);
    let totalHe = 0;
    let horasLab = 0;
    let montoHe = 0;
    let monto100 = 0;
    // TOTALES GENERALES.
    let tgeneralHe = 0;
    let tghorasLab = 0;
    let tgmontoHe = 0;
    let tgmonto100 = 0;
    let thoras100 = 0;
    let tghoras100 = 0;

    //----------------
    var dataSrc = getDataSrc(dt);
    var header = getHeaderNames(dt);

    header[0] = "Fila num.";
    header[1] = "Id. Empleado";
    header[2] = "Nombre del Empleado";
    header[5] = "Descripcion del Horario Asignado";
    header[6] = "Horario Entrada";
    header[7] = "Horario Salida";
    header[8] = "Marcaje1";
    header[9] = "Tardanza Entrada (min.)";
    header[10] = "Marcaje2";
    header[11] = "Marcaje3";
    header[12] = "Marcaje4";
    header[13] = "Ponches.";
    header[14] = "Horas Trabajadas";
    header[15] = "Horas Extras (Minutos)";
    header[16] = "Horas Extras (Horas)";
    header[17] = "sueldo Hora";
    header[18] = "% Calculo Hora Extra";
    header[19] = "Fraccion-Sueldo";
    header[20] = "Monto Extras";
    header[21] = "Horas Extras 100";
    header[22] = "Monto Extra 100";
    header[23] = "";
    header[24] = "";

    var rowNum = 1;
    var mergeCells = [];
    var mergeCol = (header.length - 1 + 10).toString(36).toUpperCase();
    var ws = "";
    var selectorModifier = {};
    if (button.exportOptions.hasOwnProperty("modifier")) {
        selectorModifier = button.exportOptions.modifier;
    }

    ws += buildCols(header);
    ws += "<sheetData>";

    // Print button.title
    if (button.title.length > 0) {
        if (button.title === "*") {
            button.title = document.getElementsByTagName("title")[0].innerHTML;
        }

        ws += buildRow([button.title], rowNum, 51);

        mergeCells.push(
            '<mergeCell ref="A' + rowNum + ":" + mergeCol + "" + rowNum + '"/>'
        );

        rowNum++;
    }

    // Print button.messageTop
    if (button.messageTop.length > 0 && button.messageTop.length != "*") {
        ws += buildRow([button.messageTop], rowNum, 51);

        mergeCells.push(
            '<mergeCell ref="A' + rowNum + ":" + mergeCol + "" + rowNum + '"/>'
        );

        rowNum++;
    }

    // All rows on one page with group names separating groups
    if (Array.isArray(groupName)) {
        if (button.header) {

            ws += buildRow(header, rowNum, 2);
            rowNum++;
        }

        var currentGroup = "";
        let fil = 0;
        // Loop through each row to append to sheet.
        tg_tardanzas = 0;
        tg_horasextras_min = 0;
        tg_montoheporc = 0;
        tg_montohex100 = 0;
        tg_montoHorasExal100 = 0;

        dt.rows(selectorModifier).every(function (rowIdx, tableLoop, rowLoop) {
            fil++;
            var data = this.data();
            
          
            if (data[dataSrc] !== currentGroup) {

                //agregar la fila del grupo  
                //Reinicio del total por grupo
                currentGroup = data[dataSrc];
                totalHe = 0;
                horasLab = 0;
                montoHe = 0;
                monto100 = 0;
                thoras100 = 0;
                ntardaGroup = 0;
                horasex = 0;
                montoExtra100 = 0;
                horasExtrasAl100 = 0;
             
                rowNum++;
            }
            //Aqui se hacen los calculos para totalizar.
            if (data[dataSrc] = currentGroup) {
                totalHe += parseFloat(data[14]);
                horasLab += parseFloat(data[13]);
                montoHe += parseFloat(data[20]);
                monto100 += parseFloat(data[22]);
                thoras100 += parseFloat(data[21]);
                horasex += parseFloat(data[16]);
                horasExtrasAl100 += parseFloat(data[23]);


                montoExtra100 += parseFloat(data[24]);
                tg_horasextras_min += parseFloat(data[16]);
                tg_montoheporc += parseFloat(data[22]);
                tg_montohex100 += parseFloat(data[23]);
                tg_montoHorasExal100 += parseFloat(data[24]);
                //Esta funcion cuenta las tardanzas por empleado en la hoja de excel
                if (parseFloat(data[9]) >= 15)
                {
                    ntardaGroup += 1;
                    tg_tardanzas += 1;
                }

                //totales generales
                tgeneralHe += parseFloat(data[14]);
                tghorasLab += parseFloat(data[13]);
                tgmontoHe += parseFloat(data[20]);
                //tgmonto100 += parseFloat(data[22]);
               
            }

            // If data is object based then it needs to be converted
            // to an array before sending to buildRow()

            // Dibuja todas las filas de la data.
            data[8] = data[8].replaceAll('&nbsp;', '');
            data[9] = data[9].replaceAll('&nbsp;', '');
            data[10] = data[10].replaceAll('&nbsp;', '');
            data[11] = data[11].replaceAll('&nbsp;', '');
            data[12] = data[12].replaceAll('&nbsp;', '');

            ws += buildRow([fil, data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[8],
                data[9], data[10], data[11], data[12], data[13], data[14], data[15], data[16], 
                data[19], data[20], data[21], data[22], data[23], data[24]], rowNum, "", 51);
                 
           
            rowNum++;
            //agregar el footer de total por empleado.

            ws += buildRow([data[2], "", "", "", "", "", "", "", "", ntardaGroup + " tardanzas. ", "", "", "",
                horasLab + " ponches.", totalHe.toFixed(2) + " horas.", parseFloat(horasex * 60).toFixed(2) +
                " minutos.", horasex + " horas.", "", "", "", 
                "$" + monto100.toFixed(2), horasExtrasAl100, montoExtra100], rowNum, "", 51);
            
          
          

        });
       
        
        rowNum++;
        ws += buildRow(["=>", "", "", "", "", "", "", "", "", ""], "", 51);

        rowNum++;
        ws += buildRow(["...", "", "", "", "", "", "", "", "", "Total Tardanzas", "", "", "", "Total Ponches", "Total Horas Trabajadas", "Horas Extras (Minutos)", "Horas Extras (Horas)", "", "", "", "Monto Horas Extras","Horas Extra 100","Monto Extra 100"], "", 51);

        //Calculo de Total de la Hora
        rowNum++;
        ws += buildRow(["TOTALES GENERALES : ", "", "", "", "", "", "", "", "", tg_tardanzas, "", "", "", tghorasLab,
            tgeneralHe.toFixed(2), (tg_horasextras_min * 60).toFixed(2), tg_horasextras_min, "", "", "", tg_montoheporc.toFixed(2),
            tg_montohex100.toFixed(2), tg_montoHorasExal100.toFixed(2)], "", 51);
        rowNum++;

    } else {
        // Place each group on individual sheets
        if (title) {
            ws += buildRow([title], rowNum, 51);

            mergeCells.push(
                '<mergeCell ref="A' +
                rowNum +
                ":" +
                mergeCol +
                "" +
                rowNum +
                '"/>'
            );
            rowNum++;
        }

        if (button.header) {
            ws += buildRow(header, rowNum, 2);
            rowNum++;
        }

        // Loop through each row to append to sheet.

        table
            .rows(function (idx, data, node) {
                return data[dataSrc] === groupName ? true : false;
            }, selectorModifier)
            .every(function (rowIdx, tableLoop, rowLoop) {
                var data = this.data();
                // If data is object based then it needs to be converted
                // to an array before sending to buildRow()
                ws += buildRow(data, rowNum, "");

                rowNum++;
            });

    }

    // Output footer
    if (button.footer) {
        ws += buildRow(getFooterNames(dt), rowNum, 2);
        rowNum++;
    }

    // Print button.messageBottom
    if (
        button.messageBottom.length > 0 &&
        button.messageBottom.length != "*"
    ) {
        ws += buildRow([button.messageBottom], rowNum, 51);

        mergeCells.push(
            '<mergeCell ref="A' + rowNum + ":" + mergeCol + "" + rowNum + '"/>'
        );
        rowNum++;
    }

    mergeCellsElement = "";

    if (mergeCells) {
        mergeCellsElement =
            '<mergeCells count="' +
            mergeCells.length +
            '">' +
            mergeCells +
            "</mergeCells>";
    }
    ws += "</sheetData>" + mergeCellsElement;

    return ws;
}
function getHeaderNames(dt) {
    // Gets header names.
    //params:F
    //  dt: Datatable API.
    //Returns:
    //  Array of column header names.

    var header = dt.columns().header().toArray();

    var names = [];
    header.forEach(function (th) {
        names.push($(th).html());
    });

    return names;
}
function buildCols(data) {
    // Builds cols XML.
    //To do: deifne widths for each column.
    //Params:
    //  data: row data.
    //Returns:
    //  String of XML formatted column widths.

    var cols = "<cols>";

    for (i = 0; i < data.length; i++) {
        colNum = i + 1;
        cols +=
            '<col min="' +
            colNum +
            '" max="' +
            colNum +
            '" width="20" customWidth="1"/>';
    }

    cols += "</cols>";

    return cols;
}
function buildRow(data, rowNum, styleNum) {
    // Builds row XML.
    //Params:
    //  data: Row data.
    //  rowNum: Excel row number.
    //  styleNum: style number or empty string for no style.
    //Returns:
    //  String of XML formatted row.
    var style = styleNum ? ' s="' + styleNum + '"' : "";
    var row = '<row r="' + rowNum + '">';

    for (i = 0; i < data.length; i++) {
        colNum = (i + 10).toString(36).toUpperCase(); // Convert to alpha

        var cr = colNum + rowNum;
        row +=
            '<c t="inlineStr" r="' +
            cr +
            '"' +
            style +
            ">" +
            "<is>" +
            "<t>" +
            data[i] +
            "</t>" +
            "</is>" +
            "</c>";
    }

    row += "</row>";

    return row;

}
function setSheetName(xlsx, name) {
    // Changes tab title for sheet.
    //Params:
    //  xlsx: xlxs worksheet object.
    //  name: name for sheet.

    if (name.length > 0) {
        var source = xlsx.xl["workbook.xml"].getElementsByTagName("sheet")[0];
        source.setAttribute("name", name);
    }
}