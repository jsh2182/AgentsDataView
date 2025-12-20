import React, { useRef, useState } from "react";
import { Toast, Button, ToastContainer } from "react-bootstrap";

/**
 * @components
* @param {object} prop
 * @param {function} prop.setMessage
 * @param {object}  prop.message
 * @param {"error"|"info"|"warning"} prop.message.type
 * @param {string} prop.message.title
 * @param {string} prop.message.text
 * @param {int} prop.delay -default:3000
 * @param {bool} prop.canCopy
 * @returns 
 */
function AutoHideToast({ setMessage, message, delay = 3, canCopy = false }) {

    const bgColors = { error: "danger" };
    const bodyRef = useRef();
    const [copied, setCopied] = useState(false);
    const copyD = () => {
        const div = bodyRef.current;
        const text = div.innerText;
        navigator.clipboard.writeText(text)
            .then(() => {
                setCopied(true);
                setTimeout(() => {
                    setCopied(false);
                }, 5000);
            })
            .catch(err => {
                console.error('خطا در کپی کردن: ', err);
            });
    }
    const copyText = (text) => {
        navigator.clipboard.writeText(text);
    }
    if (!message?.text) {
        return (<></>)
    }
    return (

        <ToastContainer className="p-3" position="top-center">
            <Toast
                className="d-inline-block m-1"
                bg={bgColors[message.type]}
                style={{ width: "100%" }}
                onClose={() => setMessage(null)}
                show={message}
                delay={delay * 1000}
                autohide
            >
                <Toast.Header closeButton={true} className="toast-header">
                    <strong className="ms-auto">{message.title}</strong>
                </Toast.Header>
                <Toast.Body className="toast-body text-white">
                    <div ref={bodyRef}>
                        {
                            message.text.split("\\n").map((p, i) => <div key={i} onClick={() => copyText(p)}>{p}</div>)
                        }
                    </div>
                    {!copied && canCopy &&
                        <Button variant="link" onClick={copyD} className="me-auto" style={{ fontSize: "12px" }}>کپی</Button>}
                </Toast.Body>
            </Toast>
        </ToastContainer>
    )
}

export default AutoHideToast;
