import { useEffect, useState } from "react";
import { Modal } from "react-bootstrap";
import ProgressBar from "./ProgressBar";
import { useLazyGetSettingQuery } from "../store/setting/settingApi";

export default function ModalAdv() {
    const defaultShow = localStorage.getItem("AdvShowedUp")?.toString() !== "true";
    const [showAdv, setShowAdv] = useState(true);
    const [showCloseButton, setShowCloseButton] = useState(false);
    const [getSetting, { data: settings }] = useLazyGetSettingQuery();
    const handleClose = () => {
        //localStorage.setItem("AdvShowedUp", true);
        setShowAdv(false);
    }
    useEffect(() => {
        if (showAdv) {
            getSetting("AdvertisingLink");
            setTimeout(() => {
                setShowCloseButton(true);
            }, 5000)
        }
    }, [showAdv])
    return (
        <Modal size="lg" show={showAdv} onHide={showCloseButton ? handleClose : undefined} backdrop={true} centered style={{ backdropFilter: "blur(5px)" }}>
            {/* <Modal.Header className="header-style" closeButton={showCloseButton}>

            </Modal.Header> */}
            {/* <ProgressBar duration={8000} trigger={showAdv ? 1 : 0} color="#0d6efd" height={2} /> */}
            <Modal.Body className="p-0">
                <img src="/images/Adv.gif" style={{
                    width: "100%", objectFit: "cover",
                    objectPosition: "center"
                }} onClick={() => window.open(settings?.settingValue ?? "", "_blank")} />
            </Modal.Body>
            <ProgressBar duration={5000} trigger={showAdv ? 1 : 0} color="#0d6efd" height={4} />
        </Modal>
    )
}