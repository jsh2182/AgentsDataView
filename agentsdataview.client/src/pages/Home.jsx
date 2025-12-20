import { useDispatch, useSelector } from "react-redux";
import { logout } from "../store/user/userSlice";
import { BsBoxArrowRight, BsPencilSquare, BsPerson, BsPersonBadge, BsClock } from "react-icons/bs";
import { CiClock1, CiClock2 } from "react-icons/ci";
import { Alert, Button, Card, Col, Form, OverlayTrigger, Popover, Row, Table } from "react-bootstrap";
import { useUpdateMeMutation } from "../store/user/userAPI";
import {
  useGetProfitReportByProvinceQuery,
  useLazyGetProfitReportByCompanyQuery,
  useLazyGetReportByCompanyAndProductQuery,
  useLazyGetReportByProvinceQuery
} from "../store/reportData/reportDataApi";
import { useForm } from "react-hook-form";
import { useEffect, useRef, useState } from "react";
import { useStickyHeader } from "../utils/useStickeHeader";
import IranMap from "../components/IranMap";
import { useGetAllProvincesQuery } from "../store/province/provinceApi";
import CustomActiveShapePieChart from "../components/CustomActiveShapePieChart";
import PieChartWithCustomizedLabel from "../components/PieChartWithCustomizedLabel";
import { useGetLastUpdateQuery } from "../store/invoice/invoiceApi";
import { Bar, BarChart, CartesianGrid, Rectangle, Tooltip, XAxis, YAxis } from "recharts";
import ModalAdv from "../components/ModalAdv";
import UpdateButton from "../components/UpdateButton";
import { MoonLoader } from "react-spinners";
import useIsMobile from "../hooks/useIsMobile";

export default function Home() {
  const dispatch = useDispatch();
  const user = useSelector((state) => state.user.currentUser);
  const tblByProvinceRef = useRef();
  const tblByProdAndCompRef = useRef();
  const tblProfitByProvinceRef = useRef();
  const tblProfitByCompanyRef = useRef();
  const [provinceForProvinceOnProvince, setProvinceForReportOnProvince] = useState(null);
  const [updateMe, { isLoading: loadingUpdateMe, error: errorUpdateMe }] = useUpdateMeMutation();
  const [getProfitReportByCompany, { data: profitReportByCompany, isFetching: profitReportByCompanyLoading, error: profitReportByCompanyError }] = useLazyGetProfitReportByCompanyQuery();
  const { data: profitReportByProvince, isFetching: profitReportByProvinceLoading, error: profitReportByProvinceError, refetch: refetchProfitReportByProvince } = useGetProfitReportByProvinceQuery();
  const [getReportByCompanyAndProduct, { data: reportByCompanyAndProduct, isFetching: reportByCompanyAndProductLoading, error: reportByCompanyAndProductError }] = useLazyGetReportByCompanyAndProductQuery();
  const [getRepotByProvince, { data: reportByProvince, isFetching: reportByProvinceQueryLoading, error: reportByProvinceError }] = useLazyGetReportByProvinceQuery();
  // const { data: reportByProvince_Cumulative, isLoading: reportByProvince_CumulativeLoading, error: reportByProvince_CumulativeError } = useGetReportByProvince_CumulativeQuery();
  const { data: provinceList, isLoading: loadingProvinceList } = useGetAllProvincesQuery();
  const { register, handleSubmit, formState: { errors } } = useForm();
  const [hiddenLabel, setHiddenLabel] = useState(null);
  const isMobile = useIsMobile();
  const handleLogout = () => {
    dispatch(logout());
  };
  const handleUpdate = async (data) => {
    await updateMe(data).unwrap();
  }
  const popUserInfo = (
    user &&
    <Popover style={{ minWidth: "max-content" }}>
      <Popover.Header className="d-flex justify-content-between">
        <span>
          <BsPersonBadge />&nbsp; اطلاعات کاربری
        </span>
        <div className="tooltip-container">
          <BsBoxArrowRight onClick={handleLogout} className="btn-logout" size={25} />
          <span className="tooltip-text">خروج</span>
        </div>
      </Popover.Header>
      <Popover.Body >
        {errorUpdateMe && <Alert variant="danger">{errorUpdateMe}</Alert>}
        <form onSubmit={handleSubmit(handleUpdate)}>
          <table>
            <tbody>
              <tr>
                <td>نام کامل: </td>
                <td>
                  <input className="table-row-input" defaultValue={user.full_name}
                    {...register("userFullName", { require: "نام کامل الزامی است" })} />
                </td>
              </tr>
              {errors.userFullName &&
                <tr>
                  <td colSpan={2} style={{ textAlign: "center" }}>
                    <small className="text-danger">{errors.userFullName?.message}</small>
                  </td>
                </tr>}
              <tr>
                <td>نام کاربری: </td>
                <td>
                  <input className="table-row-input" defaultValue={user.uName} {...register("userName", { required: "نام کاربری الزامی است" })} />
                </td>
              </tr>
              {errors.userName &&
                <tr>
                  <td colSpan={2} style={{ textAlign: "center" }}>
                    <small className="text-danger">{errors.userName?.message}</small>
                  </td>
                </tr>}
              <tr>
                <td>گذرواژه: </td>
                <td>
                  <input type="password" className="table-row-input" defaultValue={"____________________"}
                    {...register("password", {
                      required: "رمز عبور الزامی است", minLength: {
                        value: 4, message: "رمز عبور باید حداقل 4 کاراکتر باشد",
                      }
                    })} />
                </td>
              </tr>
              {errors.password &&
                <tr>
                  <td colSpan={2} style={{ textAlign: "center" }}>
                    <small className="text-danger">{errors.password?.message}</small>
                  </td>
                </tr>}
              <tr>
                <td>شماره همراه:</td>
                <td>
                  <input type="number" className="table-row-input" defaultValue={user.u_mobile}
                    {...register("userMobile", { required: "نام کاربری الزامی است" })} />
                </td>

              </tr>
              {errors.userMobile &&
                <tr>
                  <td colSpan={2} style={{ textAlign: "center" }}>
                    <small className="text-danger">{errors.userMobile?.message}</small>
                  </td>
                </tr>}
            </tbody>
          </table>
          <div>
            <Button variant="success" type="submit" className="w-100 mt-2" style={{ fontSize: "0.8rem" }} disabled={loadingUpdateMe}><BsPencilSquare size={18} /> ویرایش اطلاعات</Button>
          </div>
        </form>
      </Popover.Body>
    </Popover >
  )
  const CustomTooltip = ({ active, payload, label }) => {
    const isVisible = active && payload && payload.length;
    return (
      <div className="custom-tooltip" style={{ visibility: isVisible ? 'visible' : 'hidden' }}>
        {isVisible && (
          <p style={{ backgroundColor: "black", color: "whitesmoke", fontSize: "0.8em", borderRadius: "5px", padding: "8px" }}>
            {`${payload[0]?.payload?.provinceName}: ${Intl.NumberFormat().format(payload[0].value)}`}</p>

        )}
      </div>
    );
  };
  const handleProvinceOnReportByProvinceChange = (e) => {
    getRepotByProvince(e.target.value);
    setProvinceForReportOnProvince(e.target.value);
  }
  //#region functions

  //#endregion
  useStickyHeader([tblByProvinceRef, tblByProdAndCompRef, tblProfitByProvinceRef, tblProfitByCompanyRef], "#d4d8ef");

  useEffect(() => {

    if (!tblByProdAndCompRef.current) return;

    const scrollContainer = tblByProdAndCompRef.current.parentElement;
    if (!scrollContainer) return;
    if (Array.isArray(reportByCompanyAndProduct) && reportByCompanyAndProduct.length > 0) {
      setHiddenLabel(reportByCompanyAndProduct[0].companyName);
    }
    const handleScroll = () => {
      const cells = tblByProdAndCompRef.current.querySelectorAll("td[rowspan]");
      const containerRect = scrollContainer.getBoundingClientRect();

      cells.forEach((cell) => {
        if (cell.dataset?.label) {
          const cellRect = cell.getBoundingClientRect();
          const isVisibleVertically =
            cellRect.bottom > containerRect.top && cellRect.top < containerRect.bottom;
          if (isVisibleVertically) {
            setHiddenLabel(cell.dataset.label);
          }
        }
      });
    };

    scrollContainer.addEventListener("scroll", handleScroll);
    return () => scrollContainer.removeEventListener("scroll", handleScroll);
  }, [reportByCompanyAndProduct]);
  return (
    user &&
    <div className="p-1 border rounded m-2">
      <ModalAdv />
      <div style={{
        position: "sticky", zIndex: 100, top: 0, backdropFilter: "blur(20px)",
        borderBottom: "1px solid #a8b3f5", display: "flex", justifyContent: "space-between", padding: "8px", borderRadius: "2px"
      }}>
        <UpdateButton />
        <h2 className="text-lg font-bold mb-2">داشبورد گزارش</h2>
        <OverlayTrigger overlay={popUserInfo} trigger="click" placement="right" rootClose>
          <Button variant="outline-info tooltip-container" style={{ border: "none" }}>
            <BsPerson size={25} />
            {user.full_name}
          </Button>
        </OverlayTrigger>
      </div>
      <Row className="mt-2">
        <Col md={6}>
          <Card>
            <Card.Header style={{ backgroundColor: "transparent", borderBottom: "1px solid gray", width: "95%", margin: "auto" }}>
              <h6>
                سود/زیان (ناخالص) تجمیعی
              </h6>
            </Card.Header>
            <Card.Body >

              <Table bordered responsive className="text-center align-middle mb-0 fixedHeightTable" ref={tblProfitByProvinceRef}>
                <thead>
                  <tr>
                    <th rowSpan={3} className="auto-width-col">ردیف</th>
                    <th rowSpan={3}></th>
                    <th>فروش</th>
                    <th>بهای تمام شده</th>
                    <th>سود/زیان</th>
                  </tr>
                </thead>
                <tbody>
                  {Array.isArray(profitReportByProvince) && profitReportByProvince.length > 0 ?
                    profitReportByProvince.map((data, i) =>

                      <tr key={i} className="result-table-row">
                        <td>{i + 1}</td>
                        <td>{data.provinceName}</td>
                        <td>{data.provinceOutputTotal?.toLocaleString("fa-IR")}</td>
                        <td>{data.costOfGoodsSold?.toLocaleString("fa-IR")}</td>
                        <td>{data.profitLoss?.toLocaleString("fa-IR")}</td>
                      </tr>

                    ) :
                    <tr><td colSpan={6} className="text-center">داده ای برای نمایش وجود ندارد</td></tr>
                  }
                </tbody>
              </Table>
              <Row>
                <Col md={6} className="mt-2" style={{ border: "1px solid gray", borderRadius: "3px" }}>
                  <label style={{ borderBottom: "1px solid gray", width: "100%", padding: "4px" }}>فروش کشوری</label>
                  {Array.isArray(profitReportByProvince) ?
                    <PieChartWithCustomizedLabel title="فروش کشوری" data={profitReportByProvince.filter(p => Number(p.provinceId) > 0).map(p => ({ name: p.provinceName, value: p.provinceOutputTotal }))} /> :
                    <p>داده ای برای نمایش وجود ندارد</p>}
                </Col>
                <Col md={6} xs={12} className="mt-2" style={{ border: "1px solid gray", borderRadius: "3px" }}>
                  <label style={{ borderBottom: "1px solid gray", width: "100%", padding: "4px" }}>بهای تمام شده کشوری</label>
                  {Array.isArray(profitReportByProvince) ?
                    <CustomActiveShapePieChart data={profitReportByProvince.filter(p => Number(p.provinceId) > 0).map(p => ({ name: p.provinceName, value: p.costOfGoodsSold }))} />
                    : <p>داده ای برای نمایش وجود ندارد</p>}
                </Col>
              </Row>
            </Card.Body>
          </Card>
        </Col>
        {!isMobile &&
          <Col md={6}>
            <IranMap dataList={profitReportByProvince}
              tooltips={[{ title: "فروش", name: "provinceOutputTotal", isPrice: true },
              { title: "بهای تمام شده", name: "costOfGoodsSold", isPrice: true },
              { title: "سود/زیان", name: "profitLoss", isPrice: true }
              ]}
            />
          </Col>
          }
      </Row>
      <Row>
        <Col md={6} className="mb-1 mt-1">
          <Card>
            <Card.Header style={{ backgroundColor: "transparent", borderBottom: "1px solid gray" }}>
              <h6 style={{ margin: "auto" }}>
                سود ناخالص کشوری
              </h6>
            </Card.Header>
            <Card.Body style={{ height: "450px" }}>
              {Array.isArray(profitReportByProvince) ?
                <BarChart
                  style={{ width: '100%', height: "100%", maxWidth: '700px', maxHeight: '70vh', aspectRatio: 1.618 }}
                  responsive
                  data={profitReportByProvince.filter(p => Number(p.provinceId) > 0)}
                  margin={{
                    top: 5,
                    right: 0,
                    left: 0,
                    bottom: 5,
                  }}
                >
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis dataKey="provinceId" reversed fontSize={13} />
                  <YAxis width="auto" orientation="right" angle={35} style={{ direction: "ltr" }} fontSize={12} tickFormatter={value => value?.toLocaleString("fa-IR")} />
                  <Tooltip content={CustomTooltip} />
                  <Bar dataKey="profitLoss" fill="#496ab6ff" activeBar={<Rectangle fill="#7a88a7ff" stroke="blue" />} />
                </BarChart> :
                <p>داده ای برای نمایش وجود ندارد</p>}
            </Card.Body>
          </Card>
        </Col>
        <Col md={6} className="mb-1 mt-1">
          <Card>
            <Card.Header style={{ backgroundColor: "transparent", borderBottom: "1px solid gray", width: "95%", margin: "auto" }}>
              <h6 style={{ margin: "auto" }}>
                سود/زیان (ناخالص) تفکیکی
              </h6>
            </Card.Header>
            <Card.Body style={{ height: "450px" }}>
              <Form.Control as="select" style={{ maxWidth: "max-content" }} className="mb-1" defaultValue="-1" onChange={e => getProfitReportByCompany(e.target.value)}>
                <option value="-1">استان را انتخاب کنید</option>
                <option value="">همه استان ها</option>
                {provinceList?.map(p => <option value={p.id} key={p.id}>{p.provinceName}</option>)}
              </Form.Control>
              <Table bordered responsive className="text-center align-middle mb-0 fixedHeightTable" ref={tblProfitByCompanyRef} style={{ overflow: profitReportByCompanyLoading ? "hidden" : "auto" }}>
                <thead>
                  <tr>
                    <th>ردیف</th>
                    <th>نام شرکت</th>
                    <th>فروش</th>
                    <th>بهای تمام شده</th>
                    <th>سود (زیان)</th>
                  </tr>
                </thead>
                <tbody>
                  {profitReportByCompanyLoading &&
                    <tr>
                      <td colSpan={6} className="p-1">
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
                  {!profitReportByCompanyLoading && Array.isArray(profitReportByCompany) && profitReportByCompany.length > 0 ?
                    profitReportByCompany.map((data, i) =>
                      <tr key={i} className="result-table-row">
                        <td>{i+1}</td>
                        <td>{data.companyName}</td>
                        <td>{data.companyOutputTotal?.toLocaleString("fa-IR")}</td>
                        <td>{data.costOfGoodsSold?.toLocaleString("fa-IR")}</td>
                        <td>{data.profitLoss?.toLocaleString("fa-IR")}</td>
                      </tr>) :

                    <tr>{!profitReportByCompanyLoading &&
                      <td colSpan="11" className="text-center">
                        {profitReportByCompanyError?.length > 0 ? <span className="text-danger">{profitReportByCompanyError}</span> :
                          <span> داده ای برای نمایش وجود ندارد</span>}
                      </td>
                    }
                    </tr>
                  }
                </tbody>
              </Table>
            </Card.Body>
          </Card>
        </Col>
      </Row>
      <Row >
        <Col md={12}>
          <Card>
            <Card.Header style={{ backgroundColor: "transparent", borderBottom: "1px solid gray" }}>
              <h6 style={{ margin: "auto" }}>
                گردش کالا تجمیعی
              </h6>
            </Card.Header>
            <Card.Body style={{ height: "423.6px" }}>
              <Form.Control as="select" style={{ maxWidth: "max-content" }} className="mb-1" onChange={handleProvinceOnReportByProvinceChange}>
                <option value="">استان را انتخاب کنید</option>
                {provinceList?.map(p => <option value={p.id} key={p.id}>{p.provinceName}</option>)}
              </Form.Control>
              <Table bordered responsive className="text-center align-middle mb-0 fixedHeightTable" ref={tblByProvinceRef}>
                <thead>
                  <tr >
                    <th rowSpan="3">ردیف</th>
                    <th rowSpan="3">کد کالا</th>
                    <th rowSpan="3">نام کالا</th>
                    <th colSpan="4">تجمیعی کشور</th>
                    {Number(provinceForProvinceOnProvince) > 0 && <th colSpan="4">تجمیعی استان</th>}
                  </tr>
                  <tr >
                    <th colSpan="2">وارده</th>
                    <th colSpan="2">صادره</th>
                    {Number(provinceForProvinceOnProvince) > 0 &&
                      <>
                        <th colSpan="2">وارده</th>
                        <th colSpan="2">صادره</th>
                      </>}
                  </tr>
                  <tr >
                    <th>مقدار</th>
                    <th>ریال</th>
                    <th>مقدار</th>
                    <th>ریال</th>
                    {Number(provinceForProvinceOnProvince) > 0 &&
                      <>
                        <th>مقدار</th>
                        <th>ریال</th>
                        <th>مقدار</th>
                        <th>ریال</th>
                      </>}
                  </tr>
                </thead>
                <tbody>
                  {reportByProvinceQueryLoading &&
                    <tr>
                      <td colSpan={11} className="p-1">
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
                  {!reportByProvinceQueryLoading && Array.isArray(reportByProvince) && reportByProvince.length > 0 ?
                    reportByProvince.map((data, i) => <tr key={i} className="result-table-row">
                      <td className="auto-width-col">{i + 1}</td>
                      <td>{data.productCode}</td>
                      <td>{data.productName}</td>
                      <td style={{ background: "#547fc6", color: "white" }}>{data.countryInputCount}</td>
                      <td style={{ background: "#547fc6", color: "white" }}>{data.countryInputTotal?.toLocaleString("fa-IR")}</td>
                      <td style={{ background: "#547fc6", color: "white" }}>{data.countryOutputCount}</td>
                      <td style={{ background: "#547fc6", color: "white" }}>{data.countryOutputTotal?.toLocaleString("fa-IR")}</td>
                      {Number(provinceForProvinceOnProvince) > 0 &&
                        <>
                          <td>{data.provinceInputCount}</td>
                          <td>{data.provinceInputTotal?.toLocaleString("fa-IR")}</td>
                          <td>{data.provinceOutputCount}</td>
                          <td>{data.provinceOutputTotal?.toLocaleString("fa-IR")}</td>
                        </>}
                    </tr>) :
                    <tr>
                      {!reportByProvinceQueryLoading &&
                        <td colSpan="11" className="text-center">

                          {reportByProvinceError?.length > 0 ? <span className="text-danger">{reportByProvinceError}</span> :
                            <span> داده ای برای نمایش وجود ندارد</span>}
                        </td>
                      }
                    </tr>
                  }
                </tbody>
              </Table>
              {/* </div> */}
            </Card.Body>
          </Card>
        </Col>
      </Row>
      <Row className="mt-2">
        <Col md={12}>
          <Card>
            <Card.Header style={{ backgroundColor: "transparent", borderBottom: "1px solid gray", width: "95%", margin: "auto" }}>
              <h6 style={{ margin: "auto" }}>
                گردش کالا تفکیکی
              </h6>
            </Card.Header>
            <Card.Body style={{ height: "423.6px" }}>
              <Form.Control as="select" style={{ maxWidth: "max-content" }} className="mb-1" onChange={e => getReportByCompanyAndProduct(e.target.value)}>
                <option value="">استان را انتخاب کنید</option>
                {provinceList?.map(p => <option value={p.id} key={p.id}>{p.provinceName}</option>)}
              </Form.Control>

              <Table bordered responsive className="text-center align-middle mb-0 fixedHeightTable" ref={tblByProdAndCompRef}>
                <thead>
                  {hiddenLabel?.length > 0 && <tr><th colSpan={8}>{hiddenLabel}</th></tr>}
                  <tr>
                    <th rowSpan={3} className="auto-width-col">ردیف</th>
                    <th rowSpan={3}>نام شرکت</th>
                    <th rowSpan={3}>کد کالا</th>
                    <th rowSpan={3}>نام کالا</th>
                    <th colSpan={2}>وارده</th>
                    <th colSpan={2}>صادره</th>
                  </tr>
                  <tr>
                    <th>مقدار</th>
                    <th>ریال</th>
                    <th>مقدار</th>
                    <th>ریال</th>

                  </tr>
                </thead>
                <tbody>
                  {reportByCompanyAndProductLoading &&
                    <tr>
                      <td colSpan={11} className="p-1">
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
                  {!reportByCompanyAndProductLoading && Array.isArray(reportByCompanyAndProduct) && reportByCompanyAndProduct.length > 0 ?
                    reportByCompanyAndProduct.map((data, i) =>
                      <>
                        <tr key={i} className="result-table-row" >
                          <td rowSpan={data.products.length} style={{ boxShadow: 'inset 0 2px 0 #0037ff' }}>{i + 1}</td>
                          <td rowSpan={data.products.length} style={{ position: "relative", boxShadow: 'inset 0 2px 0 #0037ff' }} data-label={data.companyName}>
                            <div style={{
                              position: "sticky", display: "flex", justifyContent: "center", alignItems: "center", height: "30px",
                              top: "50%", transform: "translateY(-50%)"
                            }}>
                              {data.companyName}
                            </div>
                          </td>

                          {data.products?.length > 0 &&
                            <>
                              <td style={{ boxShadow: 'inset 0 2px 0 #0037ff' }}>{data.products[0].productCode}</td>
                              <td style={{ boxShadow: 'inset 0 2px 0 #0037ff' }}>{data.products[0].productName}</td>
                              <td style={{ boxShadow: 'inset 0 2px 0 #0037ff' }}>{data.products[0].inputCount}</td>
                              <td style={{ boxShadow: 'inset 0 2px 0 #0037ff' }}>{data.products[0].inputTotalPrice?.toLocaleString("fa-IR")}</td>
                              <td style={{ boxShadow: 'inset 0 2px 0 #0037ff' }}>{data.products[0].outputCount}</td>
                              <td style={{ boxShadow: 'inset 0 2px 0 #0037ff' }}>{data.products[0].outputTotalPrice?.toLocaleString("fa-IR")}</td>
                            </>}
                        </tr>
                        {
                          data.products?.length > 1 &&
                          data.products.map((product, ii) => ii > 0 &&
                            <tr key={ii} className="result-table-row" >
                              <td>{product.productCode}</td>
                              <td>{product.productName}</td>
                              <td>{product.inputCount}</td>
                              <td>{product.inputTotalPrice?.toLocaleString("fa-IR")}</td>
                              <td>{product.outputCount}</td>
                              <td>{product.outputTotalPrice?.toLocaleString("fa-IR")}</td>
                            </tr>)
                        }

                      </>
                    ) :
                    <tr>
                      {!reportByCompanyAndProductLoading &&
                        <td colSpan="11" className="text-center">
                          {reportByCompanyAndProductError?.length > 0 ? <span className="text-danger">{reportByCompanyAndProductError}</span> :
                            <span> داده ای برای نمایش وجود ندارد</span>}
                        </td>}
                    </tr>
                  }
                </tbody>
              </Table>
            </Card.Body>
          </Card>
        </Col>
      </Row>

    </div >
  );
}
