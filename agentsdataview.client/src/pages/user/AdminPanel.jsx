import { useEffect, useRef, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { logout } from "../../store/user/userSlice";
import { useCreateUserMutation, useDeleteUserMutation, useLazyFetchUserListQuery } from "../../store/user/userAPI";
import BihamtaTable from "../../components/BihamtaTable";
import { Button, FormGroup, Modal, Form, Row, Col, FormControl } from "react-bootstrap";
import { BsBoxArrowRight, BsBuilding, BsDashCircle, BsPersonPlus, BsPlusSquareFill } from "react-icons/bs";
import { useForm } from "react-hook-form";
import { BiCheck } from "react-icons/bi";
import Alert from 'react-bootstrap/Alert';
import { FaPersonCircleMinus } from "react-icons/fa6";
import { useCreateCompanyMutation, useLazyGetAllCompaniesQuery, useUpdateCompanyMutation } from "../../store/company/companiesApi";
import { useLazyGetAllProvincesQuery } from "../../store/province/provinceApi";
import { useLazyGetAllCitiesQuery } from "../../store/city/cityApi";
import { MoonLoader } from "react-spinners";
import { useLazyGetSettingQuery, useUpdateSettingMutation } from "../../store/setting/settingApi";
import { FaCheck } from "react-icons/fa";
import ModalAddEditUser from "../../components/Modals/ModalAddEditUser";

const AdminPanel = () => {
    const dispatch = useDispatch();
    const [getProvinceList, { data: provinceList, isFetching: loadingProvinceList }] = useLazyGetAllProvincesQuery();
    const [getCityList, { data: cityList, isFetching: loadingCityList }] = useLazyGetAllCitiesQuery();
    const [getUserList, { data: userList, isFetching: loadingUserList, error }] = useLazyFetchUserListQuery();

    const [deleteUser, { isLoading: deleteLoading, error: deleteError }] = useDeleteUserMutation();
    const [updateCompany, { isLoading: loadingUpdateComp, error: errorUpdateCompany, reset: resetUpdateCompany }] = useUpdateCompanyMutation();
    const [createCompany, { isLoading: loadingCreateCompany, error: errorCreateCompany, reset: resetCreateCompany }] = useCreateCompanyMutation();
    const [getCompanyList, { data: companyList, isLoading: loadingCompanyList, error: errorCompanyList }] = useLazyGetAllCompaniesQuery();
    const [getSetting, { data: settings }] = useLazyGetSettingQuery();
    const [updateSetting, { isLoading: loadingUpdateSetting, error: errorUpdateSetting }] = useUpdateSettingMutation();
    const advLink = useRef(settings?.settingValue ?? "");
    const currentUser = useSelector((state) => state.user.currentUser);
    const selectedUser = useRef();

    const [showModalDelete, setShowModalDelete] = useState(false);
    const [showModalCompany, setShowModalCompany] = useState(false);
    const [modalUserData, setModalUserData] = useState({ show: false });
    const columns = [
        { colTitle: "ردیف", colName: "RowNumber" },
        { colTitle: "نام کامل", colName: "userFullName", },
        { colTitle: "نام کاربری", colName: "userName" },
        { colTitle: "شماره همراه", colName: "userMobile" }
    ];

    const showDelete = (row) => {
        selectedUser.current = { ...row, e: undefined };
        setShowModalDelete(true);
    }
    const buttons = [{ type: "delete", onclick: showDelete }];
    const companyColumns = [
        { colTitle: "ردیف", colName: "RowNumber" },
        { colTitle: "کد شرکت", colName: "code" },
        { colTitle: "نام شرکت", colName: "name" },
        { colTitle: "شماره تماس", colName: "phoneNumber" },
        { colTitle: "استان", colName: "provinceName" },
        { colTitle: "شهر", colName: "cityName" },
        { colTitle: "نشانی", colName: "address", colType: "string" },
        { colTitle: "تاریخ آخرین ارسال", colName: "pMaxInvoiceCreationDate" },
        { colTitle: "آخرین تاریخ فاکتور", colName: "pMaxInvoiceDate" }
    ]

    const {
        register,
        handleSubmit,
        reset,
        formState: { errors },
    } = useForm();

    const prepareForAddComp = () => {
        //setCompanyInfo({});
        reset({ id: 0 });
        setTimeout(() => setShowModalCompany(true), 50);

    }



    const ModalCompanyInfo = () => {
        const handleSubmitCompany = async (data) => {
            if (Number(data.id) > 0) {
                await updateCompany(data).unwrap();
            }
            else {
                await createCompany(data).unwrap();

            }

            reset();
            resetCreateCompany();
            resetUpdateCompany();
            setShowModalCompany(false);
        }
        return (
            <Modal show={showModalCompany} variant="" animation={!showModalCompany} onHide={() => setShowModalCompany(false)} backdrop={false}>
                <Modal.Header className="header-style" closeButton >
                    <span>
                        <BsBuilding size={25} />&nbsp; اطلاعات شرکت
                    </span>
                    {/* <FiX size={25} onClick={() => setShowNewUser(false)} /> */}
                </Modal.Header>
                <Modal.Body>
                    {(errorUpdateCompany || errorCreateCompany) && <Alert variant="danger">{errorUpdateCompany || errorCreateCompany}</Alert>}
                    {showModalCompany &&
                        <form onSubmit={handleSubmit(handleSubmitCompany)} className="vstack gap-3 "
                        >
                            <input type="hidden" /*defaultValue={companyInfo?.id ?? 0}*/ {...register("id")} />
                            <FormGroup>
                                <label>نام شرکت:</label>
                                <Form.Control
                                    name="name"
                                    type="text"
                                    // defaultValue={companyInfo?.name ?? ""}
                                    placeholder="نام شرکت"
                                    {...register("name", { required: "نام شرکت الزامی است" })}
                                />
                                {errors?.name && <small className="text-danger">{errors.name.message}</small>}
                            </FormGroup>
                            <FormGroup>
                                <label>کد شرکت</label>
                                <Form.Control
                                    name="code"
                                    type="text"
                                    // defaultValue={companyInfo?.code ?? ""}
                                    placeholder="کد شرکت"
                                    {...register("code", { required: "کد شرکت الزامی است" })}
                                />
                                {errors?.code && <small className="text-danger">{errors.code?.message}</small>}
                            </FormGroup>
                            <FormGroup>
                                <label>شماره تماس: </label>
                                <Form.Control
                                    name="phoneNumber"
                                    type="number"
                                    // defaultValue={companyInfo?.phoneNumber ?? ""}
                                    placeholder="شماره تماس"
                                    {...register("phoneNumber")}
                                />
                                {errors?.phoneNumber && <small className="text-danger">{errors.phoneNumber?.message}</small>}
                            </FormGroup>
                            <FormGroup>
                                <label>استان: </label>
                                {loadingProvinceList ?
                                    <div className="form-control">
                                        <MoonLoader size={25} /></div> :
                                    <Form.Control as="select" //value={companyInfo?.provinceId ?? ""} 

                                        {...register("provinceId", {
                                            onChange: (e) => {
                                                getCityList(e.target.value);
                                            }
                                        })}

                                    >
                                        <option value="">استان را انتخاب کنید</option>
                                        {provinceList?.map(p => <option value={p.id} key={p.id}>{p.provinceName}</option>)}
                                    </Form.Control>

                                }

                            </FormGroup>
                            <FormGroup>
                                <label>شهر: </label>
                                {loadingCityList ?
                                    <div className="form-control">
                                        <MoonLoader size={25} />
                                    </div> :
                                    <Form.Control as="select" /*defaultValue={companyInfo?.cityId ?? ""}*/ {...register("cityId")}>
                                        <option value="">شهر را انتخاب کنید</option>
                                        {cityList?.map(p => <option value={p.id} key={p.id}>{p.cityName}</option>)}
                                    </Form.Control>

                                }

                            </FormGroup>
                            <FormGroup>
                                <label>نشانی: </label>
                                <Form.Control /*defaultValue={companyInfo?.address ?? ""}*/ {...register("address")} />
                            </FormGroup>
                            <Button variant="success" type="submit" disabled={loadingCreateCompany || loadingUpdateComp}><BiCheck size={18} />&nbsp;ثبت اطلاعات</Button>
                        </form>
                    }
                </Modal.Body>
                <Modal.Footer style={{ padding: 0 }}></Modal.Footer>
            </Modal>
        )
    }
    const ModalConfirmDelete = () => {
        const cancelDelete = () => {
            selectedUser.current = null;
            setShowModalDelete(false);
        }
        const handleDelete = async () => {
            if (!selectedUser.current) {
                return;
            }
            try {
                await deleteUser(selectedUser.current.id).unwrap(); // فراخوانی mutation
                selectedUser.current = null;
                setShowModalDelete(false);
            } catch (err) {
                console.error("خطا در حذف:", err);
            }

        };
        return (
            <Modal show={showModalDelete} closeButton onHide={cancelDelete} variant="danger">
                <Modal.Header className="header-style">
                    <FaPersonCircleMinus size={25} />&nbsp;حذف کاربر
                </Modal.Header>
                <Modal.Body>
                    {deleteError && <Alert variant="danger">{deleteError?.message ?? deleteError}</Alert>}
                    آیا از حذف کاربر {selectedUser.current?.userFullName} مطمئن هستید؟
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="success" onClick={handleDelete} disabled={deleteLoading}><BiCheck /> بله</Button>
                    <Button variant="danger" onClick={cancelDelete} disabled={deleteLoading}><BsDashCircle /> خیر</Button>
                </Modal.Footer>
            </Modal>)
    }
    const companyRowClick = (row) => {
        reset({ ...row, e: undefined });
        if (Number(row.provinceId) > 0)
            getCityList(row.provinceId);
        setShowModalCompany(true);
    }
    const handleLogout = () => {
        dispatch(logout());
    };
    const handleSubmitSettings = () => {
        updateSetting({ settingName: "AdvertisingLink", settingValue: advLink.current });
    }
    const showModalUser = () => {
        setModalUserData({ show: true });
    }
    const onUserRowClick = (row) => {
        const userInfo = { ...row, e: undefined, target: undefined };
        
        const orderedCompanyList = [...companyList].sort((a, b) => {
            const nameA = a.name.toUpperCase();
            const nameB = b.name.toUpperCase();
            if (nameA < nameB) {
                return -1;
            }
            if (nameA > nameB) {
                return 1;
            }

            return 0;
        });

    setModalUserData({ show: true, userInfo: userInfo, companyList: orderedCompanyList });
}
useEffect(() => {
    if (currentUser) {
        getCompanyList();
        getProvinceList(false);
        getUserList();
        getSetting("AdvertisingLink");

    }
}, [currentUser])
return (
    <div className="p-4 border rounded m-2">
        <ModalAddEditUser modalData={modalUserData} setModalData={setModalUserData} provinceList={provinceList} companyList={companyList}/>
        <ModalCompanyInfo />
        <ModalConfirmDelete />
        <div style={{ position: "relative" }}>
            <h2 className="text-lg font-bold mb-2" style={{ borderBottom: "1px solid #a8b3f5", paddingBottom: "0.3em" }}>پنل مدیریت</h2>
            <Button variant="outline-danger tooltip-container" style={{ position: "absolute", top: 0, left: 0, border: "none" }} onClick={handleLogout}>
                <BsBoxArrowRight size={25} />
                <span className="tooltip-text">خروج</span>
            </Button></div>
        <Row className="rounded" style={{ border: "1px solid gray" }}>
            <label className="w-100 p-1 mb-1" style={{ borderBottom: "1px solid gray" }}>فهرست شرکت ها</label>
            <div className="text-end">
                <Button variant="link" onClick={prepareForAddComp} className="p-0">
                    <BsPlusSquareFill size={28} />
                </Button>
            </div>
            <BihamtaTable columns={companyColumns} dataList={loadingCompanyList ? "PENDING" : { data: companyList }} maxHeight={500} rowClick={companyRowClick} searchable />

        </Row>
        <Row className="rounded mt-2" style={{ border: "1px solid gray" }}>
            <label className="w-100 p-1 mb-1" style={{ borderBottom: "1px solid gray" }}>فهرست کاربران</label>
            <div className="text-end">
                <Button variant="link" onClick={showModalUser} className="p-0">
                    <BsPlusSquareFill size={28} />
                </Button>
            </div>
            <BihamtaTable columns={columns} dataList={loadingUserList ? "PENDING" : { data: userList }} rowButtons={buttons} maxHeight={500} searchable rowClick={onUserRowClick} />
        </Row>
        <Row className="border mt-2">
            <Col md={12}>
                <label>لینک تبلیغات:</label>
                <FormControl defaultValue={settings?.settingValue} onChange={e => advLink.current = e.target.value} style={{ direction: "ltr", textAlign: "center" }} />
                <Button className="mt-1 mb-1" variant="success" onClick={handleSubmitSettings} disabled={loadingUpdateSetting}><FaCheck size={18} />&nbsp; ثبت اطلاعات</Button>
            </Col>
        </Row>
    </div>
);
};

export default AdminPanel;
