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
    // console.log('updateSheet', groupName);
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
                rowNum++;
            }
            //Aqui se hacen los calculos para totalizar.
            if (data[dataSrc] = currentGroup) {
                totalHe += parseFloat(data[14]);
                horasLab += parseFloat(data[13]);
                montoHe += parseFloat(data[20]);
                monto100 += parseFloat(data[22]);
                thoras100 += parseFloat(data[21])
                //totales generales
                tgeneralHe += parseFloat(data[14]);
                tghorasLab += parseFloat(data[13]);
                tgmontoHe += parseFloat(data[20]);
                tgmonto100 += parseFloat(data[22]);
                tghoras100 += parseFloat(data[21]);
            }

            // If data is object based then it needs to be converted
            // to an array before sending to buildRow()

            // Dibuja todas las filas de la data.
            data[8] = data[8].replaceAll('&nbsp;', '');
            data[9] = data[9].replaceAll('&nbsp;', '');
            data[10] = data[10].replaceAll('&nbsp;', '');
            data[11] = data[11].replaceAll('&nbsp;', '');
            ws += buildRow([fil, data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[8],
                data[9], data[10], data[11], data[12], data[13], data[14], data[15], data[16], data[17], data[18],
                data[19], data[20], data[21], data[22]], rowNum, "", 51);

            rowNum++;
            //agrega la fila de grupo

            ws += buildRow([data[2], "", "", "", "", "", "", "", "", "", "", "", "",
            horasLab.toFixed(2), totalHe.toFixed(2), "", "", "", "", "", montoHe.toFixed(2), thoras100.toFixed(2),
            monto100.toFixed(2)], "", 51);

        });
        //Ultimo Grupo tenia problema que no salia.
        //rowNum++;
        //ws += buildRow([currentGroup,'','',total,'',totalSalary.toFixed(2)],"",51);

        rowNum++;
        ws += buildRow(["..."], "", 51);

        //Calculo de Total de la Hora
        rowNum++;
        ws += buildRow(["Total General: ", "", "", "", "", "", "", "", "", "", "", "", "", tghorasLab.toFixed(2),
            tgeneralHe.toFixed(2), "", "", "", "", "", tgmontoHe.toFixed(2), tghoras100.toFixed(2), tgmonto100.toFixed(2)], "", 51);
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