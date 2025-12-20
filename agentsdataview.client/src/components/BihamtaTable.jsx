// import { Fragment, useEffect,  useRef, useState } from "react";
// import TableRow from "./TableRow";
// import { Table } from "react-bootstrap";
// import { useSelector } from "react-redux";
// import { ClipLoader } from "react-spinners";
// import { BsCaretLeftFill, BsCaretRightFill, BsSkipForwardFill, BsSkipBackwardFill } from "react-icons/bs";
// import useIsMobile from "../utils/useIsMobile";
// /**
//  * apiUrl When You Want Fetch Data With Url Directly From Server,
//  * apiMethod: Get/POST,
//  * getData: For When You Have Method For Getting Data And Updating State,
//  * reducerName: Name Of Reducer Affected By Your Method,
//  * objectName: Object Have Been Filled By Dispatcher,
//  * filterData Is An JSON Object,
//  **/
// /**
//  * @component
//  * @param {object} props 
//  * @param {[{colName:string, colTitle:string, colType:'boolean' | 'currency' | 'string' | 'number' |'color' |'calc' |'img', showInMobile:boolean}]} props.columns
//  * @param {function} props.searchMethod
//  * @param {object} props.filterData
//  * @param {[{title:string, type:string, tooltip:string, className:string, onClick:function, icon:JSX.Element}]} props.rowButtons
//  * @param {string} props.reducerName -برای وقتی که دیتاسورس state است
//  * @param {string} props.objectName -نام آبجکتی از reducer که دیتا را در خود دارد
//  * @param {function} props.rowClick
//  * @param {function} props.rowDoubleClick
//  * @param {object} props.dataList -برای وقتی که دیتاسورس یک فهرست مستقیم از داده است
//  * @param {boolean} props.hasPaging
//  * @param {ref} props.tableRef
//  * @param {boolean} props.forceRowClickInMobile -برای اینکه مجبور کنیم توی حالت موبایل علاوه بر باز کردن ردیف عملیاتی را هم انجام دهد
//  * @param {object} props.style
//  * @param {string} props.className
//  * @param {boolean} props.searchable -اگر برابر با true باشد کادر جستجو در اولین ردیف جدول نمایش داده می شود
//  * @param {int} props.maxHeight 
//  * @param {[int]} props.pageSizeList
//  * @returns JSX.Element
//  */
// export default function BihamtaTable(props) {
//     const { columns, searchMethod, filterData, rowButtons, reducerName, objectName, rowClick, rowDoubleClick, dataList, hasPaging, tableRef,
//         forceRowClickInMobile, style, className, searchable, maxHeight, pageSizeList } = props;

//     const isMobileApp = useIsMobile();
//     const isWebApp = !isMobileApp;
//     const rowNumber = columns.find(c => c.colName?.toLowerCase() === "rownumber");
//     const searchColSpan = columns.filter(c => c.showCondition === undefined || c.showCondition() === true).length;
//     const altTableRef = useRef();
//     const currPage = useRef(1);
//     const totalPage = useRef(0);
//     const resultList = useRef({});
//     const [fetchedData, onGetData] = useState([]);
//     const [searchError, setSearchError] = useState(null);
//     const [selectAllChecked, setSelectAllCheck] = useState(false);
//     const [tooltip, setTooltip] = useState(null);
//     const list = useSelector(state => state && reducerName ? state[reducerName][objectName] : []);
//     if (reducerName && objectName) {
//         resultList.current = list;
//     }
//     else {
//         resultList.current = fetchedData;
//     }
//     if (resultList.current)
//         if (hasPaging && filterData && (filterData.Skip === 0 || totalPage.current < 1)) {
//             const take = Number(filterData?.Take ?? 10);
//             currPage.current = 1;
//             const t = Number(resultList.current?.TotalCount ?? 0);
//             totalPage.current = take > 0 ? Math.floor(t / take) + ((t % take) > 0 ? 1 : 0) : 0;
//         }
//     function changePage(inc) {
//         if (inc) {
//             currPage.current++;
//         }
//         else {
//             currPage.current--;
//         }
//         if (filterData) {
//             filterData.Skip = currPage.current - 1;
//         }
//         if (searchMethod) {
//             if (!reducerName) {
//                 searchMethod(filterData, onGetData);
//             }
//             else {
//                 searchMethod(filterData);
//             }
//         }
//     }
//     function gotoFirstPage() {
//         currPage.current = 1;
//         if (filterData) {
//             filterData.Skip = currPage.current - 1;
//         }
//         if (searchMethod) {
//             if (!reducerName) {
//                 searchMethod(filterData, onGetData);
//             }
//             else {
//                 searchMethod(filterData);
//             }
//         }
//     }
//     function gotoLastPage() {
//         currPage.current = totalPage.current;
//         if (filterData) {
//             filterData.Skip = currPage.current - 1;
//         }
//         if (searchMethod) {
//             if (!reducerName) {
//                 searchMethod(filterData, onGetData);
//             }
//             else {
//                 searchMethod(filterData);
//             }
//         }
//     }
//     function tableClassName() {
//         let cName = className ?? "";
//         if (!className?.includes("mt-") && !maxHeight) {
//             cName += " mt-2";
//         }
//         if (maxHeight) {
//             cName += " fixedHeightTable mt-0 mb-0";
//         }
//         return cName;
//     }
//     function findInTable(e) {
//         const table = tableRef?.current ?? altTableRef.current;
//         const text = e.target.value.toLowerCase();
//         for (let i = 1; i < table.rows.length; i++) {
//             const r = table.rows[i];
//             if (r.id === "tableRowSearch" || text === "" || r.innerText.toLowerCase().includes(text)) {
//                 r.style.display = "table-row";
//             }
//             else {
//                 r.style.display = "none";
//             }

//         }
//     }
//     function handleSelectAll(e, fn) {
//         setSelectAllCheck(e.target.checked);
//         fn?.(e);
//     }
//     function onPageSizeChange(e) {
//         const filters = { ...filterData, Take: e.target.value };
//         if (searchMethod) {
//             if (!reducerName) {
//                 searchMethod(filters, onGetData);
//             }
//             else {
//                 searchMethod(filters);
//             }
//         }
//     }
//     useEffect(() => {
//         const colName = columns.find(c => c.selectAll)?.colName;
//         if (colName && dataList?.data) {
//             if (dataList.data.some(d => !JSON.parse(d[colName] ?? "false"))) {
//                 setSelectAllCheck(false);
//             }
//             else if (!selectAllChecked) {
//                 setSelectAllCheck(true);
//             }
//         }
//         if (hasPaging || (!reducerName && dataList && !(filterData?.Skip))) {
//             onGetData(dataList);
//         }
//         if (reducerName && filterData?.Skip && Number(currPage.current) - 1 !== Number(filterData.Skip)) {
//             currPage.current = filterData.Skip + 1;
//             searchMethod(filterData);
//         }
//     }, [dataList])
//     useEffect(() => {
//         if (Number(maxHeight) > 0) {
//             const table = tableRef?.current ?? altTableRef.current;
//             const wrapper = table?.closest('.table-responsive');
//             if (wrapper) {
//                 wrapper.style.setProperty('--tableMaxHeight', `${maxHeight}px`);
//             }
//         }
//     }, [maxHeight]);
//     return (
//         <Fragment>
//             {tooltip && (
//                 <div className="table-tooltip"
//                     style={{
//                         top: tooltip.y,
//                         left: tooltip.x,
//                         "--maxWidth": tooltip.width
//                     }}
//                 >
//                     <span style={{ flex: 1 }}>{tooltip.text}</span>
//                 </div>)}

//             <Table className={tableClassName()} responsive={true} ref={tableRef ?? altTableRef} style={{ ...style, borderCollapse: "separate", borderSpacing: "unset" }} /*datasource={resultList.current?.Data}*/>
//                 <thead>
//                     <tr>
//                         {rowNumber &&
//                             <th title={resultList.current?.TotalCount ?? 0} style={{
//                                 position: maxHeight ? "sticky" : "unset", top: 0, backgroundColor: "#b6b6b6", zIndex: 50,
//                                 paddingLeft: pageSizeList?.length > 0 ? "2px" : "unset",
//                                 paddingRight: pageSizeList?.length > 0 ? "2px" : "unset",
//                                 borderBottomColor: (resultList?.current?.data?.length > 0 && !searchable ? "#dfdcdc" : "unset")
//                             }}>
//                                 {pageSizeList?.length > 0 && searchMethod ?
//                                     <select onChange={onPageSizeChange} defaultValue={filterData?.Take ?? pageSizeList[0]}
//                                         style={{ padding: "0", maxWidth: "max-content", textAlign: "center", outline: "none", backgroundColor: "transparent", borderRadius: "3px", border: "1px solid #434040" }}>
//                                         {pageSizeList.map((ps, i) => <option value={ps} key={i}>{ps}</option>)}
//                                     </select> :
//                                     rowNumber.colTitle}
//                             </th>}
//                         {columns && columns.filter(c => c.colName?.toLowerCase() !== "rownumber").map((c, i) =>
//                             (c.showCondition === undefined || c.showCondition() === true) &&
//                             (isWebApp || (isMobileApp && c.showInMobile)) &&
//                             <th key={i} style={{ position: maxHeight ? "sticky" : "unset", top: 0, backgroundColor: "#b6b6b6", borderBottom: "1px solid", zIndex: 50, ...c.headerStyle }}>
//                                 {c.selectAll && c.colType === "boolean" && Array.isArray(resultList.current?.data) && resultList.current.data.length > 0 &&
//                                     <Fragment>
//                                         <input type="checkbox" className="form-check-input" checked={selectAllChecked} onChange={(e) => handleSelectAll(e, c.selectAll)} style={{ marginTop: "1px", borderColor: "#858181" }} />&nbsp;
//                                     </Fragment>
//                                 }
//                                 {c.colTitle}
//                             </th>
//                         )}
//                         {isWebApp && rowButtons?.length > 0 && <th style={{ position: maxHeight ? "sticky" : "unset", top: 0, backgroundColor: "#b6b6b6", zIndex: 50 }}>&nbsp;</th>}
//                     </tr>
//                 </thead>
//                 <tbody>
//                     {resultList.current === "PENDING" &&
//                         <tr>
//                             <td colSpan={columns.length + (rowNumber ? 1 : 0) + (rowButtons?.length > 0 ? 1 : 0)} style={{ textAlign: "center" }}>
//                                 <ClipLoader color="#bb1370" size={30} />
//                             </td>
//                         </tr>}
//                     {resultList.current?.Error &&
//                         <tr>
//                             <td colSpan={searchColSpan + (rowButtons?.length > 0 ? 1 : 0)} style={{ textAlign: "center", color: "red" }}>
//                                 {resultList.current.Error}
//                             </td>
//                         </tr>
//                     }
//                     {Array.isArray(resultList.current?.data) &&
//                         <Fragment>
//                             {isWebApp && searchable && resultList.current.data.length > 0 &&
//                                 <tr id="tableRowSearch" style={{ position: "sticky", top: 32, backgroundColor: "#F9EFEF", zIndex: 1 }}>
//                                     <td colSpan={searchColSpan + (rowButtons?.length > 0 ? 1 : 0)} style={{ padding: 3 }}>
//                                         <input className="tableRowInput" onChange={findInTable} placeholder="جستجو کنید" style={{ width: "200px" }} />
//                                     </td>
//                                 </tr>
//                             }
//                             {resultList.current.data.map((item, i) =>
//                                 <TableRow key={i} row={item} buttons={rowButtons} indx={(filterData?.Skip ?? 0) * (filterData?.Take ?? 0) + i}
//                                     cols={columns} onclick={rowClick} onDoubleClick={rowDoubleClick} forceRowClickInMobile={forceRowClickInMobile} onShowTooltip={setTooltip} />)}
//                         </Fragment>
//                     }
//                 </tbody>
//             </Table>
//             {totalPage.current > 0 && hasPaging &&
//                 <div style={{ textAlign: "center" }}>
//                     <button className="btn btn-block" style={{ padding: "2px", marginRight: "2px" }} title='صفحه نخست' disabled={currPage.current === 1} onClick={gotoFirstPage}><BsSkipForwardFill size={25} /></button>
//                     <button className="btn btn-block" style={{ padding: "2px", marginRight: "2px" }} title='صفحه قبل' disabled={currPage.current === 1} onClick={() => { changePage(false) }}><BsCaretRightFill size={25} /></button>
//                     <span>{currPage.current + " از " + totalPage.current}</span>{resultList.current?.TotalCount && <span style={{ color: "gray" }}>&nbsp;{`(${resultList.current.TotalCount})`}</span>}
//                     <button className="btn btn-block" style={{ padding: "2px", marginLeft: "2px" }} title='صفحه بعد' disabled={currPage.current === totalPage.current} onClick={() => changePage(true)}><BsCaretLeftFill size={25} /></button>
//                     <button className="btn btn-block" style={{ padding: "2px", marginLeft: "2px" }} title='آخرین صفحه' disabled={currPage.current === totalPage.current} onClick={gotoLastPage}><BsSkipBackwardFill size={25} /></button>
//                 </div>
//             }
//         </Fragment>
//     )
// }
// BihamtaTable.jsx
import React, { Fragment, useCallback, useEffect, useMemo, useRef, useState } from "react";
import { Table } from "react-bootstrap";
import { useSelector, shallowEqual } from "react-redux";
import { ClipLoader, MoonLoader } from "react-spinners";
import { BsCaretLeftFill, BsCaretRightFill, BsSkipForwardFill, BsSkipBackwardFill } from "react-icons/bs";
import useIsMobile from "../utils/useIsMobile";
import TableRow from "./TableRow";

/**
 * BihamtaTable - نسخه اصلاح‌شده و بهینه‌شده
 *
 * props:
 *  - columns
 *  - searchMethod
 *  - filterData
 *  - rowButtons
 *  - reducerName
 *  - objectName
 *  - rowClick
 *  - rowDoubleClick
 *  - dataList
 *  - hasPaging
 *  - tableRef
 *  - forceRowClickInMobile
 *  - style
 *  - className
 *  - searchable
 *  - maxHeight
 *  - pageSizeList
 */
export default function BihamtaTable(props) {
  const {
    columns = [],
    searchMethod,
    filterData,
    rowButtons = [],
    reducerName,
    objectName,
    rowClick,
    rowDoubleClick,
    dataList,
    hasPaging,
    tableRef,
    forceRowClickInMobile,
    style,
    className,
    searchable,
    maxHeight,
    pageSizeList = [],
  } = props;

  const isMobileApp = useIsMobile();
  const isWebApp = !isMobileApp;

  // ---------- Redux dynamic selector (memoized) ----------
  // اگر reducerName و objectName مشخص باشن، selector مقدار مورد نظر از state رو برمی‌گردونه
  const reduxSelector = useMemo(() => {
    if (!reducerName || !objectName) return () => undefined;
    // selector function (returns slice[field])
    return (state) => state?.[reducerName]?.[objectName];
  }, [reducerName, objectName]);

  const reduxData = useSelector(reduxSelector, shallowEqual);

  // ---------- local state when using searchMethod to fill data ----------
  const [fetchedData, setFetchedData] = useState(undefined);
  const onGetData = useCallback((d) => setFetchedData(d), []);

  // ---------- decide final data source ----------
  // priority:
  // 1) reduxData (if present)
  // 2) fetchedData (when searchMethod provides)
  // 3) dataList (prop)
  const finalData = useMemo(() => {
    // Accept different shapes: either array directly or { data: [...], TotalCount, Error } style
    // We keep whatever structure the upstream provides, but avoid creating new refs unnecessarily.
    if (reduxData !== undefined) return reduxData;
    if (fetchedData !== undefined) return fetchedData;
    if (dataList !== undefined) return dataList;
    return { data: [] };
  }, [reduxData, fetchedData, dataList]);

  // ---------- refs & paging ----------
  const altTableRef = useRef();
  const currPage = useRef(1);
  const totalPage = useRef(0);

  // compute total pages when finalData or filterData changes
  useEffect(() => {
    if (hasPaging && filterData) {
      const take = Number(filterData?.Take ?? 10);
      const totalCount = Number(finalData?.TotalCount ?? finalData?.totalCount ?? (Array.isArray(finalData?.data) ? finalData.data.length : 0));
      totalPage.current = take > 0 ? Math.floor(totalCount / take) + ((totalCount % take) > 0 ? 1 : 0) : 0;
      // If Skip provided, sync currPage
      if (Number(filterData?.Skip) >= 0) {
        currPage.current = Number(filterData.Skip) + 1;
      } else {
        currPage.current = 1;
      }
    }
  }, [finalData, filterData, hasPaging]);

  // ---------- other states ----------
  const [selectAllChecked, setSelectAllCheck] = useState(false);
  const [tooltip, setTooltip] = useState(null);
  const [searchError, setSearchError] = useState(null);

  // Check and sync selectAll if column has selectAll logic (run when data changes)
  useEffect(() => {
    const colName = columns.find((c) => c.selectAll)?.colName;
    const dataArr = Array.isArray(finalData?.data) ? finalData.data : [];
    if (colName && dataArr.length > 0) {
      // If any item is not selected -> uncheck; else check
      const someNotChecked = dataArr.some((d) => {
        try {
          return !JSON.parse(d[colName] ?? "false");
        } catch (e) {
          // if parsing fails, treat false
          return true;
        }
      });
      setSelectAllCheck(!someNotChecked && dataArr.length > 0);
    } else {
      setSelectAllCheck(false);
    }
  }, [finalData, columns]);

  // ---------- helpers ----------
  const changePage = useCallback(
    (inc) => {
      currPage.current = inc ? currPage.current + 1 : currPage.current - 1;
      if (filterData) {
        // avoid mutating original filterData object in caller if possible:
        // but to preserve original behavior, we set its Skip (if it's an object passed intentionally).
        try {
          filterData.Skip = currPage.current - 1;
        } catch (e) {
          // ignore immutability issues
        }
      }
      if (searchMethod) {
        if (!reducerName) {
          searchMethod(filterData, onGetData);
        } else {
          // if using redux searchMethod likely dispatches and updates redux slice
          searchMethod(filterData);
        }
      }
    },
    [filterData, searchMethod, reducerName, onGetData]
  );

  const gotoFirstPage = useCallback(() => {
    currPage.current = 1;
    if (filterData) {
      try { filterData.Skip = currPage.current - 1; } catch (e) { }
    }
    if (searchMethod) {
      if (!reducerName) searchMethod(filterData, onGetData);
      else searchMethod(filterData);
    }
  }, [filterData, searchMethod, reducerName, onGetData]);

  const gotoLastPage = useCallback(() => {
    currPage.current = totalPage.current || 1;
    if (filterData) {
      try { filterData.Skip = currPage.current - 1; } catch (e) { }
    }
    if (searchMethod) {
      if (!reducerName) searchMethod(filterData, onGetData);
      else searchMethod(filterData);
    }
  }, [filterData, searchMethod, reducerName, onGetData]);

  const onPageSizeChange = useCallback(
    (e) => {
      const newTake = Number(e.target.value);
      const filters = { ...(filterData ?? {}), Take: newTake, Skip: 0 };
      if (searchMethod) {
        if (!reducerName) searchMethod(filters, onGetData);
        else searchMethod(filters);
      }
    },
    [filterData, searchMethod, reducerName, onGetData]
  );

  const handleSelectAll = useCallback((e, fn) => {
    const checked = e.target.checked;
    setSelectAllCheck(checked);
    fn?.(e);
  }, []);

  const tableClassName = useCallback(() => {
    let cName = className ?? "";
    if (!className?.includes("mt-") && !maxHeight) {
      cName += " mt-2";
    }
    if (maxHeight) {
      cName += " fixedHeightTable mt-0 mb-0";
    }
    return cName;
  }, [className, maxHeight]);

  // const findInTable = useCallback((e) => {
  //   const table = tableRef?.current ?? altTableRef.current;
  //   if (!table) return;
  //   const text = e.target.value.toLowerCase();
  //   for (let i = 1; i < table.rows.length; i++) {
  //     const r = table.rows[i];
  //     if (!r) continue;
  //     let rowText = r.innerText.toLowerCase();
  //    const rowNumber = r.querySelector("[colname=rowNumber]");
  //    if(rowNumber)
  //    {
  //     rowText = rowText.replaceAll(rowNumber.innerText.toLowerCase(),"");
  //    }
  //     if (r.id === "tableRowSearch" || text === "" || rowText.includes(text)) {
  //       r.style.display = "table-row";
  //     } else {
  //       r.style.display = "none";
  //     }
  //   }
  // }, [tableRef]);
  const findInTable = useCallback((e) => {
    const table = tableRef?.current ?? altTableRef.current;
    if (!table) return;

    const text = e.target.value.toLowerCase();
    const rows = Array.from(table.rows).slice(1); // رد شدن از هدر جدول

    rows.forEach(r => {
      if (!r) return;

      let rowText = r.innerText.toLowerCase();
      const rowNumber = r.querySelector("[colname=rowNumber]");
      if (rowNumber) {
        rowText = rowText.replaceAll(rowNumber.innerText.toLowerCase(), "");
      }

      const show = r.id === "tableRowSearch" || text === "" || rowText.includes(text);
      r.style.display = show ? "table-row" : "none";
    });
  }, [tableRef]);

  // ---------- initial data sync when dataList prop changes (keeps previous behavior) ----------
  useEffect(() => {
    // If consumer passed dataList prop and we're not using redux, set fetchedData so table uses it
    if (!reducerName && dataList !== undefined) {
      setFetchedData(dataList);
    }
    // If using redux and filterData.Skip is provided, ensure current page matches
    if (reducerName && filterData?.Skip && Number(currPage.current) - 1 !== Number(filterData.Skip)) {
      currPage.current = Number(filterData.Skip) + 1;
      if (searchMethod) searchMethod(filterData);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [dataList]);

  // ---------- maxHeight styling side effect ----------
  useEffect(() => {
    if (Number(maxHeight) > 0) {
      const table = tableRef?.current ?? altTableRef.current;
      const wrapper = table?.closest?.('.table-responsive');
      if (wrapper) {
        wrapper.style.setProperty('--tableMaxHeight', `${maxHeight}px`);
      }
    }
  }, [maxHeight, tableRef]);

  // ---------- rendering ----------
  const dataArray = Array.isArray(finalData?.data) ? finalData.data : (Array.isArray(finalData) ? finalData : []);

  return (
    <Fragment>
      {tooltip && (
        <div
          className="table-tooltip"
          style={{
            top: tooltip.y,
            left: tooltip.x,
            "--maxWidth": tooltip.width
          }}
        >
          <span style={{ flex: 1 }}>{tooltip.text}</span>
        </div>
      )}

      <Table
        className={tableClassName()}
        responsive={true}
        ref={tableRef ?? altTableRef}
        style={{ ...style, borderCollapse: "separate", borderSpacing: "unset" }}
      >
        <thead>
          <tr>
            {columns.find(c => c.colName?.toLowerCase() === "rownumber") &&
              (() => {
                const rowNumber = columns.find(c => c.colName?.toLowerCase() === "rownumber");
                return (
                  <th
                    title={finalData?.TotalCount ?? finalData?.totalCount ?? 0}
                    style={{
                      position: maxHeight ? "sticky" : "unset",
                      top: 0,
                      backgroundColor: "#b1baceff",
                      zIndex: 50,
                      paddingLeft: pageSizeList?.length > 0 ? "2px" : "unset",
                      paddingRight: pageSizeList?.length > 0 ? "2px" : "unset",
                      borderBottomColor: (dataArray.length > 0 && !searchable ? "#dfdcdc" : "unset")
                    }}
                  >
                    {pageSizeList?.length > 0 && searchMethod ? (
                      <select
                        onChange={onPageSizeChange}
                        defaultValue={filterData?.Take ?? pageSizeList[0]}
                        style={{ padding: "0", maxWidth: "max-content", textAlign: "center", outline: "none", backgroundColor: "transparent", borderRadius: "3px", border: "1px solid #434040" }}
                      >
                        {pageSizeList.map((ps, i) => <option value={ps} key={i}>{ps}</option>)}
                      </select>
                    ) : rowNumber.colTitle}
                  </th>
                );
              })()
            }

            {columns && columns.filter(c => c.colName?.toLowerCase() !== "rownumber").map((c, i) =>
              (c.showCondition === undefined || c.showCondition() === true) &&
              (isWebApp || (isMobileApp && c.showInMobile)) &&
              <th key={i} style={{ position: maxHeight ? "sticky" : "unset", top: 0, backgroundColor: "#b1baceff", borderBottom: "1px solid", zIndex: 50, ...c.headerStyle }}>
                {c.selectAll && c.colType === "boolean" && dataArray.length > 0 &&
                  <Fragment>
                    <input type="checkbox" className="form-check-input" checked={selectAllChecked} onChange={(e) => handleSelectAll(e, c.selectAll)} style={{ marginTop: "1px", borderColor: "#858181" }} />&nbsp;
                  </Fragment>
                }
                {c.colTitle}
              </th>
            )}
            {isWebApp && rowButtons?.length > 0 && <th style={{ position: maxHeight ? "sticky" : "unset", top: 0, backgroundColor: "#b1baceff", zIndex: 50 }}>&nbsp;</th>}
          </tr>
        </thead>

        <tbody>
          {finalData === "PENDING" &&
            <tr>
              <td colSpan={columns.length + (columns.find(c => c.colName?.toLowerCase() === "rownumber") ? 1 : 0) + (rowButtons?.length > 0 ? 1 : 0)} style={{ textAlign: "center" }}>
                <div
                  style={{
                    display: "flex",
                    justifyContent: "center",
                    alignItems: "center",
                  }}
                >
                  <MoonLoader size={25} color="blue" />
                </div>
              </td>
            </tr>
          }

          {finalData?.Error &&
            <tr>
              <td colSpan={(columns.filter(c => c.showCondition === undefined || c.showCondition() === true).length) + (rowButtons?.length > 0 ? 1 : 0)} style={{ textAlign: "center", color: "red" }}>
                {finalData.Error}
              </td>
            </tr>
          }

          {Array.isArray(dataArray) && (
            <Fragment>
              {isWebApp && searchable && dataArray.length > 0 &&
                <tr id="tableRowSearch" style={{ position: "sticky", top: 32, backgroundColor: "#F9EFEF", zIndex: 1 }}>
                  <td colSpan={(columns.filter(c => c.showCondition === undefined || c.showCondition() === true).length) + (rowButtons?.length > 0 ? 1 : 0)} style={{ padding: 3 }}>
                    <input className="tableRowInput" onChange={findInTable} placeholder="جستجو کنید" style={{ width: "200px" }} />
                  </td>
                </tr>
              }

              {dataArray.map((item, i) =>
                <TableRow
                  key={i}
                  row={item}
                  buttons={rowButtons}
                  indx={(filterData?.Skip ?? 0) * (filterData?.Take ?? 0) + i}
                  cols={columns}
                  onclick={rowClick}
                  onDoubleClick={rowDoubleClick}
                  forceRowClickInMobile={forceRowClickInMobile}
                  onShowTooltip={setTooltip}
                />
              )}
            </Fragment>
          )}
        </tbody>
      </Table>

      {totalPage.current > 0 && hasPaging &&
        <div style={{ textAlign: "center" }}>
          <button className="btn btn-block" style={{ padding: "2px", marginRight: "2px" }} title='صفحه نخست' disabled={currPage.current === 1} onClick={gotoFirstPage}><BsSkipForwardFill size={25} /></button>
          <button className="btn btn-block" style={{ padding: "2px", marginRight: "2px" }} title='صفحه قبل' disabled={currPage.current === 1} onClick={() => { changePage(false) }}><BsCaretRightFill size={25} /></button>
          <span>{currPage.current + " از " + totalPage.current}</span>{finalData?.TotalCount && <span style={{ color: "gray" }}>&nbsp;{`(${finalData.TotalCount})`}</span>}
          <button className="btn btn-block" style={{ padding: "2px", marginLeft: "2px" }} title='صفحه بعد' disabled={currPage.current === totalPage.current} onClick={() => changePage(true)}><BsCaretLeftFill size={25} /></button>
          <button className="btn btn-block" style={{ padding: "2px", marginLeft: "2px" }} title='آخرین صفحه' disabled={currPage.current === totalPage.current} onClick={gotoLastPage}><BsSkipBackwardFill size={25} /></button>
        </div>
      }
    </Fragment>
  );
}
