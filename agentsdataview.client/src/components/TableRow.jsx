import { Fragment, useRef, useState } from "react";
import { Button, OverlayTrigger } from "react-bootstrap";
import { BsTrash, BsEyeFill, BsLink45Deg, BsPaperclip,  BsClipboard2CheckFill } from "react-icons/bs";
import { Link } from "react-router-dom";
import { getTextWidth } from "../utils/commonfunctions";
import useIsMobile from "../utils/useIsMobile";

const TableRow = ({ row, onclick, cols, indx, buttons = [], onDoubleClick, forceRowClickInMobile, onShowTooltip }) => {
    const [dispalyDetail, setDisplay] = useState(false);
    const buttonClicked = useRef(false);

    
    const isMobile = useIsMobile();
const isWeb = !isMobile;
    const rowNumber = cols.find(c => c.colName?.toLowerCase() === "rownumber");
    const visibleCols = cols.filter(c => c.colName?.toLowerCase() !== "rownumber");
    const mobileCols = visibleCols.filter(c => c.showInMobile);
    const webCols = visibleCols.filter(c => !c.showInMobile);

    const handleRowClick = (e) => {
        if (e.target.tagName.toLowerCase() !== "td" || buttonClicked.current) {
            buttonClicked.current = false;
            return;
        }
        if (isMobile) setDisplay(prev => !prev);
        if (isWeb || forceRowClickInMobile) onclick?.({ ...row, DataIndex: row.RowIndex, RowIndex: indx, ctrlIsDown: e.ctrlKey, target: e.target });
    };

    const handleColChange = (e, fn) => {
        const colName = e.target.getAttribute("colname");
        row[colName] = e.target.checked;
        fn?.({ e, ...row })
    };
    const handleDoubleClick = () => onDoubleClick?.({ ...row, RowIndex: indx });

    const getColData = (row, colName, seperator = "-") => {
        return colName.split("+").map(cn => row[cn]?.toString().trim()).filter(Boolean).join(` ${seperator} `);
    };

    const getCellContent = (item) => {
        const value = row[item.colName] ?? "";
        switch (item.colType) {
            case "currency": return (!value ? 0 : (item.showNegative ? value : Math.abs(value))).toLocaleString(navigator.language);
            case "boolean": return <input type="checkbox" className="tableRowColCheck form-check-input" defaultChecked={value} disabled={!item.editable} onChange={e => handleColChange(e, item.onChange)} colname={item.colName} />;
            case "number": return value;
            case "string": return item.colName.includes("+") ? getColData(row, item.colName, item.seprator) : isWeb ?( value?.substring(0, item.maxTextLength) + (value?.length > item.maxTextLength ? "..." : "")):value;
            case "color": return <input type="color" readOnly value={value} disabled style={{ padding: 0, border: "none", width: "100%", minWidth: "20px" }} />;
            case "calc": return item.valueFunction?.(row);
            case "img": return <img src={`data:image/png;base64,${value}`} width={100} alt="بدون تصویر" onClick={item.onClick ? () => item.onClick(row) : undefined} />;
            default: return value;
        }
    };

    const renderCell = (item, i) => {
        const content = getCellContent(item);
        const value = row[item.colName];

        return (item.showCondition === undefined || item.showCondition(row)) && (isWeb || (isMobile && item.showInMobile)) && (
            <td
                key={item.colName || i}
                style={item.style ?? item.styleCondition?.(row)}
                //className={item.hasTooltip ? "tooltipCol" : ""}
                colname={item.colName}
                onMouseEnter={e => showTdTooltip({ e, ...item })}
                onMouseLeave={handleMouseLeave}
            >
                {content}
                {item.hasTooltip && value && item.maxTextLength < value.length && <button
                    onClick={() => {
                        navigator.clipboard.writeText(value);

                    }}
                    style={{
                        background: "transparent",
                        border: "none",
                        color: "#fff",
                        cursor: "pointer",
                        fontSize: "14px",
                        outline: "none"
                    }}
                    title="کپی"
                >
                    <BsClipboard2CheckFill color="#ff5700" />
                </button>}
                {item.button && (item.button.showCondition === undefined || item.button.showCondition(row)) && (
                    <button className={(item.button.className ?? "") + " table-row-button"} onClick={() => item.button.onClick?.(row)}>
                        {item.button.icon}{item.button.caption}
                    </button>
                )}
            </td>
        );
    };

    const renderButtons = () => buttons.filter(b => b.showCondition === undefined || b.showCondition(row)).map((b, i) => {
        const handleClick = (e) => { buttonClicked.current = true; b.onclick?.({ ...row, e }); };
        const btnProps = {
            className: "row-btn" + (isMobile && b.className ? " " + b.className : ""),
            style: { marginRight: "3px", ...(b.style || {}) },
            onClick: handleClick
        };
        if (isMobile) {
            return <Button {...btnProps} key={i}>{b.title}</Button>
        }
        switch (b.type) {
            case "delete": return <BsTrash {...btnProps} color="#ff2100" size={18} key={i} />;
            case "show": return <BsEyeFill {...btnProps} color="#9377d1" size={18} key={i} />;
            case "attach": return <BsPaperclip {...btnProps} color="blue" size={18} key={i} />;
            case "related": return <BsLink45Deg {...btnProps} color="blue" size={18} key={i} />;
            case "common": return (<button {...btnProps} className={"btn btn-row-icon"} data-tooltip={b.tooltip} onMouseEnter={handleMouseEnter}
                onMouseLeave={handleMouseLeave}>{b.icon}</button>);
            case "overlay": return (
                <OverlayTrigger key={i} overlay={b.overlay} placement={b.placement ?? "right"} rootClose={b.rootClose ?? true} trigger="click">
                    <button {...btnProps} className={"btn"} data-tooltip={b.tooltip} onMouseEnter={handleMouseEnter}
                        onMouseLeave={handleMouseLeave}>{b.icon}</button>
                </OverlayTrigger>
            );
            case "link": return <Link key={i} to={b.to ?? ""} onClick={handleClick}>{b.icon}</Link>;
            default: return <Button {...btnProps} key={i}>{b.title}</Button>;
        }
    });
    const handleMouseEnter = (e) => {
        const rect = e.currentTarget.getBoundingClientRect();
        const tooltip = e.currentTarget.getAttribute("data-tooltip");
        if(!(tooltip?.length > 0)){
            return;
        }
        let tooltipWidth = getTextWidth(tooltip, "10px Vazir");
        if (tooltipWidth > 300) {
            tooltipWidth = 300;
        }
        e.currentTarget.closest("table")
        let x = rect.left + rect.width / 2;
        const tblLeft = e.currentTarget.closest(".table-responsive").getBoundingClientRect().x;
        //برای اینکه tooltip از صفحه بیرون نرود
        if (x + tooltipWidth / 2 > window.innerWidth) {
            x = window.innerWidth - tooltipWidth / 2;
        }
        // if(Math.abs(x-tooltipWidth /2) < tblLeft)
        // {
        //     x = tblLeft;
        // }

        if (x - tooltipWidth / 2 < 0) {
            x = tooltipWidth / 2;
        }

        onShowTooltip({
            text: tooltip,
            x,
            y: rect.top
        });
    };
    const showTdTooltip = (item) => {
        const value = row[item.colName];
        const e = item.e;
        if (!item.hasTooltip || !value || item.maxTextLength > value.length) {
            return;
        }
        const rect = e.currentTarget.getBoundingClientRect();
        const tooltipWidth = e.currentTarget.closest("td").offsetWidth;
        if(!(tooltipWidth> 0)){
            return;
        }        
        let x = rect.left + rect.width / 2;
        //برای اینکه tooltip از صفحه بیرون نرود
        if (x + tooltipWidth / 2 > window.innerWidth) {
            x = window.innerWidth - tooltipWidth / 2;
        }

        if (x - tooltipWidth / 2 < 0) {
            x = tooltipWidth / 2;
        }

        onShowTooltip({
            text: value,
            x,
            y: rect.top + 5,
            width: tooltipWidth,
            showCopy: true
        });
    };
    const handleMouseLeave = () => {
        onShowTooltip(null);
    };
    return (
        <Fragment key={indx}>
            <tr className="result-table-row" onClick={handleRowClick} onDoubleClick={handleDoubleClick} data-tooltip="">
                {rowNumber && <td style={{ width: "40px", backgroundColor: "#b1baceff", borderRight: "1px solid #b3b3bb" }} colname="rowNumber">{indx + 1}</td>}
                {visibleCols.map(renderCell)}
                {isWeb && buttons.length > 0 && <td className="auto-width-col">{renderButtons()}</td>}
            </tr>
            {isMobile && (
                <tr style={{ display: dispalyDetail ? "table-row" : "none" }}>
                    <td colSpan={mobileCols.length + (rowNumber ? 1 : 0)} style={{ border: "1px solid #a7aab6", borderRadius: "2px" }}>
                        <div className="row">
                            {webCols.map((item, i) => (item.showCondition === undefined || item.showCondition(row)) && (
                                <Fragment key={i}>
                                    <div className="col-5 mt-2">{item.colTitle}</div>
                                    <div className="col-7 mt-2" style={{ direction: item.colType === "currency" ? "ltr" : "rtl", overflow: "auto",whiteSpace:"normal", textAlign:"justify" }}>
                                        {getCellContent(item)}
                                    </div>
                                </Fragment>
                            ))}
                        </div>
                        <div className="row" style={{ padding: "5px 10px" }}>
                            <div className="col-md-4">{renderButtons()}</div>
                        </div>
                    </td>
                </tr>
            )}
        </Fragment>
    );
};

export default TableRow;