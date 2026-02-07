import { useEffect, useRef, useState } from "react";
import { BsSearch, BsX } from "react-icons/bs";
import { keyIsLetterOrNumber } from "../utils/commonfunctions";
export let clearAll = () => { };
const BihamtaMultiSelect = ({ dataSource, displayItem, valueItem, onItemCheckChanged, placeholder, defaultCheckedList, inputClassName, className, selectOne, inputStyle, labelStyle }) => {

    const container = useRef();
    const [selectedOptions, setSelectedOptions] = useState([]);
    const selectedText = selectedOptions.map(s => s.OptionText).join("، ");
    const [showOptions, setShowOptions] = useState(false);
    let [searchedText, setSearchedText] = useState('');
    function onCheckChanged(e) {
        const el = e.target;
        if (selectOne) {
            const option = { OptionText: el.getAttribute("text"), OptionValue: el.getAttribute("value"), Index: el.getAttribute("indx") };
            setSelectedOptions([option]);
            if (onItemCheckChanged) {
                onItemCheckChanged({ value: el.getAttribute("value"), index: el.getAttribute("indx"), text: el.getAttribute("text") });
            }
            setShowOptions(false);
        }
        else {
            if (el.checked) {
                const option = { OptionText: el.getAttribute("text"), OptionValue: el.getAttribute("value"), Index: el.getAttribute("indx") };
                selectedOptions.push(option);
                setSelectedOptions([...selectedOptions]);
            }
            else {
                setSelectedOptions(selectedOptions.filter(s => Number(s.Index) !== Number(el.getAttribute("indx"))));
            }
            if (onItemCheckChanged) {
                onItemCheckChanged({ checked: el.checked, value: el.getAttribute("value"), index: el.getAttribute("indx"), text: el.getAttribute("text") });
            }
        }

    }

    const clearAll = () => {

        container.current.querySelectorAll("input[type='checkbox']").forEach(el => {
            el.checked = false;
            const value = el.getAttribute("value");
            if (onItemCheckChanged && value) {
                onItemCheckChanged({ checked: el.checked, value: value, index: el.getAttribute("indx"), text: el.getAttribute("text") });
            }
        });
        setSelectedOptions([]);
    }
    const checkAll = (e) => {
        const options = [];
        container.current.querySelectorAll("input[type='checkbox']").forEach(el => {
            el.checked = true;
            const value = el.getAttribute("value");
            if (value) {

                const option = { OptionText: el.getAttribute("text"), OptionValue: value, Index: el.getAttribute("indx") };
                options.push(option);
                if (onItemCheckChanged) {
                    onItemCheckChanged({ checked: el.checked, value: value, index: el.getAttribute("indx"), text: el.getAttribute("text") });
                }
            }
        });
        setSelectedOptions(options);
    }
    function onItemClicked(e) {
        const div = e.target.parentNode;
        const chk = div.querySelectorAll(".form-check-input")[0];
        if (chk) {
            chk.click();
        }
    }
    function onSelectInputClick(e) {
        setShowOptions(!showOptions);
    }
    function fitLabels() {
        const labels = document.querySelectorAll('.auto-label');
        const tt = document.getElementById("tt");
        labels.forEach(label => {
            label.style.fontSize = '12px'; // Reset to base size
            const parent = label.parentElement;
            const parentWidth = tt.clientWidth;
            const radio = parent.querySelector('input[type="radio"]');
            const radioWidth = radio?.offsetWidth || 0;
            const gap = 2; // Same as CSS gap between radio and text
            const availableWidth = parentWidth - radioWidth - gap;
            const fullTextWidth = label.scrollWidth;
            if (fullTextWidth > availableWidth) {
                const scaleFactor = availableWidth / fullTextWidth;
                label.style.fontSize = `${12 * scaleFactor}px`;
            }
        });
    }
    function searchForText(e) {
        if (!showOptions) {
            return;
        }
        if (!(keyIsLetterOrNumber(e.keyCode, e.key) || e.keyCode === 8)) {
            return;
        }
        let txt = "";
        if (e.keyCode === 8) {
            txt = searchedText.slice(0, searchedText.length - 1);
        }
        else {
            txt = searchedText + e.key;
        }
        const selecteDivs = container.current.querySelectorAll("div.multi-select-item");
        setSearchedText(txt);
        selecteDivs.forEach(div => {
            div.style.display = "flex";
            if (!div.innerText.contains(txt)) {
                div.style.display = "none";
            }
        });
        clearTimeout();
        setTimeout(() => {
            setSearchedText("");
            selecteDivs.forEach(div => {
                div.style.display = "flex";
            })
        }, 5000);

    }
    useEffect(() => {
        if (showOptions) {
            // کمی صبر کنیم تا DOM آپدیت بشه و بعد اندازه بگیریم
            setTimeout(() => {
                fitLabels();
            }, 0);
        }
    }, [showOptions]);
    useEffect(() => {
        const handler = (event) => {
            if (!container.current?.contains(event.target)) {
                setShowOptions(false);
            }
        };
        document.addEventListener("click", handler);
        return () => {
            document.removeEventListener("click", handler);
        };
    }, []);
    useEffect(() => {
        if (defaultCheckedList?.length > 0) {
            // for(let i=0; i<defaultCheckedList.length; i++ )
            // {
            //     defaultCheckedList[i] = defaultCheckedList[i].toString();
            // }
            const options = []
            for (let i = 0; i < dataSource?.length; i++) {
                if (defaultCheckedList.some(c => c.toString() === dataSource[i][valueItem].toString())) {
                    const option = { OptionText: dataSource[i][displayItem], OptionValue: dataSource[i][valueItem], Index: i };
                    options.push(option);
                    onItemCheckChanged({ checked: true, value: dataSource[i][valueItem]?.toString(), text: dataSource[i][displayItem] });
                }
            }
            setSelectedOptions(options);
        }
    }, [defaultCheckedList])
    return (
        <div style={{ position: "relative" }} ref={container}>
            <input className={(className ?? "form-control") + " " + (inputClassName ?? "")} placeholder={placeholder} value={selectedText} readOnly onKeyDown={searchForText} onClick={onSelectInputClick} id="tt" style={{ paddingLeft: "20px", ...inputStyle }} />
            {selectedOptions?.length > 0 && !selectOne && <BsX size={25} color="#b10909" className="hoverIcon"
                style={{ position: "absolute", left: 2, border: "none", top: 8, background: "transparent", cursor: "pointer" }} onClick={clearAll} />}
            <div className="sc-thin" style={{
                position: "absolute", background: "whiteSmoke",
                width: "100%", padding: "5px", borderRadius: "2px", overflow: "auto", maxHeight: "287px",
                marginTop: "1px", borderTop: "1px solid #28285f",
                display: showOptions ? "block" : "none",
                boxShadow: "1px 1px 5px #28285f",
                zIndex: 52
            }}>
                {searchedText?.length > 0 &&
                    <label className="multi-select-item" style={{ display: "block", borderBottom: "1px gray solid" }}>
                        <BsSearch />&nbsp;{searchedText}</label>
                }
                {!selectOne &&
                    <div className="multi-select-item" style={{ display: "flex", alignItems: "center", gap: "2px", overflow: "hidden", borderBottom:"1px solid gray", paddingBottom:"2px" }}>
                        <input type="checkbox" className="form-check-input" style={{ flexShrink: 0 }} onChange={checkAll} />
                        <span className="auto-label" style={{ whiteSpace: "nowrap", cursor: "default", ...labelStyle, transformOrigin: "right center", overflow: "hidden", textOverflow: "ellipsis" }} onClick={checkAll}>انتخاب همه</span>
                    </div>
                }
                {Array.isArray(dataSource ?? "") && dataSource.map((item, i) =>
                    <div key={i} className="multi-select-item" style={{ display: "flex", alignItems: "center", gap: "2px", overflow: "hidden" }}>
                        <input type={selectOne ? "radio" : "checkbox"} name={selectOne ? "bihamtaRadio" : ""} className="form-check-input" value={item[valueItem]?.toString()} text={item[displayItem]} style={{ flexShrink: 0 }}
                            indx={i} onChange={onCheckChanged} defaultChecked={defaultCheckedList?.some(c => c.toString() === item[valueItem]?.toString())} />
                        <span className="auto-label" style={{ whiteSpace: "nowrap", cursor: "default", ...labelStyle, transformOrigin: "right center", overflow: "hidden", textOverflow: "ellipsis" }} onClick={onItemClicked}>{item[displayItem]}</span>
                    </div>)}
            </div>
        </div>
    )
}
export default BihamtaMultiSelect;