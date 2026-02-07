import { Alert, Button, Form, FormGroup, Modal } from "react-bootstrap";
import { useCreateUserMutation, useUpdateUserMutation } from "../../store/user/userAPI";
import { useForm } from "react-hook-form";
import { BsPersonBadge, BsPersonPlus } from "react-icons/bs";
import { BiCheck, BiPencil } from "react-icons/bi";
import { useEffect, useRef, useState } from "react";
import BihamtaMultiSelect from "../BihamtaMultiSelect";
import { useLazyGetAllProvincesQuery } from "../../store/province/provinceApi";
import { MoonLoader } from "react-spinners";

/**
 * 
 * @param {{modalData:{show:true}, setModalData:function}} param0 
 * @returns 
 */
export default function ModalAddEditUser({ modalData, setModalData, companyList, provinceList }) {
    const [createUser, { isLoading: createLoading, error: createError }] = useCreateUserMutation();
    const [updateUser, { isLoading: loadingUpdate, error: errorUpdate }] = useUpdateUserMutation();
    const { userInfo } = modalData;
    const [pendingCompanyFilter, setPendingCompanyFilter] = useState(false);
    const [filteredCompanyList, setFilteredCompanyList] = useState([]);
    const selectedCompanies = useRef([]);
    const {
        register,
        handleSubmit,
        reset,
        formState: { errors },
    } = useForm();
    const onSubmitUserData = async (data) => {
        data.password = data.userMobile;
        data.companyIds = selectedCompanies.current;
        if (Number(userInfo?.id) > 0) {

            await updateUser(data).unwrap();
        }
        else {
            await createUser(data).unwrap();
        }
        close();
    }
    function close() {
        reset({ id: 0 });
        selectedCompanies.current = [];
        if(filteredCompanyList?.length !== companyList?.length)
        {
            setFilteredCompanyList([...companyList]);
        }
        setModalData({ show: false });
    }
    function handleCompanyChecked(item) {
        const splited = item.value.split(",");
        splited.forEach(p => {
            if (item.checked) {
                if (!selectedCompanies.current.includes(p)) {

                    selectedCompanies.current.push(p);
                }
            }
            else {
                selectedCompanies.current = selectedCompanies.current.filter(a => a !== p);

            }
        });
    }
    function filterCompanies(provinceId) {
        setPendingCompanyFilter(true);
        const filtered = Number(provinceId) > 0 ?
            companyList.filter(c => Number(c.provinceId) === Number(provinceId)) :
            [...companyList];
        setFilteredCompanyList(filtered);
        setTimeout(() => {
            setPendingCompanyFilter(false);
        }, [50])
    }
    useEffect(() => {
        if (Number(userInfo?.id) > 0) {
            setPendingCompanyFilter(true);
            selectedCompanies.current = [...userInfo.companyIds];
            setTimeout(() => {
                setPendingCompanyFilter(false);
            }, [50])
            reset({ ...userInfo });
        }
    }, [userInfo?.id])
    useEffect(() => {
        if (companyList?.length > 0) {
            setFilteredCompanyList([...companyList]);
        }
    }, [companyList?.length])

    if (!modalData?.show) {
        return null;
    }
    return (
        <Modal show={modalData.show} variant="" animation={true} onHide={close} backdrop={false}>
            <Modal.Header className="header-style" closeButton >
                {Number(userInfo?.id) > 0 ?
                    <span>
                        <BsPersonBadge size={25} />&nbsp; اطلاعات کاربر

                    </span> :
                    <span>
                        <BsPersonPlus size={25} />&nbsp; کاربر جدید
                    </span>
                }
            </Modal.Header>
            <Modal.Body>
                {(createError || errorUpdate) && <Alert variant="danger">{createError || errorUpdate}</Alert>}
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
                    <FormGroup>
                        <Form.Control className="mb-1" as="select" style={{ maxWidth: "max-content", fontSize:"13px" }}
                            {...register("provinceId", {
                                onChange: (e) => {
                                    filterCompanies(e.target.value);
                                }
                            })}

                        >
                            <option value="">فیلتر استان</option>
                            {provinceList?.map(p => <option value={p.id} key={p.id}>{p.provinceName}</option>)}
                        </Form.Control>
                        {pendingCompanyFilter ?
                            <MoonLoader size={25} /> :

                            <BihamtaMultiSelect
                                dataSource={filteredCompanyList}
                                displayItem="name"
                                valueItem="id"
                                placeholder="شرکت"

                                onItemCheckChanged={handleCompanyChecked}
                                defaultCheckedList={selectedCompanies.current ?? []} />}
                    </FormGroup>
                    <Button variant="success" type="submit" disabled={createLoading || loadingUpdate}>
                        {Number(userInfo?.id) > 0 ?
                            <>
                                <BiPencil size={18} />&nbsp;ویرایش اطلاعات
                            </> :
                            <>
                                <BiCheck size={18} />&nbsp;ثبت اطلاعات
                            </>}
                    </Button>
                </form>

            </Modal.Body>
            <Modal.Footer style={{ padding: 0 }}></Modal.Footer>
        </Modal>
    )
}