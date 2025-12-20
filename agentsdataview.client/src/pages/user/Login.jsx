import { useForm } from "react-hook-form";
import { FaSignInAlt } from "react-icons/fa";
import { useLoginUserMutation } from "../../store/user/userAPI";
import AutoHideToast from "../../components/AutoHideToast";
import { Alert, Button, Form, FormGroup } from "react-bootstrap";
import { useState } from "react";

export default function Login() {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();
  const [loginUser, { isLoading: loginLoading, error: loginError }] = useLoginUserMutation();
  const [error, setError] = useState({ type: "error", text: loginError, title: "ورود به سیستم" });
  const handleLogin = async (data) => {
    try {
      await loginUser(data).unwrap();
      // currentUser به‌صورت خودکار در Store و LocalStorage ست می‌شود
    } catch (err) {
      setError({ type: "error", text: err, title: "ورود به سیستم" });
    }
  };

  return (

    <div className="d-flex align-items-center justify-content-center min-vh-100 w-100 bg-gradient-custom">
      <div className="custom-width mx-auto p-4 rounded-lg shadow position-relative" style={{ backgroundColor: "rgba(0,0,0,0.01)", outline: "1px solid #eff6ff", minWidth: "320px" }}>
        <h2 className="mb-4 fs-3 fw-semibold text-center text-primary">
          ورود
        </h2>
        <AutoHideToast message={error} setMessage={setError} />
        <form
          onSubmit={handleSubmit(handleLogin)}
          className="vstack gap-3"
          noValidate
        >
          <FormGroup>
            <Form.Control
              name="username"
              type="text"
              placeholder="نام کاربری"
              {...register("username", { required: "نام کاربری الزامی است" })}
            />
            {errors.username && <small className="text-danger">{errors.username?.message}</small>}
          </FormGroup>
          <FormGroup>
            <Form.Control
              name="password"
              type="password"
              placeholder="رمز عبور"
              {...register("password", {
                required: "رمز عبور الزامی است", minLength: {
                  value: 4, message: "رمز عبور باید حداقل 4 کاراکتر باشد",
                }
              })}
            />
            {errors.password && <small className="text-danger">{errors.password?.message}</small>}
          </FormGroup>
          {/* <div className="text-right">
              <RiLockPasswordLine
                size={17}
              />
              <Link
                to=""
                className="text-sm text-orange-600 text-right hover:text-orange-600/80"
              >
                بازیابی رمز عبور
              </Link>
            </div> */}

          <Button
            type="submit"
            disabled={loginLoading}
          ><FaSignInAlt /> ورود</Button>
        </form>
      </div>
    </div>
  );
}
