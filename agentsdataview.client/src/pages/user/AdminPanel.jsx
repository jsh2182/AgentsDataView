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
import { useGetAllProvincesQuery } from "../../store/province/provinceApi";
import { useLazyGetAllCitiesQuery } from "../../store/city/cityApi";
import { MoonLoader } from "react-spinners";
import { useLazyGetSettingQuery, useUpdateSettingMutation } from "../../store/setting/settingApi";
import { FaCheck } from "react-icons/fa";

const AdminPanel = () => {
    const dispatch = useDispatch();
    const { data: provinceList, isLoading: loadingProvinceList } = useGetAllProvincesQuery();
    const [getCityList, { data: cityList, isFetching: loadingCityList }] = useLazyGetAllCitiesQuery();
    const [getUserList, { data: userList, isFetching: loadingUserList, error }] = useLazyFetchUserListQuery();
    const [createUser, { isLoading: createLoading, error: createError }] = useCreateUserMutation();
    const [deleteUser, { isLoading: deleteLoading, error: deleteError }] = useDeleteUserMutation();
    const [updateCompany, { isLoading: loadingUpdateComp, error: errorUpdateCompany, reset: resetUpdateCompany }] = useUpdateCompanyMutation();
    const [createCompany, { isLoading: loadingCreateCompany, error: errorCreateCompany, reset: resetCreateCompany }] = useCreateCompanyMutation();
    const [getCompanyList, { data: companyList, isLoading: loadingCompanyList, error: errorCompanyList }] = useLazyGetAllCompaniesQuery();
    const [getSetting, { data: settings }] = useLazyGetSettingQuery();
    const [updateSetting, { isLoading: loadingUpdateSetting, error: errorUpdateSetting }] = useUpdateSettingMutation();
    const advLink = useRef(settings?.settingValue ?? "");
    const currentUser = useSelector((state) => state.user.currentUser);
    const selectedUser = useRef();
    const [showNewUser, setShowNewUser] = useState(false);
    const [showModalDelete, setShowModalDelete] = useState(false);
    const [companyInfo, setCompanyInfo] = useState(null);
    const [showModalCompany, setShowModalCompany] = useState(false);
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
        { colTitle: "نشانی", colName: "address", colType:"string" },
        { colTitle: "تاریخ آخرین ارسال", colName: "pMaxInvoiceCreationDate" },
        { colTitle: "آخرین تاریخ فاکتور", colName: "pMaxInvoiceDate" }
    ]
    const showCompany = (row) => {
        //setCompanyInfo({ ...row, e: undefined });
        reset({ ...row, e: undefined });
        if (Number(row.provinceId) > 0)
            getCityList(row.provinceId);
        setShowModalCompany(true);
    }
    const companyButtons = [{ type: "show", onclick: showCompany }]
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


    const ModalNewUser = () => {
        const onSubmitUserData = async (data) => {
            data.companyId = companyInfo.id;
            data.password = data.userMobile;
            await createUser(data).unwrap();
            reset();
            setShowNewUser(false);
        }
        return (
            <Modal show={showNewUser && !showModalCompany} variant="" animation={true} onHide={() => setShowNewUser(false)} backdrop={false}>
                <Modal.Header className="header-style" closeButton >
                    <span>
                        <BsPersonPlus size={25} />&nbsp; کاربر جدید
                    </span>
                    {/* <FiX size={25} onClick={() => setShowNewUser(false)} /> */}
                </Modal.Header>
                <Modal.Body>
                    {createError && <Alert variant="danger">{createError}</Alert>}
                    {showNewUser && !showModalCompany &&
                        <form onSubmit={handleSubmit(onSubmitUserData)} className="vstack gap-3 "
                        >
                            <FormGroup>
                                <Form.Control
                                    name="userName"
                                    type="text"
                                    placeholder="نام کاربری"
                                    {...register("userName", { required: "نام کاربری الزامی است" })}
                                />
                                {errors?.userName && <small className="text-danger">{errors.userName.message}</small>}
                            </FormGroup>
                            <FormGroup>
                                <Form.Control
                                    name="userFullName"
                                    type="text"
                                    placeholder="نام کامل"
                                    {...register("userFullName", { required: "نام کامل الزامی است" })}
                                />
                                {errors?.userFullName && <small className="text-danger">{errors.userFullName?.message}</small>}
                            </FormGroup>
                            <FormGroup>
                                <Form.Control
                                    name="userMobile"
                                    type="number"
                                    placeholder="شماره همراه"
                                    {...register("userMobile", { required: "شماره همراه الزامی است" })}
                                />
                                {errors?.userMobile && <small className="text-danger">{errors.userMobile?.message}</small>}
                            </FormGroup>
                            <Button variant="success" type="submit" disabled={createLoading}><BiCheck size={18} />&nbsp;ثبت اطلاعات</Button>
                        </form>
                    }
                </Modal.Body>
                <Modal.Footer style={{ padding: 0 }}></Modal.Footer>
            </Modal>
        )
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
            <Modal show={showModalCompany && !showNewUser} variant="" animation={!showModalCompany || showNewUser} onHide={() => setShowModalCompany(false)} backdrop={false}>
                <Modal.Header className="header-style" closeButton >
                    <span>
                        <BsBuilding size={25} />&nbsp; اطلاعات شرکت
                    </span>
                    {/* <FiX size={25} onClick={() => setShowNewUser(false)} /> */}
                </Modal.Header>
                <Modal.Body>
                    {(errorUpdateCompany || errorCreateCompany) && <Alert variant="danger">{errorUpdateCompany || errorCreateCompany}</Alert>}
                    {showModalCompany && !showNewUser &&
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
        setCompanyInfo({ ...row });
        getUserList(row.id);
    }
    const handleLogout = () => {
        dispatch(logout());
    };
    const handleSubmitSettings = () => {
        updateSetting({ settingName: "AdvertisingLink", settingValue: advLink.current });
    }
    useEffect(() => {
        if (currentUser) {
            getCompanyList();
            getSetting("AdvertisingLink");
        }
    }, [currentUser])
    return (
        <div className="p-4 border rounded m-2">
            <ModalNewUser />
            <ModalCompanyInfo />
            <ModalConfirmDelete />
            <div style={{ position: "relative" }}>
                <h2 className="text-lg font-bold mb-2" style={{ borderBottom: "1px solid #a8b3f5", paddingBottom: "0.3em" }}>پنل مدیریت</h2>
                <Button variant="outline-danger tooltip-container" style={{ position: "absolute", top: 0, left: 0, border: "none" }} onClick={handleLogout}>
                    <BsBoxArrowRight size={25} />
                    <span className="tooltip-text">خروج</span>
                </Button></div>
            <Row className="g-1">
                <Col md={companyInfo ? 7 : 12} className="rounded" style={{ border: "1px solid gray" }}>
                    <label className="w-100 p-1 mb-1" style={{ borderBottom: "1px solid gray" }}>فهرست شرکت ها</label>
                    <div className="text-end">
                        <Button variant="link" onClick={prepareForAddComp} className="p-0">
                            <BsPlusSquareFill size={28} />
                        </Button>
                    </div>
                    <BihamtaTable columns={companyColumns} dataList={loadingCompanyList ? "PENDING" : { data: companyList }} rowButtons={companyButtons} maxHeight={500} rowClick={companyRowClick} searchable />
                </Col>
                {companyInfo &&
                    <Col md={5} className="rounded" style={{ border: "1px solid gray" }}>
                        <label className="w-100 p-1 mb-1" style={{ borderBottom: "1px solid gray" }}>فهرست کاربران {companyInfo.name}</label>
                        <div className="text-end">
                            <Button variant="link" onClick={() => setShowNewUser(true)} className="p-0">
                                <BsPlusSquareFill size={28} />
                            </Button>
                        </div>
                        <BihamtaTable columns={columns} dataList={loadingUserList ? "PENDING" : { data: userList }} rowButtons={buttons} maxHeight={500} searchable />
                    </Col>
                }
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
