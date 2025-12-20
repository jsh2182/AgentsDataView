import * as XLSX from "xlsx";
import { saveAs } from "file-saver";
import { renderToStaticMarkup } from "react-dom/server";
import { MdPrint } from "react-icons/md";
export function printTable(
  data,
  columns,
  printFilterKeys,
  fontSize,
  printIconSvg
) {
  const printWindow = window.open("", "_blank");

  if (!printWindow) {
    console.error(
      "پنجره چاپ باز نشد. احتمالاً مرورگر پاپ‌آپ را بلاک کرده است."
    );
    return;
  }

  const doc = printWindow.document;

  // ساخت head
  const head = doc.createElement("head");
  const metaCharset = doc.createElement("meta");
  metaCharset.setAttribute("charset", "UTF-8");

  const title = doc.createElement("title");
  title.textContent = "چاپ جدول";

  // const fontLink = doc.createElement("link");
  // fontLink.rel = "stylesheet";
  // fontLink.href = "/fonts/vazirmatn/font.css";
  const fontFace = `
@font-face {
  font-family: "Vazirmatn";
  src: url("/fonts/vazirmatn/Vazirmatn-Thin.woff2") format("woff2");
  font-weight: 100;
  font-style: normal;
  font-display: swap;
}

@font-face {
  font-family: "Vazirmatn";
  src: url("/fonts/vazirmatn/Vazirmatn-ExtraLight.woff2") format("woff2");
  font-weight: 200;
  font-style: normal;
  font-display: swap;
}

@font-face {
  font-family: "Vazirmatn";
  src: url("/fonts/vazirmatn/Vazirmatn-Light.woff2") format("woff2");
  font-weight: 300;
  font-style: normal;
  font-display: swap;
}

@font-face {
  font-family: "Vazirmatn";
  src: url("/fonts/vazirmatn/Vazirmatn-Regular.woff2") format("woff2");
  font-weight: 400;
  font-style: normal;
  font-display: swap;
}

@font-face {
  font-family: "Vazirmatn";
  src: url("/fonts/vazirmatn/Vazirmatn-Medium.woff2") format("woff2");
  font-weight: 500;
  font-style: normal;
  font-display: swap;
}

@font-face {
  font-family: "Vazirmatn";
  src: url("/fonts/vazirmatn/Vazirmatn-SemiBold.woff2") format("woff2");
  font-weight: 600;
  font-style: normal;
  font-display: swap;
}

@font-face {
  font-family: "Vazirmatn";
  src: url("/fonts/vazirmatn/Vazirmatn-Bold.woff2") format("woff2");
  font-weight: 700;
  font-style: normal;
  font-display: swap;
}

@font-face {
  font-family: "Vazirmatn";
  src: url("/fonts/vazirmatn/Vazirmatn-ExtraBold.woff2") format("woff2");
  font-weight: 800;
  font-style: normal;
  font-display: swap;
}

@font-face {
  font-family: "Vazirmatn";
  src: url("/fonts/vazirmatn/Vazirmatn-Black.woff2") format("woff2");
  font-weight: 900;
  font-style: normal;
  font-display: swap;
}
@media print {
  table {
    width: 100%; /* جدول کل عرض صفحه چاپ رو بگیره */
    /*table-layout: auto;*/ /* اجازه بده عرض ستون‌ها خودکار تنظیم بشه */
    word-wrap: break-word; /* اگر متنی طولانیه بشکنه به خط بعد */
      table-layout: fixed; /* ستون‌ها با عرض ثابت */
      font-size: 1.8vw;
  }
  body {
    font-family: "Vazirmatn", Arial, sans-serif !important;
    line-height: 1.5;
    font-weight: 400;
    font-feature-settings: "ss01";
    font-synthesis: none;
    text-rendering: optimizeLegibility;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    direction: rtl;
  }
  .no-print {
      display: none !important;
  }
}

`;
  const style = doc.createElement("style");
  style.textContent = `
  ${fontFace}
    body {
      font-family: 'Vazirmatn' !important;
      font-feature-settings: "ss01";
      direction: rtl;
      margin: 20px;
      color: #000;
      font-size:${fontSize || "unset"}
    }

    table {
      width: 100%;
      border-collapse: collapse;
      margin-top: 20px;
    }

    th, td {
      border: 1px solid #999;
      padding: 8px;
      text-align: right;
      vertical-align: top;
      word-wrap: break-word;
      white-space: pre-wrap;
      font-size:${fontSize || "unset"}
    }

    tr:nth-child(even) {
      background-color: #f9f9f9;
    }

    strong {
      color: #333;
    }
  `;

  head.appendChild(metaCharset);
  head.appendChild(title);
  //head.appendChild(fontLink);
  head.appendChild(style);
  doc.head.replaceWith(head);

  // ساخت body
  const body = doc.createElement("body");

  // فیلترها
  const printDiv = doc.createElement("div");
  printDiv.onclick = () => printWindow.print();
  printDiv.innerHTML = printIconSvg;
  printDiv.className = "no-print";
  body.appendChild(printDiv);
  const filterContainer = doc.createElement("div");
  filterContainer.style.display = "flex";
  if (printFilterKeys?.length > 0) {
    printFilterKeys.forEach((f) => {
      const p = doc.createElement("p");
      p.style.padding = "5px";
      p.style.border = "1px solid gray";
      p.style.margin = "1px";
      //const f = printFilterKeys[key];
      p.innerHTML = `<strong>${f.label}:</strong> ${f.value}`;
      filterContainer.appendChild(p);
    });
  }
  body.appendChild(filterContainer);

  // جدول
  const table = doc.createElement("table");
  const thead = doc.createElement("thead");
  thead.style.backgroundColor = "#e9e4e4";
  const trHead = doc.createElement("tr");
  const th = doc.createElement("th");
  th.textContent = "";
  th.style.width = "1px";
  trHead.appendChild(th);
  columns.forEach((col) => {
    const th = doc.createElement("th");
    th.textContent = col.label;
    trHead.appendChild(th);
  });

  thead.appendChild(trHead);
  table.appendChild(thead);

  const tbody = doc.createElement("tbody");
  let i = 0;
  data.forEach((row) => {
    const tr = doc.createElement("tr");
    if (i % 2 === 1) {
      tr.style.background = "#efefef";
    }
    i++;
    const tdRowNumber = doc.createElement("td");
    tdRowNumber.style.width = "1px";
    tdRowNumber.innerHTML = i;
    tr.appendChild(tdRowNumber);
    columns.forEach((col) => {
      const td = doc.createElement("td");
      const val =
        typeof col.render === "function"
          ? col.render(row[col.name], row)
          : row[col.name];
      if (col.type === "boolean") {
        td.innerHTML = val === "true" ? "☑" : "☐";
      } else {
        td.innerHTML = val ?? "";
      }
      tr.appendChild(td);
    });
    tbody.appendChild(tr);
  });

  table.appendChild(tbody);
  body.appendChild(table);
  doc.body.replaceWith(body);
  // منتظر لود فونت و استایل باش بعد پرینت کن
  printWindow.onload = () => {
    printWindow.focus();
    printWindow.print();
  };
}

export function exportToExcel(data, columns) {
  // ردیف هدر
  const wsData = [
    columns.map((c) => c.label),
    ...data.map((row) =>
      columns.map((c) => {
        let val = row[c.name];

        if (c.type === "boolean") {
          return val ? "☑" : "☐";
        }

        return val ?? "";
      })
    ),
  ];

  const worksheet = XLSX.utils.aoa_to_sheet(wsData);

  // تنظیم راست‌چین برای همه سلول‌ها
  const range = XLSX.utils.decode_range(worksheet["!ref"]);
  for (let R = range.s.r; R <= range.e.r; ++R) {
    for (let C = range.s.c; C <= range.e.c; ++C) {
      const cell_address = XLSX.utils.encode_cell({ r: R, c: C });
      const cell = worksheet[cell_address];
      if (cell) {
        cell.s = cell.s || {};
        cell.s.alignment = { horizontal: "right" };
      }
    }
  }

  const workbook = XLSX.utils.book_new();
  XLSX.utils.book_append_sheet(workbook, worksheet, "Sheet1");

  const wbout = XLSX.write(workbook, {
    bookType: "xlsx",
    type: "array",
    cellStyles: true,
  });

  const blob = new Blob([wbout], { type: "application/octet-stream" });
  saveAs(blob, "export.xlsx");
}
